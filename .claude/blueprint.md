# blueprint — architecture plan: systems, scenes, prefabs, folders

## Systems and dependencies

- Config — holds the Gemini API key + model id (ScriptableObject, gitignored asset) — depends on: —
- Voice — Gemini Live WebSocket transport: connect/setup handshake, mic capture, activity signaling, streaming playback, tool-call dispatch, barge-in — depends on: Config
- Persistence — student profile schema, local JSON repository, merge-on-write, end-of-session summarization (one-shot REST call) — depends on: Config
- Session — flow state machine (six states), break planning, structured-decision tool wiring — depends on: Voice, Persistence
- Presentation — character reactions, camera rig (default/TV-focus lerp), rewarded-ad stub, ambient pet roaming (cat/dog waypoint routes), dance mode (timed camera/light/music/disco sequence; `DanceModeController`), game mode (timed couch/camera/push-in sequence that hands the screen to the vendored minigame; `GameModeController`), the desk book turning its own pages (`BookPageTurner`), and what she does at the desk between conversations — study mode — turning to the book to read, page by page (`DeskRoutine`, which is what decides when a page turns; started by a button for now, by the conversation later), and two-bone arm IK putting her hands and wrists on authored targets rather than wherever a clip left them (`ArmIkSolver`) — the last three depend on nothing upstream, they are ambience rather than session-driven. Dance and game mode are mutually exclusive and the check is one-way: game mode holds a read-only reference to dance mode and refuses to start while one runs — depends on: Session, Voice, FruitMerge
- UI — captions, push-to-talk button, text input, break-offer prompt, HUD, ad-stub panel — depends on: Session, Voice
- Bootstrap — composition root; the only place concrete cross-system objects are constructed and wired — depends on: Voice, Persistence, Session, Presentation, UI, Config
- Tooling — unity-dev skill's own dev-infrastructure editor scripts (e.g. `UnityMapExporter.cs`) and per-milestone scene/test-harness setup scripts (e.g. `VoiceHarnessSceneSetup.cs`); editor-only, never ships in a build — depends on: —
- FruitMerge — the merge-puzzle minigame the user plays in game mode: a complete game vendored in from a separate Unity project (`Assets/FruitMerge/`), carrying its own scene, `.asset` data, art, audio, UI and editor tools. Imported as-is and **not** wrapped: nothing in it knows this app exists, and no host system reaches inside it — the only seam is "load its scene, hand it the screen". Its own internals (state machine, event bus, pools) are mapped in `codemap-fruitmerge.md` — depends on: —

All arrows are one-directional (Config and FruitMerge are pure leaves; Bootstrap is the sole root that is allowed to depend on everything). No system above is depended on by anything it itself depends on.

## Scene inventory

- Room — the one shipping scene for this slice — single — hand-built by the user in the Editor from LowPolyBoy + LowPolyLivingRoomPack + a few ZNS3D pieces (~171 objects: walls, tiled floor, desk/computer setup, bed, wardrobe, TV corner with games/decor, books, rug); Bootstrap/Session/Voice runtime objects and the character are not wired in yet
- VoiceHarness — throwaway scratch scene (`Assets/Scenes/_Sandbox/VoiceHarness.unity`) for the M1/M2 voice-transport spike — single — a bare `GeminiLiveVoiceSession` + `VoiceHarnessHud` test harness only; never added to Build Settings, never part of the shipping scene inventory
- Game — the Fruit Merge minigame's own scene (`Assets/FruitMerge/Scenes/Game.unity`, ~173 objects: 2D board with three wall colliders, dropper, HUD/overlay canvases, splash/menu/pause/result panels, the boost rig) — **additive**, loaded on demand by game mode and unloaded on exit; in Build Settings but never at index 0. Left exactly where the vendored project had it, so the game's own `SceneFixups` editor tool (which keys off this path) keeps working
- SampleScene — stock Unity URP-template leftover, not part of this app — none (removed from Build Settings in M0) — unmodified template content (camera, directional light, global volume); slated for deletion once Room.unity is confirmed working end to end, tracked as a cleanup task rather than deleted sight-unseen

## Prefab inventory

<!-- No new prefabs are authored for M3 — the character FBX and the vendor
     packages' existing prefabs are placed directly as scene objects in
     Room.unity (scene-structure.md: unique, single-scene, authoring-placed
     → scene object, not a new prefab). Updated with the exact pieces used
     once M3's scene-setup script is written. -->
