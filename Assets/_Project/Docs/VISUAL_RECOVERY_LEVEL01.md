# Cod: Evadare - Level 01 Visual Recovery

Date: 2026-06-22

## Scope

Focused visual recovery for `Assets/_Project/Scenes/Levels/Level_01_Laboratory.unity` only. Gameplay logic, stats, colliders, triggers, tags, layers and prefab roots were not intentionally changed.

## Files Changed

- `Assets/_Project/Scripts/Editor/CodEvadareLevel01VisualRecovery.cs`
- `Assets/_Project/Docs/VISUAL_RECOVERY_LEVEL01.md`
- generated sprites under `Assets/_Project/Art/Sprites/RuntimeTransparent/VisualRecovery` after running `Tools/Cod Evadare/Art/Recovery/Recover Level 01 Laboratory Visuals`
- `Assets/_Project/Scenes/Levels/Level_01_Laboratory.unity` after running `Tools/Cod Evadare/Art/Recovery/Recover Level 01 Laboratory Visuals`

The latest recovery pass generates small clean sci-fi floor, wall, door, grate, hazard stripe and HUD panel sprites under `RuntimeTransparent/VisualRecovery`. These are used for gameplay footprint visuals because the larger art-pack room panel sprites are too large to tile directly as walls or floors.

## Scene Objects Changed

- Environment objects repaired: 55
- Decorative floor/prop visuals added under `VisualRecovery_Level01`: 12
- Shadows repaired/added: 0
- HUD panels repaired: 9

