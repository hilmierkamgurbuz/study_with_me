using UnityEngine;

// Runs the whole dance-mode sequence on one clock: camera swap, Chloe's move to
// the dance floor, the room lights going out, the disco rig coming up with
// colour cycling and a slow swing, the music, and then the same steps unwound.
//
// It never writes Chloe's transform directly — CharacterPresenter owns that and
// rebuilds it every frame, so her placement goes through SetRestingPose.
public class DanceModeController : MonoBehaviour
{
    private enum Phase { Off, Arriving, DarkPause, Warmup, Dancing, Unwinding }

    [Header("Cast")]
    public CharacterPresenter chloe;
    public Animator chloeAnimator;
    public Camera mainCamera;
    public Camera danceCamera;

    [Header("Lights")]
    // Only the Light components are switched, never the GameObjects — the lamp
    // meshes have to stay visible while they are off.
    public Light[] roomLights;
    // One entry per disco fixture. Its lights and its 'r' shell are read out of
    // its children at Start instead of being wired as separate flat arrays —
    // that is what guarantees a lamp glows its OWN light's colour, which two
    // hand-ordered arrays of different lengths could not.
    public Transform[] discoFixtures;
    public string lampChildName = "r";

    [Header("Props")]
    public Transform discoBall;
    public ParticleSystem noteParticles;
    // They circle the floor and stop to watch her while the dance runs. This
    // hands over only the centre and the thing to watch — where each pet
    // actually goes stays inside PetRoamer, which owns its transform.
    public PetRoamer[] partyPets;

    [Header("Music")]
    public AudioSource musicSource;
    public AudioClip[] tracks;

    [Header("Where she dances")]
    public Vector3 dancePosition = new Vector3(16f, 0.127f, 3.97f);
    public float danceYaw = 90f;

    [Header("Timing (seconds)")]
    public float lightsOutAt = 2f;
    public float discoOnAfterLightsOut = 1f;
    public float danceStartsAfterMusic = 1f;
    public float danceSeconds = 120f;
    public float roomLightsBackAfterEnd = 1f;
    // How many dance states the controller offers on DanceClip; one is picked at
    // random per session. A field, not a constant, so adding a fourth dance is an
    // Inspector change rather than a code change.
    public int danceClipCount = 3;
    // The masked arms layer is silenced for the duration of the dance, so the
    // dance plays from ONE playback head on the base layer. Referred to by name,
    // not index, so reordering layers cannot silently point this at the wrong one.
    public string armsLayerName = "Arms";

    [Header("Disco look")]
    public Color[] palette =
    {
        new Color(1f, 0.15f, 0.45f), new Color(0.2f, 0.85f, 1f),
        new Color(0.55f, 1f, 0.2f),  new Color(1f, 0.65f, 0.1f),
        new Color(0.7f, 0.3f, 1f),   new Color(1f, 0.95f, 0.3f)
    };
    public float colourChangeSeconds = 10f;
    public float discoIntensity = 4f;
    public float emissionBoost = 6f;
    // The beams sweep a circle on the floor: the fixture is spun about WORLD Y,
    // and because its beam already leans off vertical that traces a ring centred
    // under the fixture. beamSpreadDegrees leans it further out, which is what
    // sets the ring's radius — at rest the beams only covered a 0.46 x 0.43 m
    // patch in the middle of the floor.
    // 5 deg, not more: the spin alone already gives a 0.90 m ring under each
    // fixture, and 5 deg takes it to ~1.11 m, which sweeps through the middle of
    // the 1.55 m floor and just past its edges. Larger values throw the beams
    // metres out into the room.
    public float beamSpreadDegrees = 5f;
    public float sweepSecondsPerTurn = 8f;
    public float discoBallDegreesPerSecond = 30f;

    private Phase _phase;
    private float _clock;
    private int _colourStep = -1;
    private float _colourClock;
    private Vector3 _deskPosition;
    private Quaternion _deskRotation;
    // World rotations, not local: the sweep is defined about the world vertical.
    private Quaternion[] _fixtureRest;
    private Vector3[] _fixtureTiltAxis;
    private Light[][] _fixtureLights;
    private Renderer[] _fixtureLamp;
    private MaterialPropertyBlock _block;
    private int _armsLayer = -1;

    public bool IsRunning { get { return _phase != Phase.Off; } }