<!-- FruitMerge is vendored, but it is NOT on check_blueprint's ignore list the way
     LowPolyBoy/ZNS3D/PolyOne are: that exemption exists for packs with hundreds of
     uninventoried prefabs, and this one ships exactly two. Listing them keeps
     Game.unity visible to the scene check too, instead of being reported as
     "planned, not created yet" while sitting on disk. -->
- Fruit — FruitMerge — `Assets/FruitMerge/Prefabs/Fruit.prefab`, spawned at runtime by the game's own `FruitPool` (never placed by hand). On **layer 6 `Fruit`** with a `Rigidbody2D` + `CircleCollider2D`; that layer index is baked into the prefab, which is why `FruitMergeImportSetup` insists on the name landing on slot 6
- ComboPopup — FruitMerge — `Assets/FruitMerge/Prefabs/ComboPopup.prefab`, pooled by the game's `ComboPopupDirector` for the "x3" labels at a merge point
- Chloe — Presentation — the character, at `Assets/Prefabs/Chloe.prefab`, replacing the ZNS3D `Chloe_Rigged` scene instance — built from `Assets/Art/Character/deneme.fbx` (hand-restructured in Blender: independent `Chloe_Faces_Alpha`, `Gameroom` [skin], `Hair`, `Hat`, `Panth`, `Shirt`, `Shirt Interior` materials, same Generic rig/bone names as before) — carries `CharacterPresenter` (animator/headBone/faceRenderer wiring)

## Hierarchy conventions

- Current reality in `Room.unity` (hand-built by the user, not script-generated): `Directional Light` and `Main Camera` sit at the scene root; room dressing is a mix of ungrouped root-level objects and a few informal named groups (`wall`, `ground`, `gardrop`, `books`) the user created while building. Not the originally-planned `--Systems--`/`--World--`/`--UI--` split — that was aspirational for code-driven content, and doesn't fit hand-placed dressing well. Documented as-is rather than forcing a re-organization with no functional benefit.
- When Bootstrap/Session/Voice runtime objects land (M5+), add a `--Systems--` root for them specifically — non-visual manager objects benefit from being easy to find, unlike room dressing.
- The `--Name--` roots are the established shape for a non-visual mode: `--DanceMode--`/`--DanceUI--`, and alongside them `--GameMode--`/`--GameUI--` (built by `Tools > StudyWithMe > Set Up Game Mode`). The controller and its canvas stay separate objects because the canvas is what game mode has to hide while the minigame owns the screen — and `--GameUI--` is the one canvas that must NOT hide, since it carries the way back.
- **Room objects live on the `Room` layer at runtime.** `GameModeController.Start()` moves everything on the default layer there once, so the minigame's camera can cull the room away on ultrawide screens. Nothing needs to be authored on that layer by hand, and the room's own cameras cull nothing — but it does mean a layer-mask raycast written against room geometry has to expect `Room`, not `Default`.
- Naming: PascalCase for anything code creates or references by name; the user's own room-dressing group names stay as authored.
- **Agent-parent pattern for animated movers** (`Cat`, `Dog` in `Room.unity`): an empty parent at scale 1 carries the mover component and the world placement, and the vendor model sits under it pinned to local zero. Required, not stylistic — the PolyOne clips write the animated object's own position/rotation every frame, so movement code and the Animator must own different transforms or they are two writers on one value. Any future animated mover built from a vendor model with baked root curves gets the same shape.

## Folder layout

<!-- Each line below is the FULL path from Assets/, on one line — the parser
     reads the leading token literally and does not understand visual/indented
     nesting across lines (learned the hard way: `Art/` then an indented
     `Character/` on the next line was read as top-level `Assets/Character/`,
     not `Assets/Art/Character/`). Indentation here is cosmetic only; never
     split a path across two lines. -->

