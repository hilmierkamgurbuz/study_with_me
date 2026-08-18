using System;
using System.Globalization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

/// <summary>
/// Room.unity's composition root. It assembles the system instruction out of
/// the persona asset and the saved profile, declares the tools the conversation
/// steers everything else with, and is the one place a tool call turns into a
/// study block, a dance, a game or a line of memory.
/// </summary>
public class RoomSessionController : MonoBehaviour
{
    // Placeholders the persona asset's text is written against.
    private const string ProfileToken = "{profile}";
    private const string ActivityToken = "{activity}";
    private const string PomodoroStudyToken = "{pomodoroStudy}";
    private const string PomodoroBreakToken = "{pomodoroBreak}";

    // Tool and argument names. These are protocol, not content: what each one
    // MEANS and when to reach for it is described in the persona asset.
    private const string ToolUpdateProfile = "update_student_profile";
    private const string ToolSetInterest = "set_activity_interest";
    private const string ToolRememberEvent = "remember_event";
    private const string ToolStartStudy = "start_study_block";
    private const string ToolEndStudy = "end_study_block";
    private const string ToolStartActivity = "start_activity";
    private const string ToolStopActivity = "stop_activity";
    private const string ToolEndSession = "end_session";

    private const string ArgActivity = "activity";
    private const string ArgInterest = "interest";
    private const string ArgSummary = "summary";
    private const string ArgStudyMinutes = "studyMinutes";
    private const string ArgBreakMinutes = "breakMinutes";

    private const string ActivityDance = "dance";
    private const string ActivityGame = "game";
    private const string InterestNotNow = "not_now";

    private const string ResponseOk = "{\"result\":\"ok\"}";

    private const string UpdateProfileSchema =
        "{\"type\":\"OBJECT\",\"properties\":{" +
        "\"studentName\":{\"type\":\"STRING\"}," +
        "\"gradeOrClass\":{\"type\":\"STRING\"}}}";

    private const string SetInterestSchema =
        "{\"type\":\"OBJECT\",\"properties\":{" +
        "\"activity\":{\"type\":\"STRING\",\"enum\":[\"dance\",\"game\"]}," +
        "\"interest\":{\"type\":\"STRING\",\"enum\":[\"likes\",\"dislikes\",\"not_now\"]}}," +
        "\"required\":[\"activity\",\"interest\"]}";

    private const string RememberEventSchema =
        "{\"type\":\"OBJECT\",\"properties\":{\"summary\":{\"type\":\"STRING\"}},\"required\":[\"summary\"]}";

    private const string StartStudySchema =
        "{\"type\":\"OBJECT\",\"properties\":{" +
        "\"studyMinutes\":{\"type\":\"INTEGER\"}," +
        "\"breakMinutes\":{\"type\":\"INTEGER\"}}," +
        "\"required\":[\"studyMinutes\",\"breakMinutes\"]}";

    private const string StartActivitySchema =
        "{\"type\":\"OBJECT\",\"properties\":{" +
        "\"activity\":{\"type\":\"STRING\",\"enum\":[\"dance\",\"game\"]}}," +
        "\"required\":[\"activity\"]}";

    private const string EndSessionSchema =
        "{\"type\":\"OBJECT\",\"properties\":{\"summary\":{\"type\":\"STRING\"}},\"required\":[\"summary\"]}";

    private const string NoArgsSchema = "{\"type\":\"OBJECT\",\"properties\":{}}";

    public GeminiLiveVoiceSession session;
    public CharacterPresenter character;
    public PushToTalkButtonView talkButton;
    public ChloePersonaConfig persona;
    public DeskRoutine studyMode;
    public DanceModeController danceMode;
    public GameModeController gameMode;

    private readonly IProfileRepository _profiles = new LocalJsonProfileRepository();
    private StudentProfile _profile;
    private StudyBlockRunner _clock;
    private BreakOfferPolicy _offers;

    private TurnState _turnState = TurnState.Idle;
    private bool _started;
    private bool _connected;
    private bool _connecting;
    private bool _talking;
    private bool _activityWasRunning;
    private bool _gameQueued;
    private string _runningActivityLabel;

