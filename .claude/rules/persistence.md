---
paths:
  - "Assets/Scripts/Persistence/**"
---
<!-- Path-scoped rule — loading behavior: see the note in ui.md. -->

# Persistence domain rules

- `StudentProfile` fields are written only inside `ProfileMerger`. `LocalJsonProfileRepository` only loads/saves the object as a whole; it never edits individual fields. No other code assigns to a `StudentProfile` instance's fields directly.
- Every `profile.json` write carries `schemaVersion`; an unversioned write is never produced.
- `IProfileRepository` is the only way Session code touches persisted profile data — never read/write `Application.persistentDataPath` directly from outside this shard. This seam exists because backend/account sync is an explicitly named, deferred roadmap item (not speculative).
- `GeminiSessionSummarizer`'s REST call and `ProfileMerger`'s merge are the only place a session's transcript turns into profile updates — no other code path writes profile data from a transcript.
- `TranscriptRecorder` accumulates only final (not partial/interim) captions, and is cleared once `SessionEndState` finishes with it.
