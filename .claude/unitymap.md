<!-- stamp: 2026-08-18T07:36Z source-sig:c4cb7d066879 scenes:9 prefabs:198 generator:python-fallback status: DEGRADED 168 missing-script -->
# unitymap — scene and prefab structure

Read this instead of opening a `.unity`/`.prefab` file. Tree indentation is
the GameObject hierarchy; `[...]` lists the components on the object;
`refs:` lists serialized reference slots and whether the Inspector has
something in them. `*` marks a prefab instance.

Staleness: `source-sig` is derived from scene/prefab mtimes. Regenerate with
`python3 .claude/hooks/build_unitymap.py` or, for real type information, the
Unity menu item Tools > unity-dev > Export unitymap.

## Findings
- MISSING SCRIPT | Assets/FruitMerge/Prefabs/ComboPopup.prefab | ComboPopup
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | Background
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | BestCaption
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | BestLabel
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | BoostSlot
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | BoostSlot_Quake
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | Box
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | CloseButton
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | Cloud1
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | Cloud2
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | Cloud3
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | Cloud4
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | CountBadge
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | Dimmer
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | EventSystem
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | Face
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | FruitChainPanel
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | FruitPile
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | GameOverPanel
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | Glow
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | HUDCanvas
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | HeaderRibbon
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | HighScoreText
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | HudPanel
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | Icon
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | Label
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | MainCanvas
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | MenuButton
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | MenuPanel
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | MusicButton
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | MusicIcon
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | NewRecordRibbon
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | PanelCanvas
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | PauseButton
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | PausePanel
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | PlayButton
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | PlusBadge
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | RestartButton
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | ResumeButton
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | ScoreCaption
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | ScoreLabel
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | ScoreText
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | SfxButton
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | SfxIcon
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | Slot_00
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | Slot_01
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | Slot_02
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | Slot_03
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | Slot_04
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | Slot_05
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | Slot_06
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | Slot_07
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | Slot_08
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | Slot_09
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | Slot_10
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | SplashPanel
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | Star1
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | Star2
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | Star3
- MISSING SCRIPT | Assets/FruitMerge/Scenes/Game.unity | Text (TMP)
- ... 97 more

## PREFAB Assets/FruitMerge/Prefabs/ComboPopup.prefab   (1 object(s))
- ComboPopup  [RectTransform, MeshRenderer, MISSING SCRIPT (guid:9541d86e), ComboPopupItem]  refs: parentLinkedComponent=NULL

## PREFAB Assets/FruitMerge/Prefabs/Fruit.prefab   (2 object(s))
- Fruit  [Transform, SpriteRenderer, CircleCollider2D, Rigidbody2D, Fruit]  refs: _face=set, _faceSet=set
  - Face  [Transform, SpriteRenderer, FruitFace]

## SCENE Assets/FruitMerge/Scenes/Game.unity   (164 object(s))
- SaveService  [SaveService, Transform]
- AudioService  [AudioService, Transform]  refs: _config=set, _database=set, _dropSfx=set, _gameOverSfx=set, _maxTierSfx=set, _mergeSfx=set, _musicClip=set, _newRecordSfx=set, _panelCloseSfx=set, _panelOpenSfx=set, _quakeCrackSfx=set, _quakeRumbleSfx=set, _starSfx=set, _toggleOffSfx=set, _toggleOnSfx=set, _uiClickSfx=set
- ComboPopupDirector  [ComboPopupDirector, Transform]  refs: _config=set, _parent=set, _prefab=set
- ConfettiDirector  [ConfettiDirector, Transform]  refs: _config=set, _layer=set, _sprites=set, _worldCamera=set
- EventSystem  [MISSING SCRIPT (guid:01614664), MISSING SCRIPT (guid:76c392e4), Transform]
- Gameplay  [Transform]
  - DropZone  [Transform, DropController]  refs: _camera=set, _config=set, _dropIndicator=set, _nextDisplay=set, _pendingParent=set, _pool=set, _spawnQueue=set
    - PendingFruit  [Transform]
    - DropIndicator  [Transform, SpriteRenderer, DropIndicatorController]  refs: _config=set, _floor=set
    - DropperBranch  [Transform, SpriteRenderer]
    - NextFruit  [Transform, NextFruitDisplay, SpriteRenderer]  refs: _config=set, _faceRenderer=set, _faceSet=set
      - Face  [Transform, SpriteRenderer]
  - ActiveFruits  [Transform]
- EffectDirector  [EffectDirector, Transform]  refs: _database=set, _eatSmoke=set, _juiceDroplets=set, _juiceMist=set, _quakeDust=set, _quakeRubble=set
  - JuiceDroplets  [ParticleSystem, ParticleSystemRenderer, Transform]  refs: LightsModule=NULL, ShapeModule=NULL, SubModule=NULL, UVModule=NULL, moveWithCustomTransform=NULL
  - JuiceMist  [ParticleSystem, ParticleSystemRenderer, Transform]  refs: LightsModule=NULL, ShapeModule=NULL, SubModule=NULL, UVModule=NULL, moveWithCustomTransform=NULL
  - EatSmoke  [ParticleSystem, ParticleSystemRenderer, Transform]  refs: LightsModule=NULL, ShapeModule=NULL, SubModule=NULL, UVModule=NULL, moveWithCustomTransform=NULL
  - QuakeDust  [ParticleSystem, ParticleSystemRenderer, Transform]  refs: LightsModule=NULL, ShapeModule=NULL, SubModule=NULL, UVModule=NULL, moveWithCustomTransform=NULL
  - QuakeRubble  [ParticleSystem, ParticleSystemRenderer, Transform]  refs: LightsModule=NULL, ShapeModule=NULL, SubModule=NULL, UVModule=NULL, moveWithCustomTransform=NULL
