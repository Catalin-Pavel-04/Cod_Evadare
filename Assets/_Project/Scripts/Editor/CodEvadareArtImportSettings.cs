#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class CodEvadareArtImportSettings
{
    private const string SpriteRoot = "Assets/_Project/Art/Sprites";
    private const float SpritePixelsPerUnit = 100f;

    [MenuItem("Tools/Cod Evadare/Art/Phase 1/Apply Visual Import Settings")]
    public static void ApplyVisualImportSettings()
    {
        if (!AssetDatabase.IsValidFolder(SpriteRoot))
        {
            Debug.LogWarning($"Visual import settings skipped because folder was not found: {SpriteRoot}");
            return;
        }

        string[] textureGuids = AssetDatabase.FindAssets("t:Texture2D", new[] { SpriteRoot });
        int changedCount = 0;

        foreach (string textureGuid in textureGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(textureGuid);
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;

            if (importer == null)
            {
                continue;
            }

            if (ConfigureSpriteImporter(importer))
            {
                importer.SaveAndReimport();
                changedCount++;
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Cod Evadare Phase 1 visual import settings checked {textureGuids.Length} sprite textures and updated {changedCount} importers.");
    }

    private static bool ConfigureSpriteImporter(TextureImporter importer)
    {
        bool changed = false;

        if (importer.textureType != TextureImporterType.Sprite)
        {
            importer.textureType = TextureImporterType.Sprite;
            changed = true;
        }

        if (importer.spriteImportMode != SpriteImportMode.Single)
        {
            importer.spriteImportMode = SpriteImportMode.Single;
            changed = true;
        }

        if (!Mathf.Approximately(importer.spritePixelsPerUnit, SpritePixelsPerUnit))
        {
            importer.spritePixelsPerUnit = SpritePixelsPerUnit;
            changed = true;
        }

        if (importer.filterMode != FilterMode.Bilinear)
        {
            importer.filterMode = FilterMode.Bilinear;
            changed = true;
        }

        if (importer.mipmapEnabled)
        {
            importer.mipmapEnabled = false;
            changed = true;
        }

        if (!importer.alphaIsTransparency)
        {
            importer.alphaIsTransparency = true;
            changed = true;
        }

        if (importer.textureCompression != TextureImporterCompression.Uncompressed)
        {
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            changed = true;
        }

        if (importer.npotScale != TextureImporterNPOTScale.None)
        {
            importer.npotScale = TextureImporterNPOTScale.None;
            changed = true;
        }

        TextureImporterSettings settings = new TextureImporterSettings();
        importer.ReadTextureSettings(settings);

        if (settings.spriteMeshType != SpriteMeshType.FullRect)
        {
            settings.spriteMeshType = SpriteMeshType.FullRect;
            importer.SetTextureSettings(settings);
            changed = true;
        }

        return changed;
    }
}
#endif
