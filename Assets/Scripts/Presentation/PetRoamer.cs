using UnityEngine;

// One stop on a pet's route: where it stands, how long it stays, and optionally
// which way it faces once it gets there.
[System.Serializable]
public struct PetStop
{
    public Vector3 position;
    public float stayMinutes;
    // Left off, it keeps the heading it arrived on — which is what an animal
    // walking somewhere actually does, so that stays the default. Turn it on
    // for stops where the facing matters (curled up towards the bed, looking at
    // the desk) and the pet turns to facingYaw after arriving.
    public bool setFacing;
    public float facingYaw;
}

// Walks a pet between authored stops: idle for a while, turn towards the next
// stop, run there, idle again, and wrap around to the first stop at the end.
//
// This component must sit on a PARENT of the animated model, never on the model
// itself. The vendor clips write the animated object's own position to (0,0,0)
// and its rotation to identity on every frame, so a mover on that same object
// would be overwritten every frame — two writers for one value. Splitting them
// gives each a transform of its own: this component owns the parent, the clip
// goes on pinning the child to the parent's origin. (Apply Root Motion does not
// help here: the models have no Root node set and no Avatar, so Unity never
// classifies those curves as root motion in the first place.)
public class PetRoamer : MonoBehaviour
{
    [Header("Wiring")]
    public Animator animator;

    [Header("Route")]
    // Stops are world positions in Room.unity, so they live in the scene rather
    // than in a shared asset — the same numbers would mean nothing elsewhere.
    public PetStop[] route;

    [Header("Motion")]
    public float runSpeed = 1.2f;
    public float turnDegreesPerSecond = 220f;
    // It turns on the spot first and starts running only once it roughly faces
    // the target, which is what keeps it from sliding there sideways.
    public float startRunningWithinDegrees = 25f;
    public float arriveDistance = 0.05f;

    [Header("Animator states")]
    // The vendor controllers carry no parameters and no transitions — just two
    // loose states — so the states are driven by name. The names differ between
    // the two animals ("Cat__Run" really does have two underscores), which is
    // why these are fields rather than constants.
    public string idleStateName = "";
    public string runStateName = "";
    public float animatorCrossFade = 0.15f;

    [Header("Party mode")]
    // While dance mode runs the pet abandons its route and moves around inside
    // this rectangle instead, pausing to watch. The route data is untouched
    // throughout, so leaving party mode simply resumes it.
    //
    // A rectangle rather than a ring, and that is the whole obstacle-avoidance
    // story: a rectangle is convex, so a straight line between any two points
    // inside it also stays inside it. Keep the furniture outside the rectangle
    // and the PATHS avoid the furniture too, without a NavMesh.
    public Vector3 partyAreaCenter = new Vector3(15.90f, 0.127f, 3.80f);
    public Vector2 partyAreaSize = new Vector2(1.90f, 1.70f);
    // Keeps the pet off the spot the watched character is standing on.
    public float partyClearance = 0.9f;
    public float partyStayMin = 1.5f;
    public float partyStayMax = 4f;

    [Header("Gizmos")]
    public bool drawRoute = true;

    private int _index;
    private float _waitClock;
    private bool _moving;
    private bool _statesValid;

    private bool _party;
    private Transform _watch;
    private Vector3 _partyCenter;
    private Vector3 _partyTarget;
    private float _partyStay;

    // Freezes the pet where it stands. Deliberately NOT a separate mode like
    // _party: nothing about the route or the party is unwound, so releasing
    // resumes the exact leg it was on.
    private bool _held;

    private void Start()
    {
        _statesValid = ValidateStates();

        if (route == null || route.Length == 0)
        {
            Debug.LogWarning("[PetRoamer] " + name + ": route is empty, nothing to walk.", this);
            enabled = false;
            return;
        }

        transform.position = route[0].position;
        // Snapped rather than turned into: there is nobody watching frame one.
        if (route[0].setFacing) transform.rotation = Quaternion.Euler(0f, route[0].facingYaw, 0f);
        _index = 0;
        EnterWaiting();
    }

