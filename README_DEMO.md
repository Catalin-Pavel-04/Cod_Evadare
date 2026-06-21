# Cod: Evadare - Prototype Demo

Unity version: 2022.3.62f3 LTS

## Open The Project

Open this repository in Unity Hub using Unity 2022.3.62f3 LTS.

## Generate Demo Scenes

Use the Unity editor menu:

- `Tools/Cod Evadare/Create Main Menu Scene`
- `Tools/Cod Evadare/Create Prototype 1.0 Final Demo`
- `Tools/Cod Evadare/Create Prototype 2.0 Prison Level`
- `Tools/Cod Evadare/Update Main Menu With Level 2`

Main scene:

- `Assets/_Project/Scenes/MainMenu.unity`

Level 1 scene:

- `Assets/_Project/Scenes/Prototype_FinalDemo.unity`

Level 2 scene:

- `Assets/_Project/Scenes/Prototype_PrisonLevel.unity`

## Play From Main Menu

Open `Assets/_Project/Scenes/MainMenu.unity`, press Play, then select `Play Demo` for Level 1 or `Play Level 2: Prison` for Level 2.

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
- Ranged prison guards using enemy projectiles
- Security laser hazards that toggle on and off
- Armory shop
- Riot Brute miniboss and buff choice
- The Warden boss fight with boss health UI and `PRISON ESCAPED` victory flow

## Known Limitations

- Art and audio are generated placeholders.
- Enemy and boss AI are prototype-simple.
- Balance is tuned for short prototype demos and still needs Play Mode testing.
- No save system or campaign progression exists yet.
