---
paths:
  - "Assets/Scripts/Session/**"
---
<!-- Path-scoped rule — loading behavior: see the note in ui.md. -->

# Session domain rules

- Nothing in this shard is a MonoBehaviour and nothing here names a UI, Presentation or Voice type. Both classes are plain C# handed their inputs by Bootstrap; that is what keeps the dependency arrow one-way and lets them be exercised without entering Play mode.
- **There is no flow state machine, and free chat is not a state.** The conversation is ambient and always available, so there is nothing to transition. D-052 removed the six-state design; reintroducing a state is a decisions.md entry, not a quiet addition.
- `StudyBlockRunner` is the sole owner of study/break time. No caller keeps its own elapsed counter, and nothing else decides when a block is over.
- `BreakOfferPolicy` is the sole owner of "how often to offer" and "when to stop asking". The prompt may describe an offer; it never counts one, because a model cannot be relied on to keep score.
- Study time is spent only by studying. A dance, a game or a live voice turn **freezes** the clock — they are time spent instead of studying, never a shorter version of it.
- A break is never ended by this shard. `BreakElapsed` is a cue to ask the user, and only a fresh `StartStudy` leaves the break phase — an activity's own length (a two-minute dance, a game of unknown duration) is not this clock's business.