- MainCanvas  [MISSING SCRIPT (guid:dc42784c), MISSING SCRIPT (guid:0cd44c10), Canvas, RectTransform]
  - HUDCanvas  [RectTransform, MISSING SCRIPT (guid:dc42784c), Canvas, HUDView]  refs: _config=set, _highScoreText=set, _nextFruitImage=NULL, _pauseButton=set, _scoreText=set
    - PauseButton  [RectTransform, MISSING SCRIPT (guid:4e29b1a8), MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
      - Icon  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
    - HudPanel  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
      - ScoreText  [RectTransform, MISSING SCRIPT (guid:f4688fdb), CanvasRenderer, Canvas]  refs: parentLinkedComponent=NULL
      - HighScoreText  [RectTransform, MISSING SCRIPT (guid:f4688fdb), CanvasRenderer]  refs: parentLinkedComponent=NULL
    - FruitChainPanel  [RectTransform, FruitChainView, MISSING SCRIPT (guid:30649d3a), MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]  refs: _config=set, _database=set, _faceIcons=set, _fruitIcons=set
      - Slot_00  [RectTransform, MISSING SCRIPT (guid:306cc8c2)]
        - Icon  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
          - Face  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
      - Slot_01  [RectTransform, MISSING SCRIPT (guid:306cc8c2)]
        - Icon  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
          - Face  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
      - Slot_02  [RectTransform, MISSING SCRIPT (guid:306cc8c2)]
        - Icon  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
          - Face  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
      - Slot_03  [RectTransform, MISSING SCRIPT (guid:306cc8c2)]
        - Icon  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
          - Face  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
      - Slot_04  [RectTransform, MISSING SCRIPT (guid:306cc8c2)]
        - Icon  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
          - Face  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
      - Slot_05  [RectTransform, MISSING SCRIPT (guid:306cc8c2)]
        - Icon  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
          - Face  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
      - Slot_06  [RectTransform, MISSING SCRIPT (guid:306cc8c2)]
        - Icon  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
          - Face  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
      - Slot_07  [RectTransform, MISSING SCRIPT (guid:306cc8c2)]
        - Icon  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
          - Face  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
      - Slot_08  [RectTransform, MISSING SCRIPT (guid:306cc8c2)]
        - Icon  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
          - Face  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
      - Slot_09  [RectTransform, MISSING SCRIPT (guid:306cc8c2)]
        - Icon  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
          - Face  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
      - Slot_10  [RectTransform, MISSING SCRIPT (guid:306cc8c2)]
        - Icon  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
          - Face  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
    - BoostSlot  [RectTransform, BoostButton, CanvasGroup, MISSING SCRIPT (guid:4e29b1a8), MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]  refs: _armedGlow=set, _button=set, _countBadge=set, _countLabel=set, _icon=set, _plusBadge=set
      - Glow [inactive]  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
      - CountBadge  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
        - Label  [RectTransform, MISSING SCRIPT (guid:f4688fdb), CanvasRenderer]  refs: parentLinkedComponent=NULL
      - PlusBadge [inactive]  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
    - BoostSlot_Quake  [RectTransform, BoostButton, CanvasGroup, MISSING SCRIPT (guid:4e29b1a8), MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]  refs: _armedGlow=set, _button=set, _countBadge=set, _countLabel=set, _icon=set, _plusBadge=set
      - Glow [inactive]  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
      - CountBadge  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
        - Label  [RectTransform, MISSING SCRIPT (guid:f4688fdb), CanvasRenderer]  refs: parentLinkedComponent=NULL
      - PlusBadge [inactive]  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
  - PanelCanvas  [RectTransform, MISSING SCRIPT (guid:dc42784c), Canvas]
    - GameOverPanel  [RectTransform, CanvasGroup, GameOverPanel, Canvas, MISSING SCRIPT (guid:dc42784c)]  refs: _bestLabel=set, _config=set, _menuButton=set, _newRecordRibbon=set, _restartButton=set, _scoreLabel=set, _starEmpty=set, _starFilled=set, _stars=set
      - Dimmer  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
      - Box  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
        - Title  [RectTransform, CanvasRenderer, FruitColorTitle, MISSING SCRIPT (guid:f4688fdb)]  refs: _database=set, _label=set, parentLinkedComponent=NULL
        - ScoreCaption  [RectTransform, MISSING SCRIPT (guid:f4688fdb), CanvasRenderer]  refs: parentLinkedComponent=NULL
        - ScoreLabel  [RectTransform, MISSING SCRIPT (guid:f4688fdb), CanvasRenderer]  refs: parentLinkedComponent=NULL
        - BestCaption  [RectTransform, MISSING SCRIPT (guid:f4688fdb), CanvasRenderer]  refs: parentLinkedComponent=NULL
        - BestLabel  [RectTransform, MISSING SCRIPT (guid:f4688fdb), CanvasRenderer]  refs: parentLinkedComponent=NULL
        - RestartButton  [RectTransform, MISSING SCRIPT (guid:4e29b1a8), MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
          - Text (TMP)  [RectTransform, MISSING SCRIPT (guid:f4688fdb), CanvasRenderer]  refs: parentLinkedComponent=NULL
        - Stars  [RectTransform]
          - Star1  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
          - Star2  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
          - Star3  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
        - NewRecordRibbon [inactive]  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
        - MenuButton  [RectTransform, MISSING SCRIPT (guid:4e29b1a8), MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
          - Text (TMP)  [RectTransform, MISSING SCRIPT (guid:f4688fdb), CanvasRenderer]  refs: parentLinkedComponent=NULL
    - PausePanel  [RectTransform, PausePanel, CanvasGroup, Canvas, MISSING SCRIPT (guid:dc42784c)]  refs: _closeButton=set, _menuButton=set, _musicButton=set, _musicIcon=set, _musicOffSprite=set, _musicOnSprite=set, _restartButton=set, _resumeButton=set, _sfxButton=set, _sfxIcon=set, _sfxOffSprite=set, _sfxOnSprite=set, _vibrationButton=set, _vibrationIcon=set, _vibrationOffSprite=set, _vibrationOnSprite=set
      - Dimmer  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
      - Box  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
        - HeaderRibbon  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
          - Title  [RectTransform, MISSING SCRIPT (guid:f4688fdb), CanvasRenderer]  refs: parentLinkedComponent=NULL
        - CloseButton  [RectTransform, MISSING SCRIPT (guid:4e29b1a8), MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
        - Settings  [RectTransform]
          - SfxButton  [RectTransform, MISSING SCRIPT (guid:4e29b1a8), MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
            - SfxIcon  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
          - MusicButton  [RectTransform, MISSING SCRIPT (guid:4e29b1a8), MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
            - MusicIcon  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
          - VibrationButton  [RectTransform, MISSING SCRIPT (guid:4e29b1a8), MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
            - VibrationIcon  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
        - ResumeButton  [RectTransform, MISSING SCRIPT (guid:4e29b1a8), MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
          - Icon  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
          - Text (TMP)  [RectTransform, MISSING SCRIPT (guid:f4688fdb), CanvasRenderer]  refs: parentLinkedComponent=NULL
        - RestartButton  [RectTransform, MISSING SCRIPT (guid:4e29b1a8), MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
          - Icon  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
          - Text (TMP)  [RectTransform, MISSING SCRIPT (guid:f4688fdb), CanvasRenderer]  refs: parentLinkedComponent=NULL
        - MenuButton  [RectTransform, MISSING SCRIPT (guid:4e29b1a8), MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
          - Icon  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
          - Text (TMP)  [RectTransform, MISSING SCRIPT (guid:f4688fdb), CanvasRenderer]  refs: parentLinkedComponent=NULL
    - MenuPanel  [RectTransform, MenuPanel, CanvasGroup, Canvas, MISSING SCRIPT (guid:dc42784c)]  refs: _playButton=set
      - Background  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer, ScreenBackground]  refs: _config=set
      - Cloud1  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
      - Cloud2  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
      - Cloud3  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
      - Cloud4  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
      - FruitPile  [RectTransform, MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
      - PlayButton  [RectTransform, MISSING SCRIPT (guid:4e29b1a8), MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer]
        - Text (TMP)  [RectTransform, MISSING SCRIPT (guid:f4688fdb), CanvasRenderer]  refs: parentLinkedComponent=NULL
    - SplashPanel  [RectTransform, SplashPanel, CanvasGroup, Canvas, MISSING SCRIPT (guid:dc42784c)]  refs: _config=set, _fill=set
- ... 44 more object(s) not listed; regenerate with the Editor exporter for the full tree

## SCENE Assets/LowPolyBoy/FreeStylizedBedRoom/DemoScene/DemoScene_01_template.unity   (33 object(s))
- Directional Light  [Light, Transform, MISSING SCRIPT (guid:474bcb49)]
- Camera  [MISSING SCRIPT (guid:a79441f3), AudioListener, Camera, Transform, MISSING SCRIPT (guid:17251560)]  refs: sharedProfile=set
- models  [Transform]
- * lpbns_br_plants_01  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_plants_01.prefab)
- * lpbns_br_lamp  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_lamp.prefab)
- * lpbns_br_computer_mousepad  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_computer_mousepad.prefab)
- * lpbns_br_computer_keyboard  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_computer_keyboard.prefab)
- * lpbns_br_plants_02  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_plants_02.prefab)
- * lpbns_br_computer_monitor_02  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_computer_monitor_02.prefab)
- * lpbns_br_computer_monitor_01  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_computer_monitor_01.prefab)
- * lpbns_br_curtain  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_curtain.prefab)
- * lpbns_br_wall_door  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_wall_door.prefab)
- * lpbns_br_desk  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_desk.prefab)
- * lpbns_br_slipper  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_slipper.prefab)
- * lpbns_br_wall  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_wall.prefab)
- * lpbns_br_window  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_window.prefab)
- * lpbns_br_cabinet_01  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_cabinet_01.prefab)
- * lpbns_br_cup  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_cup.prefab)
- * lpbns_br_plants_00  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_plants_00.prefab)
- * lpbns_br_floor  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_floor.prefab)
- * lpbns_br_console  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_console.prefab)
- * lpbns_br_door  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_door.prefab)
- * lpbns_br_bag  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_bag.prefab)
- * lpbns_br_chair  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_chair.prefab)
- * lpbns_br_desk_chair  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_desk_chair.prefab)
- * lpbns_br_book  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_book.prefab)
- * lpbns_br_rug  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_rug.prefab)
- * lpbns_br_bed  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_bed.prefab)
- * lpbns_br_frame  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_frame.prefab)
- * lpbns_br_computer_mouse  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_computer_mouse.prefab)
- * lpbns_br_plants_03  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_plants_03.prefab)
- * lpbns_br_table  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_table.prefab)
- * lpbns_br_wall_window  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_wall_window.prefab)

## PREFAB Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_bag.prefab   (1 object(s))
- lpbns_br_bag  [Transform, MeshFilter, MeshRenderer, BoxCollider]

## PREFAB Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_bed.prefab   (1 object(s))
- lpbns_br_bed  [Transform, MeshFilter, MeshRenderer, BoxCollider]

## PREFAB Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_book.prefab   (1 object(s))
- lpbns_br_book  [Transform, MeshFilter, MeshRenderer, BoxCollider]

## PREFAB Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_cabinet_01.prefab   (1 object(s))
- lpbns_br_cabinet_01  [Transform, MeshFilter, MeshRenderer, BoxCollider]

## PREFAB Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_chair.prefab   (1 object(s))
- lpbns_br_chair  [Transform, MeshFilter, MeshRenderer, BoxCollider]

## PREFAB Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_computer_keyboard.prefab   (1 object(s))
- lpbns_br_computer_keyboard  [Transform, MeshFilter, MeshRenderer, BoxCollider]

## PREFAB Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_computer_monitor_01.prefab   (1 object(s))
- lpbns_br_computer_monitor_01  [Transform, MeshFilter, MeshRenderer, BoxCollider]

## PREFAB Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_computer_monitor_02.prefab   (1 object(s))
- lpbns_br_computer_monitor_02  [Transform, MeshFilter, MeshRenderer, BoxCollider]

## PREFAB Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_computer_mouse.prefab   (1 object(s))
- lpbns_br_computer_mouse  [Transform, MeshFilter, MeshRenderer, BoxCollider]

## PREFAB Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_computer_mousepad.prefab   (1 object(s))
- lpbns_br_computer_mousepad  [Transform, MeshFilter, MeshRenderer, BoxCollider]

## PREFAB Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_console.prefab   (1 object(s))
- lpbns_br_console  [Transform, MeshFilter, MeshRenderer, BoxCollider]

## PREFAB Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_cup.prefab   (1 object(s))
- lpbns_br_cup  [Transform, MeshFilter, MeshRenderer, BoxCollider]

## PREFAB Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_curtain.prefab   (1 object(s))
- lpbns_br_curtain  [Transform, MeshFilter, MeshRenderer, BoxCollider]

## PREFAB Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_desk.prefab   (6 object(s))
- lpbns_br_desk  [Transform, MeshFilter, MeshRenderer, BoxCollider]
  - lpbns_br_desk_drawer_01  [Transform, MeshFilter, MeshRenderer]
  - lpbns_br_desk_drawer_02  [Transform, MeshFilter, MeshRenderer]
  - lpbns_br_desk_drawer_03  [Transform, MeshFilter, MeshRenderer]
  - lpbns_br_desk_drawer_04  [Transform, MeshFilter, MeshRenderer]
  - lpbns_br_desk_drawer_05  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_desk_chair.prefab   (1 object(s))
