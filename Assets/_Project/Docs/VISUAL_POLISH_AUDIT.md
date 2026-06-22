# Cod: Evadare - Visual Polish Audit

Date: 2026-06-22

## Scope

This audit inspects the current Unity project structure, scenes, visual asset usage, import settings and scene YAML references. It does not change gameplay logic, prefab roots, scene content, weapon stats, enemy stats or player stats.

The audit is based on filesystem inspection, Unity YAML references, sprite `.meta` files, and the recent in-editor screenshots. Unity Play Mode was not run for this audit.

## 1. Existing Scenes

Build Settings currently include:

- `Assets/_Project/Scenes/Game/MainMenu.unity`
- `Assets/_Project/Scenes/Game/LevelSelect.unity`
- `Assets/_Project/Scenes/Levels/Level_01_Laboratory.unity`
- `Assets/_Project/Scenes/Levels/Level_02_Prison.unity`
- `Assets/_Project/Scenes/Levels/Level_03_ZombieCity.unity`
- `Assets/_Project/Scenes/Levels/Level_04_SciFiBase.unity`
- `Assets/_Project/Scenes/Levels/Level_05_HorrorHospital.unity`
- `Assets/Scenes/SampleScene.unity`

Additional project scenes exist:

- `Assets/_Project/Scenes/Game/UI_Showcase.unity`
- `Assets/_Project/Scenes/MainMenu.unity`
- `Assets/_Project/Scenes/Prototype_BossFight.unity`
- `Assets/_Project/Scenes/Prototype_FinalDemo.unity`
- `Assets/_Project/Scenes/Prototype_FullLaboratoryLevel.unity`
- `Assets/_Project/Scenes/Prototype_HealthCombat.unity`
- `Assets/_Project/Scenes/Prototype_Lab.unity`
- `Assets/_Project/Scenes/Prototype_LootResources.unity`
- `Assets/_Project/Scenes/Prototype_MinibossBuffs.unity`
- `Assets/_Project/Scenes/Prototype_PrisonLevel.unity`
- `Assets/_Project/Scenes/Prototype_PrisonLevel_Balanced.unity`
- `Assets/_Project/Scenes/Prototype_RoomLoop.unity`
- `Assets/_Project/Scenes/Prototype_Shop.unity`
- `Assets/_Project/Scenes/Prototype_WeaponLoot.unity`

## 2. Main Playable Scene

The main boot scene is:

- `Assets/_Project/Scenes/Game/MainMenu.unity`

The first campaign gameplay scene is:

- `Assets/_Project/Scenes/Levels/Level_01_Laboratory.unity`

`MainMenuController2D` starts a new game by loading `Level_01_Laboratory`. `LevelSelectController2D` maps the campaign to five level scenes:

1. `Level_01_Laboratory`
2. `Level_02_Prison`
3. `Level_03_ZombieCity`
4. `Level_04_SciFiBase`
5. `Level_05_HorrorHospital`

The prototype scenes are legacy/test scenes and should not be treated as the final visual target.

## 3. Visual Assets Actually Used

The art pack contains 299 image assets under `Assets/_Project/Art/Sprites` and `Assets/_Project/Art/Thumbnails`.

Current campaign scenes and prefabs reference mostly the generated `RuntimeTransparent` copies, not the original art pack paths. This is good for transparency, but it also means the original folders are largely unused directly.

Used in campaign scenes directly:

- Player: `RuntimeTransparent/Player/Player_Body_Base.png`, `Player_SelectionRing.png`, `Player_Shadow.png`
- Weapons: `RuntimeTransparent/Weapons/Weapon_Pistol.png`
- Environment: `RuntimeTransparent/Tiles/Tile_WallHorizontal.png`, `Tile_WallVertical.png`, `Tile_SciFiFloor.png`
- Props: `RuntimeTransparent/Props/Prop_AutomaticDoor_Vertical.png`, `Prop_LowCoverBlock.png`, `Prop_ShopKiosk.png`
- UI: `RuntimeTransparent/UI/Buttons/UI_Button_Normal.png`, `UI_Button_Hover.png`, `UI_Button_Pressed.png`
- HUD: `RuntimeTransparent/UI/Panels/UI_HealthBar_Frame.png`, `UI_HealthBar_Fill.png`, `UI_BossHealthBar_Frame.png`, `UI_BossHealthBar_Fill.png`, `UI_BuffChoiceCard.png`

