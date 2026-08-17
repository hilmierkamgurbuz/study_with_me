using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Runs game mode on one clock, the same shape as DanceModeController: she moves to
// the couch facing the TV, the room settles, the camera pushes in on the screen, and
// the vendored Fruit Merge scene takes the screen over. Leaving unwinds all of it.
//
// Like dance mode it never writes Chloe's transform directly — CharacterPresenter
// rebuilds her root every frame, so placement goes through SetRestingPose.
//
// The one thing it owns outright is camera_game's transform, field of view and near
// clip. The authored pose is read when the mode starts and is the pose everything
// lerps FROM and back TO, so framing the shot stays an Inspector job rather than a
// constant in here.
public class GameModeController : MonoBehaviour
{
    private enum Phase { Off, Watching, Zooming, Playing, Returning }

    [Header("Cast")]
    public CharacterPresenter chloe;
    public Animator chloeAnimator;
    public Camera mainCamera;
    public Camera gameCamera;
    // They stop where they are and play idle for as long as this runs — the room
    // going quiet is the point, so nothing here picks a destination for them.
    public PetRoamer[] pets;
    // Read-only, and only to refuse: the two modes fight over the same camera,
    // character and pets, so they may never overlap. The arrow is one-way — dance
    // mode knows nothing about game mode, and while the game is on screen the room
    // UI (including the dance button) is hidden anyway.
    public DanceModeController danceMode;

    [Header("Room UI hidden while the game has the screen")]
    // NOT the game-mode canvas itself: that one carries the way out.
    public GameObject[] roomUi;

    [Header("Where she sits")]
    // Derived from her desk pose rather than eyeballed: measured against her desk
    // chair she sits 5.6 cm to its right, 2.4 cm up and 1.7 cm forward, and the TV
    // couch is the same chair prefab, so the same local offset carries over. Her
    // desk yaw offset (-15.6 deg) deliberately did NOT carry over — that one exists
    // because she is angled at her monitor, and here the TV is already dead ahead.
    public Vector3 couchPosition = new Vector3(14.416f, 0.123f, 4.168f);
    public float couchYaw = -90f;

    [Header("Push in on the TV")]
    // A second, permanently DISABLED camera, placed the same way camera_game was:
    // drag it in the Scene view and read its camera preview until the TV screen fills
    // the frame. The push-in ends exactly on its pose — position, rotation, field of
    // view AND near clip all come from it, so there is no number here to guess at.
    //
    // A camera rather than a position + look-at pair precisely because of that
    // preview: three coordinates and three more for a target is six values with no
    // feedback until Play, which is not authoring, it is trial and error.
    //
    // Its near clip matters as much as its position: the scene default is 0.3, which
    // slices the screen away well before the camera reaches it. The setup tool ships
    // it at 0.05 so the move can finish against the glass.
    public Camera zoomCamera;
    // She sits between camera_game and the TV, so a straight line would fly through
    // her head. A half-sine lift over the move carries the camera well above her and
    // settles level again at the screen.
    public float zoomArcHeight = 0.9f;

    [Header("Timing (seconds)")]
    public float zoomStartsAfter = 3f;
    public float zoomSeconds = 1.5f;

    [Header("How the game is framed on screen")]
    // The game is authored portrait. Handed a landscape screen it does not break so
    // much as spread: the board keeps its world size while the HUD stretches to the
    // far corners. Holding it to this shape and painting the leftovers keeps it the
    // game it was designed as.
    public Vector2 playResolution = new Vector2(1080f, 1920f);
    // The game's own camera background, so the bars read as part of it.
    public Color letterboxColor = new Color(0.192f, 0.302f, 0.475f, 1f);
    // Added to every sorting canvas in the loaded game. Needed because binding a
    // canvas to a camera drops it into the SAME sort as the sprites, where its
    // authored order was never meant to compete: the game's canvases sit at 0-3
    // while fruits take 100 - tier (90..101) and its particles reach 201, so the
    // pause and result panels came up behind the fruit. As an Overlay canvas none of
    // that mattered, because Overlay always draws last. 1000 clears the lot with
    // room to spare, and adding rather than assigning keeps the panels' order
    // relative to each other exactly as authored.
    public int canvasSortingBoost = 1000;

    [Header("The game")]
    public string gameScenePath = "Assets/FruitMerge/Scenes/Game.unity";
    // The layer the room is moved onto so the game's camera can cull it away.
    public string roomLayerName = "Room";

