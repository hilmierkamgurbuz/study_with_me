using UnityEngine;

public class CharacterPresenter : MonoBehaviour
{
    private enum FaceExpression
    {
        Neutral, TalkSmall, TalkMid, TalkWide,
        LookLeft, LookRight, LookDown, Blink,
        SmileSoft, Laugh, Surprised, Bored,
        Yawn, Thoughtful, Caring, Cheeky
    }

    private static readonly FaceExpression[] TalkCycle =
    {
        FaceExpression.Neutral, FaceExpression.TalkSmall, FaceExpression.TalkMid,
        FaceExpression.TalkWide, FaceExpression.TalkMid, FaceExpression.TalkSmall
    };

    // Grid cells (3,1), (3,2) and (4,4) — the happy end of the sheet, cycled while
    // she is dancing.
    private static readonly FaceExpression[] DanceCycle =
    {
        FaceExpression.SmileSoft, FaceExpression.Laugh, FaceExpression.Cheeky
    };

    public Animator animator;
    public SkinnedMeshRenderer faceRenderer;
    public float talkFrameSeconds = 0.15f;
    // Much slower than the mouth cycle on purpose: these are whole expressions,
    // and they should sit for a beat rather than flicker like mouth shapes.
    public float danceFrameSeconds = 1.5f;
    public float faceZoom = 1.2f;

    [Header("Speaking pose")]
    // The Sitting Talking clip leans her far enough forward that the room camera
    // sees the top of her head instead of her face, and a clip's pose cannot be
    // edited from here. These shift the whole body while she speaks and blend back
    // out afterwards; they are in her own resting frame, so +Z is the direction she
    // faces and -Y sits her lower. Values are meant to be tuned in the Inspector.
    public Vector3 speakingPositionOffset = new Vector3(0f, -0.05f, 0.12f);
    public Vector3 speakingRotationOffset = new Vector3(-8f, 0f, 0f);
    public float poseBlendSeconds = 0.25f;

    [Header("Tuning aids")]
    // The offsets above only show while she is really speaking, which needs a live
    // voice session — ticking IsTalking on the Animator drives the clip but not
    // this component. previewSpeaking makes them observable while tuning; it drives
    // the pose only, so the face is never left parked on a talk cell.
    public bool previewSpeaking;
    // LateUpdate otherwise reverts any drag in the Scene view during Play, which
    // makes placing her on the chair while the Idle clip runs impossible.
    public bool lockPose = true;

    private Vector3 _fixedPosition;
    private Quaternion _fixedRotation;
    private float _poseBlend;
    private Material _faceMaterial;
    private Vector2 _faceUvMin;
    private Vector2 _faceUvScale;
    private bool _isSpeaking;

    // Set by the desk routine while she is turned to the book. Lowest priority of
    // the three face drivers: dancing beats speaking beats reading.
    private bool _isReading;
    private float _talkClock;
    private int _talkFrame;
    private bool _isDancing;
    private float _danceClock;
    private int _danceFrame;

    private void Start()
    {
        _fixedPosition = transform.position;
        _fixedRotation = transform.rotation;
        if (faceRenderer != null)
        {
            _faceMaterial = faceRenderer.material;
            ComputeFaceUvTransform();
        }
        SetExpression(FaceExpression.Neutral);
    }

    // The face submesh's UV does not span the full [0,1] square — it's
    // whatever small sub-rectangle was left over from the old shared atlas
    // layout. Reading it directly (instead of assuming 0-1) is what lets the
    // grid-cell offset land correctly regardless of where that rectangle sits.
    private void ComputeFaceUvTransform()
    {
        Mesh mesh = faceRenderer.sharedMesh;
        Vector2[] uvs = mesh != null ? mesh.uv : null;
        if (uvs == null || uvs.Length == 0) return;

        float minU = float.MaxValue, maxU = float.MinValue, minV = float.MaxValue, maxV = float.MinValue;
        foreach (var uv in uvs)
        {
            if (uv.x < minU) minU = uv.x;
            if (uv.x > maxU) maxU = uv.x;
            if (uv.y < minV) minV = uv.y;
            if (uv.y > maxV) maxV = uv.y;
        }

        _faceUvMin = new Vector2(minU, minV);
        _faceUvScale = new Vector2(0.25f / Mathf.Max(maxU - minU, 0.0001f), 0.25f / Mathf.Max(maxV - minV, 0.0001f));
    }

    // The Animator has written the clip's pose by this point, so LateUpdate is the
    // only place where overriding the root transform survives the frame. Position
    // is rebuilt from the Start-captured pose rather than nudged, so the speaking
    // offset cannot accumulate frame over frame.
    private void LateUpdate()
    {
        // Dancing owns the face while it lasts, so the two cycles can never both
        // write a cell on the same frame.
        if (_isDancing) TickDanceCycle();
        else if (_isSpeaking) TickTalkCycle();
        if (!lockPose) return;

        float target = _isSpeaking || previewSpeaking ? 1f : 0f;
        _poseBlend = poseBlendSeconds > 0f
            ? Mathf.MoveTowards(_poseBlend, target, Time.deltaTime / poseBlendSeconds)
            : target;

        transform.SetPositionAndRotation(
            _fixedPosition + _fixedRotation * (speakingPositionOffset * _poseBlend),
            _fixedRotation * Quaternion.Euler(speakingRotationOffset * _poseBlend));
    }

    public void SetTalking(bool talking)
    {
        if (animator != null) animator.SetBool("IsTalking", talking);
    }

