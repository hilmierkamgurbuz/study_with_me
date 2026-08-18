using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// Builds game mode's scene object in Room.unity and wires it up. A menu item
// rather than hand-edited YAML for the reason scene-structure.md gives: the Editor
// holds the scene in memory and writes it back on save, so an outside edit is lost,
// and inventing fileIDs by hand risks corrupting it.
//
// Safe to run more than once: everything is looked up by name first and only created
// when missing, and references are re-resolved on every run.
public static class GameModeSceneSetup
{
    private const string RoomScenePath = "Assets/Scenes/Room.unity";
    private const string ModeObjectName = "--GameMode--";
    private const string GameCameraName = "camera_game";
    private const string ZoomCameraName = "camera_game_zoom";
    private const string TvObjectName = "TV_Set";

    [MenuItem("Tools/StudyWithMe/Set Up Game Mode")]
    public static void SetUp()
    {
        Scene scene = SceneManager.GetActiveScene();

        if (scene.path != RoomScenePath)
        {
            Debug.LogError("[GameModeSetup] open " + RoomScenePath + " first — the active scene is '" +
                           (string.IsNullOrEmpty(scene.path) ? "(unsaved)" : scene.path) + "'.");
            return;
        }

        GameModeController mode = FindOrCreateMode();

        if (!WireCast(mode)) return;

        EditorUtility.SetDirty(mode);
        EditorSceneManager.MarkSceneDirty(scene);

        Debug.Log("[GameModeSetup] " + ModeObjectName + " is in place and wired. Frame the couch " +
                  "pose and the push-in with the gizmos on " + ModeObjectName + ", then save the " +
                  "scene. There is no game-mode UI to build any more: the way in and the way out " +
                  "are both the conversation (D-054).", mode);
    }

    private static GameModeController FindOrCreateMode()
    {
        GameModeController existing = Object.FindFirstObjectByType<GameModeController>();

        if (existing != null) return existing;

        GameObject go = new GameObject(ModeObjectName);
        Undo.RegisterCreatedObjectUndo(go, "Create " + ModeObjectName);

        return go.AddComponent<GameModeController>();
    }

    /// <summary>
    /// Resolves the cast from what is already in the scene. Dance mode has the same
    /// character, animator, main camera and pets wired correctly, so those are READ
    /// OFF IT rather than searched for again — one wiring to be wrong instead of two,
    /// and the two modes cannot end up pointing at different objects.
    /// </summary>
    private static bool WireCast(GameModeController mode)
    {
        DanceModeController dance = Object.FindFirstObjectByType<DanceModeController>();

        if (dance == null)
        {
            Debug.LogError("[GameModeSetup] no DanceModeController in the scene; game mode reads " +
                           "its cast from it. Is this the right scene?");
            return false;
        }

        mode.danceMode = dance;
        mode.chloe = dance.chloe != null ? dance.chloe : Object.FindFirstObjectByType<CharacterPresenter>();
        mode.chloeAnimator = dance.chloeAnimator;
        mode.mainCamera = dance.mainCamera;
        mode.pets = dance.partyPets;
        mode.gameCamera = FindCameraNamed(GameCameraName);
        mode.zoomCamera = FindOrCreateZoomCamera();

        // roomUi is deliberately NOT written any more. It used to be filled with the
        // dance canvas, and with the mode buttons gone (D-054) the room has no canvas
        // left to hide — the only thing on screen during the game is the push-to-talk
        // harness, which is IMGUI and must stay, since talking is now the way out.
        // The field is left as authored; SetRoomUiVisible is null-safe either way.

        bool ok = true;
        ok &= Require(mode.chloe, "chloe (CharacterPresenter)");
        ok &= Require(mode.chloeAnimator, "chloeAnimator");
        ok &= Require(mode.mainCamera, "mainCamera");
        ok &= Require(mode.gameCamera, GameCameraName);
        ok &= Require(mode.zoomCamera, ZoomCameraName);

        if (mode.pets == null || mode.pets.Length == 0)
            Debug.LogWarning("[GameModeSetup] no pets came off DanceModeController.partyPets — " +
                             "the cat and dog will keep roaming during game mode.");

        return ok;
    }

    // By name, because there is nothing else to tell the room's cameras apart: they
    // are all plain Cameras and which one frames what is the author's choice, not a
    // property of the component. Case-insensitive so 'Camera_game' also matches, and
    // inactive ones are included because the zoom camera is deliberately disabled.
    private static Camera FindCameraNamed(string cameraName)
    {
        Camera[] cameras = Object.FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        for (int i = 0; i < cameras.Length; i++)
            if (string.Equals(cameras[i].name, cameraName, System.StringComparison.OrdinalIgnoreCase))
                return cameras[i];

        return null;
    }

    /// <summary>
    /// The camera the push-in ends on. Created ALREADY POINTED AT THE TV, so framing
    /// it is a nudge with the move gizmo while watching the camera preview, not six
    /// coordinates typed in blind.
    ///
    /// An existing one is never repositioned: once it has been framed by hand, that
    /// pose IS the authored shot, and a re-run of this tool must not throw it away.
    ///
    /// The starting pose is derived from the TV's own transform rather than from
    /// world constants — 45 cm out along the face it points with, 70 cm up from its
    /// base — so it lands correctly even if the TV is moved or turned first.
    /// </summary>
    private static Camera FindOrCreateZoomCamera()
    {
        Camera existing = FindCameraNamed(ZoomCameraName);

        if (existing != null)
        {
            existing.enabled = false;
            return existing;
        }

        GameObject go = new GameObject(ZoomCameraName);
        Undo.RegisterCreatedObjectUndo(go, "Create " + ZoomCameraName);

        Camera cam = go.AddComponent<Camera>();
        // Never renders; it exists to be looked through in the Scene view. AddComponent
        // adds no AudioListener, unlike the GameObject > Camera menu item, so there is
        // no second listener to fight the main one.
        cam.enabled = false;
        cam.fieldOfView = 30f;
        // The scene default of 0.3 would slice the screen away before the camera ever
        // reached it; the whole point of this shot is to finish against the glass.
        cam.nearClipPlane = 0.05f;
        cam.farClipPlane = 10f;

        GameObject tv = GameObject.Find(TvObjectName);

        if (tv != null)
        {
            go.transform.position = tv.transform.position + tv.transform.forward * 0.45f + Vector3.up * 0.70f;
            go.transform.rotation = Quaternion.LookRotation(-tv.transform.forward, Vector3.up);
        }
        else
        {
            Debug.LogWarning("[GameModeSetup] no '" + TvObjectName + "' in the scene, so " + ZoomCameraName +
                             " starts at the origin — drag it onto the TV screen yourself.");
        }

        return cam;
    }

    private static bool Require(Object value, string label)
    {
        if (value != null) return true;

        Debug.LogError("[GameModeSetup] could not resolve '" + label + "'; assign it by hand on " + ModeObjectName + ".");
        return false;
    }

}