    [Header("Gizmos")]
    public bool drawFraming = true;

    private Phase _phase;
    private float _clock;

    private Vector3 _restPosition;
    private Quaternion _restRotation;
    private float _restFov;
    private float _restNearClip;
    private Vector3 _deskPosition;
    private Quaternion _deskRotation;

    private AsyncOperation _load;
    private Scene _gameScene;
    private int _roomLayer = -1;

    // The handover spans frames, and Esc works during it. Without these two the way
    // out could start a teardown while the handover was still standing things up —
    // unloading a scene the next line then sets active. Leaving is therefore QUEUED
    // while a handover is in flight, and taken the moment it lands.
    private bool _handingOver;
    private bool _leaveRequested;

    // How far into the push-in the camera currently is. The unwind reads it rather
    // than assuming 1: leaving during the three-second wait is normal, and unwinding
    // from 1 would snap the camera onto the TV before pulling it back out.
    private float _framing;

    private Camera[] _gameCameras;
    private Camera _letterbox;
    private int _appliedWidth;
    private int _appliedHeight;

    public bool IsRunning { get { return _phase != Phase.Off; } }

    // False during the unwind: by then the way out has already been taken, and a
    // second press would start a return inside a return.
    public bool CanLeave { get { return _phase != Phase.Off && _phase != Phase.Returning; } }

    private void Start()
    {
        if (chloe == null || gameCamera == null)
        {
            Debug.LogError("[GameMode] chloe and gameCamera must both be assigned.", this);
            enabled = false;
            return;
        }

        _restPosition = gameCamera.transform.position;
        _restRotation = gameCamera.transform.rotation;
        _restFov = gameCamera.fieldOfView;
        _restNearClip = gameCamera.nearClipPlane;

        if (zoomCamera == null)
            Debug.LogError("[GameMode] zoomCamera is not assigned — run Tools > StudyWithMe > " +
                           "Set Up Game Mode. The game will still open, but the camera will not " +
                           "push in on the TV.", this);
        else
            // It exists to be framed, never to render: a live second camera would
            // draw over the room, and its AudioListener would fight the main one.
            SetCameraLive(zoomCamera, false);

        // camera_game ships enabled in the scene, exactly like the dance camera did.
        // The resting state is defined here rather than trusted from the scene.
        ShowRoomCamera(mainCamera);

        MoveRoomOntoItsOwnLayer();
    }

    /// <summary>
    /// Puts the whole room on its own layer, once, so the game's camera can cull it.
    ///
    /// Why this is needed at all: the Fruit Merge camera is orthographic, sits at
    /// world (0, 0.5, -10) and sizes itself to the aspect. At 16:9 it sees x in
    /// -9.8..9.8 while the room starts around x 11 — no overlap. At 21:9 its
    /// half-width grows to 12.8 and it reaches into the room's TV corner, and
    /// because the room's geometry is nearer the camera than the game's background
    /// sprite it wins the depth test and shows through.
    ///
    /// Why the ROOM moves and not the game: the game spawns objects at runtime on
    /// the default layer (worm-boost cursors, combo popups), so culling the default
    /// layer away from its camera would delete parts of the game. The room is
    /// static, so it is the side that can safely be relabelled.
    ///
    /// Why it is never restored: the room's own cameras cull nothing, so the room
    /// still sees itself, and nothing in this project raycasts against a layer mask.
    /// Doing it here rather than baking it into Room.unity keeps a 171-object diff
    /// out of a hand-built scene and means anything added to the room later is
    /// covered without re-running a tool.
    /// </summary>
    private void MoveRoomOntoItsOwnLayer()
    {
        _roomLayer = LayerMask.NameToLayer(roomLayerName);

        if (_roomLayer < 0)
        {
            Debug.LogError("[GameMode] there is no layer called '" + roomLayerName +
                           "'; run Tools > Fruit Merge > Apply Import Settings. The game's " +
                           "camera will show room geometry on wide screens.", this);
            return;
        }

        GameObject[] roots = gameObject.scene.GetRootGameObjects();

        for (int i = 0; i < roots.Length; i++) SetLayerRecursively(roots[i].transform);
    }

    // Objects already on the UI layer are left alone: a Screen Space Overlay canvas
    // is not culled by any camera mask, so moving it would buy nothing and would
    // change a layer someone may have set on purpose.
    private void SetLayerRecursively(Transform t)
    {
        if (t.gameObject.layer == 0) t.gameObject.layer = _roomLayer;

        for (int i = 0; i < t.childCount; i++) SetLayerRecursively(t.GetChild(i));
    }

