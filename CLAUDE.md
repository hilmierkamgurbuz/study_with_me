# Study With Me

## Phase

- **Phase:** production
<!-- production | shipping — release-grade in both; the phase only sets the
     proof standard for optimization (cost threshold vs measurement).
     Transitions happen only via the phase-transition audit (skill router). -->

## Invariants

- Content data (numbers, curves, tables, text) is never embedded in code; it is read from data.
- Save data carries a version number; unversioned saves are never written.
- Every piece of data has a single writer; if a second writer appears, code halts.
- Dependency arrows between systems are one-directional (plan: .claude/blueprint.md).
- C# changes are made only with the Edit/Write tools; writing files via Bash is forbidden.
- `StudentProfile` fields are written only by `ProfileMerger` — no other code assigns profile fields directly.
- Session flow transitions happen only through `SessionFlowStateMachine.ChangeState`, called from a state itself or via `ISessionActions` — never by reaching into the machine from UI/Presentation.
- UI and Presentation code depend only on interfaces/events from Session and Voice — never on concrete `GeminiLiveVoiceSession`, session state classes, or Newtonsoft/NativeWebSocket types.
- The Gemini API key lives only in the gitignored `Assets/Config/GeminiApiConfig.asset` — never hardcoded in a committed script.
- Structured user decisions (study duration, break-offer intent, onboarding fields) are resolved via Gemini Live function-calling tools, never by string/keyword matching on transcript text.
- A received `toolCall` always gets an immediate `toolResponse` in the same handling pass.
- `Assets/FruitMerge/` is vendored code, not ours. No file in that tree is edited: the single ported file is `PointerInput.cs` (D-031), and any further edit there needs its own decision entry. Its gameplay types (`GameManager`, `CameraFit`, `SaveService`, …) are never referenced from app code — not by call, not by `using`. The integration seam is allowed to set exactly these things on the loaded scene from outside, and nothing else: the game camera's **culling mask** (so the room can be culled) and **viewport rect** (so a portrait game stays portrait), its root canvases' **render mode / world camera / plane distance** (so the UI obeys that same viewport), the loaded scene's **EventSystem enabled state** (the room's serves both), and **which scene is active**. All of it is set at runtime on a freshly loaded scene and dies with the unload — no file, no serialized asset. Anything beyond this list is a new decision.

## Fingerprint summary

<!-- Full version in .claude/fingerprint.md — this block is a few-line copy. -->
- Space: 3D, free/continuous; one static room scene; no grid.
- Determinism: not required (real-time voice conversation, not a simulation/replay system).
- Authorities: `StudentProfile` → `ProfileMerger`; flow state → `SessionFlowStateMachine`; voice turn state → `GeminiLiveVoiceSession`. Inside the vendored minigame: game state (and `Time.timeScale`) → `GameManager`; its camera → `CameraFit`; its save file → `SaveService`.
- Scale: small-n throughout in the app itself (6 flow states, 1 concurrent voice session, ~10 UI elements). One exception, and it is vendored: the minigame loops per frame over ~60 fruit bodies and ~60 faces, each from a single `Update` — that consolidation is the game's own answer to this cost question, so the budget is already accounted for upstream.

## Pointers

- **Start here every task:** .claude/index.md (system → where it lives)
- Scope: .claude/scope.md · Fingerprint: .claude/fingerprint.md
- Blueprint (systems/scenes/prefabs/folders): .claude/blueprint.md
- Decisions: .claude/decisions.md · Code map: .claude/codemap-*.md
- Scene map: .claude/unitymap.md · Asset map: .claude/assetmap.md
- Shard definition: .claude/shards.json · Domain rules: .claude/rules/
- Full vertical-slice implementation plan: ~/.claude/plans/study-with-me-cheeky-lerdorf.md
