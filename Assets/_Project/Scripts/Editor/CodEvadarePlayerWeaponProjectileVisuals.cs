#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class CodEvadarePlayerWeaponProjectileVisuals
{
    private const string RuntimeArtRoot = "Assets/_Project/Art/Sprites/RuntimeTransparent";
    private const string SceneRoot = "Assets/_Project/Scenes";
    private const string BulletPrefabPath = "Assets/_Project/Prefabs/Weapons/Bullet.prefab";
    private const string EnemyProjectilePrefabPath = "Assets/_Project/Prefabs/Projectiles/EnemyProjectile.prefab";

    [MenuItem("Tools/Cod Evadare/Art/Phase 2-3/Apply Player Weapon Projectile Visuals")]
    public static void ApplyPlayerWeaponProjectileVisuals()
    {
        CodEvadareArtImportSettings.ApplyVisualImportSettings();

        SpriteLibrary sprites = SpriteLibrary.Load();
        int changedPrefabs = ApplyPrefabVisuals(sprites);
        int changedScenes = ApplyScenePlayerVisuals(sprites);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Applied player, weapon and projectile visual integration. Prefabs changed: {changedPrefabs}. Scenes changed: {changedScenes}.");
    }

    private static int ApplyPrefabVisuals(SpriteLibrary sprites)
    {
        int changedCount = 0;

        if (ApplyProjectilePrefabVisual(BulletPrefabPath, sprites.PlayerBullet, "BulletSprite", 0.24f, -90f, 25))
        {
            changedCount++;
        }

        if (ApplyProjectilePrefabVisual(EnemyProjectilePrefabPath, sprites.EnemyProjectile, "EnemyProjectileSprite", 0.22f, -90f, 25))
        {
            changedCount++;
        }

        foreach (string prefabPath in FindPrefabPaths("Assets/_Project/Prefabs/Pickups"))
        {
            GameObject prefabRoot = PrefabUtility.LoadPrefabContents(prefabPath);
            bool changed = false;

            try
            {
                WeaponPickup2D pickup = prefabRoot.GetComponent<WeaponPickup2D>();

                if (pickup == null)
                {
                    continue;
                }

                WeaponDefinition2D weapon = GetSerializedObjectReference<WeaponDefinition2D>(pickup, "weapon");
                Sprite weaponSprite = sprites.GetWeaponSprite(weapon != null ? weapon.WeaponName : prefabRoot.name);

                if (weaponSprite == null)
                {
                    Debug.LogWarning($"Skipped weapon pickup visual for '{prefabPath}' because no matching weapon sprite was found.");
                    continue;
                }

                changed = ApplyChildSpriteVisual(prefabRoot, "Visual", "WeaponSpriteRenderer", weaponSprite, 0.11f, 0f, 16, new Vector3(0f, 0f, 0f));
                changed |= DisableRootSpriteRenderer(prefabRoot);

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

        return changedCount;
    }

    private static bool ApplyProjectilePrefabVisual(string prefabPath, Sprite sprite, string childName, float scale, float rotationZ, int sortingOrder)
    {
        if (sprite == null)
        {
            Debug.LogWarning($"Skipped projectile visual for '{prefabPath}' because the sprite is missing.");
            return false;
        }

        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(prefabPath);
        bool changed = false;

        try
        {
            changed = ApplyChildSpriteVisual(prefabRoot, "Visual", childName, sprite, scale, rotationZ, sortingOrder, Vector3.zero);
            changed |= DisableRootSpriteRenderer(prefabRoot);

            if (changed)
            {
                PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabPath);
            }
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(prefabRoot);
        }

        return changed;
    }

    private static int ApplyScenePlayerVisuals(SpriteLibrary sprites)
    {
        int changedSceneCount = 0;

        foreach (string scenePath in FindScenePaths())
        {
            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            bool sceneChanged = false;

            foreach (GameObject root in scene.GetRootGameObjects())
            {
                PlayerShooting2D[] shootingComponents = root.GetComponentsInChildren<PlayerShooting2D>(true);

                foreach (PlayerShooting2D shooting in shootingComponents)
                {
                    GameObject player = shooting.gameObject;

                    if (player.GetComponent<PlayerMovement2D>() == null)
                    {
                        continue;
                    }

                    sceneChanged |= ApplyPlayerVisual(player, shooting, sprites);
                }
            }

            if (sceneChanged)
            {
                EditorSceneManager.SaveScene(scene);
                changedSceneCount++;
            }
        }

        return changedSceneCount;
    }

    private static bool ApplyPlayerVisual(GameObject player, PlayerShooting2D shooting, SpriteLibrary sprites)
    {
        bool changed = false;
        SpriteRenderer rootRenderer = player.GetComponent<SpriteRenderer>();
        int baseSortingOrder = rootRenderer != null ? rootRenderer.sortingOrder : 10;

        if (rootRenderer != null)
        {
            changed |= DisableRenderer(rootRenderer);
        }

        changed |= ApplyChildSpriteVisual(player, "Visual", "Shadow", sprites.PlayerShadow, 0.08f, 0f, baseSortingOrder - 2, new Vector3(0f, -0.08f, 0f), new Color(1f, 1f, 1f, 0.7f));
        changed |= ApplyChildSpriteVisual(player, "Visual", "SelectionRing", sprites.SelectionRing, 0.08f, 0f, baseSortingOrder - 1, Vector3.zero, new Color(1f, 1f, 1f, 0.85f));
        changed |= ApplyChildSpriteVisual(player, "Visual", "BodySpriteRenderer", sprites.PlayerBody, 0.075f, 0f, baseSortingOrder, Vector3.zero);

        Sprite weaponSprite = sprites.GetWeaponSprite(GetStartingWeaponName(shooting));
        changed |= ApplyChildSpriteVisual(player, "Visual/WeaponPivot", "WeaponSpriteRenderer", weaponSprite, 0.045f, 0f, baseSortingOrder + 1, new Vector3(0.28f, -0.02f, 0f));

        return changed;
    }

    private static bool ApplyChildSpriteVisual(
        GameObject owner,
        string visualPath,
        string rendererObjectName,
        Sprite sprite,
        float uniformScale,
        float rotationZ,
        int sortingOrder,
        Vector3 localPosition)
    {
        return ApplyChildSpriteVisual(owner, visualPath, rendererObjectName, sprite, uniformScale, rotationZ, sortingOrder, localPosition, Color.white);
    }

    private static bool ApplyChildSpriteVisual(
        GameObject owner,
        string visualPath,
        string rendererObjectName,
        Sprite sprite,
        float uniformScale,
        float rotationZ,
        int sortingOrder,
        Vector3 localPosition,
        Color color)
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

        if (changed)
        {
            EditorUtility.SetDirty(rendererTransform.gameObject);
        }

        return changed;
    }

    private static bool DisableRootSpriteRenderer(GameObject owner)
    {
        SpriteRenderer rootRenderer = owner.GetComponent<SpriteRenderer>();
        return rootRenderer != null && DisableRenderer(rootRenderer);
    }

    private static bool DisableRenderer(SpriteRenderer renderer)
    {
        if (!renderer.enabled)
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

    private static string GetStartingWeaponName(PlayerShooting2D shooting)
    {
        WeaponDefinition2D startingWeapon = GetSerializedObjectReference<WeaponDefinition2D>(shooting, "startingWeapon");

        if (startingWeapon != null)
        {
            return startingWeapon.WeaponName;
        }

        WeaponDefinition2D equippedWeapon = GetSerializedObjectReference<WeaponDefinition2D>(shooting, "equippedWeapon");
        return equippedWeapon != null ? equippedWeapon.WeaponName : "Pistol";
    }

    private static T GetSerializedObjectReference<T>(Object target, string propertyName) where T : Object
    {
        if (target == null)
        {
            return null;
        }

        SerializedObject serializedObject = new SerializedObject(target);
        SerializedProperty property = serializedObject.FindProperty(propertyName);

        return property != null ? property.objectReferenceValue as T : null;
    }

    private static IEnumerable<string> FindPrefabPaths(string folder)
    {
        if (!AssetDatabase.IsValidFolder(folder))
        {
            yield break;
        }

        foreach (string guid in AssetDatabase.FindAssets("t:Prefab", new[] { folder }))
        {
            yield return AssetDatabase.GUIDToAssetPath(guid);
        }
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
        private readonly Dictionary<string, Sprite> weaponSprites = new Dictionary<string, Sprite>();

        public Sprite PlayerBody { get; private set; }
        public Sprite SelectionRing { get; private set; }
        public Sprite PlayerShadow { get; private set; }
        public Sprite PlayerBullet { get; private set; }
        public Sprite EnemyProjectile { get; private set; }

        public static SpriteLibrary Load()
        {
            SpriteLibrary library = new SpriteLibrary
            {
                PlayerBody = LoadSprite("Player/Player_Body_Base.png"),
                SelectionRing = LoadSprite("Player/Player_SelectionRing.png"),
                PlayerShadow = LoadSprite("Player/Player_Shadow.png"),
                PlayerBullet = LoadSprite("Projectiles/Projectile_BulletTracer.png"),
                EnemyProjectile = LoadSprite("Projectiles/Projectile_EnemyLaser.png")
            };

            library.AddWeapon("ArcaneStaff", "Weapons/Weapon_ArcaneStaff.png");
            library.AddWeapon("AssaultRifle", "Weapons/Weapon_AssaultRifle.png");
            library.AddWeapon("Grenade", "Weapons/Weapon_Grenade.png");
            library.AddWeapon("MeleeBlade", "Weapons/Weapon_MeleeBlade.png");
            library.AddWeapon("Pistol", "Weapons/Weapon_Pistol.png");
            library.AddWeapon("PlasmaRifle", "Weapons/Weapon_PlasmaRifle.png");
            library.AddWeapon("Revolver", "Weapons/Weapon_Revolver.png");
            library.AddWeapon("Shotgun", "Weapons/Weapon_Shotgun.png");
            library.AddWeapon("SMG", "Weapons/Weapon_SMG.png");

            return library;
        }

        public Sprite GetWeaponSprite(string weaponName)
        {
            string normalizedName = NormalizeWeaponName(weaponName);

            if (!string.IsNullOrEmpty(normalizedName) && weaponSprites.TryGetValue(normalizedName, out Sprite sprite))
            {
                return sprite;
            }

            return weaponSprites.TryGetValue("Pistol", out Sprite fallback) ? fallback : null;
        }

        private void AddWeapon(string weaponName, string relativePath)
        {
            Sprite sprite = LoadSprite(relativePath);

            if (sprite != null)
            {
                weaponSprites[NormalizeWeaponName(weaponName)] = sprite;
            }
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

        private static string NormalizeWeaponName(string weaponName)
        {
            if (string.IsNullOrWhiteSpace(weaponName))
            {
                return string.Empty;
            }

            return weaponName.Replace(" ", string.Empty).Replace("_", string.Empty);
        }
    }
}
#endif
