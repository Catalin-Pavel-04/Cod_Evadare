# Cod: Evadare - Visual Repair Phase A

Date: 2026-06-22

## Scope

Phase A is a conservative visual repair pass. It does not change gameplay logic, prefab roots, player stats, weapon stats, enemy stats, pickup logic, shop logic, buff logic or level progression.

The repair is implemented as Unity Editor menu tools. The tools only touch visual presentation data when explicitly run from the Unity menu.

## Added Unity Menu Items

- `Tools/Cod Evadare/Art/Phase A/Report Campaign Visual Footprints`
- `Tools/Cod Evadare/Art/Phase A/Repair Campaign Visual Footprints`

## What The Report Tool Checks

The report tool opens each campaign scene under:

- `Assets/_Project/Scenes/Levels`

It scans Wall, Floor, Door, Cover and Prop objects and logs:

- missing `Visual` children
- tiny or mismatched `Visual` SpriteRenderers
- disabled root SpriteRenderers with missing or broken replacement visuals
- sprite references under `Assets/_Project/Art/Sprites` that are not from `RuntimeTransparent`
- suspicious `Visual` transform scale values

The report tool does not save scene changes.

## What The Repair Tool Does

The repair tool:

- applies existing visual import settings for art sprites
- opens each campaign scene under `Assets/_Project/Scenes/Levels`
- finds Wall, Floor, Door, Cover and Prop objects
- derives the intended footprint from `BoxCollider2D`, fallback `Collider2D.bounds`, fallback root `SpriteRenderer`, fallback current visual renderer
- creates a missing `Visual` child only when needed
- sizes only the visual child SpriteRenderer
- preserves gameplay root object positions
- preserves colliders, Rigidbody2D components, triggers, tags and layers
- prefers `Assets/_Project/Art/Sprites/RuntimeTransparent` sprites
- keeps root SpriteRenderers disabled only when a usable replacement visual exists
- re-enables a root SpriteRenderer as a fallback if the replacement visual is missing or broken

Walls and floors are set to tiled SpriteRenderer draw mode. Doors, covers and props are set to stretched SpriteRenderer draw mode so their visible sprite footprint matches their gameplay collider/root footprint.

## Existing Tool Hardening

`CodEvadarePickupPropHazardEnvironmentVisuals` was hardened so future runs of the existing environment art pass also size Door, LockedGate, Cover and Prop visuals from their gameplay footprint. Root SpriteRenderers are no longer disabled by that pass unless the replacement visual is usable.

## Scenes Targeted

The Phase A tools target the campaign scenes only:

- `Assets/_Project/Scenes/Levels/Level_01_Laboratory.unity`
- `Assets/_Project/Scenes/Levels/Level_02_Prison.unity`
- `Assets/_Project/Scenes/Levels/Level_03_ZombieCity.unity`
- `Assets/_Project/Scenes/Levels/Level_04_SciFiBase.unity`
- `Assets/_Project/Scenes/Levels/Level_05_HorrorHospital.unity`

Prototype scenes, menu scenes and `Assets/Scenes/SampleScene.unity` are not repaired by Phase A.

## How To Run Phase A In Unity

1. Open the project in Unity.
2. Let scripts compile.
3. Run `Tools/Cod Evadare/Art/Phase A/Report Campaign Visual Footprints`.
4. Read the Console summary. Expect it to report existing broken/tiny visuals before repair.
5. Run `Tools/Cod Evadare/Art/Phase A/Repair Campaign Visual Footprints`.
6. Run `Tools/Cod Evadare/Art/Phase A/Report Campaign Visual Footprints` again.
7. Open `Assets/_Project/Scenes/Levels/Level_01_Laboratory.unity`.
8. Inspect the scene in the Scene and Game views before approving further visual work.

## What To Inspect In Level_01_Laboratory

Check these items before moving to UI polish:

- Walls are visible around every room and corridor.
- Wall visuals match the collider footprint and are not tiny 1x1 patches.
- Floors cover the intended rooms without giant stretched artifacts.
- Doors and locked gates line up with wall openings.
- Cover blocks are readable and not oversized.
- Shop kiosk and terminal props remain visually present but do not cover the player/HUD.
- Player movement is not blocked differently than before.
- Door triggers, room triggers, pickups, hazards and enemies still behave as before.
- No opaque square backgrounds appear on gameplay sprites.
- Console does not show missing sprite, missing component or broken reference errors from the repair pass.

## Known Limitations

Phase A repairs footprint and import safety only. It does not solve:

- menu layout problems
- HUD layout problems
- level-specific art theme variety
- final room composition
- lighting and post-processing
- enemy/player silhouette polish

Those belong to later phases after the repaired gameplay view is approved.
