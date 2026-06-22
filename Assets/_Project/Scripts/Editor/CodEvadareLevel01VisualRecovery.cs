#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class CodEvadareLevel01VisualRecovery
{
    private const string Level01ScenePath = "Assets/_Project/Scenes/Levels/Level_01_Laboratory.unity";
    private const string RuntimeArtRoot = "Assets/_Project/Art/Sprites/RuntimeTransparent/";
    private const string GeneratedRecoveryArtRoot = "Assets/_Project/Art/Sprites/RuntimeTransparent/VisualRecovery";
    private const string DocumentationPath = "Assets/_Project/Docs/VISUAL_RECOVERY_LEVEL01.md";
    private const string RecoveryRootName = "VisualRecovery_Level01";
    private const float GeneratedSpritePixelsPerUnit = 100f;

    [MenuItem("Tools/Cod Evadare/Art/Recovery/Recover Level 01 Laboratory Visuals")]
    public static void RecoverLevel01LaboratoryVisuals()
    {
        if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            Debug.LogWarning("Level 01 visual recovery cancelled because the current scene has unsaved changes.");
            return;
        }

        string originalScenePath = SceneManager.GetActiveScene().path;
        RecoveryStats stats = new RecoveryStats();

        try
        {
            CodEvadareArtImportSettings.ApplyVisualImportSettings();
            EnsureGeneratedRecoverySprites();

            Scene scene = EditorSceneManager.OpenScene(Level01ScenePath, OpenSceneMode.Single);
            ArtLibrary art = ArtLibrary.Load();

            RecoverWorldVisuals(scene, art, stats);
            RecoverScaleAndShadows(scene, art, stats);
            RecoverHud(scene, art, stats);

            EditorSceneManager.SaveScene(scene);
            WriteDocumentation(stats);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log(
                $"Level 01 visual recovery complete. " +
                $"Environment objects repaired: {stats.EnvironmentObjectsRepaired}. " +
                $"Decorative floor/prop visuals added: {stats.DecorativeVisualsAdded}. " +
                $"Shadows repaired: {stats.ShadowsRepaired}. HUD panels repaired: {stats.HudPanelsRepaired}.");
        }
        finally
        {
            if (!string.IsNullOrEmpty(originalScenePath) && originalScenePath != Level01ScenePath)
            {
                EditorSceneManager.OpenScene(originalScenePath, OpenSceneMode.Single);
            }
        }
    }

    private static void RecoverWorldVisuals(Scene scene, ArtLibrary art, RecoveryStats stats)
    {
        Bounds levelBounds = CalculateLevelBounds(scene);
        GameObject recoveryRoot = EnsureSceneRoot(scene, RecoveryRootName);
        ClearChildren(recoveryRoot.transform);

        AddFloorBackdrop(recoveryRoot.transform, levelBounds, art, stats);
        AddFloorDetails(recoveryRoot.transform, levelBounds, art, stats);
        AddLaboratoryAccentProps(recoveryRoot.transform, levelBounds, art, stats);

        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (Transform transform in root.GetComponentsInChildren<Transform>(true))
            {
                if (transform == null || transform.gameObject == recoveryRoot || IsUnderRecoveryRoot(transform) || IsUnderCanvas(transform) || IsVisualDescendant(transform))
                {
                    continue;
                }

                GameObject owner = transform.gameObject;
                EnvironmentKind kind = ClassifyEnvironment(owner);

                if (kind == EnvironmentKind.None)
                {
                    continue;
                }

                if (RepairEnvironmentObject(owner, kind, art))
                {
                    stats.EnvironmentObjectsRepaired++;
                    stats.SceneObjectsChanged.Add(GetHierarchyPath(owner.transform));
                }
            }
        }
    }

    private static bool RepairEnvironmentObject(GameObject owner, EnvironmentKind kind, ArtLibrary art)
    {
        SpriteRenderer rootRenderer = owner.GetComponent<SpriteRenderer>();
        bool createdVisual = false;
        SpriteRenderer visualRenderer = FindOrCreateVisualRenderer(owner, GetRendererName(kind), ref createdVisual);

        if (!TryGetLocalFootprint(owner, rootRenderer, visualRenderer, out Vector2 targetSize, out Vector2 targetOffset))
        {
            return EnsureRootVisible(rootRenderer);
        }

        bool changed = createdVisual;
        bool horizontal = targetSize.x >= targetSize.y;
        Sprite sprite = GetEnvironmentSprite(kind, horizontal, owner.name, art);

        if (sprite == null)
        {
            return EnsureRootVisible(rootRenderer);
        }

        SpriteDrawMode drawMode = kind == EnvironmentKind.Wall || kind == EnvironmentKind.Floor
            ? SpriteDrawMode.Tiled
            : SpriteDrawMode.Sliced;

        if (kind == EnvironmentKind.Cover || kind == EnvironmentKind.Prop)
        {
            changed |= ApplySimpleFittedVisual(visualRenderer, sprite, ClampDecorVisualSize(kind, targetSize), targetOffset, GetSortingOrder(kind, rootRenderer), 0.82f);
        }
        else
        {
            changed |= ApplySizedVisual(visualRenderer, sprite, drawMode, targetSize, targetOffset, GetSortingOrder(kind, rootRenderer), Color.white);
        }

        changed |= DisableBrokenExtraVisuals(owner, visualRenderer, targetSize);

        if (rootRenderer != null)
        {
            if (IsUsableVisual(visualRenderer, targetSize))
            {
                if (rootRenderer.enabled)
                {
                    rootRenderer.enabled = false;
                    EditorUtility.SetDirty(rootRenderer);
                    changed = true;
                }
            }
            else if (!rootRenderer.enabled && rootRenderer.sprite != null)
            {
                rootRenderer.enabled = true;
                EditorUtility.SetDirty(rootRenderer);
                changed = true;
            }
        }

        return changed;
    }

    private static void RecoverScaleAndShadows(Scene scene, ArtLibrary art, RecoveryStats stats)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (Transform transform in root.GetComponentsInChildren<Transform>(true))
            {
                if (transform == null || IsUnderCanvas(transform) || IsUnderRecoveryRoot(transform))
                {
                    continue;
                }

                GameObject owner = transform.gameObject;

                if (owner.CompareTag("Player"))
                {
                    if (RepairActorVisualScale(owner, 1.05f, 12))
                    {
                        stats.SceneObjectsChanged.Add(GetHierarchyPath(owner.transform));
                    }

                    if (EnsureShadow(owner, art.PlayerShadow, new Vector2(0.95f, 0.38f), 3))
                    {
                        stats.ShadowsRepaired++;
                    }

                    continue;
                }

                if (owner.GetComponent<EnemyHealth>() != null)
                {
                    if (RepairActorVisualScale(owner, 0.95f, 12))
                    {
                        stats.SceneObjectsChanged.Add(GetHierarchyPath(owner.transform));
                    }

                    if (EnsureShadow(owner, art.RoomShadow, new Vector2(0.8f, 0.32f), 3))
                    {
                        stats.ShadowsRepaired++;
                    }

                    continue;
                }

                if (IsPickup(owner))
                {
                    if (RepairPickupVisualScale(owner))
                    {
                        stats.SceneObjectsChanged.Add(GetHierarchyPath(owner.transform));
                    }

                    if (EnsureShadow(owner, art.RoomShadow, new Vector2(0.45f, 0.18f), 5))
                    {
                        stats.ShadowsRepaired++;
                    }

                    continue;
                }

                if (IsHazard(owner))
                {
                    if (RepairHazardVisualScale(owner))
                    {
                        stats.SceneObjectsChanged.Add(GetHierarchyPath(owner.transform));
                    }
                }
            }
        }
    }

    private static void RecoverHud(Scene scene, ArtLibrary art, RecoveryStats stats)
    {
        foreach (Canvas canvas in FindSceneComponents<Canvas>(scene))
        {
            foreach (RectTransform rectTransform in canvas.GetComponentsInChildren<RectTransform>(true))
            {
                GameObject current = rectTransform.gameObject;
                string lowerName = current.name.ToLowerInvariant();

                if (!lowerName.Contains("panel") && !lowerName.Contains("background"))
                {
                    continue;
                }

                if (lowerName.Contains("mainmenu") || lowerName.Contains("levelselect"))
                {
                    continue;
                }

                Sprite panelSprite = ChooseHudPanelSprite(lowerName, art);

                if (panelSprite == null)
                {
                    continue;
                }

                if (ApplyHudPanel(current, panelSprite, lowerName))
                {
                    stats.HudPanelsRepaired++;
                    stats.SceneObjectsChanged.Add(GetHierarchyPath(current.transform));
                }
            }

            foreach (Text text in canvas.GetComponentsInChildren<Text>(true))
            {
                if (StyleHudText(text))
                {
                    stats.SceneObjectsChanged.Add(GetHierarchyPath(text.transform));
                }
            }
        }
    }

    private static void AddFloorBackdrop(Transform parent, Bounds bounds, ArtLibrary art, RecoveryStats stats)
    {
        if (art.FloorTile == null)
        {
            return;
        }

        Vector2 size = new Vector2(Mathf.Max(8f, bounds.size.x + 3f), Mathf.Max(6f, bounds.size.y + 3f));
        SpriteRenderer backdrop = CreateRenderer(parent, "Floor_Backdrop", art.FloorTile, bounds.center + new Vector3(0f, 0f, 0.2f));
        ApplySizedVisual(backdrop, art.FloorTile, SpriteDrawMode.Tiled, size, Vector2.zero, -30, new Color(0.62f, 0.72f, 0.78f, 0.95f));
        stats.DecorativeVisualsAdded++;
    }

    private static void AddFloorDetails(Transform parent, Bounds bounds, ArtLibrary art, RecoveryStats stats)
    {
        Vector3 center = bounds.center;
        Vector3 extents = bounds.extents;

        AddTiledDetail(parent, "Floor_Grate_Left", art.FloorGrate, center + new Vector3(-extents.x * 0.35f, extents.y * 0.18f, 0.1f), new Vector2(1.25f, 0.65f), -24, stats);
        AddTiledDetail(parent, "Floor_Grate_Right", art.FloorGrate, center + new Vector3(extents.x * 0.32f, -extents.y * 0.18f, 0.1f), new Vector2(1.35f, 0.65f), -24, stats);
        AddTiledDetail(parent, "Hazard_Stripe_LeftDoor", art.HazardStripe, center + new Vector3(-extents.x + 1.4f, 0f, 0.1f), new Vector2(1.1f, 0.45f), -23, stats);
        AddTiledDetail(parent, "Hazard_Stripe_RightDoor", art.HazardStripe, center + new Vector3(extents.x - 1.4f, 0f, 0.1f), new Vector2(1.1f, 0.45f), -23, stats);
        AddSimpleDetail(parent, "Decal_CrackMetal_A", art.CrackDecal, center + new Vector3(-extents.x * 0.12f, extents.y * 0.38f, 0.1f), 0.075f, -22, stats);
        AddSimpleDetail(parent, "Decal_CrackMetal_B", art.CrackDecal, center + new Vector3(extents.x * 0.24f, -extents.y * 0.35f, 0.1f), 0.065f, -22, stats);
        AddSimpleDetail(parent, "Decal_Biohazard", art.BiohazardDecal, center + new Vector3(extents.x * 0.12f, extents.y * 0.04f, 0.1f), 0.06f, -22, stats);
    }

    private static void AddLaboratoryAccentProps(Transform parent, Bounds bounds, ArtLibrary art, RecoveryStats stats)
    {
        Vector3 center = bounds.center;
        Vector3 extents = bounds.extents;

        AddSimpleDetail(parent, "WallLight_North_A", art.CyanWallLight, center + new Vector3(-extents.x * 0.45f, extents.y - 0.35f, 0.1f), 0.08f, 5, stats);
        AddSimpleDetail(parent, "WallLight_North_B", art.CyanWallLight, center + new Vector3(extents.x * 0.35f, extents.y - 0.35f, 0.1f), 0.08f, 5, stats);
        AddSimpleDetail(parent, "WallTerminal_West", art.WallTerminal, center + new Vector3(-extents.x + 0.55f, extents.y * 0.22f, 0.1f), 0.045f, 6, stats);
        AddSimpleDetail(parent, "SciFiCrate_Corner", art.SmallCrate, center + new Vector3(extents.x - 1.1f, -extents.y + 0.9f, 0.1f), 0.055f, 4, stats);
    }

    private static void EnsureGeneratedRecoverySprites()
    {
        Directory.CreateDirectory(GeneratedRecoveryArtRoot);

        WriteGeneratedSprite("LabFloorTile_Clean.png", CreateFloorTileTexture());
        WriteGeneratedSprite("LabWallHorizontal_Clean.png", CreateWallTexture(horizontal: true));
        WriteGeneratedSprite("LabWallVertical_Clean.png", CreateWallTexture(horizontal: false));
        WriteGeneratedSprite("LabDoorHorizontal_Clean.png", CreateDoorTexture(horizontal: true));
        WriteGeneratedSprite("LabDoorVertical_Clean.png", CreateDoorTexture(horizontal: false));
        WriteGeneratedSprite("LabFloorGrate_Clean.png", CreateFloorGrateTexture());
        WriteGeneratedSprite("LabHazardStripe_Clean.png", CreateHazardStripeTexture());
        WriteGeneratedSprite("HudPanel_Clean.png", CreateHudPanelTexture());

        AssetDatabase.Refresh();

        foreach (string guid in AssetDatabase.FindAssets("t:Texture2D", new[] { GeneratedRecoveryArtRoot }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

            if (importer == null)
            {
                continue;
            }

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

            if (!Mathf.Approximately(importer.spritePixelsPerUnit, GeneratedSpritePixelsPerUnit))
            {
                importer.spritePixelsPerUnit = GeneratedSpritePixelsPerUnit;
                changed = true;
            }

            if (!importer.alphaIsTransparency)
            {
                importer.alphaIsTransparency = true;
                changed = true;
            }

            if (importer.mipmapEnabled)
            {
                importer.mipmapEnabled = false;
                changed = true;
            }

            if (importer.filterMode != FilterMode.Bilinear)
            {
                importer.filterMode = FilterMode.Bilinear;
                changed = true;
            }

            if (importer.textureCompression != TextureImporterCompression.Uncompressed)
            {
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                changed = true;
            }

            if (changed)
            {
                importer.SaveAndReimport();
            }
        }
    }

    private static void WriteGeneratedSprite(string fileName, Texture2D texture)
    {
        string path = $"{GeneratedRecoveryArtRoot}/{fileName}";
        byte[] bytes = texture.EncodeToPNG();
        Object.DestroyImmediate(texture);

        if (File.Exists(path))
        {
            byte[] existingBytes = File.ReadAllBytes(path);

            if (BytesMatch(existingBytes, bytes))
            {
                return;
            }
        }

        File.WriteAllBytes(path, bytes);
    }

    private static bool BytesMatch(byte[] first, byte[] second)
    {
        if (first == null || second == null || first.Length != second.Length)
        {
            return false;
        }

        for (int i = 0; i < first.Length; i++)
        {
            if (first[i] != second[i])
            {
                return false;
            }
        }

        return true;
    }

    private static Texture2D CreateFloorTileTexture()
    {
        Texture2D texture = CreateTexture(256, 256, new Color32(28, 39, 43, 255));

        for (int y = 0; y < 256; y += 64)
        {
            FillRect(texture, 0, y, 256, 2, new Color32(55, 71, 76, 210));
        }

        for (int x = 0; x < 256; x += 64)
        {
            FillRect(texture, x, 0, 2, 256, new Color32(55, 71, 76, 210));
        }

        DrawRect(texture, 10, 10, 236, 236, new Color32(41, 58, 63, 215));
        DrawRect(texture, 38, 38, 180, 180, new Color32(47, 63, 68, 160));
        FillRect(texture, 104, 126, 48, 4, new Color32(34, 198, 214, 190));
        FillRect(texture, 110, 122, 36, 12, new Color32(22, 92, 101, 90));
        AddFineNoise(texture, 10);
        texture.Apply();
        return texture;
    }

    private static Texture2D CreateWallTexture(bool horizontal)
    {
        int width = horizontal ? 256 : 64;
        int height = horizontal ? 64 : 256;
        Texture2D texture = CreateTexture(width, height, new Color32(35, 43, 49, 255));

        FillRect(texture, 0, 0, width, height, new Color32(31, 39, 45, 255));
        DrawRect(texture, 1, 1, width - 2, height - 2, new Color32(78, 91, 97, 255));
        DrawRect(texture, 5, 5, width - 10, height - 10, new Color32(18, 25, 30, 220));

        if (horizontal)
        {
            FillRect(texture, 18, 27, 44, 10, new Color32(45, 62, 68, 230));
            FillRect(texture, 106, 27, 44, 10, new Color32(31, 187, 203, 210));
            FillRect(texture, 194, 27, 44, 10, new Color32(45, 62, 68, 230));

            for (int x = 48; x < width; x += 64)
            {
                FillRect(texture, x, 8, 3, height - 16, new Color32(82, 96, 102, 210));
            }
        }
        else
        {
            FillRect(texture, 27, 18, 10, 44, new Color32(45, 62, 68, 230));
            FillRect(texture, 27, 106, 10, 44, new Color32(31, 187, 203, 210));
            FillRect(texture, 27, 194, 10, 44, new Color32(45, 62, 68, 230));

            for (int y = 48; y < height; y += 64)
            {
                FillRect(texture, 8, y, width - 16, 3, new Color32(82, 96, 102, 210));
            }
        }

        AddFineNoise(texture, 8);
        texture.Apply();
        return texture;
    }

    private static Texture2D CreateDoorTexture(bool horizontal)
    {
        int width = horizontal ? 256 : 72;
        int height = horizontal ? 72 : 256;
        Texture2D texture = CreateTexture(width, height, new Color32(0, 0, 0, 0));

        FillRect(texture, 0, 0, width, height, new Color32(38, 51, 58, 235));
        DrawRect(texture, 3, 3, width - 6, height - 6, new Color32(104, 125, 132, 240));
        DrawRect(texture, 10, 10, width - 20, height - 20, new Color32(13, 22, 27, 230));

        if (horizontal)
        {
            FillRect(texture, width / 2 - 2, 11, 4, height - 22, new Color32(83, 95, 101, 230));
            FillRect(texture, width / 2 - 34, height / 2 - 4, 68, 8, new Color32(40, 212, 230, 230));
        }
        else
        {
            FillRect(texture, 11, height / 2 - 2, width - 22, 4, new Color32(83, 95, 101, 230));
            FillRect(texture, width / 2 - 4, height / 2 - 34, 8, 68, new Color32(40, 212, 230, 230));
        }

        texture.Apply();
        return texture;
    }

    private static Texture2D CreateFloorGrateTexture()
    {
        Texture2D texture = CreateTexture(128, 96, new Color32(0, 0, 0, 0));
        FillRect(texture, 6, 6, 116, 84, new Color32(12, 18, 20, 210));
        DrawRect(texture, 6, 6, 116, 84, new Color32(84, 100, 106, 230));

        for (int y = 16; y < 82; y += 10)
        {
            FillRect(texture, 18, y, 92, 3, new Color32(64, 76, 82, 220));
        }

        texture.Apply();
        return texture;
    }

    private static Texture2D CreateHazardStripeTexture()
    {
        Texture2D texture = CreateTexture(128, 48, new Color32(0, 0, 0, 0));
        FillRect(texture, 0, 0, 128, 48, new Color32(12, 15, 16, 205));

        for (int x = -48; x < 128; x += 24)
        {
            for (int i = 0; i < 16; i++)
            {
                FillRect(texture, x + i * 2, i, 12, 2, new Color32(214, 164, 35, 230));
                FillRect(texture, x + i * 2 + 32, 32 + i, 12, 2, new Color32(214, 164, 35, 230));
            }
        }

        DrawRect(texture, 1, 1, 126, 46, new Color32(80, 92, 94, 220));
        texture.Apply();
        return texture;
    }

    private static Texture2D CreateHudPanelTexture()
    {
        Texture2D texture = CreateTexture(256, 96, new Color32(0, 0, 0, 0));
        FillRect(texture, 0, 0, 256, 96, new Color32(6, 21, 27, 190));
        DrawRect(texture, 2, 2, 252, 92, new Color32(26, 226, 237, 210));
        DrawRect(texture, 8, 8, 240, 80, new Color32(12, 82, 91, 150));
        FillRect(texture, 18, 77, 220, 2, new Color32(40, 228, 239, 120));
        texture.Apply();
        return texture;
    }

    private static Texture2D CreateTexture(int width, int height, Color32 fill)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color32[] pixels = new Color32[width * height];

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = fill;
        }

        texture.SetPixels32(pixels);
        return texture;
    }

    private static void FillRect(Texture2D texture, int x, int y, int width, int height, Color32 color)
    {
        int minX = Mathf.Clamp(x, 0, texture.width);
        int minY = Mathf.Clamp(y, 0, texture.height);
        int maxX = Mathf.Clamp(x + width, 0, texture.width);
        int maxY = Mathf.Clamp(y + height, 0, texture.height);

        for (int py = minY; py < maxY; py++)
        {
            for (int px = minX; px < maxX; px++)
            {
                texture.SetPixel(px, py, color);
            }
        }
    }

    private static void DrawRect(Texture2D texture, int x, int y, int width, int height, Color32 color)
    {
        FillRect(texture, x, y, width, 2, color);
        FillRect(texture, x, y + height - 2, width, 2, color);
        FillRect(texture, x, y, 2, height, color);
        FillRect(texture, x + width - 2, y, 2, height, color);
    }

    private static void AddFineNoise(Texture2D texture, byte range)
    {
        for (int y = 0; y < texture.height; y += 7)
        {
            for (int x = 0; x < texture.width; x += 7)
            {
                Color pixel = texture.GetPixel(x, y);
                float offset = ((x * 17 + y * 31) % range) / 255f;
                pixel.r = Mathf.Clamp01(pixel.r + offset);
                pixel.g = Mathf.Clamp01(pixel.g + offset);
                pixel.b = Mathf.Clamp01(pixel.b + offset);
                texture.SetPixel(x, y, pixel);
            }
        }
    }

    private static Vector2 ClampDecorVisualSize(EnvironmentKind kind, Vector2 size)
    {
        if (kind == EnvironmentKind.Cover)
        {
            return new Vector2(Mathf.Clamp(size.x, 0.45f, 1.25f), Mathf.Clamp(size.y, 0.35f, 1.0f));
        }

        return new Vector2(Mathf.Clamp(size.x, 0.35f, 1.15f), Mathf.Clamp(size.y, 0.35f, 1.15f));
    }

    private static bool ApplyHudPanel(GameObject panelObject, Sprite panelSprite, string lowerName)
    {
        bool changed = false;
        Image image = panelObject.GetComponent<Image>();

        if (image == null)
        {
            image = panelObject.AddComponent<Image>();
            changed = true;
        }

        Color targetColor = lowerName.Contains("health")
            ? new Color(0.02f, 0.12f, 0.11f, 0.78f)
            : new Color(0.015f, 0.045f, 0.055f, 0.76f);

        if (image.sprite != panelSprite)
        {
            image.sprite = panelSprite;
            changed = true;
        }

        if (image.type != Image.Type.Sliced)
        {
            image.type = Image.Type.Sliced;
            changed = true;
        }

        if (image.color != targetColor)
        {
            image.color = targetColor;
            changed = true;
        }

        if (image.raycastTarget)
        {
            image.raycastTarget = false;
            changed = true;
        }

        RectTransform rectTransform = panelObject.GetComponent<RectTransform>();

        if (rectTransform != null)
        {
            Vector2 targetSize = GetHudPanelSize(panelObject.name, rectTransform.sizeDelta);

            if (targetSize.x > 0f && targetSize.y > 0f && rectTransform.sizeDelta != targetSize)
            {
                rectTransform.sizeDelta = targetSize;
                changed = true;
            }
        }

        if (changed)
        {
            EditorUtility.SetDirty(panelObject);
        }

        return changed;
    }

    private static Vector2 GetHudPanelSize(string name, Vector2 currentSize)
    {
        string lower = name.ToLowerInvariant();

        if (lower.Contains("resource"))
        {
            return new Vector2(330f, 136f);
        }

        if (lower.Contains("health"))
        {
            return new Vector2(Mathf.Clamp(currentSize.x, 180f, 260f), Mathf.Clamp(currentSize.y, 34f, 58f));
        }

        if (lower.Contains("objective"))
        {
            return new Vector2(430f, 66f);
        }

        if (currentSize.x > 560f || currentSize.y > 260f)
        {
            return new Vector2(Mathf.Min(currentSize.x, 520f), Mathf.Min(currentSize.y, 220f));
        }

        return currentSize;
    }

    private static bool StyleHudText(Text text)
    {
        if (text == null || text.GetComponentInParent<Canvas>() == null)
        {
            return false;
        }

        bool changed = false;
        Color targetColor = new Color(0.9f, 1f, 1f, 1f);

        if (text.color != targetColor)
        {
            text.color = targetColor;
            changed = true;
        }

        int targetSize = Mathf.Clamp(text.fontSize, 13, 22);

        if (text.fontSize != targetSize)
        {
            text.fontSize = targetSize;
            changed = true;
        }

        if (text.horizontalOverflow != HorizontalWrapMode.Overflow)
        {
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            changed = true;
        }

        if (text.verticalOverflow != VerticalWrapMode.Overflow)
        {
            text.verticalOverflow = VerticalWrapMode.Overflow;
            changed = true;
        }

        if (changed)
        {
            EditorUtility.SetDirty(text);
        }

        return changed;
    }

    private static bool RepairActorVisualScale(GameObject owner, float targetWorldHeight, int sortingOrder)
    {
        SpriteRenderer renderer = FindPrimaryVisualRenderer(owner);

        if (renderer == null || renderer.sprite == null)
        {
            return false;
        }

        return ApplySimpleFittedVisual(renderer, renderer.sprite, new Vector2(targetWorldHeight, targetWorldHeight), renderer.transform.localPosition, sortingOrder, 1f);
    }

    private static bool RepairPickupVisualScale(GameObject owner)
    {
        SpriteRenderer renderer = FindPrimaryVisualRenderer(owner);

        if (renderer == null || renderer.sprite == null)
        {
            return false;
        }

        return ApplySimpleFittedVisual(renderer, renderer.sprite, new Vector2(0.46f, 0.46f), renderer.transform.localPosition, 10, 1f);
    }

    private static bool RepairHazardVisualScale(GameObject owner)
    {
        SpriteRenderer renderer = FindPrimaryVisualRenderer(owner);

        if (renderer == null || renderer.sprite == null)
        {
            return false;
        }

        if (!TryGetLocalFootprint(owner, owner.GetComponent<SpriteRenderer>(), renderer, out Vector2 footprint, out Vector2 offset))
        {
            footprint = new Vector2(1.2f, 1.2f);
            offset = renderer.transform.localPosition;
        }

        footprint = new Vector2(Mathf.Min(footprint.x, 1.8f), Mathf.Min(footprint.y, 1.8f));
        return ApplySimpleFittedVisual(renderer, renderer.sprite, footprint, offset, 6, 1f);
    }

    private static bool EnsureShadow(GameObject owner, Sprite shadowSprite, Vector2 targetSize, int sortingOrder)
    {
        if (shadowSprite == null)
        {
            return false;
        }

        bool changed = false;
        SpriteRenderer shadow = FindOrCreateVisualRenderer(owner, "Shadow", ref changed);
        changed |= ApplySimpleFittedVisual(shadow, shadowSprite, targetSize, new Vector2(0f, -0.12f), sortingOrder, 1f);

        if (changed)
        {
            shadow.gameObject.name = "Shadow";
        }

        return changed;
    }

    private static bool ApplySizedVisual(
        SpriteRenderer renderer,
        Sprite sprite,
        SpriteDrawMode drawMode,
        Vector2 size,
        Vector2 localPosition,
        int sortingOrder,
        Color color)
    {
        bool changed = false;

        if (renderer.sprite != sprite)
        {
            renderer.sprite = sprite;
            changed = true;
        }

        if (renderer.drawMode != drawMode)
        {
            renderer.drawMode = drawMode;
            changed = true;
        }

        if (renderer.size != size)
        {
            renderer.size = size;
            changed = true;
        }

        if (renderer.transform.localPosition != (Vector3)localPosition)
        {
            renderer.transform.localPosition = localPosition;
            changed = true;
        }

        if (renderer.transform.localRotation != Quaternion.identity)
        {
            renderer.transform.localRotation = Quaternion.identity;
            changed = true;
        }

        if (renderer.transform.localScale != Vector3.one)
        {
            renderer.transform.localScale = Vector3.one;
            changed = true;
        }

        if (renderer.sortingOrder != sortingOrder)
        {
            renderer.sortingOrder = sortingOrder;
            changed = true;
        }

        if (renderer.color != color)
        {
            renderer.color = color;
            changed = true;
        }

        if (!renderer.enabled)
        {
            renderer.enabled = true;
            changed = true;
        }

        if (!renderer.gameObject.activeSelf)
        {
            renderer.gameObject.SetActive(true);
            changed = true;
        }

        if (changed)
        {
            EditorUtility.SetDirty(renderer);
            EditorUtility.SetDirty(renderer.transform);
        }

        return changed;
    }

    private static bool ApplySimpleFittedVisual(
        SpriteRenderer renderer,
        Sprite sprite,
        Vector2 targetSize,
        Vector2 localPosition,
        int sortingOrder,
        float fillPercent)
    {
        bool changed = false;

        if (renderer.sprite != sprite)
        {
            renderer.sprite = sprite;
            changed = true;
        }

        if (renderer.drawMode != SpriteDrawMode.Simple)
        {
            renderer.drawMode = SpriteDrawMode.Simple;
            changed = true;
        }

        Vector2 spriteSize = new Vector2(sprite.bounds.size.x, sprite.bounds.size.y);
        float scale = Mathf.Min(targetSize.x / Mathf.Max(0.001f, spriteSize.x), targetSize.y / Mathf.Max(0.001f, spriteSize.y)) * fillPercent;
        Vector3 targetScale = new Vector3(scale, scale, 1f);

        if (renderer.transform.localScale != targetScale)
        {
            renderer.transform.localScale = targetScale;
            changed = true;
        }

        if (renderer.transform.localPosition != (Vector3)localPosition)
        {
            renderer.transform.localPosition = localPosition;
            changed = true;
        }

        if (renderer.transform.localRotation != Quaternion.identity)
        {
            renderer.transform.localRotation = Quaternion.identity;
            changed = true;
        }

        if (renderer.sortingOrder != sortingOrder)
        {
            renderer.sortingOrder = sortingOrder;
            changed = true;
        }

        if (renderer.color != Color.white)
        {
            renderer.color = Color.white;
            changed = true;
        }

        if (!renderer.enabled)
        {
            renderer.enabled = true;
            changed = true;
        }

        if (changed)
        {
            EditorUtility.SetDirty(renderer);
            EditorUtility.SetDirty(renderer.transform);
        }

        return changed;
    }

    private static bool DisableBrokenExtraVisuals(GameObject owner, SpriteRenderer keepRenderer, Vector2 targetSize)
    {
        Transform visualRoot = owner.transform.Find("Visual");

        if (visualRoot == null)
        {
            return false;
        }

        bool changed = false;

        foreach (SpriteRenderer renderer in visualRoot.GetComponentsInChildren<SpriteRenderer>(true))
        {
            if (renderer == keepRenderer || renderer.name == "Shadow")
            {
                continue;
            }

            if (!renderer.enabled)
            {
                continue;
            }

            Vector2 footprint = GetRendererFootprint(renderer);

            if (footprint.x > targetSize.x * 2.5f || footprint.y > targetSize.y * 2.5f || footprint.x < 0.05f || footprint.y < 0.05f)
            {
                renderer.enabled = false;
                EditorUtility.SetDirty(renderer);
                changed = true;
            }
        }

        return changed;
    }

    private static bool TryGetLocalFootprint(GameObject owner, SpriteRenderer rootRenderer, SpriteRenderer visualRenderer, out Vector2 size, out Vector2 offset)
    {
        BoxCollider2D box = owner.GetComponent<BoxCollider2D>();

        if (box != null && box.size.x > 0.01f && box.size.y > 0.01f)
        {
            size = Abs(box.size);
            offset = box.offset;
            return true;
        }

        Collider2D collider = owner.GetComponent<Collider2D>();

        if (collider != null && collider.bounds.size.x > 0.01f && collider.bounds.size.y > 0.01f)
        {
            Vector3 localSize = owner.transform.InverseTransformVector(collider.bounds.size);
            Vector3 localCenter = owner.transform.InverseTransformPoint(collider.bounds.center);
            size = Abs(localSize);
            offset = new Vector2(localCenter.x, localCenter.y);
            return true;
        }

        if (TryGetRendererSize(rootRenderer, out size))
        {
            offset = Vector2.zero;
            return true;
        }

        if (TryGetRendererSize(visualRenderer, out size))
        {
            offset = visualRenderer != null ? (Vector2)visualRenderer.transform.localPosition : Vector2.zero;
            return true;
        }

        size = Vector2.zero;
        offset = Vector2.zero;
        return false;
    }

    private static bool TryGetRendererSize(SpriteRenderer renderer, out Vector2 size)
    {
        if (renderer == null || renderer.sprite == null)
        {
            size = Vector2.zero;
            return false;
        }

        size = renderer.drawMode == SpriteDrawMode.Simple
            ? new Vector2(renderer.sprite.bounds.size.x, renderer.sprite.bounds.size.y)
            : renderer.size;

        return size.x > 0.01f && size.y > 0.01f;
    }

    private static bool IsUsableVisual(SpriteRenderer renderer, Vector2 targetSize)
    {
        if (renderer == null || renderer.sprite == null || !renderer.enabled || !renderer.gameObject.activeSelf)
        {
            return false;
        }

        Vector2 footprint = GetRendererFootprint(renderer);
        return footprint.x > 0.05f &&
               footprint.y > 0.05f &&
               footprint.x >= targetSize.x * 0.45f &&
               footprint.y >= targetSize.y * 0.45f;
    }

    private static Vector2 GetRendererFootprint(SpriteRenderer renderer)
    {
        if (renderer == null || renderer.sprite == null)
        {
            return Vector2.zero;
        }

        Vector2 size = renderer.drawMode == SpriteDrawMode.Simple
            ? new Vector2(renderer.sprite.bounds.size.x, renderer.sprite.bounds.size.y)
            : renderer.size;

        return Vector2.Scale(size, Abs(renderer.transform.localScale));
    }

    private static SpriteRenderer FindOrCreateVisualRenderer(GameObject owner, string rendererName, ref bool changed)
    {
        Transform visualRoot = owner.transform.Find("Visual");

        if (visualRoot == null)
        {
            GameObject visualObject = new GameObject("Visual");
            visualObject.transform.SetParent(owner.transform, false);
            visualRoot = visualObject.transform;
            changed = true;
        }

        Transform rendererTransform = visualRoot.Find(rendererName);

        if (rendererTransform == null)
        {
            GameObject rendererObject = new GameObject(rendererName);
            rendererObject.transform.SetParent(visualRoot, false);
            rendererTransform = rendererObject.transform;
            changed = true;
        }

        SpriteRenderer renderer = rendererTransform.GetComponent<SpriteRenderer>();

        if (renderer == null)
        {
            renderer = rendererTransform.gameObject.AddComponent<SpriteRenderer>();
            changed = true;
        }

        return renderer;
    }

    private static SpriteRenderer FindPrimaryVisualRenderer(GameObject owner)
    {
        Transform visualRoot = owner.transform.Find("Visual");

        if (visualRoot != null)
        {
            foreach (SpriteRenderer renderer in visualRoot.GetComponentsInChildren<SpriteRenderer>(true))
            {
                if (renderer.name != "Shadow" && renderer.sprite != null)
                {
                    return renderer;
                }
            }
        }

        return owner.GetComponent<SpriteRenderer>();
    }

    private static SpriteRenderer CreateRenderer(Transform parent, string name, Sprite sprite, Vector3 worldPosition)
    {
        GameObject child = new GameObject(name);
        child.transform.SetParent(parent, false);
        child.transform.position = worldPosition;
        SpriteRenderer renderer = child.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        return renderer;
    }

    private static void AddTiledDetail(Transform parent, string name, Sprite sprite, Vector3 position, Vector2 size, int sortingOrder, RecoveryStats stats)
    {
        if (sprite == null)
        {
            return;
        }

        SpriteRenderer renderer = CreateRenderer(parent, name, sprite, position);
        ApplySizedVisual(renderer, sprite, SpriteDrawMode.Tiled, size, Vector2.zero, sortingOrder, new Color(0.85f, 0.95f, 1f, 0.9f));
        stats.DecorativeVisualsAdded++;
    }

    private static void AddSimpleDetail(Transform parent, string name, Sprite sprite, Vector3 position, float scale, int sortingOrder, RecoveryStats stats)
    {
        if (sprite == null)
        {
            return;
        }

        SpriteRenderer renderer = CreateRenderer(parent, name, sprite, position);
        renderer.drawMode = SpriteDrawMode.Simple;
        renderer.transform.localScale = new Vector3(scale, scale, 1f);
        renderer.sortingOrder = sortingOrder;
        renderer.color = Color.white;
        EditorUtility.SetDirty(renderer);
        stats.DecorativeVisualsAdded++;
    }

    private static Bounds CalculateLevelBounds(Scene scene)
    {
        bool hasBounds = false;
        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (Collider2D collider in root.GetComponentsInChildren<Collider2D>(true))
            {
                if (collider == null || collider.isTrigger || IsUnderCanvas(collider.transform))
                {
                    continue;
                }

                if (!hasBounds)
                {
                    bounds = collider.bounds;
                    hasBounds = true;
                }
                else
                {
                    bounds.Encapsulate(collider.bounds);
                }
            }
        }

        if (!hasBounds)
        {
            bounds = new Bounds(Vector3.zero, new Vector3(18f, 10f, 0f));
        }

        return bounds;
    }

    private static GameObject EnsureSceneRoot(Scene scene, string name)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            if (root.name == name)
            {
                return root;
            }
        }

        GameObject created = new GameObject(name);
        SceneManager.MoveGameObjectToScene(created, scene);
        return created;
    }

    private static void ClearChildren(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Object.DestroyImmediate(parent.GetChild(i).gameObject);
        }
    }

    private static EnvironmentKind ClassifyEnvironment(GameObject owner)
    {
        if (owner == null || owner.GetComponentInParent<Canvas>() != null)
        {
            return EnvironmentKind.None;
        }

        bool hasSurface = owner.GetComponent<Collider2D>() != null ||
                          owner.GetComponent<SpriteRenderer>() != null ||
                          owner.transform.Find("Visual") != null;

        if (!hasSurface)
        {
            return EnvironmentKind.None;
        }

        string lowerName = owner.name.ToLowerInvariant();

        if (owner.GetComponent<DoorController2D>() != null || owner.GetComponent<LockedGate2D>() != null || lowerName.Contains("door") || lowerName.Contains("gate"))
        {
            return EnvironmentKind.Door;
        }

        if (lowerName.Contains("wall"))
        {
            return EnvironmentKind.Wall;
        }

        if (lowerName.Contains("floor"))
        {
            return EnvironmentKind.Floor;
        }

        if (lowerName.Contains("cover") || lowerName.Contains("crate"))
        {
            return EnvironmentKind.Cover;
        }

        if (lowerName.Contains("terminal") || lowerName.Contains("shop") || lowerName.Contains("kiosk") || lowerName.Contains("barrel") || lowerName.Contains("pillar"))
        {
            return EnvironmentKind.Prop;
        }

        return EnvironmentKind.None;
    }

    private static Sprite GetEnvironmentSprite(EnvironmentKind kind, bool horizontal, string objectName, ArtLibrary art)
    {
        string lowerName = objectName.ToLowerInvariant();

        switch (kind)
        {
            case EnvironmentKind.Wall:
                return horizontal ? art.WallHorizontal : art.WallVertical;
            case EnvironmentKind.Floor:
                return art.FloorTile;
            case EnvironmentKind.Door:
                return horizontal ? art.DoorHorizontal : art.DoorVertical;
            case EnvironmentKind.Cover:
                return lowerName.Contains("tall") ? art.TallCover : art.LowCover;
            case EnvironmentKind.Prop:
                if (lowerName.Contains("terminal"))
                {
                    return lowerName.Contains("objective") ? art.ObjectiveTerminal : art.WallTerminal;
                }

                if (lowerName.Contains("shop") || lowerName.Contains("kiosk"))
                {
                    return art.ShopKiosk;
                }

                if (lowerName.Contains("barrel"))
                {
                    return art.ExplosiveBarrel;
                }

                if (lowerName.Contains("pillar"))
                {
                    return art.MetalPillar;
                }

                return art.SmallCrate;
            default:
                return null;
        }
    }

    private static int GetSortingOrder(EnvironmentKind kind, SpriteRenderer rootRenderer)
    {
        if (rootRenderer != null && rootRenderer.sortingOrder != 0)
        {
            return rootRenderer.sortingOrder;
        }

        switch (kind)
        {
            case EnvironmentKind.Floor:
                return -20;
            case EnvironmentKind.Wall:
                return 1;
            case EnvironmentKind.Cover:
            case EnvironmentKind.Prop:
                return 4;
            case EnvironmentKind.Door:
                return 7;
            default:
                return 0;
        }
    }

    private static string GetRendererName(EnvironmentKind kind)
    {
        switch (kind)
        {
            case EnvironmentKind.Wall:
                return "WallSpriteRenderer";
            case EnvironmentKind.Floor:
                return "FloorSpriteRenderer";
            case EnvironmentKind.Door:
                return "DoorSpriteRenderer";
            case EnvironmentKind.Cover:
                return "CoverSpriteRenderer";
            case EnvironmentKind.Prop:
                return "PropSpriteRenderer";
            default:
                return "EnvironmentSpriteRenderer";
        }
    }

    private static Sprite ChooseHudPanelSprite(string lowerName, ArtLibrary art)
    {
        if (lowerName.Contains("health"))
        {
            return art.HealthPanel;
        }

        if (lowerName.Contains("boss"))
        {
            return art.BossHealthFrame;
        }

        if (lowerName.Contains("resource") || lowerName.Contains("weapon") || lowerName.Contains("buff") || lowerName.Contains("keycard"))
        {
            return art.ResourcePanel;
        }

        return art.DarkPanel;
    }

    private static bool IsPickup(GameObject owner)
    {
        return owner.GetComponent<ResourcePickup2D>() != null ||
               owner.GetComponent<KeycardPickup2D>() != null ||
               owner.GetComponent<RewardPickup2D>() != null ||
               owner.GetComponent<WeaponPickup2D>() != null;
    }

    private static bool IsHazard(GameObject owner)
    {
        string lowerName = owner.name.ToLowerInvariant();
        return owner.GetComponent<SecurityLaserHazard2D>() != null ||
               lowerName.Contains("toxic") ||
               lowerName.Contains("hazard") ||
               lowerName.Contains("electric") ||
               lowerName.Contains("puddle");
    }

    private static bool EnsureRootVisible(SpriteRenderer rootRenderer)
    {
        if (rootRenderer == null || rootRenderer.enabled || rootRenderer.sprite == null)
        {
            return false;
        }

        rootRenderer.enabled = true;
        EditorUtility.SetDirty(rootRenderer);
        return true;
    }

    private static bool IsVisualDescendant(Transform transform)
    {
        Transform current = transform.parent;

        while (current != null)
        {
            if (current.name == "Visual" || current.name == "OpenVisual")
            {
                return true;
            }

            current = current.parent;
        }

        return false;
    }

    private static bool IsUnderCanvas(Transform transform)
    {
        return transform.GetComponentInParent<Canvas>() != null;
    }

    private static bool IsUnderRecoveryRoot(Transform transform)
    {
        Transform current = transform;

        while (current != null)
        {
            if (current.name == RecoveryRootName)
            {
                return true;
            }

            current = current.parent;
        }

        return false;
    }

    private static IEnumerable<T> FindSceneComponents<T>(Scene scene) where T : Component
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (T component in root.GetComponentsInChildren<T>(true))
            {
                yield return component;
            }
        }
    }

    private static Vector2 Abs(Vector2 vector)
    {
        return new Vector2(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
    }

    private static Vector2 Abs(Vector3 vector)
    {
        return new Vector2(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
    }

    private static string GetHierarchyPath(Transform transform)
    {
        List<string> names = new List<string>();
        Transform current = transform;

        while (current != null)
        {
            names.Add(current.name);
            current = current.parent;
        }

        names.Reverse();
        return string.Join("/", names);
    }

    private static void WriteDocumentation(RecoveryStats stats)
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendLine("# Cod: Evadare - Level 01 Visual Recovery");
        builder.AppendLine();
        builder.AppendLine("Date: 2026-06-22");
        builder.AppendLine();
        builder.AppendLine("## Scope");
        builder.AppendLine();
        builder.AppendLine("Focused visual recovery for `Assets/_Project/Scenes/Levels/Level_01_Laboratory.unity` only. Gameplay logic, stats, colliders, triggers, tags, layers and prefab roots were not intentionally changed.");
        builder.AppendLine();
        builder.AppendLine("## Files Changed");
        builder.AppendLine();
        builder.AppendLine("- `Assets/_Project/Scripts/Editor/CodEvadareLevel01VisualRecovery.cs`");
        builder.AppendLine("- `Assets/_Project/Docs/VISUAL_RECOVERY_LEVEL01.md`");
        builder.AppendLine("- generated sprites under `Assets/_Project/Art/Sprites/RuntimeTransparent/VisualRecovery` after running `Tools/Cod Evadare/Art/Recovery/Recover Level 01 Laboratory Visuals`");
        builder.AppendLine("- `Assets/_Project/Scenes/Levels/Level_01_Laboratory.unity` after running `Tools/Cod Evadare/Art/Recovery/Recover Level 01 Laboratory Visuals`");
        builder.AppendLine();
        builder.AppendLine("The latest recovery pass generates small clean sci-fi floor, wall, door, grate, hazard stripe and HUD panel sprites under `RuntimeTransparent/VisualRecovery`. These are used for gameplay footprint visuals because the larger art-pack room panel sprites are too large to tile directly as walls or floors.");
        builder.AppendLine();
        builder.AppendLine("## Scene Objects Changed");
        builder.AppendLine();
        builder.AppendLine($"- Environment objects repaired: {stats.EnvironmentObjectsRepaired}");
        builder.AppendLine($"- Decorative floor/prop visuals added under `{RecoveryRootName}`: {stats.DecorativeVisualsAdded}");
        builder.AppendLine($"- Shadows repaired/added: {stats.ShadowsRepaired}");
        builder.AppendLine($"- HUD panels repaired: {stats.HudPanelsRepaired}");
        builder.AppendLine();

        if (stats.SceneObjectsChanged.Count > 0)
        {
            builder.AppendLine("Representative changed scene objects:");

            int limit = Mathf.Min(stats.SceneObjectsChanged.Count, 80);

            for (int i = 0; i < limit; i++)
            {
                builder.AppendLine($"- `{stats.SceneObjectsChanged[i]}`");
            }

            if (stats.SceneObjectsChanged.Count > limit)
            {
                builder.AppendLine($"- ...and {stats.SceneObjectsChanged.Count - limit} more visual-only scene object changes.");
            }

            builder.AppendLine();
        }

        builder.AppendLine("## Prefabs Changed");
        builder.AppendLine();
        builder.AppendLine("None. This recovery pass is scene-focused and does not replace or edit prefab roots.");
        builder.AppendLine();
        builder.AppendLine("## Remaining Visual Issues");
        builder.AppendLine();
        builder.AppendLine("- The other campaign levels still need separate visual passes after Level 01 is approved.");
        builder.AppendLine("- Main Menu and Level Select were intentionally not redesigned in this pass.");
        builder.AppendLine("- HUD is only made readable, not fully redesigned.");
        builder.AppendLine("- Lighting and post-processing are still future work.");
        builder.AppendLine("- Runtime-spawned enemy visuals depend on their existing prefabs; this pass does not edit those prefabs.");
        builder.AppendLine();
        builder.AppendLine("## Play Mode Test Checklist");
        builder.AppendLine();
        builder.AppendLine("1. Open `Assets/_Project/Scenes/Levels/Level_01_Laboratory.unity`.");
        builder.AppendLine("2. Confirm the floor is visible and no longer a black void.");
        builder.AppendLine("3. Confirm walls are visible, aligned with room/corridor boundaries and not clipped in huge corner chunks.");
        builder.AppendLine("4. Confirm doors and locked gates line up with wall openings.");
        builder.AppendLine("5. Press Play.");
        builder.AppendLine("6. Move with WASD / arrow keys.");
        builder.AppendLine("7. Aim with the mouse.");
        builder.AppendLine("8. Shoot with left click.");
        builder.AppendLine("9. Reload with R while alive.");
        builder.AppendLine("10. Verify enemies, pickups, hazards, shop, buffs and keycards still work.");
        builder.AppendLine("11. Verify HUD text remains dynamic and readable: HP, ammo, reserve ammo, money, weapon, buffs and keycards.");
        builder.AppendLine("12. Check Console for missing reference, missing sprite or compile errors.");

        File.WriteAllText(DocumentationPath, builder.ToString());
        AssetDatabase.ImportAsset(DocumentationPath);
    }

    private enum EnvironmentKind
    {
        None,
        Wall,
        Floor,
        Door,
        Cover,
        Prop
    }

    private sealed class RecoveryStats
    {
        public int EnvironmentObjectsRepaired;
        public int DecorativeVisualsAdded;
        public int ShadowsRepaired;
        public int HudPanelsRepaired;
        public readonly List<string> SceneObjectsChanged = new List<string>();
    }

    private sealed class ArtLibrary
    {
        public Sprite WallHorizontal;
        public Sprite WallVertical;
        public Sprite FloorTile;
        public Sprite FloorGrate;
        public Sprite HazardStripe;
        public Sprite CrackDecal;
        public Sprite BiohazardDecal;
        public Sprite DoorHorizontal;
        public Sprite DoorVertical;
        public Sprite LowCover;
        public Sprite TallCover;
        public Sprite SmallCrate;
        public Sprite WallTerminal;
        public Sprite ObjectiveTerminal;
        public Sprite ShopKiosk;
        public Sprite ExplosiveBarrel;
        public Sprite MetalPillar;
        public Sprite CyanWallLight;
        public Sprite PlayerShadow;
        public Sprite RoomShadow;
        public Sprite HealthPanel;
        public Sprite ResourcePanel;
        public Sprite DarkPanel;
        public Sprite BossHealthFrame;

        public static ArtLibrary Load()
        {
            return new ArtLibrary
            {
                WallHorizontal = LoadGenerated("LabWallHorizontal_Clean.png"),
                WallVertical = LoadGenerated("LabWallVertical_Clean.png"),
                FloorTile = LoadGenerated("LabFloorTile_Clean.png"),
                FloorGrate = LoadGenerated("LabFloorGrate_Clean.png"),
                HazardStripe = LoadGenerated("LabHazardStripe_Clean.png"),
                CrackDecal = Load("Tiles/Decal_CrackMetal.png"),
                BiohazardDecal = Load("Tiles/Decal_Biohazard.png"),
                DoorHorizontal = LoadGenerated("LabDoorHorizontal_Clean.png"),
                DoorVertical = LoadGenerated("LabDoorVertical_Clean.png"),
                LowCover = Load("Props/Prop_LowCoverBlock.png"),
                TallCover = Load("Props/Prop_TallCoverCrate.png"),
                SmallCrate = Load("Tiles/Prop_SciFiCrateSmall.png"),
                WallTerminal = Load("Props/Prop_WallTerminal.png"),
                ObjectiveTerminal = Load("Props/Prop_ObjectiveTerminal.png"),
                ShopKiosk = Load("Props/Prop_ShopKiosk.png"),
                ExplosiveBarrel = Load("Props/Prop_ExplosiveBarrel.png"),
                MetalPillar = Load("Tiles/Tile_MetalPillar.png"),
                CyanWallLight = Load("Tiles/Light_NeonCyanWall.png"),
                PlayerShadow = Load("Player/Player_Shadow.png"),
                RoomShadow = Load("Tiles/Shadow_RoomBlob.png"),
                HealthPanel = LoadGenerated("HudPanel_Clean.png"),
                ResourcePanel = LoadGenerated("HudPanel_Clean.png"),
                DarkPanel = LoadGenerated("HudPanel_Clean.png"),
                BossHealthFrame = LoadGenerated("HudPanel_Clean.png")
            };
        }

        private static Sprite LoadGenerated(string fileName)
        {
            string path = $"{GeneratedRecoveryArtRoot}/{fileName}";
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);

            if (sprite == null)
            {
                Debug.LogWarning($"Level 01 visual recovery missing generated sprite: {path}");
            }

            return sprite;
        }

        private static Sprite Load(string relativePath)
        {
            string path = RuntimeArtRoot + relativePath;
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);

            if (sprite == null)
            {
                Debug.LogWarning($"Level 01 visual recovery missing sprite: {path}");
            }

            return sprite;
        }
    }
}
#endif
