---
paths:
  - "Assets/Scripts/Session/**"
---
<!-- Path-scoped rule — loading behavior: see the note in ui.md. -->

# Session domain rules

- Session flow code holds no references to UI or Presentation (dependency arrow is one-way: UI/Presentation → Session, never back). States communicate outward only via `SessionFlowStateMachine.StateChanged` and `SessionContext` fields — never by calling into a UI/Presentation type.
- Only `SessionFlowStateMachine.ChangeState` transitions the current state; a state never sets `_state`/`Current` directly, and nothing outside the machine calls `Enter`/`Exit` directly.
- Free chat is not a state — it is the ambient default of `SessionActiveState` (push-to-talk always live, no tool restrictions). Do not add a `FreeChatState`; it would either freeze or duplicate the break-timer tick that must keep running underneath free chat.
- `BreakPlanner` stays a pure, dependency-free function (no Unity/voice/network types) — it is the one thing in this shard worth a unit test via `com.unity.test-framework`.
- `SessionContext` is the only shared mutable blackboard between states; a state never holds its own copy of data another state also needs (e.g. elapsed minutes, break checkpoints) — that's how the same field gets two writers.
- Each state's `Enter` declares its own `MicPolicy` and `VoiceSessionConfig` (tools/system-instruction) explicitly — never assume a policy/config carried over from the previous state.