    [ContextMenu("Start Game Mode")]
    public void StartGameMode()
    {
        if (_phase != Phase.Off) return;

        if (danceMode != null && danceMode.IsRunning)
        {
            Debug.Log("[GameMode] a dance is running; game mode stays put until it ends.", this);
            return;
        }

        // Read HERE, not in Start(): CharacterPresenter fills RestingPosition in its
        // own Start(), and the order between two Start() calls is undefined — losing
        // that race captured (0,0,0) and sent her to the world origin on the way back.
        // By the time this runs, everything in the scene is long since awake.
        _deskPosition = chloe.RestingPosition;
        _deskRotation = chloe.RestingRotation;

        _phase = Phase.Watching;
        _clock = 0f;

        ShowRoomCamera(gameCamera);
        ApplyCameraFraming(0f);

        chloe.SetRestingPose(couchPosition, Quaternion.Euler(0f, couchYaw, 0f));
        // Sitting idle is the animator's default; saying so explicitly means game
        // mode defines the pose it wants instead of inheriting whatever was last set.
        if (chloeAnimator != null) chloeAnimator.SetBool("IsStanding", false);

        HoldPets(true);

        // Started now, activated at the handover: the load has the whole wait plus
        // the push-in to finish in, so the hitch never lands on the moving camera,
        // and holding activation back keeps the game's overlay canvas off screen
        // while the room is still being looked at.
        _load = SceneManager.LoadSceneAsync(gameScenePath, LoadSceneMode.Additive);

        if (_load == null)
        {
            Debug.LogError("[GameMode] could not load '" + gameScenePath +
                           "'. Is it in Build Settings? (Tools > Fruit Merge > Apply Import Settings)", this);
            _phase = Phase.Off;
            return;
        }

        _load.allowSceneActivation = false;
    }

    [ContextMenu("Stop Game Mode")]
    public void StopGameMode()
    {
        if (!CanLeave) return;

        if (_handingOver)
        {
            _leaveRequested = true;
            return;
        }

        StartCoroutine(ReturnToRoom());
    }

    // Unscaled throughout, and that is not a detail: the game sets Time.timeScale to
    // 0 while it is paused, and leaving from its pause screen is a normal way out.
    // On scaled time the camera would simply never move again.
    private void Update()
    {
        if (_phase == Phase.Off) return;

        if (CanLeave && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            StopGameMode();
            return;
        }

        if (_phase == Phase.Playing)
        {
            // A resized window changes the shape of the bars, and there is no event
            // for it. Two int compares a frame, and nothing is written unless they differ.
            if (Screen.width != _appliedWidth || Screen.height != _appliedHeight) ApplyPlayViewport();
            return;
        }

        _clock += Time.unscaledDeltaTime;

        switch (_phase)
        {
            case Phase.Watching:
                if (_clock < zoomStartsAfter) break;
                _phase = Phase.Zooming;
                _clock = 0f;
                break;

            case Phase.Zooming:
                float t = zoomSeconds > 0f ? Mathf.Clamp01(_clock / zoomSeconds) : 1f;
                ApplyCameraFraming(t);
                if (t < 1f) break;
                _phase = Phase.Playing;
                StartCoroutine(HandOverToGame());
                break;
        }
    }

    /// <param name="t">0 = camera_game's own pose, 1 = zoomCamera's pose exactly.</param>
    private void ApplyCameraFraming(float t)
    {
        _framing = t;

        // Read every frame rather than cached in Start: the pose is authored by
        // dragging zoomCamera around, and reading it live means a nudge in the Scene
        // view shows up on the very next Play without a re-cache step.
        Vector3 endPosition = zoomCamera != null ? zoomCamera.transform.position : _restPosition;
        Quaternion endRotation = zoomCamera != null ? zoomCamera.transform.rotation : _restRotation;
        float endFov = zoomCamera != null ? zoomCamera.fieldOfView : _restFov;
        float endNearClip = zoomCamera != null ? zoomCamera.nearClipPlane : _restNearClip;

        float e = Mathf.SmoothStep(0f, 1f, t);

        Vector3 pos = Vector3.Lerp(_restPosition, endPosition, e);
        pos.y += zoomArcHeight * Mathf.Sin(Mathf.PI * e);

        // Slerped between the two authored rotations rather than aimed at a look-at
        // point, so whatever tilt or roll the shot was framed with is what it ends on.
        Quaternion rot = Quaternion.Slerp(_restRotation, endRotation, e);

        gameCamera.transform.SetPositionAndRotation(pos, rot);
        gameCamera.fieldOfView = Mathf.Lerp(_restFov, endFov, e);
        gameCamera.nearClipPlane = Mathf.Lerp(_restNearClip, endNearClip, e);
    }