- lpbns_br_desk_chair  [Transform, MeshFilter, MeshRenderer, BoxCollider]

## PREFAB Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_door.prefab   (1 object(s))
- lpbns_br_door  [Transform, MeshFilter, MeshRenderer, BoxCollider]

## PREFAB Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_floor.prefab   (1 object(s))
- lpbns_br_floor  [Transform, MeshFilter, MeshRenderer, BoxCollider]

## PREFAB Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_frame.prefab   (1 object(s))
- lpbns_br_frame  [Transform, MeshFilter, MeshRenderer, BoxCollider]

## PREFAB Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_lamp.prefab   (1 object(s))
- lpbns_br_lamp  [Transform, MeshFilter, MeshRenderer, BoxCollider]

## PREFAB Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_plants_00.prefab   (1 object(s))
- lpbns_br_plants_00  [Transform, MeshFilter, MeshRenderer, BoxCollider]

## PREFAB Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_plants_01.prefab   (1 object(s))
- lpbns_br_plants_01  [Transform, MeshFilter, MeshRenderer, BoxCollider]

## PREFAB Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_plants_02.prefab   (1 object(s))
- lpbns_br_plants_02  [Transform, MeshFilter, MeshRenderer, BoxCollider]

## PREFAB Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_plants_03.prefab   (1 object(s))
- lpbns_br_plants_03  [Transform, MeshFilter, MeshRenderer, BoxCollider]

## PREFAB Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_rug.prefab   (1 object(s))
- lpbns_br_rug  [Transform, MeshFilter, MeshRenderer, BoxCollider]

## PREFAB Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_slipper.prefab   (1 object(s))
- lpbns_br_slipper  [Transform, MeshFilter, MeshRenderer, BoxCollider]

## PREFAB Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_table.prefab   (1 object(s))
- lpbns_br_table  [Transform, MeshFilter, MeshRenderer, BoxCollider]

## PREFAB Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_wall.prefab   (1 object(s))
- lpbns_br_wall  [Transform, MeshFilter, MeshRenderer, BoxCollider]

## PREFAB Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_wall_door.prefab   (1 object(s))
- lpbns_br_wall_door  [Transform, MeshFilter, MeshRenderer, BoxCollider]

## PREFAB Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_wall_window.prefab   (1 object(s))
- lpbns_br_wall_window  [Transform, MeshFilter, MeshRenderer, BoxCollider]

## PREFAB Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_window.prefab   (1 object(s))
- lpbns_br_window  [Transform, MeshFilter, MeshRenderer, BoxCollider]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Armchair_Classic.prefab   (1 object(s))
- Armchair_Classic  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Armchair_Classic_2.prefab   (1 object(s))
- Armchair_Classic_2  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Book_1.prefab   (1 object(s))
- Book_1  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Book_10.prefab   (1 object(s))
- Book_10  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Book_2.prefab   (1 object(s))
- Book_2  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Book_3.prefab   (1 object(s))
- Book_3  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Book_4.prefab   (1 object(s))
- Book_4  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Book_5.prefab   (1 object(s))
- Book_5  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Book_6.prefab   (1 object(s))
- Book_6  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Book_7.prefab   (1 object(s))
- Book_7  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Book_8.prefab   (1 object(s))
- Book_8  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Book_9.prefab   (1 object(s))
- Book_9  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Bookshelf_Tall.prefab   (1 object(s))
- Bookshelf_Tall  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Box_1.prefab   (1 object(s))
- Box_1  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Box_2.prefab   (1 object(s))
- Box_2  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Box_Open.prefab   (1 object(s))
- Box_Open  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Carpet_1.prefab   (1 object(s))
- Carpet_1  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Carpet_2.prefab   (1 object(s))
- Carpet_2  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Clock_WallRound.prefab   (1 object(s))
- Clock_WallRound  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Coffee_table_1.prefab   (1 object(s))
- Coffee_table_1  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Coffee_table_2.prefab   (1 object(s))
- Coffee_table_2  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Coffee_table_3.prefab   (1 object(s))
- Coffee_table_3  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Coffee_table_4.prefab   (1 object(s))
- Coffee_table_4  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Coffee_table_5.prefab   (1 object(s))
- Coffee_table_5  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Console_Modern.prefab   (1 object(s))
- Console_Modern  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/DVD_Player.prefab   (1 object(s))
- DVD_Player  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/DeskClock_1.prefab   (1 object(s))
- DeskClock_1  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/DeskClock_2.prefab   (1 object(s))
- DeskClock_2  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Gamepad_Classic.prefab   (1 object(s))
- Gamepad_Classic  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Hanger.prefab   (1 object(s))
- Hanger  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Lamp_Tall.prefab   (1 object(s))
- Lamp_Tall  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Mirror.prefab   (1 object(s))
- Mirror  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Mug.prefab   (1 object(s))
- Mug  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Newspaper.prefab   (1 object(s))
- Newspaper  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Night_light_1.prefab   (1 object(s))
- Night_light_1  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Night_light_2.prefab   (1 object(s))
- Night_light_2  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Night_light_3.prefab   (1 object(s))
- Night_light_3  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Night_stand_1.prefab   (1 object(s))
- Night_stand_1  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Night_stand_2.prefab   (1 object(s))
- Night_stand_2  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Night_stand_3.prefab   (1 object(s))
- Night_stand_3  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Night_stand_4.prefab   (1 object(s))
- Night_stand_4  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Nightstand_1.prefab   (1 object(s))
- Nightstand_1  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Nightstand_2.prefab   (1 object(s))
- Nightstand_2  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Painting_Modern_1.prefab   (1 object(s))
- Painting_Modern_1  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Painting_Modern_2.prefab   (1 object(s))
- Painting_Modern_2  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Painting_Modern_3.prefab   (1 object(s))
- Painting_Modern_3  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Pillow_Square_1.prefab   (1 object(s))
- Pillow_Square_1  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Pillow_Square_2.prefab   (1 object(s))
- Pillow_Square_2  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/PottedPlant_Small_1.prefab   (1 object(s))
- PottedPlant_Small_1  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/PottedPlant_Small_2.prefab   (1 object(s))
- PottedPlant_Small_2  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/PottedPlant_Small_3.prefab   (1 object(s))
- PottedPlant_Small_3  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/PottedPlant_Tall_1.prefab   (1 object(s))
- PottedPlant_Tall_1  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/PottedPlant_Tall_2.prefab   (1 object(s))
- PottedPlant_Tall_2  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/PottedPlant_Tall_3.prefab   (1 object(s))
- PottedPlant_Tall_3  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Side_table_1.prefab   (1 object(s))
- Side_table_1  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Sofa_2Seat.prefab   (1 object(s))
- Sofa_2Seat  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Speaker_Floor.prefab   (1 object(s))
- Speaker_Floor  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Speaker_Soundbar.prefab   (1 object(s))
- Speaker_Soundbar  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Stereo_MainUnit.prefab   (1 object(s))
- Stereo_MainUnit  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Stereo_Speaker.prefab   (1 object(s))
- Stereo_Speaker  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Stool_1.prefab   (1 object(s))
- Stool_1  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Stool_2.prefab   (1 object(s))
- Stool_2  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Stool_3.prefab   (1 object(s))
- Stool_3  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Stool_4.prefab   (1 object(s))
- Stool_4  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/TV_Modern.prefab   (1 object(s))
- TV_Modern  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/TV_Retro.prefab   (1 object(s))
- TV_Retro  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/TV_remote_control.prefab   (1 object(s))
- TV_remote_control  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Table.prefab   (1 object(s))
- Table  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Vase_1.prefab   (1 object(s))
- Vase_1  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Vase_2.prefab   (1 object(s))
- Vase_2  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/Prefabs/Vase_3.prefab   (1 object(s))
- Vase_3  [Transform, MeshFilter, MeshRenderer]

