using UnityEngine;

// What she does at her desk between conversations: she reads. That is the whole
// routine — turned to the book, hands resting on her legs, and every few minutes one
// hand reaches out to turn a page.
//
// It used to also have her working at the computer, and that is gone rather than
// switched off. The measurement that ended it: her arm reaches 0.259 m and the
// keyboard sits 0.70-0.81 m from her shoulder, so her hands could never arrive there —
// not with any clip, and not with IK either. Everything that existed to serve that
// beat went with it (see D-049).
//
// She turns her BODY towards the book, not her head. The rig is Generic, so there is
// no humanoid look-at, and aiming the head by hand would put a second writer on the
// head bone — the shape D-015 already had to undo. Turning the seated body goes
// through SetRestingPose, CharacterPresenter's sanctioned single-writer path.
//
// The ARMS are not driven from here at all. ArmIkSolver overrides the whole chain, so
// the masked Arms layer only ever contributes the shoulder, which its own default
// state gives for free. That is why this component no longer touches the Animator.
public class DeskRoutine : MonoBehaviour
{
    [Header("Cast")]
    public CharacterPresenter chloe;
    public BookPageTurner book;
    public ArmIkSolver armIk;

    [Header("The book")]
    // A Transform, not coordinates: move the book and the angle she turns through
    // follows it with nothing to edit here.
    public Transform bookTarget;

    [Header("Hands")]
    // The ONLY hand target. Resting hands come from the animation — IK is off entirely
    // between page turns, so there is nothing to place for them. Its ROTATION matters
    // as much as its position: the wrist takes it, which is what makes the reach look
    // like a hand turning a page rather than a hand arriving at a coordinate.
    public Transform pageHand;
    // Pins the right hand to pageHand so dragging the target moves the hand and turning
    // it turns the wrist. A tuning aid only — the reach is otherwise a 1.5 second event
    // nobody can adjust anything against. Same kind of field, and the same reason, as
    // CharacterPresenter.previewSpeaking. Untick before leaving Play.
    public bool previewPageReach;

    [Header("Who else can own her")]
    public DanceModeController danceMode;
    public GameModeController gameMode;

    [Header("Rhythm (seconds)")]
    public float pageTurnSeconds = 180f;
    // How long the hand is out. The page is in the air for about this long too.
    public float pageGestureSeconds = 1.5f;

    [Header("Turning")]
    public float turnDegreesPerSecond = 60f;
    // How close to facing the book counts as arrived — the page clock only runs once
    // she is actually looking at it.
    public float arrivedDegrees = 6f;
    // Applied in her own frame (+Z is where she faces) while reading, for lining her
    // up with the book without moving the book.
    public Vector3 readingPositionOffset;
    public float readingYawOffset;
    public float offsetMetresPerSecond = 0.5f;

    [Header("Tuning")]
    // While this is on, her pose is NOT written, so anything dragged in the Scene view
    // during Play survives instead of being overwritten next frame. Untick
    // CharacterPresenter.lockPose as well — it rebuilds her root on its own. Read the
    // numbers back out with ArmIkSolver's Log Hand Targets, because Play-mode scene
    // edits are discarded on exit while the console is not.
    public bool tunePose;

    // Off until the button says otherwise. She is meant to be talked to first, and a
    // character who turns to her book the moment the scene loads has decided the
    // conversation is over before it started.
    private bool _studying;

    private float _pageClock;
    private float _gestureClock;

    private Vector3 _deskPosition;
    private Quaternion _deskRotation;
    private bool _captured;

    private Vector3 _appliedOffset;

    private void Start()
    {
        if (chloe == null)
        {
            Debug.LogError("[DeskRoutine] chloe is not assigned — run Tools > StudyWithMe > Set Up Desk Routine.", this);
            enabled = false;
        }
    }

    /// <summary>Studying, whether or not she is mid-page or currently talking.</summary>
    public bool IsRunning { get { return _studying; } }

    [ContextMenu("Start Studying")]
    public void StartStudying()
    {
        _studying = true;
        _pageClock = 0f;
    }

    [ContextMenu("Stop Studying")]
    public void StopStudying()
    {
        _studying = false;
        _gestureClock = 0f;
    }

    private void Update()
    {
        // Not started, or already stopped: she keeps the pose the scene was authored
        // with — at her desk, facing the monitor, waiting to be spoken to. Nothing is
        // written at all, so in this state her pose has exactly one owner again.
        if (!_studying)
        {
            chloe.SetReading(false);
            ApplyHands(false);

            // Stopping mid-page leaves her turned to the book, so she is walked back
            // to the authored pose and only THEN left alone — once she has arrived,
            // nothing here writes at all and her pose has a single owner again.
            if (_captured && !tunePose && !ModeOwnsHer && NotYetHome) FaceTowards(false);

            return;
        }

        // Dance and game mode own her pose outright while they run, so this does not
        // touch it — two writers on her root is the one thing CharacterPresenter
        // exists to prevent. Studying is SUSPENDED by them, never cancelled: when they
        // finish she carries on where she left off.
        if (ModeOwnsHer)
        {
            chloe.SetReading(false);
            ApplyHands(false);
            return;
        }

        CaptureDeskPose();

        // Talking wins. She comes back to the pose the scene was authored with, which
        // is the one facing the monitor, and the page clock freezes — conversation
        // time is deliberately not counted as reading time.
        bool working = !chloe.IsSpeaking;

        chloe.SetReading(working);
        ApplyHands(working);

        // Skipped while tuning so a drag in the Scene view survives the frame. The
        // routine keeps running, which is the point: the pose has to be live to judge
        // where a hand belongs.
        if (!tunePose) FaceTowards(working);

        if (!working) return;

        if (_gestureClock > 0f) _gestureClock -= Time.deltaTime;

        TickPageTurn();
    }