    private IEnumerator HandOverToGame()
    {
        if (_load == null) yield break;

        _handingOver = true;
        _load.allowSceneActivation = true;

        // Activation spans at least a frame. The room camera stays live across it,
        // otherwise there is a frame with no camera at all and the screen goes black.
        while (!_load.isDone) yield return null;

        _load = null;
        _gameScene = SceneManager.GetSceneByPath(gameScenePath);

        if (!_gameScene.IsValid())
        {
            Debug.LogError("[GameMode] the game scene loaded but could not be found at '" +
                           gameScenePath + "'; returning to the room.", this);
            _handingOver = false;
            StartCoroutine(ReturnToRoom());
            yield break;
        }

        PrepareLoadedGame();

        // Makes the game's own lighting/render settings the live ones, and sends
        // anything it instantiates into its own scene rather than into the room.
        SceneManager.SetActiveScene(_gameScene);

        // Both room cameras off: from here the game's camera is the only one drawing.
        ShowRoomCamera(null);
        SetRoomUiVisible(false);

        _handingOver = false;

        // Someone pressed Esc mid-handover. Finishing the setup first and tearing it
        // down immediately costs one frame and is worth it: the alternative is a
        // teardown racing a half-built handover, which is where the hard bugs live.
        if (!_leaveRequested) yield break;

        _leaveRequested = false;
        StartCoroutine(ReturnToRoom());
    }

    private void PrepareLoadedGame()
    {
        GameObject[] roots = _gameScene.GetRootGameObjects();

        var cameras = new System.Collections.Generic.List<Camera>();

        // FIRST pass collects the cameras, and it has to finish before any canvas is
        // touched. Binding a canvas inside this loop looked equivalent and was not:
        // canvases and the camera live under different roots, so the lookup fell
        // through to a camera list that had not been filled yet and set worldCamera
        // to null — and a ScreenSpaceCamera canvas with no camera behaves exactly
        // like an Overlay one, which is the spreading HUD this was meant to fix.
        for (int i = 0; i < roots.Length; i++)
        {
            cameras.AddRange(roots[i].GetComponentsInChildren<Camera>(true));

            // Two EventSystems means Unity warns every frame and ignores one of
            // them anyway. The room's already drives InputSystemUIInputModule, and
            // the game reads UI hits through EventSystem.current, so one serves both.
            EventSystem[] systems = roots[i].GetComponentsInChildren<EventSystem>(true);

            for (int e = 0; e < systems.Length; e++) systems[e].gameObject.SetActive(false);
        }

        _gameCameras = cameras.ToArray();

        for (int c = 0; c < _gameCameras.Length; c++)
        {
            // Set from out here rather than in the scene: the game is vendored and
            // no file of it is edited. A culling mask is not the camera's transform,
            // so CameraFit stays its only mover.
            if (_roomLayer >= 0) _gameCameras[c].cullingMask &= ~(1 << _roomLayer);
        }

        Camera worldCamera = PickGameWorldCamera();

        // SECOND pass, now that there is something to bind to.
        for (int i = 0; i < roots.Length; i++) BindCanvasesTo(roots[i], worldCamera);

        EnsureLetterbox();
        ApplyPlayViewport();
    }

    // The camera the game's UI hangs off. Tag first, because that is what the game's
    // own code resolves through (Camera.main); depth as the tie-break, since the
    // lowest-depth camera is the one everything else is drawn over.
    private Camera PickGameWorldCamera()
    {
        if (_gameCameras == null || _gameCameras.Length == 0) return null;

        for (int i = 0; i < _gameCameras.Length; i++)
            if (_gameCameras[i] != null && _gameCameras[i].CompareTag("MainCamera")) return _gameCameras[i];

        Camera best = null;

        for (int i = 0; i < _gameCameras.Length; i++)
            if (_gameCameras[i] != null && (best == null || _gameCameras[i].depth < best.depth))
                best = _gameCameras[i];

        return best;
    }

