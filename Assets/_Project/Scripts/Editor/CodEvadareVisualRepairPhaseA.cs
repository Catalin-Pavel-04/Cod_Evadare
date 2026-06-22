#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class CodEvadareVisualRepairPhaseA
{
    private const string CampaignSceneRoot = "Assets/_Project/Scenes/Levels";
    private const string RuntimeTransparentRoot = "Assets/_Project/Art/Sprites/RuntimeTransparent/";
    private const int MaxExamplesPerCategory = 25;

    [MenuItem("Tools/Cod Evadare/Art/Phase A/Report Campaign Visual Footprints")]
    public static void ReportCampaignVisualFootprints()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            Debug.LogWarning("Phase A visual footprint report cancelled because the current scene has unsaved changes.");
            return;
        }

        string originalScenePath = SceneManager.GetActiveScene().path;
        RepairReport totalReport = new RepairReport();

        try
        {
            foreach (string scenePath in FindCampaignScenePaths())
            {
                Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                RepairReport sceneReport = ScanScene(scene, repair: false);
                totalReport.Add(sceneReport);

                Debug.Log(
                    $"Phase A visual report for {scenePath}: " +
                    $"candidates={sceneReport.Candidates}, " +
                    $"missingVisuals={sceneReport.MissingVisuals}, " +
                    $"tinyVisuals={sceneReport.TinyVisuals}, " +
                    $"disabledRootsWithBrokenVisuals={sceneReport.DisabledRootsWithBrokenVisuals}, " +
                    $"unsafeSourceSprites={sceneReport.UnsafeSourceSprites}, " +
                    $"suspiciousScales={sceneReport.SuspiciousScales}.");

                sceneReport.LogExamples(scenePath);
            }
        }
        finally
        {
            RestoreScene(originalScenePath);
        }

        Debug.Log(
            "Phase A visual footprint report complete. " +
            $"scenes={totalReport.Scenes}, candidates={totalReport.Candidates}, " +
            $"missingVisuals={totalReport.MissingVisuals}, tinyVisuals={totalReport.TinyVisuals}, " +
            $"disabledRootsWithBrokenVisuals={totalReport.DisabledRootsWithBrokenVisuals}, " +
            $"unsafeSourceSprites={totalReport.UnsafeSourceSprites}, suspiciousScales={totalReport.SuspiciousScales}.");
    }

    [MenuItem("Tools/Cod Evadare/Art/Phase A/Repair Campaign Visual Footprints")]
    public static void RepairCampaignVisualFootprints()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            Debug.LogWarning("Phase A visual footprint repair cancelled because the current scene has unsaved changes.");
            return;
        }

        CodEvadareArtImportSettings.ApplyVisualImportSettings();

        string originalScenePath = SceneManager.GetActiveScene().path;
        RepairReport totalReport = new RepairReport();
        int changedScenes = 0;

        try
        {
            foreach (string scenePath in FindCampaignScenePaths())
            {
                Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                RepairReport sceneReport = ScanScene(scene, repair: true);
                totalReport.Add(sceneReport);

                if (sceneReport.ChangedObjects > 0)
                {
                    EditorSceneManager.SaveScene(scene);
                    changedScenes++;
                }

                Debug.Log(
                    $"Phase A visual repair for {scenePath}: " +
                    $"changedObjects={sceneReport.ChangedObjects}, " +
                    $"candidates={sceneReport.Candidates}, " +
                    $"missingVisuals={sceneReport.MissingVisuals}, " +
                    $"tinyVisuals={sceneReport.TinyVisuals}, " +
                    $"disabledRootsWithBrokenVisuals={sceneReport.DisabledRootsWithBrokenVisuals}, " +
                    $"unsafeSourceSprites={sceneReport.UnsafeSourceSprites}, " +
                    $"suspiciousScales={sceneReport.SuspiciousScales}.");

                sceneReport.LogExamples(scenePath);
            }
        }
        finally
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            RestoreScene(originalScenePath);
        }

        Debug.Log(
            "Phase A campaign visual footprint repair complete. " +
            $"changedScenes={changedScenes}, changedObjects={totalReport.ChangedObjects}, " +
            $"missingVisualsBeforeRepair={totalReport.MissingVisuals}, tinyVisualsBeforeRepair={totalReport.TinyVisuals}, " +
            $"disabledRootsWithBrokenVisualsBeforeRepair={totalReport.DisabledRootsWithBrokenVisuals}, " +
            $"unsafeSourceSpritesBeforeRepair={totalReport.UnsafeSourceSprites}, suspiciousScalesBeforeRepair={totalReport.SuspiciousScales}.");
    }

    private static RepairReport ScanScene(Scene scene, bool repair)
    {
        RepairReport report = new RepairReport { Scenes = 1 };

        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (SpriteRenderer renderer in root.GetComponentsInChildren<SpriteRenderer>(true))
            {
                ScanSpriteReference(renderer, report);
            }

            foreach (Transform transform in root.GetComponentsInChildren<Transform>(true))
            {
                if (IsVisualDescendant(transform))
                {
                    continue;
                }

                GameObject owner = transform.gameObject;
                VisualKind kind = Classify(owner);

                if (kind == VisualKind.None)
                {
                    continue;
                }

                report.Candidates++;

                SpriteRenderer rootRenderer = owner.GetComponent<SpriteRenderer>();
                SpriteRenderer visualRenderer = FindBestVisualRenderer(owner, kind);
                bool hasFootprint = TryGetFootprint(owner, rootRenderer, visualRenderer, out Vector2 targetSize, out Vector2 targetOffset);
                bool missingVisual = visualRenderer == null;
                bool tinyVisual = visualRenderer != null && IsTinyOrMismatched(visualRenderer, hasFootprint, targetSize);
                bool suspiciousScale = visualRenderer != null && HasSuspiciousScale(visualRenderer.transform);
                bool brokenReplacement = rootRenderer != null &&
                                         !rootRenderer.enabled &&
                                         !HasUsableReplacement(visualRenderer, hasFootprint, targetSize);

                if (missingVisual)
                {
                    report.MissingVisuals++;
                    report.AddExample(report.MissingVisualExamples, SceneLabel(owner, "missing Visual child"));
                }

                if (tinyVisual)
                {
                    report.TinyVisuals++;
                    report.AddExample(report.TinyVisualExamples, SceneLabel(owner, $"tiny or mismatched visual ({DescribeRendererFootprint(visualRenderer)}, target {DescribeSize(targetSize, hasFootprint)})"));
                }

                if (brokenReplacement)
                {
                    report.DisabledRootsWithBrokenVisuals++;
                    report.AddExample(report.DisabledRootExamples, SceneLabel(owner, "root SpriteRenderer disabled but replacement visual is missing or broken"));
                }

                if (suspiciousScale)
                {
                    report.SuspiciousScales++;
                    report.AddExample(report.SuspiciousScaleExamples, SceneLabel(owner, $"Visual scale {visualRenderer.transform.localScale}"));
                }

                if (repair && RepairOwner(owner, kind, rootRenderer, visualRenderer, hasFootprint, targetSize, targetOffset))
                {
                    report.ChangedObjects++;
                }
            }
        }

        return report;
    }

    private static bool RepairOwner(
        GameObject owner,
        VisualKind kind,
        SpriteRenderer rootRenderer,
        SpriteRenderer visualRenderer,
        bool hasFootprint,
        Vector2 targetSize,
        Vector2 targetOffset)
    {
        if (!hasFootprint)
        {
            return EnsureFallbackRootVisible(rootRenderer);
        }

        bool changed = false;

        if (visualRenderer == null)
        {
            visualRenderer = EnsureVisualRenderer(owner, kind, ref changed);
        }

        if (visualRenderer == null)
        {
            return changed | EnsureFallbackRootVisible(rootRenderer);
        }

        Sprite replacementSprite = visualRenderer.sprite;

        if (replacementSprite == null || IsUnsafeSourceSprite(replacementSprite))
        {
            replacementSprite = GetSafeSpriteFor(owner, kind, targetSize) ?? rootRenderer?.sprite ?? replacementSprite;
        }

        if (replacementSprite == null)
        {
            return changed | EnsureFallbackRootVisible(rootRenderer);
        }

        changed |= ApplyFootprint(visualRenderer, replacementSprite, owner, kind, targetSize, targetOffset, rootRenderer);

        if (rootRenderer != null)
        {
            if (HasUsableReplacement(visualRenderer, true, targetSize))
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

    private static bool ApplyFootprint(
        SpriteRenderer renderer,
        Sprite sprite,
        GameObject owner,
        VisualKind kind,
        Vector2 targetSize,
        Vector2 targetOffset,
        SpriteRenderer rootRenderer)
    {
        bool changed = false;
        SpriteDrawMode drawMode = kind == VisualKind.Wall || kind == VisualKind.Floor
            ? SpriteDrawMode.Tiled
            : SpriteDrawMode.Sliced;

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

        if (renderer.size != targetSize)
        {
            renderer.size = targetSize;
            changed = true;
        }

        if (renderer.transform.localPosition != (Vector3)targetOffset)
        {
            renderer.transform.localPosition = targetOffset;
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

        int sortingOrder = GetSortingOrder(kind, rootRenderer);

        if (renderer.sortingOrder != sortingOrder)
        {
            renderer.sortingOrder = sortingOrder;
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
            EditorUtility.SetDirty(owner);
        }

        return changed;
    }

    private static SpriteRenderer EnsureVisualRenderer(GameObject owner, VisualKind kind, ref bool changed)
    {
        Transform visualRoot = owner.transform.Find("Visual");

        if (visualRoot == null)
        {
            GameObject visualRootObject = new GameObject("Visual");
            visualRootObject.transform.SetParent(owner.transform, false);
            visualRoot = visualRootObject.transform;
            changed = true;
        }

        string rendererName = GetRendererName(kind);
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

    private static SpriteRenderer FindBestVisualRenderer(GameObject owner, VisualKind kind)
    {
        Transform visualRoot = owner.transform.Find("Visual");

        if (visualRoot == null)
        {
            return null;
        }

        SpriteRenderer[] renderers = visualRoot.GetComponentsInChildren<SpriteRenderer>(true);

        if (renderers.Length == 0)
        {
            return null;
        }

        string expectedName = GetRendererName(kind);

        foreach (SpriteRenderer renderer in renderers)
        {
            if (renderer.name == expectedName)
            {
                return renderer;
            }
        }

        foreach (SpriteRenderer renderer in renderers)
        {
            if (renderer.name.Contains(kind.ToString()))
            {
                return renderer;
            }
        }

        return renderers[0];
    }

    private static bool TryGetFootprint(
        GameObject owner,
        SpriteRenderer rootRenderer,
        SpriteRenderer visualRenderer,
        out Vector2 targetSize,
        out Vector2 targetOffset)
    {
        BoxCollider2D boxCollider = owner.GetComponent<BoxCollider2D>();

        if (boxCollider != null && boxCollider.size.x > 0.01f && boxCollider.size.y > 0.01f)
        {
            targetSize = Abs(boxCollider.size);
            targetOffset = boxCollider.offset;
            return true;
        }

        Collider2D collider = owner.GetComponent<Collider2D>();

        if (collider != null && collider.bounds.size.x > 0.01f && collider.bounds.size.y > 0.01f)
        {
            Vector3 localSize = owner.transform.InverseTransformVector(collider.bounds.size);
            Vector3 localCenter = owner.transform.InverseTransformPoint(collider.bounds.center);
            targetSize = Abs(localSize);
            targetOffset = new Vector2(localCenter.x, localCenter.y);
            return true;
        }

        if (TryGetRendererFootprint(rootRenderer, out targetSize))
        {
            targetOffset = Vector2.zero;
            return true;
        }

        if (TryGetRendererFootprint(visualRenderer, out targetSize))
        {
            targetOffset = visualRenderer != null ? (Vector2)visualRenderer.transform.localPosition : Vector2.zero;
            return true;
        }

        targetSize = Vector2.zero;
        targetOffset = Vector2.zero;
        return false;
    }

    private static bool TryGetRendererFootprint(SpriteRenderer renderer, out Vector2 size)
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

    private static bool IsTinyOrMismatched(SpriteRenderer renderer, bool hasFootprint, Vector2 targetSize)
    {
        if (renderer == null || renderer.sprite == null || !renderer.enabled)
        {
            return true;
        }

        Vector2 current = GetRendererLocalFootprint(renderer);

        if (current.x < 0.05f || current.y < 0.05f)
        {
            return true;
        }

        if (!hasFootprint)
        {
            return false;
        }

        bool tooNarrow = targetSize.x >= 1f && current.x < targetSize.x * 0.65f;
        bool tooShort = targetSize.y >= 1f && current.y < targetSize.y * 0.65f;
        bool sizeOneOnLargeTarget = Mathf.Approximately(renderer.size.x, 1f) &&
                                    Mathf.Approximately(renderer.size.y, 1f) &&
                                    (targetSize.x >= 2f || targetSize.y >= 2f);

        return tooNarrow || tooShort || sizeOneOnLargeTarget;
    }

    private static bool HasUsableReplacement(SpriteRenderer renderer, bool hasFootprint, Vector2 targetSize)
    {
        return renderer != null &&
               renderer.sprite != null &&
               renderer.enabled &&
               renderer.gameObject.activeSelf &&
               !IsTinyOrMismatched(renderer, hasFootprint, targetSize);
    }

    private static bool HasSuspiciousScale(Transform transform)
    {
        if (transform == null)
        {
            return false;
        }

        Vector3 scale = transform.localScale;
        float absX = Mathf.Abs(scale.x);
        float absY = Mathf.Abs(scale.y);

        if (absX < 0.05f || absY < 0.05f || absX > 4f || absY > 4f)
        {
            return true;
        }

        float min = Mathf.Max(0.001f, Mathf.Min(absX, absY));
        float max = Mathf.Max(absX, absY);
        return max / min > 8f;
    }

    private static Vector2 GetRendererLocalFootprint(SpriteRenderer renderer)
    {
        if (renderer == null || renderer.sprite == null)
        {
            return Vector2.zero;
        }

        Vector2 baseSize = renderer.drawMode == SpriteDrawMode.Simple
            ? new Vector2(renderer.sprite.bounds.size.x, renderer.sprite.bounds.size.y)
            : renderer.size;

        return Vector2.Scale(baseSize, Abs(renderer.transform.localScale));
    }

    private static bool EnsureFallbackRootVisible(SpriteRenderer rootRenderer)
    {
        if (rootRenderer == null || rootRenderer.enabled || rootRenderer.sprite == null)
        {
            return false;
        }

        rootRenderer.enabled = true;
        EditorUtility.SetDirty(rootRenderer);
        return true;
    }

    private static VisualKind Classify(GameObject owner)
    {
        if (owner == null || !HasRenderableSurface(owner))
        {
            return VisualKind.None;
        }

        string lowerName = owner.name.ToLowerInvariant();

        if (owner.GetComponent<DoorController2D>() != null ||
            owner.GetComponent<LockedGate2D>() != null ||
            lowerName.Contains("door") ||
            lowerName.Contains("gate"))
        {
            return VisualKind.Door;
        }

        if (lowerName.Contains("wall"))
        {
            return VisualKind.Wall;
        }

        if (lowerName.Contains("floor") || lowerName.Contains("tile"))
        {
            return VisualKind.Floor;
        }

        if (lowerName.Contains("cover") || lowerName.Contains("crate"))
        {
            return VisualKind.Cover;
        }

        if (lowerName.Contains("prop") ||
            lowerName.Contains("barrel") ||
            lowerName.Contains("terminal") ||
            lowerName.Contains("shop") ||
            lowerName.Contains("kiosk") ||
            lowerName.Contains("pillar"))
        {
            return VisualKind.Prop;
        }

        return VisualKind.None;
    }

    private static bool HasRenderableSurface(GameObject owner)
    {
        return owner.GetComponent<Collider2D>() != null ||
               owner.GetComponent<SpriteRenderer>() != null ||
               owner.transform.Find("Visual") != null ||
               owner.transform.Find("OpenVisual") != null;
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

    private static void ScanSpriteReference(SpriteRenderer renderer, RepairReport report)
    {
        if (renderer == null || renderer.sprite == null)
        {
            return;
        }

        if (!IsUnsafeSourceSprite(renderer.sprite))
        {
            return;
        }

        report.UnsafeSourceSprites++;
        string path = AssetDatabase.GetAssetPath(renderer.sprite);
        report.AddExample(report.UnsafeSourceExamples, SceneLabel(renderer.gameObject, $"uses non-RuntimeTransparent sprite {path}"));
    }

    private static bool IsUnsafeSourceSprite(Sprite sprite)
    {
        if (sprite == null)
        {
            return false;
        }

        string path = AssetDatabase.GetAssetPath(sprite).Replace("\\", "/");
        return path.StartsWith("Assets/_Project/Art/Sprites/") &&
               !path.StartsWith(RuntimeTransparentRoot);
    }

    private static Sprite GetSafeSpriteFor(GameObject owner, VisualKind kind, Vector2 targetSize)
    {
        bool horizontal = targetSize.x >= targetSize.y;
        string lowerName = owner.name.ToLowerInvariant();
        string relativePath = kind switch
        {
            VisualKind.Wall => horizontal ? "Tiles/Tile_WallHorizontal.png" : "Tiles/Tile_WallVertical.png",
            VisualKind.Floor => lowerName.Contains("hazard") ? "Tiles/Tile_HazardStripeFloor.png" : "Tiles/Tile_SciFiFloor.png",
            VisualKind.Door => horizontal ? "Props/Prop_AutomaticDoor_Horizontal.png" : "Props/Prop_AutomaticDoor_Vertical.png",
            VisualKind.Cover => lowerName.Contains("tall") ? "Props/Prop_TallCoverCrate.png" : "Props/Prop_LowCoverBlock.png",
            VisualKind.Prop => GetPropSpritePath(lowerName),
            _ => null
        };

        if (string.IsNullOrEmpty(relativePath))
        {
            return null;
        }

        return AssetDatabase.LoadAssetAtPath<Sprite>($"{RuntimeTransparentRoot}{relativePath}");
    }

    private static string GetPropSpritePath(string lowerName)
    {
        if (lowerName.Contains("barrel"))
        {
            return "Props/Prop_ExplosiveBarrel.png";
        }

        if (lowerName.Contains("objective"))
        {
            return "Props/Prop_ObjectiveTerminal.png";
        }

        if (lowerName.Contains("terminal"))
        {
            return "Props/Prop_WallTerminal.png";
        }

        if (lowerName.Contains("shop") || lowerName.Contains("kiosk"))
        {
            return "Props/Prop_ShopKiosk.png";
        }

        if (lowerName.Contains("pillar"))
        {
            return "Tiles/Tile_MetalPillar.png";
        }

        return "Props/Prop_LowCoverBlock.png";
    }

    private static int GetSortingOrder(VisualKind kind, SpriteRenderer rootRenderer)
    {
        if (rootRenderer != null)
        {
            return rootRenderer.sortingOrder;
        }

        return kind switch
        {
            VisualKind.Floor => -5,
            VisualKind.Wall => 1,
            VisualKind.Door => 8,
            VisualKind.Cover => 4,
            VisualKind.Prop => 4,
            _ => 0
        };
    }

    private static string GetRendererName(VisualKind kind)
    {
        return kind switch
        {
            VisualKind.Wall => "WallSpriteRenderer",
            VisualKind.Floor => "FloorSpriteRenderer",
            VisualKind.Door => "DoorSpriteRenderer",
            VisualKind.Cover => "CoverSpriteRenderer",
            VisualKind.Prop => "PropSpriteRenderer",
            _ => "EnvironmentSpriteRenderer"
        };
    }

    private static IEnumerable<string> FindCampaignScenePaths()
    {
        if (!AssetDatabase.IsValidFolder(CampaignSceneRoot))
        {
            yield break;
        }

        List<string> paths = new List<string>();

        foreach (string guid in AssetDatabase.FindAssets("t:Scene", new[] { CampaignSceneRoot }))
        {
            paths.Add(AssetDatabase.GUIDToAssetPath(guid));
        }

        paths.Sort();

        foreach (string path in paths)
        {
            yield return path;
        }
    }

    private static void RestoreScene(string scenePath)
    {
        if (!string.IsNullOrEmpty(scenePath))
        {
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
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

    private static string SceneLabel(GameObject owner, string message)
    {
        return $"{owner.scene.path} :: {GetHierarchyPath(owner.transform)} - {message}";
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

    private static string DescribeRendererFootprint(SpriteRenderer renderer)
    {
        if (renderer == null)
        {
            return "no renderer";
        }

        Vector2 size = GetRendererLocalFootprint(renderer);
        return $"{size.x:0.###}x{size.y:0.###}";
    }

    private static string DescribeSize(Vector2 size, bool hasSize)
    {
        return hasSize ? $"{size.x:0.###}x{size.y:0.###}" : "unknown";
    }

    private enum VisualKind
    {
        None,
        Wall,
        Floor,
        Door,
        Cover,
        Prop
    }

    private sealed class RepairReport
    {
        public int Scenes;
        public int Candidates;
        public int MissingVisuals;
        public int TinyVisuals;
        public int DisabledRootsWithBrokenVisuals;
        public int UnsafeSourceSprites;
        public int SuspiciousScales;
        public int ChangedObjects;

        public readonly List<string> MissingVisualExamples = new List<string>();
        public readonly List<string> TinyVisualExamples = new List<string>();
        public readonly List<string> DisabledRootExamples = new List<string>();
        public readonly List<string> UnsafeSourceExamples = new List<string>();
        public readonly List<string> SuspiciousScaleExamples = new List<string>();

        public void Add(RepairReport other)
        {
            Scenes += other.Scenes;
            Candidates += other.Candidates;
            MissingVisuals += other.MissingVisuals;
            TinyVisuals += other.TinyVisuals;
            DisabledRootsWithBrokenVisuals += other.DisabledRootsWithBrokenVisuals;
            UnsafeSourceSprites += other.UnsafeSourceSprites;
            SuspiciousScales += other.SuspiciousScales;
            ChangedObjects += other.ChangedObjects;
        }

        public void AddExample(List<string> examples, string value)
        {
            if (examples.Count < MaxExamplesPerCategory)
            {
                examples.Add(value);
            }
        }

        public void LogExamples(string scenePath)
        {
            LogExamples(scenePath, "Missing Visual children", MissingVisualExamples);
            LogExamples(scenePath, "Tiny or mismatched visuals", TinyVisualExamples);
            LogExamples(scenePath, "Disabled roots with broken replacements", DisabledRootExamples);
            LogExamples(scenePath, "Unsafe source sprite references", UnsafeSourceExamples);
            LogExamples(scenePath, "Suspicious visual scales", SuspiciousScaleExamples);
        }

        private static void LogExamples(string scenePath, string label, List<string> examples)
        {
            if (examples.Count == 0)
            {
                return;
            }

            Debug.Log($"Phase A report examples for {scenePath} - {label}:\n- {string.Join("\n- ", examples)}");
        }
    }
}
#endif
