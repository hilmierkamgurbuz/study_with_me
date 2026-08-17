# scope — what the game is

- **Game:** A Unity app where a single fixed 3D character stands in a room and talks to the user by voice while they study — greeting them by name, asking how long they plan to study, proposing rough breaks, bridging into a rewarded-ad moment, and chatting freely whenever asked. Not a game with win/lose states; it's a study-companion utility app.
- **Core loop:** User opens the app, tells the character how long they intend to study (by voice) → the character tracks time and, at a rough/flexible checkpoint, asks if they want a break → accepting bridges to a TV/ad moment then back to studying, declining continues the timer → the user can push-to-talk or type to chat freely at any point → ending the session summarizes it into the character's persistent memory of the user, so the next session picks up the thread.
- **Win/lose:** None. A session has a natural end (user says/taps "end") rather than a win/lose condition.
- **Vertical-slice boundary** (this is the current build target — see `../plans/study-with-me-cheeky-lerdorf.md` for the full plan):
  - Real Gemini Live voice (native voice-to-voice), push-to-talk + auto-open mic at scripted moments, barge-in.
  - All six flow states working end to end: Onboarding → SessionStart → SessionActive (with ambient free chat) → BreakOffer → AdBridge (stub) → SessionEnd.
  - Placeholder character (primitive rig, not final art) and placeholder room (primitives).
  - Local JSON profile persistence with next-session continuity (character references prior session's name/goals).
  - Editor/Standalone only — no mobile or WebGL build target yet.
  - Stub rewarded ad (timer + panel, no real ad SDK).
- **Release scope** (from the product concept — the full production target this slice is a step toward): Android + iOS + WebGL; multilingual (device-locale driven, not hardcoded Turkish); freemium subscription (~$4.99–9.99/mo) gating most voice features, with rewarded ads as a secondary revenue path; final character art/animation; backend-issued ephemeral tokens instead of a client-held API key; eventually a chained STT/LLM/TTS pipeline for cost at scale; optional backend profile sync.
- **Out of scope** (deliberately not built in this slice): real ad SDK integration, final character art/animation, Android/iOS/WebGL builds, subscription/paywall enforcement, the chained-pipeline implementation itself (only the seam for it), a full relationship/leveling system beyond prompt-injected profile continuity, and a backend ephemeral-token service for the API key.
