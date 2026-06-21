# Cod: Evadare - Legacy Prototype Demo

Unity version: 2022.3.62f3 LTS

## Open The Project

Open this repository in Unity Hub using Unity 2022.3.62f3 LTS.

## Current Recommended Flow

Use `README_GAME_COMPLETE.md` and generate the current game with:

- `Tools/Cod Evadare/Build/Content Complete MVP`

The active game scenes are under:

- `Assets/_Project/Scenes/Game`
- `Assets/_Project/Scenes/Levels`

## Generate Legacy Demo Scenes

Use the Unity editor menu:

- `Tools/Cod Evadare/Legacy Prototypes/Create Main Menu Scene`
- `Tools/Cod Evadare/Legacy Prototypes/Create Prototype 1.0 Final Demo`
- `Tools/Cod Evadare/Legacy Prototypes/Create Prototype 2.0 Prison Level`
- `Tools/Cod Evadare/Legacy Prototypes/Create Prototype 2.1 Balanced Prison Level`
- `Tools/Cod Evadare/Legacy Prototypes/Update Main Menu With Level 2`

Main scene:

- `Assets/_Project/Scenes/MainMenu.unity`

Level 1 scene:

- `Assets/_Project/Scenes/Prototype_FinalDemo.unity`

Level 2 scene:

- `Assets/_Project/Scenes/Prototype_PrisonLevel.unity`
- `Assets/_Project/Scenes/Prototype_PrisonLevel_Balanced.unity`

## Play From Legacy Main Menu

Open `Assets/_Project/Scenes/MainMenu.unity`, press Play, then select `Play Demo` for Level 1, `Play Level 2: Prison` for the original Level 2, or `Play Level 2: Prison Balanced` for Prototype 2.1.

## Controls

- WASD / Arrow Keys: Move
- Mouse: Aim
- Left Click: Shoot
- R: Reload
- E: Interact / buy shop item
- Escape: Pause

## Implemented Systems

Level 1: Laboratory Escape Demo

- Player movement, aiming, shooting, ammo, reload
- Health, damage, invincibility frames, game over
- Enemy health, enemy chasing, contact damage
- Combat rooms, triggers, doors, enemy spawners
- Loot pickups, weapon pickups, resources
- Shop items and shop UI
- Miniboss room and buff choice UI
- Final boss fight, boss projectiles, boss health UI, victory flow
- Objective UI, pause menu, basic audio, sprite flash, pickup bob, camera shake

Level 2: Prison Escape

- Keycard pickup and keycard UI
- Locked gate opened with E near the gate
- Ranged prison guards using enemy projectiles and aim telegraphs
- Security laser hazards with inactive, warning, and active states
- Armory shop
- Riot Brute miniboss and buff choice
- The Warden boss fight with boss health UI and `PRISON ESCAPED` victory flow

## Known Limitations

- These scenes are kept only as legacy prototype references and are not part of the cleaned Build Settings.
- Art and audio are generated placeholders.
- Enemy and boss AI are prototype-simple.
- Balance is tuned for short prototype demos and still needs Play Mode testing.
- No save system or campaign progression exists yet.