## PREFAB Assets/LowPolyLivingRoomPack/ScenesDemo/LowPolyLivingRoomPack_Demo.prefab   (72 object(s))
- LowPolyLivingRoomPack_Demo  [Transform, MeshFilter, MeshRenderer]
  - Armchair_Classic  [Transform, MeshFilter, MeshRenderer]
  - Armchair_Classic_2  [Transform, MeshFilter, MeshRenderer]
  - Book_1  [Transform, MeshFilter, MeshRenderer]
  - Book_2  [Transform, MeshFilter, MeshRenderer]
  - Book_3  [Transform, MeshFilter, MeshRenderer]
  - Book_4  [Transform, MeshFilter, MeshRenderer]
  - Book_5  [Transform, MeshFilter, MeshRenderer]
  - Book_6  [Transform, MeshFilter, MeshRenderer]
  - Book_7  [Transform, MeshFilter, MeshRenderer]
  - Book_8  [Transform, MeshFilter, MeshRenderer]
  - Book_9  [Transform, MeshFilter, MeshRenderer]
  - Book_10  [Transform, MeshFilter, MeshRenderer]
  - Bookshelf_Tall  [Transform, MeshFilter, MeshRenderer]
  - Box_1  [Transform, MeshFilter, MeshRenderer]
  - Box_2  [Transform, MeshFilter, MeshRenderer]
  - Box_Open  [Transform, MeshFilter, MeshRenderer]
  - Carpet_1  [Transform, MeshFilter, MeshRenderer]
  - Carpet_2  [Transform, MeshFilter, MeshRenderer]
  - Clock_WallRound  [Transform, MeshFilter, MeshRenderer]
  - Coffee_table_1  [Transform, MeshFilter, MeshRenderer]
  - Coffee_table_2  [Transform, MeshFilter, MeshRenderer]
  - Coffee_table_3  [Transform, MeshFilter, MeshRenderer]
  - Coffee_table_4  [Transform, MeshFilter, MeshRenderer]
  - Coffee_table_5  [Transform, MeshFilter, MeshRenderer]
  - Console_Modern  [Transform, MeshFilter, MeshRenderer]
  - DeskClock_1  [Transform, MeshFilter, MeshRenderer]
  - DeskClock_2  [Transform, MeshFilter, MeshRenderer]
  - DVD_Player  [Transform, MeshFilter, MeshRenderer]
  - Gamepad_Classic  [Transform, MeshFilter, MeshRenderer]
  - Hanger  [Transform, MeshFilter, MeshRenderer]
  - Lamp_Tall  [Transform, MeshFilter, MeshRenderer]
  - Mirror  [Transform, MeshFilter, MeshRenderer]
  - Mug  [Transform, MeshFilter, MeshRenderer]
  - Newspaper  [Transform, MeshFilter, MeshRenderer]
  - Night_light_1  [Transform, MeshFilter, MeshRenderer]
  - Night_light_2  [Transform, MeshFilter, MeshRenderer]
  - Night_light_3  [Transform, MeshFilter, MeshRenderer]
  - Night_stand_1  [Transform, MeshFilter, MeshRenderer]
  - Night_stand_2  [Transform, MeshFilter, MeshRenderer]
  - Night_stand_3  [Transform, MeshFilter, MeshRenderer]
  - Night_stand_4  [Transform, MeshFilter, MeshRenderer]
  - Nightstand_1  [Transform, MeshFilter, MeshRenderer]
  - Nightstand_2  [Transform, MeshFilter, MeshRenderer]
  - Painting_Modern_1  [Transform, MeshFilter, MeshRenderer]
  - Painting_Modern_2  [Transform, MeshFilter, MeshRenderer]
  - Painting_Modern_3  [Transform, MeshFilter, MeshRenderer]
  - Pillow_Square_1  [Transform, MeshFilter, MeshRenderer]
  - Pillow_Square_2  [Transform, MeshFilter, MeshRenderer]
  - PottedPlant_Small_1  [Transform, MeshFilter, MeshRenderer]
  - PottedPlant_Small_2  [Transform, MeshFilter, MeshRenderer]
  - PottedPlant_Small_3  [Transform, MeshFilter, MeshRenderer]
  - PottedPlant_Tall_1  [Transform, MeshFilter, MeshRenderer]
  - PottedPlant_Tall_2  [Transform, MeshFilter, MeshRenderer]
  - PottedPlant_Tall_3  [Transform, MeshFilter, MeshRenderer]
  - Side_table_1  [Transform, MeshFilter, MeshRenderer]
  - Sofa_2Seat  [Transform, MeshFilter, MeshRenderer]
  - Speaker_Floor  [Transform, MeshFilter, MeshRenderer]
  - Speaker_Soundbar  [Transform, MeshFilter, MeshRenderer]
  - Stereo_MainUnit  [Transform, MeshFilter, MeshRenderer]
  - Stereo_Speaker  [Transform, MeshFilter, MeshRenderer]
  - Stool_1  [Transform, MeshFilter, MeshRenderer]
  - Stool_2  [Transform, MeshFilter, MeshRenderer]
  - Stool_3  [Transform, MeshFilter, MeshRenderer]
  - Stool_4  [Transform, MeshFilter, MeshRenderer]
  - Table  [Transform, MeshFilter, MeshRenderer]
  - TV_Modern  [Transform, MeshFilter, MeshRenderer]
  - TV_remote_control  [Transform, MeshFilter, MeshRenderer]
  - TV_Retro  [Transform, MeshFilter, MeshRenderer]
  - Vase_1  [Transform, MeshFilter, MeshRenderer]
  - Vase_2  [Transform, MeshFilter, MeshRenderer]
  - Vase_3  [Transform, MeshFilter, MeshRenderer]

## SCENE Assets/LowPolyLivingRoomPack/ScenesDemo/LowPolyLivingRoomPack_Demo.unity   (3 object(s))
- 'Main Camera '  [AudioListener, Camera, Transform]
- Sun  [Light, Transform]
- * LowPolyLivingRoomPack_Demo  (prefab instance of Assets/LowPolyLivingRoomPack/ScenesDemo/LowPolyLivingRoomPack_Demo.prefab)

## PREFAB Assets/PolyOne/Cartoon Dog, Cat/Prefab/SM_CartoonAnimal_Cat.prefab   (31 object(s))
- SM_CartoonAnimal_Cat  [Transform, Animator]
  - Root  [Transform]
    - LeftUpLeg  [Transform]
      - LeftLeg  [Transform]
        - LeftFoot  [Transform]
          - LeftToeBase  [Transform]
    - RightUpLeg  [Transform]
      - RightLeg  [Transform]
        - RightFoot  [Transform]
          - RightToeBase  [Transform]
    - Spine  [Transform]
      - Spine1  [Transform]
        - Spine2  [Transform]
          - LeftShoulder  [Transform]
            - LeftArm  [Transform]
              - LeftForeArm  [Transform]
                - LeftHand  [Transform]
          - Neck  [Transform]
            - Head  [Transform]
              - HeadTop_End  [Transform]
                - LeftEar  [Transform]
                - RightEar  [Transform]
          - RightShoulder  [Transform]
            - RightArm  [Transform]
              - RightForeArm  [Transform]
                - RightHand  [Transform]
    - Tail  [Transform]
      - Tail1  [Transform]
        - Tail2  [Transform]
          - Tail3  [Transform]
  - SM_CartoonAnimal_Cat  [Transform, SkinnedMeshRenderer]

## PREFAB Assets/PolyOne/Cartoon Dog, Cat/Prefab/SM_CartoonAnimal_Dog.prefab   (33 object(s))
- SM_CartoonAnimal_Dog  [Transform, Animator]
  - Root  [Transform]
    - LeftUpLeg  [Transform]
      - LeftLeg  [Transform]
        - LeftFoot  [Transform]
          - LeftToeBase  [Transform]
    - Neck  [Transform]
      - Head  [Transform]
        - HeadTop_End  [Transform]
          - LeftEar  [Transform]
            - LeftEar1  [Transform]
          - RightEar  [Transform]
            - RightEar1  [Transform]
    - RightUpLeg  [Transform]
      - RightLeg  [Transform]
        - RightFoot  [Transform]
          - RightToeBase  [Transform]
    - Spine  [Transform]
      - Spine1  [Transform]
        - Spine2  [Transform]
          - LeftShoulder  [Transform]
            - LeftArm  [Transform]
              - LeftForeArm  [Transform]
                - LeftHand  [Transform]
          - RightShoulder  [Transform]
            - RightArm  [Transform]
              - RightForeArm  [Transform]
                - RightHand  [Transform]
    - Tail  [Transform]
      - Tail1  [Transform]
        - Tail2  [Transform]
          - Tail3  [Transform]
  - SM_CartoonAnimal_Dog  [Transform, SkinnedMeshRenderer]

## SCENE Assets/PolyOne/Cartoon Dog, Cat/Scene/Demo Scene - Cartoon Dog, Cat.unity   (6 object(s))
- Scene  [Transform]
  - Main Camera  [AudioListener, Camera, Transform, MISSING SCRIPT (guid:a79441f3)]
  - Directional Light  [Light, Transform, MISSING SCRIPT (guid:474bcb49)]
  - Plane  [Transform, MeshCollider, MeshRenderer, MeshFilter]
- * SM_CartoonAnimal_Cat  (prefab instance of Assets/PolyOne/Cartoon Dog, Cat/Prefab/SM_CartoonAnimal_Cat.prefab)
- * SM_CartoonAnimal_Dog  (prefab instance of Assets/PolyOne/Cartoon Dog, Cat/Prefab/SM_CartoonAnimal_Dog.prefab)

## PREFAB Assets/Prefabs/Chloe.prefab   variant-of: Assets/Art/Character/deneme.fbx   (2 object(s))
- * Chloe  (prefab instance of Assets/Art/Character/deneme.fbx)

## SCENE Assets/Scenes/Room.unity   (181 object(s))
- wall  [Transform]
  - wall_window  [Transform]
  - wall_door  [Transform]
- camera_game  [MISSING SCRIPT (guid:a79441f3), AudioListener, Camera, Transform]
- Quad (1)  [MeshCollider, MeshRenderer, MeshFilter, Transform]
- Cat  [PetRoamer, Transform]  refs: animator=set
- --GameMode--  [GameModeController, Transform]  refs: chloe=set, chloeAnimator=set, danceMode=set, gameCamera=set, mainCamera=set, pets=set, roomUi=NULL, zoomCamera=set
- ground  [Transform]
- Quad  [MeshCollider, MeshRenderer, MeshFilter, Transform]
- spot_light  [Transform]
  - lamp (1)  [Transform, MeshRenderer, MeshFilter]
    - Spot Light (1)  [Transform, MISSING SCRIPT (guid:474bcb49), Light]
  - lamp (6)  [Transform, MeshRenderer, MeshFilter]
    - Spot Light (2)  [Transform, MISSING SCRIPT (guid:474bcb49), Light]
  - lamp (7)  [Transform, MeshRenderer, MeshFilter]
    - Spot Light (3)  [Transform, MISSING SCRIPT (guid:474bcb49), Light]
  - lamp (8)  [Transform, MeshRenderer, MeshFilter]
    - Spot Light (4)  [Transform, MISSING SCRIPT (guid:474bcb49), Light]
  - lamp (2)  [Transform, MeshRenderer, MeshFilter]
    - Spot Light (5)  [Transform, MISSING SCRIPT (guid:474bcb49), Light]
  - lamp (3)  [Transform, MeshRenderer, MeshFilter]
    - Spot Light (6)  [Transform, MISSING SCRIPT (guid:474bcb49), Light]
