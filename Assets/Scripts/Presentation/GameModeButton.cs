using UnityEngine;
using UnityEngine.UI;

// The on-screen way in and out of game mode. DanceModeButton's sibling rather than
// its copy: it drives two buttons, because game mode is the one thing that takes the
// whole screen and so has to carry its own way back.
//
// The back button lives on this canvas, not on the game's, for two reasons: the
// vendored game is never written into, and this canvas outranks the game's own so
// the way out cannot end up underneath it.
public class GameModeButton : MonoBehaviour
{
    public GameModeController gameMode;
    public Button startButton;
    public Text startLabel;
    // Hidden until game mode runs, and hidden again the moment the unwind starts —
    // pressing it twice would otherwise begin a return inside a return.
    public Button backButton;

    public string idleLabel = "Oyun Oyna";
    public string runningLabel = "Oyun modunda…";

    private bool _wasRunning;
    private bool _wasLeavable;

    private void Start()
    {
        if (gameMode == null || startButton == null)
        {
            Debug.LogError("[GameModeButton] gameMode and startButton must both be assigned.", this);
            enabled = false;
            return;
        }

        startButton.onClick.AddListener(OnStartClicked);

        if (backButton != null) backButton.onClick.AddListener(OnBackClicked);

        Paint(false, false);
    }

    private void OnDestroy()
    {
        if (startButton != null) startButton.onClick.RemoveListener(OnStartClicked);
        if (backButton != null) backButton.onClick.RemoveListener(OnBackClicked);
    }

    // Both of these are already no-ops at the wrong moment — StartGameMode ignores a
    // second call while it runs, StopGameMode ignores one while it is unwinding — so
    // neither click needs a guard of its own here.
    private void OnStartClicked()
    {
        gameMode.StartGameMode();
    }

    private void OnBackClicked()
    {
        gameMode.StopGameMode();
    }

    // Polled rather than event-driven, same trade as DanceModeButton: two bool
    // compares on one object against a new member plus a subscription to keep in sync.
    private void Update()
    {
        // Teardown order between two components is not guaranteed; on play-mode exit
        // the controller can die first and polling it then throws every frame.
        if (gameMode == null) { enabled = false; return; }

        bool running = gameMode.IsRunning;
        bool leavable = gameMode.CanLeave;

        if (running == _wasRunning && leavable == _wasLeavable) return;

        Paint(running, leavable);
    }

    private void Paint(bool running, bool leavable)
    {
        _wasRunning = running;
        _wasLeavable = leavable;

        startButton.interactable = !running;

        if (startLabel != null) startLabel.text = running ? runningLabel : idleLabel;

        if (backButton != null) backButton.gameObject.SetActive(leavable);
    }
}
