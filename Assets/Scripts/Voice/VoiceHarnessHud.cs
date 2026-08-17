using System.Text;
using UnityEngine;

[RequireComponent(typeof(GeminiLiveVoiceSession))]
public class VoiceHarnessHud : MonoBehaviour
{
    private const string SystemInstruction =
        "You are a friendly, encouraging study companion. When the user tells you how many " +
        "minutes they plan to study, call the set_study_minutes function with that number of " +
        "minutes, then briefly confirm out loud. Keep spoken replies short.";

    private const string SetStudyMinutesSchema =
        "{\"type\":\"OBJECT\",\"properties\":{\"minutes\":{\"type\":\"INTEGER\",\"description\":" +
        "\"Number of minutes the user wants to study.\"}},\"required\":[\"minutes\"]}";

    private GeminiLiveVoiceSession _session;
    private readonly StringBuilder _log = new StringBuilder();
    private bool _connected;
    private bool _connecting;
    private bool _talking;

    private void Awake()
    {
        _session = GetComponent<GeminiLiveVoiceSession>();
        _session.OnLog += AppendLog;
        _session.CaptionReceived += c => AppendLog((c.IsUser ? "You: " : "Character: ") + c.Text);
        _session.ToolCallReceived += HandleToolCall;
        _session.TurnStateChanged += s => AppendLog("[state] " + s);
        _session.OnConnected += () =>
        {
            _connected = true;
            _connecting = false;
        };
        _session.OnDisconnected += () =>
        {
            _connected = false;
            _connecting = false;
            _talking = false;
        };
    }

    private void HandleToolCall(ToolCallEvent e)
    {
        AppendLog($"ToolCall: {e.FunctionName}({e.ArgsJson}) id={e.CallId}");
        _session.RespondToToolCall(e.CallId, e.FunctionName, "{\"result\":\"ok\"}");
    }

    private void AppendLog(string line)
    {
        _log.AppendLine(line);
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(20, 20, 520, 420));

        if (!_connected)
        {
            GUI.enabled = !_connecting;
            if (GUILayout.Button(_connecting ? "Connecting..." : "Connect", GUILayout.Height(40)))
            {
                _connecting = true;
                var sessionConfig = new VoiceSessionConfig
                {
                    SystemInstruction = SystemInstruction,
                    Tools =
                    {
                        new VoiceToolDeclaration
                        {
                            Name = "set_study_minutes",
                            Description = "Records how many minutes the user wants to study in this session.",
                            ParametersJsonSchema = SetStudyMinutesSchema
                        }
                    }
                };
                _ = _session.Connect(sessionConfig);
            }
            GUI.enabled = true;
        }
        else
        {
            string label = _talking ? "Listening... (click to stop)" : "Click to talk";
            if (GUILayout.Button(label, GUILayout.Height(60)))
            {
                _talking = !_talking;
                if (_talking) _session.BeginPushToTalk();
                else _session.EndPushToTalk();
            }
        }

        GUILayout.Space(10);
        GUILayout.Label(_log.ToString());
        GUILayout.EndArea();
    }
}