- camera_game_zoom  [MISSING SCRIPT (guid:a79441f3), Camera, Transform]
- --DanceMode--  [AudioSource, DanceModeController, Transform]  refs: OutputAudioMixerGroup=NULL, chloe=set, chloeAnimator=set, danceCamera=set, discoBall=set, discoFixtures=set, mainCamera=set, musicSource=set, noteParticles=set, partyPets=set, roomLights=set, tracks=set
- Camera_dance  [MISSING SCRIPT (guid:a79441f3), AudioListener, Camera, Transform]
- EventSystem  [MISSING SCRIPT (guid:76c392e4), Transform, MISSING SCRIPT (guid:01614664)]
- books  [Transform]
- --Systems--  [Transform]
  - SessionController  [Transform, RoomSessionController, GeminiLiveVoiceSession]  refs: character=set, danceMode=set, gameMode=set, persona=set, session=set, studyMode=set, talkButton=set, config=set
- spotlight_disco  [Transform]
  - Dance_Floor_Spotlight.001  [Transform, MeshRenderer, MeshFilter]
    - r  [Transform, CapsuleCollider, MeshRenderer, MeshFilter]
    - Spot Light  [Transform, MISSING SCRIPT (guid:474bcb49), Light]
  - Dance_Floor_Spotlight.001 (2)  [Transform, MeshRenderer, MeshFilter]
    - r  [Transform, CapsuleCollider, MeshRenderer, MeshFilter]
    - Spot Light  [Transform, MISSING SCRIPT (guid:474bcb49), Light]
  - Dance_Floor_Spotlight.001 (3)  [Transform, MeshRenderer, MeshFilter]
    - r  [Transform, CapsuleCollider, MeshRenderer, MeshFilter]
    - Spot Light  [Transform, MISSING SCRIPT (guid:474bcb49), Light]
  - Dance_Floor_Spotlight.001 (1)  [Transform, MeshRenderer, MeshFilter]
    - r  [Transform, CapsuleCollider, MeshRenderer, MeshFilter]
    - Spot Light  [Transform, MISSING SCRIPT (guid:474bcb49), Light]
- gardrop  [Transform]
  - lpbns_br_desk_drawer_01 (1)  [Transform, MeshRenderer, MeshFilter]
  - lpbns_br_desk_drawer_01 (2)  [Transform, MeshRenderer, MeshFilter]
- Dog  [PetRoamer, Transform]  refs: animator=set
- Main Camera  [AudioListener, Camera, Transform, MISSING SCRIPT (guid:a79441f3)]
- --SessionUI--  [PushToTalkButtonView, MISSING SCRIPT (guid:dc42784c), MISSING SCRIPT (guid:0cd44c10), Canvas, RectTransform]  refs: button=set, idleSprite=set, image=set, listeningSprite=set
  - TalkButton  [MISSING SCRIPT (guid:fe87c0e1), CanvasRenderer, RectTransform, MISSING SCRIPT (guid:4e29b1a8)]
- * lpbns_br_floor  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_floor.prefab)
- * lpbns_br_computer_mousepad  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Models/lpbns_br_computer_mousepad.fbx)
- * lpbns_br_floor (1)  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_floor.prefab)
- * lpbns_br_wall_door  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_wall_door.prefab)
- * lpbns_br_floor (1)  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_floor.prefab)
- * lpbns_br_floor  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_floor.prefab)
- * lpbns_br_floor  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_floor.prefab)
- * Book_1 (2)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Book_1.prefab)
- * Plant_2  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Plant_2.prefab)
- * Book_3  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Book_3.prefab)
- * lpbns_br_floor  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_floor.prefab)
- * lpbns_br_floor  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_floor.prefab)
- * lpbns_br_floor (1)  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_floor.prefab)
- * Book_5 (4)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Book_5.prefab)
- * lpbns_br_wall (4)  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_wall.prefab)
- * lpbns_br_floor (1)  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_floor.prefab)
- * SM_CartoonAnimal_Dog  (prefab instance of Assets/PolyOne/Cartoon Dog, Cat/Prefab/SM_CartoonAnimal_Dog.prefab)
- * lpbns_br_wall (2)  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_wall.prefab)
- * Book  (prefab instance of Assets/Art/Book/Book.fbx)
- * Book_5 (5)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Book_5.prefab)
- * lpbns_br_floor (1)  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_floor.prefab)
- * lpbns_br_floor (1)  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_floor.prefab)
- * lpbns_br_computer_keyboard  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_computer_keyboard.prefab)
- * lpbns_br_floor  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_floor.prefab)
- * Headphone  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Headphone.prefab)
- * lpbns_br_floor (1)  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_floor.prefab)
- * Decor_Game_Controller  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Decor_Game_Controller.prefab)
- * Lamp  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Lamp.prefab)
- * lpbns_br_floor (1)  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_floor.prefab)
- * lpbns_br_floor  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_floor.prefab)
- * Book_2 (4)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Book_2.prefab)
- * lpbns_br_floor (1)  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_floor.prefab)
- * Book_1  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Book_1.prefab)
- * lpbns_br_floor  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_floor.prefab)
- * lpbns_br_floor  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_floor.prefab)
- * Game_Controller_Orange  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Game_Controller_Orange.prefab)
- * lpbns_br_curtain (1)  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_curtain.prefab)
- * Globe  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Globe.prefab)
- * Book_2  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Book_2.prefab)
- * Disco_Speaker (2)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Disco_Speaker.prefab)
- * lpbns_br_floor  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_floor.prefab)
- * Book_5 (1)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Book_5.prefab)
- * Book_5 (2)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Book_5.prefab)
- * lpbns_br_computer_monitor_01  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_computer_monitor_01.prefab)
- * lpbns_br_floor  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_floor.prefab)
- * Book_2 (3)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Book_2.prefab)
- * Disco_Speaker (1)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Disco_Speaker.prefab)
- * lpbns_br_floor  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_floor.prefab)
- * lpbns_br_floor (1)  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_floor.prefab)
- * lpbns_br_wall  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_wall.prefab)
- * lpbns_br_desk (1)  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_desk.prefab)
- * lpbns_br_window  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_window.prefab)
- * Ground_Default (1)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Ground_Default.prefab)
- * Toy_Bear  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Toy_Bear.prefab)
- * lpbns_br_floor  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_floor.prefab)
- * lpbns_br_floor (1)  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_floor.prefab)
- * lpbns_br_floor (1)  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_floor.prefab)
- * lpbns_br_floor (1)  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_floor.prefab)
- * TV_Set  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/TV_Set.prefab)
- * Book_3 (2)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Book_3.prefab)
- * lpbns_br_floor (1)  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_floor.prefab)
- * Musical_Notes  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Musical_Notes.prefab)
- * lpbns_br_wall_window  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_wall_window.prefab)
- * lpbns_br_floor  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_floor.prefab)
- * lpbns_br_chair  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_chair.prefab)
- * lpbns_br_floor  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_floor.prefab)
- * Book_2 (1)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Book_2.prefab)
- * lpbns_br_floor  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_floor.prefab)
- * lpbns_br_floor (1)  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_floor.prefab)
- * Book_5  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Book_5.prefab)
- * lpbns_br_plants_03  (prefab instance of Assets/LowPolyBoy/FreeStylizedBedRoom/Prefabs/lpbns_br_plants_03.prefab)
- ... 61 more object(s) not listed; regenerate with the Editor exporter for the full tree

## SCENE Assets/Scenes/SampleScene.unity   (3 object(s))
- Main Camera  [AudioListener, Camera, Transform, MISSING SCRIPT (guid:a79441f3)]
- Directional Light  [Light, Transform, MISSING SCRIPT (guid:474bcb49)]
- Global Volume  [MISSING SCRIPT (guid:17251560), Transform]  refs: sharedProfile=set

