# Cod: Evadare — Final Art Pack

This zip contains an organized Unity art pack for the project `Cod_Evadare`.

## Main folders

- `Assets/_Project/Art/Sprites/Player`
- `Assets/_Project/Art/Sprites/Weapons`
- `Assets/_Project/Art/Sprites/Projectiles`
- `Assets/_Project/Art/Sprites/Pickups`
- `Assets/_Project/Art/Sprites/Buffs`
- `Assets/_Project/Art/Sprites/Props`
- `Assets/_Project/Art/Sprites/Hazards`
- `Assets/_Project/Art/Sprites/Tiles`
- `Assets/_Project/Art/Sprites/UI`
- `Assets/_Project/Art/Thumbnails`
- `Assets/_Project/Art/References/Mockups`

## Recommended Unity import settings

For gameplay sprites:
- Texture Type: Sprite (2D and UI)
- Sprite Mode: Single
- Pixels Per Unit: 100
- Mesh Type: Full Rect
- Filter Mode: Bilinear
- Compression: None or High Quality
- Alpha Is Transparency: enabled

For UI sprites:
- Texture Type: Sprite (2D and UI)
- Sprite Mode: Single
- Use `Image` components in Canvas
- Use 9-slice/Sliced mode for panels where useful

For thumbnails/mockups:
- Use them as UI images or references only.
- Do not use gameplay mockups as live gameplay sprites.

## Transparency note

The images were prepared as RGBA PNG files where possible. Some AI-generated images may still need manual cleanup if a fake checkerboard or light halo is visible after import. Test every critical sprite on a dark and colored background inside Unity.

## Files

See `ASSET_MANIFEST.csv` and `ASSET_MANIFEST.json`.