    private void Awake()
    {
        _profile = _profiles.Load() ?? new StudentProfile();
        ProfileMerger.Migrate(_profile);

        _clock = new StudyBlockRunner();

        if (persona == null)
        {
            Debug.LogError("[RoomSession] No ChloePersonaConfig assigned — run Tools > StudyWithMe > Set Up Session AI.");
        }
        else
        {
            ProfileMerger.PruneEvents(_profile, persona.memoryDays, DateTime.UtcNow);
            _offers = new BreakOfferPolicy(persona.offerEveryNthBreak, persona.declineLimit);
            _offers.SetInterest(BreakActivity.Dance, InterestFrom(_profile.likesDancing));
            _offers.SetInterest(BreakActivity.Game, InterestFrom(_profile.likesGames));
        }

        // Nothing about the conversation is drawn on screen any more. Captions go
        // to the console so they are still there in `adb logcat -s Unity` when
        // something needs diagnosing; GeminiLiveVoiceSession already logs its own.
        session.CaptionReceived += c => Debug.Log((c.IsUser ? "[Sen] " : "[Chloe] ") + c.Text);
        session.ToolCallReceived += HandleToolCall;
        session.TurnStateChanged += HandleTurnStateChanged;
        session.OnConnected += HandleConnected;
        session.OnDisconnected += HandleDisconnected;

        if (talkButton != null) talkButton.Pressed += HandleTalkButtonPressed;
    }

    private void OnDestroy()
    {
        if (talkButton != null) talkButton.Pressed -= HandleTalkButtonPressed;
    }

    /// <summary>
    /// The whole interface, in one press: open the session the first time, then
    /// open and close the mic. A press while the socket is down does nothing on
    /// purpose — reconnecting is the transport's own job, and calling Connect()
    /// again there would throw away the resumption handle and start a second
    /// conversation.
    /// </summary>
    private void HandleTalkButtonPressed()
    {
        if (!_started)
        {
            Connect();
            return;
        }

        if (!_connected) return;

        _talking = !_talking;
        if (_talking) session.BeginPushToTalk();
        else session.EndPushToTalk();
    }

    private PttButtonState ButtonState()
    {
        if (_connecting) return PttButtonState.Connecting;
        if (!_connected) return _started ? PttButtonState.Connecting : PttButtonState.Offline;
        return _talking ? PttButtonState.Listening : PttButtonState.Ready;
    }

    private void Update()
    {
        // Painted before the persona guard: a missing persona asset is a setup
        // mistake worth seeing on a live button rather than a dead screen.
        if (talkButton != null) talkButton.SetState(ButtonState());

        if (persona == null) return;

        bool activityRunning = (danceMode != null && danceMode.IsRunning) || (gameMode != null && gameMode.IsRunning);

        // A game asked for during a dance waits for the dance to finish unwinding
        // — lights back on, her back at the desk — and only then takes the screen.
        if (_gameQueued && (danceMode == null || !danceMode.IsRunning))
        {
            _gameQueued = false;
            if (gameMode != null) gameMode.StartGameMode();
        }

        // A dance runs out its own two minutes and a game lasts as long as the
        // player wants, so what ends a break is the activity ENDING, not a clock.
        if (activityRunning)
        {
            // Named on the rising edge, so a mode started from its debug button
            // is described as accurately as one the conversation asked for.
            if (!_activityWasRunning)
                _runningActivityLabel = danceMode != null && danceMode.IsRunning ? persona.danceLabel : persona.gameLabel;
            _activityWasRunning = true;
        }
        else if (_activityWasRunning)
        {
            _activityWasRunning = false;
            if (_clock.Phase == StudyPhase.Break)
                SendNudge(Fill(persona.activityOverNudge, ActivityToken, _runningActivityLabel));
        }

        bool studying = studyMode != null && studyMode.IsRunning;
        bool voiceBusy = _turnState != TurnState.Idle;

        switch (_clock.Tick(Time.deltaTime, studying, activityRunning, voiceBusy))
        {
            case StudyClockEvent.StudyBlockEnded:
                HandleStudyBlockEnded();
                break;
            case StudyClockEvent.BreakElapsed:
                SendNudge(persona.breakOverNudge);
                break;
        }
    }

    private void OnApplicationQuit()
    {
        StampAndSave();
    }

    public void Connect()
    {
        if (persona == null) return;

        _started = true;
        _connecting = true;
        var sessionConfig = new VoiceSessionConfig
        {
            SystemInstruction = BuildSystemInstruction(),
            ReconnectBackoffMaxSeconds = persona.reconnectBackoffMaxSeconds,
            Tools =
            {
                Tool(ToolUpdateProfile, "Store the student's name and school grade.", UpdateProfileSchema),
                Tool(ToolSetInterest, "Record how the student feels about dancing or playing: likes, dislikes, or not right now.", SetInterestSchema),
                Tool(ToolRememberEvent, "Remember one notable thing about the student's life, in a single short sentence.", RememberEventSchema),
                Tool(ToolStartStudy, "Begin a study block of the agreed length, followed by a break of the agreed length.", StartStudySchema),
                Tool(ToolEndStudy, "Stop the current study block.", NoArgsSchema),
                Tool(ToolStartActivity, "Start dancing together, or start the game.", StartActivitySchema),
                Tool(ToolStopActivity, "Stop the dance or the game that is running and come back to the room.", NoArgsSchema),
                Tool(ToolEndSession, "End the session with a one-sentence summary of it.", EndSessionSchema)
            }
        };
        _ = session.Connect(sessionConfig);
    }