```
Assets/
  Scripts/Config/          ← .cs: GeminiApiConfig (ScriptableObject definition)      (codemap: core)
  Scripts/Voice/           ← .cs: IVoiceSession, GeminiLiveVoiceSession, StreamingAudioPlayer,
                               GeminiLiveMessages DTOs, VoiceSessionConfig, MicPolicy, TurnState,
                               CaptionEvent, ToolCallEvent                            (codemap: voice)
  Scripts/Session/         ← .cs: ISessionState, SessionContext, SessionFlowStateMachine,
                               SessionFlowRunner, ISessionActions, BreakPlanner,
                               Session/States/*.cs                                   (codemap: session)
  Scripts/Presentation/    ← .cs: PresentationCoordinator, CharacterPresenter, RewardedAdStub,
                               (a camera-rig approach TBD — first attempt reverted, D-007)  (codemap: presentation)
  Scripts/Persistence/     ← .cs: StudentProfile, IProfileRepository,
                               LocalJsonProfileRepository, ProfileMerger,
                               TranscriptRecorder, GeminiSessionSummarizer           (codemap: persistence)
  Scripts/UI/              ← .cs: CaptionPanelView, PushToTalkButtonView, TextInputView,
                               BreakOfferPromptView, SessionHudView                  (codemap: ui)
  Scripts/Bootstrap/       ← .cs: GameBootstrapper (composition root)                (codemap: core)
  Editor/                  ← UnityMapExporter.cs, VoiceHarnessSceneSetup.cs,
                               ChloeUsedRegionExporter.cs, ChloeClipPathFixer.cs, and future
                               per-milestone scene-setup scripts                     (codemap: editor/tooling)
  Config/                  ← GeminiApiConfig.asset (gitignored, holds the real key)  (assetmap)
  Art/Character/           ← imported character model (.fbx etc.), pre-rig          (assetmap)
  Art/Textures/            ← room and character texture art, not just the character's: the 4x4
                               face-expression atlas, the book page images (book_page.png,
                               book_page_2.png, 1254x1254 sRGB) and Rugs/               (assetmap)
  Art/Book/                ← the book model (Book.fbx: Cover_L/R, PageBlock_L/R, Page_Flip).
                               Its FBX-embedded materials are read-only; the pages are dressed by
                               assigning materials to the renderer slots, never by editing those
                                                                                        (assetmap)
  Art/material/            ← hand-authored .mat assets for room props: dance, lamp, the four
                               disco lamps, rugs, and the book pages. Lowercase, as authored
                                                                                        (assetmap)
  Art/chloe/               ← Mixamo clip FBXs (Chloe_Rigged@*.fbx) + ChloeController.controller
                               + ChloeArmsOnly.mask (the two-layer split: the two arm chains
                               from the masked "Arms" layer, everything else — spine, neck,
                               head, hips, legs — from the Base layer's Sitting Idle) (assetmap)
  Art/chloe/Generated/     ← .anim clips emitted by ChloeClipPathFixer — sole writer is
                               that tool; never hand-edited                          (assetmap)
  Art/Room/                ← reserved for any custom (non-vendor-package) room art  (assetmap)
  Trip Hop Music/          ← licensed music pack, 10 .wav tracks. Declared rather than
                               added to check_blueprint's vendor-ignore list because dance
                               mode reads from it by reference, unlike the prop packs which
                               are only dressing                                     (assetmap)
  Prefabs/                 ← (empty for now — M3 uses vendor prefabs + the character
                               FBX directly as scene objects, not new prefabs)        (assetmap + unitymap)
  Scenes/                  ← Room.unity ; _Sandbox/VoiceHarness.unity                (mirrors scene inventory)
  Settings/                ← StudyWithMeControls.inputactions (dedicated PTT action) (assetmap)
  LowPolyBoy/              ← vendor asset pack (FreeStylizedBedRoom) — bulk content,
                               not individually inventoried (see decisions.md D-006)  (assetmap; exempted from per-item blueprint checks)
  LowPolyLivingRoomPack/   ← vendor asset pack — same as above                        (assetmap; exempted)
  ZNS3D/                   ← vendor asset pack (FREE_STYLIZED_GAMEROOM_PACK) — same    (assetmap; exempted)
  PolyOne/                 ← vendor asset pack (cartoon animals) — same                (assetmap; exempted)
  FruitMerge/              ← the vendored minigame, whole and unsplit: Scripts/, Scenes/Game.unity,
                               Data/ (GameConfig + FruitDatabase + 11 FruitDefinition assets),
                               Art/, Audio/, Fonts/, Prefabs/, Editor/. Kept at the SAME relative
                               path as the source project so every internal GUID reference and the
                               game's own path-keyed editor tooling still resolve
                                                                                      (codemap: fruitmerge + assetmap)
```

`Resources/` and `StreamingAssets/` are absent on purpose (per SKILL.md default) — nothing in this slice needs string-loaded/always-shipped assets. Adding either later is a `decisions.md` entry, not a quiet addition.

Vendor asset packages (`LowPolyBoy/`, `LowPolyLivingRoomPack/`, `ZNS3D/`) are deliberately **not** individually listed in the Prefab/Scene inventories below — hundreds of items, no architectural meaning per-item. `check_blueprint.py`'s per-file inventory check is told to skip these top-level folders entirely (same mechanism as the pre-existing `Art`/`Samples`/`ThirdParty` exemptions, extended to the scene/prefab check, not just the folder-layout check — see D-006). Only the specific pieces actually placed in `Room.unity` are named in the Prefab inventory below.
