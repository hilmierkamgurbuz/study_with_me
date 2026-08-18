---
paths:
  - "Assets/Scripts/Voice/**"
---
<!-- Path-scoped rule — loading behavior: see the note in ui.md. -->

# Voice domain rules

- `GeminiLiveVoiceSession` is the only thing that owns the WebSocket, the mic capture loop, and the playback ring buffer. Everything else talks to it only through `IVoiceSession`.
- Turn boundaries are client-driven: `setup.realtimeInputConfig.automaticActivityDetection.disabled = true` always; every listening window (push-to-talk hold, or an auto-open window) is wrapped in explicit `activityStart`/`activityEnd` messages. Do not re-enable server-side auto-VAD without updating this rule and the plan's Architecture section — it was deliberately disabled because push-to-talk is already the app's real "is the user talking" signal.
- A received `toolCall` always gets an immediate `toolResponse` in the same handling pass — an unacknowledged tool call can stall the model's turn.
- Barge-in (flushing the local playback ring buffer and sending a fresh `activityStart`) happens synchronously on the calling thread the moment PTT is pressed or text is sent — it never waits on a network round trip.
- Any code touching a UnityEngine API (AudioSource, AudioClip, UI events) from the WebSocket receive path must be marshaled onto the main thread first (a queue drained in `Update()`); the receive loop itself runs on a background thread. The socket's own `OnClose`/`OnError` callbacks count as that path: they raise a flag and nothing else.
- A dropped socket is reconnected by `GeminiLiveVoiceSession` itself, using the newest session-resumption handle; no caller reconnects by calling `Connect` again, and `Disconnect()` is the only way to say a close was meant. `setup.contextWindowCompression.slidingWindow` and `setup.sessionResumption` stay enabled together — they are what keeps a conversation alive past the audio-only session duration cap, and a study block is expected to outlive a single connection.
- The streaming-playback ring buffer is fixed-size and pre-allocated — no allocation inside the `AudioClip` `pcmReaderCallback`, which runs on Unity's audio DSP thread.
- Structured decisions (session duration, break-offer intent, onboarding profile fields) are resolved via Gemini function-calling tools declared per session state, never by string/keyword matching on transcript text — this is what makes decision parsing multilingual for free.
- Model IDs for the Live API are preview-tier and rotate; never hardcode one without checking `ai.google.dev/gemini-api/docs/models/live` first, and keep the current ID in exactly one place (`GeminiApiConfig`).
