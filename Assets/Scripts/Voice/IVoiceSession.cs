using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IVoiceSession
{
    Task Connect(VoiceSessionConfig sessionConfig);

    /// <summary>
    /// Deliberate shutdown. A socket that drops on its own is reconnected
    /// automatically; this is how the caller says not to.
    /// </summary>
    void Disconnect();

    void SetMicPolicy(MicPolicy policy);
    void BeginPushToTalk();
    void EndPushToTalk();
    void SendText(string message);
    void RespondToToolCall(string callId, string functionName, string responseJson);

    event Action<TurnState> TurnStateChanged;
    event Action<CaptionEvent> CaptionReceived;
    event Action<ToolCallEvent> ToolCallReceived;
    event Action<Exception> Faulted;
}

public enum MicPolicy
{
    Disabled,
    PushToTalk,
    AutoOpen
}

public enum TurnState
{
    Idle,
    Listening,
    Thinking,
    Speaking
}

public class VoiceSessionConfig
{
    public string SystemInstruction;
    public List<VoiceToolDeclaration> Tools = new List<VoiceToolDeclaration>();

    /// <summary>
    /// Ceiling on the exponential backoff between reconnect attempts, in
    /// seconds. Supplied by the caller so the transport reads no config asset
    /// of its own.
    /// </summary>
    public float ReconnectBackoffMaxSeconds;
}

public class VoiceToolDeclaration
{
    public string Name;
    public string Description;
    public string ParametersJsonSchema;
}

public readonly struct CaptionEvent
{
    public readonly bool IsUser;
    public readonly string Text;

    public CaptionEvent(bool isUser, string text)
    {
        IsUser = isUser;
        Text = text;
    }
}

public readonly struct ToolCallEvent
{
    public readonly string CallId;
    public readonly string FunctionName;
    public readonly string ArgsJson;

    public ToolCallEvent(string callId, string functionName, string argsJson)
    {
        CallId = callId;
        FunctionName = functionName;
        ArgsJson = argsJson;
    }
}