    /// <summary>
    /// Re-parents the game's ROOT canvases onto its own camera.
    ///
    /// Without this, holding the game to a portrait viewport would only hold half of
    /// it: a Screen Space Overlay canvas ignores every camera viewport there is, so
    /// the board would sit in a portrait strip while the HUD kept stretching to the
    /// corners of a landscape screen — which is the spreading that made it look
    /// broken in the first place. Bound to the camera, the canvas inherits the
    /// viewport, and the CanvasScaler's 1080x1920 reference then means what it says.
    ///
    /// Root canvases only: a nested Canvas takes its render mode from its root, and
    /// the game uses nested ones purely for sort order.
    ///
    /// planeDistance 5, not the default 100: the camera is orthographic with the
    /// board at z 0 and the camera at z -10, so 5 puts the UI plane in front of the
    /// fruit and well inside the near/far range.
    /// </summary>
    private void BindCanvasesTo(GameObject root, Camera worldCamera)
    {
        if (worldCamera == null)
        {
            Debug.LogError("[GameMode] the loaded game has no camera to bind its UI to; the HUD " +
                           "will spread across the whole window instead of staying in the portrait area.", this);
            return;
        }

        Canvas[] canvases = root.GetComponentsInChildren<Canvas>(true);

        for (int i = 0; i < canvases.Length; i++)
        {
            Canvas canvas = canvases[i];

            if (canvas.isRootCanvas && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = worldCamera;
                canvas.planeDistance = 5f;
            }

            // Root canvases AND nested ones that override sorting: an overriding
            // nested canvas carries its own order into the same global sort, so
            // lifting only the roots would leave 8 of this game's 10 canvases —
            // its panels — still underneath the fruit.
            if (canvas.isRootCanvas || canvas.overrideSorting) canvas.sortingOrder += canvasSortingBoost;
        }
    }

    /// <summary>
    /// A full-screen camera that draws nothing but a colour, sitting behind the
    /// game's. Outside a camera's viewport rect Unity clears nothing at all, so
    /// without this the bars beside a portrait game are undefined framebuffer rather
    /// than the blue they should be.
    /// </summary>
    private void EnsureLetterbox()
    {
        if (_letterbox != null) return;

        GameObject go = new GameObject("GameMode Letterbox");
        go.transform.SetParent(transform, false);

        _letterbox = go.AddComponent<Camera>();
        _letterbox.clearFlags = CameraClearFlags.SolidColor;
        _letterbox.backgroundColor = letterboxColor;
        _letterbox.cullingMask = 0;
        // Below the game's own camera (-1) so it is painted first and everything
        // else lands on top of it.
        _letterbox.depth = -100f;
        // No AudioListener is added on purpose: a second live listener makes Unity
        // warn every frame and leaves the mix undefined.
    }

    private void ApplyPlayViewport()
    {
        _appliedWidth = Screen.width;
        _appliedHeight = Screen.height;

        if (_letterbox != null)
        {
            _letterbox.backgroundColor = letterboxColor;
            _letterbox.enabled = true;
        }

        if (_gameCameras == null) return;

        Rect rect = PortraitViewport();

        for (int i = 0; i < _gameCameras.Length; i++)
            if (_gameCameras[i] != null) _gameCameras[i].rect = rect;
    }

    // The largest centred rectangle of the authored shape that fits the window.
    private Rect PortraitViewport()
    {
        if (playResolution.x <= 0f || playResolution.y <= 0f || _appliedHeight <= 0)
            return new Rect(0f, 0f, 1f, 1f);

        float want = playResolution.x / playResolution.y;
        float have = (float)_appliedWidth / _appliedHeight;

        if (have > want)
        {
            float w = want / have;
            return new Rect((1f - w) * 0.5f, 0f, w, 1f);
        }

        float h = have / want;
        return new Rect(0f, (1f - h) * 0.5f, 1f, h);
    }