    // Still turned towards the book, or still carrying the reading offset.
    private bool NotYetHome
    {
        get
        {
            return Quaternion.Angle(chloe.RestingRotation, _deskRotation) > 0.1f
                || _appliedOffset.sqrMagnitude > 0.000001f;
        }
    }

    private bool ModeOwnsHer
    {
        get
        {
            if (danceMode != null && danceMode.IsRunning) return true;
            if (gameMode != null && gameMode.IsRunning) return true;
            return false;
        }
    }

    /// <summary>
    /// Her desk pose, read on the first frame this actually runs rather than in
    /// Start(): CharacterPresenter fills RestingPosition in its own Start() and the
    /// order between two Start() calls is undefined — the same race D-037 traced when
    /// game mode captured (0,0,0) and sent her to the world origin.
    /// </summary>
    private void CaptureDeskPose()
    {
        if (_captured) return;

        _deskPosition = chloe.RestingPosition;
        _deskRotation = chloe.RestingRotation;
        _captured = true;
    }

    /// <param name="towardsBook">false returns her to the pose the scene was authored
    /// with, which is NOT recomputed from the monitor's position — she was placed
    /// facing it by hand (measured 0.9 degrees off a computed look-at) and that
    /// authored pose is the one to come back to.</param>
    private void FaceTowards(bool towardsBook)
    {
        Quaternion want = towardsBook ? BookRotation() : _deskRotation;

        if (towardsBook) want *= Quaternion.Euler(0f, readingYawOffset, 0f);

        Vector3 wantOffset = towardsBook ? readingPositionOffset : Vector3.zero;

        Quaternion next = Quaternion.RotateTowards(chloe.RestingRotation, want,
                                                   turnDegreesPerSecond * Time.deltaTime);

        _appliedOffset = Vector3.MoveTowards(_appliedOffset, wantOffset,
                                             offsetMetresPerSecond * Time.deltaTime);

        // Never transform.position: LateUpdate rebuilds her from the resting pose, so
        // a direct write would be reverted on the same frame.
        chloe.SetRestingPose(_deskPosition + next * _appliedOffset, next);
    }

    private Quaternion BookRotation()
    {
        if (bookTarget == null) return _deskRotation;

        Vector3 flat = bookTarget.position - _deskPosition;
        flat.y = 0f;

        return flat.sqrMagnitude > 0.000001f ? Quaternion.LookRotation(flat, Vector3.up) : _deskRotation;
    }

    /// <summary>
    /// The page reach, and nothing else.
    ///
    /// IK is OFF between page turns: her resting arms come from the animation, which
    /// already looks right, so there is no resting target to place and no override to
    /// fight. Turning a page fades the right hand out to the page and back — the WEIGHT
    /// is the arc, so the hand leaves from wherever the animation has it and returns
    /// there by itself. That is what removed the lap targets: their only job had been
    /// to be the place the hand came back to.
    ///
    /// The left arm is never solved. One hand turns a page, and leaving the other to
    /// the animation is one less thing to place and one less thing to go wrong.
    ///
    /// There is no page-turn CLIP for this, in this project or in the Mixamo set it
    /// came from. A target is what makes the gesture possible with no new art — and it
    /// puts the look of it in the scene, adjustable by eye, wrist included.
    /// </summary>
    private void ApplyHands(bool working)
    {
        if (armIk == null) return;

        armIk.leftTarget = null;
        armIk.rightTarget = pageHand;

        // Held at full reach while tuning, so dragging the target moves the hand and
        // turning it turns the wrist. Otherwise a 1.5 second event is the only time
        // anything is visible, which is no way to place anything.
        if (previewPageReach && pageHand != null)
        {
            armIk.weight = 1f;
            return;
        }

        if (!working || pageHand == null || _gestureClock <= 0f || pageGestureSeconds <= 0f)
        {
            armIk.weight = 0f;
            return;
        }

        float through = Mathf.Clamp01(1f - _gestureClock / pageGestureSeconds);

        armIk.weight = Mathf.Sin(Mathf.PI * through);
    }

    /// <summary>
    /// A page every pageTurnSeconds, and the clock only runs once she has ARRIVED
    /// facing the book. Turning during the swing would be a page turning behind her
    /// back, which is why BookPageTurner stopped driving itself (D-038).
    /// </summary>
    private void TickPageTurn()
    {
        if (book == null)
        {
            _pageClock = 0f;
            return;
        }

        if (Quaternion.Angle(chloe.RestingRotation, BookRotation()) > arrivedDegrees) return;

        _pageClock += Time.deltaTime;

        if (_pageClock < pageTurnSeconds) return;

        _pageClock = 0f;
        // Her hand is out for as long as the page is in the air.
        _gestureClock = pageGestureSeconds;
        book.TurnPage();
    }

    [ContextMenu("Turn A Page Now")]
    public void TurnAPageNow()
    {
        if (book == null) return;

        _pageClock = 0f;
        _gestureClock = pageGestureSeconds;
        book.TurnPage();
    }

    private void OnDrawGizmosSelected()
    {
        if (chloe == null || bookTarget == null) return;

        Vector3 from = Application.isPlaying && _captured ? _deskPosition : chloe.transform.position;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(from, bookTarget.position);
    }
}
