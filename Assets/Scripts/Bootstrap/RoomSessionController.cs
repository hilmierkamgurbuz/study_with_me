using System;
using System.Text;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class RoomSessionController : MonoBehaviour
{
    private const string UpdateProfileSchema =
        "{\"type\":\"OBJECT\",\"properties\":{" +
        "\"studentName\":{\"type\":\"STRING\",\"description\":\"The user's name.\"}," +
        "\"gradeOrClass\":{\"type\":\"STRING\",\"description\":\"The user's school grade/class or year.\"}," +
        "\"examTarget\":{\"type\":\"STRING\",\"description\":\"The exam or goal the user is preparing for.\"}," +
        "\"usualStudyTime\":{\"type\":\"STRING\",\"description\":\"When the user usually studies, e.g. mornings or evenings.\"}," +
        "\"typicalSessionMinutes\":{\"type\":\"INTEGER\",\"description\":\"How many minutes the user typically studies in one sitting.\"}," +
        "\"preferredBreakFrequencyMinutes\":{\"type\":\"INTEGER\",\"description\":\"How often, in minutes, the user likes to take a break.\"}" +
        "}}";

    public GeminiLiveVoiceSession session;
    public CharacterPresenter character;

    private readonly IProfileRepository _profiles = new LocalJsonProfileRepository();
    private StudentProfile _profile;
    private DateTime _sessionStartUtc;
    private readonly StringBuilder _log = new StringBuilder();
    private bool _connected;
    private bool _connecting;
    private bool _talking;

    private void Awake()
    {
        _profile = _profiles.Load();

        session.OnLog += AppendLog;
        session.CaptionReceived += c => AppendLog((c.IsUser ? "Sen: " : "Chloe: ") + c.Text);
        session.ToolCallReceived += HandleToolCall;
        session.TurnStateChanged += HandleTurnStateChanged;
        session.OnConnected += HandleConnected;
        session.OnDisconnected += HandleDisconnected;
    }

    private void HandleConnected()
    {
        _connected = true;
        _connecting = false;
        _sessionStartUtc = DateTime.UtcNow;
    }

    private void HandleDisconnected()
    {
        _connected = false;
        _connecting = false;
        _talking = false;
        EndSession();
    }

    private void HandleTurnStateChanged(TurnState state)
    {
        character.SetTurnState(state);
    }

    private void HandleToolCall(ToolCallEvent e)
    {
        if (e.FunctionName == "update_student_profile")
        {
            var args = JObject.Parse(e.ArgsJson);
            if (_profile == null) _profile = new StudentProfile();
            ProfileMerger.ApplyToolCallArgs(_profile, args);
            _profiles.Save(_profile);
            AppendLog("[profil güncellendi] " + e.ArgsJson);
        }
        session.RespondToToolCall(e.CallId, e.FunctionName, "{\"result\":\"ok\"}");
    }

    private void EndSession()
    {
        if (_profile == null) _profile = new StudentProfile();
        double minutes = (DateTime.UtcNow - _sessionStartUtc).TotalMinutes;
        ProfileMerger.ApplySessionStats(_profile, minutes);
        _profiles.Save(_profile);
        AppendLog(string.Format("[oturum bitti] +{0:F1} dk, toplam {1:F0} dk, {2} oturum",
            minutes, _profile.totalStudyMinutes, _profile.totalStudySessions));
    }

    public void Connect()
    {
        _connecting = true;
        var sessionConfig = new VoiceSessionConfig
        {
            SystemInstruction = BuildSystemInstruction(),
            Tools =
            {
                new VoiceToolDeclaration
                {
                    Name = "update_student_profile",
                    Description = "Saves or updates what we know about the student — name, grade, exam target, study habits.",
                    ParametersJsonSchema = UpdateProfileSchema
                }
            }
        };
        _ = session.Connect(sessionConfig);
    }

    private string BuildSystemInstruction()
    {
        var sb = new StringBuilder();
        sb.Append("Sen Chloe'sin — kullanıcıyla birlikte-çalışma uygulaması üzerinden tanışmış, ")
          .Append("samimi ve destekleyici bir çalışma arkadaşısın. Türkçe konuş. Sesli konuşurken ")
          .Append("kısa, doğal cümleler kur; bir form doldurtuyormuş gibi hissettirme.\n\n");

        if (_profile == null)
        {
            sb.Append("Bu ilk tanışmanız. Kendini kısaca tanıt (\"Merhaba, ben Chloe\"), ")
              .Append("sonra doğal bir sohbet akışında öğrenmen gerekenler: kullanıcının adı, hangi sınıfta/bölümde olduğu, ")
              .Append("hangi sınava/hedefe hazırlandığı, genelde ne zaman ve ne kadar süre çalıştığı, ne kadar sürede bir mola vermeyi sevdiği. ")
              .Append("Bunları art arda soru bombardımanı gibi sorma — sohbetin doğal akışında öğren. ")
              .Append("Her yeni bilgiyi öğrendiğinde update_student_profile fonksiyonunu çağır.");
        }
        else
        {
            sb.Append("Kullanıcıyla önceden tanıştınız. Bildiklerin: ");
            if (!string.IsNullOrEmpty(_profile.studentName)) sb.Append("adı ").Append(_profile.studentName).Append("; ");
            if (!string.IsNullOrEmpty(_profile.gradeOrClass)) sb.Append("sınıfı/bölümü ").Append(_profile.gradeOrClass).Append("; ");
            if (!string.IsNullOrEmpty(_profile.examTarget)) sb.Append("hedefi ").Append(_profile.examTarget).Append("; ");
            if (_profile.typicalSessionMinutes > 0) sb.Append("genelde ").Append(_profile.typicalSessionMinutes).Append(" dk çalışır; ");
            if (_profile.preferredBreakFrequencyMinutes > 0) sb.Append(_profile.preferredBreakFrequencyMinutes).Append(" dk'da bir mola sever; ");
            if (_profile.totalStudySessions > 0)
                sb.Append("toplam ").Append(_profile.totalStudySessions).Append(" oturumda ")
                  .Append((int)_profile.totalStudyMinutes).Append(" dk çalışmışsınız. ");
            sb.Append("\nOnu ismiyle karşıla, geçmişe doğal şekilde gönderme yap (\"yine görüşüyoruz\" tarzı). ")
              .Append("Konuşma sırasında yeni bir şey öğrenirsen (mesela çalışma alışkanlığı değiştiyse) update_student_profile'ı tekrar çağır.");
        }
        return sb.ToString();
    }

    private void AppendLog(string line)
    {
        _log.AppendLine(line);
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(20, 20, 560, 460));

        if (!_connected)
        {
            GUI.enabled = !_connecting;
            if (GUILayout.Button(_connecting ? "Bağlanıyor..." : "Chloe'ye bağlan", GUILayout.Height(40)))
            {
                Connect();
            }
            GUI.enabled = true;
        }
        else
        {
            string label = _talking ? "Dinliyor... (durdurmak için tıkla)" : "Konuşmak için tıkla";
            if (GUILayout.Button(label, GUILayout.Height(60)))
            {
                _talking = !_talking;
                if (_talking) session.BeginPushToTalk();
                else session.EndPushToTalk();
            }
        }

        GUILayout.Space(10);
        GUILayout.Label(_log.ToString());
        GUILayout.EndArea();
    }
}
