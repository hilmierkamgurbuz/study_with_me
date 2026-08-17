---
paths:
  - "Assets/Scripts/UI/**"
---
<!-- Path-scoped rule: loads only when a matching file is read (not on every
     tool use), and does not auto-reload after compaction until a matching
     file is read again. Verify loading with /memory after installation. -->

# UI domain rules

- UI reads flow/voice state via `SessionFlowStateMachine.StateChanged` and `IVoiceSession`'s events (`TurnStateChanged`, `CaptionReceived`); it never polls state per frame and never writes flow/session/voice state directly.
- UI's only two write paths are `IVoiceSession.BeginPushToTalk/EndPushToTalk/SendText` (for voice/text input) and `ISessionActions` (for deterministic UI-only shortcuts like the break-offer Yes/No/End buttons). No other entry points into Session/Voice internals.
- UI depends only on interfaces/DTOs (`IVoiceSession`, `ISessionActions`, `SessionFlowStateMachine.StateChanged`'s payload) — never on concrete `GeminiLiveVoiceSession`, Newtonsoft DTOs, or NativeWebSocket types.
- The break-offer Yes/No/End buttons and the debug "End session" button exist purely to speed up manual testing (bypassing a full voice round-trip) — they call the exact same transition path a resolved voice/text decision would, never a shortcut that skips state-machine rules.
