using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;

// Stands up the desk routine: the arm IK rig, the hand targets, and the wiring.
//
// It used to do a great deal more — add animator states, add parameters, scan clips
// for a frame to hold, measure the desk. All of that served the computer beat and the
// idea that a clip could place her hands; both are gone (D-049), and so is the code
// for them. What it still does is measure and report, because the numbers are what
// settle where a hand can actually go.
//
// Safe to run more than once: nothing already placed is repositioned.
public static class DeskRoutineSetup
{
    private const string TypingStateName = "Typing";
    private const string ReadingStateName = "Reading";
    private const string BaseLayerName = "Base Layer";
    private const string ArmsLayerName = "Arms";

    private const string TypingClipPath = "Assets/Art/chloe/Generated/Typing.anim";
    private const string ReadingClipPath = "Assets/Art/chloe/Generated/Writing.anim";

    private const string LeftHandBone = "hand.L";
    private const string RightHandBone = "hand.R";

    [MenuItem("Tools/StudyWithMe/Set Up Desk Routine")]
    public static void SetUp()
    {
        CharacterPresenter presenter = Object.FindFirstObjectByType<CharacterPresenter>();

        if (presenter == null)
        {
            Debug.LogError("[DeskRoutine] no CharacterPresenter in the open scene — open Room.unity first.");
            return;
        }

        Animator animator = presenter.animator != null
            ? presenter.animator
            : presenter.GetComponentInChildren<Animator>();

        if (animator == null)
        {
            Debug.LogError("[DeskRoutine] Chloe has no Animator, so her arm bones cannot be found.", presenter);
            return;
        }

        DropRetiredStates(animator);

        ArmIkSolver ik = EnsureArmIk(presenter, animator);
        DeskRoutine routine = WireRoutine(presenter, ik);

        SeedHandTargets(routine);
        ReportReach(ik, routine);

        AssetDatabase.SaveAssets();
        EditorSceneManager.MarkSceneDirty(presenter.gameObject.scene);

        Debug.Log("[DeskRoutine] done. The scene is marked dirty — press Cmd+S to keep it.", presenter);
    }

    /// <summary>
    /// Takes out the animator states this tool added in earlier passes.
    ///
    /// Nothing drives them any more: ArmIkSolver overrides the whole arm chain, so the
    /// masked layer only ever contributed the shoulder, and its own default state gives
    /// that. Leaving them would be dead wiring that still plays.
    ///
    /// Gated the same way it always was — a state goes only if its motion is the exact
    /// clip this tool would have assigned, so nothing hand-made is ever caught. If the
    /// clips are missing the removal is SKIPPED and said out loud, rather than falling
    /// back to matching on name alone.
    /// </summary>
    private static void DropRetiredStates(Animator animator)
    {
        AnimatorController controller = animator.runtimeAnimatorController as AnimatorController;

        if (controller == null) return;

        AnimationClip typing = AssetDatabase.LoadAssetAtPath<AnimationClip>(TypingClipPath);
        AnimationClip reading = AssetDatabase.LoadAssetAtPath<AnimationClip>(ReadingClipPath);

        if (typing == null && reading == null)
        {
            Debug.Log("[DeskRoutine] the Typing/Writing clips are gone, so any leftover states cannot be " +
                      "identified safely and were left alone. Remove them by hand if they are still there.");
            return;
        }

        foreach (AnimatorControllerLayer layer in controller.layers)
        {
            if (layer.name != BaseLayerName && layer.name != ArmsLayerName) continue;

            DropIfOurs(layer.stateMachine, TypingStateName, typing);
            DropIfOurs(layer.stateMachine, ReadingStateName, reading);
        }

        EditorUtility.SetDirty(controller);
    }

