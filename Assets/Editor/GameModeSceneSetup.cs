using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Builds game mode's two scene objects in Room.unity and wires them up. A menu item
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
    private const string UiObjectName = "--GameUI--";
    private const string GameCameraName = "camera_game";
    private const string ZoomCameraName = "camera_game_zoom";
    private const string TvObjectName = "TV_Set";

    // The vendored game's own canvases top out at 201, so the way out sits well
    // clear of them. Below that and the back button ends up under the game's HUD.
    private const int UiSortingOrder = 500;

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
        GameModeButton button = FindOrCreateUi();

        if (!WireCast(mode)) return;

        button.gameMode = mode;

        EditorUtility.SetDirty(mode);
        EditorUtility.SetDirty(button);
        EditorSceneManager.MarkSceneDirty(scene);

        Debug.Log("[GameModeSetup] " + ModeObjectName + " and " + UiObjectName +
                  " are in place and wired. Frame the couch pose and the push-in with the " +
                  "gizmos on " + ModeObjectName + ", drag the two buttons where you want them, " +
                  "then save the scene.", mode);
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

        // The dance canvas is the thing that has to get out of the way while the game
        // owns the screen; the game-mode canvas stays, because it carries the way back.
        DanceModeButton danceUi = Object.FindFirstObjectByType<DanceModeButton>();
        mode.roomUi = danceUi != null
            ? new[] { danceUi.gameObject }
            : new GameObject[0];

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

    private static GameModeButton FindOrCreateUi()
    {
        GameModeButton existing = Object.FindFirstObjectByType<GameModeButton>();

        if (existing != null && existing.startButton != null && existing.backButton != null)
        {
            // Re-run on a complete setup: keep the buttons where they were dragged,
            // only make sure the canvas still outranks the game's.
            Canvas c = existing.GetComponent<Canvas>();
            if (c != null) c.sortingOrder = UiSortingOrder;
            return existing;
        }

        // Half-built, from a run that threw partway through. Keeping it would be worse
        // than rebuilding: the object exists, so a re-run would "find" it and skip
        // creation forever, leaving a canvas with no way out on it.
        if (existing != null)
        {
            Debug.LogWarning("[GameModeSetup] " + UiObjectName + " was incomplete (a previous run " +
                             "did not finish); rebuilding it.", existing);
            Undo.DestroyObjectImmediate(existing.gameObject);
        }

        Font font = ResolveUiFont();

        GameObject root = new GameObject(UiObjectName, typeof(RectTransform));
        Undo.RegisterCreatedObjectUndo(root, "Create " + UiObjectName);

        Canvas canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = UiSortingOrder;

        CanvasScaler scaler = root.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        root.AddComponent<GraphicRaycaster>();

        GameModeButton button = root.AddComponent<GameModeButton>();

        button.startButton = CreateButton(root.transform, "GameModeButton", "Oyun Oyna", font,
                                          new Vector2(1f, 1f), new Vector2(-24f, -104f), out Text startLabel);
        button.startLabel = startLabel;

        // Measured against the dance button rather than guessed at: a hardcoded
        // corner is exactly how the first attempt landed on top of it.
        StackUnderDanceButton((RectTransform)button.startButton.transform);

        // Bottom-left: the game is portrait and centred, so on any wide screen this
        // corner is background rather than gameplay.
        button.backButton = CreateButton(root.transform, "BackToRoomButton", "Odaya dön", font,
                                         new Vector2(0f, 0f), new Vector2(120f, 44f), out Text _);
        button.backButton.gameObject.SetActive(false);

        return button;
    }

    /// <summary>
    /// Copies the dance button's anchor, pivot and size, then drops one height below
    /// it. Both are the same kind of thing and now read as a stack rather than two
    /// buttons that happen to share a corner — and, unlike a hardcoded position, this
    /// still holds after the dance button is dragged somewhere else.
    /// </summary>
    private static void StackUnderDanceButton(RectTransform rect)
    {
        DanceModeButton dance = Object.FindFirstObjectByType<DanceModeButton>();

        RectTransform other = dance != null && dance.button != null
            ? dance.button.transform as RectTransform
            : null;

        if (other == null)
        {
            Debug.LogWarning("[GameModeSetup] no dance button to measure against; the game-mode " +
                             "button is at a default corner and may need dragging.");
            return;
        }

        rect.anchorMin = other.anchorMin;
        rect.anchorMax = other.anchorMax;
        rect.pivot = other.pivot;
        rect.sizeDelta = other.sizeDelta;

        const float gap = 12f;
        rect.anchoredPosition = other.anchoredPosition - new Vector2(0f, other.sizeDelta.y + gap);
    }

    /// <summary>
    /// The dance button's own label font, and only if that fails a built-in one.
    ///
    /// Copying what is already working in this scene beats naming a built-in font:
    /// the built-in name is a moving target (Unity 6 retired Arial.ttf for
    /// LegacyRuntime.ttf), and fonts come from Resources.GetBuiltinResource — NOT
    /// AssetDatabase.GetBuiltinExtraResource, which serves a different resource file
    /// and fails on every font name you hand it. Reusing the existing label also
    /// keeps the two buttons looking like the same app.
    /// </summary>
    private static Font ResolveUiFont()
    {
        DanceModeButton dance = Object.FindFirstObjectByType<DanceModeButton>();

        if (dance != null && dance.label != null && dance.label.font != null) return dance.label.font;

        Font builtin = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        if (builtin == null)
            Debug.LogWarning("[GameModeSetup] no font could be resolved; the buttons will have " +
                             "labels with no font set. Assign one on their Label objects.");

        return builtin;
    }

    // Legacy UnityEngine.UI.Text, not TextMeshPro, and deliberately: same call
    // DanceModeButton made — one line of label text is not worth a TMP dependency,
    // and swapping it later is an Inspector change.
    private static Button CreateButton(Transform parent, string name, string label, Font font,
                                       Vector2 anchor, Vector2 position, out Text text)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);

        RectTransform rect = (RectTransform)go.transform;
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(200f, 56f);
        rect.anchoredPosition = position;

        go.AddComponent<CanvasRenderer>();
        Image image = go.AddComponent<Image>();
        image.color = new Color(0.12f, 0.12f, 0.14f, 0.85f);

        Button button = go.AddComponent<Button>();
        button.targetGraphic = image;

        GameObject labelGo = new GameObject("Label", typeof(RectTransform));
        labelGo.transform.SetParent(go.transform, false);

        RectTransform labelRect = (RectTransform)labelGo.transform;
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        labelGo.AddComponent<CanvasRenderer>();
        text = labelGo.AddComponent<Text>();
        text.text = label;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.fontSize = 22;
        text.font = font;

        return button;
    }
}