    // A mistyped state name makes CrossFade silently do nothing, which reads as
    // "the animation is broken" rather than "the string is wrong" — so it is
    // checked once and reported loudly instead.
    private bool ValidateStates()
    {
        if (animator == null)
        {
            Debug.LogError("[PetRoamer] " + name + ": animator is not assigned.", this);
            return false;
        }

        bool ok = true;
        if (!HasState(idleStateName))
        {
            Debug.LogError("[PetRoamer] " + name + ": idleStateName '" + idleStateName +
                           "' is not a state on layer 0 of " + animator.runtimeAnimatorController, this);
            ok = false;
        }
        if (!HasState(runStateName))
        {
            Debug.LogError("[PetRoamer] " + name + ": runStateName '" + runStateName +
                           "' is not a state on layer 0 of " + animator.runtimeAnimatorController, this);
            ok = false;
        }
        return ok;
    }

    private bool HasState(string stateName)
    {
        return !string.IsNullOrEmpty(stateName) && animator.HasState(0, Animator.StringToHash(stateName));
    }

    // Called by dance mode, which supplies only the centre to circle and the
    // thing to watch — where exactly the pet goes stays this component's
    // business, so the agent transform keeps a single writer.
    public void EnterParty(Vector3 center, Transform watch)
    {
        if (route == null || route.Length == 0) enabled = true;
        _party = true;
        _watch = watch;
        _partyCenter = center;
        PickPartyTarget();
        EnterMoving();
    }

    public void ExitParty()
    {
        if (!_party) return;
        _party = false;
        _watch = null;
        // Straight back to the stop it was heading for; the route was never
        // altered, so the loop carries on from where it left off.
        if (route != null && route.Length > 0) EnterMoving();
        else EnterWaiting();
    }

    // Called by game mode: stand still, right here, playing idle. The pet is part
    // of the room's stillness while the user is at the TV, so this is the whole
    // requirement — no destination, no facing, no route change.
    //
    // Why a flag over Update rather than reusing EnterWaiting(): EnterWaiting
    // zeroes _waitClock and commits to "I have arrived", which would both restart
    // a stop's dwell time and, mid-leg, throw away the fact that the pet was
    // walking somewhere. Holding the whole Update instead leaves _moving, _index
    // and _waitClock exactly as they were, so Release() carries on rather than
    // starting over.
    public void Hold()
    {
        if (_held) return;
        _held = true;
        Play(idleStateName);
    }

    public void Release()
    {
        if (!_held) return;
        _held = false;
        // Back to the clip that matches the state it was frozen in — walking pets
        // resume running, waiting pets stay idle. Without this a pet held mid-leg
        // would slide to its target with no leg motion.
        Play(_moving ? runStateName : idleStateName);
    }

    private void PickPartyTarget()
    {
        Vector2 half = partyAreaSize * 0.5f;
        Vector2 watched = _watch != null
            ? new Vector2(_watch.position.x, _watch.position.z)
            : new Vector2(_partyCenter.x, _partyCenter.z);
        Vector2 from = new Vector2(transform.position.x, transform.position.z);

        Vector3 best = partyAreaCenter;
        float bestClearance = -1f;

        // Rejection sampling on the PATH, not just the destination: she stands
        // inside the rectangle, so a corner-to-corner run passed within 0.06 m of
        // her even though both endpoints were clear. Bounded at 16 tries, and it
        // keeps the best draw rather than the last, so the worst case is still
        // the furthest-from-her option seen.
        for (int i = 0; i < 16; i++)
        {
            var candidate = new Vector3(
                partyAreaCenter.x + Random.Range(-half.x, half.x),
                // The rectangle's own Y, NOT the pet's current height — taking it
                // from the pet meant that starting the party while it stood on
                // the bed left it circling the floor at bed height.
                partyAreaCenter.y,
                partyAreaCenter.z + Random.Range(-half.y, half.y));

            float clearance = DistanceToSegment(watched, from, new Vector2(candidate.x, candidate.z));
            if (clearance > bestClearance) { bestClearance = clearance; best = candidate; }
            if (clearance >= partyClearance) break;
        }

        _partyTarget = best;
        _partyStay = Random.Range(partyStayMin, partyStayMax);
    }

