using UnityEngine;

/// <summary>
/// Everything Chloe is told, and every number that shapes a session, as data.
/// No wording and no threshold lives in code — the runtime only assembles what
/// this asset holds. Seed values are written once by Tools > StudyWithMe >
/// Set Up Session AI; after that this asset is the authority.
/// </summary>
[CreateAssetMenu(fileName = "ChloePersona", menuName = "StudyWithMe/Chloe Persona")]
public class ChloePersonaConfig : ScriptableObject
{
    [Header("Sistem talimatı — her oturumda gönderilir")]
    [TextArea(4, 14)] public string persona;
    [TextArea(4, 14)] public string safetyRules;
    [TextArea(4, 14)] public string conversationRules;

    [Tooltip("{pomodoroStudy} ve {pomodoroBreak} yer tutucuları aşağıdaki sayılarla doldurulur.")]
    [TextArea(4, 20)] public string toolRules;

    [Header("Açılış — profile göre biri seçilir")]
    [TextArea(4, 14)] public string firstMeeting;

    [Tooltip("{profile} yer tutucusu kayıtlı profilin JSON hâliyle doldurulur.")]
    [TextArea(4, 14)] public string returning;

    [Header("Dürtüler — modele kullanıcı turu olarak gider")]
    [TextArea(2, 6)] public string openingNudge;
    [TextArea(2, 6)] public string studyBlockEndedNudge;

    [Tooltip("{activity} yer tutucusu aşağıdaki etkinlik adıyla doldurulur.")]
    [TextArea(2, 6)] public string offerActivityNudge;

    [Tooltip("{activity} yer tutucusu aşağıdaki etkinlik adıyla doldurulur.")]
    [TextArea(2, 6)] public string askActivityInterestNudge;

    [TextArea(2, 6)] public string breakOverNudge;

    [Tooltip("{activity} yer tutucusu aşağıdaki etkinlik adıyla doldurulur.")]
    [TextArea(2, 6)] public string activityOverNudge;

    [Header("Etkinlik adları")]
    public string danceLabel;
    public string gameLabel;

    [Header("Sayılar")]
    [Tooltip("Kullanıcı süreyi Chloe'ye bırakırsa kullanılacak pomodoro değerleri.")]
    public int pomodoroStudyMinutes;
    public int pomodoroBreakMinutes;

    [Tooltip("Hatırlanan olaylar bu kadar günden eskiyse silinir.")]
    public int memoryDays;

    [Tooltip("Aynı etkinlik için üst üste bu kadar 'şimdi olmaz'dan sonra o oturumda bir daha teklif edilmez.")]
    public int declineLimit;

    [Tooltip("Kaç molada bir teklif gelebilir. 2 = her ikinci molada.")]
    public int offerEveryNthBreak;

    [Header("Yeniden bağlanma")]
    [Tooltip("Kopan bağlantı için geri çekilme tavanı (saniye).")]
    public float reconnectBackoffMaxSeconds;
}
