# Codex Prompt — Cod: Evadare Visual Integration Pass

You are working inside the existing Unity repository:

https://github.com/Catalin-Pavel-04/Cod_Evadare

The user has provided an art pack zip. After extracting it into the repository root, the assets should be located under:

Assets/_Project/Art/

Project context:
- Unity version: 2022.3.62f3 LTS.
- Render pipeline: Universal 2D / URP.
- Game: Cod: Evadare.
- Genre: 2D top-down sci-fi / prison-escape shooter with roguelike progression.
- Existing gameplay logic must be preserved.
- Do not reinitialize the project.
- Do not delete existing scenes.
- Do not manually rewrite Unity scene YAML unless absolutely necessary.
- Do not modify Packages/manifest.json.
- Do not install packages.
- Use existing Unity UI systems already in the project.

GOAL:
Integrate the provided visual asset pack into the existing game without breaking gameplay.
The current game already works logically; this is a visual implementation and polish pass.

ASSET LOCATION:
Use the following folders:
- Assets/_Project/Art/Sprites/Player
- Assets/_Project/Art/Sprites/Weapons
- Assets/_Project/Art/Sprites/Projectiles
- Assets/_Project/Art/Sprites/Pickups
- Assets/_Project/Art/Sprites/Buffs
- Assets/_Project/Art/Sprites/Props
- Assets/_Project/Art/Sprites/Hazards
- Assets/_Project/Art/Sprites/Tiles
- Assets/_Project/Art/Sprites/UI
- Assets/_Project/Art/Thumbnails
- Assets/_Project/Art/References/Mockups

IMPORTANT RULES:
1. Preserve all gameplay scripts, colliders, Rigidbody2D components, triggers, serialized references, tags and layers.
2. Do not replace gameplay prefab roots. Add/update child GameObjects named `Visual` where needed.
3. If a prefab already has a SpriteRenderer, update only the visual sprite safely.
4. If a scene object already works, do not move it unless necessary.
5. Use visuals as children of gameplay objects so colliders remain stable.
6. Do not use reference/mockup images as gameplay sprites. They are only visual references.
7. Use the `Thumbnails` folder only for level select previews.
8. Use UI panels as reusable frames; Unity should provide dynamic text, numbers and icons.
9. Run the project after changes and fix compile errors immediately.

PHASE 1 — Import settings
Create an Editor script if useful:
Assets/_Project/Scripts/Editor/CodEvadareArtImportSettings.cs

Set import settings for all sprites under Assets/_Project/Art/Sprites:
- Texture Type: Sprite (2D and UI)
- Sprite Mode: Single
- Pixels Per Unit: 100
- Filter Mode: Bilinear
- Compression: None or high quality
- Alpha Is Transparency: enabled
- Mesh Type: Full Rect where appropriate

For thumbnails and references:
- Keep as Sprite or Default texture as appropriate.
- They are not gameplay sprites.

PHASE 2 — Player visual setup
Find the existing Player prefab/object.
Preserve all gameplay components.
Add or update:
Player
  Visual
    BodySpriteRenderer
    WeaponPivot
      WeaponSpriteRenderer
    SelectionRing
    Shadow

Use:
- Player_Body_Base.png as default player body
- Player_Crosshair.png for crosshair if the project has crosshair logic
- Player_SelectionRing.png as a child under player if useful
- Player_Shadow.png as a child under player

If the project has weapon switching:
- connect weapon icons/sprites from Assets/_Project/Art/Sprites/Weapons
- keep weapon statistics and firing logic unchanged

PHASE 3 — Weapons and projectiles
Update existing weapon pickups, projectile prefabs and UI icons where safe.
Use:
- Weapon_Pistol.png
- Weapon_Revolver.png
- Weapon_SMG.png
- Weapon_Shotgun.png
- Weapon_AssaultRifle.png
- Weapon_PlasmaRifle.png
- Weapon_ArcaneStaff.png
- Weapon_Grenade.png
- Weapon_MeleeBlade.png
- Projectile_BulletTracer.png
- Projectile_PlasmaBolt.png
- Projectile_ArcaneOrb.png
- Projectile_ToxicSpit.png
- Projectile_EnemyLaser.png
- Effect_MuzzleFlash_Small.png
- Effect_MuzzleFlash_Large.png
- Effect_ImpactSpark.png
- Effect_ExplosionBurst.png

Preserve projectile speed, damage, lifetime, collision logic and prefabs.

PHASE 4 — Pickups and loot
Update pickup visuals only:
- Pickup_Medkit.png
- Pickup_Ammo.png
- Pickup_Money.png
- Pickup_Keycard.png
- Pickup_BuffVial.png
- Pickup_ArmorShard.png
- Pickup_WeaponCrate.png
- LootRing_Rare.png
- LootRing_Epic.png
- LootRing_Legendary.png

Do not change pickup values or trigger logic.