    private static float DistanceToSegment(Vector2 point, Vector2 a, Vector2 b)
    {
        Vector2 ab = b - a;
        float lengthSq = ab.sqrMagnitude;
        if (lengthSq < 0.000001f) return Vector2.Distance(point, a);

        float t = Mathf.Clamp01(Vector2.Dot(point - a, ab) / lengthSq);
        return Vector2.Distance(point, a + ab * t);
    }

    private void Update()
    {
        // Before both ticks, so a held pet neither moves nor advances its dwell
        // clock — "exactly where it is now" has to survive a long game session.
        if (_held) return;

        if (_moving) TickMoving();
        else TickWaiting();
    }

    private void TickWaiting()
    {
        if (_party)
        {
            TurnTowards(_watch != null ? _watch.position : _partyCenter);
            _waitClock += Time.deltaTime;
            if (_waitClock < _partyStay) return;
            PickPartyTarget();
            EnterMoving();
            return;
        }

        // Before the length check: a pet parked on a one-stop route still gets
        // to face where it was told to.
        ApplyStopFacing();

        // A one-stop route is a legitimate way to say "just stand here".
        if (route.Length < 2) return;

        _waitClock += Time.deltaTime;
        if (_waitClock < route[_index].stayMinutes * 60f) return;

        _index = (_index + 1) % route.Length;
        EnterMoving();
    }

    private void TurnTowards(Vector3 worldPoint)
    {
        Vector3 flat = worldPoint - transform.position;
        flat.y = 0f;
        if (flat.sqrMagnitude < 0.000001f) return;

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation, Quaternion.LookRotation(flat, Vector3.up),
            turnDegreesPerSecond * Time.deltaTime);
    }

    // Runs alongside the wait clock rather than gating it, so stayMinutes keeps
    // meaning "how long it stands here" instead of "how long after it finishes
    // turning".
    private void ApplyStopFacing()
    {
        if (!route[_index].setFacing) return;

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            Quaternion.Euler(0f, route[_index].facingYaw, 0f),
            turnDegreesPerSecond * Time.deltaTime);
    }

    private void TickMoving()
    {
        Vector3 target = _party ? _partyTarget : route[_index].position;

        Vector3 flat = target - transform.position;
        flat.y = 0f;
        if (flat.sqrMagnitude > 0.000001f)
        {
            Quaternion want = Quaternion.LookRotation(flat, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, want, turnDegreesPerSecond * Time.deltaTime);

            if (Quaternion.Angle(transform.rotation, want) > startRunningWithinDegrees) return;
        }

        transform.position = Vector3.MoveTowards(transform.position, target, runSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) <= arriveDistance)
        {
            transform.position = target;
            EnterWaiting();
        }
    }

    private void EnterWaiting()
    {
        _moving = false;
        _waitClock = 0f;
        Play(idleStateName);
    }

    private void EnterMoving()
    {
        _moving = true;
        Play(runStateName);
    }

    private void Play(string stateName)
    {
        if (!_statesValid) return;
        // Fixed-time, not normalized: the two clips are 1.33 s and 0.33 s long,
        // so a normalized duration would mean a different blend each way.
        animator.CrossFadeInFixedTime(stateName, animatorCrossFade, 0);
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawRoute || route == null || route.Length == 0) return;

        for (int i = 0; i < route.Length; i++)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(route[i].position, 0.12f);
            // the closing leg back to stop 0 is drawn too — the route loops
            if (route.Length > 1) Gizmos.DrawLine(route[i].position, route[(i + 1) % route.Length].position);

            if (!route[i].setFacing) continue;
            // yellow stub = the authored facing, so the yaw can be typed and
            // checked in the Scene view instead of guessed and play-tested
            Gizmos.color = Color.yellow;
            Vector3 look = Quaternion.Euler(0f, route[i].facingYaw, 0f) * Vector3.forward;
            Gizmos.DrawLine(route[i].position, route[i].position + look * 0.45f);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(route[0].position, 0.06f);

        // The party rectangle, drawn so a bad one is visible in the Scene view
        // rather than only in Play: every furniture piece must sit OUTSIDE it.
        Gizmos.color = new Color(1f, 0.4f, 0.9f, 1f);
        Gizmos.DrawWireCube(partyAreaCenter, new Vector3(partyAreaSize.x, 0.02f, partyAreaSize.y));
    }
}
