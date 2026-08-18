using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The app's only button. It knows nothing about sessions, sockets or
/// microphones: it reports that it was pressed and paints whichever of four
/// states it is told to. Deciding what a press MEANS belongs to the composition
/// root, which already holds the session state — putting it here would mean a
/// second copy of that state to keep in sync.
/// </summary>
public class PushToTalkButtonView : MonoBehaviour
{
    public Button button;
    public Image image;

    public Sprite idleSprite;
    public Sprite listeningSprite;

    [Tooltip("Bağlanırken butonun karartılma çarpanı.")]
    [Range(0f, 1f)] public float connectingDim = 0.6f;

    public event Action Pressed;

    private PttButtonState _state;
    private bool _painted;

    private void Awake()
    {
        if (button != null) button.onClick.AddListener(OnClicked);
    }

    private void Start()
    {
        // Whatever the scene was saved showing, the authored state is repainted
        // once so the button never opens on a stale sprite.
        Repaint();
    }

    private void OnDestroy()
    {
        if (button != null) button.onClick.RemoveListener(OnClicked);
    }

    private void OnClicked()
    {
        var handler = Pressed;
        if (handler != null) handler();
    }

    public void SetState(PttButtonState state)
    {
        if (_painted && _state == state) return;
        _state = state;
        Repaint();
    }

    private void Repaint()
    {
        _painted = true;

        if (image != null)
        {
            // Offline and Ready look the same on purpose: to the user both read
            // as "press to talk", and the first press happening to be what opens
            // the connection is not something worth explaining on screen.
            Sprite sprite = _state == PttButtonState.Listening ? listeningSprite : idleSprite;
            if (sprite != null) image.sprite = sprite;

            // Multiplies with the Button's own colour transition rather than
            // fighting it — that tint lives on the CanvasRenderer, this one on
            // the Graphic, so press feedback survives the dimming.
            image.color = _state == PttButtonState.Connecting
                ? new Color(connectingDim, connectingDim, connectingDim, 1f)
                : Color.white;
        }

        // Nothing useful can happen mid-connect, and a press then would reach a
        // Connect() that must not run twice.
        if (button != null) button.interactable = _state != PttButtonState.Connecting;
    }
}

public enum PttButtonState
{
    /// <summary>Never connected yet — a press opens the session.</summary>
    Offline,

    /// <summary>Connecting, or reconnecting on its own after a drop.</summary>
    Connecting,

    /// <summary>Connected and quiet — a press opens the mic.</summary>
    Ready,

    /// <summary>Mic is open — a press closes it.</summary>
    Listening
}
