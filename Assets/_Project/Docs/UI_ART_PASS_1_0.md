# Cod: Evadare - UI Art Pass 1.0

Unity version: 2022.3.62f3 LTS

## Repository Audit
### Existing paths

- `Assets/_Project`
- `Assets/_Project/Scripts`
- `Assets/_Project/Scripts/UI`
- `Assets/_Project/Scripts/Player`
- `Assets/_Project/Scripts/Weapons`
- `Assets/_Project/Scripts/Rooms`
- `Assets/_Project/Scripts/Shop`
- `Assets/_Project/Scripts/Buffs`
- `Assets/_Project/Scripts/Menu`
- `Assets/_Project/Scripts/Editor`
- `Assets/_Project/Scenes`
- `Assets/_Project/Scenes/Game`
- `Assets/_Project/Scenes/Levels`
- `Assets/_Project/Prefabs`
- `Assets/_Project/ScriptableObjects`
- `Assets/_Project/Art/Generated`

### Missing paths

- None

### Existing scripts

- `HealthUI2D`
- `ResourceUI2D`
- `WeaponUI2D`
- `KeycardUI2D`
- `BuffStatusUI2D`
- `ObjectiveUI2D`
- `BossHealthUI2D`
- `ShopUI2D`
- `MainMenuController2D`
- `LevelSelectController2D`
- `PauseMenuController2D`
- `GameOverController2D`
- `LevelEndController2D`
- `PlayerShooting2D`
- `PlayerHealth2D`
- `PlayerResources2D`
- `BuffChoiceController2D`

### Missing scripts

- None

### Existing scenes

- `Assets/_Project/Scenes/Game/MainMenu.unity`
- `Assets/_Project/Scenes/Game/LevelSelect.unity`
- `Assets/_Project/Scenes/Levels/Level_01_Laboratory.unity`
- `Assets/_Project/Scenes/Levels/Level_02_Prison.unity`
- `Assets/_Project/Scenes/Levels/Level_03_ZombieCity.unity`
- `Assets/_Project/Scenes/Levels/Level_04_SciFiBase.unity`
- `Assets/_Project/Scenes/Levels/Level_05_HorrorHospital.unity`
- `Assets/_Project/Scenes/Prototype_FinalDemo.unity`
- `Assets/_Project/Scenes/Prototype_PrisonLevel_Balanced.unity`

### Missing scenes

- None

## Style Direction

Dark tactical sci-fi UI with teal/cyan primary accents, orange warnings, red danger states, green success states, framed panels, generated icons, rarity cards, and level-themed color accents.

## Generated Assets

- Sprites: `Assets/_Project/Art/Generated/UI`
- Icons: `Assets/_Project/Art/Generated/UI/Icons`
- Theme assets: `Assets/_Project/ScriptableObjects/UI`
- Showcase scene: `Assets/_Project/Scenes/Game/UI_Showcase.unity`

### Scenes upgraded

- `Assets/_Project/Scenes/Game/MainMenu.unity`
- `Assets/_Project/Scenes/Game/LevelSelect.unity`
- `Assets/_Project/Scenes/Levels/Level_01_Laboratory.unity`
- `Assets/_Project/Scenes/Levels/Level_02_Prison.unity`
- `Assets/_Project/Scenes/Levels/Level_03_ZombieCity.unity`
- `Assets/_Project/Scenes/Levels/Level_04_SciFiBase.unity`
- `Assets/_Project/Scenes/Levels/Level_05_HorrorHospital.unity`
- `Assets/_Project/Scenes/Prototype_FinalDemo.unity`
- `Assets/_Project/Scenes/Prototype_PrisonLevel_Balanced.unity`

### Scenes skipped

- None

## Run

Use `Tools/Cod Evadare/UI/Build Complete UI Art Pass`.

## Inspect

Open `Assets/_Project/Scenes/Game/UI_Showcase.unity` and inspect the generated UI kit.

## Manual Test Checklist

- Main menu buttons still load the correct campaign scenes.
- Level select still respects locked/unlocked states.
- HUD text remains readable in all five gameplay scenes.
- Shop, buff choice, boss health, pause, game over, and victory panels still function.
- Crosshair is only enabled where intentionally assigned.

## Known Limitations

This pass uses generated placeholder UI sprites and broad scene styling by object names. Final bespoke layout polish still requires Play Mode review.