Used mainly through prefabs:

- Bosses: Experiment-01, Warden, AI Core, Abomination, Nightmare Doctor
- Enemies: lab mutant, prison guard, ranged guard, security bot, zombie walker/runner, ghost, haunted medic, drone
- Pickups: ammo, medkit, money, keycard, buff vial, weapon crate
- Projectiles: bullet tracer, enemy laser
- Hazards: toxic puddle, electric floor, laser emitter/beam, necromancer VFX
- Gates: locked gate vertical and door frame

Important mismatch: all five campaign levels currently reference `Tile_SciFiFloor` and the same wall tile set. This makes the different level themes less distinct.

## 4. Existing But Unused Art Assets

The original source folders are currently unused directly in playable scenes/prefabs:

- `Assets/_Project/Art/Sprites/Bosses`
- `Assets/_Project/Art/Sprites/Buffs`
- `Assets/_Project/Art/Sprites/Enemies`
- `Assets/_Project/Art/Sprites/Hazards`
- `Assets/_Project/Art/Sprites/Pickups`
- `Assets/_Project/Art/Sprites/Player`
- `Assets/_Project/Art/Sprites/Projectiles`
- `Assets/_Project/Art/Sprites/Props`
- `Assets/_Project/Art/Sprites/Tiles`
- `Assets/_Project/Art/Sprites/UI`
- `Assets/_Project/Art/Sprites/Weapons`

That is expected because the project uses `RuntimeTransparent` copies. However, many `RuntimeTransparent` assets are also unused:

- Most equipped player variants
- Buff icons
- Most UI icons
- Several UI panels and cards
- Level thumbnails in actual level select scenes
- Projectile VFX such as muzzle flash, impact spark and explosion burst
- Tile corner pieces and floor grate
- Horizontal doors/gates
- Several props such as healing station, security camera, wall terminal and extraction beacon
- Several hazard sprites such as warning signs and turret trap

## 5. Objects Too Large Or Too Small

Major size problems:

- Wall visuals are effectively too small or invisible. In `Level_01_Laboratory`, `WallSpriteRenderer` entries show `m_Size: {x: 1, y: 1}` while the original root wall renderers are often disabled. The same pattern appears across all five campaign scenes.
- Each campaign level has around 132 `WallSpriteRenderer` entries and around 99 disabled wall renderer blocks. This strongly indicates that the visual child replacement did not preserve the wall footprint.
- The art pack sprites are 1254x1254 at 100 PPU, so using them as simple sprites without tiled sizing creates large art files squeezed into tiny world-space objects.
- Main menu and level select buttons use ornate frames that are too visually heavy for short text buttons.
- HUD decorative frames can overpower text when they are applied directly to small resource panels.
- Toxic puddles and some pickup visuals read larger and louder than gameplay importance suggests.
- Player visuals are readable, but weapon/pivot visuals can look small compared to the body and room scale.

## 6. Transparency And Background Problems

The original art pack PNGs have opaque backgrounds. Sampled corner alpha values:

- `Sprites/Player/Player_Body_Base.png`: corner alpha 255
- `Sprites/Tiles/Tile_WallHorizontal.png`: corner alpha 255
- `Sprites/UI/Panels/UI_PauseMenuPanel.png`: corner alpha 255

The `RuntimeTransparent` copies have transparent corners:

- `RuntimeTransparent/Player/Player_Body_Base.png`: corner alpha 0
- `RuntimeTransparent/Tiles/Tile_WallHorizontal.png`: corner alpha 0
- `RuntimeTransparent/UI/Panels/UI_PauseMenuPanel.png`: corner alpha 0

Recommendation: gameplay should continue to use `RuntimeTransparent` copies unless source assets are cleaned/re-exported. Using original art pack files directly will bring back opaque square backgrounds.

Import settings are mostly correct:

- Texture Type: Sprite
- Alpha Is Transparency: enabled
- PPU: 100
- Compression: uncompressed/high quality
- Filter Mode: bilinear

The issue is not primarily import settings. It is sprite footprint, UI layout and scene composition.

