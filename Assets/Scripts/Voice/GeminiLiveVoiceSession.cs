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

    // The server delivers a turn's audio faster than it is spoken, so this is
    // sized for a whole turn arriving at once rather than for the ~1s of
    // jitter a real-time stream would need. 30s of 24kHz mono floats is 2.9MB.
    private const int RingBufferSeconds = 30;

    // How long a barge-in keeps discarding incoming audio when the server is
    // still generating the turn that was interrupted. The window normally ends
    // the moment the server acknowledges with serverContent.interrupted; this
    // is only the bound for the case where that acknowledgement never comes.
    private const float BargeInDropSeconds = 1f;

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

    // Barge-in state. Flushing the ring only disposes of audio that has already
    // arrived; the server keeps sending the cancelled turn until it processes
    // the interruption, and that tail would otherwise be played over the answer
    // to what was just said. _serverTurnGenerating says whether there is such a
    // tail to expect at all — without it, a press made while she is silent
    // would throw away the beginning of her next answer.
    private bool _serverTurnGenerating;
    private float _dropIncomingAudioUntil = -1f;

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
        // The socket's close callbacks raise a flag and nothing else, and a
        // close is handled here, once. The installed package happens to post its
        // callbacks to Unity's SynchronizationContext, so they already arrive on
        // the main thread — but that is the package's choice, not a promise, and
        // one owner of "the connection is gone" is worth the flag either way.
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
        CloseSocket(_ws);
    }

    /// <summary>
    /// Opens the next socket, and makes sure it is the ONLY one talking to us.
    /// Both halves of that matter: the package posts its callbacks straight to
    /// Unity's SynchronizationContext, so a socket nobody polls any more still
    /// delivers messages — an abandoned one goes on pouring a second voice into
    /// the same playback ring, and its late OnClose would be read as the CURRENT
    /// connection dropping and start a reconnect that nothing asked for.
    /// </summary>
    private async Task OpenSocket()
    {
        CloseSocket(_ws);

        _closedSignal = false;
        _closedHandled = false;
        _sendQueue.Clear();
        _sending = false;

        // Captured so every callback can check whether it still speaks for the
        // connection this class considers current, and say nothing if it does not.
        var socket = new WebSocket(_url);
        _ws = socket;

        socket.OnOpen += () =>
        {
            if (socket != _ws) return;
            Log("WebSocket open, sending setup...");
            SendSetup();
        };
        socket.OnError += e =>
        {
            if (socket != _ws) return;
            Log("WebSocket error: " + e);
            Faulted?.Invoke(new Exception("WebSocket error: " + e));
            _closedSignal = true;
        };
        socket.OnClose += e =>
        {
            if (socket != _ws) return;
            Log("WebSocket closed: " + e);
            _closedSignal = true;
        };
        socket.OnMessage += bytes =>
        {
            if (socket != _ws) return;
            HandleMessage(bytes);
        };

        await socket.Connect();
    }

    /// <summary>
    /// Closing is not enough on its own: a socket still opening ignores Close()
    /// (it is not Open yet), and only cancelling its token stops it.
    /// </summary>
    private static void CloseSocket(WebSocket socket)
    {
        if (socket == null) return;
        if (socket.State == WebSocketState.Open) _ = socket.Close();
        else socket.CancelConnection();
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

        // Nothing more can arrive on a socket that is gone, so a barge-in has
        // nothing left to wait for. What is already buffered is still hers and
        // is left to play out — a rotated connection is meant to be inaudible.
        _serverTurnGenerating = false;
        _dropIncomingAudioUntil = -1f;

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

        BeginBargeIn();

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

        BeginBargeIn();

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
        // Read before the audio in the same message, never after: everything
        // buffered belongs to an answer the server has just abandoned, and
        // flushing afterwards would discard its replacement instead.
        if (content.Interrupted)
        {
            Log("Model turn interrupted (server ack).");
            _serverTurnGenerating = false;
            _dropIncomingAudioUntil = -1f;
            FlushPlayback();
            // An abandoned turn never gets its turnComplete, so nothing else
            // would move her out of Speaking. Listening is left alone: a
            // push-to-talk barge-in has already said what the state is.
            if (_turnState == TurnState.Speaking) SetTurnState(TurnState.Idle);
        }

        if (content.InputTranscription != null && !string.IsNullOrEmpty(content.InputTranscription.Text))
            CaptionReceived?.Invoke(new CaptionEvent(true, content.InputTranscription.Text));

        if (content.OutputTranscription != null && !string.IsNullOrEmpty(content.OutputTranscription.Text))
            CaptionReceived?.Invoke(new CaptionEvent(false, content.OutputTranscription.Text));

        if (content.ModelTurn?.Parts != null)
        {
            bool discard = DiscardingInterruptedAudio();
            bool hasAudio = false;
            foreach (var part in content.ModelTurn.Parts)
            {
                if (part.InlineData == null || string.IsNullOrEmpty(part.InlineData.Data)) continue;
                hasAudio = true;
                if (discard) continue;
                EnqueuePlayback(Convert.FromBase64String(part.InlineData.Data));
            }
            if (hasAudio)
            {
                // True whether the audio was kept or dropped — either way the
                // server has a turn in hand, which is what the next barge-in
                // needs to know.
                _serverTurnGenerating = true;
                if (!discard) SetTurnState(TurnState.Speaking);
            }
        }

        if (content.TurnComplete)
        {
            _serverTurnGenerating = false;
            _dropIncomingAudioUntil = -1f;
            SetTurnState(TurnState.Idle);
        }
    }

    private void SetTurnState(TurnState state)
    {
        if (_turnState == state) return;
        _turnState = state;
        TurnStateChanged?.Invoke(state);
    }

    /// <summary>
    /// A full ring drops what has just arrived and keeps what is already
    /// playing. The other way round — advancing the read cursor to make room —
    /// leaves the two cursors adjacent, so every DSP block lands somewhere else
    /// in the utterance and a sentence is heard shredded into another one.
    /// Losing the tail of a turn this long is audible too, hence the warning:
    /// if it ever appears, the buffer is the number to revisit.
    /// </summary>
    private void EnqueuePlayback(byte[] pcm16)
    {
        int sampleCount = pcm16.Length / 2;
        int dropped = 0;

        lock (_ringLock)
        {
            for (int i = 0; i < sampleCount; i++)
            {
                if (_ringFilled >= _ringBuffer.Length)
                {
                    dropped = sampleCount - i;
                    break;
                }
                short s = (short)(pcm16[i * 2] | (pcm16[i * 2 + 1] << 8));
                _ringBuffer[_ringWrite] = s / 32768f;
                _ringWrite = (_ringWrite + 1) % _ringBuffer.Length;
                _ringFilled++;
            }
        }

        if (dropped > 0)
            Log("Playback buffer full (" + RingBufferSeconds + "s): dropped " + dropped + " samples.");
    }

    /// <summary>
    /// What a press of the button, or a line of text, does to audio that is
    /// already on its way. The flush is synchronous — barge-in never waits on a
    /// round trip — and the drop window covers the gap between that flush and
    /// the server hearing about the interruption.
    /// </summary>
    private void BeginBargeIn()
    {
        FlushPlayback();

        if (!_serverTurnGenerating) return;
        _dropIncomingAudioUntil = Time.unscaledTime + BargeInDropSeconds;
        Log("Barge-in: discarding the interrupted turn's audio until the server acks.");
    }

    /// <summary>
    /// True while a barge-in is still waiting for the server to stop sending the
    /// turn it was asked to abandon. Decided here, at the one place that reads
    /// it, rather than counted down in Update: a frame that receives no audio
    /// has nothing to decide.
    /// </summary>
    private bool DiscardingInterruptedAudio()
    {
        if (_dropIncomingAudioUntil < 0f) return false;
        if (Time.unscaledTime < _dropIncomingAudioUntil) return true;
        _dropIncomingAudioUntil = -1f;
        return false;
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
