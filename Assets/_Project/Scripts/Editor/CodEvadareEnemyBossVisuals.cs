#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class CodEvadareEnemyBossVisuals
{
    private const string RuntimeArtRoot = "Assets/_Project/Art/Sprites/RuntimeTransparent";
    private const string SceneRoot = "Assets/_Project/Scenes";

    [MenuItem("Tools/Cod Evadare/Art/Phase 5/Apply Enemy Boss Visuals")]
    public static void ApplyEnemyBossVisuals()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            Debug.LogWarning("Enemy, miniboss and boss visual integration cancelled because the current scene has unsaved changes.");
            return;
        }

        string activeScenePath = SceneManager.GetActiveScene().path;

        CodEvadareArtImportSettings.ApplyVisualImportSettings();

        SpriteLibrary sprites = SpriteLibrary.Load();
        int changedPrefabs = ApplyPrefabVisuals(sprites);
        int changedScenes = ApplySceneVisuals(sprites);

        RestoreActiveScene(activeScenePath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Applied enemy, miniboss and boss visual integration. Prefabs changed: {changedPrefabs}. Scenes changed: {changedScenes}.");
    }

    private static int ApplyPrefabVisuals(SpriteLibrary sprites)
    {
        int changedCount = 0;
        string[] folders =
        {
            "Assets/_Project/Prefabs/Enemies",
            "Assets/_Project/Prefabs/Boss"
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
                    changed |= ApplyEnemyBossVisual(prefabRoot, sprites);

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
            bool sceneChanged = false;

            foreach (GameObject root in scene.GetRootGameObjects())
            {
                EnemyHealth[] enemyHealthComponents = root.GetComponentsInChildren<EnemyHealth>(true);

                foreach (EnemyHealth enemyHealth in enemyHealthComponents)
                {
                    GameObject owner = enemyHealth.gameObject;

                    if (IsPrefabInstanceWithManagedSource(owner))
                    {
                        continue;
                    }

                    sceneChanged |= ApplyEnemyBossVisual(owner, sprites);
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

    private static void RestoreActiveScene(string activeScenePath)
    {
        if (string.IsNullOrEmpty(activeScenePath))
        {
            return;
        }

        EditorSceneManager.OpenScene(activeScenePath, OpenSceneMode.Single);
    }

    private static bool ApplyEnemyBossVisual(GameObject owner, SpriteLibrary sprites)
    {
        if (owner == null || owner.GetComponent<EnemyHealth>() == null)
        {
            return false;
        }

        VisualProfile profile = ResolveVisualProfile(owner.name, sprites);

        if (profile == null || profile.BodySprite == null)
        {
            return false;
        }

        SpriteRenderer rootRenderer = owner.GetComponent<SpriteRenderer>();
        int baseSortingOrder = rootRenderer != null ? rootRenderer.sortingOrder : 10;

        bool changed = ApplySpriteVisual(owner, "Visual", "BodySpriteRenderer", profile.BodySprite, profile.Scale, profile.RotationZ, baseSortingOrder, Vector3.zero, profile.Color);
        SpriteRenderer bodyRenderer = FindVisualRenderer(owner.transform, "Visual/BodySpriteRenderer");

        if (bodyRenderer != null)
        {
            changed |= SetSpriteFlashRenderer(owner, bodyRenderer);
        }

        if (profile.AuraSprite != null)
        {
            changed |= ApplySpriteVisual(owner, "Visual", "NecromancerAura", profile.AuraSprite, profile.AuraScale, 0f, baseSortingOrder - 2, Vector3.zero, new Color(1f, 1f, 1f, 0.75f));
        }

        if (profile.SummonSprite != null)
        {
            changed |= ApplySpriteVisual(owner, "Visual", "NecromancerSummonEffect", profile.SummonSprite, profile.SummonScale, 0f, baseSortingOrder - 1, new Vector3(0f, -0.05f, 0f), new Color(1f, 1f, 1f, 0.65f));
        }

        changed |= DisableRootSpriteRenderer(owner);
        return changed;
    }

    private static bool IsPrefabInstanceWithManagedSource(GameObject owner)
    {
        if (!PrefabUtility.IsPartOfPrefabInstance(owner))
        {
            return false;
        }

        GameObject source = PrefabUtility.GetCorrespondingObjectFromSource(owner);

        if (source == null)
        {
            return false;
        }

        string sourcePath = AssetDatabase.GetAssetPath(source);
        return sourcePath.StartsWith("Assets/_Project/Prefabs/Enemies/") || sourcePath.StartsWith("Assets/_Project/Prefabs/Boss/");
    }

    private static VisualProfile ResolveVisualProfile(string objectName, SpriteLibrary sprites)
    {
        string normalizedName = NormalizeName(objectName);

        if (normalizedName.Contains("finalescape") || normalizedName.Contains("escapeoverlord"))
        {
            return VisualProfile.Boss(sprites.FinalEscapeOverlord);
        }

        if (normalizedName.Contains("aicore"))
        {
            return VisualProfile.Boss(sprites.AICoreBoss);
        }

        if (normalizedName.Contains("abomination"))
        {
            return VisualProfile.Boss(sprites.AbominationBoss);
        }

        if (normalizedName.Contains("experiment01") || normalizedName.Contains("experiment"))
        {
            return VisualProfile.Boss(sprites.ExperimentBoss);
        }

        if (normalizedName.Contains("nightmaredoctor"))
        {
            return VisualProfile.Boss(sprites.NightmareDoctorBoss);
        }

        if (normalizedName.Contains("wardenboss"))
        {
            return VisualProfile.Boss(sprites.WardenBoss);
        }

        if (normalizedName.Contains("necromancer"))
        {
            VisualProfile profile = VisualProfile.Miniboss(sprites.NecromancerMiniboss);
            profile.AuraSprite = sprites.NecromancerAura;
            profile.SummonSprite = sprites.NecromancerSummonEffect;
            return profile;
        }

        if (normalizedName.Contains("reactor"))
        {
            return VisualProfile.Miniboss(sprites.ReactorMiniboss);
        }

        if (normalizedName.Contains("surgeon"))
        {
            return VisualProfile.Miniboss(sprites.SurgeonMiniboss);
        }

        if (normalizedName.Contains("riotbrute"))
        {
            return VisualProfile.Miniboss(sprites.RiotBruteMiniboss);
        }

        if (normalizedName.Contains("shadowwarden"))
        {
            return VisualProfile.Miniboss(sprites.ShadowWardenMiniboss);
        }

        if (normalizedName.Contains("labminiboss") || normalizedName.Contains("prototype") || normalizedName.Contains("labalpha"))
        {
            return VisualProfile.Miniboss(sprites.LabAlphaMiniboss);
        }

        if (normalizedName.Contains("drone"))
        {
            return VisualProfile.Enemy(sprites.DroneEnemy);
        }

        if (normalizedName.Contains("ghost") || normalizedName.Contains("shadowcrawler"))
        {
            return VisualProfile.Enemy(sprites.GhostEnemy);
        }

        if (normalizedName.Contains("haunted") || normalizedName.Contains("nurse") || normalizedName.Contains("medic"))
        {
            return VisualProfile.Enemy(sprites.HauntedMedicEnemy);
        }

        if (normalizedName.Contains("riotguard"))
        {
            return VisualProfile.Enemy(sprites.RiotGuardEnemy);
        }

        if (normalizedName.Contains("rangedguard") || normalizedName.Contains("ranged"))
        {
            return VisualProfile.Enemy(sprites.RangedGuardEnemy);
        }

        if (normalizedName.Contains("prisonguard") || normalizedName.Contains("guard"))
        {
            return VisualProfile.Enemy(sprites.PrisonGuardEnemy);
        }

        if (normalizedName.Contains("securitybot") || normalizedName.Contains("laser") || normalizedName.Contains("turret"))
        {
            return VisualProfile.Enemy(sprites.SecurityBotEnemy);
        }

        if (normalizedName.Contains("runner"))
        {
            return VisualProfile.Enemy(sprites.ZombieRunnerEnemy);
        }

        if (normalizedName.Contains("zombie") || normalizedName.Contains("walker") || normalizedName.Contains("exploder"))
        {
            return VisualProfile.Enemy(sprites.ZombieWalkerEnemy);
        }

        if (normalizedName.Contains("lab") || normalizedName.Contains("mutant") || normalizedName.Contains("spitter") || normalizedName.Contains("testenemy"))
        {
            return VisualProfile.Enemy(sprites.LabMutantEnemy);
        }

        if (normalizedName.Contains("miniboss"))
        {
            return VisualProfile.Miniboss(sprites.LabAlphaMiniboss);
        }

        return null;
    }

    private static bool ApplySpriteVisual(
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

    private static bool SetSpriteFlashRenderer(GameObject owner, SpriteRenderer renderer)
    {
        SpriteFlash2D spriteFlash = owner.GetComponent<SpriteFlash2D>();

        if (spriteFlash == null)
        {
            return false;
        }

        return SetSerializedObjectReference(spriteFlash, "spriteRenderer", renderer);
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

    private static string NormalizeName(string objectName)
    {
        if (string.IsNullOrWhiteSpace(objectName))
        {
            return string.Empty;
        }

        return objectName
            .ToLowerInvariant()
            .Replace(" ", string.Empty)
            .Replace("_", string.Empty)
            .Replace("-", string.Empty)
            .Replace("(clone)", string.Empty);
    }

    private sealed class VisualProfile
    {
        private VisualProfile(Sprite bodySprite, float scale)
        {
            BodySprite = bodySprite;
            Scale = scale;
            RotationZ = 0f;
            AuraScale = 0.13f;
            SummonScale = 0.11f;
            Color = Color.white;
        }

        public Sprite BodySprite { get; }
        public float Scale { get; }
        public float RotationZ { get; }
        public float AuraScale { get; }
        public float SummonScale { get; }
        public Color Color { get; }
        public Sprite AuraSprite { get; set; }
        public Sprite SummonSprite { get; set; }

        public static VisualProfile Enemy(Sprite bodySprite)
        {
            return new VisualProfile(bodySprite, 0.075f);
        }

        public static VisualProfile Miniboss(Sprite bodySprite)
        {
            return new VisualProfile(bodySprite, 0.095f);
        }

        public static VisualProfile Boss(Sprite bodySprite)
        {
            return new VisualProfile(bodySprite, 0.145f);
        }
    }

    private sealed class SpriteLibrary
    {
        public Sprite PrisonGuardEnemy { get; private set; }
        public Sprite RangedGuardEnemy { get; private set; }
        public Sprite RiotGuardEnemy { get; private set; }
        public Sprite LabMutantEnemy { get; private set; }
        public Sprite ZombieWalkerEnemy { get; private set; }
        public Sprite ZombieRunnerEnemy { get; private set; }
        public Sprite DroneEnemy { get; private set; }
        public Sprite SecurityBotEnemy { get; private set; }
        public Sprite HauntedMedicEnemy { get; private set; }
        public Sprite GhostEnemy { get; private set; }

        public Sprite RiotBruteMiniboss { get; private set; }
        public Sprite LabAlphaMiniboss { get; private set; }
        public Sprite NecromancerMiniboss { get; private set; }
        public Sprite ReactorMiniboss { get; private set; }
        public Sprite SurgeonMiniboss { get; private set; }
        public Sprite ShadowWardenMiniboss { get; private set; }

        public Sprite ExperimentBoss { get; private set; }
        public Sprite WardenBoss { get; private set; }
        public Sprite AbominationBoss { get; private set; }
        public Sprite AICoreBoss { get; private set; }
        public Sprite NightmareDoctorBoss { get; private set; }
        public Sprite FinalEscapeOverlord { get; private set; }

        public Sprite NecromancerAura { get; private set; }
        public Sprite NecromancerSummonEffect { get; private set; }

        public static SpriteLibrary Load()
        {
            return new SpriteLibrary
            {
                PrisonGuardEnemy = LoadSprite("Enemies/Enemy_PrisonGuard.png"),
                RangedGuardEnemy = LoadSprite("Enemies/Enemy_RangedGuard.png"),
                RiotGuardEnemy = LoadSprite("Enemies/Enemy_RiotGuard.png"),
                LabMutantEnemy = LoadSprite("Enemies/Enemy_LabMutant.png"),
                ZombieWalkerEnemy = LoadSprite("Enemies/Enemy_ZombieWalker.png"),
                ZombieRunnerEnemy = LoadSprite("Enemies/Enemy_ZombieRunner.png"),
                DroneEnemy = LoadSprite("Enemies/Enemy_Drone.png"),
                SecurityBotEnemy = LoadSprite("Enemies/Enemy_SecurityBot.png"),
                HauntedMedicEnemy = LoadSprite("Enemies/Enemy_HauntedMedic.png"),
                GhostEnemy = LoadSprite("Enemies/Enemy_Ghost.png"),

                RiotBruteMiniboss = LoadSprite("Bosses/Miniboss_RiotBrute.png"),
                LabAlphaMiniboss = LoadSprite("Bosses/Miniboss_LabAlpha.png"),
                NecromancerMiniboss = LoadSprite("Bosses/Miniboss_Necromancer.png"),
                ReactorMiniboss = LoadSprite("Bosses/Miniboss_ReactorGuardian.png"),
                SurgeonMiniboss = LoadSprite("Bosses/Miniboss_Surgeon.png"),
                ShadowWardenMiniboss = LoadSprite("Bosses/Miniboss_ShadowWarden.png"),

                ExperimentBoss = LoadSprite("Bosses/Boss_Experiment01.png"),
                WardenBoss = LoadSprite("Bosses/Boss_TheWarden.png"),
                AbominationBoss = LoadSprite("Bosses/Boss_Abomination.png"),
                AICoreBoss = LoadSprite("Bosses/Boss_AICore.png"),
                NightmareDoctorBoss = LoadSprite("Bosses/Boss_NightmareDoctor.png"),
                FinalEscapeOverlord = LoadSprite("Bosses/Boss_FinalEscapeOverlord.png"),

                NecromancerAura = LoadSprite("Hazards/VFX/Necromancer_Aura.png"),
                NecromancerSummonEffect = LoadSprite("Hazards/VFX/Necromancer_SummonEffect.png")
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
