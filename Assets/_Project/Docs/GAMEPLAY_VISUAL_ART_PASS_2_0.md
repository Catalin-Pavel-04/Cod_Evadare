# Cod: Evadare - Gameplay Visual Art Pass 2.0

This pass refreshes generated gameplay sprites in-place so existing prefab and scene references keep their Unity GUIDs.

Version 2.0 uses larger source sprites, smoother import settings, stronger silhouettes, richer environment panels, and clearer pickups/projectiles. Sprite pixels-per-unit are matched to each generated PNG size so bigger art does not stretch the map.

Run `Tools/Cod Evadare/Art/Build Gameplay Visual Art Pass` from Unity to apply import settings. Existing polished PNGs are preserved; missing generated sprites are recreated with fallback procedural art.

Run `Tools/Cod Evadare/Art/Apply Mobile Art Scale To Open Scene` on an open generated scene to convert existing walls, floors, doors, gates, bars, and corridor pieces to tiled rendering with corrected sizes. Save the scene after checking the layout.

Generated assets are under `Assets/_Project/Art/Generated` and include richer player, enemy, pickup, weapon, projectile, wall, floor, door, cover, hazard, and boss sprites.

Manual verification: open the current level scenes and confirm actor silhouettes, walls, doors, pickups, hazards, and projectiles are readable in Play Mode.
