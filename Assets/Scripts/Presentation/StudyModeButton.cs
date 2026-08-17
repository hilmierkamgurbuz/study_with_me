using UnityEngine;
using UnityEngine.UI;

// The on-screen way in and out of study mode. DanceModeButton's sibling rather than
// GameModeButton's: one button, because study mode does not take the screen and so
// needs no separate way back.
//
// It is a TOGGLE, unlike the other two. A dance ends when its own clock runs out and
// game mode has its "back to the room"; studying has no natural end, so the way out is
// the same button again.
//
// Temporary by design: this is here so the mode can be driven by hand while the voice
// side is built. The intent is for the conversation to start and end study sessions,
// at which point this becomes a debug affordance rather than the way in.
public class StudyModeButton : MonoBehaviour
{
    public DeskRoutine studyMode;
    public Button button;
    public Text label;

    public string idleLabel = "Ders Çalış";
    public string runningLabel = "Çalışmayı Bitir";

    private bool _wasRunning;

    private void Start()
    {
        if (studyMode == null || button == null)
        {
            Debug.LogError("[StudyModeButton] studyMode and button must both be assigned.", this);
            enabled = false;
            return;
        }

        button.onClick.AddListener(OnClick);
        Paint(studyMode.IsRunning);
    }

    private void OnDestroy()
    {
        if (button != null) button.onClick.RemoveListener(OnClick);
    }

    private void OnClick()
    {
        if (studyMode.IsRunning) studyMode.StopStudying();
        else studyMode.StartStudying();
    }

    // Polled rather than event-driven, the same trade the other two buttons made: one
    // bool compare on one object against a new member plus a subscription to keep in
    // sync. It matters more here than there, because the mode can also be turned off
    // from the component's own context menu.
    private void Update()
    {
        // Teardown order between two components is not guaranteed; on play-mode exit
        // the routine can die first and polling it then throws every frame.
        if (studyMode == null) { enabled = false; return; }

        bool running = studyMode.IsRunning;

        if (running == _wasRunning) return;

        Paint(running);
    }

    private void Paint(bool running)
    {
        _wasRunning = running;

        if (label != null) label.text = running ? runningLabel : idleLabel;
    }
}