## 7. Sorting Layers And Orders

Current issues:

- Environment objects mostly use the default sorting layer, with floors at about `-5`, walls around `1`, doors around `8`, and player/weapon/projectiles higher.
- This is workable but fragile because the project does not clearly separate sorting layers like `Floor`, `Environment`, `Actors`, `Projectiles`, `UI`.
- Root wall renderers are disabled, so if a `Visual/WallSpriteRenderer` is mis-sized or missing, the gameplay wall collider exists but the wall becomes invisible.
- Cover/wall/door sorting can look inconsistent when sprites overlap, especially with large transparent art bounds.
- HUD sorting is Canvas-based, but the UI style pass previously inserted decorative images that competed with text.

## 8. UI Problems

Observed and inspected UI problems:

- Main menu composition is visually inconsistent: oversized title, button art not matching button dimensions, and decorative panel art competing with text.
- Level select uses button frames but no useful level thumbnails in-scene, despite thumbnail assets existing.
- HUD resource panel text is packed tightly and can overlap with decorative frames.
- Health bar is readable, but frame/fill styling needs fixed dimensions and consistent anchoring.
- Boss health panel uses better art than the rest of the HUD but still needs consistent spacing.
- Shop, buff choice, game over and victory panels need one shared visual grammar rather than separate ad hoc frame choices.
- UI icons were not a good fit when automatically inserted into small panels; they made the HUD noisier instead of clearer.

## 9. Closest Room/Level To Final Quality

`Level_01_Laboratory.unity` is closest to the intended final quality because:

- It is the first campaign level.
- The laboratory theme best matches the current sci-fi wall/floor art.
- It already contains player, HUD, room structure, doors, covers, enemies, pickups and objective flow.

However, it is not ready visually. The wall sizing problem must be fixed before judging composition. After that, `Level_01_Laboratory` should be polished first and used as the visual standard for other levels.

## 10. Top 10 Visual Problems

1. Wall visuals are not preserving gameplay wall footprint, so levels can appear to have missing walls.
2. The visual pass disabled root SpriteRenderers before proving child visuals were correctly sized.
3. All campaign levels reuse the same sci-fi floor/wall art, so Prison, Zombie City, Sci-Fi Base and Horror Hospital do not read as distinct places.
4. Main menu and level select UI use decorative art at unsuitable dimensions.
5. HUD panel styling competes with text and reduces readability.
6. Original art pack assets have opaque backgrounds; only `RuntimeTransparent` copies are safe for gameplay.
7. Level thumbnails exist but are not properly integrated into the actual Level Select scene.
8. Sorting is functional but implicit; there is no clear layer/order policy for floor, environment, actors, projectiles and VFX.
9. Props and hazards lack consistent scale relative to player and walls.
10. There is no single polished reference room; visual changes were applied globally before one room was approved.

## Polish Plan

### Phase A: Import, Transparency And Sprite Scale Fixes

Goal: make current assets safe and predictable.

- Keep using `RuntimeTransparent` assets for gameplay.
- Do not use original art pack PNGs until opaque backgrounds are fixed.
- Repair wall/floor visual sizing by deriving `SpriteRenderer.size` from the gameplay collider or original root renderer footprint.
- Keep root SpriteRenderers disabled only after child visual sizing is verified.
- Standardize visual scales for player, weapons, enemies, pickups, props, cover and hazards.
- Add an audit/repair utility that reports missing or tiny visual children before saving scenes.

### Phase B: One-Room/One-Level Visual Composition Polish

Goal: polish only `Level_01_Laboratory` first.

- Choose one representative room in `Level_01_Laboratory`.
- Fix walls, floor tiles, door placement, cover spacing and prop placement in that room.
- Add floor variation: grates, decals, hazard stripes, wall lights, terminals.
- Keep colliders and gameplay object positions stable; scale/move only `Visual` children where possible.
- Verify camera framing after wall sizes are repaired.
- Do not apply to all scenes until this room looks approved.

### Phase C: UI/HUD Polish

Goal: restore readability and use art as framing, not noise.