    private static void DropIfOurs(AnimatorStateMachine machine, string stateName, AnimationClip clip)
    {
        if (clip == null) return;

        AnimatorState state = FindState(machine, stateName);

        if (state == null) return;

        if (state.motion != clip)
        {
            Debug.LogWarning("[DeskRoutine] '" + stateName + "' plays '" +
                             (state.motion != null ? state.motion.name : "nothing") + "', not '" + clip.name +
                             "', so it was not put there by this tool and has been left alone.");
            return;
        }

        foreach (ChildAnimatorState child in machine.states)
        {
            if (child.state == null || child.state == state) continue;

            foreach (AnimatorStateTransition transition in child.state.transitions)
                if (transition.destinationState == state) child.state.RemoveTransition(transition);
        }

        machine.RemoveState(state);
        Debug.Log("[DeskRoutine] removed the retired '" + stateName + "' state; the arms are IK-driven now.");
    }

    private static AnimatorState FindState(AnimatorStateMachine machine, string name)
    {
        foreach (ChildAnimatorState child in machine.states)
            if (child.state != null && child.state.name == name) return child.state;

        return null;
    }

    private static ArmIkSolver EnsureArmIk(CharacterPresenter presenter, Animator animator)
    {
        GameObject host = presenter.gameObject;
        ArmIkSolver ik = host.GetComponent<ArmIkSolver>();

        if (ik == null)
        {
            ik = Undo.AddComponent<ArmIkSolver>(host);
            Debug.Log("[DeskRoutine] added ArmIkSolver to '" + host.name + "'.", host);
        }

        Undo.RecordObject(ik, "Wire ArmIkSolver");

        ik.characterRoot = presenter.transform;

        // Bone names come straight out of ChloeArmsOnly.mask, the same set the Arms
        // layer is masked to — so the chain being solved is the chain that layer owns,
        // by construction rather than by coincidence.
        Transform root = animator.transform;
        ik.leftArm.upperArm = FindBone(root, "upper_arm.L");
        ik.leftArm.forearm = FindBone(root, "forearm.L");
        ik.leftArm.hand = FindBone(root, LeftHandBone);
        ik.rightArm.upperArm = FindBone(root, "upper_arm.R");
        ik.rightArm.forearm = FindBone(root, "forearm.R");
        ik.rightArm.hand = FindBone(root, RightHandBone);

        if (!ik.leftArm.IsComplete || !ik.rightArm.IsComplete)
            Debug.LogError("[DeskRoutine] could not find all six arm bones (upper_arm/forearm/hand, L and R). " +
                           "Assign them by hand on ArmIkSolver.", ik);

        EditorUtility.SetDirty(ik);
        return ik;
    }

    private static DeskRoutine WireRoutine(CharacterPresenter presenter, ArmIkSolver ik)
    {
        GameObject host = presenter.gameObject;
        DeskRoutine routine = host.GetComponent<DeskRoutine>();

        if (routine == null)
        {
            routine = Undo.AddComponent<DeskRoutine>(host);
            Debug.Log("[DeskRoutine] added DeskRoutine to '" + host.name + "'.", host);
        }

        Undo.RecordObject(routine, "Wire DeskRoutine");

        routine.chloe = presenter;
        routine.armIk = ik;
        routine.book = Object.FindFirstObjectByType<BookPageTurner>();
        routine.danceMode = Object.FindFirstObjectByType<DanceModeController>();
        routine.gameMode = Object.FindFirstObjectByType<GameModeController>();

        // The book itself is what she turns towards, so its own transform is the
        // target: move the book and the angle follows.
        if (routine.bookTarget == null && routine.book != null) routine.bookTarget = routine.book.transform;

        // Who she turns to when spoken to: the room camera, i.e. where the user is.
        // Read off DanceModeController rather than searched for again, the same way
        // game mode reads its cast — one wiring to be wrong instead of two, and the
        // modes cannot end up pointing at different cameras.
        if (routine.conversationTarget == null && routine.danceMode != null && routine.danceMode.mainCamera != null)
            routine.conversationTarget = routine.danceMode.mainCamera.transform;

        if (routine.conversationTarget == null)
            Debug.LogWarning("[DeskRoutine] no conversation target could be resolved; she will " +
                             "return to the authored monitor pose while being talked to instead " +
                             "of turning to the user. Assign it by hand.", routine);

        if (routine.book == null)
            Debug.LogWarning("[DeskRoutine] no BookPageTurner in the scene, so no page will turn. " +
                             "Run Tools > StudyWithMe > Set Up Book Pages.", routine);
        else
        {
            Undo.RecordObject(routine.book, "Hand page turning to the desk routine");
            // The timer inside BookPageTurner was always a stand-in for this (D-038).
            routine.book.selfDriven = false;
            EditorUtility.SetDirty(routine.book);
        }

        EditorUtility.SetDirty(routine);
        return routine;
    }