    private void Start()
    {
        if (chloe == null) { Debug.LogError("[DanceMode] chloe is not assigned.", this); enabled = false; return; }

        if (chloeAnimator != null)
        {
            _armsLayer = chloeAnimator.GetLayerIndex(armsLayerName);
            if (_armsLayer < 0)
                Debug.LogError("[DanceMode] no layer called '" + armsLayerName + "' on " +
                               chloeAnimator.runtimeAnimatorController +
                               "; the arms layer will not be silenced during the dance.", this);
        }

        _block = new MaterialPropertyBlock();
        int n = discoFixtures != null ? discoFixtures.Length : 0;
        _fixtureRest = new Quaternion[n];
        _fixtureTiltAxis = new Vector3[n];
        _fixtureLights = new Light[n][];
        _fixtureLamp = new Renderer[n];
        for (int i = 0; i < n; i++)
        {
            if (discoFixtures[i] == null) { _fixtureLights[i] = new Light[0]; _fixtureTiltAxis[i] = Vector3.right; continue; }
            _fixtureRest[i] = discoFixtures[i].rotation;
            _fixtureLights[i] = discoFixtures[i].GetComponentsInChildren<Light>(true);

            // Lean the beam further from vertical by turning about the horizontal
            // axis square to where it already points, so the spread pushes it
            // outward rather than sideways.
            Vector3 beam = _fixtureLights[i].Length > 0 ? _fixtureLights[i][0].transform.forward : Vector3.down;
            Vector3 flat = new Vector3(beam.x, 0f, beam.z);
            // flat x up, not up x flat: measured, the other order leans the beam
            // back TOWARDS vertical and shrinks the ring instead of widening it.
            _fixtureTiltAxis[i] = flat.sqrMagnitude > 0.0001f
                ? Vector3.Cross(flat.normalized, Vector3.up)
                : Vector3.right;
            var lamp = discoFixtures[i].Find(lampChildName);
            _fixtureLamp[i] = lamp != null ? lamp.GetComponent<Renderer>() : null;
            if (_fixtureLamp[i] == null)
                Debug.LogWarning("[DanceMode] fixture '" + discoFixtures[i].name + "' has no '" +
                                 lampChildName + "' renderer; its lamp will not glow.", this);
        }

        // The scene ships with both cameras enabled and the disco rig lit; the
        // resting state is defined here rather than trusted from the scene.
        SetDisco(false);
        SetRoomLights(true);
        ShowDanceCamera(false);
        if (noteParticles != null) noteParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    [ContextMenu("Start Dance")]
    public void StartDance()
    {
        if (_phase != Phase.Off) return;

        // Read HERE, not in Start(). CharacterPresenter fills RestingPosition in
        // its own Start() and the order between two Start() calls is undefined —
        // D-037 fixed exactly this race in GameModeController and left it standing
        // here because it happened to be won on the desktop. On a phone it was
        // lost: the dance captured (0,0,0) and returned her to the world origin
        // instead of her chair. By the time a dance is asked for, everything in
        // the scene has been awake for many frames, so the race is gone rather
        // than made less likely.
        _deskPosition = chloe.RestingPosition;
        _deskRotation = chloe.RestingRotation;

        _phase = Phase.Arriving;
        _clock = 0f;

        ShowDanceCamera(true);
        chloe.SetRestingPose(dancePosition, Quaternion.Euler(0f, danceYaw, 0f));
        SetAnimatorFlags(standing: true, dancing: false);
    }

    [ContextMenu("Stop Dance")]
    public void StopDance()
    {
        if (_phase == Phase.Off || _phase == Phase.Unwinding) return;
        BeginUnwind();
    }

    private void Update()
    {
        if (_phase == Phase.Off) return;

        _clock += Time.deltaTime;

        switch (_phase)
        {
            case Phase.Arriving:
                if (_clock < lightsOutAt) break;
                SetRoomLights(false);
                Advance(Phase.DarkPause);
                break;

            case Phase.DarkPause:
                if (_clock < discoOnAfterLightsOut) break;
                SetDisco(true);
                PlayRandomTrack();
                if (noteParticles != null) noteParticles.Play(true);
                Advance(Phase.Warmup);
                break;

            case Phase.Warmup:
                if (_clock < danceStartsAfterMusic) break;
                if (chloeAnimator != null && danceClipCount > 0)
                    chloeAnimator.SetInteger("DanceClip", Random.Range(0, danceClipCount));
                SetAnimatorFlags(standing: true, dancing: true);
                Advance(Phase.Dancing);
                break;

            case Phase.Dancing:
                if (_clock < danceSeconds) break;
                BeginUnwind();
                break;

            case Phase.Unwinding:
                if (_clock < roomLightsBackAfterEnd) break;
                SetRoomLights(true);
                // Camera and teleport land on the same frame so the jump back to
                // the desk is never on screen.
                ShowDanceCamera(false);
                chloe.SetRestingPose(_deskPosition, _deskRotation);
                SetAnimatorFlags(standing: false, dancing: false);
                _phase = Phase.Off;
                break;
        }

        if (_phase == Phase.Warmup || _phase == Phase.Dancing) TickDiscoLook();
    }

    private void Advance(Phase next)
    {
        _phase = next;
        _clock = 0f;
    }

    // Music and the disco rig stop on the same frame, and she leaves the dance
    // for the standing idle — the controller blends that over 1 s.
    private void BeginUnwind()
    {
        if (musicSource != null) musicSource.Stop();
        SetDisco(false);
        if (noteParticles != null) noteParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        SetAnimatorFlags(standing: true, dancing: false);
        Advance(Phase.Unwinding);
    }

    private void TickDiscoLook()
    {
        _colourClock += Time.deltaTime;
        if (_colourStep < 0 || _colourClock >= colourChangeSeconds)
        {
            _colourClock = 0f;
            _colourStep++;
            ApplyColours();
        }

        if (discoBall != null)
            discoBall.Rotate(0f, discoBallDegreesPerSecond * Time.deltaTime, 0f, Space.Self);

        if (discoFixtures == null || sweepSecondsPerTurn <= 0f) return;
        for (int i = 0; i < discoFixtures.Length; i++)
        {
            if (discoFixtures[i] == null) continue;
            // a quarter turn apart, so four beams read as a spread pattern rather
            // than one shape moving in lockstep
            float turns = Time.time / sweepSecondsPerTurn + i / (float)discoFixtures.Length;
            float spin = turns * 360f;
            discoFixtures[i].rotation =
                Quaternion.AngleAxis(spin, Vector3.up) *
                Quaternion.AngleAxis(beamSpreadDegrees, _fixtureTiltAxis[i]) *
                _fixtureRest[i];
        }
    }

    // One colour per fixture, shared by its lights and its lamp shell — that
    // pairing is the whole point of grouping by fixture.
    private void ApplyColours()
    {
        if (palette == null || palette.Length == 0) return;

        for (int f = 0; f < _fixtureLights.Length; f++)
        {
            Color c = palette[(f + _colourStep) % palette.Length];
            foreach (var l in _fixtureLights[f]) if (l != null) l.color = c;
            PaintLamp(f, c * emissionBoost);
        }
    }

    // A property block, never the shared material: these four materials are
    // assets, and writing to sharedMaterial at runtime edits them on disk.
    private void PaintLamp(int fixtureIndex, Color emission)
    {
        var r = _fixtureLamp[fixtureIndex];
        if (r == null) return;
        r.GetPropertyBlock(_block);
        _block.SetColor("_EmissionColor", emission);
        r.SetPropertyBlock(_block);
    }

    private void SetDisco(bool on)
    {
        for (int f = 0; f < _fixtureLights.Length; f++)
            foreach (var l in _fixtureLights[f])
                if (l != null) { l.enabled = on; l.intensity = discoIntensity; }

        if (on) { _colourStep = -1; _colourClock = 0f; return; }

        for (int f = 0; f < _fixtureLamp.Length; f++) PaintLamp(f, Color.black);
        for (int f = 0; f < _fixtureRest.Length; f++)
            if (discoFixtures[f] != null) discoFixtures[f].rotation = _fixtureRest[f];
    }

    private void SetRoomLights(bool on)
    {
        if (roomLights == null) return;
        foreach (var l in roomLights) if (l != null) l.enabled = on;
    }

    // Camera.enabled does not touch the AudioListener on the same object, so
    // switching cameras alone leaves both listeners live and Unity warns about
    // it every frame — and with two listeners the music's mix is undefined.
    private void ShowDanceCamera(bool on)
    {
        SetCameraLive(danceCamera, on);
        SetCameraLive(mainCamera, !on);
    }

    private static void SetCameraLive(Camera cam, bool live)
    {
        if (cam == null) return;
        cam.enabled = live;
        var ear = cam.GetComponent<AudioListener>();
        if (ear != null) ear.enabled = live;
    }

    private void SetAnimatorFlags(bool standing, bool dancing)
    {
        if (chloeAnimator == null) return;
        chloeAnimator.SetBool("IsStanding", standing);
        chloeAnimator.SetBool("IsDancing", dancing);
        // Silenced while dancing so the base layer is the only thing driving the
        // rig; back to full afterwards, which hands the arms straight back to the
        // talking behaviour without it needing to know a dance happened.
        if (_armsLayer >= 0) chloeAnimator.SetLayerWeight(_armsLayer, dancing ? 0f : 1f);
        // Only the flag — which face cell shows is CharacterPresenter's business.
        if (chloe != null) chloe.SetDancing(dancing);

        // Both the timed ending and StopDance() reach the unwind through here,
        // so there is no path that leaves a pet stuck in party mode.
        if (partyPets == null) return;
        foreach (var pet in partyPets)
        {
            if (pet == null) continue;
            if (dancing) pet.EnterParty(dancePosition, chloe != null ? chloe.transform : null);
            else pet.ExitParty();
        }
    }

    private void PlayRandomTrack()
    {
        if (musicSource == null || tracks == null || tracks.Length == 0) return;
        musicSource.clip = tracks[Random.Range(0, tracks.Length)];
        musicSource.loop = true;
        musicSource.Play();
    }
}