    private static VoiceToolDeclaration Tool(string name, string description, string schema)
    {
        return new VoiceToolDeclaration { Name = name, Description = description, ParametersJsonSchema = schema };
    }

    private string BuildSystemInstruction()
    {
        var sb = new StringBuilder();
        AppendBlock(sb, persona.persona);
        AppendBlock(sb, persona.safetyRules);
        AppendBlock(sb, persona.conversationRules);
        AppendBlock(sb, persona.toolRules
            .Replace(PomodoroStudyToken, persona.pomodoroStudyMinutes.ToString(CultureInfo.InvariantCulture))
            .Replace(PomodoroBreakToken, persona.pomodoroBreakMinutes.ToString(CultureInfo.InvariantCulture)));

        // Not knowing their name is what "we have not met" means here — and it
        // is the whole reason the first conversation must not mention studying.
        bool firstMeeting = string.IsNullOrEmpty(_profile.studentName);
        AppendBlock(sb, firstMeeting
            ? persona.firstMeeting
            : persona.returning.Replace(ProfileToken, JsonConvert.SerializeObject(_profile)));

        return sb.ToString();
    }

    private static void AppendBlock(StringBuilder sb, string block)
    {
        if (string.IsNullOrEmpty(block)) return;
        if (sb.Length > 0) sb.Append("\n\n");
        sb.Append(block);
    }

    private void HandleConnected()
    {
        _connected = true;
        _connecting = false;

        // A resumed connection is the same conversation carrying on; greeting
        // again there would read as her forgetting the last five minutes.
        if (!session.ResumedFromHandle) SendNudge(persona.openingNudge);
    }

    private void HandleDisconnected()
    {
        _connected = false;
        _connecting = false;
        _talking = false;
        StampAndSave();
    }

    private void HandleTurnStateChanged(TurnState state)
    {
        _turnState = state;
        character.SetTurnState(state);
    }

    private void HandleStudyBlockEnded()
    {
        if (studyMode != null && studyMode.IsRunning) studyMode.StopStudying();

        BreakOffer offer = _offers.NextOffer();
        if (!offer.HasOffer)
        {
            SendNudge(persona.studyBlockEndedNudge);
            return;
        }

        string label = LabelFor(offer.Activity);
        string template = offer.AskInterestFirst ? persona.askActivityInterestNudge : persona.offerActivityNudge;
        SendNudge(Fill(template, ActivityToken, label));
    }

    private void HandleToolCall(ToolCallEvent e)
    {
        JObject args = ParseArgs(e.ArgsJson);
        DateTime now = DateTime.UtcNow;

        switch (e.FunctionName)
        {
            case ToolUpdateProfile:
                ProfileMerger.ApplyToolCallArgs(_profile, args);
                SaveProfile();
                break;

            case ToolSetInterest:
                ApplyInterest(ReadString(args, ArgActivity), ReadString(args, ArgInterest));
                break;

            case ToolRememberEvent:
                ProfileMerger.AddEvent(_profile, ReadString(args, ArgSummary), now);
                ProfileMerger.PruneEvents(_profile, persona.memoryDays, now);
                SaveProfile();
                break;

            case ToolStartStudy:
                // Studying cannot begin while a mode owns her: DeskRoutine.Update
                // returns early the whole time one runs, and the clock does not
                // advance either, so a bare start would look like nothing happened.
                // Leaving the mode is part of starting to study, not a second thing
                // for the model to remember.
                StopActivity();
                _clock.StartStudy(
                    ReadInt(args, ArgStudyMinutes, persona.pomodoroStudyMinutes),
                    ReadInt(args, ArgBreakMinutes, persona.pomodoroBreakMinutes));
                if (studyMode != null && !studyMode.IsRunning) studyMode.StartStudying();
                break;

            case ToolEndStudy:
                _clock.Stop();
                if (studyMode != null && studyMode.IsRunning) studyMode.StopStudying();
                break;

            case ToolStartActivity:
                StartActivity(ReadString(args, ArgActivity));
                break;

            case ToolStopActivity:
                StopActivity();
                break;

            case ToolEndSession:
                // Saying goodbye with the minigame still on screen is not an ending.
                StopActivity();
                ProfileMerger.SetSessionSummary(_profile, ReadString(args, ArgSummary), now);
                _profiles.Save(_profile);
                break;
        }

        // Voice rule: a tool call is always answered in the same pass.
        session.RespondToToolCall(e.CallId, e.FunctionName, ResponseOk);
    }