    /// <summary>
    /// Creates the one hand target, if it is not already there.
    ///
    /// There used to be lap targets for the resting hands as well. They are gone with
    /// the IK that needed them: between page turns the arms come from the animation,
    /// which already looks right, so there is nothing resting to place — and the reach
    /// returns by fading the weight out rather than by aiming at a lap position, which
    /// was the only thing those targets were ever for.
    /// </summary>
    private static void SeedHandTargets(DeskRoutine routine)
    {
        if (routine == null) return;

        if (routine.pageHand == null)
            routine.pageHand = MakeTarget(routine, "HandTarget_Page", routine.bookTarget, new Vector3(0f, 0.05f, 0f));

        EditorUtility.SetDirty(routine);
    }

    private static Transform MakeTarget(DeskRoutine routine, string name, Transform at)
    {
        return MakeTarget(routine, name, at, Vector3.zero);
    }

    private static Transform MakeTarget(DeskRoutine routine, string name, Transform at, Vector3 worldOffset)
    {
        GameObject go = new GameObject(name);
        Undo.RegisterCreatedObjectUndo(go, "Create " + name);
        go.transform.SetParent(routine.transform, true);

        if (at != null)
        {
            go.transform.position = at.position + worldOffset;
            go.transform.rotation = at.rotation;
        }

        Debug.Log("[DeskRoutine] created " + name + " at " + go.transform.position.ToString("F3") +
                  (at == null ? " (nothing to place it on — drag it into place)" : ""), go);

        return go.transform;
    }

    /// <summary>
    /// Whether each target is even reachable. An arm has a fixed length; a target
    /// further from the shoulder than the arm can extend leaves it straight and
    /// pointing, which looks like grasping at air. This is the report that settled the
    /// computer beat — her arm reaches 0.259 m and the keyboard sat 0.70-0.81 m away —
    /// so it stays, in metres, where it can settle the next such question too.
    /// </summary>
    private static void ReportReach(ArmIkSolver ik, DeskRoutine routine)
    {
        if (ik == null || routine == null) return;
        if (!ik.leftArm.IsComplete || !ik.rightArm.IsComplete) return;

        System.Text.StringBuilder report = new System.Text.StringBuilder();
        report.Append("[DeskRoutine] arm reach vs targets. Anything OUT OF REACH has to move in the scene:\n")
              .Append("  left arm reaches ").Append(ik.leftArm.Reach.ToString("F3"))
              .Append(" m,  right arm ").Append(ik.rightArm.Reach.ToString("F3")).Append(" m\n");

        Describe(report, "page (right hand)", ik.rightArm, routine.pageHand);

        Debug.Log(report.ToString(), routine);
    }

    private static void Describe(System.Text.StringBuilder report, string label, ArmIkSolver.Arm arm, Transform target)
    {
        if (target == null)
        {
            report.Append("  ").Append(label).Append(": not assigned\n");
            return;
        }

        float distance = Vector3.Distance(arm.upperArm.position, target.position);
        bool reachable = distance <= arm.Reach;

        report.Append("  ").Append(label).Append(": ").Append(distance.ToString("F3"))
              .Append(" m from the shoulder — ")
              .Append(reachable ? "in reach" : "OUT OF REACH by " + (distance - arm.Reach).ToString("F3") + " m")
              .Append("\n");
    }

    private static Transform FindBone(Transform root, string boneName)
    {
        if (root.name == boneName) return root;

        for (int i = 0; i < root.childCount; i++)
        {
            Transform found = FindBone(root.GetChild(i), boneName);
            if (found != null) return found;
        }

        return null;
    }
}
