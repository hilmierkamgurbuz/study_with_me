<!-- stamp: 2026-08-21 systems:9 unmapped:0 unassigned-files:0 -->
# index — system to location, one screen

Step 1 of `procedures/locate.md`: read this before anything else, and only
descend into blueprint/codemap/unitymap for what this table points at.
Regenerate with `python3 .claude/hooks/build_index.py`; it joins existing
maps and never invents a name.

| system | shard(s) | entry files | scenes | prefabs | data | status |
|---|---|---|---|---|---|---|
| Bootstrap | core | Assets/Scripts/Bootstrap/RoomSessionController.cs | Room | - | - | OK |
| Config | core | Assets/Scripts/Config/ChloePersonaConfig.cs, Assets/Scripts/Config/GeminiApiConfig.cs | - | - | Assets/Config | OK |
| FruitMerge | fruitmerge | Assets/FruitMerge/Scripts/Core/BoostGate.cs, Assets/FruitMerge/Scripts/Core/GameEvents.cs (+55) | Game | Fruit, ComboPopup | Assets/FruitMerge/Data, Assets/FruitMerge/Data/Fruits | OK |
| Persistence | persistence | Assets/Scripts/Persistence/IProfileRepository.cs, Assets/Scripts/Persistence/LocalJsonProfileRepository.cs (+2) | - | - | - | OK |
| Presentation | presentation | Assets/Scripts/Presentation/CharacterPresenter.cs, Assets/Scripts/Presentation/ArmIkSolver.cs (+5) | - | Chloe | - | OK |
| Session | session | Assets/Scripts/Session/BreakOfferPolicy.cs, Assets/Scripts/Session/StudyBlockRunner.cs | Room, VoiceHarness | - | - | OK |
| Tooling | editor | Assets/Editor/BookPageSetup.cs, Assets/Editor/ChloeClipPathFixer.cs (+8) | - | - | - | OK |
| UI | ui | Assets/Scripts/UI/PushToTalkButtonView.cs | Room, VoiceHarness, Game, SampleScene | - | - | OK |
| Voice | voice | Assets/Scripts/Voice/GeminiLiveMessages.cs, Assets/Scripts/Voice/GeminiLiveVoiceSession.cs (+2) | Room, VoiceHarness | - | - | OK |

## Gaps
- none
