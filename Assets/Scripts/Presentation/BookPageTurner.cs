using System.Collections;
using UnityEngine;

// Turns a page of the desk book on its own while she works, as room ambience.
//
// The sheet is hidden except during a turn, which is what keeps the loop seamless:
// it appears carrying exactly what the right page was showing, the page underneath
// quietly becomes the next one while it is covered, and at the end the left page
// becomes what the sheet carried. Nothing ever snaps back on screen.
//
// TurnPage() is public on purpose. The desk behaviour that will have her glance at
// the computer and then at the book should be what decides WHEN a page turns — a
// page turning while she is looking at the monitor is a page turning behind her
// back. Until that exists the timer below stands in for it; when it lands, set
// selfDriven false and call TurnPage() from there. That is the whole integration.
public class BookPageTurner : MonoBehaviour
{
    [Header("The book")]
    public Renderer leftPage;
    public Renderer rightPage;
    // Disabled whenever a turn is not running.
    public Renderer flipPage;
    // Sits on the spine; the sheet swings about its FORWARD axis. Placed by
    // Tools > StudyWithMe > Set Up Book Pages from the spine's measured bounds.
    public Transform hinge;

    [Header("Which material slot is the paper")]
    // Measured once in the editor rather than searched for every turn: a page block
    // is two submeshes and only one of them is the face you read.
    public int leftPageSlot = 1;
    public int rightPageSlot = 1;
    public int flipPageSlot = 0;

    [Header("The two page images, alternated")]
    public Material pageA;
    public Material pageB;

    [Header("When it is allowed to turn")]
    public CharacterPresenter chloe;
    public DanceModeController danceMode;
    public GameModeController gameMode;

    [Header("Timing")]
    // Off once the desk behaviour drives this instead.
    public bool selfDriven = true;
    public float intervalSeconds = 120f;
    public float turnSeconds = 1.2f;

    [Header("Gizmos")]
    public bool drawHinge = true;

    private float _clock;
    private bool _turning;

    public bool IsTurning { get { return _turning; } }

    private void Start()
    {
        if (leftPage == null || rightPage == null || flipPage == null || hinge == null ||
            pageA == null || pageB == null)
        {
            Debug.LogError("[BookPages] BookPageTurner is missing a reference — run " +
                           "Tools > StudyWithMe > Set Up Book Pages.", this);
            enabled = false;
            return;
        }

        // The sheet is scenery only while it moves. It starts lying on the right
        // showing the same image as the block beneath it, so hiding it changes
        // nothing on screen.
        flipPage.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!selfDriven) return;

        // The clock does NOT advance while a turn is blocked, and that is the point:
        // if it ran during a five-minute conversation, a page would turn the instant
        // she stopped talking, which reads as a reply rather than as ambience.
        if (!CanTurn) return;

        _clock += Time.deltaTime;

        if (_clock < intervalSeconds) return;

        _clock = 0f;
        TurnPage();
    }

    /// <summary>She is at her desk, not talking, and no other mode owns the room.</summary>
    public bool CanTurn
    {
        get
        {
            if (_turning || !enabled) return false;
            // Speaking only — listening and thinking are her being quiet, and a page
            // turning while she listens is exactly the sort of life this is for.
            if (chloe != null && chloe.IsSpeaking) return false;
            // "At her desk" has to be built from the negative: there is no session
            // state machine yet to ask, so the room is a study room whenever neither
            // of the two modes that take it over is running.
            if (danceMode != null && danceMode.IsRunning) return false;
            if (gameMode != null && gameMode.IsRunning) return false;
            return true;
        }
    }

    [ContextMenu("Turn Page")]
    public void TurnPage()
    {
        if (_turning || !isActiveAndEnabled) return;

        StartCoroutine(Turn());
    }

    private IEnumerator Turn()
    {
        _turning = true;

        Material carried = GetSlot(rightPage, rightPageSlot);
        Material next = carried == pageA ? pageB : pageA;

        Transform sheet = flipPage.transform;
        Vector3 restPosition = sheet.position;
        Quaternion restRotation = sheet.rotation;

        SetSlot(flipPage, flipPageSlot, carried);
        flipPage.gameObject.SetActive(true);

        // Changed while the sheet is lying flat on top of it, so the swap itself is
        // never visible — the whole reason the sheet is raised first.
        SetSlot(rightPage, rightPageSlot, next);

        float direction = SweepDirection();
        float clock = 0f;
        float applied = 0f;

        while (clock < turnSeconds)
        {
            clock += Time.deltaTime;

            float angle = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(clock / turnSeconds)) * 180f * direction;
            sheet.RotateAround(hinge.position, hinge.forward, angle - applied);
            applied = angle;

            yield return null;
        }

        SetSlot(leftPage, leftPageSlot, carried);

        // Hidden first, then put back, so the return to the right-hand side happens
        // off screen.
        flipPage.gameObject.SetActive(false);
        sheet.SetPositionAndRotation(restPosition, restRotation);

        _turning = false;
    }

    /// <summary>
    /// Which way round the hinge the sheet should sweep.
    ///
    /// +180 and -180 land in the SAME place, so the endpoint cannot decide this — the
    /// difference is entirely in the path: one arcs up over the book, the other sweeps
    /// down through it and the table. So the midpoints are compared and the one that
    /// rises wins. Measured per turn rather than serialized because it stays correct
    /// if the book is moved or turned around.
    /// </summary>
    private float SweepDirection()
    {
        Vector3 center = flipPage.bounds.center;

        Vector3 up = Swing(center, 90f);
        Vector3 down = Swing(center, -90f);

        return up.y >= down.y ? 1f : -1f;
    }

    private Vector3 Swing(Vector3 point, float degrees)
    {
        return hinge.position + Quaternion.AngleAxis(degrees, hinge.forward) * (point - hinge.position);
    }

    // sharedMaterials, never materials: the latter clones the material on first touch
    // and leaks a new instance on every page turn.
    private static Material GetSlot(Renderer renderer, int slot)
    {
        Material[] materials = renderer.sharedMaterials;

        return slot >= 0 && slot < materials.Length ? materials[slot] : null;
    }

    private static void SetSlot(Renderer renderer, int slot, Material material)
    {
        Material[] materials = renderer.sharedMaterials;

        if (slot < 0 || slot >= materials.Length) return;

        materials[slot] = material;
        renderer.sharedMaterials = materials;
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawHinge || hinge == null) return;

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(hinge.position, 0.02f);
        // The spine line the sheet swings about.
        Gizmos.DrawLine(hinge.position - hinge.forward * 0.12f, hinge.position + hinge.forward * 0.12f);

        if (flipPage == null) return;

        // The arc the sheet's centre will travel, drawn the way it will actually go.
        Vector3 center = flipPage.bounds.center;
        float direction = Application.isPlaying ? 1f : (Swing(center, 90f).y >= Swing(center, -90f).y ? 1f : -1f);

        Gizmos.color = Color.yellow;
        Vector3 previous = center;

        for (int i = 1; i <= 18; i++)
        {
            Vector3 point = Swing(center, i * 10f * direction);
            Gizmos.DrawLine(previous, point);
            previous = point;
        }
    }
}