## SCENE Assets/Scenes/_Sandbox/VoiceHarness.unity   (4 object(s))
- VoiceHarness  [VoiceHarnessHud, GeminiLiveVoiceSession, Transform]  refs: config=set
- Main Camera  [AudioListener, Camera, Transform]
- Directional Light  [MISSING SCRIPT (guid:474bcb49), Light, Transform]
- * Meshy_AI_Cozy_Casual_Ensemble_0810100721_texture  (prefab instance of Assets/Art/Character/Meshy_AI_Cozy_Casual_Ensemble_0810100721_texture.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Armchair.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Armchair.fbx   (1 object(s))
- * Armchair  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Armchair.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Bean_Bag_1.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Bean_Bag_1.fbx   (1 object(s))
- * Bean_Bag_1  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Bean_Bag_1.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Bean_Bag_2.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Bean_Bag_2.fbx   (1 object(s))
- * Bean_Bag_2  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Bean_Bag_2.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Bed.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Bed.fbx   (1 object(s))
- * Bed  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Bed.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Billiard.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Billiard.fbx   (1 object(s))
- * Billiard  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Billiard.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Bobby_Rigged.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/CHARACTERS/Bobby_Rigged.fbx   (1 object(s))
- * Bobby_Rigged  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/CHARACTERS/Bobby_Rigged.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Book_1.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Book_1.fbx   (1 object(s))
- * Book_1  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Book_1.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Book_2.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Book_2.fbx   (1 object(s))
- * Book_2  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Book_2.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Book_3.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Book_3.fbx   (1 object(s))
- * Book_3  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Book_3.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Book_4.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Book_4.fbx   (1 object(s))
- * Book_4  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Book_4.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Book_5.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Book_5.fbx   (1 object(s))
- * Book_5  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Book_5.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Bookshelf.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Bookshelf.fbx   (1 object(s))
- * Bookshelf  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Bookshelf.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Cactus_1.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Cactus_1.fbx   (1 object(s))
- * Cactus_1  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Cactus_1.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Cactus_2.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Cactus_2.fbx   (1 object(s))
- * Cactus_2  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Cactus_2.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Car_Decor_Blue.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Car_Decor_Blue.fbx   (1 object(s))
- * Car_Decor_Blue  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Car_Decor_Blue.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Car_Decor_Purple.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Car_Decor_Purple.fbx   (1 object(s))
- * Car_Decor_Purple  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Car_Decor_Purple.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Car_Decor_Red.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Car_Decor_Red.fbx   (1 object(s))
- * Car_Decor_Red  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Car_Decor_Red.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Ceiling_Only.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Ceiling_Only.fbx   (1 object(s))
- * Ceiling_Only  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Ceiling_Only.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Chair.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Chair.fbx   (1 object(s))
- * Chair  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Chair.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Chess_Chair.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Chess_Chair.fbx   (1 object(s))
- * Chess_Chair  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Chess_Chair.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Chess_Table.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Chess_Table.fbx   (1 object(s))
- * Chess_Table  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Chess_Table.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Chloe_Rigged.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/CHARACTERS/Chloe_Rigged.fbx   (1 object(s))
- * Chloe_Rigged  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/CHARACTERS/Chloe_Rigged.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Claw_Machine.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Claw_Machine.fbx   (1 object(s))
- * Claw_Machine  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Claw_Machine.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Closet.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Closet.fbx   (1 object(s))
- * Closet  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Closet.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Coke.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Coke.fbx   (1 object(s))
- * Coke  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Coke.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Dance_Floor.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Dance_Floor.fbx   (1 object(s))
- * Dance_Floor  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Dance_Floor.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Dart.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Dart.fbx   (1 object(s))
- * Dart  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Dart.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Dart_Blue.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Dart_Blue.fbx   (1 object(s))
- * Dart_Blue  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Dart_Blue.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Dart_Green.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Dart_Green.fbx   (1 object(s))
- * Dart_Green  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Dart_Green.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Dart_Red.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Dart_Red.fbx   (1 object(s))
- * Dart_Red  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Dart_Red.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Decor_Dance_Text.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Decor_Dance_Text.fbx   (1 object(s))
- * Decor_Dance_Text  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Decor_Dance_Text.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Decor_Disco_Text.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Decor_Disco_Text.fbx   (1 object(s))
- * Decor_Disco_Text  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Decor_Disco_Text.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Decor_Game_Controller.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Decor_Game_Controller.fbx   (1 object(s))
- * Decor_Game_Controller  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Decor_Game_Controller.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Decor_Game_Over.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Decor_Game_Over.fbx   (1 object(s))
- * Decor_Game_Over  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Decor_Game_Over.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Desk.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Desk.fbx   (1 object(s))
- * Desk  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Desk.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Disco_Speaker.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Disco_Speaker.fbx   (1 object(s))
- * Disco_Speaker  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Disco_Speaker.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Discoball.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Discoball.fbx   (1 object(s))
- * Discoball  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Discoball.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Drawer.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Drawer.fbx   (1 object(s))
- * Drawer  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Drawer.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Foosball_Table.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Foosball_Table.fbx   (1 object(s))
- * Foosball_Table  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Foosball_Table.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Game_Controller_Black.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Game_Controller_Black.fbx   (1 object(s))
- * Game_Controller_Black  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Game_Controller_Black.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Game_Controller_Blue.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Game_Controller_Blue.fbx   (1 object(s))
- * Game_Controller_Blue  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Game_Controller_Blue.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Game_Controller_Green.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Game_Controller_Green.fbx   (1 object(s))
- * Game_Controller_Green  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Game_Controller_Green.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Game_Controller_Orange.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Game_Controller_Orange.fbx   (1 object(s))
- * Game_Controller_Orange  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Game_Controller_Orange.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Globe.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Globe.fbx   (1 object(s))
- * Globe  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Globe.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Ground_Blue.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Ground_Blue.fbx   (1 object(s))
- * Ground_Blue  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Ground_Blue.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Ground_Brown.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Ground_Brown.fbx   (1 object(s))
- * Ground_Brown  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Ground_Brown.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Ground_Default.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Ground_Default.fbx   (1 object(s))
- * Ground_Default  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Ground_Default.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Ground_Disco_1.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Ground_Disco_1.fbx   (1 object(s))
- * Ground_Disco_1  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Ground_Disco_1.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Ground_Disco_2.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Ground_Disco_2.fbx   (1 object(s))
- * Ground_Disco_2  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Ground_Disco_2.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Ground_Red.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Ground_Red.fbx   (1 object(s))
- * Ground_Red  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Ground_Red.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Guitar.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Guitar.fbx   (1 object(s))
- * Guitar  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Guitar.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Headphone.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Headphone.fbx   (1 object(s))
- * Headphone  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Headphone.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Jackson_Rigged.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/CHARACTERS/Jackson_Rigged.fbx   (1 object(s))
- * Jackson_Rigged  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/CHARACTERS/Jackson_Rigged.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Lamp.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Lamp.fbx   (1 object(s))
- * Lamp  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Lamp.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Lamp_Big.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Lamp_Big.fbx   (1 object(s))
- * Lamp_Big  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Lamp_Big.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Lisa_Rigged.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/CHARACTERS/Lisa_Rigged.fbx   (1 object(s))
- * Lisa_Rigged  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/CHARACTERS/Lisa_Rigged.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Lucas_Rigged.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/CHARACTERS/Lucas_Rigged.fbx   (1 object(s))
- * Lucas_Rigged  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/CHARACTERS/Lucas_Rigged.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Musical_Notes (1).prefab   (1 object(s))
- Musical_Notes (1)  [Transform, ParticleSystem, ParticleSystemRenderer]  refs: LightsModule=NULL, ShapeModule=NULL, SubModule=NULL, UVModule=set, moveWithCustomTransform=NULL

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Musical_Notes (2).prefab   (1 object(s))
- Musical_Notes (2)  [Transform, ParticleSystem, ParticleSystemRenderer]  refs: LightsModule=NULL, ShapeModule=NULL, SubModule=NULL, UVModule=set, moveWithCustomTransform=NULL

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Musical_Notes.prefab   (1 object(s))
- Musical_Notes  [Transform, ParticleSystem, ParticleSystemRenderer]  refs: LightsModule=NULL, ShapeModule=NULL, SubModule=NULL, UVModule=set, moveWithCustomTransform=NULL

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Nora_Rigged.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/CHARACTERS/Nora_Rigged.fbx   (1 object(s))
- * Nora_Rigged  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/CHARACTERS/Nora_Rigged.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/PC_Set.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/PC_Set.fbx   (1 object(s))
- * PC_Set  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/PC_Set.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Patrick_Rigged.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/CHARACTERS/Patrick_Rigged.fbx   (1 object(s))
- * Patrick_Rigged  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/CHARACTERS/Patrick_Rigged.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Pizza.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Pizza.fbx   (1 object(s))
- * Pizza  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Pizza.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Plant_1.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Plant_1.fbx   (1 object(s))
- * Plant_1  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Plant_1.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Plant_2.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Plant_2.fbx   (1 object(s))
- * Plant_2  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Plant_2.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Poster_Blue_Quotation.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Poster_Blue_Quotation.fbx   (1 object(s))
- * Poster_Blue_Quotation  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Poster_Blue_Quotation.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Poster_Game.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Poster_Game.fbx   (1 object(s))
- * Poster_Game  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Poster_Game.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Poster_Green_Quotation.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Poster_Green_Quotation.fbx   (1 object(s))
- * Poster_Green_Quotation  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Poster_Green_Quotation.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Poster_Loading.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Poster_Loading.fbx   (1 object(s))
- * Poster_Loading  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Poster_Loading.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Poster_Red_Quotation.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Poster_Red_Quotation.fbx   (1 object(s))
- * Poster_Red_Quotation  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Poster_Red_Quotation.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Room_Fence.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Room_Fence.fbx   (1 object(s))
- * Room_Fence  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Room_Fence.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Rubiks_Cube.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Rubiks_Cube.fbx   (1 object(s))
- * Rubiks_Cube  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Rubiks_Cube.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Stacey_Rigged.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/CHARACTERS/Stacey_Rigged.fbx   (1 object(s))
- * Stacey_Rigged  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/CHARACTERS/Stacey_Rigged.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Stairs.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Stairs.fbx   (1 object(s))
- * Stairs  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Stairs.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/TV_Set.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/TV_Set.fbx   (1 object(s))
- * TV_Set  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/TV_Set.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Table.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Table.fbx   (1 object(s))
- * Table  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Table.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Toy_Bear.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Toy_Bear.fbx   (1 object(s))
- * Toy_Bear  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Toy_Bear.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Toy_Monkey.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Toy_Monkey.fbx   (1 object(s))
- * Toy_Monkey  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Toy_Monkey.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Toy_Penguin.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Toy_Penguin.fbx   (1 object(s))
- * Toy_Penguin  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Toy_Penguin.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Toy_Rabbit.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Toy_Rabbit.fbx   (1 object(s))
- * Toy_Rabbit  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Toy_Rabbit.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_Corner.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Wall_Corner.fbx   (1 object(s))
- * Wall_Corner  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Wall_Corner.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_Corner_Ceiling.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Wall_Corner_Ceiling.fbx   (1 object(s))
- * Wall_Corner_Ceiling  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Wall_Corner_Ceiling.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_Default.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Wall_Default.fbx   (1 object(s))
- * Wall_Default  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Wall_Default.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_Default_Ceiling.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Wall_Default_Ceiling.fbx   (1 object(s))
- * Wall_Default_Ceiling  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Wall_Default_Ceiling.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_Discoroom.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Wall_Discoroom.fbx   (1 object(s))
- * Wall_Discoroom  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Wall_Discoroom.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_Only.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Wall_Only.fbx   (1 object(s))
- * Wall_Only  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Wall_Only.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_With_Door.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Wall_With_Door.fbx   (1 object(s))
- * Wall_With_Door  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Wall_With_Door.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_With_Door_Ceiling.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Wall_With_Door_Ceiling.fbx   (1 object(s))
- * Wall_With_Door_Ceiling  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Wall_With_Door_Ceiling.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_With_Window.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Wall_With_Window.fbx   (1 object(s))
- * Wall_With_Window  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Wall_With_Window.fbx)

