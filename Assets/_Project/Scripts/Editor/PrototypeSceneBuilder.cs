#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class PrototypeSceneBuilder
{
    private const string ScenePath = "Assets/_Project/Scenes/Prototype_Lab.unity";
    private const string RoomLoopScenePath = "Assets/_Project/Scenes/Prototype_RoomLoop.unity";
    private const string BulletPrefabPath = "Assets/_Project/Prefabs/Weapons/Bullet.prefab";
    private const string EnemyPrefabPath = "Assets/_Project/Prefabs/Enemies/TestEnemy.prefab";
    private const string RewardPrefabPath = "Assets/_Project/Prefabs/Pickups/PrototypeReward.prefab";
    private const string GeneratedArtFolder = "Assets/_Project/Art/Generated";
    private const string PlayerSpritePath = GeneratedArtFolder + "/Player_Prototype.png";
    private const string EnemySpritePath = GeneratedArtFolder + "/Enemy_Prototype.png";
    private const string BulletSpritePath = GeneratedArtFolder + "/Bullet_Prototype.png";
    private const string WallSpritePath = GeneratedArtFolder + "/Wall_Prototype.png";
    private const string RewardSpritePath = GeneratedArtFolder + "/Reward_Prototype.png";
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
        "Assets/_Project/Prefabs/Rooms",
        "Assets/_Project/Scenes",
        "Assets/_Project/Scripts",
        "Assets/_Project/Scripts/Core",
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

        CreateRoomLoopWalls(root.transform, wallSprite, out DoorController2D leftDoor, out DoorController2D rightDoor);

        GameObject enemyPrefab = CreateEnemyPrefab(enemySprite);
        GameObject rewardPrefab = CreateRewardPrefab(rewardSprite);
        EnemySpawner2D enemySpawner = CreateEnemySpawner(root.transform, enemyPrefab);
        Transform rewardSpawnPoint = CreateMarker(root.transform, "RewardSpawnPoint", new Vector3(4f, 0f, 0f));

        RoomController2D roomController = CreateRoomController(
            root.transform,
            new[] { leftDoor, rightDoor },
            enemySpawner,
            rewardPrefab,
            rewardSpawnPoint);

        CreateRoomTrigger(root.transform, roomController);

        EditorSceneManager.SaveScene(scene, RoomLoopScenePath);
        AddSceneToBuildSettings(RoomLoopScenePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Created Prototype 0.2 room loop scene at {RoomLoopScenePath}.");
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
        CreateWall(walls.transform, "Wall_Left_Top", sprite, new Vector3(-7f, 2.7f, 0f), new Vector3(0.4f, 2.6f, 1f));
        CreateWall(walls.transform, "Wall_Left_Bottom", sprite, new Vector3(-7f, -2.7f, 0f), new Vector3(0.4f, 2.6f, 1f));
        CreateWall(walls.transform, "Wall_Right_Top", sprite, new Vector3(7f, 2.7f, 0f), new Vector3(0.4f, 2.6f, 1f));
        CreateWall(walls.transform, "Wall_Right_Bottom", sprite, new Vector3(7f, -2.7f, 0f), new Vector3(0.4f, 2.6f, 1f));

        leftDoor = CreateDoor(walls.transform, "Door_Left", sprite, new Vector3(-7f, 0f, 0f));
        rightDoor = CreateDoor(walls.transform, "Door_Right", sprite, new Vector3(7f, 0f, 0f));
    }

    private static DoorController2D CreateDoor(Transform parent, string name, Sprite sprite, Vector3 position)
    {
        GameObject door = new GameObject(name);
        door.transform.SetParent(parent);
        door.transform.position = position;
        door.transform.localScale = new Vector3(0.5f, 2f, 1f);

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
        GameObject spawnerObject = new GameObject("EnemySpawner2D");
        spawnerObject.transform.SetParent(parent);

        Transform[] spawnPoints =
        {
            CreateMarker(spawnerObject.transform, "SpawnPoint_01", new Vector3(2.5f, 1.5f, 0f)),
            CreateMarker(spawnerObject.transform, "SpawnPoint_02", new Vector3(2.5f, -1.5f, 0f)),
            CreateMarker(spawnerObject.transform, "SpawnPoint_03", new Vector3(0f, 2f, 0f))
        };

        EnemySpawner2D spawner = spawnerObject.AddComponent<EnemySpawner2D>();
        AssignObjectReference(spawner, "enemyPrefab", enemyPrefab);
        AssignObjectReferenceArray(spawner, "spawnPoints", spawnPoints);
        AssignInt(spawner, "enemyCount", 3);

        return spawner;
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
