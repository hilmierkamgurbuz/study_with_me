using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

/// <summary>
/// Tek parmağın / farenin o KAREDEKİ hâli, tek yerden.
///
/// <see cref="DropController"/> ve <see cref="WormBoostDirector"/> aynı touch/mouse okuma
/// bloğunu birebir kopyalamıştı. İkisi de artık buradan besleniyor — bir backend farkı
/// çıktığında düzeltilecek tek bir yer var.
///
/// <b>study_with_me'ye gömülürken bu dosya ESKİ <c>Input</c> API'sinden YENİ Input
/// System'e taşındı — oyunun tek legacy girdi tüketicisi burasıydı.</b> Ev sahibi proje
/// bilerek <c>activeInputHandler: 1</c> (sadece yeni sistem): eski API oradan
/// çağrıldığında <c>InvalidOperationException</c> atıyor. Alternatif "Active Input
/// Handling = Both" tek ayardı ama editör yeniden başlatması ister ve projede iki input
/// backend'i birden yaşatırdı. Yukarıdaki gerekçenin kendisi bu dosyanın var oluş
/// sebebi: backend değişince düzeltilecek TEK nokta burası.
///
/// <b>Neden <see cref="IsOverUI"/> ayrı bir metot:</b> Sahnedeki EventSystem
/// <c>InputSystemUIInputModule</c> kullanıyor ve oynanış girdisi de artık aynı
/// backend'den okunuyor — eski sürümdeki "eski <c>fingerId</c> 0'dan, yeni
/// <c>touchId</c> 1'den başlıyor" kayması ORTADAN KALKTI. Bu yüzden ±1 deneyen yedek
/// arama da silindi: aynı uzaydayız, <c>touchId</c> doğrudan pointer id olarak geçiyor
/// ve kaydırılmış bir id sormak artık yanlış pozitif üretirdi.
/// </summary>
public static class PointerInput
{
    /// <summary>Bu karede yeni bir dokunuş/tık BAŞLADI mı.</summary>
    public static bool Began { get { Sample(); return _began; } }

    /// <summary>Parmak/tuş şu an basılı mı.</summary>
    public static bool Held { get { Sample(); return _held; } }

    /// <summary>Bu karede BIRAKILDI mı.</summary>
    public static bool Released { get { Sample(); return _released; } }

    /// <summary>Ekran koordinatı.</summary>
    public static Vector2 Position { get { Sample(); return _position; } }

    /// <summary>Dokunuşun <c>touchId</c>'si; fare ise -1.</summary>
    public static int FingerId { get { Sample(); return _fingerId; } }

    static int     _frame = -1;
    static bool    _began, _held, _released;
    static Vector2 _position;
    static int     _fingerId = -1;

    /// <summary>
    /// Kare başına BİR KEZ okur. Ham girdi bir kare içinde değişmediği için çağrı sırası
    /// önemli değil — script execution order'ı farklı iki abone de aynı değerleri görüyor.
    /// </summary>
    static void Sample()
    {
        if (_frame == Time.frameCount) return;

        _frame = Time.frameCount;

        TouchControl touch = Touchscreen.current != null ? Touchscreen.current.primaryTouch : null;

        // Bırakma karesi de dokunuş sayılıyor: eski kodda o kare hâlâ touchCount > 0
        // olduğu için touch dalına giriyordu, konumu ve fingerId'si oradan okunuyordu.
        if (touch != null && (touch.press.isPressed || touch.press.wasReleasedThisFrame))
        {
            _position = touch.position.ReadValue();
            _fingerId = touch.touchId.ReadValue();

            _began    = touch.press.wasPressedThisFrame;
            _held     = touch.press.isPressed;
            _released = touch.press.wasReleasedThisFrame;

            return;
        }

        Mouse mouse = Mouse.current;

        if (mouse != null)
        {
            _position = mouse.position.ReadValue();
            _fingerId = -1;

            _began    = mouse.leftButton.wasPressedThisFrame;
            _held     = mouse.leftButton.isPressed;
            _released = mouse.leftButton.wasReleasedThisFrame;

            return;
        }

        // Ne fare ne dokunmatik: basılı hiçbir şey yok. Konum SON bilinen değerde
        // bırakılıyor — eski kod burada mousePosition'ı okuyup (0,0)'a düşürüyordu,
        // ki parmak kalktıktan sonra Position okuyan biri için sıfır sıçraması demekti.
        _began = false;
        _held = false;
        _released = false;
        _fingerId = -1;
    }

    /// <summary>Bu pointer şu an bir UI elemanının üstünde mi.</summary>
    public static bool IsOverUI()
    {
        EventSystem es = EventSystem.current;

        if (es == null) return false;

        // Asıl güvenilir yol: "o anki pointer".
        if (es.IsPointerOverGameObject()) return true;

        int f = FingerId;

        if (f < 0) return false;

        // Yedek: dokunuşun kendi id'si. Yeni modül pointer id olarak touchId'yi
        // kullanıyor, o yüzden kaydırma yok.
        return es.IsPointerOverGameObject(f);
    }

    // Domain reload kapalıyken statikler bir sonraki oturuma taşınmasın —
    // GameEvents.ResetStatics ve BoostGate.ResetStatics ile aynı desen.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics()
    {
        _frame    = -1;
        _began    = false;
        _held     = false;
        _released = false;
        _position = default;
        _fingerId = -1;
    }
}