## PREFAB Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_With_Window_Ceiling.prefab   variant-of: Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Wall_With_Window_Ceiling.fbx   (1 object(s))
- * Wall_With_Window_Ceiling  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PROPS/Wall_With_Window_Ceiling.fbx)

## SCENE Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/SCENES/All_Prefabs_Scene.unity   (99 object(s))
- Plane (1)  [MeshCollider, MeshRenderer, MeshFilter, Transform]
- Furnitures  [Transform]
- Characters  [Transform]
- Level_Base_Props  [Transform]
- Decorations  [Transform]
- Game_Props  [Transform]
- Main Camera  [AudioListener, Camera, Transform, MISSING SCRIPT (guid:a79441f3)]
- Directional Light  [Light, Transform, MISSING SCRIPT (guid:474bcb49)]
- Plane  [MeshCollider, MeshRenderer, MeshFilter, Transform]
- Disco_Room  [Transform]
- * Book_2  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Book_2.prefab)
- * Ceiling_Only  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Ceiling_Only.prefab)
- * Car_Decor_Red  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Car_Decor_Red.prefab)
- * Book_3  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Book_3.prefab)
- * Wall_Corner_Ceiling  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_Corner_Ceiling.prefab)
- * Book_5  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Book_5.prefab)
- * Car_Decor_Purple  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Car_Decor_Purple.prefab)
- * Cactus_1  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Cactus_1.prefab)
- * Book_1  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Book_1.prefab)
- * Stairs  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Stairs.prefab)
- * Ground_Red  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Ground_Red.prefab)
- * Foosball_Table  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Foosball_Table.prefab)
- * Disco_Speaker (1)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Disco_Speaker.prefab)
- * Coke  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Coke.prefab)
- * Book_4  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Book_4.prefab)
- * Lamp_Big  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Lamp_Big.prefab)
- * Car_Decor_Blue  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Car_Decor_Blue.prefab)
- * Ground_Blue  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Ground_Blue.prefab)
- * Wall_Corner  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_Corner.prefab)
- * Wall_Only  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_Only.prefab)
- * Ground_Brown  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Ground_Brown.prefab)
- * Desk  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Desk.prefab)
- * Cactus_2  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Cactus_2.prefab)
- * Game_Controller_Blue  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Game_Controller_Blue.prefab)
- * Ground_Default  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Ground_Default.prefab)
- * Lisa_Rigged  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Lisa_Rigged.prefab)
- * Globe  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Globe.prefab)
- * Game_Controller_Green  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Game_Controller_Green.prefab)
- * Chess_Chair  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Chess_Chair.prefab)
- * Claw_Machine  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Claw_Machine.prefab)
- * Headphone  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Headphone.prefab)
- * Poster_Game  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Poster_Game.prefab)
- * Toy_Penguin  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Toy_Penguin.prefab)
- * Game_Controller_Black  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Game_Controller_Black.prefab)
- * Bobby_Rigged  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Bobby_Rigged.prefab)
- * Guitar  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Guitar.prefab)
- * TV_Set  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/TV_Set.prefab)
- * Dance_Floor  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Dance_Floor.prefab)
- * Decor_Dance_Text  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Decor_Dance_Text.prefab)
- * PC_Set  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/PC_Set.prefab)
- * Toy_Bear  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Toy_Bear.prefab)
- * Dart  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Dart.prefab)
- * Toy_Rabbit  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Toy_Rabbit.prefab)
- * Decor_Game_Controller  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Decor_Game_Controller.prefab)
- * Wall_Default  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_Default.prefab)
- * Armchair  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Armchair.prefab)
- * Billiard  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Billiard.prefab)
- * Poster_Green_Quotation  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Poster_Green_Quotation.prefab)
- * Toy_Monkey  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Toy_Monkey.prefab)
- * Ground_Disco_2  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Ground_Disco_2.prefab)
- * Dart_Green  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Dart_Green.prefab)
- * Decor_Game_Over  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Decor_Game_Over.prefab)
- * Bed  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Bed.prefab)
- * Ground_Disco_1  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Ground_Disco_1.prefab)
- * Dart_Red  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Dart_Red.prefab)
- * Plant_1  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Plant_1.prefab)
- * Game_Controller_Orange  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Game_Controller_Orange.prefab)
- * Lamp  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Lamp.prefab)
- * Wall_Default_Ceiling  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_Default_Ceiling.prefab)
- * Chloe_Rigged  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Chloe_Rigged.prefab)
- * Patrick_Rigged  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Patrick_Rigged.prefab)
- * Wall_With_Door_Ceiling  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_With_Door_Ceiling.prefab)
- * Rubiks_Cube  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Rubiks_Cube.prefab)
- * Closet  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Closet.prefab)
- * Bean_Bag_2  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Bean_Bag_2.prefab)
- * Drawer  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Drawer.prefab)
- * Nora_Rigged  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Nora_Rigged.prefab)
- * Wall_Discoroom  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_Discoroom.prefab)
- * Poster_Blue_Quotation  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Poster_Blue_Quotation.prefab)
- * Room_Fence  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Room_Fence.prefab)
- * Poster_Red_Quotation  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Poster_Red_Quotation.prefab)
- * Table  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Table.prefab)
- * Pizza  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Pizza.prefab)
- * Chair  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Chair.prefab)
- * Wall_With_Window_Ceiling  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_With_Window_Ceiling.prefab)
- * Bookshelf  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Bookshelf.prefab)
- * Poster_Loading  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Poster_Loading.prefab)
- * Chess_Table  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Chess_Table.prefab)
- * Plant_2  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Plant_2.prefab)
- * Decor_Disco_Text  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Decor_Disco_Text.prefab)
- * Stacey_Rigged  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Stacey_Rigged.prefab)
- * Discoball  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Discoball.prefab)
- * Wall_With_Window  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_With_Window.prefab)
- * Jackson_Rigged  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Jackson_Rigged.prefab)
- * Dart_Blue  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Dart_Blue.prefab)
- * Disco_Speaker  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Disco_Speaker.prefab)
- * Bean_Bag_1  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Bean_Bag_1.prefab)
- * Lucas_Rigged  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Lucas_Rigged.prefab)
- * Wall_With_Door  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_With_Door.prefab)

## SCENE Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/SCENES/Demo_Scene.unity   (306 object(s))
- Renders  [Transform]
  - Camera_Detail_View_1 [inactive]  [MISSING SCRIPT (guid:a79441f3), AudioListener, Camera, Transform]
  - Camera_Detail_View_2 [inactive]  [MISSING SCRIPT (guid:a79441f3), AudioListener, Camera, Transform]
  - Camera_Detail_View_3 [inactive]  [MISSING SCRIPT (guid:a79441f3), AudioListener, Camera, Transform]
  - Camera_Detail_View_4 [inactive]  [MISSING SCRIPT (guid:a79441f3), AudioListener, Camera, Transform]
  - Camera_Detail_View_5 [inactive]  [MISSING SCRIPT (guid:a79441f3), AudioListener, Camera, Transform]
  - Camera_Detail_View_6 [inactive]  [MISSING SCRIPT (guid:a79441f3), AudioListener, Camera, Transform]
  - Camera_Detail_View_7 [inactive]  [MISSING SCRIPT (guid:a79441f3), AudioListener, Camera, Transform]
  - Camera_Detail_View_8 [inactive]  [Transform, MISSING SCRIPT (guid:a79441f3), AudioListener, Camera]
  - Camera_Detail_View_9 [inactive]  [MISSING SCRIPT (guid:a79441f3), AudioListener, Camera, Transform]
  - Camera_Detail_View_10 [inactive]  [MISSING SCRIPT (guid:a79441f3), AudioListener, Camera, Transform]
  - Camera_Plan_View_1 [inactive]  [Transform, MISSING SCRIPT (guid:a79441f3), AudioListener, Camera]
  - Camera_Plan_View_2 [inactive]  [Transform, MISSING SCRIPT (guid:a79441f3), AudioListener, Camera]
  - Camera_Plan_VIew_3 [inactive]  [Transform, MISSING SCRIPT (guid:a79441f3), AudioListener, Camera]
  - Camera_Plan_View_4 [inactive]  [Transform, MISSING SCRIPT (guid:a79441f3), AudioListener, Camera]
  - Camera_Cover_View  [Transform, MISSING SCRIPT (guid:a79441f3), AudioListener, Camera]
