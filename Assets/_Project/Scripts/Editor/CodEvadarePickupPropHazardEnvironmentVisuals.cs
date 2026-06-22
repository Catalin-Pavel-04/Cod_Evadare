#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class CodEvadarePickupPropHazardEnvironmentVisuals
{
    private const string RuntimeArtRoot = "Assets/_Project/Art/Sprites/RuntimeTransparent";
    private const string SceneRoot = "Assets/_Project/Scenes";

    [MenuItem("Tools/Cod Evadare/Art/Phase 4-7/Apply Pickup Prop Hazard Environment Visuals")]
    public static void ApplyPickupPropHazardEnvironmentVisuals()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            Debug.LogWarning("Pickup/prop/hazard/environment visual integration cancelled because the current scene has unsaved changes.");
            return;
        }

        CodEvadareArtImportSettings.ApplyVisualImportSettings();

        SpriteLibrary sprites = SpriteLibrary.Load();
        int changedPrefabs = ApplyPrefabVisuals(sprites);
        int changedScenes = ApplySceneVisuals(sprites);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Applied pickup, prop, hazard and environment visuals. Prefabs changed: {changedPrefabs}. Scenes changed: {changedScenes}.");
    }

    private static int ApplyPrefabVisuals(SpriteLibrary sprites)
    {
        int changedCount = 0;
        string[] folders =
        {
            "Assets/_Project/Prefabs/Pickups",
            "Assets/_Project/Prefabs/Hazards",
            "Assets/_Project/Prefabs/Environment",
            "Assets/_Project/Prefabs/Prison"
        };

        foreach (string folder in folders)
        {
            if (!AssetDatabase.IsValidFolder(folder))
            {
                continue;
            }

            foreach (string guid in AssetDatabase.FindAssets("t:Prefab", new[] { folder }))
            {
                string prefabPath = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefabRoot = PrefabUtility.LoadPrefabContents(prefabPath);
                bool changed = false;

                try
                {
                    changed |= ApplyPickupVisual(prefabRoot, sprites);
                    changed |= ApplyHazardVisual(prefabRoot, sprites);
                    changed |= ApplyLockedGateVisual(prefabRoot, sprites);

                    if (changed)
                    {
                        PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabPath);
                        changedCount++;
                    }
                }
                finally
                {
                    PrefabUtility.UnloadPrefabContents(prefabRoot);
                }
            }
        }

        return changedCount;
    }

    private static int ApplySceneVisuals(SpriteLibrary sprites)
    {
        int changedSceneCount = 0;

        foreach (string scenePath in FindScenePaths())
        {
            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            bool changed = false;

            foreach (GameObject root in scene.GetRootGameObjects())
            {
                foreach (Transform transform in root.GetComponentsInChildren<Transform>(true))
                {
                    GameObject current = transform.gameObject;

                    changed |= ApplyPickupVisual(current, sprites);
                    changed |= ApplyHazardVisual(current, sprites);
                    changed |= ApplyLockedGateVisual(current, sprites);
                    changed |= ApplyDoorVisual(current, sprites);
                    changed |= ApplyEnvironmentVisual(current, sprites);
                }
            }

            if (changed)
            {
                EditorSceneManager.SaveScene(scene);
                changedSceneCount++;
            }
        }

        return changedSceneCount;
    }

    private static bool ApplyPickupVisual(GameObject owner, SpriteLibrary sprites)
    {
        Sprite sprite = null;

        ResourcePickup2D resourcePickup = owner.GetComponent<ResourcePickup2D>();

        if (resourcePickup != null)
        {
            int pickupType = GetEnumIndex(resourcePickup, "pickupType");
            sprite = pickupType switch
            {
                0 => sprites.HealthPickup,
                1 => sprites.AmmoPickup,
                2 => sprites.MoneyPickup,
                _ => null
            };
        }
        else if (owner.GetComponent<KeycardPickup2D>() != null)
        {
            sprite = sprites.KeycardPickup;
        }
        else if (owner.GetComponent<RewardPickup2D>() != null)
        {
            sprite = sprites.RewardPickup;
        }
        else if (owner.GetComponent<WeaponPickup2D>() != null)
        {
            sprite = sprites.WeaponCratePickup;
        }

        if (sprite == null)
        {
            return false;
        }

        bool changed = ApplySpriteVisual(owner, "Visual", "PickupSpriteRenderer", sprite, 0.11f, 0f, 16, Vector3.zero);
        changed |= DisableRootSpriteRenderer(owner);
        return changed;
    }

    private static bool ApplyHazardVisual(GameObject owner, SpriteLibrary sprites)
    {
        SecurityLaserHazard2D laser = owner.GetComponent<SecurityLaserHazard2D>();

        if (laser != null)
        {
            return ApplySecurityLaserVisual(owner, laser, sprites);
        }

        Sprite sprite = null;
        string lowerName = owner.name.ToLowerInvariant();

        if (lowerName.Contains("electric"))
        {
            sprite = sprites.ElectricFloor;
        }
        else if (lowerName.Contains("toxic") || lowerName.Contains("slime"))
        {
            sprite = lowerName.Contains("small") ? sprites.ToxicPuddleSmall : sprites.ToxicPuddleLarge;
        }
        else if (lowerName.Contains("blood"))
        {
            sprite = sprites.DangerStain;
        }

        if (sprite == null)
        {
            return false;
        }

        bool changed = ApplySpriteVisual(owner, "Visual", "HazardSpriteRenderer", sprite, 0.095f, 0f, 6, Vector3.zero);
        changed |= DisableRootSpriteRenderer(owner);
        return changed;
    }

    private static bool ApplySecurityLaserVisual(GameObject owner, SecurityLaserHazard2D laser, SpriteLibrary sprites)
    {
        bool horizontal = IsMostlyHorizontal(owner.transform);
        Sprite beamSprite = horizontal ? sprites.LaserBeamHorizontal : sprites.LaserBeamVertical;

        bool changed = ApplySpriteVisual(owner, "Visual", "LaserBeam", beamSprite, 0.08f, 0f, 8, Vector3.zero);
        SpriteRenderer beamRenderer = FindVisualRenderer(owner.transform, "Visual/LaserBeam");

        if (beamRenderer != null)
        {
            changed |= SetSerializedObjectReference(laser, "spriteRenderer", beamRenderer);
        }

        Vector3 firstEmitterPosition = horizontal ? new Vector3(-0.5f, 0f, 0f) : new Vector3(0f, -0.5f, 0f);
        Vector3 secondEmitterPosition = horizontal ? new Vector3(0.5f, 0f, 0f) : new Vector3(0f, 0.5f, 0f);
        float emitterRotation = horizontal ? 90f : 0f;

        changed |= ApplySpriteVisual(owner, "Visual", "LaserEmitter_A", sprites.LaserEmitter, 0.1f, emitterRotation, 9, firstEmitterPosition);
        changed |= ApplySpriteVisual(owner, "Visual", "LaserEmitter_B", sprites.LaserEmitter, 0.1f, emitterRotation, 9, secondEmitterPosition);
        changed |= DisableRootSpriteRenderer(owner);
        return changed;
    }

    private static bool ApplyLockedGateVisual(GameObject owner, SpriteLibrary sprites)
    {
        LockedGate2D lockedGate = owner.GetComponent<LockedGate2D>();

        if (lockedGate == null)
        {
            return false;
        }

        bool horizontal = IsMostlyHorizontal(owner.transform);
        Sprite lockedSprite = horizontal ? sprites.LockedGateHorizontal : sprites.LockedGateVertical;
        float rotation = 0f;

        bool changed = ApplySpriteVisual(owner, "Visual", "LockedGateSpriteRenderer", lockedSprite, 0.095f, rotation, 8, Vector3.zero);
        changed |= ApplySpriteVisual(owner, "OpenVisual", "OpenGateSpriteRenderer", sprites.DoorFrame, 0.095f, rotation, 7, Vector3.zero);

        Transform lockedVisual = owner.transform.Find("Visual/LockedGateSpriteRenderer");
        Transform openVisual = owner.transform.Find("OpenVisual");
        SpriteRenderer lockedRenderer = lockedVisual != null ? lockedVisual.GetComponent<SpriteRenderer>() : null;

        if (lockedRenderer != null)
        {
            changed |= SetSerializedObjectReference(lockedGate, "spriteRenderer", lockedRenderer);
        }

        if (lockedVisual != null)
        {
            changed |= SetSerializedObjectReference(lockedGate, "lockedVisual", lockedVisual.gameObject);
        }

        if (openVisual != null)
        {
            changed |= SetSerializedObjectReference(lockedGate, "openVisual", openVisual.gameObject);
        }

        changed |= DisableRootSpriteRenderer(owner);
        return changed;
    }

    private static bool ApplyDoorVisual(GameObject owner, SpriteLibrary sprites)
    {
        DoorController2D door = owner.GetComponent<DoorController2D>();

        if (door == null)
        {
            return false;
        }

        bool horizontal = IsMostlyHorizontal(owner.transform);
        Sprite doorSprite = horizontal ? sprites.AutomaticDoorHorizontal : sprites.AutomaticDoorVertical;

        bool changed = ApplySpriteVisual(owner, "Visual", "DoorSpriteRenderer", doorSprite, 0.09f, 0f, 8, Vector3.zero);
        SpriteRenderer doorRenderer = FindVisualRenderer(owner.transform, "Visual/DoorSpriteRenderer");

        if (doorRenderer != null)
        {
            changed |= SetSerializedObjectReference(door, "spriteRenderer", doorRenderer);
        }

        changed |= DisableRootSpriteRenderer(owner);
        return changed;
    }

    private static bool ApplyEnvironmentVisual(GameObject owner, SpriteLibrary sprites)
    {
        if (owner.GetComponent<DoorController2D>() != null ||
            owner.GetComponent<LockedGate2D>() != null ||
            owner.GetComponent<SecurityLaserHazard2D>() != null ||
            owner.GetComponent<ResourcePickup2D>() != null ||
            owner.GetComponent<KeycardPickup2D>() != null ||
            owner.GetComponent<RewardPickup2D>() != null ||
            owner.GetComponent<WeaponPickup2D>() != null)
        {
            return false;
        }

        SpriteRenderer rootRenderer = owner.GetComponent<SpriteRenderer>();

        if (rootRenderer == null)
        {
            return false;
        }

        string lowerName = owner.name.ToLowerInvariant();
        Sprite sprite = null;
        string rendererName = "EnvironmentSpriteRenderer";
        float scale = 0.08f;
        int sortingOrder = rootRenderer.sortingOrder;

        if (lowerName.Contains("wall"))
        {
            sprite = IsMostlyHorizontal(owner.transform) ? sprites.WallHorizontal : sprites.WallVertical;
            rendererName = "WallSpriteRenderer";
        }
        else if (lowerName.Contains("floor"))
        {
            sprite = lowerName.Contains("hazard") ? sprites.HazardStripeFloor : sprites.SciFiFloor;
            rendererName = "FloorSpriteRenderer";
            sortingOrder = Mathf.Min(sortingOrder, -1);
        }
        else if (lowerName.Contains("cover") || lowerName.Contains("crate"))
        {
            sprite = lowerName.Contains("tall") ? sprites.TallCoverCrate : sprites.LowCoverBlock;
            rendererName = "CoverSpriteRenderer";
            sortingOrder = Mathf.Max(sortingOrder, 4);
        }
        else if (lowerName.Contains("barrel"))
        {
            sprite = sprites.ExplosiveBarrel;
            rendererName = "PropSpriteRenderer";
        }
        else if (lowerName.Contains("terminal"))
        {
            sprite = lowerName.Contains("objective") ? sprites.ObjectiveTerminal : sprites.WallTerminal;
            rendererName = "PropSpriteRenderer";
        }
        else if (lowerName.Contains("shop") || lowerName.Contains("kiosk"))
        {
            sprite = sprites.ShopKiosk;
            rendererName = "PropSpriteRenderer";
        }
        else if (lowerName.Contains("pillar"))
        {
            sprite = sprites.MetalPillar;
            rendererName = "PropSpriteRenderer";
        }

        if (sprite == null)
        {
            return false;
        }

        bool changed = ApplySpriteVisual(owner, "Visual", rendererName, sprite, scale, 0f, sortingOrder, Vector3.zero);
        SpriteRenderer visualRenderer = FindVisualRenderer(owner.transform, $"Visual/{rendererName}");

        if (visualRenderer != null && (lowerName.Contains("wall") || lowerName.Contains("floor")))
        {
            changed |= SetTiledRenderer(visualRenderer);
        }

        changed |= DisableRootSpriteRenderer(owner);
        return changed;
    }

    private static bool ApplySpriteVisual(
        GameObject owner,
        string visualPath,
        string rendererObjectName,
        Sprite sprite,
        float uniformScale,
        float rotationZ,
        int sortingOrder,
        Vector3 localPosition)
    {
        if (owner == null || sprite == null)
        {
            return false;
        }

        bool changed = false;
        Transform parent = EnsureChildPath(owner.transform, visualPath, ref changed);
        Transform rendererTransform = EnsureDirectChild(parent, rendererObjectName, ref changed);

        changed |= SetTransform(rendererTransform, localPosition, Quaternion.Euler(0f, 0f, rotationZ), new Vector3(uniformScale, uniformScale, 1f));

        SpriteRenderer renderer = rendererTransform.GetComponent<SpriteRenderer>();

        if (renderer == null)
        {
            renderer = rendererTransform.gameObject.AddComponent<SpriteRenderer>();
            changed = true;
        }

        if (renderer.sprite != sprite)
        {
            renderer.sprite = sprite;
            changed = true;
        }

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

        if (changed)
        {
            EditorUtility.SetDirty(rendererTransform.gameObject);
        }

        return changed;
    }

    private static bool SetTiledRenderer(SpriteRenderer renderer)
    {
        bool changed = false;

        if (renderer.drawMode != SpriteDrawMode.Tiled)
        {
            renderer.drawMode = SpriteDrawMode.Tiled;
            changed = true;
        }

        if (renderer.size != Vector2.one)
        {
            renderer.size = Vector2.one;
            changed = true;
        }

        return changed;
    }

    private static bool DisableRootSpriteRenderer(GameObject owner)
    {
        SpriteRenderer renderer = owner.GetComponent<SpriteRenderer>();

        if (renderer == null || !renderer.enabled)
        {
            return false;
        }

        renderer.enabled = false;
        EditorUtility.SetDirty(renderer);
        return true;
    }

    private static Transform EnsureChildPath(Transform owner, string childPath, ref bool changed)
    {
        Transform current = owner;
        string[] parts = childPath.Split('/');

        foreach (string part in parts)
        {
            current = EnsureDirectChild(current, part, ref changed);
        }

        return current;
    }

    private static Transform EnsureDirectChild(Transform parent, string childName, ref bool changed)
    {
        Transform child = parent.Find(childName);

        if (child != null)
        {
            return child;
        }

        GameObject childObject = new GameObject(childName);
        childObject.transform.SetParent(parent, false);
        changed = true;
        return childObject.transform;
    }

    private static SpriteRenderer FindVisualRenderer(Transform owner, string path)
    {
        Transform child = owner.Find(path);
        return child != null ? child.GetComponent<SpriteRenderer>() : null;
    }

    private static bool SetTransform(Transform transform, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
    {
        bool changed = false;

        if (transform.localPosition != localPosition)
        {
            transform.localPosition = localPosition;
            changed = true;
        }

        if (transform.localRotation != localRotation)
        {
            transform.localRotation = localRotation;
            changed = true;
        }

        if (transform.localScale != localScale)
        {
            transform.localScale = localScale;
            changed = true;
        }

        return changed;
    }

    private static bool SetSerializedObjectReference(Object target, string propertyName, Object value)
    {
        SerializedObject serializedObject = new SerializedObject(target);
        SerializedProperty property = serializedObject.FindProperty(propertyName);

        if (property == null || property.objectReferenceValue == value)
        {
            return false;
        }

        property.objectReferenceValue = value;
        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);
        return true;
    }

    private static int GetEnumIndex(Object target, string propertyName)
    {
        SerializedObject serializedObject = new SerializedObject(target);
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        return property != null ? property.enumValueIndex : -1;
    }

    private static bool IsMostlyHorizontal(Transform transform)
    {
        Vector3 scale = transform.localScale;
        return Mathf.Abs(scale.x) >= Mathf.Abs(scale.y);
    }

    private static IEnumerable<string> FindScenePaths()
    {
        foreach (string guid in AssetDatabase.FindAssets("t:Scene", new[] { SceneRoot }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            if (path.Contains("/Game/") || path.EndsWith("/MainMenu.unity"))
            {
                continue;
            }

            yield return path;
        }
    }

    private sealed class SpriteLibrary
    {
        public Sprite HealthPickup { get; private set; }
        public Sprite AmmoPickup { get; private set; }
        public Sprite MoneyPickup { get; private set; }
        public Sprite KeycardPickup { get; private set; }
        public Sprite RewardPickup { get; private set; }
        public Sprite WeaponCratePickup { get; private set; }

        public Sprite AutomaticDoorHorizontal { get; private set; }
        public Sprite AutomaticDoorVertical { get; private set; }
        public Sprite DoorFrame { get; private set; }
        public Sprite LockedGateHorizontal { get; private set; }
        public Sprite LockedGateVertical { get; private set; }
        public Sprite LowCoverBlock { get; private set; }
        public Sprite TallCoverCrate { get; private set; }
        public Sprite ExplosiveBarrel { get; private set; }
        public Sprite ShopKiosk { get; private set; }
        public Sprite WallTerminal { get; private set; }
        public Sprite ObjectiveTerminal { get; private set; }

        public Sprite ElectricFloor { get; private set; }
        public Sprite LaserBeamHorizontal { get; private set; }
        public Sprite LaserBeamVertical { get; private set; }
        public Sprite LaserEmitter { get; private set; }
        public Sprite ToxicPuddleSmall { get; private set; }
        public Sprite ToxicPuddleLarge { get; private set; }
        public Sprite DangerStain { get; private set; }

        public Sprite WallHorizontal { get; private set; }
        public Sprite WallVertical { get; private set; }
        public Sprite SciFiFloor { get; private set; }
        public Sprite HazardStripeFloor { get; private set; }
        public Sprite MetalPillar { get; private set; }

        public static SpriteLibrary Load()
        {
            return new SpriteLibrary
            {
                HealthPickup = LoadSprite("Pickups/Pickup_Medkit.png"),
                AmmoPickup = LoadSprite("Pickups/Pickup_Ammo.png"),
                MoneyPickup = LoadSprite("Pickups/Pickup_Money.png"),
                KeycardPickup = LoadSprite("Pickups/Pickup_Keycard.png"),
                RewardPickup = LoadSprite("Pickups/Pickup_BuffVial.png"),
                WeaponCratePickup = LoadSprite("Pickups/Pickup_WeaponCrate.png"),

                AutomaticDoorHorizontal = LoadSprite("Props/Prop_AutomaticDoor_Horizontal.png"),
                AutomaticDoorVertical = LoadSprite("Props/Prop_AutomaticDoor_Vertical.png"),
                DoorFrame = LoadSprite("Tiles/Tile_DoorFrame.png"),
                LockedGateHorizontal = LoadSprite("Props/Prop_LockedPrisonGate_Horizontal.png"),
                LockedGateVertical = LoadSprite("Props/Prop_LockedPrisonGate_Vertical.png"),
                LowCoverBlock = LoadSprite("Props/Prop_LowCoverBlock.png"),
                TallCoverCrate = LoadSprite("Props/Prop_TallCoverCrate.png"),
                ExplosiveBarrel = LoadSprite("Props/Prop_ExplosiveBarrel.png"),
                ShopKiosk = LoadSprite("Props/Prop_ShopKiosk.png"),
                WallTerminal = LoadSprite("Props/Prop_WallTerminal.png"),
                ObjectiveTerminal = LoadSprite("Props/Prop_ObjectiveTerminal.png"),

                ElectricFloor = LoadSprite("Hazards/Hazard_ElectricFloorTile.png"),
                LaserBeamHorizontal = LoadSprite("Hazards/Hazard_LaserBeamHorizontal.png"),
                LaserBeamVertical = LoadSprite("Hazards/Hazard_LaserBeamVertical.png"),
                LaserEmitter = LoadSprite("Hazards/Hazard_LaserEmitterPost.png"),
                ToxicPuddleSmall = LoadSprite("Hazards/Hazard_ToxicPuddleSmall.png"),
                ToxicPuddleLarge = LoadSprite("Hazards/Hazard_ToxicPuddleLarge.png"),
                DangerStain = LoadSprite("Hazards/Hazard_BloodlessDangerStain.png"),

                WallHorizontal = LoadSprite("Tiles/Tile_WallHorizontal.png"),
                WallVertical = LoadSprite("Tiles/Tile_WallVertical.png"),
                SciFiFloor = LoadSprite("Tiles/Tile_SciFiFloor.png"),
                HazardStripeFloor = LoadSprite("Tiles/Tile_HazardStripeFloor.png"),
                MetalPillar = LoadSprite("Tiles/Tile_MetalPillar.png")
            };
        }

        private static Sprite LoadSprite(string relativePath)
        {
            string path = $"{RuntimeArtRoot}/{relativePath}";
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);

            if (sprite == null)
            {
                Debug.LogWarning($"Missing visual sprite: {path}");
            }

            return sprite;
        }
    }
}
#endif