    private IEnumerator ReturnToRoom()
    {
        _phase = Phase.Returning;

        SilenceTheGame();

        if (_letterbox != null) _letterbox.enabled = false;
        _gameCameras = null;

        // A pending load cannot be cancelled — it has to be allowed to finish before
        // the scene can be unloaded. This is the path taken when the way out is used
        // during the wait or the push-in, before the handover ever happened.
        if (_load != null)
        {
            _load.allowSceneActivation = true;
            while (!_load.isDone) yield return null;
            _load = null;
            _gameScene = SceneManager.GetSceneByPath(gameScenePath);
        }

        if (_gameScene.IsValid() && _gameScene.isLoaded)
        {
            SceneManager.SetActiveScene(gameObject.scene);
            yield return SceneManager.UnloadSceneAsync(_gameScene);
        }

        // The game pauses by zeroing it, and leaving from its pause screen is normal.
        Time.timeScale = 1f;

        SetRoomUiVisible(true);
        ShowRoomCamera(gameCamera);

        // The push-in, played backwards from wherever it actually got to — leaving
        // during the wait or halfway through the move are both normal, and only the
        // distance still to cover should cost time.
        float from = _framing;
        float duration = zoomSeconds * from;
        float clock = 0f;

        while (clock < duration)
        {
            clock += Time.unscaledDeltaTime;
            ApplyCameraFraming(from * (1f - Mathf.Clamp01(clock / duration)));
            yield return null;
        }

        ApplyCameraFraming(0f);

        // Camera and teleport land together, so the jump back to the desk is never
        // on screen — the same reason dance mode pairs them.
        chloe.SetRestingPose(_deskPosition, _deskRotation);
        if (chloeAnimator != null) chloeAnimator.SetBool("IsStanding", false);
        HoldPets(false);
        ShowRoomCamera(mainCamera);

        _phase = Phase.Off;
    }

    /// <summary>
    /// Stops every AudioSource that does not belong to the room.
    ///
    /// The game's AudioService is DontDestroyOnLoad, so unloading its scene does NOT
    /// silence it — its music object outlives the unload and would follow the user
    /// back into the room. Selecting by scene rather than by type keeps this free of
    /// a compile-time reference into the vendored tree, and it cannot touch the
    /// room's own sources (dance mode's lives on --DanceMode--, in this scene).
    /// </summary>
    private void SilenceTheGame()
    {
        AudioSource[] sources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);

        for (int i = 0; i < sources.Length; i++)
            if (sources[i].gameObject.scene != gameObject.scene) sources[i].Stop();
    }

    private void HoldPets(bool held)
    {
        if (pets == null) return;

        for (int i = 0; i < pets.Length; i++)
        {
            if (pets[i] == null) continue;
            if (held) pets[i].Hold();
            else pets[i].Release();
        }
    }

    private void SetRoomUiVisible(bool visible)
    {
        if (roomUi == null) return;

        for (int i = 0; i < roomUi.Length; i++)
            if (roomUi[i] != null) roomUi[i].SetActive(visible);
    }

    /// <param name="which">null puts both room cameras out, which is what the
    /// handover wants: the game's camera is the only one left drawing.</param>
    private void ShowRoomCamera(Camera which)
    {
        SetCameraLive(mainCamera, which == mainCamera);
        SetCameraLive(gameCamera, which == gameCamera);
    }

    // Camera.enabled leaves the AudioListener on the same object running, and two
    // live listeners make Unity warn every frame and leave the mix undefined.
    private static void SetCameraLive(Camera cam, bool live)
    {
        if (cam == null) return;

        cam.enabled = live;

        AudioListener ear = cam.GetComponent<AudioListener>();

        if (ear != null) ear.enabled = live;
    }

    // Lets the couch pose and the push-in be framed by eye in the Scene view, without
    // entering Play — the same reason PetRoamer draws its route. The frustum is drawn
    // at the END pose rather than the camera being moved there, so the authored pose
    // on camera_game can never be lost to a preview.
    private void OnDrawGizmosSelected()
    {
        if (!drawFraming) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(couchPosition, 0.15f);
        Gizmos.DrawRay(couchPosition, Quaternion.Euler(0f, couchYaw, 0f) * Vector3.forward * 0.6f);

        if (zoomCamera == null) return;

        // Unity's own camera preview already shows what zoomCamera sees at the Game
        // view's aspect. This adds the one thing that preview cannot: the PORTRAIT
        // crop the game actually plays inside, which is the framing that has to land
        // on the screen.
        Gizmos.color = Color.yellow;

        Matrix4x4 saved = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(zoomCamera.transform.position, zoomCamera.transform.rotation, Vector3.one);
        float aspect = playResolution.y > 0f ? playResolution.x / playResolution.y : 0.5625f;
        Gizmos.DrawFrustum(Vector3.zero, zoomCamera.fieldOfView, 1.2f, zoomCamera.nearClipPlane, aspect);
        Gizmos.matrix = saved;

        if (gameCamera != null)
        {
            Gizmos.color = new Color(1f, 1f, 0f, 0.35f);
            Gizmos.DrawLine(gameCamera.transform.position, zoomCamera.transform.position);
        }
    }
}
