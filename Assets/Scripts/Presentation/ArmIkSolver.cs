using UnityEngine;

// Puts her hands where we say, instead of where the clip happens to leave them.
//
// Why this exists at all: three rounds went into hunting for a clip frame whose hands
// landed on the keyboard, and the measurement finally settled it — the typing clip is
// 0.50 m short across all 17.6 seconds of itself. Choosing frames could never have
// worked. Solving to a target ends that whole class of problem, and it is also the
// only thing that can rotate the WRIST, which no amount of frame-picking addresses.
//
// Analytic two-bone IK, not a package: the rig is Generic (animationType 2), so
// Unity's humanoid IK is unavailable, and the closed-form solution for two joints is
// short enough to read in one sitting. The chain the mask already defines is exactly
// two joints and a tip — upper_arm -> forearm -> hand.
//
// The writer chain on those six bones is: the Animator writes them, then this
// overrides. Same shape as CharacterPresenter overriding the ROOT, in LateUpdate for
// the same reason — the Animator has finished by then. It is the ONLY thing that
// overrides them, and it never touches the root, so the two do not overlap.
[DefaultExecutionOrder(100)]
public class ArmIkSolver : MonoBehaviour
{
    [System.Serializable]
    public class Arm
    {
        public Transform upperArm;
        public Transform forearm;
        public Transform hand;
        // Which way the elbow should break, in the character's own frame. Down and
        // back is what a seated person's elbow does; get it wrong and the elbow bends
        // the wrong way, which is why the gizmo draws it.
        public Vector3 elbowHintLocal = new Vector3(0f, -0.6f, -0.6f);

        public bool IsComplete { get { return upperArm != null && forearm != null && hand != null; } }

        /// <summary>Shoulder-to-wrist distance when the arm is straight. The tool
        /// reports it so a target beyond it is caught as a number rather than as a
        /// strange-looking shoulder.</summary>
        public float Reach
        {
            get
            {
                if (!IsComplete) return 0f;

                return Vector3.Distance(upperArm.position, forearm.position)
                     + Vector3.Distance(forearm.position, hand.position);
            }
        }
    }

    [Header("Chain")]
    // Only used as the frame the elbow hint is expressed in, so that hint keeps
    // meaning "down and back" after she turns.
    public Transform characterRoot;
    public Arm leftArm = new Arm();
    public Arm rightArm = new Arm();

    [Header("Where the hands go")]
    // Written by DeskRoutine as the beat changes. Null on a side leaves that arm to
    // the animation, which is what should happen when nothing is being reached for.
    public Transform leftTarget;
    public Transform rightTarget;
    // Blended rather than switched: snapping from animation to a solved pose reads as
    // the hands teleporting.
    [Range(0f, 1f)] public float weight;

    [Header("Gizmos")]
    public bool drawGizmos = true;

    private void LateUpdate()
    {
        // Costs nothing when nobody is reaching for anything.
        if (weight <= 0f) return;

        Apply(leftArm, leftTarget);
        Apply(rightArm, rightTarget);
    }

    private void Apply(Arm arm, Transform target)
    {
        if (target == null || !arm.IsComplete) return;

        // Blended in LOCAL space: the three bones are a parent chain, so saving world
        // rotations and slerping them back would fight itself as each parent moves.
        Quaternion upperAnimated = arm.upperArm.localRotation;
        Quaternion foreAnimated = arm.forearm.localRotation;
        Quaternion handAnimated = arm.hand.localRotation;

        Solve(arm, target);

        if (weight >= 1f) return;

        arm.upperArm.localRotation = Quaternion.Slerp(upperAnimated, arm.upperArm.localRotation, weight);
        arm.forearm.localRotation = Quaternion.Slerp(foreAnimated, arm.forearm.localRotation, weight);
        arm.hand.localRotation = Quaternion.Slerp(handAnimated, arm.hand.localRotation, weight);
    }