- Rebuild Main Menu and Level Select layout with fixed responsive dimensions.
- Use simpler button frames or correctly sliced button sprites at stable sizes.
- Use level thumbnails as cards only in Level Select, not as button backgrounds.
- Keep HUD minimal: health bar, ammo, money, weapon and buffs in readable text blocks.
- Avoid auto-inserting icons into cramped HUD rows unless layout is redesigned around them.
- Create one UI prefab/style set for HUD, menu, modal, button and card.

### Phase D: Enemies, Player And Props Readability Polish

Goal: make gameplay silhouettes clear.

- Keep player visual children, but tune body, weapon and shadow scale in one scene first.
- Ensure enemies are visually distinct by archetype: melee, ranged, miniboss, boss.
- Add optional VFX only where it supports readability: muzzle flash, projectile trail, impact spark, death burst.
- Scale pickups smaller than enemies and larger than bullets.
- Use props as room dressing without blocking player readability.

### Phase E: Lighting And Post-Processing Polish

Goal: add atmosphere without hiding gameplay.

- Use URP 2D lights only if already available and stable.
- Add subtle global light and room accent lights.
- Add small cyan/red/orange local lights near doors, terminals, hazards and boss areas.
- Use camera background and vignette-like UI/backdrop carefully; avoid darkening bullets/enemies.
- Test performance and readability at the target Game view scale.

## Files Inspected

- `ProjectSettings/EditorBuildSettings.asset`
- `Assets/_Project/Scripts/Menu/MainMenuController2D.cs`
- `Assets/_Project/Scripts/Menu/LevelSelectController2D.cs`
- `Assets/_Project/Scripts/Editor/CodEvadareUiMenuVisuals.cs`
- `Assets/_Project/Scripts/Editor/CodEvadarePickupPropHazardEnvironmentVisuals.cs`
- `Assets/_Project/Scripts/Editor/CodEvadarePlayerWeaponProjectileVisuals.cs`
- `Assets/_Project/Scripts/Editor/CodEvadareEnemyBossVisuals.cs`
- `Assets/_Project/Scripts/Editor/CodEvadareArtImportSettings.cs`
- `Assets/_Project/Art/CODEX_PROMPT_VISUAL_INTEGRATION.md`
- `Assets/_Project/Art/ASSET_MANIFEST.json`
- Representative `.meta` files under `Assets/_Project/Art/Sprites`
- Scene YAML files under `Assets/_Project/Scenes/Game`
- Scene YAML files under `Assets/_Project/Scenes/Levels`
- Prefab YAML files under `Assets/_Project/Prefabs`

## Scenes Inspected

- `Assets/_Project/Scenes/Game/MainMenu.unity`
- `Assets/_Project/Scenes/Game/LevelSelect.unity`
- `Assets/_Project/Scenes/Game/UI_Showcase.unity`
- `Assets/_Project/Scenes/Levels/Level_01_Laboratory.unity`
- `Assets/_Project/Scenes/Levels/Level_02_Prison.unity`
- `Assets/_Project/Scenes/Levels/Level_03_ZombieCity.unity`
- `Assets/_Project/Scenes/Levels/Level_04_SciFiBase.unity`
- `Assets/_Project/Scenes/Levels/Level_05_HorrorHospital.unity`
- Prototype scenes were inventoried but not treated as final visual targets.

## Exact Recommended Next Prompt

Use this as the next implementation prompt:

```text
Continue in the existing Unity project Cod_Evadare. Implement only Phase A from Assets/_Project/Docs/VISUAL_POLISH_AUDIT.md.

Rules:
- Do not change gameplay logic.
- Do not replace prefab roots.
- Do not modify player, weapon or enemy stats.
- Do not manually edit scene YAML.
- Do not delete scenes.
- Keep using RuntimeTransparent sprites for gameplay.

Goal:
Create a conservative visual repair tool that fixes import/transparency and sprite footprint problems without redesigning levels.

Requirements:
1. Update/extend the existing editor visual tools so wall and floor Visual child SpriteRenderers preserve the gameplay footprint from BoxCollider2D or the previous root SpriteRenderer.
2. Add a report-only menu item that scans campaign scenes and logs tiny/missing visual children, disabled root SpriteRenderers, and unsafe source art references.
3. Do not apply global UI redesign.
4. Do not move gameplay objects.
5. After implementation, list files changed and tell me exactly which Unity menu item to run and what to inspect manually.
```
