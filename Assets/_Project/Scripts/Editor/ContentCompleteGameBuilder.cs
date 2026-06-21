using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class ContentCompleteGameBuilder
{
    private const string GeneratedArtFolder = "Assets/_Project/Art/Generated";
    private const string GeneratedAudioFolder = "Assets/_Project/Audio/Generated";
    private const string GameScenesFolder = "Assets/_Project/Scenes/Game";
    private const string LevelScenesFolder = "Assets/_Project/Scenes/Levels";
    private const string MainMenuScenePath = GameScenesFolder + "/MainMenu.unity";
    private const string LevelSelectScenePath = GameScenesFolder + "/LevelSelect.unity";
    private const string BulletPrefabPath = "Assets/_Project/Prefabs/Weapons/Bullet.prefab";
    private const string EnemyProjectilePrefabPath = "Assets/_Project/Prefabs/Projectiles/EnemyProjectile.prefab";
    private const string HealthPickupPrefabPath = "Assets/_Project/Prefabs/Pickups/HealthPickup.prefab";
    private const string AmmoPickupPrefabPath = "Assets/_Project/Prefabs/Pickups/AmmoPickup.prefab";
    private const string MoneyPickupPrefabPath = "Assets/_Project/Prefabs/Pickups/MoneyPickup.prefab";
    private const string WeaponPickupGenericPrefabPath = "Assets/_Project/Prefabs/Pickups/WeaponPickup_Generic.prefab";
    private const string KeycardPickupPrefabPath = "Assets/_Project/Prefabs/Pickups/KeycardPickup.prefab";
    private const string LockedGatePrefabPath = "Assets/_Project/Prefabs/Environment/LockedGate.prefab";
    private const string MainMenuSceneName = "MainMenu";
    private const string LevelSelectSceneName = "LevelSelect";
    private const string PlayerTag = "Player";

    private static readonly string[] RequiredFolders =
    {
        "Assets/_Project",
        "Assets/_Project/Scripts",
        "Assets/_Project/Scripts/Game",
        "Assets/_Project/Scripts/Level",
        "Assets/_Project/Scripts/Menu",
        "Assets/_Project/Scripts/Enemies",
        "Assets/_Project/Scripts/Boss",
        "Assets/_Project/Scripts/Hazards",
        "Assets/_Project/Scripts/Weapons",
        "Assets/_Project/Scripts/Loot",
        "Assets/_Project/Scripts/Buffs",
        "Assets/_Project/Scripts/UI",
        "Assets/_Project/Scripts/Audio",
        "Assets/_Project/Scripts/Feedback",
        "Assets/_Project/Scripts/Editor",
        GameScenesFolder,
        LevelScenesFolder,
        "Assets/_Project/Prefabs",
        "Assets/_Project/Prefabs/Player",
        "Assets/_Project/Prefabs/Enemies",
        "Assets/_Project/Prefabs/Boss",
        "Assets/_Project/Prefabs/Weapons",
        "Assets/_Project/Prefabs/Projectiles",
        "Assets/_Project/Prefabs/Pickups",
        "Assets/_Project/Prefabs/Hazards",
        "Assets/_Project/Prefabs/Environment",
        "Assets/_Project/Prefabs/UI",
        "Assets/_Project/Prefabs/Shop",
        "Assets/_Project/ScriptableObjects",
        "Assets/_Project/ScriptableObjects/Weapons",
        "Assets/_Project/ScriptableObjects/Buffs",
        "Assets/_Project/ScriptableObjects/Shop",
        "Assets/_Project/ScriptableObjects/Levels",
        "Assets/_Project/ScriptableObjects/Balance",
        GeneratedArtFolder,
        GeneratedAudioFolder,
        "Assets/_Project/Docs"
    };

    private enum EnemyArchetype
    {
        Chaser,
        Ranged,
        Exploder,
        Turret,
        Miniboss,
        Boss
    }

    private sealed class LevelDefinition
    {
        public int Index;
        public string SceneName;
        public string DisplayName;
        public string StartObjective;
        public string ControlsHint;
        public string CombatObjective;
        public string SpecialObjective;
        public string BossObjective;
        public string VictoryMessage;
        public Color ThemeColor;
        public string WallSprite;
        public string FloorSprite;
        public string HazardSprite;
        public string HazardPrefabName;
        public bool UsesSecurityLasers;
        public bool UsesKeycardGate;
        public string[] CombatOneEnemies;
        public string[] CombatTwoEnemies;
        public string MinibossPrefab;
        public string BossPrefab;
        public string ShopWeapon;
        public int StartingMoney;
    }

    [MenuItem("Tools/Cod Evadare/Build Content Complete MVP")]
    public static void BuildContentCompleteMvp()
    {
        CreateRequiredFolders();
        GeneratePlaceholderAssets();
        CreateScriptableObjectContent();
        CreatePrefabs();
        BuildGameMenus();
        BuildAllFullLevels();
        UpdateDocs();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Built content complete MVP.");
    }

    [MenuItem("Tools/Cod Evadare/Build Game Menus")]
    public static void BuildGameMenus()
    {
        CreateRequiredFolders();
        CreateMainMenuScene();
        CreateLevelSelectScene();
        UpdateBuildSettings();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Built game menus.");
    }

    [MenuItem("Tools/Cod Evadare/Build All Full Levels")]
    public static void BuildAllFullLevels()
    {
        CreateRequiredFolders();
        GeneratePlaceholderAssets();
        CreateScriptableObjectContent();
        CreatePrefabs();

        foreach (LevelDefinition level in GetLevels())
        {
            CreateLevelScene(level);
        }

        UpdateBuildSettings();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Built all full levels.");
    }

    [MenuItem("Tools/Cod Evadare/Build Generated Assets Only")]
    public static void BuildGeneratedAssetsOnly()
    {
        CreateRequiredFolders();
        GeneratePlaceholderAssets();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Built generated placeholder assets.");
    }

    [MenuItem("Tools/Cod Evadare/Build ScriptableObject Content Only")]
    public static void BuildScriptableObjectContentOnly()
    {
        CreateRequiredFolders();
        GeneratePlaceholderAssets();
        CreateScriptableObjectContent();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Built ScriptableObject content.");
    }

    private static void CreateRequiredFolders()
    {
        foreach (string folder in RequiredFolders)
        {
            Directory.CreateDirectory(ToAbsolutePath(folder));
        }

        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        EnsureTag(PlayerTag);
    }

    private static void GeneratePlaceholderAssets()
    {
        foreach (SpriteSpec spec in GetSpriteSpecs())
        {
            WriteSpriteTexture($"{GeneratedArtFolder}/{spec.Name}.png", spec.Primary, spec.Secondary, spec.Accent, spec.Shape);
        }

        WriteWav("Shoot.wav", 620f, 0.08f);
        WriteWav("Hit.wav", 180f, 0.12f);
        WriteWav("Pickup.wav", 880f, 0.1f);
        WriteWav("Door.wav", 260f, 0.16f);
        WriteWav("Shop.wav", 520f, 0.12f);
        WriteWav("Buff.wav", 740f, 0.15f);
        WriteWav("BossPhase.wav", 120f, 0.25f);
        WriteWav("Victory.wav", 660f, 0.3f);
        WriteWav("GameOver.wav", 90f, 0.35f);
        WriteWav("Explosion.wav", 110f, 0.22f);
        WriteWav("Laser.wav", 980f, 0.12f);
        WriteWav("Keycard.wav", 1040f, 0.1f);

        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
    }

    private static void CreateScriptableObjectContent()
    {
        GameObject bulletPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(BulletPrefabPath);

        CreateWeapon("Pistol", WeaponRarity2D.Common, bulletPrefab, 0.20f, 12, 1, 1.0f, 1, 12f, 2f, 1, 0f);
        CreateWeapon("SMG", WeaponRarity2D.Rare, bulletPrefab, 0.08f, 24, 1, 1.2f, 1, 13f, 2f, 1, 3f);
        CreateWeapon("Shotgun", WeaponRarity2D.Epic, bulletPrefab, 0.55f, 6, 1, 1.4f, 1, 11f, 1.2f, 5, 35f);
        CreateWeapon("AssaultRifle", WeaponRarity2D.Uncommon, bulletPrefab, 0.12f, 30, 1, 1.3f, 1, 14f, 2f, 1, 4f);
        CreateWeapon("Revolver", WeaponRarity2D.Rare, bulletPrefab, 0.42f, 6, 1, 1.1f, 3, 15f, 2.2f, 1, 0f);
        CreateWeapon("PlasmaRifle", WeaponRarity2D.Epic, bulletPrefab, 0.16f, 20, 1, 1.5f, 2, 12f, 2.5f, 1, 2f);
        CreateWeapon("ArcaneStaff", WeaponRarity2D.Legendary, bulletPrefab, 0.65f, 8, 2, 1.8f, 4, 9f, 3f, 3, 18f);

        CreateBuff("Thick Skin", "Gain +1 max HP.", BuffType2D.MaxHealth, 1, 1f);
        CreateBuff("Emergency Medkit", "Heal 2 HP.", BuffType2D.Heal, 2, 1f);
        CreateBuff("Adrenaline", "Move 15% faster.", BuffType2D.MoveSpeedMultiplier, 0, 1.15f);
        CreateBuff("Trigger Discipline", "Fire 20% faster.", BuffType2D.FireRateMultiplier, 0, 1.20f);
        CreateBuff("High Caliber", "Bullets deal +1 damage.", BuffType2D.DamageBonus, 1, 1f);
        CreateBuff("Fast Hands", "Reload 25% faster.", BuffType2D.ReloadSpeedMultiplier, 0, 1.25f);
        CreateBuff("Extra Pockets", "Gain +20 reserve ammo capacity.", BuffType2D.MaxAmmo, 20, 1f);
        CreateBuff("Found Credits", "Gain 50 money.", BuffType2D.MoneyBonus, 50, 1f);
        CreateBuff("Glass Cannon", "Bullets deal +2 damage.", BuffType2D.DamageBonus, 2, 1f);
        CreateBuff("Runner", "Move 25% faster.", BuffType2D.MoveSpeedMultiplier, 0, 1.25f);
        CreateBuff("Ammo Cache", "Gain +40 reserve ammo capacity.", BuffType2D.MaxAmmo, 40, 1f);
        CreateBuff("Veteran Training", "Fire 10% faster.", BuffType2D.FireRateMultiplier, 0, 1.10f);

        CreateShopDefinition("Shop_HealthSmall", "Health +2", ShopItemType2D.Health, 25, 2, null, LoadSprite("ShopIcon_Health"));
        CreateShopDefinition("Shop_AmmoPack", "Ammo +25", ShopItemType2D.Ammo, 20, 25, null, LoadSprite("ShopIcon_Ammo"));

        foreach (string weaponName in new[] { "SMG", "Shotgun", "AssaultRifle", "Revolver", "PlasmaRifle", "ArcaneStaff" })
        {
            WeaponDefinition2D weapon = LoadWeapon(weaponName);
            int price = weaponName == "ArcaneStaff" ? 100 : weaponName == "PlasmaRifle" ? 90 : 70;
            CreateShopDefinition($"Shop_{weaponName}", weaponName, ShopItemType2D.Weapon, price, 0, weapon, LoadSprite("ShopIcon_Weapon"));
        }
    }

    private static void CreatePrefabs()
    {
        GameObject bulletPrefab = CreateBulletPrefab();
        CreateScriptableObjectContent();
        GameObject enemyProjectilePrefab = CreateEnemyProjectilePrefab();

        CreateResourcePickupPrefab("HealthPickup", HealthPickupPrefabPath, LoadSprite("Pickup_Health"), ResourcePickupType.Health, 2);
        CreateResourcePickupPrefab("AmmoPickup", AmmoPickupPrefabPath, LoadSprite("Pickup_Ammo"), ResourcePickupType.Ammo, 25);
        CreateResourcePickupPrefab("MoneyPickup", MoneyPickupPrefabPath, LoadSprite("Pickup_Money"), ResourcePickupType.Money, 20);
        CreateWeaponPickupPrefab("WeaponPickup_Generic", WeaponPickupGenericPrefabPath, LoadSprite("WeaponPickup"), LoadWeapon("SMG"));
        CreateKeycardPickupPrefab();
        CreateLockedGatePrefab();

        foreach (LevelDefinition level in GetLevels())
        {
            CreateDamageZonePrefab(level.HazardPrefabName, LoadSprite(level.HazardSprite), level.UsesSecurityLasers);
        }

        CreateEnemyPrefab("LabEnemy", LoadSprite("LabEnemy"), EnemyArchetype.Chaser, 3, 1.5f, 1, null, null);
        CreateEnemyPrefab("LabRangedEnemy", LoadSprite("LabRangedEnemy"), EnemyArchetype.Ranged, 3, 1.1f, 1, enemyProjectilePrefab, null);
        CreateEnemyPrefab("LabMiniboss", LoadSprite("LabMiniboss"), EnemyArchetype.Miniboss, 18, 1.0f, 2, null, "Lab Mutant");
        CreateEnemyPrefab("Experiment01Boss", LoadSprite("Experiment01Boss"), EnemyArchetype.Boss, 55, 1.05f, 1, enemyProjectilePrefab, "Experiment-01");

        CreateEnemyPrefab("PrisonGuard", LoadSprite("PrisonGuard"), EnemyArchetype.Chaser, 3, 1.55f, 1, null, null);
        CreateEnemyPrefab("PrisonRangedGuard", LoadSprite("PrisonRangedGuard"), EnemyArchetype.Ranged, 3, 1.15f, 1, enemyProjectilePrefab, null);
        CreateEnemyPrefab("RiotBruteMiniboss", LoadSprite("RiotBruteMiniboss"), EnemyArchetype.Miniboss, 20, 1.05f, 2, null, "Riot Brute");
        CreateEnemyPrefab("WardenBoss", LoadSprite("WardenBoss"), EnemyArchetype.Boss, 58, 0.95f, 1, enemyProjectilePrefab, "The Warden");

        CreateEnemyPrefab("ZombieWalker", LoadSprite("ZombieWalker"), EnemyArchetype.Chaser, 4, 1.25f, 1, null, null);
        CreateEnemyPrefab("ZombieRunner", LoadSprite("ZombieRunner"), EnemyArchetype.Chaser, 2, 2.15f, 1, null, null);
        CreateEnemyPrefab("ZombieSpitter", LoadSprite("ZombieSpitter"), EnemyArchetype.Ranged, 3, 0.9f, 1, enemyProjectilePrefab, null);
        CreateEnemyPrefab("ExploderZombie", LoadSprite("ExploderZombie"), EnemyArchetype.Exploder, 2, 1.9f, 2, null, null);
        CreateEnemyPrefab("NecromancerMiniboss", LoadSprite("NecromancerMiniboss"), EnemyArchetype.Miniboss, 22, 1.0f, 1, enemyProjectilePrefab, "Necromancer");
        CreateEnemyPrefab("AbominationBoss", LoadSprite("AbominationBoss"), EnemyArchetype.Boss, 65, 0.9f, 1, enemyProjectilePrefab, "The Abomination");

        CreateEnemyPrefab("DroneEnemy", LoadSprite("DroneEnemy"), EnemyArchetype.Ranged, 3, 1.6f, 1, enemyProjectilePrefab, null);
        CreateEnemyPrefab("SecurityBot", LoadSprite("SecurityBot"), EnemyArchetype.Chaser, 5, 1.25f, 1, null, null);
        CreateEnemyPrefab("LaserTurret", LoadSprite("LaserTurret"), EnemyArchetype.Turret, 4, 0f, 1, enemyProjectilePrefab, null);
        CreateEnemyPrefab("ReactorMiniboss", LoadSprite("ReactorMiniboss"), EnemyArchetype.Miniboss, 25, 0.95f, 2, enemyProjectilePrefab, "Reactor Guard");
        CreateEnemyPrefab("AICoreBoss", LoadSprite("AICoreBoss"), EnemyArchetype.Boss, 70, 0.8f, 1, enemyProjectilePrefab, "AI Core");

        CreateEnemyPrefab("HauntedNurse", LoadSprite("HauntedNurse"), EnemyArchetype.Chaser, 3, 1.7f, 1, null, null);
        CreateEnemyPrefab("ShadowCrawler", LoadSprite("ShadowCrawler"), EnemyArchetype.Chaser, 2, 2.35f, 1, null, null);
        CreateEnemyPrefab("GhostEnemy", LoadSprite("GhostEnemy"), EnemyArchetype.Ranged, 3, 1.1f, 1, enemyProjectilePrefab, null);
        CreateEnemyPrefab("SurgeonMiniboss", LoadSprite("SurgeonMiniboss"), EnemyArchetype.Miniboss, 24, 1.25f, 2, null, "Surgeon");
        CreateEnemyPrefab("NightmareDoctorBoss", LoadSprite("NightmareDoctorBoss"), EnemyArchetype.Boss, 68, 1.05f, 1, enemyProjectilePrefab, "Nightmare Doctor");

        _ = bulletPrefab;
    }

    private static void CreateMainMenuScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        SceneManager.SetActiveScene(scene);

        GameObject root = new GameObject("MainMenu");
        CreateMenuCamera(root.transform);

        Canvas canvas = CreateCanvas(root.transform);
        GameObject mainPanel = CreatePanel(canvas.transform, "MainPanel", new Vector2(0f, 0f), new Vector2(620f, 620f), new Color(0.02f, 0.025f, 0.03f, 0.92f));
        Text title = CreateText(mainPanel.transform, "Title", "COD: EVADARE", new Vector2(0f, 230f), new Vector2(560f, 70f), 42, TextAnchor.MiddleCenter);
        title.color = new Color(0.92f, 0.96f, 1f, 1f);

        GameObject controlsPanel = CreatePanel(canvas.transform, "ControlsPanel", new Vector2(0f, 0f), new Vector2(680f, 430f), new Color(0.02f, 0.025f, 0.03f, 0.96f));
        controlsPanel.SetActive(false);
        CreateText(controlsPanel.transform, "ControlsTitle", "Controls", new Vector2(0f, 150f), new Vector2(500f, 55f), 34, TextAnchor.MiddleCenter);
        CreateText(controlsPanel.transform, "ControlsText", "WASD / Arrow Keys: Move\nMouse: Aim\nLeft Click: Shoot\nR: Reload\nE: Interact\nEscape: Pause", new Vector2(0f, 20f), new Vector2(560f, 210f), 22, TextAnchor.MiddleCenter);

        MainMenuController2D controller = root.AddComponent<MainMenuController2D>();
        AssignObjectReference(controller, "mainPanel", mainPanel);
        AssignObjectReference(controller, "controlsPanel", controlsPanel);
        AssignStringArray(controller, "campaignSceneNames", GetLevelSceneNames());
        AssignStringArray(controller, "campaignScenePaths", GetLevelScenePaths());
        AssignString(controller, "levelSelectSceneName", LevelSelectSceneName);
        AssignString(controller, "levelSelectScenePath", LevelSelectScenePath);

        Button newGameButton = CreateButton(mainPanel.transform, "NewGameButton", "New Game", new Vector2(0f, 140f));
        Button continueButton = CreateButton(mainPanel.transform, "ContinueButton", "Continue", new Vector2(0f, 80f));
        Button levelSelectButton = CreateButton(mainPanel.transform, "LevelSelectButton", "Level Select", new Vector2(0f, 20f));
        Button demoButton = CreateButton(mainPanel.transform, "LaboratoryDemoButton", "Laboratory Demo", new Vector2(0f, -40f));
        Button controlsButton = CreateButton(mainPanel.transform, "ControlsButton", "Controls", new Vector2(0f, -100f));
        Button quitButton = CreateButton(mainPanel.transform, "QuitButton", "Quit", new Vector2(0f, -160f));
        Button backButton = CreateButton(controlsPanel.transform, "BackButton", "Back", new Vector2(0f, -160f));

        UnityEventTools.AddPersistentListener(newGameButton.onClick, controller.PlayNewGame);
        UnityEventTools.AddPersistentListener(continueButton.onClick, controller.ContinueGame);
        UnityEventTools.AddPersistentListener(levelSelectButton.onClick, controller.ShowLevelSelect);
        UnityEventTools.AddPersistentListener(demoButton.onClick, controller.PlayDemo);
        UnityEventTools.AddPersistentListener(controlsButton.onClick, controller.ShowControls);
        UnityEventTools.AddPersistentListener(quitButton.onClick, controller.QuitGame);
        UnityEventTools.AddPersistentListener(backButton.onClick, controller.ShowMainMenu);

        CreateEventSystem(root.transform);
        EditorSceneManager.SaveScene(scene, MainMenuScenePath);
    }

    private static void CreateLevelSelectScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        SceneManager.SetActiveScene(scene);

        GameObject root = new GameObject("LevelSelect");
        CreateMenuCamera(root.transform);
        Canvas canvas = CreateCanvas(root.transform);

        GameObject panel = CreatePanel(canvas.transform, "LevelSelectPanel", new Vector2(0f, 0f), new Vector2(720f, 620f), new Color(0.02f, 0.025f, 0.03f, 0.94f));
        CreateText(panel.transform, "Title", "Level Select", new Vector2(0f, 240f), new Vector2(560f, 60f), 38, TextAnchor.MiddleCenter);

        LevelSelectController2D controller = root.AddComponent<LevelSelectController2D>();
        AssignStringArray(controller, "levelSceneNames", GetLevelSceneNames());
        AssignString(controller, "mainMenuSceneName", MainMenuSceneName);

        Button[] buttons = new Button[5];
        Text[] texts = new Text[5];
        string[] labels =
        {
            "Level 1: Laboratory",
            "Level 2: Prison",
            "Level 3: Zombie City",
            "Level 4: Sci-Fi Base",
            "Level 5: Horror Hospital"
        };

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i] = CreateButton(panel.transform, $"LevelButton_{i + 1:00}", labels[i], new Vector2(0f, 150f - i * 58f));
            texts[i] = buttons[i].GetComponentInChildren<Text>();
            UnityEventTools.AddIntPersistentListener(buttons[i].onClick, controller.LoadLevel, i + 1);
        }

        Button backButton = CreateButton(panel.transform, "BackButton", "Back", new Vector2(0f, -190f));
        UnityEventTools.AddPersistentListener(backButton.onClick, controller.ReturnToMainMenu);

        AssignObjectReferenceArray(controller, "levelButtons", buttons);
        AssignObjectReferenceArray(controller, "levelButtonTexts", texts);

        CreateEventSystem(root.transform);
        EditorSceneManager.SaveScene(scene, LevelSelectScenePath);
    }

    private static void CreateLevelScene(LevelDefinition level)
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        SceneManager.SetActiveScene(scene);

        GameObject root = new GameObject(level.SceneName);
        GameObject cameraObject = CreateGameplayCamera(root.transform);
        CreateLighting(root.transform);

        GameObject bulletPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(BulletPrefabPath);
        WeaponDefinition2D pistol = LoadWeapon("Pistol");
        GameObject player = CreatePlayer(root.transform, LoadSprite("Player_Prototype"), bulletPrefab, pistol, level.StartingMoney, out PlayerHealth2D playerHealth, out PlayerResources2D resources, out PlayerShooting2D shooting, out PlayerBuffs2D playerBuffs, out PlayerKeyring2D keyring);
        player.transform.position = new Vector3(-28f, 0f, 0f);
        AssignObjectReference(cameraObject.GetComponent<CameraFollow2D>(), "target", player.transform);

        GameObject uiRoot = CreateGameplayUi(root.transform, playerHealth, resources, shooting, playerBuffs, keyring, out ObjectiveUI2D objectiveUI, out ShopUI2D shopUI, out BuffChoiceController2D buffChoices, out GameObject gameOverPanel, out GameObject victoryPanel, out Text victoryText, out GameObject nextLevelButton, out GameObject pausePanel);
        GameObject gameSystems = CreateGameSystems(root.transform, level, playerHealth, playerBuffs, objectiveUI, gameOverPanel, victoryPanel, victoryText, nextLevelButton, pausePanel, buffChoices);
        _ = uiRoot;
        _ = gameSystems;

        Sprite wallSprite = LoadSprite(level.WallSprite);
        Sprite floorSprite = LoadSprite(level.FloorSprite);
        GameObject hazardPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/_Project/Prefabs/Hazards/{level.HazardPrefabName}.prefab");
        GameObject healthPickup = AssetDatabase.LoadAssetAtPath<GameObject>(HealthPickupPrefabPath);
        GameObject ammoPickup = AssetDatabase.LoadAssetAtPath<GameObject>(AmmoPickupPrefabPath);
        GameObject moneyPickup = AssetDatabase.LoadAssetAtPath<GameObject>(MoneyPickupPrefabPath);
        GameObject weaponPickup = CreateWeaponPickupPrefab($"{level.ShopWeapon}Pickup", $"Assets/_Project/Prefabs/Pickups/{level.ShopWeapon}Pickup.prefab", LoadSprite("WeaponPickup"), LoadWeapon(level.ShopWeapon));

        CreateStartArea(root.transform, level, wallSprite, floorSprite, objectiveUI);
        CreateCombatRoom(root.transform, "Combat_Room_01", -18f, level, wallSprite, floorSprite, level.CombatOneEnemies, hazardPrefab, new[] { healthPickup, ammoPickup, level.UsesKeycardGate ? AssetDatabase.LoadAssetAtPath<GameObject>(KeycardPickupPrefabPath) : moneyPickup }, objectiveUI, "Clear the first room", level.UsesKeycardGate ? "Find the keycard" : "Move to the shop");

        if (level.UsesKeycardGate)
        {
            CreateLockedGateCheckpoint(root.transform, -11f, objectiveUI);
        }

        CreateShopRoom(root.transform, -7f, level, wallSprite, floorSprite, shopUI, objectiveUI);
        CreateCombatRoom(root.transform, "Combat_Room_02", 4f, level, wallSprite, floorSprite, level.CombatTwoEnemies, hazardPrefab, new[] { ammoPickup, moneyPickup, weaponPickup }, objectiveUI, level.CombatObjective, "Proceed to the miniboss");
        CreateMinibossRoom(root.transform, 16f, level, wallSprite, floorSprite, objectiveUI, buffChoices);
        CreateBossRoom(root.transform, 29f, level, wallSprite, floorSprite, objectiveUI, victoryText);

        CreateEventSystem(root.transform);
        EditorSceneManager.SaveScene(scene, $"{LevelScenesFolder}/{level.SceneName}.unity");
    }

    private static GameObject CreatePlayer(Transform parent, Sprite sprite, GameObject bulletPrefab, WeaponDefinition2D startingWeapon, int startingMoney, out PlayerHealth2D health, out PlayerResources2D resources, out PlayerShooting2D shooting, out PlayerBuffs2D buffs, out PlayerKeyring2D keyring)
    {
        GameObject player = new GameObject("Player");
        player.transform.SetParent(parent);
        player.tag = PlayerTag;
        player.transform.localScale = new Vector3(0.7f, 0.7f, 1f);

        SpriteRenderer renderer = player.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sortingOrder = 10;

        Rigidbody2D body = player.AddComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.freezeRotation = true;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        player.AddComponent<BoxCollider2D>();

        PlayerMovement2D movement = player.AddComponent<PlayerMovement2D>();
        AssignFloat(movement, "moveSpeed", 5f);
        shooting = player.AddComponent<PlayerShooting2D>();
        health = player.AddComponent<PlayerHealth2D>();
        AssignInt(health, "maxHealth", 6);
        AssignFloat(health, "invincibilityDuration", 0.75f);
        AssignBool(health, "destroyOnDeath", false);

        resources = player.AddComponent<PlayerResources2D>();
        AssignInt(resources, "startingAmmo", 100);
        AssignInt(resources, "maxAmmo", 160);
        AssignInt(resources, "startingMoney", startingMoney);

        buffs = player.AddComponent<PlayerBuffs2D>();
        keyring = player.AddComponent<PlayerKeyring2D>();
        player.AddComponent<SpriteFlash2D>();

        GameObject firePoint = new GameObject("FirePoint");
        firePoint.transform.SetParent(player.transform);
        firePoint.transform.localPosition = new Vector3(0.6f, 0f, 0f);

        AssignObjectReference(shooting, "firePoint", firePoint.transform);
        AssignObjectReference(shooting, "bulletPrefab", bulletPrefab);
        AssignBool(shooting, "useAmmo", true);
        AssignBool(shooting, "autoReloadWhenEmpty", false);
        AssignObjectReference(shooting, "playerResources", resources);
        AssignObjectReference(shooting, "startingWeapon", startingWeapon);
        AssignObjectReference(shooting, "equippedWeapon", startingWeapon);
        return player;
    }

    private static GameObject CreateGameplayUi(Transform parent, PlayerHealth2D health, PlayerResources2D resources, PlayerShooting2D shooting, PlayerBuffs2D buffs, PlayerKeyring2D keyring, out ObjectiveUI2D objectiveUI, out ShopUI2D shopUI, out BuffChoiceController2D buffChoices, out GameObject gameOverPanel, out GameObject victoryPanel, out Text victoryText, out GameObject nextLevelButton, out GameObject pausePanel)
    {
        GameObject uiRoot = new GameObject("UI");
        uiRoot.transform.SetParent(parent);
        Canvas canvas = CreateCanvas(uiRoot.transform);

        GameObject healthPanel = CreatePanel(canvas.transform, "HealthPanel", new Vector2(-520f, 300f), new Vector2(220f, 46f), new Color(0f, 0f, 0f, 0.85f));
        Image healthFill = CreateImage(healthPanel.transform, "HealthFill", Vector2.zero, new Vector2(205f, 30f), new Color(0.1f, 0.8f, 0.25f, 1f));
        Text healthText = CreateText(healthPanel.transform, "HealthText", "HP", Vector2.zero, new Vector2(205f, 30f), 18, TextAnchor.MiddleCenter);
        HealthUI2D healthUI = healthPanel.AddComponent<HealthUI2D>();
        AssignObjectReference(healthUI, "playerHealth", health);
        AssignObjectReference(healthUI, "healthFill", healthFill);
        AssignObjectReference(healthUI, "healthText", healthText);

        GameObject resourcePanel = CreatePanel(canvas.transform, "ResourcePanel", new Vector2(-440f, 215f), new Vector2(380f, 130f), new Color(0f, 0f, 0f, 0.72f));
        Text ammoText = CreateText(resourcePanel.transform, "AmmoText", "Ammo", new Vector2(0f, 38f), new Vector2(350f, 30f), 16, TextAnchor.MiddleLeft);
        Text moneyText = CreateText(resourcePanel.transform, "MoneyText", "Money", new Vector2(0f, 10f), new Vector2(350f, 30f), 16, TextAnchor.MiddleLeft);
        Text weaponText = CreateText(resourcePanel.transform, "WeaponText", "Weapon", new Vector2(0f, -24f), new Vector2(350f, 30f), 16, TextAnchor.MiddleLeft);
        Text buffText = CreateText(resourcePanel.transform, "BuffStatusText", "Buffs", new Vector2(0f, -52f), new Vector2(350f, 30f), 16, TextAnchor.MiddleLeft);
        Text keycardText = CreateText(resourcePanel.transform, "KeycardText", "Keycards", new Vector2(0f, -78f), new Vector2(350f, 30f), 16, TextAnchor.MiddleLeft);

        ResourceUI2D resourceUI = resourcePanel.AddComponent<ResourceUI2D>();
        AssignObjectReference(resourceUI, "playerResources", resources);
        AssignObjectReference(resourceUI, "playerShooting", shooting);
        AssignObjectReference(resourceUI, "ammoText", ammoText);
        AssignObjectReference(resourceUI, "moneyText", moneyText);
        WeaponUI2D weaponUI = resourcePanel.AddComponent<WeaponUI2D>();
        AssignObjectReference(weaponUI, "playerShooting", shooting);
        AssignObjectReference(weaponUI, "weaponText", weaponText);
        BuffStatusUI2D buffStatusUI = resourcePanel.AddComponent<BuffStatusUI2D>();
        AssignObjectReference(buffStatusUI, "playerBuffs", buffs);
        AssignObjectReference(buffStatusUI, "buffStatusText", buffText);
        KeycardUI2D keycardUI = resourcePanel.AddComponent<KeycardUI2D>();
        AssignObjectReference(keycardUI, "playerKeyring", keyring);
        AssignObjectReference(keycardUI, "keycardText", keycardText);

        GameObject objectivePanel = CreatePanel(canvas.transform, "ObjectivePanel", new Vector2(0f, 310f), new Vector2(520f, 80f), new Color(0f, 0f, 0f, 0.75f));
        Text objectiveText = CreateText(objectivePanel.transform, "ObjectiveText", "Objective", new Vector2(0f, 16f), new Vector2(500f, 30f), 18, TextAnchor.MiddleCenter);
        Text hintText = CreateText(objectivePanel.transform, "HintText", "Hint", new Vector2(0f, -16f), new Vector2(500f, 26f), 14, TextAnchor.MiddleCenter);
        objectiveUI = objectivePanel.AddComponent<ObjectiveUI2D>();
        AssignObjectReference(objectiveUI, "objectiveText", objectiveText);
        AssignObjectReference(objectiveUI, "hintText", hintText);

        GameObject shopPanel = CreatePanel(canvas.transform, "ShopPanel", new Vector2(0f, -270f), new Vector2(520f, 90f), new Color(0f, 0f, 0f, 0.82f));
        Text promptText = CreateText(shopPanel.transform, "PromptText", string.Empty, new Vector2(0f, 18f), new Vector2(500f, 30f), 16, TextAnchor.MiddleCenter);
        Text messageText = CreateText(shopPanel.transform, "MessageText", string.Empty, new Vector2(0f, -18f), new Vector2(500f, 30f), 16, TextAnchor.MiddleCenter);
        shopUI = shopPanel.AddComponent<ShopUI2D>();
        AssignObjectReference(shopUI, "shopPanel", shopPanel);
        AssignObjectReference(shopUI, "promptText", promptText);
        AssignObjectReference(shopUI, "messageText", messageText);
        shopPanel.SetActive(false);

        GameObject bossPanel = CreatePanel(canvas.transform, "BossHealthPanel", new Vector2(0f, 250f), new Vector2(420f, 70f), new Color(0f, 0f, 0f, 0.85f));
        Text bossNameText = CreateText(bossPanel.transform, "BossNameText", "Boss", new Vector2(0f, 20f), new Vector2(390f, 26f), 18, TextAnchor.MiddleCenter);
        Image bossFill = CreateImage(bossPanel.transform, "BossHealthFill", new Vector2(0f, -10f), new Vector2(380f, 18f), new Color(0.95f, 0.05f, 0.16f, 1f));
        Text bossHealthText = CreateText(bossPanel.transform, "BossHealthText", "HP", new Vector2(0f, -10f), new Vector2(380f, 18f), 13, TextAnchor.MiddleCenter);
        BossHealthUI2D bossUI = bossPanel.AddComponent<BossHealthUI2D>();
        AssignObjectReference(bossUI, "bossPanel", bossPanel);
        AssignObjectReference(bossUI, "healthFill", bossFill);
        AssignObjectReference(bossUI, "healthText", bossHealthText);
        AssignObjectReference(bossUI, "bossNameText", bossNameText);
        bossPanel.SetActive(false);

        GameObject choicePanel = CreatePanel(canvas.transform, "BuffChoicePanel", Vector2.zero, new Vector2(680f, 260f), new Color(0f, 0f, 0f, 0.92f));
        CreateText(choicePanel.transform, "BuffTitle", "Choose a Buff", new Vector2(0f, 92f), new Vector2(560f, 40f), 26, TextAnchor.MiddleCenter);
        Button[] choiceButtons = new Button[3];
        Text[] choiceTexts = new Text[3];

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            choiceButtons[i] = CreateButton(choicePanel.transform, $"BuffChoice_{i + 1}", "Buff", new Vector2(-210f + i * 210f, -20f), new Vector2(190f, 120f));
            choiceTexts[i] = choiceButtons[i].GetComponentInChildren<Text>();
        }

        choicePanel.SetActive(false);

        gameOverPanel = CreatePanel(canvas.transform, "GameOverPanel", Vector2.zero, new Vector2(520f, 170f), new Color(0f, 0f, 0f, 0.9f));
        CreateText(gameOverPanel.transform, "GameOverText", "GAME OVER\nPress R to restart", Vector2.zero, new Vector2(500f, 120f), 30, TextAnchor.MiddleCenter);
        gameOverPanel.SetActive(false);

        victoryPanel = CreatePanel(canvas.transform, "VictoryPanel", Vector2.zero, new Vector2(620f, 230f), new Color(0f, 0f, 0f, 0.9f));
        victoryText = CreateText(victoryPanel.transform, "VictoryText", "LEVEL CLEARED", new Vector2(0f, 40f), new Vector2(560f, 100f), 28, TextAnchor.MiddleCenter);
        Button nextButton = CreateButton(victoryPanel.transform, "NextLevelButton", "Next Level", new Vector2(0f, -70f), new Vector2(220f, 44f));
        nextLevelButton = nextButton.gameObject;
        victoryPanel.SetActive(false);

        pausePanel = CreatePanel(canvas.transform, "PausePanel", Vector2.zero, new Vector2(460f, 260f), new Color(0f, 0f, 0f, 0.92f));
        CreateText(pausePanel.transform, "PauseTitle", "Paused", new Vector2(0f, 84f), new Vector2(360f, 44f), 30, TextAnchor.MiddleCenter);
        CreateButton(pausePanel.transform, "ResumeButton", "Resume", new Vector2(0f, 20f));
        CreateButton(pausePanel.transform, "RestartButton", "Restart", new Vector2(0f, -40f));
        CreateButton(pausePanel.transform, "MainMenuButton", "Main Menu", new Vector2(0f, -100f));
        pausePanel.SetActive(false);

        buffChoices = canvas.gameObject.AddComponent<BuffChoiceController2D>();
        AssignObjectReference(buffChoices, "playerBuffs", buffs);
        AssignObjectReferenceArray(buffChoices, "buffPool", LoadAllBuffs());
        AssignObjectReference(buffChoices, "choicePanel", choicePanel);
        AssignObjectReferenceArray(buffChoices, "choiceButtons", choiceButtons);
        AssignObjectReferenceArray(buffChoices, "choiceTexts", choiceTexts);
        AssignBool(buffChoices, "pauseGameWhileChoosing", true);
        AssignInt(buffChoices, "choicesToShow", 3);

        return uiRoot;
    }

    private static GameObject CreateGameSystems(Transform parent, LevelDefinition level, PlayerHealth2D playerHealth, PlayerBuffs2D playerBuffs, ObjectiveUI2D objectiveUI, GameObject gameOverPanel, GameObject victoryPanel, Text victoryText, GameObject nextLevelButton, GameObject pausePanel, BuffChoiceController2D buffChoices)
    {
        GameObject systems = new GameObject("GameSystems");
        systems.transform.SetParent(parent);

        GameOverController2D gameOver = systems.AddComponent<GameOverController2D>();
        AssignObjectReference(gameOver, "playerHealth", playerHealth);
        AssignObjectReference(gameOver, "gameOverPanel", gameOverPanel);

        LevelEndController2D levelEnd = systems.AddComponent<LevelEndController2D>();
        AssignObjectReference(levelEnd, "victoryPanel", victoryPanel);
        AssignObjectReference(levelEnd, "victoryText", victoryText);
        AssignInt(levelEnd, "completedLevelIndex", level.Index);
        AssignInt(levelEnd, "unlockLevelIndex", level.Index < 5 ? level.Index + 1 : 6);
        AssignString(levelEnd, "nextLevelSceneName", level.Index < 5 ? GetLevels()[level.Index].SceneName : MainMenuSceneName);
        AssignBool(levelEnd, "showNextLevelButton", true);
        AssignObjectReference(levelEnd, "nextLevelButtonObject", nextLevelButton);

        if (nextLevelButton != null)
        {
            Button button = nextLevelButton.GetComponent<Button>();

            if (button != null)
            {
                UnityEventTools.AddPersistentListener(button.onClick, levelEnd.LoadNextLevel);
            }
        }

        PauseMenuController2D pause = systems.AddComponent<PauseMenuController2D>();
        AssignObjectReference(pause, "pausePanel", pausePanel);
        AssignString(pause, "mainMenuSceneName", MainMenuSceneName);
        WirePauseButtons(pausePanel, pause);

        DemoAudioManager2D audio = systems.AddComponent<DemoAudioManager2D>();
        AssignObjectReference(audio, "shootClip", LoadAudio("Shoot"));
        AssignObjectReference(audio, "hitClip", LoadAudio("Hit"));
        AssignObjectReference(audio, "pickupClip", LoadAudio("Pickup"));
        AssignObjectReference(audio, "doorClip", LoadAudio("Door"));
        AssignObjectReference(audio, "shopClip", LoadAudio("Shop"));
        AssignObjectReference(audio, "buffClip", LoadAudio("Buff"));
        AssignObjectReference(audio, "bossPhaseClip", LoadAudio("BossPhase"));
        AssignObjectReference(audio, "victoryClip", LoadAudio("Victory"));
        AssignObjectReference(audio, "gameOverClip", LoadAudio("GameOver"));
        AssignObjectReference(audio, "explosionClip", LoadAudio("Explosion"));
        AssignObjectReference(audio, "laserClip", LoadAudio("Laser"));
        AssignObjectReference(audio, "keycardClip", LoadAudio("Keycard"));

        LevelFlowController2D flow = systems.AddComponent<LevelFlowController2D>();
        AssignObjectReference(flow, "objectiveUI", objectiveUI);
        AssignObjectReference(flow, "levelEndController", levelEnd);
        AssignString(flow, "levelName", level.DisplayName);
        AssignString(flow, "startingObjective", level.StartObjective);
        AssignString(flow, "controlsHint", level.ControlsHint);

        _ = playerBuffs;
        _ = buffChoices;
        return systems;
    }

    private static void CreateStartArea(Transform parent, LevelDefinition level, Sprite wallSprite, Sprite floorSprite, ObjectiveUI2D objectiveUI)
    {
        GameObject area = new GameObject("Start_Area");
        area.transform.SetParent(parent);
        area.transform.position = new Vector3(-28f, 0f, 0f);
        CreateFloor(area.transform, "StartFloor", floorSprite, new Vector3(-28f, 0f, 0.2f), new Vector3(7f, 5f, 1f), level.ThemeColor);
        CreateWall(area.transform, "StartWall_Top", wallSprite, new Vector3(-28f, 3f, 0f), new Vector3(7.2f, 0.35f, 1f));
        CreateWall(area.transform, "StartWall_Bottom", wallSprite, new Vector3(-28f, -3f, 0f), new Vector3(7.2f, 0.35f, 1f));
        CreateWall(area.transform, "StartWall_Left", wallSprite, new Vector3(-31.7f, 0f, 0f), new Vector3(0.35f, 6f, 1f));
        CreateObjectiveTrigger(area.transform, "ObjectiveTrigger_Start", new Vector3(-28f, 0f, 0f), new Vector2(6f, 5f), objectiveUI, level.StartObjective, level.ControlsHint, false);
    }

    private static void CreateCombatRoom(Transform parent, string roomName, float centerX, LevelDefinition level, Sprite wallSprite, Sprite floorSprite, string[] enemies, GameObject hazardPrefab, GameObject[] lootPrefabs, ObjectiveUI2D objectiveUI, string objectiveOnActivate, string objectiveOnClear)
    {
        GameObject room = new GameObject(roomName);
        room.transform.SetParent(parent);
        CreateFloor(room.transform, "Floor", floorSprite, new Vector3(centerX, 0f, 0.2f), new Vector3(11.8f, 7f, 1f), level.ThemeColor);
        CreateRoomWalls(room.transform, wallSprite, centerX, 6f, 3.8f, out DoorController2D leftDoor, out DoorController2D rightDoor);
        CreateHazards(room.transform, hazardPrefab, centerX, level.UsesSecurityLasers);

        EnemySpawner2D spawner = CreateEnemySpawner(room.transform, enemies, new[]
        {
            new Vector3(centerX - 2.6f, 1.35f, 0f),
            new Vector3(centerX - 0.5f, -1.45f, 0f),
            new Vector3(centerX + 1.6f, 1.4f, 0f),
            new Vector3(centerX + 2.8f, -0.8f, 0f)
        });

        RoomLootSpawner2D lootSpawner = CreateLootSpawner(room.transform, lootPrefabs, new[]
        {
            new Vector3(centerX + 2.8f, 1.2f, 0f),
            new Vector3(centerX + 3.5f, 0f, 0f),
            new Vector3(centerX + 2.8f, -1.2f, 0f)
        });

        RoomController2D controller = room.AddComponent<RoomController2D>();
        AssignObjectReferenceArray(controller, "doors", new[] { leftDoor, rightDoor });
        AssignObjectReference(controller, "enemySpawner", spawner);
        AssignObjectReference(controller, "lootSpawner", lootSpawner);
        AssignObjectReference(controller, "objectiveUI", objectiveUI);
        AssignString(controller, "objectiveOnActivate", objectiveOnActivate);
        AssignString(controller, "objectiveOnClear", objectiveOnClear);
        AssignFloat(controller, "doorCloseDelay", 0.5f);
        CreateRoomTrigger(room.transform, centerX, 10.8f, 6.8f, controller);
        CreateCameraZone(room.transform, new Vector3(centerX, 0f, 0f), new Vector2(12.5f, 8.2f), 7f);
    }

    private static void CreateShopRoom(Transform parent, float centerX, LevelDefinition level, Sprite wallSprite, Sprite floorSprite, ShopUI2D shopUI, ObjectiveUI2D objectiveUI)
    {
        GameObject room = new GameObject("Shop_Area");
        room.transform.SetParent(parent);
        CreateFloor(room.transform, "Floor", floorSprite, new Vector3(centerX, 0f, 0.2f), new Vector3(8.2f, 5.4f, 1f), level.ThemeColor);
        CreateWall(room.transform, "ShopWall_Top", wallSprite, new Vector3(centerX, 2.9f, 0f), new Vector3(8.4f, 0.35f, 1f));
        CreateWall(room.transform, "ShopWall_Bottom", wallSprite, new Vector3(centerX, -2.9f, 0f), new Vector3(8.4f, 0.35f, 1f));
        CreateWall(room.transform, "ShopWall_Left_Upper", wallSprite, new Vector3(centerX - 4f, 2f, 0f), new Vector3(0.35f, 1.7f, 1f));
        CreateWall(room.transform, "ShopWall_Left_Lower", wallSprite, new Vector3(centerX - 4f, -2f, 0f), new Vector3(0.35f, 1.7f, 1f));
        CreateWall(room.transform, "ShopWall_Right_Upper", wallSprite, new Vector3(centerX + 4f, 2f, 0f), new Vector3(0.35f, 1.7f, 1f));
        CreateWall(room.transform, "ShopWall_Right_Lower", wallSprite, new Vector3(centerX + 4f, -2f, 0f), new Vector3(0.35f, 1.7f, 1f));

        CreateShopItem(room.transform, "ShopItem_Health", "Shop_HealthSmall", LoadSprite("ShopIcon_Health"), new Vector3(centerX - 1.7f, 0.8f, 0f), shopUI);
        CreateShopItem(room.transform, "ShopItem_Ammo", "Shop_AmmoPack", LoadSprite("ShopIcon_Ammo"), new Vector3(centerX, 0f, 0f), shopUI);
        CreateShopItem(room.transform, "ShopItem_Weapon", $"Shop_{level.ShopWeapon}", LoadSprite("ShopIcon_Weapon"), new Vector3(centerX + 1.7f, -0.8f, 0f), shopUI);
        CreateObjectiveTrigger(room.transform, "ObjectiveTrigger_Shop", new Vector3(centerX, 0f, 0f), new Vector2(7.4f, 5.2f), objectiveUI, level.SpecialObjective, "Press E near an item to buy it", false);
        CreateCameraZone(room.transform, new Vector3(centerX, 0f, 0f), new Vector2(9f, 7f), 6.5f);
    }

    private static void CreateMinibossRoom(Transform parent, float centerX, LevelDefinition level, Sprite wallSprite, Sprite floorSprite, ObjectiveUI2D objectiveUI, BuffChoiceController2D buffChoices)
    {
        GameObject room = new GameObject("Miniboss_Room");
        room.transform.SetParent(parent);
        CreateFloor(room.transform, "Floor", floorSprite, new Vector3(centerX, 0f, 0.2f), new Vector3(10f, 7f, 1f), level.ThemeColor);
        CreateRoomWalls(room.transform, wallSprite, centerX, 5f, 3.8f, out DoorController2D leftDoor, out DoorController2D rightDoor);

        EnemySpawner2D spawner = CreateEnemySpawner(room.transform, new[] { level.MinibossPrefab }, new[] { new Vector3(centerX + 1f, 0f, 0f) });
        RoomController2D controller = room.AddComponent<RoomController2D>();
        AssignObjectReferenceArray(controller, "doors", new[] { leftDoor, rightDoor });
        AssignObjectReference(controller, "enemySpawner", spawner);
        AssignObjectReference(controller, "buffChoiceController", buffChoices);
        AssignBool(controller, "showBuffChoiceOnClear", true);
        AssignObjectReference(controller, "objectiveUI", objectiveUI);
        AssignString(controller, "objectiveOnActivate", "Defeat the miniboss");
        AssignString(controller, "objectiveOnClear", "Choose a buff, then continue");
        AssignFloat(controller, "doorCloseDelay", 0.5f);
        CreateRoomTrigger(room.transform, centerX, 8.8f, 6.8f, controller);
        CreateCameraZone(room.transform, new Vector3(centerX, 0f, 0f), new Vector2(10.5f, 8f), 6.7f);
    }

    private static void CreateBossRoom(Transform parent, float centerX, LevelDefinition level, Sprite wallSprite, Sprite floorSprite, ObjectiveUI2D objectiveUI, Text victoryText)
    {
        GameObject room = new GameObject("Boss_Room");
        room.transform.SetParent(parent);
        CreateFloor(room.transform, "Floor", floorSprite, new Vector3(centerX, 0f, 0.2f), new Vector3(16f, 8.8f, 1f), level.ThemeColor);
        CreateRoomWalls(room.transform, wallSprite, centerX, 8f, 4.5f, out DoorController2D leftDoor, out DoorController2D rightDoor);
        CreateCover(room.transform, LoadSprite("Cover_Block"), centerX);

        EnemySpawner2D spawner = CreateEnemySpawner(room.transform, new[] { level.BossPrefab }, new[] { new Vector3(centerX + 2.4f, 0f, 0f) });
        RoomController2D controller = room.AddComponent<RoomController2D>();
        AssignObjectReferenceArray(controller, "doors", new[] { leftDoor, rightDoor });
        AssignObjectReference(controller, "enemySpawner", spawner);
        AssignObjectReference(controller, "objectiveUI", objectiveUI);
        AssignString(controller, "objectiveOnActivate", level.BossObjective);
        AssignString(controller, "objectiveOnClear", level.Index == 5 ? "Escape complete" : "Level complete");
        AssignFloat(controller, "doorCloseDelay", 0.5f);
        AssignBool(controller, "showVictoryOnClear", true);
        AssignString(controller, "victoryMessage", level.VictoryMessage);
        LevelEndController2D levelEnd = UnityEngine.Object.FindObjectOfType<LevelEndController2D>();
        AssignObjectReference(controller, "levelEndController", levelEnd);
        _ = victoryText;
        CreateRoomTrigger(room.transform, centerX, 14.8f, 8f, controller);
        CreateCameraZone(room.transform, new Vector3(centerX, 0f, 0f), new Vector2(16.8f, 9.6f), 7.4f);
    }

    private static void CreateLockedGateCheckpoint(Transform parent, float centerX, ObjectiveUI2D objectiveUI)
    {
        GameObject area = new GameObject("LockedGate_Checkpoint");
        area.transform.SetParent(parent);
        GameObject gatePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(LockedGatePrefabPath);
        GameObject gate = PrefabUtility.InstantiatePrefab(gatePrefab, area.transform) as GameObject;

        if (gate != null)
        {
            gate.name = "LockedGate";
            gate.transform.position = new Vector3(centerX, 0f, 0f);
            gate.transform.localScale = new Vector3(0.55f, 2.4f, 1f);
        }

        Sprite wallSprite = LoadSprite("PrisonWall");
        CreateWall(area.transform, "GateCorridor_Top", wallSprite, new Vector3(centerX, 1.45f, 0f), new Vector3(6.2f, 0.25f, 1f));
        CreateWall(area.transform, "GateCorridor_Bottom", wallSprite, new Vector3(centerX, -1.45f, 0f), new Vector3(6.2f, 0.25f, 1f));
        CreateObjectiveTrigger(area.transform, "ObjectiveTrigger_Gate", new Vector3(centerX, 0f, 0f), new Vector2(4f, 4f), objectiveUI, "Open the security gate", "Press E near the gate if you have a keycard", false);
    }

    private static void CreateRoomWalls(Transform parent, Sprite sprite, float centerX, float halfWidth, float halfHeight, out DoorController2D leftDoor, out DoorController2D rightDoor)
    {
        GameObject walls = new GameObject("Walls");
        walls.transform.SetParent(parent);
        CreateWall(walls.transform, "Wall_Top", sprite, new Vector3(centerX, halfHeight, 0f), new Vector3(halfWidth * 2f + 0.5f, 0.35f, 1f));
        CreateWall(walls.transform, "Wall_Bottom", sprite, new Vector3(centerX, -halfHeight, 0f), new Vector3(halfWidth * 2f + 0.5f, 0.35f, 1f));
        CreateWall(walls.transform, "Wall_Left_Upper", sprite, new Vector3(centerX - halfWidth, 2.65f, 0f), new Vector3(0.35f, 2.2f, 1f));
        CreateWall(walls.transform, "Wall_Left_Lower", sprite, new Vector3(centerX - halfWidth, -2.65f, 0f), new Vector3(0.35f, 2.2f, 1f));
        CreateWall(walls.transform, "Wall_Right_Upper", sprite, new Vector3(centerX + halfWidth, 2.65f, 0f), new Vector3(0.35f, 2.2f, 1f));
        CreateWall(walls.transform, "Wall_Right_Lower", sprite, new Vector3(centerX + halfWidth, -2.65f, 0f), new Vector3(0.35f, 2.2f, 1f));

        GameObject doors = new GameObject("Doors");
        doors.transform.SetParent(parent);
        leftDoor = CreateDoor(doors.transform, "Door_Left", sprite, new Vector3(centerX - halfWidth, 0f, 0f));
        rightDoor = CreateDoor(doors.transform, "Door_Right", sprite, new Vector3(centerX + halfWidth, 0f, 0f));
    }

    private static DoorController2D CreateDoor(Transform parent, string name, Sprite sprite, Vector3 position)
    {
        GameObject door = new GameObject(name);
        door.transform.SetParent(parent);
        door.transform.position = position;
        door.transform.localScale = new Vector3(0.38f, 2.4f, 1f);
        SpriteRenderer renderer = door.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sortingOrder = 6;
        renderer.enabled = false;
        BoxCollider2D collider = door.AddComponent<BoxCollider2D>();
        collider.enabled = false;
        DoorController2D controller = door.AddComponent<DoorController2D>();
        AssignObjectReference(controller, "spriteRenderer", renderer);
        AssignObjectReference(controller, "doorCollider", collider);
        return controller;
    }

    private static EnemySpawner2D CreateEnemySpawner(Transform parent, string[] enemyPrefabNames, Vector3[] positions)
    {
        GameObject spawnerObject = new GameObject("EnemySpawner");
        spawnerObject.transform.SetParent(parent);
        Transform[] spawnPoints = new Transform[positions.Length];

        for (int i = 0; i < positions.Length; i++)
        {
            GameObject marker = new GameObject($"SpawnPoint_{i + 1:00}");
            marker.transform.SetParent(spawnerObject.transform);
            marker.transform.position = positions[i];
            spawnPoints[i] = marker.transform;
        }

        GameObject[] prefabs = new GameObject[enemyPrefabNames.Length];

        for (int i = 0; i < enemyPrefabNames.Length; i++)
        {
            prefabs[i] = LoadEnemyPrefab(enemyPrefabNames[i]);
        }

        EnemySpawner2D spawner = spawnerObject.AddComponent<EnemySpawner2D>();
        AssignObjectReferenceArray(spawner, "enemyPrefabs", prefabs);
        AssignObjectReferenceArray(spawner, "spawnPoints", spawnPoints);
        AssignInt(spawner, "enemyCount", enemyPrefabNames.Length);
        return spawner;
    }

    private static RoomLootSpawner2D CreateLootSpawner(Transform parent, GameObject[] lootPrefabs, Vector3[] positions)
    {
        GameObject lootObject = new GameObject("LootSpawner");
        lootObject.transform.SetParent(parent);
        Transform[] spawnPoints = new Transform[positions.Length];

        for (int i = 0; i < positions.Length; i++)
        {
            GameObject marker = new GameObject($"LootSpawnPoint_{i + 1:00}");
            marker.transform.SetParent(lootObject.transform);
            marker.transform.position = positions[i];
            spawnPoints[i] = marker.transform;
        }

        RoomLootSpawner2D spawner = lootObject.AddComponent<RoomLootSpawner2D>();
        AssignObjectReferenceArray(spawner, "lootPrefabs", lootPrefabs);
        AssignObjectReferenceArray(spawner, "spawnPoints", spawnPoints);
        AssignBool(spawner, "spawnAll", true);
        AssignInt(spawner, "randomSpawnCount", lootPrefabs != null ? lootPrefabs.Length : 0);
        return spawner;
    }

    private static void CreateRoomTrigger(Transform parent, float centerX, float width, float height, RoomController2D controller)
    {
        GameObject triggerObject = new GameObject("RoomTrigger");
        triggerObject.transform.SetParent(parent);
        triggerObject.transform.position = new Vector3(centerX, 0f, 0f);
        BoxCollider2D collider = triggerObject.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(width, height);
        RoomTrigger2D trigger = triggerObject.AddComponent<RoomTrigger2D>();
        AssignObjectReference(trigger, "roomController", controller);
    }

    private static void CreateObjectiveTrigger(Transform parent, string name, Vector3 position, Vector2 size, ObjectiveUI2D objectiveUI, string objective, string hint, bool once)
    {
        GameObject triggerObject = new GameObject(name);
        triggerObject.transform.SetParent(parent);
        triggerObject.transform.position = position;
        BoxCollider2D collider = triggerObject.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = size;
        ObjectiveTrigger2D trigger = triggerObject.AddComponent<ObjectiveTrigger2D>();
        AssignObjectReference(trigger, "objectiveUI", objectiveUI);
        AssignString(trigger, "objectiveMessage", objective);
        AssignString(trigger, "hintMessage", hint);
        AssignBool(trigger, "triggerOnce", once);
    }

    private static void CreateCameraZone(Transform parent, Vector3 position, Vector2 size, float orthographicSize)
    {
        GameObject zoneObject = new GameObject("CameraZone");
        zoneObject.transform.SetParent(parent);
        zoneObject.transform.position = position;
        BoxCollider2D collider = zoneObject.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = size;
        CameraZone2D zone = zoneObject.AddComponent<CameraZone2D>();
        AssignFloat(zone, "orthographicSize", orthographicSize);
    }

    private static void CreateHazards(Transform parent, GameObject hazardPrefab, float centerX, bool useVerticalLayout)
    {
        if (hazardPrefab == null)
        {
            return;
        }

        Vector3[] positions = useVerticalLayout
            ? new[] { new Vector3(centerX - 1.5f, 1.6f, 0f), new Vector3(centerX + 2.4f, -1.4f, 0f) }
            : new[] { new Vector3(centerX - 2.1f, 0.9f, 0f), new Vector3(centerX + 1.9f, -1.2f, 0f) };

        foreach (Vector3 position in positions)
        {
            GameObject hazard = PrefabUtility.InstantiatePrefab(hazardPrefab, parent) as GameObject;

            if (hazard != null)
            {
                hazard.transform.position = position;
            }
        }
    }

    private static void CreateCover(Transform parent, Sprite sprite, float centerX)
    {
        GameObject coverRoot = new GameObject("Cover");
        coverRoot.transform.SetParent(parent);
        CreateCoverObject(coverRoot.transform, "Cover_01", sprite, new Vector3(centerX - 3f, 1.8f, 0f));
        CreateCoverObject(coverRoot.transform, "Cover_02", sprite, new Vector3(centerX - 2f, -1.8f, 0f));
        CreateCoverObject(coverRoot.transform, "Cover_03", sprite, new Vector3(centerX + 2f, 1.7f, 0f));
        CreateCoverObject(coverRoot.transform, "Cover_04", sprite, new Vector3(centerX + 3f, -1.6f, 0f));
    }

    private static void CreateCoverObject(Transform parent, string name, Sprite sprite, Vector3 position)
    {
        GameObject cover = new GameObject(name);
        cover.transform.SetParent(parent);
        cover.transform.position = position;
        cover.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
        SpriteRenderer renderer = cover.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sortingOrder = 2;
        cover.AddComponent<BoxCollider2D>();
        cover.AddComponent<CoverObject2D>();
    }

    private static void CreateShopItem(Transform parent, string name, string definitionName, Sprite sprite, Vector3 position, ShopUI2D shopUI)
    {
        GameObject item = new GameObject(name);
        item.transform.SetParent(parent);
        item.transform.position = position;
        item.transform.localScale = new Vector3(0.55f, 0.55f, 1f);
        SpriteRenderer renderer = item.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sortingOrder = 12;
        CircleCollider2D collider = item.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        ShopItem2D shopItem = item.AddComponent<ShopItem2D>();
        AssignObjectReference(shopItem, "itemDefinition", AssetDatabase.LoadAssetAtPath<ShopItemDefinition2D>($"Assets/_Project/ScriptableObjects/Shop/{definitionName}.asset"));
        AssignObjectReference(shopItem, "spriteRenderer", renderer);
        AssignObjectReference(shopItem, "shopUI", shopUI);
        item.AddComponent<PickupBob2D>();
    }

    private static void CreateWall(Transform parent, string name, Sprite sprite, Vector3 position, Vector3 scale)
    {
        GameObject wall = new GameObject(name);
        wall.transform.SetParent(parent);
        wall.transform.position = position;
        wall.transform.localScale = scale;
        SpriteRenderer renderer = wall.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sortingOrder = 1;
        wall.AddComponent<BoxCollider2D>();
    }

    private static void CreateFloor(Transform parent, string name, Sprite sprite, Vector3 position, Vector3 scale, Color themeColor)
    {
        GameObject floor = new GameObject(name);
        floor.transform.SetParent(parent);
        floor.transform.position = position;
        floor.transform.localScale = scale;
        SpriteRenderer renderer = floor.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sortingOrder = -5;
        renderer.color = new Color(themeColor.r, themeColor.g, themeColor.b, 0.35f);
    }

    private static GameObject CreateBulletPrefab()
    {
        GameObject bullet = new GameObject("Bullet");
        bullet.transform.localScale = new Vector3(0.16f, 0.16f, 1f);
        SpriteRenderer renderer = bullet.AddComponent<SpriteRenderer>();
        renderer.sprite = LoadSprite("Bullet_Prototype");
        renderer.sortingOrder = 20;
        Rigidbody2D body = bullet.AddComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        CircleCollider2D collider = bullet.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        Bullet2D bullet2D = bullet.AddComponent<Bullet2D>();
        AssignFloat(bullet2D, "speed", 12f);
        AssignFloat(bullet2D, "lifetime", 2f);
        AssignInt(bullet2D, "damage", 1);
        return SavePrefab(bullet, BulletPrefabPath);
    }

    private static GameObject CreateEnemyProjectilePrefab()
    {
        GameObject projectile = new GameObject("EnemyProjectile");
        projectile.transform.localScale = new Vector3(0.18f, 0.18f, 1f);
        SpriteRenderer renderer = projectile.AddComponent<SpriteRenderer>();
        renderer.sprite = LoadSprite("EnemyProjectile");
        renderer.sortingOrder = 20;
        Rigidbody2D body = projectile.AddComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        CircleCollider2D collider = projectile.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        EnemyProjectile2D projectile2D = projectile.AddComponent<EnemyProjectile2D>();
        AssignFloat(projectile2D, "speed", 7f);
        AssignFloat(projectile2D, "lifetime", 4f);
        AssignInt(projectile2D, "damage", 1);
        return SavePrefab(projectile, EnemyProjectilePrefabPath);
    }

    private static GameObject CreateResourcePickupPrefab(string name, string path, Sprite sprite, ResourcePickupType type, int amount)
    {
        GameObject pickup = new GameObject(name);
        pickup.transform.localScale = new Vector3(0.45f, 0.45f, 1f);
        SpriteRenderer renderer = pickup.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sortingOrder = 14;
        CircleCollider2D collider = pickup.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        ResourcePickup2D resourcePickup = pickup.AddComponent<ResourcePickup2D>();
        AssignEnum(resourcePickup, "pickupType", (int)type);
        AssignInt(resourcePickup, "amount", amount);
        pickup.AddComponent<PickupBob2D>();
        return SavePrefab(pickup, path);
    }

    private static GameObject CreateWeaponPickupPrefab(string name, string path, Sprite sprite, WeaponDefinition2D weapon)
    {
        GameObject pickup = new GameObject(name);
        pickup.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
        SpriteRenderer renderer = pickup.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sortingOrder = 14;
        CircleCollider2D collider = pickup.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        WeaponPickup2D weaponPickup = pickup.AddComponent<WeaponPickup2D>();
        AssignObjectReference(weaponPickup, "weapon", weapon);
        pickup.AddComponent<PickupBob2D>();
        return SavePrefab(pickup, path);
    }

    private static GameObject CreateKeycardPickupPrefab()
    {
        GameObject pickup = new GameObject("KeycardPickup");
        pickup.transform.localScale = new Vector3(0.42f, 0.42f, 1f);
        SpriteRenderer renderer = pickup.AddComponent<SpriteRenderer>();
        renderer.sprite = LoadSprite("Keycard");
        renderer.sortingOrder = 14;
        CircleCollider2D collider = pickup.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        pickup.AddComponent<KeycardPickup2D>();
        pickup.AddComponent<PickupBob2D>();
        return SavePrefab(pickup, KeycardPickupPrefabPath);
    }

    private static GameObject CreateLockedGatePrefab()
    {
        GameObject gate = new GameObject("LockedGate");
        gate.transform.localScale = new Vector3(0.55f, 2.4f, 1f);
        SpriteRenderer renderer = gate.AddComponent<SpriteRenderer>();
        renderer.sprite = LoadSprite("LockedGate");
        renderer.sortingOrder = 8;
        BoxCollider2D blockingCollider = gate.AddComponent<BoxCollider2D>();
        blockingCollider.isTrigger = false;
        BoxCollider2D promptCollider = gate.AddComponent<BoxCollider2D>();
        promptCollider.isTrigger = true;
        promptCollider.size = new Vector2(4f, 1.4f);
        LockedGate2D lockedGate = gate.AddComponent<LockedGate2D>();
        AssignObjectReference(lockedGate, "spriteRenderer", renderer);
        AssignObjectReference(lockedGate, "gateCollider", blockingCollider);
        AssignString(lockedGate, "lockedMessage", "Requires keycard");
        AssignString(lockedGate, "openMessage", "Gate opened");
        return SavePrefab(gate, LockedGatePrefabPath);
    }

    private static GameObject CreateDamageZonePrefab(string name, Sprite sprite, bool securityLaser)
    {
        string path = $"Assets/_Project/Prefabs/Hazards/{name}.prefab";
        GameObject hazard = new GameObject(name);
        hazard.transform.localScale = securityLaser ? new Vector3(0.22f, 2.8f, 1f) : new Vector3(1.4f, 1.0f, 1f);
        SpriteRenderer renderer = hazard.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sortingOrder = 4;
        BoxCollider2D collider = hazard.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;

        if (securityLaser)
        {
            SecurityLaserHazard2D laser = hazard.AddComponent<SecurityLaserHazard2D>();
            AssignInt(laser, "damage", 1);
            AssignFloat(laser, "warningDuration", 0.45f);
            AssignFloat(laser, "activeDuration", 1.6f);
            AssignFloat(laser, "inactiveDuration", 2.0f);
        }
        else
        {
            DamageZone2D zone = hazard.AddComponent<DamageZone2D>();
            AssignInt(zone, "damage", 1);
            AssignFloat(zone, "damageCooldown", 1f);
        }

        return SavePrefab(hazard, path);
    }

    private static GameObject CreateEnemyPrefab(string name, Sprite sprite, EnemyArchetype archetype, int health, float speed, int contactDamage, GameObject projectilePrefab, string displayName)
    {
        string path = archetype == EnemyArchetype.Boss
            ? $"Assets/_Project/Prefabs/Boss/{name}.prefab"
            : $"Assets/_Project/Prefabs/Enemies/{name}.prefab";

        GameObject enemy = new GameObject(name);
        enemy.transform.localScale = archetype == EnemyArchetype.Boss ? new Vector3(1.8f, 1.8f, 1f) : archetype == EnemyArchetype.Miniboss ? new Vector3(1.4f, 1.4f, 1f) : Vector3.one;
        SpriteRenderer renderer = enemy.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sortingOrder = 10;
        Rigidbody2D body = enemy.AddComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.freezeRotation = true;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        enemy.AddComponent<BoxCollider2D>();
        EnemyHealth enemyHealth = enemy.AddComponent<EnemyHealth>();
        AssignInt(enemyHealth, "maxHealth", health);
        enemy.AddComponent<SpriteFlash2D>();

        if (contactDamage > 0 && archetype != EnemyArchetype.Turret)
        {
            EnemyContactDamage2D contact = enemy.AddComponent<EnemyContactDamage2D>();
            AssignInt(contact, "damage", contactDamage);
            AssignFloat(contact, "damageCooldown", 1f);
        }

        switch (archetype)
        {
            case EnemyArchetype.Chaser:
                AddChaser(enemy, speed, 1f);
                break;
            case EnemyArchetype.Ranged:
                AddRanged(enemy, projectilePrefab, speed);
                break;
            case EnemyArchetype.Exploder:
                ExplodingEnemy2D exploder = enemy.AddComponent<ExplodingEnemy2D>();
                AssignFloat(exploder, "moveSpeed", speed);
                AssignFloat(exploder, "explosionRadius", 1.55f);
                AssignInt(exploder, "explosionDamage", contactDamage);
                break;
            case EnemyArchetype.Turret:
                AddTurret(enemy, projectilePrefab);
                break;
            case EnemyArchetype.Miniboss:
                if (projectilePrefab != null)
                {
                    AddRanged(enemy, projectilePrefab, speed);
                }
                else
                {
                    AddChaser(enemy, speed, 1.2f);
                }
                MinibossMarker2D miniboss = enemy.AddComponent<MinibossMarker2D>();
                AssignString(miniboss, "minibossName", displayName ?? name);
                break;
            case EnemyArchetype.Boss:
                AddBoss(enemy, projectilePrefab, speed, displayName ?? name);
                break;
        }

        return SavePrefab(enemy, path);
    }

    private static void AddChaser(GameObject enemy, float speed, float stopDistance)
    {
        SimpleEnemyChaser2D chaser = enemy.AddComponent<SimpleEnemyChaser2D>();
        AssignFloat(chaser, "moveSpeed", speed);
        AssignFloat(chaser, "stopDistance", stopDistance);
    }

    private static void AddRanged(GameObject enemy, GameObject projectilePrefab, float speed)
    {
        GameObject spawnPoint = new GameObject("ProjectileSpawnPoint");
        spawnPoint.transform.SetParent(enemy.transform);
        spawnPoint.transform.localPosition = new Vector3(0.55f, 0f, 0f);
        RangedEnemyShooter2D shooter = enemy.AddComponent<RangedEnemyShooter2D>();
        AssignObjectReference(shooter, "projectileSpawnPoint", spawnPoint.transform);
        AssignObjectReference(shooter, "enemyProjectilePrefab", projectilePrefab);
        AssignFloat(shooter, "moveSpeed", speed);
        AssignFloat(shooter, "preferredDistance", 4.5f);
        AssignFloat(shooter, "minimumDistance", 2.2f);
        AssignFloat(shooter, "attackInterval", 1.35f);
        AssignFloat(shooter, "aimTelegraphDuration", 0.25f);
        AddAimLine(enemy, shooter);
    }

    private static void AddTurret(GameObject enemy, GameObject projectilePrefab)
    {
        GameObject spawnPoint = new GameObject("ProjectileSpawnPoint");
        spawnPoint.transform.SetParent(enemy.transform);
        spawnPoint.transform.localPosition = new Vector3(0.5f, 0f, 0f);
        StationaryTurret2D turret = enemy.AddComponent<StationaryTurret2D>();
        AssignObjectReference(turret, "projectileSpawnPoint", spawnPoint.transform);
        AssignObjectReference(turret, "enemyProjectilePrefab", projectilePrefab);
        AssignFloat(turret, "detectionRange", 8f);
        AssignFloat(turret, "attackInterval", 1f);
        AssignFloat(turret, "aimTelegraphDuration", 0.25f);
        AddAimLine(enemy, turret);
    }

    private static void AddBoss(GameObject enemy, GameObject projectilePrefab, float speed, string bossName)
    {
        GameObject spawnPoint = new GameObject("ProjectileSpawnPoint");
        spawnPoint.transform.SetParent(enemy.transform);
        spawnPoint.transform.localPosition = new Vector3(0.8f, 0f, 0f);
        BossAttackController2D boss = enemy.AddComponent<BossAttackController2D>();
        AssignString(boss, "bossName", bossName);
        AssignObjectReference(boss, "projectileSpawnPoint", spawnPoint.transform);
        AssignObjectReference(boss, "enemyProjectilePrefab", projectilePrefab);
        AssignFloat(boss, "moveSpeed", speed);
        AssignFloat(boss, "stopDistance", 3.2f);
        AssignFloat(boss, "attackInterval", 1.45f);
        AssignInt(boss, "phaseOneRadialProjectiles", 8);
        AssignInt(boss, "phaseTwoRadialProjectiles", 14);
        BossMarker2D marker = enemy.AddComponent<BossMarker2D>();
        AssignString(marker, "bossName", bossName);
    }

    private static void AddAimLine(GameObject enemy, UnityEngine.Object owner)
    {
        LineRenderer line = enemy.AddComponent<LineRenderer>();
        line.enabled = false;
        line.positionCount = 2;
        line.startWidth = 0.035f;
        line.endWidth = 0.035f;
        line.material = GetOrCreateMaterial("Assets/_Project/Materials/MvpAimTelegraph.mat", new Color(1f, 0.12f, 0.08f, 0.65f));
        AssignObjectReference(owner, "aimLine", line);
    }

    private static void WirePauseButtons(GameObject pausePanel, PauseMenuController2D pause)
    {
        if (pausePanel == null || pause == null)
        {
            return;
        }

        Button resume = pausePanel.transform.Find("ResumeButton")?.GetComponent<Button>();
        Button restart = pausePanel.transform.Find("RestartButton")?.GetComponent<Button>();
        Button mainMenu = pausePanel.transform.Find("MainMenuButton")?.GetComponent<Button>();

        if (resume != null)
        {
            UnityEventTools.AddPersistentListener(resume.onClick, pause.Resume);
        }

        if (restart != null)
        {
            UnityEventTools.AddPersistentListener(restart.onClick, pause.RestartScene);
        }

        if (mainMenu != null)
        {
            UnityEventTools.AddPersistentListener(mainMenu.onClick, pause.ReturnToMainMenu);
        }
    }

    private static GameObject CreateGameplayCamera(Transform parent)
    {
        GameObject cameraObject = new GameObject("Main Camera");
        cameraObject.transform.SetParent(parent);
        cameraObject.transform.position = new Vector3(-28f, 0f, -10f);
        Camera camera = cameraObject.AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 6.5f;
        camera.backgroundColor = new Color(0.04f, 0.05f, 0.055f, 1f);
        camera.clearFlags = CameraClearFlags.SolidColor;
        cameraObject.tag = "MainCamera";
        cameraObject.AddComponent<CameraFollow2D>();
        cameraObject.AddComponent<SimpleCameraShake2D>();
        return cameraObject;
    }

    private static void CreateMenuCamera(Transform parent)
    {
        GameObject cameraObject = new GameObject("Main Camera");
        cameraObject.transform.SetParent(parent);
        cameraObject.transform.position = new Vector3(0f, 0f, -10f);
        Camera camera = cameraObject.AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 5f;
        camera.backgroundColor = new Color(0.015f, 0.018f, 0.022f, 1f);
        camera.clearFlags = CameraClearFlags.SolidColor;
        cameraObject.tag = "MainCamera";
    }

    private static void CreateLighting(Transform parent)
    {
        GameObject lighting = new GameObject("Lighting");
        lighting.transform.SetParent(parent);
        GameObject lightObject = new GameObject("Global Light 2D");
        lightObject.transform.SetParent(lighting.transform);
        Type lightType = Type.GetType("UnityEngine.Rendering.Universal.Light2D, Unity.RenderPipelines.Universal.Runtime");

        if (lightType == null)
        {
            Debug.LogWarning("Light2D type not found. Skipping 2D lighting setup.");
            return;
        }

        Component light = lightObject.AddComponent(lightType);
        SerializedObject serializedLight = new SerializedObject(light);
        SerializedProperty lightTypeProperty = serializedLight.FindProperty("m_LightType");

        if (lightTypeProperty != null)
        {
            lightTypeProperty.intValue = 4;
        }

        SerializedProperty intensityProperty = serializedLight.FindProperty("m_Intensity");

        if (intensityProperty != null)
        {
            intensityProperty.floatValue = 1f;
        }

        serializedLight.ApplyModifiedPropertiesWithoutUndo();
    }

    private static Canvas CreateCanvas(Transform parent)
    {
        GameObject canvasObject = new GameObject("Canvas");
        canvasObject.transform.SetParent(parent);
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280f, 720f);
        canvasObject.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    private static GameObject CreatePanel(Transform parent, string name, Vector2 anchoredPosition, Vector2 size, Color color)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchoredPosition = anchoredPosition;
        Image image = panel.AddComponent<Image>();
        image.color = color;
        return panel;
    }

    private static Text CreateText(Transform parent, string name, string value, Vector2 anchoredPosition, Vector2 size, int fontSize, TextAnchor alignment)
    {
        GameObject textObject = new GameObject(name);
        textObject.transform.SetParent(parent, false);
        RectTransform rect = textObject.AddComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchoredPosition = anchoredPosition;
        Text text = textObject.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.text = value;
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = Color.white;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        return text;
    }

    private static Image CreateImage(Transform parent, string name, Vector2 anchoredPosition, Vector2 size, Color color)
    {
        GameObject imageObject = new GameObject(name);
        imageObject.transform.SetParent(parent, false);
        RectTransform rect = imageObject.AddComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchoredPosition = anchoredPosition;
        Image image = imageObject.AddComponent<Image>();
        image.color = color;
        return image;
    }

    private static Button CreateButton(Transform parent, string name, string label, Vector2 anchoredPosition)
    {
        return CreateButton(parent, name, label, anchoredPosition, new Vector2(300f, 44f));
    }

    private static Button CreateButton(Transform parent, string name, string label, Vector2 anchoredPosition, Vector2 size)
    {
        GameObject buttonObject = new GameObject(name);
        buttonObject.transform.SetParent(parent, false);
        RectTransform rect = buttonObject.AddComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchoredPosition = anchoredPosition;
        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.14f, 0.2f, 0.25f, 1f);
        Button button = buttonObject.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.highlightedColor = new Color(0.2f, 0.32f, 0.38f, 1f);
        colors.pressedColor = new Color(0.08f, 0.14f, 0.18f, 1f);
        colors.disabledColor = new Color(0.05f, 0.055f, 0.06f, 0.75f);
        button.colors = colors;
        CreateText(buttonObject.transform, "Text", label, Vector2.zero, size - new Vector2(16f, 8f), 18, TextAnchor.MiddleCenter);
        return button;
    }

    private static void CreateEventSystem(Transform parent)
    {
        if (UnityEngine.Object.FindObjectOfType<EventSystem>() != null)
        {
            return;
        }

        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.transform.SetParent(parent);
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();
    }

    private static GameObject SavePrefab(GameObject gameObject, string path)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(ToAbsolutePath(path)) ?? ToAbsolutePath("Assets/_Project/Prefabs"));
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(gameObject, path);
        UnityEngine.Object.DestroyImmediate(gameObject);
        return prefab != null ? prefab : AssetDatabase.LoadAssetAtPath<GameObject>(path);
    }

    private static WeaponDefinition2D CreateWeapon(string name, WeaponRarity2D rarity, GameObject bulletPrefab, float cooldown, int magazineSize, int ammoPerShot, float reloadDuration, int damage, float speed, float lifetime, int projectiles, float spread)
    {
        string path = $"Assets/_Project/ScriptableObjects/Weapons/{name}.asset";
        WeaponDefinition2D weapon = AssetDatabase.LoadAssetAtPath<WeaponDefinition2D>(path);

        if (weapon == null)
        {
            weapon = ScriptableObject.CreateInstance<WeaponDefinition2D>();
            AssetDatabase.CreateAsset(weapon, path);
        }

        AssignString(weapon, "weaponName", name);
        AssignEnum(weapon, "rarity", (int)rarity);
        AssignObjectReference(weapon, "bulletPrefab", bulletPrefab);
        AssignFloat(weapon, "fireCooldown", cooldown);
        AssignInt(weapon, "magazineSize", magazineSize);
        AssignInt(weapon, "ammoPerShot", ammoPerShot);
        AssignFloat(weapon, "reloadDuration", reloadDuration);
        AssignInt(weapon, "projectileDamage", damage);
        AssignFloat(weapon, "projectileSpeed", speed);
        AssignFloat(weapon, "projectileLifetime", lifetime);
        AssignInt(weapon, "projectilesPerShot", projectiles);
        AssignFloat(weapon, "spreadAngle", spread);
        EditorUtility.SetDirty(weapon);
        return weapon;
    }

    private static BuffDefinition2D CreateBuff(string name, string description, BuffType2D type, int amount, float multiplier)
    {
        string path = $"Assets/_Project/ScriptableObjects/Buffs/Buff_{SanitizeAssetName(name)}.asset";
        BuffDefinition2D buff = AssetDatabase.LoadAssetAtPath<BuffDefinition2D>(path);

        if (buff == null)
        {
            buff = ScriptableObject.CreateInstance<BuffDefinition2D>();
            AssetDatabase.CreateAsset(buff, path);
        }

        AssignString(buff, "displayName", name);
        AssignString(buff, "description", description);
        AssignEnum(buff, "buffType", (int)type);
        AssignInt(buff, "amount", amount);
        AssignFloat(buff, "multiplier", multiplier);
        EditorUtility.SetDirty(buff);
        return buff;
    }

    private static ShopItemDefinition2D CreateShopDefinition(string assetName, string displayName, ShopItemType2D type, int price, int amount, WeaponDefinition2D weapon, Sprite icon)
    {
        string path = $"Assets/_Project/ScriptableObjects/Shop/{assetName}.asset";
        ShopItemDefinition2D definition = AssetDatabase.LoadAssetAtPath<ShopItemDefinition2D>(path);

        if (definition == null)
        {
            definition = ScriptableObject.CreateInstance<ShopItemDefinition2D>();
            AssetDatabase.CreateAsset(definition, path);
        }

        AssignString(definition, "displayName", displayName);
        AssignEnum(definition, "itemType", (int)type);
        AssignInt(definition, "price", price);
        AssignInt(definition, "amount", amount);
        AssignObjectReference(definition, "weaponDefinition", weapon);
        AssignObjectReference(definition, "icon", icon);
        EditorUtility.SetDirty(definition);
        return definition;
    }

    private static void WriteSpriteTexture(string assetPath, Color primary, Color secondary, Color accent, int shape)
    {
        const int size = 64;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color clear = new Color(0f, 0f, 0f, 0f);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                texture.SetPixel(x, y, GetSpritePixel(x, y, size, primary, secondary, accent, shape));
            }
        }

        texture.Apply();
        Directory.CreateDirectory(Path.GetDirectoryName(ToAbsolutePath(assetPath)) ?? ToAbsolutePath(GeneratedArtFolder));
        File.WriteAllBytes(ToAbsolutePath(assetPath), texture.EncodeToPNG());
        UnityEngine.Object.DestroyImmediate(texture);
        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceSynchronousImport);
        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;

        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = 64f;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }

        _ = clear;
    }

    private static Color GetSpritePixel(int x, int y, int size, Color primary, Color secondary, Color accent, int shape)
    {
        float center = (size - 1) * 0.5f;
        float dx = x - center;
        float dy = y - center;
        bool border = x < 5 || x >= size - 5 || y < 5 || y >= size - 5;
        bool stripe = (x + y) % 16 < 7;

        switch (shape % 5)
        {
            case 0:
                return border ? secondary : (stripe ? accent : primary);
            case 1:
                {
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);

                    if (dist > center * 0.9f)
                    {
                        return Color.clear;
                    }

                    return dist > center * 0.75f ? secondary : (Mathf.Abs(dx) < 6f || Mathf.Abs(dy) < 6f ? accent : primary);
                }
            case 2:
                return Mathf.Abs(dx) < 8f || Mathf.Abs(dy) < 8f || border ? accent : primary;
            case 3:
                return stripe ? primary : secondary;
            default:
                {
                    bool diamond = Mathf.Abs(dx) + Mathf.Abs(dy) < center * 1.25f;
                    return diamond ? (border ? secondary : primary) : Color.clear;
                }
        }
    }

    private static void WriteWav(string fileName, float frequency, float duration)
    {
        const int sampleRate = 22050;
        int sampleCount = Mathf.Max(1, Mathf.RoundToInt(sampleRate * duration));
        byte[] data = new byte[44 + sampleCount * 2];

        using (MemoryStream stream = new MemoryStream(data))
        using (BinaryWriter writer = new BinaryWriter(stream))
        {
            writer.Write(new[] { 'R', 'I', 'F', 'F' });
            writer.Write(36 + sampleCount * 2);
            writer.Write(new[] { 'W', 'A', 'V', 'E' });
            writer.Write(new[] { 'f', 'm', 't', ' ' });
            writer.Write(16);
            writer.Write((short)1);
            writer.Write((short)1);
            writer.Write(sampleRate);
            writer.Write(sampleRate * 2);
            writer.Write((short)2);
            writer.Write((short)16);
            writer.Write(new[] { 'd', 'a', 't', 'a' });
            writer.Write(sampleCount * 2);

            for (int i = 0; i < sampleCount; i++)
            {
                float t = (float)i / sampleRate;
                float envelope = 1f - (float)i / sampleCount;
                short sample = (short)(Mathf.Sin(Mathf.PI * 2f * frequency * t) * envelope * short.MaxValue * 0.35f);
                writer.Write(sample);
            }
        }

        string assetPath = $"{GeneratedAudioFolder}/{fileName}";
        Directory.CreateDirectory(ToAbsolutePath(GeneratedAudioFolder));
        File.WriteAllBytes(ToAbsolutePath(assetPath), data);
        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceSynchronousImport);
    }

    private static void UpdateBuildSettings()
    {
        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>();
        AddBuildSceneIfExists(scenes, MainMenuScenePath);
        AddBuildSceneIfExists(scenes, LevelSelectScenePath);

        foreach (LevelDefinition level in GetLevels())
        {
            AddBuildSceneIfExists(scenes, $"{LevelScenesFolder}/{level.SceneName}.unity");
        }

        foreach (EditorBuildSettingsScene existingScene in EditorBuildSettings.scenes)
        {
            if (existingScene == null || string.IsNullOrWhiteSpace(existingScene.path) || ContainsScene(scenes, existingScene.path))
            {
                continue;
            }

            scenes.Add(existingScene);
        }

        EditorBuildSettings.scenes = scenes.ToArray();
    }

    private static void AddBuildSceneIfExists(List<EditorBuildSettingsScene> scenes, string scenePath)
    {
        if (!File.Exists(ToAbsolutePath(scenePath)) || ContainsScene(scenes, scenePath))
        {
            return;
        }

        scenes.Add(new EditorBuildSettingsScene(scenePath, true));
    }

    private static bool ContainsScene(List<EditorBuildSettingsScene> scenes, string scenePath)
    {
        foreach (EditorBuildSettingsScene scene in scenes)
        {
            if (scene != null && scene.path == scenePath)
            {
                return true;
            }
        }

        return false;
    }

    private static void UpdateDocs()
    {
        File.WriteAllText(ToAbsolutePath("README_GAME_COMPLETE.md"), BuildReadmeText());
        File.WriteAllText(ToAbsolutePath("Assets/_Project/Docs/CONTENT_COMPLETE_MVP.md"), BuildContentMatrixText());
        AssetDatabase.ImportAsset("README_GAME_COMPLETE.md");
        AssetDatabase.ImportAsset("Assets/_Project/Docs/CONTENT_COMPLETE_MVP.md");
    }

    private static string BuildReadmeText()
    {
        return @"# Cod: Evadare - Content Complete MVP

Unity version: 2022.3.62f3 LTS

## Generate the MVP

Open the Unity project and run:

`Tools/Cod Evadare/Build Content Complete MVP`

Main scene:

`Assets/_Project/Scenes/Game/MainMenu.unity`

Level scenes:

- `Assets/_Project/Scenes/Levels/Level_01_Laboratory.unity`
- `Assets/_Project/Scenes/Levels/Level_02_Prison.unity`
- `Assets/_Project/Scenes/Levels/Level_03_ZombieCity.unity`
- `Assets/_Project/Scenes/Levels/Level_04_SciFiBase.unity`
- `Assets/_Project/Scenes/Levels/Level_05_HorrorHospital.unity`

## Controls

- WASD / Arrow Keys: Move
- Mouse: Aim
- Left Click: Shoot
- R: Reload
- E: Interact
- Escape: Pause

## Implemented Systems

Movement, aiming, shooting, ammo, reload, health, game over, doors, rooms, enemy spawners, loot, shops, buffs, bosses, boss UI, hazards, locked gates, level select, campaign unlocks, pause menu, and victory flow.

## Weapons

Pistol, SMG, Shotgun, AssaultRifle, Revolver, PlasmaRifle, ArcaneStaff.

## Levels

1. Laboratory Escape
2. Prison Escape
3. Zombie City
4. Sci-Fi Base
5. Horror Hospital

## Known Limitations

All art and audio are generated placeholders. Layouts are functional MVP combat rooms, not final designed levels. Balance requires Play Mode tuning.

## Manual Test Checklist

- New Game starts Level 1.
- Level Select locks/unlocks levels.
- Each level can be completed from start to victory.
- Shops, pickups, buffs, bosses, hazards, pause, game over, reload, and next level flow work.
";
    }

    private static string BuildContentMatrixText()
    {
        return @"# Content Complete MVP Matrix

| Level | Enemies | Miniboss | Boss | Hazards | Weapons | Special |
| --- | --- | --- | --- | --- | --- | --- |
| Laboratory Escape | LabEnemy, LabRangedEnemy | LabMiniboss | Experiment-01 | ToxicPuddle | SMG, Shotgun | Toxic lab rooms |
| Prison Escape | PrisonGuard, PrisonRangedGuard | RiotBruteMiniboss | The Warden | SecurityLaser | Shotgun, AssaultRifle | Keycard gate |
| Zombie City | ZombieWalker, ZombieRunner, ZombieSpitter, ExploderZombie | NecromancerMiniboss | AbominationBoss | ToxicSlime | AssaultRifle, Revolver | Exploding zombies |
| Sci-Fi Base | DroneEnemy, SecurityBot, LaserTurret | ReactorMiniboss | AICoreBoss | ElectricFloor | PlasmaRifle | Turrets |
| Horror Hospital | HauntedNurse, ShadowCrawler, GhostEnemy | SurgeonMiniboss | NightmareDoctorBoss | BloodHazard | ArcaneStaff | Fast horror enemies |

## Placeholder Assets

Sprites and short WAV files are generated into `Assets/_Project/Art/Generated` and `Assets/_Project/Audio/Generated`.

## Future Replacement Work

Replace generated sprites, audio, room layouts, and tuning with final art direction, level design, animation, and authored sound effects.
";
    }

    private static LevelDefinition[] GetLevels()
    {
        return new[]
        {
            new LevelDefinition
            {
                Index = 1,
                SceneName = "Level_01_Laboratory",
                DisplayName = "Laboratory Escape",
                StartObjective = "Escape the Laboratory",
                ControlsHint = "WASD move | Mouse aim | Left click shoot | R reload",
                CombatObjective = "Clear the chamber",
                SpecialObjective = "Buy supplies if needed",
                BossObjective = "Defeat Experiment-01",
                VictoryMessage = "LABORATORY CLEARED",
                ThemeColor = new Color(0.08f, 0.18f, 0.18f, 1f),
                WallSprite = "LabWall",
                FloorSprite = "LabFloor",
                HazardSprite = "ToxicPuddle",
                HazardPrefabName = "ToxicPuddle",
                CombatOneEnemies = new[] { "LabEnemy", "LabEnemy", "LabRangedEnemy" },
                CombatTwoEnemies = new[] { "LabEnemy", "LabRangedEnemy", "LabRangedEnemy", "LabEnemy" },
                MinibossPrefab = "LabMiniboss",
                BossPrefab = "Experiment01Boss",
                ShopWeapon = "SMG",
                StartingMoney = 45
            },
            new LevelDefinition
            {
                Index = 2,
                SceneName = "Level_02_Prison",
                DisplayName = "Prison Escape",
                StartObjective = "Escape the Prison",
                ControlsHint = "Find a keycard, open gates, avoid lasers",
                CombatObjective = "Survive the prison yard",
                SpecialObjective = "Armory: buy supplies if needed",
                BossObjective = "Defeat the Warden",
                VictoryMessage = "PRISON ESCAPED",
                ThemeColor = new Color(0.14f, 0.16f, 0.17f, 1f),
                WallSprite = "PrisonWall",
                FloorSprite = "PrisonFloor",
                HazardSprite = "SecurityLaser",
                HazardPrefabName = "SecurityLaser",
                UsesSecurityLasers = true,
                UsesKeycardGate = true,
                CombatOneEnemies = new[] { "PrisonGuard", "PrisonGuard", "PrisonRangedGuard" },
                CombatTwoEnemies = new[] { "PrisonGuard", "PrisonRangedGuard", "PrisonRangedGuard", "PrisonGuard" },
                MinibossPrefab = "RiotBruteMiniboss",
                BossPrefab = "WardenBoss",
                ShopWeapon = "AssaultRifle",
                StartingMoney = 55
            },
            new LevelDefinition
            {
                Index = 3,
                SceneName = "Level_03_ZombieCity",
                DisplayName = "Zombie City",
                StartObjective = "Survive Zombie City",
                ControlsHint = "Keep moving; exploding zombies punish standing still",
                CombatObjective = "Fight through the streets",
                SpecialObjective = "Survivor shop: restock ammo",
                BossObjective = "Defeat the Abomination",
                VictoryMessage = "CITY SURVIVED",
                ThemeColor = new Color(0.16f, 0.12f, 0.08f, 1f),
                WallSprite = "CityWall",
                FloorSprite = "StreetFloor",
                HazardSprite = "ToxicSlime",
                HazardPrefabName = "ToxicSlime",
                CombatOneEnemies = new[] { "ZombieWalker", "ZombieRunner", "ZombieRunner", "ZombieSpitter" },
                CombatTwoEnemies = new[] { "ZombieWalker", "ZombieRunner", "ZombieSpitter", "ExploderZombie", "ExploderZombie" },
                MinibossPrefab = "NecromancerMiniboss",
                BossPrefab = "AbominationBoss",
                ShopWeapon = "Revolver",
                StartingMoney = 60
            },
            new LevelDefinition
            {
                Index = 4,
                SceneName = "Level_04_SciFiBase",
                DisplayName = "Sci-Fi Base",
                StartObjective = "Infiltrate the Sci-Fi Base",
                ControlsHint = "Use cover against drones and turrets",
                CombatObjective = "Disable the reactor security",
                SpecialObjective = "Tech shop: buy plasma gear",
                BossObjective = "Defeat the AI Core",
                VictoryMessage = "BASE OVERRIDDEN",
                ThemeColor = new Color(0.05f, 0.11f, 0.18f, 1f),
                WallSprite = "SciFiWall",
                FloorSprite = "SciFiFloor",
                HazardSprite = "ElectricFloor",
                HazardPrefabName = "ElectricFloor",
                CombatOneEnemies = new[] { "DroneEnemy", "SecurityBot", "LaserTurret" },
                CombatTwoEnemies = new[] { "DroneEnemy", "DroneEnemy", "SecurityBot", "LaserTurret" },
                MinibossPrefab = "ReactorMiniboss",
                BossPrefab = "AICoreBoss",
                ShopWeapon = "PlasmaRifle",
                StartingMoney = 65
            },
            new LevelDefinition
            {
                Index = 5,
                SceneName = "Level_05_HorrorHospital",
                DisplayName = "Horror Hospital",
                StartObjective = "Escape the Horror Hospital",
                ControlsHint = "Fast enemies pressure health; use pickups carefully",
                CombatObjective = "Survive the wards",
                SpecialObjective = "Supply room: prepare for surgery wing",
                BossObjective = "Defeat the Nightmare Doctor",
                VictoryMessage = "ESCAPE COMPLETE\nYou survived every scenario",
                ThemeColor = new Color(0.12f, 0.04f, 0.05f, 1f),
                WallSprite = "HospitalWall",
                FloorSprite = "HospitalFloor",
                HazardSprite = "BloodHazard",
                HazardPrefabName = "BloodHazard",
                CombatOneEnemies = new[] { "HauntedNurse", "ShadowCrawler", "GhostEnemy" },
                CombatTwoEnemies = new[] { "HauntedNurse", "ShadowCrawler", "ShadowCrawler", "GhostEnemy" },
                MinibossPrefab = "SurgeonMiniboss",
                BossPrefab = "NightmareDoctorBoss",
                ShopWeapon = "ArcaneStaff",
                StartingMoney = 70
            }
        };
    }

    private sealed class SpriteSpec
    {
        public string Name;
        public Color Primary;
        public Color Secondary;
        public Color Accent;
        public int Shape;

        public SpriteSpec(string name, Color primary, Color secondary, Color accent, int shape)
        {
            Name = name;
            Primary = primary;
            Secondary = secondary;
            Accent = accent;
            Shape = shape;
        }
    }

    private static IEnumerable<SpriteSpec> GetSpriteSpecs()
    {
        Color blue = new Color(0.12f, 0.55f, 1f, 1f);
        Color red = new Color(0.95f, 0.08f, 0.08f, 1f);
        Color green = new Color(0.1f, 0.8f, 0.3f, 1f);
        Color yellow = new Color(1f, 0.78f, 0.1f, 1f);
        Color gray = new Color(0.32f, 0.42f, 0.48f, 1f);
        Color dark = new Color(0.05f, 0.07f, 0.09f, 1f);
        Color purple = new Color(0.52f, 0.18f, 0.95f, 1f);
        Color orange = new Color(0.95f, 0.42f, 0.1f, 1f);

        string[] names =
        {
            "Player_Prototype", "Bullet_Prototype", "EnemyProjectile", "Pickup_Health", "Pickup_Ammo", "Pickup_Money", "WeaponPickup", "ShopIcon_Health", "ShopIcon_Ammo", "ShopIcon_Weapon", "Door", "Cover_Block",
            "LabWall", "LabFloor", "LabEnemy", "LabRangedEnemy", "LabMiniboss", "Experiment01Boss", "ToxicPuddle",
            "PrisonWall", "PrisonFloor", "PrisonBars", "PrisonGuard", "PrisonRangedGuard", "RiotBruteMiniboss", "WardenBoss", "Keycard", "LockedGate", "SecurityLaser",
            "CityWall", "StreetFloor", "ZombieWalker", "ZombieRunner", "ZombieSpitter", "ExploderZombie", "NecromancerMiniboss", "AbominationBoss", "ToxicSlime",
            "SciFiWall", "SciFiFloor", "DroneEnemy", "SecurityBot", "LaserTurret", "ReactorMiniboss", "AICoreBoss", "ElectricFloor", "PlasmaDoor",
            "HospitalWall", "HospitalFloor", "HauntedNurse", "ShadowCrawler", "GhostEnemy", "SurgeonMiniboss", "NightmareDoctorBoss", "BloodHazard", "FlickerLight",
            "Button_Normal", "Panel_Background", "Icon_Keycard", "Icon_Buff", "Icon_Boss"
        };

        for (int i = 0; i < names.Length; i++)
        {
            Color primary = i % 7 == 0 ? blue : i % 7 == 1 ? red : i % 7 == 2 ? green : i % 7 == 3 ? yellow : i % 7 == 4 ? gray : i % 7 == 5 ? purple : orange;
            yield return new SpriteSpec(names[i], primary, dark, Color.Lerp(primary, Color.white, 0.45f), i);
        }
    }

    private static string[] GetLevelSceneNames()
    {
        LevelDefinition[] levels = GetLevels();
        string[] names = new string[levels.Length];

        for (int i = 0; i < levels.Length; i++)
        {
            names[i] = levels[i].SceneName;
        }

        return names;
    }

    private static string[] GetLevelScenePaths()
    {
        LevelDefinition[] levels = GetLevels();
        string[] paths = new string[levels.Length];

        for (int i = 0; i < levels.Length; i++)
        {
            paths[i] = $"{LevelScenesFolder}/{levels[i].SceneName}.unity";
        }

        return paths;
    }

    private static Sprite LoadSprite(string name)
    {
        return AssetDatabase.LoadAssetAtPath<Sprite>($"{GeneratedArtFolder}/{name}.png");
    }

    private static AudioClip LoadAudio(string name)
    {
        return AssetDatabase.LoadAssetAtPath<AudioClip>($"{GeneratedAudioFolder}/{name}.wav");
    }

    private static WeaponDefinition2D LoadWeapon(string name)
    {
        return AssetDatabase.LoadAssetAtPath<WeaponDefinition2D>($"Assets/_Project/ScriptableObjects/Weapons/{name}.asset");
    }

    private static GameObject LoadEnemyPrefab(string name)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/_Project/Prefabs/Enemies/{name}.prefab");
        return prefab != null ? prefab : AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/_Project/Prefabs/Boss/{name}.prefab");
    }

    private static BuffDefinition2D[] LoadAllBuffs()
    {
        List<BuffDefinition2D> buffs = new List<BuffDefinition2D>();
        string[] guids = AssetDatabase.FindAssets("t:BuffDefinition2D", new[] { "Assets/_Project/ScriptableObjects/Buffs" });

        foreach (string guid in guids)
        {
            BuffDefinition2D buff = AssetDatabase.LoadAssetAtPath<BuffDefinition2D>(AssetDatabase.GUIDToAssetPath(guid));

            if (buff != null)
            {
                buffs.Add(buff);
            }
        }

        return buffs.ToArray();
    }

    private static Material GetOrCreateMaterial(string path, Color color)
    {
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);

        if (material == null)
        {
            material = new Material(Shader.Find("Sprites/Default"));
            AssetDatabase.CreateAsset(material, path);
        }

        material.color = color;
        EditorUtility.SetDirty(material);
        return material;
    }

    private static string SanitizeAssetName(string value)
    {
        return value.Replace(" ", string.Empty).Replace("-", string.Empty).Replace(":", string.Empty);
    }

    private static string ToAbsolutePath(string assetPath)
    {
        if (Path.IsPathRooted(assetPath))
        {
            return assetPath;
        }

        string projectRoot = Directory.GetCurrentDirectory();
        return Path.Combine(projectRoot, assetPath.Replace('/', Path.DirectorySeparatorChar));
    }

    private static void AssignObjectReference(UnityEngine.Object target, string fieldName, UnityEngine.Object value)
    {
        SerializedObject serializedObject = new SerializedObject(target);
        SerializedProperty property = serializedObject.FindProperty(fieldName);

        if (property != null)
        {
            property.objectReferenceValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
        }
    }

    private static void AssignObjectReferenceArray(UnityEngine.Object target, string fieldName, UnityEngine.Object[] values)
    {
        SerializedObject serializedObject = new SerializedObject(target);
        SerializedProperty property = serializedObject.FindProperty(fieldName);

        if (property != null)
        {
            property.arraySize = values != null ? values.Length : 0;

            for (int i = 0; i < property.arraySize; i++)
            {
                property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
        }
    }

    private static void AssignStringArray(UnityEngine.Object target, string fieldName, string[] values)
    {
        SerializedObject serializedObject = new SerializedObject(target);
        SerializedProperty property = serializedObject.FindProperty(fieldName);

        if (property != null)
        {
            property.arraySize = values != null ? values.Length : 0;

            for (int i = 0; i < property.arraySize; i++)
            {
                property.GetArrayElementAtIndex(i).stringValue = values[i];
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
        }
    }

    private static void AssignString(UnityEngine.Object target, string fieldName, string value)
    {
        SerializedObject serializedObject = new SerializedObject(target);
        SerializedProperty property = serializedObject.FindProperty(fieldName);

        if (property != null)
        {
            property.stringValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
        }
    }

    private static void AssignInt(UnityEngine.Object target, string fieldName, int value)
    {
        SerializedObject serializedObject = new SerializedObject(target);
        SerializedProperty property = serializedObject.FindProperty(fieldName);

        if (property != null)
        {
            property.intValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
        }
    }

    private static void AssignFloat(UnityEngine.Object target, string fieldName, float value)
    {
        SerializedObject serializedObject = new SerializedObject(target);
        SerializedProperty property = serializedObject.FindProperty(fieldName);

        if (property != null)
        {
            property.floatValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
        }
    }

    private static void AssignBool(UnityEngine.Object target, string fieldName, bool value)
    {
        SerializedObject serializedObject = new SerializedObject(target);
        SerializedProperty property = serializedObject.FindProperty(fieldName);

        if (property != null)
        {
            property.boolValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
        }
    }

    private static void AssignEnum(UnityEngine.Object target, string fieldName, int value)
    {
        SerializedObject serializedObject = new SerializedObject(target);
        SerializedProperty property = serializedObject.FindProperty(fieldName);

        if (property != null)
        {
            property.enumValueIndex = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
        }
    }

    private static void EnsureTag(string tag)
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
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
        tagManager.ApplyModifiedPropertiesWithoutUndo();
    }
}