    private void ApplyInterest(string activity, string interest)
    {
        BreakActivity which = ActivityFrom(activity);
        if (which == BreakActivity.None) return;

        // "Not right now" is a pass, not a dislike: it counts towards dropping
        // the subject for this session but is never written down as a feeling.
        if (interest == InterestNotNow)
        {
            _offers.RecordNotNow(which);
            return;
        }

        string stored = interest == StudentProfile.InterestLikes ? StudentProfile.InterestLikes
            : interest == StudentProfile.InterestDislikes ? StudentProfile.InterestDislikes
            : StudentProfile.InterestUnknown;

        if (which == BreakActivity.Dance) ProfileMerger.SetDanceInterest(_profile, stored);
        else ProfileMerger.SetGameInterest(_profile, stored);

        _offers.SetInterest(which, InterestFrom(stored));
        SaveProfile();
    }

    /// <summary>
    /// Starts a mode, and gets out of the current one first when it has to.
    ///
    /// Game mode refuses to start while a dance runs — that check is deliberate
    /// and stays — so "let's play a game" said mid-dance used to do nothing at
    /// all: the request was logged and dropped. The sequencing lives HERE rather
    /// than inside GameModeController because the blueprint has game mode looking
    /// at dance mode read-only; letting it call StopDance would make it a second
    /// writer of dance's flow. The request is instead queued, and Update starts it
    /// once the dance has finished properly — lights back, camera back, her back
    /// at the desk — which is what was asked for.
    /// </summary>
    private void StartActivity(string activity)
    {
        // Which activity is running names itself on the rising edge in Update;
        // this only starts one, so the label keeps a single writer.
        BreakActivity which = ActivityFrom(activity);

        if (which == BreakActivity.Dance && danceMode != null)
        {
            _gameQueued = false;
            danceMode.StartDance();
            return;
        }

        if (which != BreakActivity.Game || gameMode == null) return;

        if (danceMode != null && danceMode.IsRunning)
        {
            _gameQueued = true;
            danceMode.StopDance();
            return;
        }

        gameMode.StartGameMode();
    }

    /// <summary>
    /// The way out of a mode, now that the buttons are gone. Takes no argument
    /// because only one of the two can be running — game mode refuses to start
    /// while a dance does — so "stop what we are doing" is unambiguous, and one
    /// less argument is one less thing for the model to get wrong. Both calls
    /// are already no-ops at the wrong moment.
    /// </summary>
    private void StopActivity()
    {
        // Also kills a game waiting behind a dance: "let's stop" said between the
        // two must not be followed by the game starting anyway.
        _gameQueued = false;

        if (gameMode != null && gameMode.IsRunning) gameMode.StopGameMode();
        else if (danceMode != null && danceMode.IsRunning) danceMode.StopDance();
    }

    private string LabelFor(BreakActivity activity)
    {
        return activity == BreakActivity.Dance ? persona.danceLabel : persona.gameLabel;
    }

    private static BreakActivity ActivityFrom(string activity)
    {
        if (activity == ActivityDance) return BreakActivity.Dance;
        if (activity == ActivityGame) return BreakActivity.Game;
        return BreakActivity.None;
    }

    private static ActivityInterest InterestFrom(string stored)
    {
        if (stored == StudentProfile.InterestLikes) return ActivityInterest.Likes;
        if (stored == StudentProfile.InterestDislikes) return ActivityInterest.Dislikes;
        return ActivityInterest.Unknown;
    }

    private static string Fill(string template, string token, string value)
    {
        return string.IsNullOrEmpty(template) ? template : template.Replace(token, value ?? string.Empty);
    }

    /// <summary>
    /// Sends a line of stage direction. It travels as a user turn because the
    /// Live API has no other inbound channel, but it never reaches the caption
    /// log as speech, so it does not look like something the user said.
    /// </summary>
    private void SendNudge(string text)
    {
        if (string.IsNullOrEmpty(text) || !_connected) return;
        session.SendText(text);
        Debug.Log("[dürtü] " + text);
    }

    private void SaveProfile()
    {
        _profiles.Save(_profile);
    }

    private void StampAndSave()
    {
        if (_profile == null) return;
        ProfileMerger.StampSession(_profile, DateTime.UtcNow);
        _profiles.Save(_profile);
    }

    private static JObject ParseArgs(string argsJson)
    {
        if (string.IsNullOrEmpty(argsJson)) return new JObject();
        try
        {
            return JObject.Parse(argsJson);
        }
        catch (Exception)
        {
            return new JObject();
        }
    }

    private static string ReadString(JObject args, string key)
    {
        var token = args[key];
        return token == null || token.Type == JTokenType.Null ? null : token.ToString();
    }

    private static int ReadInt(JObject args, string key, int fallback)
    {
        var token = args[key];
        if (token == null || token.Type == JTokenType.Null) return fallback;

        int value;
        if (int.TryParse(token.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out value) && value > 0)
            return value;
        return fallback;
    }

}