PHASE 5 — Enemies and bosses
For every existing enemy prefab/object, preserve AI, health, contact damage, Rigidbody2D and colliders.
Add/update `Visual` child with the closest sprite:
- Enemy_PrisonGuard.png
- Enemy_RangedGuard.png
- Enemy_RiotGuard.png
- Enemy_LabMutant.png
- Enemy_ZombieWalker.png
- Enemy_ZombieRunner.png
- Enemy_Drone.png
- Enemy_SecurityBot.png
- Enemy_HauntedMedic.png
- Enemy_Ghost.png

For minibosses and bosses use:
- Miniboss_RiotBrute.png
- Miniboss_LabAlpha.png
- Miniboss_Necromancer.png
- Miniboss_ReactorGuardian.png
- Miniboss_Surgeon.png
- Miniboss_ShadowWarden.png
- Boss_Experiment01.png
- Boss_TheWarden.png
- Boss_Abomination.png
- Boss_AICore.png
- Boss_NightmareDoctor.png
- Boss_FinalEscapeOverlord.png

Add optional VFX children for necromancer:
- Necromancer_Aura.png
- Necromancer_SummonEffect.png

PHASE 6 — Environment and props
Use environment sprites for floors, walls, gates, doors, cover and modular decorations:
- Tile_WallHorizontal.png
- Tile_WallVertical.png
- Tile_InnerCorner.png
- Tile_OuterCorner.png
- Tile_DoorFrame.png
- Tile_PrisonFence.png
- Tile_SciFiFloor.png
- Tile_FloorGrate.png
- Tile_HazardStripeFloor.png
- Tile_MetalPillar.png

Use props:
- Prop_AutomaticDoor_Horizontal.png
- Prop_AutomaticDoor_Vertical.png
- Prop_LockedPrisonGate_Horizontal.png
- Prop_LockedPrisonGate_Vertical.png
- Prop_WallTerminal.png
- Prop_ShopKiosk.png
- Prop_ExtractionBeacon.png
- Prop_ObjectiveTerminal.png
- Prop_LowCoverBlock.png
- Prop_TallCoverCrate.png
- Prop_ExplosiveBarrel.png
- Prop_HealingStation.png
- Prop_SecurityCamera.png
- Prop_AlarmLight.png

Do not move colliders unless absolutely necessary.
If a visual sprite is larger/smaller than the collider, scale only the visual child, not the gameplay object.

PHASE 7 — Hazards
Update hazard visuals only:
- Hazard_LaserEmitterPost.png
- Hazard_LaserBeamHorizontal.png
- Hazard_LaserBeamVertical.png
- Hazard_ElectricFloorTile.png
- Hazard_ToxicPuddleSmall.png
- Hazard_ToxicPuddleLarge.png
- Hazard_BloodlessDangerStain.png
- Hazard_TurretTrap.png
- Hazard_WarningSign.png
- Marker_BossWarning.png

For laser beams:
- Keep emitters and beams separate.
- Scale the beam sprite along X or Y to fit the hazard area.
- Preserve damage trigger/collider size.

PHASE 8 — UI and menus
Update UI visuals without baking text into images.
Use UI icons from:
Assets/_Project/Art/Sprites/UI/Icons

Use UI panels from:
Assets/_Project/Art/Sprites/UI/Panels

Use UI buttons from:
Assets/_Project/Art/Sprites/UI/Buttons

For level select thumbnails use:
Assets/_Project/Art/Thumbnails

If Main Menu / Level Select scenes exist, update them visually.
If they do not exist, create visual prototype scenes only:
- Assets/_Project/Scenes/MainMenu_Visual.unity
- Assets/_Project/Scenes/LevelSelect_Visual.unity

Do not claim they are fully wired unless actual scene controller scripts exist.

PHASE 9 — Showcase scene
Create:
Assets/_Project/Scenes/Visual_Showcase.unity

Show grouped sections:
- Player
- Weapons
- Projectiles/VFX
- Pickups/Loot/Buffs
- Props
- Hazards
- Tiles
- Enemies
- Minibosses/Bosses
- UI
- Level thumbnails

PHASE 10 — Documentation
Create or update:
- Assets/_Project/Docs/VISUAL_ASSET_INTEGRATION.md
- Assets/_Project/Docs/ASSET_USAGE_MANIFEST.md

Include:
- files used
- prefabs updated
- scenes updated
- scenes skipped
- manual testing checklist
- any asset transparency or scaling issues found

ACCEPTANCE CRITERIA:
- Unity compiles without errors.
- Existing gameplay still works.
- No duplicate class definitions.
- No broken serialized references.
- No external packages added.
- Player can move, aim, shoot and reload.
- Enemies spawn/chase/shoot/die as before.
- Pickups still collect.
- Doors/gates/hazards still work.
- Shop and buff UI still work if already implemented.
- Game over/victory UI still works if already implemented.
- Visual_Showcase scene exists.
- Updated scenes/prefabs are listed in documentation.

After finishing, summarize:
1. Files created/changed.
2. Prefabs updated.
3. Scenes updated.
4. Any skipped scenes.
5. How to test in Play Mode.
6. Any assets that still require manual transparency cleanup.