- Bedroom  [Transform]
- Living_Room  [Transform]
  - Dart  [Transform]
- Directional Light  [Light, Transform, MISSING SCRIPT (guid:474bcb49)]
- Particle_System  [Transform]
- Disco_Room  [Transform]
  - Spot Light  [Transform, MISSING SCRIPT (guid:474bcb49), Light]
  - Spot Light (2)  [Transform, MISSING SCRIPT (guid:474bcb49), Light]
  - Spot Light (1)  [Transform, MISSING SCRIPT (guid:474bcb49), Light]
  - Spot Light (3)  [Transform, MISSING SCRIPT (guid:474bcb49), Light]
- Characters  [Transform]
- Level_Base  [Transform]
- * Ground_Default (10)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Ground_Default.prefab)
- * Bed  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Bed.prefab)
- * Wall_With_Window (7)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_With_Window.prefab)
- * Wall_Default (20)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_Default.prefab)
- * Ground_Default (7)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Ground_Default.prefab)
- * Dart_Red  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Dart_Red.prefab)
- * Plant_1 (2)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Plant_1.prefab)
- * Wall_Default (1)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_Default.prefab)
- * Chess_Chair (1)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Chess_Chair.prefab)
- * Ground_Disco_1 (9)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Ground_Disco_1.prefab)
- * Wall_Only (12)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_Only.prefab)
- * Wall_Discoroom (6)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_Discoroom.prefab)
- * Room_Fence  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Room_Fence.prefab)
- * Ground_Blue (27)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Ground_Blue.prefab)
- * Ground_Blue (12)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Ground_Blue.prefab)
- * Ground_Default (11)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Ground_Default.prefab)
- * Wall_Only (18)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_Only.prefab)
- * Cactus_2 (1)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Cactus_2.prefab)
- * Wall_Default (25)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_Default.prefab)
- * Wall_Only (3)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_Only.prefab)
- * Wall_Default (2)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_Default.prefab)
- * Room_Fence (3)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Room_Fence.prefab)
- * Wall_Only (27)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_Only.prefab)
- * Poster_Game  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Poster_Game.prefab)
- * Toy_Bear (1)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Toy_Bear.prefab)
- * Wall_Corner (5)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_Corner.prefab)
- * Car_Decor_Blue  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Car_Decor_Blue.prefab)
- * Ground_Blue (4)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Ground_Blue.prefab)
- * Ground_Blue (30)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Ground_Blue.prefab)
- * Wall_Discoroom (15)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_Discoroom.prefab)
- * Desk  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Desk.prefab)
- * Wall_Default (19)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_Default.prefab)
- * Patrick_Rigged  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Patrick_Rigged.prefab)
- * Bean_Bag_2 (2)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Bean_Bag_2.prefab)
- * Chair  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Chair.prefab)
- * Wall_Default  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_Default.prefab)
- * Ground_Default (22)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Ground_Default.prefab)
- * Toy_Bear (2)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Toy_Bear.prefab)
- * Ground_Blue (25)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Ground_Blue.prefab)
- * Wall_Discoroom (12)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_Discoroom.prefab)
- * Lisa_Rigged  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Lisa_Rigged.prefab)
- * Rubiks_Cube  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Rubiks_Cube.prefab)
- * Poster_Loading  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Poster_Loading.prefab)
- * Poster_Red_Quotation  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Poster_Red_Quotation.prefab)
- * Toy_Penguin (3)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Toy_Penguin.prefab)
- * Ground_Blue (10)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Ground_Blue.prefab)
- * Ground_Default (24)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Ground_Default.prefab)
- * Wall_Only (7)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_Only.prefab)
- * Book_3  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Book_3.prefab)
- * Wall_With_Window (4)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_With_Window.prefab)
- * Nora_Rigged  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Nora_Rigged.prefab)
- * Toy_Rabbit  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Toy_Rabbit.prefab)
- * Disco_Speaker (1)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Disco_Speaker.prefab)
- * Wall_Only (26)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_Only.prefab)
- * Lamp_Big  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Lamp_Big.prefab)
- * Ground_Blue (23)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Ground_Blue.prefab)
- * Wall_With_Window (9)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_With_Window.prefab)
- * Wall_Default (24)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_Default.prefab)
- * Lamp  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Lamp.prefab)
- * Toy_Monkey (1)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Toy_Monkey.prefab)
- * Book_5  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Book_5.prefab)
- * Wall_Only (15)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_Only.prefab)
- * Ground_Blue (36)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Ground_Blue.prefab)
- * Ground_Default (12)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Ground_Default.prefab)
- * Decor_Disco_Text  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Decor_Disco_Text.prefab)
- * Toy_Penguin (2)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Toy_Penguin.prefab)
- * Decor_Game_Over  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Decor_Game_Over.prefab)
- * Wall_Corner  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_Corner.prefab)
- * Ground_Blue (29)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Ground_Blue.prefab)
- * Ground_Disco_2 (4)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Ground_Disco_2.prefab)
- * Ground_Blue (32)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Ground_Blue.prefab)
- * Wall_Discoroom (11)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_Discoroom.prefab)
- * Ground_Blue (28)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Ground_Blue.prefab)
- * Ground_Default (23)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Ground_Default.prefab)
- * Toy_Monkey  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Toy_Monkey.prefab)
- * Wall_Only (28)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_Only.prefab)
- * Wall_With_Window (8)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_With_Window.prefab)
- * Wall_Default (18)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_Default.prefab)
- * Wall_Only (4)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_Only.prefab)
- * Ground_Blue (5)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Ground_Blue.prefab)
- * Toy_Rabbit (2)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Toy_Rabbit.prefab)
- * Claw_Machine  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Claw_Machine.prefab)
- * Ground_Default (25)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Ground_Default.prefab)
- * Toy_Penguin (1)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Toy_Penguin.prefab)
- * Ground_Blue (11)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Ground_Blue.prefab)
- * Wall_Discoroom (10)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_Discoroom.prefab)
- * Wall_With_Window (1)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_With_Window.prefab)
- * Table  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Table.prefab)
- * Cactus_1 (2)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Cactus_1.prefab)
- * Wall_With_Door  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Wall_With_Door.prefab)
- * Ground_Disco_2 (1)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Ground_Disco_2.prefab)
- * Ground_Blue (20)  (prefab instance of Assets/ZNS3D/FREE_STYLIZED_GAMEROOM_PACK/PREFABS/Ground_Blue.prefab)
- ... 186 more object(s) not listed; regenerate with the Editor exporter for the full tree

## Script index — component name -> codemap path

- AudioService | Assets/FruitMerge/Scripts/Services/AudioService.cs
- BoostButton | Assets/FruitMerge/Scripts/UI/BoostButton.cs
- ComboPopupDirector | Assets/FruitMerge/Scripts/Services/ComboPopupDirector.cs
- ComboPopupItem | Assets/FruitMerge/Scripts/Gameplay/ComboPopupItem.cs
- ConfettiDirector | Assets/FruitMerge/Scripts/Services/ConfettiDirector.cs
- DanceModeController | Assets/Scripts/Presentation/DanceModeController.cs
- DropController | Assets/FruitMerge/Scripts/Gameplay/DropController.cs
- DropIndicatorController | Assets/FruitMerge/Scripts/Gameplay/DropIndicatorController.cs
- EffectDirector | Assets/FruitMerge/Scripts/Services/EffectDirector.cs
- Fruit | Assets/FruitMerge/Scripts/Gameplay/Fruit.cs
- FruitChainView | Assets/FruitMerge/Scripts/UI/FruitChainView.cs
- FruitColorTitle | Assets/FruitMerge/Scripts/UI/FruitColorTitle.cs
- FruitFace | Assets/FruitMerge/Scripts/Gameplay/FruitFace.cs
- GameModeController | Assets/Scripts/Presentation/GameModeController.cs
- GameOverPanel | Assets/FruitMerge/Scripts/UI/GameOverPanel.cs
- GeminiLiveVoiceSession | Assets/Scripts/Voice/GeminiLiveVoiceSession.cs
- HUDView | Assets/FruitMerge/Scripts/UI/HUDView.cs
- MenuPanel | Assets/FruitMerge/Scripts/UI/MenuPanel.cs
- NextFruitDisplay | Assets/FruitMerge/Scripts/Gameplay/NextFruitDisplay.cs
- PausePanel | Assets/FruitMerge/Scripts/UI/PausePanel.cs
- PetRoamer | Assets/Scripts/Presentation/PetRoamer.cs
- PushToTalkButtonView | Assets/Scripts/UI/PushToTalkButtonView.cs
- RoomSessionController | Assets/Scripts/Bootstrap/RoomSessionController.cs
- SaveService | Assets/FruitMerge/Scripts/Services/SaveService.cs
- ScreenBackground | Assets/FruitMerge/Scripts/UI/ScreenBackground.cs
- SplashPanel | Assets/FruitMerge/Scripts/UI/SplashPanel.cs
- VoiceHarnessHud | Assets/Scripts/Voice/VoiceHarnessHud.cs
