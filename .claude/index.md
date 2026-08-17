<!-- stamp: 2026-08-17 systems:9 unmapped:2 unassigned-files:0 -->
# index — system to location, one screen

Step 1 of `procedures/locate.md`: read this before anything else, and only
descend into blueprint/codemap/unitymap for what this table points at.
Regenerate with `python3 .claude/hooks/build_index.py`; it joins existing
maps and never invents a name.

| system | shard(s) | entry files | scenes | prefabs | data | status |
|---|---|---|---|---|---|---|
| Bootstrap | core | Assets/Scripts/Bootstrap/RoomSessionController.cs | Room | - | - | OK |
| Config | core | Assets/Scripts/Config/GeminiApiConfig.cs | - | - | Assets/Config | OK |
| FruitMerge | fruitmerge | Assets/FruitMerge/Scripts/Core/BoostGate.cs, Assets/FruitMerge/Scripts/Core/GameEvents.cs (+55) | Game | Fruit, ComboPopup | Assets/FruitMerge/Data, Assets/FruitMerge/Data/Fruits | OK |
| Persistence | persistence | Assets/Scripts/Persistence/LocalJsonProfileRepository.cs, Assets/Scripts/Persistence/ProfileMerger.cs (+1) | - | - | - | OK |
| Presentation | presentation | Assets/Scripts/Presentation/CharacterPresenter.cs, Assets/Scripts/Presentation/ArmIkSolver.cs (+8) | - | Chloe | - | OK |
| Session | - | - | Room, VoiceHarness | - | - | UNMAPPED — blueprint system with no code |
| Tooling | editor | Assets/Editor/BookPageSetup.cs, Assets/Editor/ChloeClipPathFixer.cs (+6) | - | - | - | OK |
| UI | - | - | Room, VoiceHarness, Game, SampleScene | - | - | UNMAPPED — blueprint system with no code |
| Voice | voice | Assets/Scripts/Voice/GeminiLiveMessages.cs, Assets/Scripts/Voice/GeminiLiveVoiceSession.cs (+2) | Room, VoiceHarness | - | - | OK |

## Gaps
- 1 flagged codemap line(s) excluded from this table (STALE)
