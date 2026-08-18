using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class GeminiSetupMessage
{
    [JsonProperty("setup")] public GeminiSetup Setup = new GeminiSetup();
}

public class GeminiSetup
{
    [JsonProperty("model")] public string Model;
    [JsonProperty("generationConfig")] public GeminiGenerationConfig GenerationConfig = new GeminiGenerationConfig();
    [JsonProperty("systemInstruction")] public GeminiSystemInstruction SystemInstruction;
    [JsonProperty("tools")] public GeminiTool[] Tools;
    [JsonProperty("inputAudioTranscription")] public object InputAudioTranscription = new object();
    [JsonProperty("outputAudioTranscription")] public object OutputAudioTranscription = new object();
    [JsonProperty("realtimeInputConfig")] public GeminiRealtimeInputConfig RealtimeInputConfig = new GeminiRealtimeInputConfig();
    [JsonProperty("sessionResumption")] public GeminiSessionResumption SessionResumption = new GeminiSessionResumption();
    [JsonProperty("contextWindowCompression")] public GeminiContextWindowCompression ContextWindowCompression = new GeminiContextWindowCompression();
}

/// <summary>
/// Present with no handle = start a fresh session but send resumption updates.
/// Present with a handle = carry on the session that handle names.
/// </summary>
public class GeminiSessionResumption
{
    [JsonProperty("handle", NullValueHandling = NullValueHandling.Ignore)] public string Handle;
}

/// <summary>
/// What actually removes the ~15-minute audio session cap: with a sliding
/// window the server compresses old turns instead of ending the session.
/// targetTokens/triggerTokens are left to the server's documented defaults.
/// </summary>
public class GeminiContextWindowCompression
{
    [JsonProperty("slidingWindow")] public object SlidingWindow = new object();
}

public class GeminiGenerationConfig
{
    [JsonProperty("responseModalities")] public string[] ResponseModalities = { "AUDIO" };
    [JsonProperty("speechConfig")] public GeminiSpeechConfig SpeechConfig = new GeminiSpeechConfig();
}

public class GeminiSpeechConfig
{
    [JsonProperty("voiceConfig")] public GeminiVoiceConfig VoiceConfig = new GeminiVoiceConfig();
}

public class GeminiVoiceConfig
{
    [JsonProperty("prebuiltVoiceConfig")] public GeminiPrebuiltVoiceConfig PrebuiltVoiceConfig = new GeminiPrebuiltVoiceConfig();
}

public class GeminiPrebuiltVoiceConfig
{
    [JsonProperty("voiceName")] public string VoiceName = "Leda";
}

public class GeminiRealtimeInputConfig
{
    [JsonProperty("automaticActivityDetection")] public GeminiAutoActivityDetection AutomaticActivityDetection = new GeminiAutoActivityDetection();
}

public class GeminiAutoActivityDetection
{
    [JsonProperty("disabled")] public bool Disabled = true;
}

public class GeminiSystemInstruction
{
    [JsonProperty("parts")] public GeminiTextPart[] Parts;
}

public class GeminiTextPart
{
    [JsonProperty("text")] public string Text;
}

public class GeminiTool
{
    [JsonProperty("functionDeclarations")] public GeminiFunctionDeclaration[] FunctionDeclarations;
}

public class GeminiFunctionDeclaration
{
    [JsonProperty("name")] public string Name;
    [JsonProperty("description")] public string Description;
    [JsonProperty("parameters")] public JRaw Parameters;
}

public class GeminiClientContentMessage
{
    [JsonProperty("clientContent")] public GeminiClientContent ClientContent = new GeminiClientContent();
}

public class GeminiClientContent
{
    [JsonProperty("turns")] public GeminiClientTurn[] Turns;
    [JsonProperty("turnComplete")] public bool TurnComplete = true;
}

public class GeminiClientTurn
{
    [JsonProperty("role")] public string Role = "user";
    [JsonProperty("parts")] public GeminiTextPart[] Parts;
}

public class GeminiToolResponseMessage
{
    [JsonProperty("toolResponse")] public GeminiToolResponse ToolResponse = new GeminiToolResponse();
}

public class GeminiToolResponse
{
    [JsonProperty("functionResponses")] public GeminiFunctionResponse[] FunctionResponses;
}

public class GeminiFunctionResponse
{
    [JsonProperty("id")] public string Id;
    [JsonProperty("name")] public string Name;
    [JsonProperty("response")] public JRaw Response;
}

public class GeminiServerMessage
{
    [JsonProperty("setupComplete")] public object SetupComplete;
    [JsonProperty("serverContent")] public GeminiServerContent ServerContent;
    [JsonProperty("toolCall")] public GeminiToolCall ToolCall;
    [JsonProperty("sessionResumptionUpdate")] public GeminiSessionResumptionUpdate SessionResumptionUpdate;
    [JsonProperty("goAway")] public GeminiGoAway GoAway;
}

public class GeminiSessionResumptionUpdate
{
    [JsonProperty("newHandle")] public string NewHandle;
    [JsonProperty("resumable")] public bool Resumable;
}

/// <summary>Warning that the connection is about to be closed as ABORTED.</summary>
public class GeminiGoAway
{
    [JsonProperty("timeLeft")] public string TimeLeft;
}

public class GeminiServerContent
{
    [JsonProperty("modelTurn")] public GeminiModelTurn ModelTurn;
    [JsonProperty("turnComplete")] public bool TurnComplete;
    [JsonProperty("interrupted")] public bool Interrupted;
    [JsonProperty("inputTranscription")] public GeminiTranscription InputTranscription;
    [JsonProperty("outputTranscription")] public GeminiTranscription OutputTranscription;
}

public class GeminiModelTurn
{
    [JsonProperty("parts")] public GeminiPart[] Parts;
}

public class GeminiPart
{
    [JsonProperty("inlineData")] public GeminiInlineData InlineData;
    [JsonProperty("text")] public string Text;
}

public class GeminiInlineData
{
    [JsonProperty("mimeType")] public string MimeType;
    [JsonProperty("data")] public string Data;
}

public class GeminiTranscription
{
    [JsonProperty("text")] public string Text;
}

public class GeminiToolCall
{
    [JsonProperty("functionCalls")] public GeminiFunctionCall[] FunctionCalls;
}

public class GeminiFunctionCall
{
    [JsonProperty("id")] public string Id;
    [JsonProperty("name")] public string Name;
    [JsonProperty("args")] public JObject Args;
}