Representative changed scene objects:
- `Level_01_Laboratory/Start_Area/StartFloor`
- `Level_01_Laboratory/Start_Area/StartWall_Top`
- `Level_01_Laboratory/Start_Area/StartWall_Bottom`
- `Level_01_Laboratory/Start_Area/StartWall_Left`
- `Level_01_Laboratory/Combat_Room_01/Floor`
- `Level_01_Laboratory/Combat_Room_01/Walls/Wall_Top`
- `Level_01_Laboratory/Combat_Room_01/Walls/Wall_Bottom`
- `Level_01_Laboratory/Combat_Room_01/Walls/Wall_Left_Upper`
- `Level_01_Laboratory/Combat_Room_01/Walls/Wall_Left_Lower`
- `Level_01_Laboratory/Combat_Room_01/Walls/Wall_Right_Upper`
- `Level_01_Laboratory/Combat_Room_01/Walls/Wall_Right_Lower`
- `Level_01_Laboratory/Combat_Room_01/Doors/Door_Left`
- `Level_01_Laboratory/Combat_Room_01/Doors/Door_Right`
- `Level_01_Laboratory/Shop_Area/Floor`
- `Level_01_Laboratory/Shop_Area/ShopWall_Top`
- `Level_01_Laboratory/Shop_Area/ShopWall_Bottom`
- `Level_01_Laboratory/Shop_Area/ShopWall_Left_Upper`
- `Level_01_Laboratory/Shop_Area/ShopWall_Left_Lower`
- `Level_01_Laboratory/Shop_Area/ShopWall_Right_Upper`
- `Level_01_Laboratory/Shop_Area/ShopWall_Right_Lower`
- `Level_01_Laboratory/Shop_Area/ShopItem_Health`
- `Level_01_Laboratory/Shop_Area/ShopItem_Ammo`
- `Level_01_Laboratory/Shop_Area/ShopItem_Weapon`
- `Level_01_Laboratory/Shop_Area/ObjectiveTrigger_Shop`
- `Level_01_Laboratory/Combat_Room_02/Floor`
- `Level_01_Laboratory/Combat_Room_02/Walls/Wall_Top`
- `Level_01_Laboratory/Combat_Room_02/Walls/Wall_Bottom`
- `Level_01_Laboratory/Combat_Room_02/Walls/Wall_Left_Upper`
- `Level_01_Laboratory/Combat_Room_02/Walls/Wall_Left_Lower`
- `Level_01_Laboratory/Combat_Room_02/Walls/Wall_Right_Upper`
- `Level_01_Laboratory/Combat_Room_02/Walls/Wall_Right_Lower`
- `Level_01_Laboratory/Combat_Room_02/Doors/Door_Left`
- `Level_01_Laboratory/Combat_Room_02/Doors/Door_Right`
- `Level_01_Laboratory/Miniboss_Room/Floor`
- `Level_01_Laboratory/Miniboss_Room/Walls/Wall_Top`
- `Level_01_Laboratory/Miniboss_Room/Walls/Wall_Bottom`
- `Level_01_Laboratory/Miniboss_Room/Walls/Wall_Left_Upper`
- `Level_01_Laboratory/Miniboss_Room/Walls/Wall_Left_Lower`
- `Level_01_Laboratory/Miniboss_Room/Walls/Wall_Right_Upper`
- `Level_01_Laboratory/Miniboss_Room/Walls/Wall_Right_Lower`
- `Level_01_Laboratory/Miniboss_Room/Doors/Door_Left`
- `Level_01_Laboratory/Miniboss_Room/Doors/Door_Right`
- `Level_01_Laboratory/Boss_Room/Floor`
- `Level_01_Laboratory/Boss_Room/Walls/Wall_Top`
- `Level_01_Laboratory/Boss_Room/Walls/Wall_Bottom`
- `Level_01_Laboratory/Boss_Room/Walls/Wall_Left_Upper`
- `Level_01_Laboratory/Boss_Room/Walls/Wall_Left_Lower`
- `Level_01_Laboratory/Boss_Room/Walls/Wall_Right_Upper`
- `Level_01_Laboratory/Boss_Room/Walls/Wall_Right_Lower`
- `Level_01_Laboratory/Boss_Room/Doors/Door_Left`
- `Level_01_Laboratory/Boss_Room/Doors/Door_Right`
- `Level_01_Laboratory/Boss_Room/Cover/Cover_01`
- `Level_01_Laboratory/Boss_Room/Cover/Cover_02`
- `Level_01_Laboratory/Boss_Room/Cover/Cover_03`
- `Level_01_Laboratory/Boss_Room/Cover/Cover_04`
- `Level_01_Laboratory/Combat_Room_01/ToxicPuddle`
- `Level_01_Laboratory/Combat_Room_01/ToxicPuddle/Visual/HazardSpriteRenderer`
- `Level_01_Laboratory/Combat_Room_01/ToxicPuddle`
- `Level_01_Laboratory/Combat_Room_01/ToxicPuddle/Visual/HazardSpriteRenderer`
- `Level_01_Laboratory/Combat_Room_02/ToxicPuddle`
- `Level_01_Laboratory/Combat_Room_02/ToxicPuddle/Visual/HazardSpriteRenderer`
- `Level_01_Laboratory/Combat_Room_02/ToxicPuddle`
- `Level_01_Laboratory/Combat_Room_02/ToxicPuddle/Visual/HazardSpriteRenderer`
- `Level_01_Laboratory/UI/Canvas/HealthPanel`
- `Level_01_Laboratory/UI/Canvas/ResourcePanel`
- `Level_01_Laboratory/UI/Canvas/ObjectivePanel`
- `Level_01_Laboratory/UI/Canvas/ShopPanel`
- `Level_01_Laboratory/UI/Canvas/BossHealthPanel`
- `Level_01_Laboratory/UI/Canvas/BuffChoicePanel`
- `Level_01_Laboratory/UI/Canvas/GameOverPanel`
- `Level_01_Laboratory/UI/Canvas/VictoryPanel`
- `Level_01_Laboratory/UI/Canvas/PausePanel`

## Prefabs Changed

None. This recovery pass is scene-focused and does not replace or edit prefab roots.

## Remaining Visual Issues

- The other campaign levels still need separate visual passes after Level 01 is approved.
- Main Menu and Level Select were intentionally not redesigned in this pass.
- HUD is only made readable, not fully redesigned.
- Lighting and post-processing are still future work.
- Runtime-spawned enemy visuals depend on their existing prefabs; this pass does not edit those prefabs.

## Play Mode Test Checklist

1. Open `Assets/_Project/Scenes/Levels/Level_01_Laboratory.unity`.
2. Confirm the floor is visible and no longer a black void.
3. Confirm walls are visible, aligned with room/corridor boundaries and not clipped in huge corner chunks.
4. Confirm doors and locked gates line up with wall openings.
5. Press Play.
6. Move with WASD / arrow keys.
7. Aim with the mouse.
8. Shoot with left click.
9. Reload with R while alive.
10. Verify enemies, pickups, hazards, shop, buffs and keycards still work.
11. Verify HUD text remains dynamic and readable: HP, ammo, reserve ammo, money, weapon, buffs and keycards.
12. Check Console for missing reference, missing sprite or compile errors.
