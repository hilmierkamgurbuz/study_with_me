using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NativeWebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class GeminiLiveVoiceSession : MonoBehaviour, IVoiceSession
{
    private const string ActivityStartJson = "{\"realtimeInput\":{\"activityStart\":{}}}";
    private const string ActivityEndJson = "{\"realtimeInput\":{\"activityEnd\":{}}}";

    private const int InputSampleRate = 16000;
    private const int OutputSampleRate = 24000;
    private const int RingBufferSeconds = 8;

    public GeminiApiConfig config;

    public event Action<string> OnLog;
    public event Action OnConnected;
    public event Action OnDisconnected;

    public event Action<TurnState> TurnStateChanged;
    public event Action<CaptionEvent> CaptionReceived;
    public event Action<ToolCallEvent> ToolCallReceived;
    public event Action<Exception> Faulted;

    private VoiceSessionConfig _sessionConfig;
    private MicPolicy _micPolicy = MicPolicy.PushToTalk;
    private TurnState _turnState = TurnState.Idle;

    private WebSocket _ws;
    private bool _setupComplete;
    private bool _isListening;
    private string _micDevice;
    private AudioClip _micClip;
    private int _micReadPos;

    private AudioSource _playbackSource;
    private readonly object _ringLock = new object();
    private float[] _ringBuffer;
    private int _ringWrite;
    private int _ringRead;
    private int _ringFilled;

    private readonly Queue<string> _sendQueue = new Queue<string>();
    private bool _sending;

    private void Awake()
    {
        _ringBuffer = new float[OutputSampleRate * RingBufferSeconds];

        _playbackSource = gameObject.AddComponent<AudioSource>();
        var playbackClip = AudioClip.Create("GeminiPlayback", _ringBuffer.Length, 1, OutputSampleRate, true, ReadPlaybackSamples);
        _playbackSource.clip = playbackClip;
        _playbackSource.loop = true;
        _playbackSource.Play();
    }

    private void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        _ws?.DispatchMessageQueue();
#endif
        if (_isListening)
        {
            PumpMicAudio();
        }
    }

    private void OnDestroy()
    {
        if (_isListening) EndPushToTalk();
        _ws?.Close();
    }

    public async Task Connect(VoiceSessionConfig sessionConfig)
    {
        _sessionConfig = sessionConfig;

        if (config == null || string.IsNullOrEmpty(config.apiKey))
        {
            Log("No GeminiApiConfig / apiKey assigned.");
            return;
        }

        string url = "wss://generativelanguage.googleapis.com/ws/google.ai.generativelanguage.v1beta.GenerativeService.BidiGenerateContent?key=" + config.apiKey;
        _ws = new WebSocket(url);

        _ws.OnOpen += () =>
        {
            Log("WebSocket open, sending setup...");
            SendSetup();
        };
        _ws.OnError += e =>
        {
            Log("WebSocket error: " + e);
            _setupComplete = false;
            OnDisconnected?.Invoke();
            Faulted?.Invoke(new Exception("WebSocket error: " + e));
        };
        _ws.OnClose += e =>
        {
            Log("WebSocket closed: " + e);
            _setupComplete = false;
            OnDisconnected?.Invoke();
        };
        _ws.OnMessage += HandleMessage;

        await _ws.Connect();
    }

    public void SetMicPolicy(MicPolicy policy)
    {
        _micPolicy = policy;
    }

    public void BeginPushToTalk()
    {
        if (_micPolicy == MicPolicy.Disabled)
        {
            Log("Mic is disabled by current policy.");
            return;
        }
        if (_ws == null || _ws.State != WebSocketState.Open || !_setupComplete)
        {
            Log("Cannot start listening: not connected/setup.");
            return;
        }
        if (_isListening) return;

        FlushPlayback();

        _micDevice = null;
        _micClip = Microphone.Start(_micDevice, true, 1, InputSampleRate);
        _micReadPos = 0;
        _isListening = true;
        SendRaw(ActivityStartJson);
        SetTurnState(TurnState.Listening);
        Log("Listening...");
    }

    public void EndPushToTalk()
    {
        if (!_isListening) return;
        _isListening = false;
        Microphone.End(_micDevice);
        SendRaw(ActivityEndJson);
        SetTurnState(TurnState.Thinking);
        Log("Stopped listening.");
    }

    public void SendText(string message)
    {
        if (_ws == null || _ws.State != WebSocketState.Open) return;

        FlushPlayback();

        var msg = new GeminiClientContentMessage
        {
            ClientContent = new GeminiClientContent
            {
                Turns = new[] { new GeminiClientTurn { Parts = new[] { new GeminiTextPart { Text = message } } } }
            }
        };
        SendRaw(JsonConvert.SerializeObject(msg));
        SetTurnState(TurnState.Thinking);
    }

    public void RespondToToolCall(string callId, string functionName, string responseJson)
    {
        var msg = new GeminiToolResponseMessage
        {
            ToolResponse = new GeminiToolResponse
            {
                FunctionResponses = new[]
                {
                    new GeminiFunctionResponse { Id = callId, Name = functionName, Response = new JRaw(responseJson) }
                }
            }
        };
        SendRaw(JsonConvert.SerializeObject(msg));
    }

    private void SendSetup()
    {
        var setup = new GeminiSetupMessage
        {
            Setup = new GeminiSetup { Model = config.liveModelId }
        };

        if (!string.IsNullOrEmpty(_sessionConfig?.SystemInstruction))
        {
            setup.Setup.SystemInstruction = new GeminiSystemInstruction
            {
                Parts = new[] { new GeminiTextPart { Text = _sessionConfig.SystemInstruction } }
            };
        }

        if (_sessionConfig?.Tools != null && _sessionConfig.Tools.Count > 0)
        {
            var declarations = new GeminiFunctionDeclaration[_sessionConfig.Tools.Count];
            for (int i = 0; i < _sessionConfig.Tools.Count; i++)
            {
                var t = _sessionConfig.Tools[i];
                declarations[i] = new GeminiFunctionDeclaration
                {
                    Name = t.Name,
                    Description = t.Description,
                    Parameters = new JRaw(t.ParametersJsonSchema)
                };
            }
            setup.Setup.Tools = new[] { new GeminiTool { FunctionDeclarations = declarations } };
        }

        SendRaw(JsonConvert.SerializeObject(setup));
    }

    private void PumpMicAudio()
    {
        int clipSamples = _micClip.samples;
        int pos = Microphone.GetPosition(_micDevice);
        if (pos < 0) return; // device still initializing right after Microphone.Start()
        if (pos == _micReadPos) return;

        int available = pos - _micReadPos;
        if (available < 0) available += clipSamples;

        var samples = new float[available];
        if (_micReadPos + available <= clipSamples)
        {
            _micClip.GetData(samples, _micReadPos);
        }
        else
        {
            int firstPart = clipSamples - _micReadPos;
            var head = new float[firstPart];
            _micClip.GetData(head, _micReadPos);
            Array.Copy(head, samples, firstPart);

            var tail = new float[available - firstPart];
            _micClip.GetData(tail, 0);
            Array.Copy(tail, 0, samples, firstPart, tail.Length);
        }
        _micReadPos = pos;

        SendAudioChunk(samples);
    }

    private void SendAudioChunk(float[] samples)
    {
        var pcm = new byte[samples.Length * 2];
        for (int i = 0; i < samples.Length; i++)
        {
            short s = (short)Mathf.Clamp(Mathf.RoundToInt(samples[i] * short.MaxValue), short.MinValue, short.MaxValue);
            pcm[i * 2] = (byte)(s & 0xFF);
            pcm[i * 2 + 1] = (byte)((s >> 8) & 0xFF);
        }

        string base64 = Convert.ToBase64String(pcm);
        // base64 has no JSON-special characters, so direct interpolation is safe here.
        string json = "{\"realtimeInput\":{\"audio\":{\"mimeType\":\"audio/pcm;rate=16000\",\"data\":\"" + base64 + "\"}}}";
        SendRaw(json);
    }

    private void HandleMessage(byte[] bytes)
    {
        string json = Encoding.UTF8.GetString(bytes);

        GeminiServerMessage msg;
        try
        {
            msg = JsonConvert.DeserializeObject<GeminiServerMessage>(json);
        }
        catch (Exception e)
        {
            Log("Failed to parse server message: " + e.Message);
            return;
        }

        if (msg == null) return;

        if (msg.SetupComplete != null)
        {
            _setupComplete = true;
            Log("Setup complete.");
            OnConnected?.Invoke();
            return;
        }

        if (msg.ToolCall?.FunctionCalls != null)
        {
            foreach (var fc in msg.ToolCall.FunctionCalls)
            {
                ToolCallReceived?.Invoke(new ToolCallEvent(fc.Id, fc.Name, fc.Args?.ToString() ?? "{}"));
            }
        }

        if (msg.ServerContent != null)
        {
            HandleServerContent(msg.ServerContent);
        }
    }

    private void HandleServerContent(GeminiServerContent content)
    {
        if (content.InputTranscription != null && !string.IsNullOrEmpty(content.InputTranscription.Text))
            CaptionReceived?.Invoke(new CaptionEvent(true, content.InputTranscription.Text));

        if (content.OutputTranscription != null && !string.IsNullOrEmpty(content.OutputTranscription.Text))
            CaptionReceived?.Invoke(new CaptionEvent(false, content.OutputTranscription.Text));

        if (content.ModelTurn?.Parts != null)
        {
            bool hasContent = false;
            foreach (var part in content.ModelTurn.Parts)
            {
                if (part.InlineData != null && !string.IsNullOrEmpty(part.InlineData.Data))
                {
                    byte[] pcm = Convert.FromBase64String(part.InlineData.Data);
                    EnqueuePlayback(pcm);
                    hasContent = true;
                }
            }
            if (hasContent) SetTurnState(TurnState.Speaking);
        }

        if (content.TurnComplete)
        {
            SetTurnState(TurnState.Idle);
        }

        if (content.Interrupted)
        {
            Log("Model turn interrupted (server ack).");
        }
    }

    private void SetTurnState(TurnState state)
    {
        if (_turnState == state) return;
        _turnState = state;
        TurnStateChanged?.Invoke(state);
    }

    private void EnqueuePlayback(byte[] pcm16)
    {
        int sampleCount = pcm16.Length / 2;
        lock (_ringLock)
        {
            for (int i = 0; i < sampleCount; i++)
            {
                short s = (short)(pcm16[i * 2] | (pcm16[i * 2 + 1] << 8));
                _ringBuffer[_ringWrite] = s / 32768f;
                _ringWrite = (_ringWrite + 1) % _ringBuffer.Length;
                if (_ringFilled < _ringBuffer.Length) _ringFilled++;
                else _ringRead = (_ringRead + 1) % _ringBuffer.Length;
            }
        }
    }

    private void ReadPlaybackSamples(float[] data)
    {
        lock (_ringLock)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (_ringFilled <= 0)
                {
                    data[i] = 0f;
                    continue;
                }
                data[i] = _ringBuffer[_ringRead];
                _ringRead = (_ringRead + 1) % _ringBuffer.Length;
                _ringFilled--;
            }
        }
    }

    private void FlushPlayback()
    {
        lock (_ringLock)
        {
            _ringRead = _ringWrite;
            _ringFilled = 0;
        }
    }

    private void SendRaw(string json)
    {
        if (_ws == null || _ws.State != WebSocketState.Open) return;
        _sendQueue.Enqueue(json);
        if (!_sending) _ = DrainSendQueue();
    }

    private async Task DrainSendQueue()
    {
        _sending = true;
        while (_sendQueue.Count > 0)
        {
            if (_ws == null || _ws.State != WebSocketState.Open)
            {
                _sendQueue.Clear();
                break;
            }
            string next = _sendQueue.Dequeue();
            await _ws.SendText(next);
        }
        _sending = false;
    }

    private void Log(string message)
    {
        Debug.Log("[GeminiLive] " + message);
        OnLog?.Invoke(message);
    }
}