    // Moves the pose LateUpdate rebuilds her from, instead of letting a caller
    // write transform.position directly — that write would be reverted on the
    // same frame, and adding a second writer to the root transform is exactly
    // what this component exists to prevent. Dance mode uses this to put her on
    // the dance floor and to bring her back to the desk afterwards.
    public void SetRestingPose(Vector3 position, Quaternion rotation)
    {
        _fixedPosition = position;
        _fixedRotation = rotation;
        transform.SetPositionAndRotation(position, rotation);
    }

    public Vector3 RestingPosition { get { return _fixedPosition; } }
    public Quaternion RestingRotation { get { return _fixedRotation; } }

    // Read-only, and read-only is the whole point: _isSpeaking has exactly one
    // writer (SetTurnState, from the voice session) and this adds none. Ambient
    // room behaviour asks it so it can stay out of her way while she talks —
    // BookPageTurner is the first to, and it needs the turn state without
    // subscribing to the voice session itself, which Presentation may not do.
    public bool IsSpeaking { get { return _isSpeaking; } }

    // Dance mode says only whether she is dancing; which cell shows stays in
    // here, so the face keeps a single writer.
    public void SetDancing(bool dancing)
    {
        _isDancing = dancing;
        _danceClock = 0f;
        _danceFrame = 0;
        SetExpression(dancing ? DanceCycle[0] : FaceExpression.Neutral);
    }

    // The desk routine says only whether she is reading; WHICH cell that shows stays
    // in here, exactly as SetDancing does, so the face keeps a single writer. Grid
    // cell (2,3) is already the looking-down face, so this needs no new art.
    public void SetReading(bool reading)
    {
        if (_isReading == reading) return;

        _isReading = reading;

        // Speaking and dancing both outrank reading and are already painting; letting
        // this through would fight them for the same frame.
        if (_isDancing || _isSpeaking) return;

        SetExpression(reading ? FaceExpression.LookDown : FaceExpression.Neutral);
    }

    public void SetTurnState(TurnState state)
    {
        SetTalking(state == TurnState.Speaking);
        _isSpeaking = state == TurnState.Speaking;
        _talkClock = 0f;
        _talkFrame = 0;

        // A live voice session must not repaint the face mid-dance — that would
        // put two writers on it. The animator flags above still update, so the
        // session picks up where it left off once the dance ends.
        if (_isDancing) return;

        switch (state)
        {
            case TurnState.Listening:
                SetExpression(FaceExpression.Caring);
                break;
            case TurnState.Thinking:
                SetExpression(FaceExpression.Thoughtful);
                break;
            case TurnState.Speaking:
                SetExpression(TalkCycle[0]);
                break;
            default:
                // Not Neutral if she is reading: the turn state churns back to Idle
                // between sentences, and repainting Neutral there would blink the
                // looking-down face off for no reason the viewer can see.
                SetExpression(_isReading ? FaceExpression.LookDown : FaceExpression.Neutral);
                break;
        }
    }

    private void TickDanceCycle()
    {
        _danceClock += Time.deltaTime;
        if (_danceClock < danceFrameSeconds) return;
        _danceClock = 0f;
        _danceFrame = (_danceFrame + 1) % DanceCycle.Length;
        SetExpression(DanceCycle[_danceFrame]);
    }

    private void TickTalkCycle()
    {
        _talkClock += Time.deltaTime;
        if (_talkClock < talkFrameSeconds) return;
        _talkClock = 0f;
        _talkFrame = (_talkFrame + 1) % TalkCycle.Length;
        SetExpression(TalkCycle[_talkFrame]);
    }

    private void SetExpression(FaceExpression expression)
    {
        if (_faceMaterial == null) return;
        (int row, int col) = CellFor(expression);
        // authored art is top-to-bottom row 1..4, but Unity UV v=0 is the
        // bottom of the texture, so the row must be inverted here
        float centerU = (col - 1) * 0.25f + 0.125f;
        float centerV = (4 - row) * 0.25f + 0.125f;
        // faceZoom > 1 samples a smaller, centered slice of the cell instead
        // of the whole cell, cropping in on the drawn art so it fills more of
        // the head's fixed decal area without needing new art
        float half = 0.125f / Mathf.Max(faceZoom, 0.01f);
        float cellU = centerU - half;
        float cellV = centerV - half;
        float cellSize = half * 2f;
        var scale = new Vector2(cellSize * _faceUvScale.x / 0.25f, cellSize * _faceUvScale.y / 0.25f);
        var offset = new Vector2(cellU - _faceUvMin.x * scale.x, cellV - _faceUvMin.y * scale.y);
        _faceMaterial.SetTextureScale("_BaseMap", scale);
        _faceMaterial.SetTextureScale("_MainTex", scale);
        _faceMaterial.SetTextureOffset("_BaseMap", offset);
        _faceMaterial.SetTextureOffset("_MainTex", offset);
    }

    private static (int row, int col) CellFor(FaceExpression expression)
    {
        switch (expression)
        {
            case FaceExpression.Neutral: return (1, 1);
            case FaceExpression.TalkSmall: return (1, 2);
            case FaceExpression.TalkMid: return (1, 3);
            case FaceExpression.TalkWide: return (1, 4);
            case FaceExpression.LookLeft: return (2, 1);
            case FaceExpression.LookRight: return (2, 2);
            case FaceExpression.LookDown: return (2, 3);
            case FaceExpression.Blink: return (2, 4);
            case FaceExpression.SmileSoft: return (3, 1);
            case FaceExpression.Laugh: return (3, 2);
            case FaceExpression.Surprised: return (3, 3);
            case FaceExpression.Bored: return (3, 4);
            case FaceExpression.Yawn: return (4, 1);
            case FaceExpression.Thoughtful: return (4, 2);
            case FaceExpression.Caring: return (4, 3);
            case FaceExpression.Cheeky: return (4, 4);
            default: return (1, 1);
        }
    }
}
