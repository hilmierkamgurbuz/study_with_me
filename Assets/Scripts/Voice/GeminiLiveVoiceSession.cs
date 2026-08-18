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
    private string _url;
    private bool _setupComplete;

    // Reconnect state. A dropped socket is not the end of a conversation: the
    // server hands out a resumption handle, and reopening with it continues the
    // same session, which is what removes the session-length ceiling.
    private string _resumptionHandle;
    private volatile bool _closedSignal;
    private bool _closedHandled;
    private bool _intentionalClose;
    private bool _everConnected;
    private int _reconnectAttempt;
    private float _reconnectAt = -1f;

    /// <summary>
    /// True when the connection that just completed setup carried a resumption
    /// handle — i.e. this is the same conversation, not a new one. Callers use
    /// it to avoid greeting the user twice.
    /// </summary>
    public bool ResumedFromHandle { get; private set; }

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
        // The socket's close callback can land off the main thread, so it only
        // raises a flag and everything that touches Unity happens here.
        if (_closedSignal && !_closedHandled)
        {
            _closedHandled = true;
            HandleSocketClosed();
        }

        if (_reconnectAt >= 0f && Time.unscaledTime >= _reconnectAt)
        {
            _reconnectAt = -1f;
            Log("Reconnecting" + (string.IsNullOrEmpty(_resumptionHandle) ? "..." : " with resumption handle..."));
            _ = OpenSocket();
        }

        if (_isListening)
        {
            PumpMicAudio();
        }
    }

    private void OnDestroy()
    {
        Disconnect();
    }

    public async Task Connect(VoiceSessionConfig sessionConfig)
    {
        _sessionConfig = sessionConfig;
        _intentionalClose = false;
        _resumptionHandle = null;
        _everConnected = false;
        _reconnectAttempt = 0;

        if (config == null || string.IsNullOrEmpty(config.apiKey))
        {
            Log("No GeminiApiConfig / apiKey assigned.");
            return;
        }

        _url = "wss://generativelanguage.googleapis.com/ws/google.ai.generativelanguage.v1beta.GenerativeService.BidiGenerateContent?key=" + config.apiKey;
        await OpenSocket();
    }

    public void Disconnect()
    {
        _intentionalClose = true;
        _reconnectAt = -1f;
        if (_isListening) EndPushToTalk();
        _ws?.Close();
    }

    private async Task OpenSocket()
    {
        _closedSignal = false;
        _closedHandled = false;
        _sendQueue.Clear();
        _sending = false;

        _ws = new WebSocket(_url);

        _ws.OnOpen += () =>
        {
            Log("WebSocket open, sending setup...");
            SendSetup();
        };
        _ws.OnError += e =>
        {
            Log("WebSocket error: " + e);
            Faulted?.Invoke(new Exception("WebSocket error: " + e));
            _closedSignal = true;
        };
        _ws.OnClose += e =>
        {
            Log("WebSocket closed: " + e);
            _closedSignal = true;
        };
        _ws.OnMessage += HandleMessage;

        await _ws.Connect();
    }

    /// <summary>
    /// Runs on the main thread, once per close. Reopens the socket unless the
    /// close was asked for — or unless the very first connection never
    /// succeeded, since retrying a bad key or a retired model forever is a loop
    /// with no way out of it.
    /// </summary>
    private void HandleSocketClosed()
    {
        _setupComplete = false;

        if (_isListening)
        {
            _isListening = false;
            Microphone.End(_micDevice);
        }
        SetTurnState(TurnState.Idle);
        OnDisconnected?.Invoke();

        if (_intentionalClose || !_everConnected) return;

        _reconnectAttempt++;
        float max = _sessionConfig != null ? _sessionConfig.ReconnectBackoffMaxSeconds : 0f;
        float delay = _reconnectAttempt <= 1 ? 0f : Mathf.Pow(2f, _reconnectAttempt - 2);
        _reconnectAt = Time.unscaledTime + (max > 0f ? Mathf.Min(delay, max) : delay);
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

        ResumedFromHandle = !string.IsNullOrEmpty(_resumptionHandle);
        setup.Setup.SessionResumption.Handle = _resumptionHandle;

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
            _everConnected = true;
            _reconnectAttempt = 0;
            Log("Setup complete." + (ResumedFromHandle ? " (resumed)" : string.Empty));
            OnConnected?.Invoke();
            return;
        }

        if (msg.SessionResumptionUpdate != null)
        {
            // Keep the newest handle only; it is what the next reconnect resumes
            // from. resumable=false means there is nothing to keep yet.
            if (msg.SessionResumptionUpdate.Resumable && !string.IsNullOrEmpty(msg.SessionResumptionUpdate.NewHandle))
                _resumptionHandle = msg.SessionResumptionUpdate.NewHandle;
        }

        if (msg.GoAway != null)
        {
            // Not an error: the server is rotating the connection. The close
            // that follows is reconnected like any other.
            Log("Server goAway, timeLeft=" + msg.GoAway.TimeLeft);
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
