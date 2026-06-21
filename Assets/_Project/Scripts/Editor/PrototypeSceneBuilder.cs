#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class PrototypeSceneBuilder
{
    private const string ScenePath = "Assets/_Project/Scenes/Prototype_Lab.unity";
    private const string RoomLoopScenePath = "Assets/_Project/Scenes/Prototype_RoomLoop.unity";
    private const string HealthCombatScenePath = "Assets/_Project/Scenes/Prototype_HealthCombat.unity";
    private const string LootResourcesScenePath = "Assets/_Project/Scenes/Prototype_LootResources.unity";
    private const string BulletPrefabPath = "Assets/_Project/Prefabs/Weapons/Bullet.prefab";
    private const string EnemyPrefabPath = "Assets/_Project/Prefabs/Enemies/TestEnemy.prefab";
    private const string RewardPrefabPath = "Assets/_Project/Prefabs/Pickups/PrototypeReward.prefab";
    private const string HealthPickupPrefabPath = "Assets/_Project/Prefabs/Pickups/HealthPickup.prefab";
    private const string AmmoPickupPrefabPath = "Assets/_Project/Prefabs/Pickups/AmmoPickup.prefab";
    private const string MoneyPickupPrefabPath = "Assets/_Project/Prefabs/Pickups/MoneyPickup.prefab";
    private const string GeneratedArtFolder = "Assets/_Project/Art/Generated";
    private const string PlayerSpritePath = GeneratedArtFolder + "/Player_Prototype.png";
    private const string EnemySpritePath = GeneratedArtFolder + "/Enemy_Prototype.png";
    private const string BulletSpritePath = GeneratedArtFolder + "/Bullet_Prototype.png";
    private const string WallSpritePath = GeneratedArtFolder + "/Wall_Prototype.png";
    private const string RewardSpritePath = GeneratedArtFolder + "/Reward_Prototype.png";
    private const string HealthPickupSpritePath = GeneratedArtFolder + "/HealthPickup_Prototype.png";
    private const string AmmoPickupSpritePath = GeneratedArtFolder + "/AmmoPickup_Prototype.png";
    private const string MoneyPickupSpritePath = GeneratedArtFolder + "/MoneyPickup_Prototype.png";
    private const string PlayerTag = "Player";

    private static readonly string[] RequiredFolders =
    {
        "Assets/_Project",
        "Assets/_Project/Art",
        "Assets/_Project/Art/Generated",
        "Assets/_Project/Audio",
        "Assets/_Project/Materials",
        "Assets/_Project/Prefabs",
        "Assets/_Project/Prefabs/Player",
        "Assets/_Project/Prefabs/Weapons",
        "Assets/_Project/Prefabs/Enemies",
        "Assets/_Project/Prefabs/Pickups",
        "Assets/_Project/Prefabs/Loot",
        "Assets/_Project/Prefabs/Rooms",
        "Assets/_Project/Scenes",
        "Assets/_Project/Scripts",
        "Assets/_Project/Scripts/Core",
        "Assets/_Project/Scripts/Loot",
        "Assets/_Project/Scripts/Resources",
        "Assets/_Project/Scripts/UI",
        "Assets/_Project/Scripts/Player",
        "Assets/_Project/Scripts/Weapons",
        "Assets/_Project/Scripts/Enemies",
        "Assets/_Project/Scripts/Rooms",
        "Assets/_Project/Scripts/Pickups",
        "Assets/_Project/Scripts/Camera",
        "Assets/_Project/Scripts/Editor"
    };

    [MenuItem("Tools/Cod Evadare/Create Prototype 0.1 Scene")]
    public static void CreatePrototypeScene()
    {
        if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return;
        }

        CreateRequiredFolders();
        GeneratePlaceholderSprites();
        EnsureTag(PlayerTag);

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        SceneManager.SetActiveScene(scene);

        Sprite playerSprite = AssetDatabase.LoadAssetAtPath<Sprite>(PlayerSpritePath);
        Sprite enemySprite = AssetDatabase.LoadAssetAtPath<Sprite>(EnemySpritePath);
        Sprite bulletSprite = AssetDatabase.LoadAssetAtPath<Sprite>(BulletSpritePath);
        Sprite wallSprite = AssetDatabase.LoadAssetAtPath<Sprite>(WallSpritePath);

        GameObject root = new GameObject("Prototype_Lab");
        GameObject cameraObject = CreateCamera(root.transform);
        CreateLighting(root.transform);

        GameObject player = CreatePlayer(root.transform, playerSprite, out Transform firePoint, out PlayerShooting2D playerShooting);
        GameObject bulletPrefab = CreateBulletPrefab(bulletSprite);
        AssignObjectReference(playerShooting, "firePoint", firePoint);
        AssignObjectReference(playerShooting, "bulletPrefab", bulletPrefab);

        CameraFollow2D cameraFollow = cameraObject.GetComponent<CameraFollow2D>();
        AssignObjectReference(cameraFollow, "target", player.transform);

        CreateEnemy(root.transform, enemySprite);
        CreateWalls(root.transform, wallSprite);

        EditorSceneManager.SaveScene(scene, ScenePath);
        AddSceneToBuildSettings(ScenePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Created Prototype 0.1 scene at {ScenePath}.");
    }

    [MenuItem("Tools/Cod Evadare/Create Prototype 0.2 Room Scene")]
    public static void CreatePrototypeRoomScene()
    {
        if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return;
        }

        CreateRequiredFolders();
        GeneratePlaceholderSprites();
        EnsureTag(PlayerTag);

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        SceneManager.SetActiveScene(scene);

        Sprite playerSprite = AssetDatabase.LoadAssetAtPath<Sprite>(PlayerSpritePath);
        Sprite enemySprite = AssetDatabase.LoadAssetAtPath<Sprite>(EnemySpritePath);
        Sprite bulletSprite = AssetDatabase.LoadAssetAtPath<Sprite>(BulletSpritePath);
        Sprite wallSprite = AssetDatabase.LoadAssetAtPath<Sprite>(WallSpritePath);
        Sprite rewardSprite = AssetDatabase.LoadAssetAtPath<Sprite>(RewardSpritePath);

        GameObject root = new GameObject("Prototype_RoomLoop");
        GameObject cameraObject = CreateCamera(root.transform);
        CreateLighting(root.transform);

        GameObject player = CreatePlayer(root.transform, playerSprite, out Transform firePoint, out PlayerShooting2D playerShooting);
        player.transform.position = new Vector3(-9f, 0f, 0f);

        GameObject bulletPrefab = GetOrCreateBulletPrefab(bulletSprite);
        AssignObjectReference(playerShooting, "firePoint", firePoint);
        AssignObjectReference(playerShooting, "bulletPrefab", bulletPrefab);

        CameraFollow2D cameraFollow = cameraObject.GetComponent<CameraFollow2D>();
        AssignObjectReference(cameraFollow, "target", player.transform);

        GameObject enemyPrefab = CreateEnemyPrefab(enemySprite);
        GameObject rewardPrefab = CreateRewardPrefab(rewardSprite);
        CreateRoomLoopRoom(root.transform, wallSprite, enemyPrefab, rewardPrefab);

        EditorSceneManager.SaveScene(scene, RoomLoopScenePath);
        AddSceneToBuildSettings(RoomLoopScenePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Created Prototype 0.2 room loop scene at {RoomLoopScenePath}.");
    }

    [MenuItem("Tools/Cod Evadare/Create Prototype 0.3 Health Combat Scene")]
    public static void CreatePrototypeHealthCombatScene()
    {
        if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return;
        }

        CreateRequiredFolders();
        GeneratePlaceholderSprites();
        EnsureTag(PlayerTag);

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        SceneManager.SetActiveScene(scene);

        Sprite playerSprite = AssetDatabase.LoadAssetAtPath<Sprite>(PlayerSpritePath);
        Sprite enemySprite = AssetDatabase.LoadAssetAtPath<Sprite>(EnemySpritePath);
        Sprite bulletSprite = AssetDatabase.LoadAssetAtPath<Sprite>(BulletSpritePath);
        Sprite wallSprite = AssetDatabase.LoadAssetAtPath<Sprite>(WallSpritePath);
        Sprite rewardSprite = AssetDatabase.LoadAssetAtPath<Sprite>(RewardSpritePath);

        GameObject root = new GameObject("Prototype_HealthCombat");
        GameObject cameraObject = CreateCamera(root.transform);
        CreateLighting(root.transform);

        GameObject player = CreatePlayer(root.transform, playerSprite, out Transform firePoint, out PlayerShooting2D playerShooting);
        player.transform.position = new Vector3(-9f, 0f, 0f);

        PlayerHealth2D playerHealth = player.AddComponent<PlayerHealth2D>();
        AssignInt(playerHealth, "maxHealth", 5);
        AssignFloat(playerHealth, "invincibilityDuration", 0.75f);
        AssignBool(playerHealth, "destroyOnDeath", false);

        GameObject bulletPrefab = GetOrCreateBulletPrefab(bulletSprite);
        AssignObjectReference(playerShooting, "firePoint", firePoint);
        AssignObjectReference(playerShooting, "bulletPrefab", bulletPrefab);

        CameraFollow2D cameraFollow = cameraObject.GetComponent<CameraFollow2D>();
        AssignObjectReference(cameraFollow, "target", player.transform);

        GameObject enemyPrefab = CreateEnemyPrefab(enemySprite);
        GameObject rewardPrefab = CreateRewardPrefab(rewardSprite);
        CreateRoomLoopRoom(root.transform, wallSprite, enemyPrefab, rewardPrefab);

        GameObject gameOverPanel = CreateHealthCombatUI(root.transform, playerHealth);
        CreateGameSystems(root.transform, playerHealth, gameOverPanel);

        EditorSceneManager.SaveScene(scene, HealthCombatScenePath);
        AddSceneToBuildSettings(HealthCombatScenePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Created Prototype 0.3 health combat scene at {HealthCombatScenePath}.");
    }

    [MenuItem("Tools/Cod Evadare/Create Prototype 0.4 Loot Resources Scene")]
    public static void CreatePrototypeLootResourcesScene()
    {
        if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return;
        }

        CreateRequiredFolders();
        GeneratePlaceholderSprites();
        EnsureTag(PlayerTag);

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        SceneManager.SetActiveScene(scene);

        Sprite playerSprite = AssetDatabase.LoadAssetAtPath<Sprite>(PlayerSpritePath);
        Sprite enemySprite = AssetDatabase.LoadAssetAtPath<Sprite>(EnemySpritePath);
        Sprite bulletSprite = AssetDatabase.LoadAssetAtPath<Sprite>(BulletSpritePath);
        Sprite wallSprite = AssetDatabase.LoadAssetAtPath<Sprite>(WallSpritePath);
        Sprite healthPickupSprite = AssetDatabase.LoadAssetAtPath<Sprite>(HealthPickupSpritePath);
        Sprite ammoPickupSprite = AssetDatabase.LoadAssetAtPath<Sprite>(AmmoPickupSpritePath);
        Sprite moneyPickupSprite = AssetDatabase.LoadAssetAtPath<Sprite>(MoneyPickupSpritePath);

        GameObject root = new GameObject("Prototype_LootResources");
        GameObject cameraObject = CreateCamera(root.transform);
        CreateLighting(root.transform);

        GameObject player = CreatePlayer(root.transform, playerSprite, out Transform firePoint, out PlayerShooting2D playerShooting);
        player.transform.position = new Vector3(-9f, 0f, 0f);

        PlayerHealth2D playerHealth = player.AddComponent<PlayerHealth2D>();
        AssignInt(playerHealth, "maxHealth", 5);
        AssignFloat(playerHealth, "invincibilityDuration", 0.75f);
        AssignBool(playerHealth, "destroyOnDeath", false);

        PlayerResources2D playerResources = player.AddComponent<PlayerResources2D>();
        AssignInt(playerResources, "startingAmmo", 30);
        AssignInt(playerResources, "maxAmmo", 99);
        AssignInt(playerResources, "startingMoney", 0);

        GameObject bulletPrefab = GetOrCreateBulletPrefab(bulletSprite);
        AssignObjectReference(playerShooting, "firePoint", firePoint);
        AssignObjectReference(playerShooting, "bulletPrefab", bulletPrefab);
        AssignBool(playerShooting, "useAmmo", true);
        AssignInt(playerShooting, "ammoPerShot", 1);
        AssignObjectReference(playerShooting, "playerResources", playerResources);
        AssignInt(playerShooting, "magazineSize", 12);
        AssignInt(playerShooting, "currentMagazineAmmo", 12);
        AssignFloat(playerShooting, "reloadDuration", 1f);
        AssignEnumByName(playerShooting, "reloadKey", nameof(KeyCode.R));
        AssignBool(playerShooting, "autoReloadWhenEmpty", false);

        CameraFollow2D cameraFollow = cameraObject.GetComponent<CameraFollow2D>();
        AssignObjectReference(cameraFollow, "target", player.transform);

        GameObject enemyPrefab = CreateEnemyPrefab(enemySprite);
        GameObject[] lootPrefabs =
        {
            CreateResourcePickupPrefab("HealthPickup", HealthPickupPrefabPath, healthPickupSprite, ResourcePickupType.Health, 2),
            CreateResourcePickupPrefab("AmmoPickup", AmmoPickupPrefabPath, ammoPickupSprite, ResourcePickupType.Ammo, 10),
            CreateResourcePickupPrefab("MoneyPickup", MoneyPickupPrefabPath, moneyPickupSprite, ResourcePickupType.Money, 25)
        };

        CreateLootResourcesRoom(root.transform, wallSprite, enemyPrefab, lootPrefabs);

        GameObject gameOverPanel = CreateLootResourcesUI(root.transform, playerHealth, playerResources, playerShooting);
        CreateGameSystems(root.transform, playerHealth, gameOverPanel);

        EditorSceneManager.SaveScene(scene, LootResourcesScenePath);
        AddSceneToBuildSettings(LootResourcesScenePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Created Prototype 0.4 loot resources scene at {LootResourcesScenePath}.");
    }

    private static void CreateRequiredFolders()
    {
        foreach (string folder in RequiredFolders)
        {
            Directory.CreateDirectory(ToAbsoluteAssetPath(folder));
        }

        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
    }

    private static void GeneratePlaceholderSprites()
    {
        WriteSpriteTexture(PlayerSpritePath, 64, CreatePlayerPixel);
        WriteSpriteTexture(EnemySpritePath, 64, CreateEnemyPixel);
        WriteSpriteTexture(BulletSpritePath, 32, CreateBulletPixel);
        WriteSpriteTexture(WallSpritePath, 64, CreateWallPixel);
        WriteSpriteTexture(RewardSpritePath, 64, CreateRewardPixel);
        WriteSpriteTexture(HealthPickupSpritePath, 64, CreateHealthPickupPixel);
        WriteSpriteTexture(AmmoPickupSpritePath, 64, CreateAmmoPickupPixel);
        WriteSpriteTexture(MoneyPickupSpritePath, 64, CreateMoneyPickupPixel);
    }

    private static GameObject CreateCamera(Transform parent)
    {
        GameObject cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
        cameraObject.transform.SetParent(parent);
        cameraObject.transform.position = new Vector3(0f, 0f, -10f);
        cameraObject.tag = "MainCamera";

        Camera camera = cameraObject.GetComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 6.5f;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.06f, 0.07f, 0.08f);

        CameraFollow2D cameraFollow = cameraObject.AddComponent<CameraFollow2D>();
        AssignFloat(cameraFollow, "smoothSpeed", 8f);
        AssignVector3(cameraFollow, "offset", Vector3.zero);

        return cameraObject;
    }

    private static void CreateLighting(Transform parent)
    {
        GameObject lighting = new GameObject("Lighting");
        lighting.transform.SetParent(parent);

        GameObject globalLightObject = new GameObject("Global Light 2D");
        globalLightObject.transform.SetParent(lighting.transform);

        Type light2DType = FindType("UnityEngine.Rendering.Universal.Light2D");

        if (light2DType == null)
        {
            Debug.LogWarning("UnityEngine.Rendering.Universal.Light2D was not available. Skipping Global Light 2D component setup.");
            return;
        }

        Component lightComponent = globalLightObject.AddComponent(light2DType);
        ConfigureLight2D(lightComponent);
    }

    private static GameObject CreatePlayer(Transform parent, Sprite sprite, out Transform firePoint, out PlayerShooting2D playerShooting)
    {
        GameObject player = new GameObject("Player");
        player.transform.SetParent(parent);
        player.transform.position = Vector3.zero;
        player.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
        player.tag = PlayerTag;

        SpriteRenderer spriteRenderer = player.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = 10;

        Rigidbody2D body = player.AddComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        body.freezeRotation = true;

        player.AddComponent<BoxCollider2D>();

        PlayerMovement2D movement = player.AddComponent<PlayerMovement2D>();
        AssignFloat(movement, "moveSpeed", 5f);

        playerShooting = player.AddComponent<PlayerShooting2D>();
        AssignFloat(playerShooting, "fireCooldown", 0.15f);

        GameObject firePointObject = new GameObject("FirePoint");
        firePointObject.transform.SetParent(player.transform);
        firePointObject.transform.localPosition = new Vector3(0.6f, 0f, 0f);
        firePointObject.transform.localRotation = Quaternion.identity;
        firePointObject.transform.localScale = Vector3.one;
        firePoint = firePointObject.transform;

        return player;
    }

    private static GameObject CreateBulletPrefab(Sprite sprite)
    {
        GameObject bullet = new GameObject("Bullet");
        bullet.transform.localScale = new Vector3(0.15f, 0.15f, 1f);

        SpriteRenderer spriteRenderer = bullet.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = 20;

        Rigidbody2D body = bullet.AddComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        CircleCollider2D collider = bullet.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;

        Bullet2D bulletComponent = bullet.AddComponent<Bullet2D>();
        AssignFloat(bulletComponent, "speed", 12f);
        AssignFloat(bulletComponent, "lifetime", 2f);
        AssignInt(bulletComponent, "damage", 1);

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(bullet, BulletPrefabPath);
        UnityEngine.Object.DestroyImmediate(bullet);

        if (prefab == null)
        {
            prefab = AssetDatabase.LoadAssetAtPath<GameObject>(BulletPrefabPath);
        }

        return prefab;
    }

    private static GameObject GetOrCreateBulletPrefab(Sprite sprite)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(BulletPrefabPath);
        return prefab != null ? prefab : CreateBulletPrefab(sprite);
    }

    private static GameObject CreateEnemyPrefab(Sprite sprite)
    {
        GameObject enemy = new GameObject("TestEnemy");
        enemy.transform.localScale = new Vector3(0.8f, 0.8f, 1f);

        SpriteRenderer spriteRenderer = enemy.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = 10;

        Rigidbody2D body = enemy.AddComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;
        body.freezeRotation = true;

        enemy.AddComponent<BoxCollider2D>();

        EnemyHealth health = enemy.AddComponent<EnemyHealth>();
        AssignInt(health, "maxHealth", 3);

        SimpleEnemyChaser2D chaser = enemy.AddComponent<SimpleEnemyChaser2D>();
        AssignFloat(chaser, "moveSpeed", 1.5f);
        AssignFloat(chaser, "stopDistance", 1f);

        EnemyContactDamage2D contactDamage = enemy.AddComponent<EnemyContactDamage2D>();
        AssignInt(contactDamage, "damage", 1);
        AssignFloat(contactDamage, "damageCooldown", 1f);

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(enemy, EnemyPrefabPath);
        UnityEngine.Object.DestroyImmediate(enemy);

        if (prefab == null)
        {
            prefab = AssetDatabase.LoadAssetAtPath<GameObject>(EnemyPrefabPath);
        }

        return prefab;
    }

    private static GameObject CreateRewardPrefab(Sprite sprite)
    {
        GameObject reward = new GameObject("PrototypeReward");
        reward.transform.localScale = new Vector3(0.45f, 0.45f, 1f);

        SpriteRenderer spriteRenderer = reward.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = 15;

        CircleCollider2D collider = reward.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;

        RewardPickup2D pickup = reward.AddComponent<RewardPickup2D>();
        AssignString(pickup, "rewardName", "Prototype Reward");

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(reward, RewardPrefabPath);
        UnityEngine.Object.DestroyImmediate(reward);

        if (prefab == null)
        {
            prefab = AssetDatabase.LoadAssetAtPath<GameObject>(RewardPrefabPath);
        }

        return prefab;
    }

    private static GameObject CreateResourcePickupPrefab(string pickupName, string prefabPath, Sprite sprite, ResourcePickupType pickupType, int amount)
    {
        GameObject pickupObject = new GameObject(pickupName);
        pickupObject.transform.localScale = new Vector3(0.45f, 0.45f, 1f);

        SpriteRenderer spriteRenderer = pickupObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = 15;

        CircleCollider2D collider = pickupObject.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;

        ResourcePickup2D pickup = pickupObject.AddComponent<ResourcePickup2D>();
        AssignEnum(pickup, "pickupType", (int)pickupType);
        AssignInt(pickup, "amount", amount);
        AssignBool(pickup, "destroyOnCollect", true);

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(pickupObject, prefabPath);
        UnityEngine.Object.DestroyImmediate(pickupObject);

        if (prefab == null)
        {
            prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        }

        return prefab;
    }

    private static void CreateEnemy(Transform parent, Sprite sprite)
    {
        GameObject enemy = new GameObject("TestEnemy");
        enemy.transform.SetParent(parent);
        enemy.transform.position = new Vector3(3f, 0f, 0f);
        enemy.transform.localScale = new Vector3(0.8f, 0.8f, 1f);

        SpriteRenderer spriteRenderer = enemy.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = 10;

        Rigidbody2D body = enemy.AddComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;
        body.freezeRotation = true;

        enemy.AddComponent<BoxCollider2D>();

        EnemyHealth health = enemy.AddComponent<EnemyHealth>();
        AssignInt(health, "maxHealth", 3);

        SimpleEnemyChaser2D chaser = enemy.AddComponent<SimpleEnemyChaser2D>();
        AssignFloat(chaser, "moveSpeed", 1.5f);
        AssignFloat(chaser, "stopDistance", 1f);
    }

    private static void CreateWalls(Transform parent, Sprite sprite)
    {
        GameObject walls = new GameObject("Walls");
        walls.transform.SetParent(parent);

        CreateWall(walls.transform, "Wall_Top", sprite, new Vector3(0f, 4f, 0f), new Vector3(14.5f, 0.4f, 1f));
        CreateWall(walls.transform, "Wall_Bottom", sprite, new Vector3(0f, -4f, 0f), new Vector3(14.5f, 0.4f, 1f));
        CreateWall(walls.transform, "Wall_Left", sprite, new Vector3(-7f, 0f, 0f), new Vector3(0.4f, 8.4f, 1f));
        CreateWall(walls.transform, "Wall_Right", sprite, new Vector3(7f, 0f, 0f), new Vector3(0.4f, 8.4f, 1f));
    }

    private static void CreateWall(Transform parent, string name, Sprite sprite, Vector3 position, Vector3 scale)
    {
        GameObject wall = new GameObject(name);
        wall.transform.SetParent(parent);
        wall.transform.position = position;
        wall.transform.localScale = scale;

        SpriteRenderer spriteRenderer = wall.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = 0;

        wall.AddComponent<BoxCollider2D>();
    }

    private static void CreateRoomLoopWalls(Transform parent, Sprite sprite, out DoorController2D leftDoor, out DoorController2D rightDoor)
    {
        GameObject walls = new GameObject("Walls");
        walls.transform.SetParent(parent);

        CreateWall(walls.transform, "Wall_Top", sprite, new Vector3(0f, 4f, 0f), new Vector3(14.5f, 0.4f, 1f));
        CreateWall(walls.transform, "Wall_Bottom", sprite, new Vector3(0f, -4f, 0f), new Vector3(14.5f, 0.4f, 1f));
        CreateWallWithDoorGap(walls.transform, "Wall_Left", sprite, -7f);
        CreateWallWithDoorGap(walls.transform, "Wall_Right", sprite, 7f);

        GameObject doors = new GameObject("Doors");
        doors.transform.SetParent(parent);

        leftDoor = CreateDoor(doors.transform, "Door_Left", sprite, new Vector3(-7f, 0f, 0f));
        rightDoor = CreateDoor(doors.transform, "Door_Right", sprite, new Vector3(7f, 0f, 0f));
    }

    private static void CreateRoomLoopRoom(Transform parent, Sprite wallSprite, GameObject enemyPrefab, GameObject rewardPrefab)
    {
        GameObject room = new GameObject("Room_01");
        room.transform.SetParent(parent);

        CreateRoomLoopWalls(room.transform, wallSprite, out DoorController2D leftDoor, out DoorController2D rightDoor);

        EnemySpawner2D enemySpawner = CreateEnemySpawner(room.transform, enemyPrefab);
        Transform rewardSpawnPoint = CreateMarker(room.transform, "RewardSpawnPoint", new Vector3(4.5f, 0f, 0f));

        RoomController2D roomController = room.AddComponent<RoomController2D>();
        AssignObjectReferenceArray(roomController, "doors", new[] { leftDoor, rightDoor });
        AssignObjectReference(roomController, "enemySpawner", enemySpawner);
        AssignObjectReference(roomController, "rewardPrefab", rewardPrefab);
        AssignObjectReference(roomController, "rewardSpawnPoint", rewardSpawnPoint);
        AssignFloat(roomController, "doorCloseDelay", 0.5f);

        CreateRoomTrigger(room.transform, roomController);
    }

    private static void CreateLootResourcesRoom(Transform parent, Sprite wallSprite, GameObject enemyPrefab, GameObject[] lootPrefabs)
    {
        GameObject room = new GameObject("Room_01");
        room.transform.SetParent(parent);

        CreateRoomLoopWalls(room.transform, wallSprite, out DoorController2D leftDoor, out DoorController2D rightDoor);

        EnemySpawner2D enemySpawner = CreateEnemySpawner(room.transform, enemyPrefab);
        RoomLootSpawner2D lootSpawner = CreateRoomLootSpawner(room.transform, lootPrefabs);

        RoomController2D roomController = room.AddComponent<RoomController2D>();
        AssignObjectReferenceArray(roomController, "doors", new[] { leftDoor, rightDoor });
        AssignObjectReference(roomController, "enemySpawner", enemySpawner);
        AssignObjectReference(roomController, "lootSpawner", lootSpawner);
        AssignFloat(roomController, "doorCloseDelay", 0.5f);

        CreateRoomTrigger(room.transform, roomController);
    }

    private static void CreateWallWithDoorGap(Transform parent, string name, Sprite sprite, float x)
    {
        GameObject wall = new GameObject(name);
        wall.transform.SetParent(parent);

        CreateWallSegment(wall.transform, name + "_Upper", sprite, new Vector3(x, 2.7f, 0f), new Vector3(0.4f, 2.6f, 1f));
        CreateWallSegment(wall.transform, name + "_Lower", sprite, new Vector3(x, -2.7f, 0f), new Vector3(0.4f, 2.6f, 1f));
    }

    private static void CreateWallSegment(Transform parent, string name, Sprite sprite, Vector3 position, Vector3 scale)
    {
        GameObject segment = new GameObject(name);
        segment.transform.SetParent(parent);
        segment.transform.position = position;
        segment.transform.localScale = scale;

        SpriteRenderer spriteRenderer = segment.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = 0;

        segment.AddComponent<BoxCollider2D>();
    }

    private static DoorController2D CreateDoor(Transform parent, string name, Sprite sprite, Vector3 position)
    {
        GameObject door = new GameObject(name);
        door.transform.SetParent(parent);
        door.transform.position = position;
        door.transform.localScale = new Vector3(0.4f, 2.8f, 1f);

        SpriteRenderer spriteRenderer = door.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = 5;
        spriteRenderer.enabled = false;

        BoxCollider2D collider = door.AddComponent<BoxCollider2D>();
        collider.enabled = false;

        DoorController2D doorController = door.AddComponent<DoorController2D>();
        AssignObjectReference(doorController, "spriteRenderer", spriteRenderer);
        AssignObjectReference(doorController, "doorCollider", collider);

        return doorController;
    }

    private static EnemySpawner2D CreateEnemySpawner(Transform parent, GameObject enemyPrefab)
    {
        GameObject spawnerObject = new GameObject("EnemySpawner");
        spawnerObject.transform.SetParent(parent);

        Transform[] spawnPoints =
        {
            CreateMarker(spawnerObject.transform, "SpawnPoint_01", new Vector3(1.5f, 1.5f, 0f)),
            CreateMarker(spawnerObject.transform, "SpawnPoint_02", new Vector3(3f, 0f, 0f)),
            CreateMarker(spawnerObject.transform, "SpawnPoint_03", new Vector3(1.5f, -1.5f, 0f))
        };

        EnemySpawner2D spawner = spawnerObject.AddComponent<EnemySpawner2D>();
        AssignObjectReference(spawner, "enemyPrefab", enemyPrefab);
        AssignObjectReferenceArray(spawner, "spawnPoints", spawnPoints);
        AssignInt(spawner, "enemyCount", 3);

        return spawner;
    }

    private static RoomLootSpawner2D CreateRoomLootSpawner(Transform parent, GameObject[] lootPrefabs)
    {
        GameObject lootSpawnerObject = new GameObject("LootSpawner");
        lootSpawnerObject.transform.SetParent(parent);

        Transform[] spawnPoints =
        {
            CreateMarker(lootSpawnerObject.transform, "LootSpawnPoint_Health", new Vector3(3.8f, 1.1f, 0f)),
            CreateMarker(lootSpawnerObject.transform, "LootSpawnPoint_Ammo", new Vector3(4.5f, 0f, 0f)),
            CreateMarker(lootSpawnerObject.transform, "LootSpawnPoint_Money", new Vector3(3.8f, -1.1f, 0f))
        };

        RoomLootSpawner2D lootSpawner = lootSpawnerObject.AddComponent<RoomLootSpawner2D>();
        AssignObjectReferenceArray(lootSpawner, "lootPrefabs", lootPrefabs);
        AssignObjectReferenceArray(lootSpawner, "spawnPoints", spawnPoints);
        AssignBool(lootSpawner, "spawnAll", true);
        AssignInt(lootSpawner, "randomSpawnCount", 3);

        return lootSpawner;
    }

    private static RoomController2D CreateRoomController(
        Transform parent,
        DoorController2D[] doors,
        EnemySpawner2D enemySpawner,
        GameObject rewardPrefab,
        Transform rewardSpawnPoint)
    {
        GameObject roomControllerObject = new GameObject("RoomController2D");
        roomControllerObject.transform.SetParent(parent);

        RoomController2D roomController = roomControllerObject.AddComponent<RoomController2D>();
        AssignObjectReferenceArray(roomController, "doors", doors);
        AssignObjectReference(roomController, "enemySpawner", enemySpawner);
        AssignObjectReference(roomController, "rewardPrefab", rewardPrefab);
        AssignObjectReference(roomController, "rewardSpawnPoint", rewardSpawnPoint);
        AssignFloat(roomController, "doorCloseDelay", 0.5f);

        return roomController;
    }

    private static void CreateRoomTrigger(Transform parent, RoomController2D roomController)
    {
        GameObject trigger = new GameObject("RoomTrigger");
        trigger.transform.SetParent(parent);
        trigger.transform.position = Vector3.zero;

        BoxCollider2D collider = trigger.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(12.5f, 6.5f);

        RoomTrigger2D roomTrigger = trigger.AddComponent<RoomTrigger2D>();
        AssignObjectReference(roomTrigger, "roomController", roomController);
    }

    private static Transform CreateMarker(Transform parent, string name, Vector3 position)
    {
        GameObject marker = new GameObject(name);
        marker.transform.SetParent(parent);
        marker.transform.position = position;
        marker.transform.localRotation = Quaternion.identity;
        marker.transform.localScale = Vector3.one;

        return marker.transform;
    }

    private static GameObject CreateHealthCombatUI(Transform parent, PlayerHealth2D playerHealth)
    {
        GameObject ui = new GameObject("UI");
        ui.transform.SetParent(parent);

        GameObject canvasObject = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.transform.SetParent(ui.transform);

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler canvasScaler = canvasObject.GetComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1280f, 720f);
        canvasScaler.matchWidthOrHeight = 0.5f;

        GameObject healthPanel = CreateRectObject("HealthPanel", canvasObject.transform);
        SetRectTransform(healthPanel.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(24f, -24f), new Vector2(220f, 42f));

        Image healthPanelImage = healthPanel.AddComponent<Image>();
        healthPanelImage.color = new Color(0f, 0f, 0f, 0.65f);

        GameObject healthFillObject = CreateRectObject("HealthFill", healthPanel.transform);
        StretchRectTransform(healthFillObject.GetComponent<RectTransform>(), new Vector2(4f, 4f), new Vector2(-4f, -4f));

        Image healthFill = healthFillObject.AddComponent<Image>();
        healthFill.color = new Color(0.18f, 0.85f, 0.32f, 0.9f);
        healthFill.type = Image.Type.Filled;
        healthFill.fillMethod = Image.FillMethod.Horizontal;
        healthFill.fillOrigin = (int)Image.OriginHorizontal.Left;
        healthFill.fillAmount = 1f;

        GameObject healthTextObject = CreateRectObject("HealthText", healthPanel.transform);
        StretchRectTransform(healthTextObject.GetComponent<RectTransform>(), Vector2.zero, Vector2.zero);

        Text healthText = healthTextObject.AddComponent<Text>();
        healthText.font = GetBuiltinUIFont();
        healthText.text = "HP: 5 / 5";
        healthText.fontSize = 18;
        healthText.alignment = TextAnchor.MiddleCenter;
        healthText.color = Color.white;
        healthText.raycastTarget = false;

        HealthUI2D healthUI = healthPanel.AddComponent<HealthUI2D>();
        AssignObjectReference(healthUI, "playerHealth", playerHealth);
        AssignObjectReference(healthUI, "healthFill", healthFill);
        AssignObjectReference(healthUI, "healthText", healthText);

        GameObject gameOverPanel = CreateRectObject("GameOverPanel", canvasObject.transform);
        SetRectTransform(gameOverPanel.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(360f, 160f));

        Image gameOverBackground = gameOverPanel.AddComponent<Image>();
        gameOverBackground.color = new Color(0f, 0f, 0f, 0.82f);

        GameObject gameOverTextObject = CreateRectObject("GameOverText", gameOverPanel.transform);
        StretchRectTransform(gameOverTextObject.GetComponent<RectTransform>(), new Vector2(16f, 16f), new Vector2(-16f, -16f));

        Text gameOverText = gameOverTextObject.AddComponent<Text>();
        gameOverText.font = GetBuiltinUIFont();
        gameOverText.text = "GAME OVER\nPress R to restart";
        gameOverText.fontSize = 30;
        gameOverText.alignment = TextAnchor.MiddleCenter;
        gameOverText.color = Color.white;
        gameOverText.raycastTarget = false;

        gameOverPanel.SetActive(false);
        return gameOverPanel;
    }

    private static GameObject CreateLootResourcesUI(Transform parent, PlayerHealth2D playerHealth, PlayerResources2D playerResources, PlayerShooting2D playerShooting)
    {
        GameObject ui = new GameObject("UI");
        ui.transform.SetParent(parent);

        GameObject canvasObject = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.transform.SetParent(ui.transform);

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler canvasScaler = canvasObject.GetComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1280f, 720f);
        canvasScaler.matchWidthOrHeight = 0.5f;

        GameObject healthPanel = CreateRectObject("HealthPanel", canvasObject.transform);
        SetRectTransform(healthPanel.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(24f, -24f), new Vector2(220f, 42f));

        Image healthPanelImage = healthPanel.AddComponent<Image>();
        healthPanelImage.color = new Color(0f, 0f, 0f, 0.65f);

        GameObject healthFillObject = CreateRectObject("HealthFill", healthPanel.transform);
        StretchRectTransform(healthFillObject.GetComponent<RectTransform>(), new Vector2(4f, 4f), new Vector2(-4f, -4f));

        Image healthFill = healthFillObject.AddComponent<Image>();
        healthFill.color = new Color(0.18f, 0.85f, 0.32f, 0.9f);
        healthFill.type = Image.Type.Filled;
        healthFill.fillMethod = Image.FillMethod.Horizontal;
        healthFill.fillOrigin = (int)Image.OriginHorizontal.Left;
        healthFill.fillAmount = 1f;

        GameObject healthTextObject = CreateRectObject("HealthText", healthPanel.transform);
        StretchRectTransform(healthTextObject.GetComponent<RectTransform>(), Vector2.zero, Vector2.zero);

        Text healthText = healthTextObject.AddComponent<Text>();
        healthText.font = GetBuiltinUIFont();
        healthText.text = "HP: 5 / 5";
        healthText.fontSize = 18;
        healthText.alignment = TextAnchor.MiddleCenter;
        healthText.color = Color.white;
        healthText.raycastTarget = false;

        HealthUI2D healthUI = healthPanel.AddComponent<HealthUI2D>();
        AssignObjectReference(healthUI, "playerHealth", playerHealth);
        AssignObjectReference(healthUI, "healthFill", healthFill);
        AssignObjectReference(healthUI, "healthText", healthText);

        GameObject resourcePanel = CreateRectObject("ResourcePanel", canvasObject.transform);
        SetRectTransform(resourcePanel.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(24f, -76f), new Vector2(430f, 64f));

        Image resourcePanelImage = resourcePanel.AddComponent<Image>();
        resourcePanelImage.color = new Color(0f, 0f, 0f, 0.65f);

        GameObject ammoTextObject = CreateRectObject("AmmoText", resourcePanel.transform);
        SetRectTransform(ammoTextObject.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -8f), new Vector2(-16f, 24f));

        Text ammoText = ammoTextObject.AddComponent<Text>();
        ammoText.font = GetBuiltinUIFont();
        ammoText.text = "Ammo: 12 / 12 | Reserve: 30 / 99";
        ammoText.fontSize = 16;
        ammoText.alignment = TextAnchor.MiddleLeft;
        ammoText.color = Color.white;
        ammoText.raycastTarget = false;

        GameObject moneyTextObject = CreateRectObject("MoneyText", resourcePanel.transform);
        SetRectTransform(moneyTextObject.GetComponent<RectTransform>(), new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 8f), new Vector2(-16f, 24f));

        Text moneyText = moneyTextObject.AddComponent<Text>();
        moneyText.font = GetBuiltinUIFont();
        moneyText.text = "Money: 0";
        moneyText.fontSize = 18;
        moneyText.alignment = TextAnchor.MiddleLeft;
        moneyText.color = Color.white;
        moneyText.raycastTarget = false;

        ResourceUI2D resourceUI = resourcePanel.AddComponent<ResourceUI2D>();
        AssignObjectReference(resourceUI, "playerResources", playerResources);
        AssignObjectReference(resourceUI, "playerShooting", playerShooting);
        AssignObjectReference(resourceUI, "ammoText", ammoText);
        AssignObjectReference(resourceUI, "moneyText", moneyText);

        GameObject gameOverPanel = CreateRectObject("GameOverPanel", canvasObject.transform);
        SetRectTransform(gameOverPanel.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(360f, 160f));

        Image gameOverBackground = gameOverPanel.AddComponent<Image>();
        gameOverBackground.color = new Color(0f, 0f, 0f, 0.82f);

        GameObject gameOverTextObject = CreateRectObject("GameOverText", gameOverPanel.transform);
        StretchRectTransform(gameOverTextObject.GetComponent<RectTransform>(), new Vector2(16f, 16f), new Vector2(-16f, -16f));

        Text gameOverText = gameOverTextObject.AddComponent<Text>();
        gameOverText.font = GetBuiltinUIFont();
        gameOverText.text = "GAME OVER\nPress R to restart";
        gameOverText.fontSize = 30;
        gameOverText.alignment = TextAnchor.MiddleCenter;
        gameOverText.color = Color.white;
        gameOverText.raycastTarget = false;

        gameOverPanel.SetActive(false);
        return gameOverPanel;
    }

    private static void CreateGameSystems(Transform parent, PlayerHealth2D playerHealth, GameObject gameOverPanel)
    {
        GameObject gameSystems = new GameObject("GameSystems");
        gameSystems.transform.SetParent(parent);

        GameObject gameOverControllerObject = new GameObject("GameOverController");
        gameOverControllerObject.transform.SetParent(gameSystems.transform);

        GameOverController2D gameOverController = gameOverControllerObject.AddComponent<GameOverController2D>();
        AssignObjectReference(gameOverController, "playerHealth", playerHealth);
        AssignObjectReference(gameOverController, "gameOverPanel", gameOverPanel);
        AssignString(gameOverController, "restartKey", "r");
    }

    private static GameObject CreateRectObject(string name, Transform parent)
    {
        GameObject gameObject = new GameObject(name, typeof(RectTransform));
        gameObject.transform.SetParent(parent, false);
        return gameObject;
    }

    private static void SetRectTransform(RectTransform rectTransform, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPosition, Vector2 sizeDelta)
    {
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.pivot = pivot;
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = sizeDelta;
        rectTransform.localScale = Vector3.one;
    }

    private static void StretchRectTransform(RectTransform rectTransform, Vector2 offsetMin, Vector2 offsetMax)
    {
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.offsetMin = offsetMin;
        rectTransform.offsetMax = offsetMax;
        rectTransform.localScale = Vector3.one;
    }

    private static Font GetBuiltinUIFont()
    {
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        return font != null ? font : Resources.GetBuiltinResource<Font>("Arial.ttf");
    }

    private static void ConfigureLight2D(Component lightComponent)
    {
        try
        {
            Type lightType = lightComponent.GetType();
            Type enumType = lightType.GetNestedType("LightType");

            if (enumType != null)
            {
                object globalLightType = Enum.Parse(enumType, "Global");
                SetReflectedProperty(lightComponent, "lightType", globalLightType);
            }

            SetReflectedProperty(lightComponent, "intensity", 1f);
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"Global Light 2D was created, but its settings could not be configured: {exception.Message}");
        }
    }

    private static void WriteSpriteTexture(string assetPath, int size, Func<int, int, int, Color> pixelFactory)
    {
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                texture.SetPixel(x, y, pixelFactory(x, y, size));
            }
        }

        texture.Apply();

        string absolutePath = ToAbsoluteAssetPath(assetPath);
        string directory = Path.GetDirectoryName(absolutePath);

        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllBytes(absolutePath, texture.EncodeToPNG());
        UnityEngine.Object.DestroyImmediate(texture);

        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
        ConfigureSpriteImporter(assetPath);
    }

    private static void ConfigureSpriteImporter(string assetPath)
    {
        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;

        if (importer == null)
        {
            Debug.LogWarning($"Could not configure sprite importer for {assetPath}.");
            return;
        }

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = 64f;
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = true;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.SaveAndReimport();
    }

    private static Color CreatePlayerPixel(int x, int y, int size)
    {
        Color fill = new Color(0.15f, 0.55f, 1f, 1f);
        Color outline = new Color(0.04f, 0.14f, 0.35f, 1f);
        Color front = new Color(0.85f, 0.98f, 1f, 1f);
        Color circle = CirclePixel(x, y, size, fill, outline);

        if (circle.a <= 0f)
        {
            return circle;
        }

        if (x > size * 0.6f && Mathf.Abs(y - size * 0.5f) < (x - size * 0.6f) * 0.45f + 2f)
        {
            return front;
        }

        return circle;
    }

    private static Color CreateEnemyPixel(int x, int y, int size)
    {
        Color fill = new Color(0.95f, 0.2f, 0.18f, 1f);
        Color outline = new Color(0.35f, 0.02f, 0.02f, 1f);
        return CirclePixel(x, y, size, fill, outline);
    }

    private static Color CreateBulletPixel(int x, int y, int size)
    {
        Color fill = new Color(1f, 0.86f, 0.12f, 1f);
        Color outline = new Color(0.85f, 0.42f, 0.04f, 1f);
        return CirclePixel(x, y, size, fill, outline);
    }

    private static Color CreateWallPixel(int x, int y, int size)
    {
        bool border = x < 4 || y < 4 || x >= size - 4 || y >= size - 4;
        bool stripe = (x + y) % 16 < 8;

        if (border)
        {
            return new Color(0.1f, 0.12f, 0.14f, 1f);
        }

        return stripe
            ? new Color(0.38f, 0.42f, 0.46f, 1f)
            : new Color(0.3f, 0.34f, 0.38f, 1f);
    }

    private static Color CreateRewardPixel(int x, int y, int size)
    {
        float halfSize = size * 0.5f;
        float distance = Mathf.Abs(x + 0.5f - halfSize) + Mathf.Abs(y + 0.5f - halfSize);
        float radius = size * 0.42f;

        if (distance > radius)
        {
            return Color.clear;
        }

        if (distance > radius - 4f)
        {
            return new Color(0.95f, 0.8f, 0.1f, 1f);
        }

        return new Color(0.25f, 0.9f, 0.65f, 1f);
    }

    private static Color CreateHealthPickupPixel(int x, int y, int size)
    {
        Color fill = new Color(0.12f, 0.8f, 0.28f, 1f);
        Color outline = new Color(0.02f, 0.28f, 0.08f, 1f);
        Color basePixel = CirclePixel(x, y, size, fill, outline);

        if (basePixel.a <= 0f)
        {
            return basePixel;
        }

        float center = size * 0.5f;
        bool verticalBar = Mathf.Abs(x + 0.5f - center) < size * 0.09f && Mathf.Abs(y + 0.5f - center) < size * 0.28f;
        bool horizontalBar = Mathf.Abs(y + 0.5f - center) < size * 0.09f && Mathf.Abs(x + 0.5f - center) < size * 0.28f;

        return verticalBar || horizontalBar ? Color.white : basePixel;
    }

    private static Color CreateAmmoPickupPixel(int x, int y, int size)
    {
        bool border = x < 7 || y < 10 || x >= size - 7 || y >= size - 10;
        bool body = x >= 10 && x < size - 10 && y >= 14 && y < size - 14;
        bool stripe = body && (x + y) % 12 < 6;

        if (!body && !border)
        {
            return Color.clear;
        }

        if (border)
        {
            return new Color(0.58f, 0.25f, 0.02f, 1f);
        }

        return stripe
            ? new Color(1f, 0.82f, 0.18f, 1f)
            : new Color(0.95f, 0.5f, 0.08f, 1f);
    }

    private static Color CreateMoneyPickupPixel(int x, int y, int size)
    {
        Color fill = new Color(1f, 0.78f, 0.16f, 1f);
        Color outline = new Color(0.55f, 0.32f, 0.04f, 1f);
        Color basePixel = CirclePixel(x, y, size, fill, outline);

        if (basePixel.a <= 0f)
        {
            return basePixel;
        }

        float center = size * 0.5f;
        float dx = Mathf.Abs(x + 0.5f - center);
        float dy = Mathf.Abs(y + 0.5f - center);
        bool coinMark = dx < size * 0.08f && dy < size * 0.25f;

        return coinMark ? new Color(0.55f, 0.32f, 0.04f, 1f) : basePixel;
    }

    private static Color CirclePixel(int x, int y, int size, Color fill, Color outline)
    {
        float halfSize = size * 0.5f;
        float radius = size * 0.42f;
        float distance = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(halfSize, halfSize));

        if (distance > radius)
        {
            return Color.clear;
        }

        if (distance > radius - 3f)
        {
            return outline;
        }

        return fill;
    }

    private static void EnsureTag(string tag)
    {
        foreach (string existingTag in InternalEditorUtility.tags)
        {
            if (existingTag == tag)
            {
                return;
            }
        }

        UnityEngine.Object tagManagerAsset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0];
        SerializedObject tagManager = new SerializedObject(tagManagerAsset);
        SerializedProperty tags = tagManager.FindProperty("tags");

        for (int i = 0; i < tags.arraySize; i++)
        {
            if (tags.GetArrayElementAtIndex(i).stringValue == tag)
            {
                return;
            }
        }

        tags.InsertArrayElementAtIndex(tags.arraySize);
        tags.GetArrayElementAtIndex(tags.arraySize - 1).stringValue = tag;
        tagManager.ApplyModifiedProperties();
    }

    private static void AddSceneToBuildSettings(string scenePath)
    {
        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);

        foreach (EditorBuildSettingsScene scene in scenes)
        {
            if (scene.path == scenePath)
            {
                return;
            }
        }

        scenes.Add(new EditorBuildSettingsScene(scenePath, true));
        EditorBuildSettings.scenes = scenes.ToArray();
    }

    private static Type FindType(string typeName)
    {
        Type type = Type.GetType($"{typeName}, Unity.RenderPipelines.Universal.Runtime");

        if (type != null)
        {
            return type;
        }

        foreach (System.Reflection.Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            type = assembly.GetType(typeName);

            if (type != null)
            {
                return type;
            }
        }

        return null;
    }

    private static void SetReflectedProperty(Component component, string propertyName, object value)
    {
        System.Reflection.PropertyInfo property = component.GetType().GetProperty(propertyName);

        if (property != null && property.CanWrite)
        {
            property.SetValue(component, value, null);
        }
    }

    private static void AssignObjectReference(UnityEngine.Object target, string propertyName, UnityEngine.Object value)
    {
        SerializedProperty property = FindSerializedProperty(target, propertyName);

        if (property == null)
        {
            return;
        }

        property.objectReferenceValue = value;
        property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void AssignFloat(UnityEngine.Object target, string propertyName, float value)
    {
        SerializedProperty property = FindSerializedProperty(target, propertyName);

        if (property == null)
        {
            return;
        }

        property.floatValue = value;
        property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void AssignInt(UnityEngine.Object target, string propertyName, int value)
    {
        SerializedProperty property = FindSerializedProperty(target, propertyName);

        if (property == null)
        {
            return;
        }

        property.intValue = value;
        property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void AssignBool(UnityEngine.Object target, string propertyName, bool value)
    {
        SerializedProperty property = FindSerializedProperty(target, propertyName);

        if (property == null)
        {
            return;
        }

        property.boolValue = value;
        property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void AssignEnum(UnityEngine.Object target, string propertyName, int enumValueIndex)
    {
        SerializedProperty property = FindSerializedProperty(target, propertyName);

        if (property == null)
        {
            return;
        }

        property.enumValueIndex = enumValueIndex;
        property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void AssignEnumByName(UnityEngine.Object target, string propertyName, string enumName)
    {
        SerializedProperty property = FindSerializedProperty(target, propertyName);

        if (property == null)
        {
            return;
        }

        for (int i = 0; i < property.enumNames.Length; i++)
        {
            if (property.enumNames[i] == enumName)
            {
                property.enumValueIndex = i;
                property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                return;
            }
        }

        Debug.LogWarning($"Could not assign enum property '{propertyName}' on {target.name} because value '{enumName}' was not found.");
    }

    private static void AssignString(UnityEngine.Object target, string propertyName, string value)
    {
        SerializedProperty property = FindSerializedProperty(target, propertyName);

        if (property == null)
        {
            return;
        }

        property.stringValue = value;
        property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void AssignVector3(UnityEngine.Object target, string propertyName, Vector3 value)
    {
        SerializedProperty property = FindSerializedProperty(target, propertyName);

        if (property == null)
        {
            return;
        }

        property.vector3Value = value;
        property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void AssignObjectReferenceArray(UnityEngine.Object target, string propertyName, UnityEngine.Object[] values)
    {
        SerializedProperty property = FindSerializedProperty(target, propertyName);

        if (property == null)
        {
            return;
        }

        property.arraySize = values != null ? values.Length : 0;

        for (int i = 0; i < property.arraySize; i++)
        {
            property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
        }

        property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }

    private static SerializedProperty FindSerializedProperty(UnityEngine.Object target, string propertyName)
    {
        if (target == null)
        {
            Debug.LogWarning($"Cannot assign serialized property '{propertyName}' because the target object is missing.");
            return null;
        }

        SerializedObject serializedObject = new SerializedObject(target);
        SerializedProperty property = serializedObject.FindProperty(propertyName);

        if (property == null)
        {
            Debug.LogWarning($"Could not find serialized property '{propertyName}' on {target.name}.");
        }

        return property;
    }

    private static string ToAbsoluteAssetPath(string assetPath)
    {
        if (!assetPath.StartsWith("Assets/", StringComparison.Ordinal))
        {
            throw new ArgumentException($"Expected an Assets-relative path, got '{assetPath}'.", nameof(assetPath));
        }

        string relativePath = assetPath.Substring("Assets/".Length);
        return Path.Combine(Application.dataPath, relativePath);
    }
}
#endif