    /// <summary>
    /// Closed-form two-joint IK. Three rotations, no iteration: open or close the
    /// elbow to the angle the triangle needs, correct the shoulder by the matching
    /// amount, then swing the whole straightened chain onto the target.
    ///
    /// The reach is CLAMPED rather than allowed to fail: a target further than the arm
    /// is long leaves the arm straight and pointing at it, which looks like reaching
    /// for something out of range — the honest result, and the setup tool reports the
    /// distance so it can be fixed in the scene rather than hidden here.
    /// </summary>
    private void Solve(Arm arm, Transform target)
    {
        Vector3 a = arm.upperArm.position;
        Vector3 b = arm.forearm.position;
        Vector3 c = arm.hand.position;

        Vector3 ab = b - a;
        Vector3 cb = b - c;
        Vector3 ac = c - a;
        Vector3 at = target.position - a;

        float lengthAb = ab.magnitude;
        float lengthCb = cb.magnitude;

        if (lengthAb <= 0.0001f || lengthCb <= 0.0001f) return;

        float lengthAt = Mathf.Clamp(at.magnitude, 0.001f, lengthAb + lengthCb - 0.001f);

        // Angles the pose has now.
        float acAbNow = Angle(ac, ab);
        float baBcNow = Angle(a - b, c - b);
        float acAtNow = Angle(ac, at);

        // Angles the triangle needs, by the law of cosines.
        float acAbWanted = Mathf.Acos(Mathf.Clamp(
            (lengthCb * lengthCb - lengthAb * lengthAb - lengthAt * lengthAt) / (-2f * lengthAb * lengthAt), -1f, 1f));
        float baBcWanted = Mathf.Acos(Mathf.Clamp(
            (lengthAt * lengthAt - lengthAb * lengthAb - lengthCb * lengthCb) / (-2f * lengthAb * lengthCb), -1f, 1f));

        Vector3 hint = characterRoot != null
            ? a + characterRoot.rotation * arm.elbowHintLocal
            : a + arm.elbowHintLocal;

        // The axis the elbow swings about comes from the HINT, so which way the joint
        // breaks is authored rather than left to whatever the numbers prefer.
        Vector3 bendAxis = Vector3.Cross(ac, hint - a);
        if (bendAxis.sqrMagnitude < 0.000001f) bendAxis = Vector3.Cross(ac, Vector3.up);
        bendAxis = bendAxis.normalized;

        Vector3 swingAxis = Vector3.Cross(ac, at);
        if (swingAxis.sqrMagnitude < 0.000001f) swingAxis = bendAxis;
        swingAxis = swingAxis.normalized;

        Quaternion shoulderBend = Quaternion.AngleAxis((acAbWanted - acAbNow) * Mathf.Rad2Deg, bendAxis);
        Quaternion elbowBend = Quaternion.AngleAxis((baBcWanted - baBcNow) * Mathf.Rad2Deg, bendAxis);
        Quaternion swingOntoTarget = Quaternion.AngleAxis(acAtNow * Mathf.Rad2Deg, swingAxis);

        arm.upperArm.rotation = swingOntoTarget * shoulderBend * arm.upperArm.rotation;
        arm.forearm.rotation = elbowBend * arm.forearm.rotation;

        // The wrist. This is the part no clip choice could ever have fixed: the hand
        // takes the target's rotation, so a hand on the keyboard lies flat on it and a
        // hand on the book turns a page the way the target is turned.
        arm.hand.rotation = target.rotation;
    }

    private static float Angle(Vector3 from, Vector3 to)
    {
        if (from.sqrMagnitude < 0.000001f || to.sqrMagnitude < 0.000001f) return 0f;

        return Mathf.Acos(Mathf.Clamp(Vector3.Dot(from.normalized, to.normalized), -1f, 1f));
    }

    /// <summary>
    /// Prints the targets' world transforms to the console.
    ///
    /// The only practical way to keep a Transform tuned during Play: scene changes are
    /// thrown away on exit, but the console survives it, so the numbers can be copied
    /// back afterwards. (Components have Copy/Paste Component Values; Transforms
    /// dragged in the Scene view do not.)
    /// </summary>
    [ContextMenu("Log Hand Targets")]
    public void LogHandTargets()
    {
        LogTarget("left", leftTarget, leftArm);
        LogTarget("right", rightTarget, rightArm);
    }

    private void LogTarget(string label, Transform target, Arm arm)
    {
        if (target == null)
        {
            Debug.Log("[ArmIk] " + label + " target: none", this);
            return;
        }

        string reach = arm.IsComplete
            ? "   arm reach " + arm.Reach.ToString("F3") + " m, target " +
              Vector3.Distance(arm.upperArm.position, target.position).ToString("F3") + " m from the shoulder"
            : "";

        Debug.Log("[ArmIk] " + label + " target '" + target.name + "'  position " +
                  target.position.ToString("F4") + "  euler " + target.rotation.eulerAngles.ToString("F2") + reach, target);
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;

        DrawArm(leftArm, leftTarget);
        DrawArm(rightArm, rightTarget);
    }

    private void DrawArm(Arm arm, Transform target)
    {
        if (!arm.IsComplete) return;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(arm.upperArm.position, arm.forearm.position);
        Gizmos.DrawLine(arm.forearm.position, arm.hand.position);

        // The elbow hint, so a joint bending the wrong way can be seen rather than
        // guessed at.
        Vector3 hint = characterRoot != null
            ? arm.upperArm.position + characterRoot.rotation * arm.elbowHintLocal
            : arm.upperArm.position + arm.elbowHintLocal;

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(arm.upperArm.position, hint);

        if (target == null) return;

        // Reach as a sphere: a target outside it cannot be met, and that is a scene
        // problem rather than a solver one.
        Gizmos.color = new Color(0f, 1f, 1f, 0.25f);
        Gizmos.DrawWireSphere(arm.upperArm.position, arm.Reach);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(arm.hand.position, target.position);
        Gizmos.DrawWireCube(target.position, new Vector3(0.03f, 0.03f, 0.03f));
    }
}
