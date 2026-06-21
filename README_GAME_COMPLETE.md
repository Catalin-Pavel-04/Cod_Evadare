# Cod: Evadare - Content Complete MVP

Unity version: 2022.3.62f3 LTS

## Generate the MVP

Open the Unity project and run:

`Tools/Cod Evadare/Build/Content Complete MVP`

Main scene:

`Assets/_Project/Scenes/Game/MainMenu.unity`

Level scenes:

- `Assets/_Project/Scenes/Levels/Level_01_Laboratory.unity`
- `Assets/_Project/Scenes/Levels/Level_02_Prison.unity`
- `Assets/_Project/Scenes/Levels/Level_03_ZombieCity.unity`
- `Assets/_Project/Scenes/Levels/Level_04_SciFiBase.unity`
- `Assets/_Project/Scenes/Levels/Level_05_HorrorHospital.unity`

## Controls

- WASD / Arrow Keys: Move
- Mouse: Aim
- Left Click: Shoot
- R: Reload
- E: Interact
- Escape: Pause

## Implemented Systems

Movement, aiming, shooting, ammo, reload, health, game over, doors, rooms, enemy spawners, loot, shops, buffs, bosses, boss UI, hazards, locked gates, level select, campaign unlocks, pause menu, and victory flow.

## Weapons

Pistol, SMG, Shotgun, AssaultRifle, Revolver, PlasmaRifle, ArcaneStaff.

## Levels

1. Laboratory Escape
2. Prison Escape
3. Zombie City
4. Sci-Fi Base
5. Horror Hospital

## Known Limitations

All art and audio are generated placeholders. Layouts are functional MVP combat rooms, not final designed levels. Balance requires Play Mode tuning.

## Manual Test Checklist

- New Game starts Level 1.
- Level Select locks/unlocks levels.
- Each level can be completed from start to victory.
- Shops, pickups, buffs, bosses, hazards, pause, game over, reload, and next level flow work.

## UI Art Pass 1.0

Run `Tools/Cod Evadare/UI/Build Complete UI Art Pass` to regenerate the generated UI sprites, theme assets, showcase scene, and safe scene styling pass.

Showcase scene: `Assets/_Project/Scenes/Game/UI_Showcase.unity`
