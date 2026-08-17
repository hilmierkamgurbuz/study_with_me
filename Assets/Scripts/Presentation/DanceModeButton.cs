using UnityEngine;
using UnityEngine.UI;

// The on-screen entry point for dance mode. It exists only to give the button
// feedback for the two minutes the dance runs — wiring onClick straight to
// DanceModeController.StartDance would work, but would leave a button that looks
// dead while pressing it does nothing.
public class DanceModeButton : MonoBehaviour
{
    public DanceModeController danceMode;
    public Button button;
    public Text label;

    public string idleLabel = "Dance Mode";
    public string runningLabel = "Dancing…";

    private bool _wasRunning;

    private void Start()
    {
        if (danceMode == null || button == null)
        {
            Debug.LogError("[DanceModeButton] danceMode and button must both be assigned.", this);
            enabled = false;
            return;
        }

        button.onClick.AddListener(OnClick);
        Paint(false);
    }

    private void OnDestroy()
    {
        if (button != null) button.onClick.RemoveListener(OnClick);
    }

    private void OnClick()
    {
        // StartDance already ignores a second call while a dance is running, so
        // this only has to not fight it.
        danceMode.StartDance();
    }

    // Polled rather than event-driven: it is one bool compare on one object, and
    // an event would mean a new member on DanceModeController plus a subscription
    // to keep in sync — more machinery than the saving is worth.
    private void Update()
    {
        // Teardown order is not guaranteed: on play-mode exit the controller can
        // be destroyed while this component still ticks, and polling a destroyed
        // object throws every frame.
        if (danceMode == null) { enabled = false; return; }

        bool running = danceMode.IsRunning;
        if (running == _wasRunning) return;
        Paint(running);
    }

    private void Paint(bool running)
    {
        _wasRunning = running;
        button.interactable = !running;
        if (label != null) label.text = running ? runningLabel : idleLabel;
    }
}
