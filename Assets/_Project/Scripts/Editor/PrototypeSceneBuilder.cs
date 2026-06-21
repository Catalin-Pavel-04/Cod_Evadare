#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class PrototypeSceneBuilder
{
    private const string ScenePath = "Assets/_Project/Scenes/Prototype_Lab.unity";
    private const string RoomLoopScenePath = "Assets/_Project/Scenes/Prototype_RoomLoop.unity";
    private const string HealthCombatScenePath = "Assets/_Project/Scenes/Prototype_HealthCombat.unity";
    private const string LootResourcesScenePath = "Assets/_Project/Scenes/Prototype_LootResources.unity";
    private const string WeaponLootScenePath = "Assets/_Project/Scenes/Prototype_WeaponLoot.unity";
    private const string ShopScenePath = "Assets/_Project/Scenes/Prototype_Shop.unity";
    private const string MinibossBuffScenePath = "Assets/_Project/Scenes/Prototype_MinibossBuffs.unity";
    private const string BossFightScenePath = "Assets/_Project/Scenes/Prototype_BossFight.unity";
    private const string FullLaboratoryLevelScenePath = "Assets/_Project/Scenes/Prototype_FullLaboratoryLevel.unity";
    private const string MainMenuScenePath = "Assets/_Project/Scenes/MainMenu.unity";
    private const string FinalDemoScenePath = "Assets/_Project/Scenes/Prototype_FinalDemo.unity";
    private const string PrisonLevelScenePath = "Assets/_Project/Scenes/Prototype_PrisonLevel.unity";
    private const string BalancedPrisonLevelScenePath = "Assets/_Project/Scenes/Prototype_PrisonLevel_Balanced.unity";
    private const string BulletPrefabPath = "Assets/_Project/Prefabs/Weapons/Bullet.prefab";
    private const string EnemyPrefabPath = "Assets/_Project/Prefabs/Enemies/TestEnemy.prefab";
    private const string PrisonGuardPrefabPath = "Assets/_Project/Prefabs/Enemies/PrisonGuard.prefab";
    private const string PrisonRangedGuardPrefabPath = "Assets/_Project/Prefabs/Enemies/PrisonRangedGuard.prefab";
    private const string RiotBrutePrefabPath = "Assets/_Project/Prefabs/Enemies/RiotBruteMiniboss.prefab";
    private const string BalancedPrisonGuardPrefabPath = "Assets/_Project/Prefabs/Enemies/PrisonGuard_Balanced.prefab";
    private const string BalancedPrisonRangedGuardPrefabPath = "Assets/_Project/Prefabs/Enemies/PrisonRangedGuard_Balanced.prefab";
    private const string BalancedRiotBrutePrefabPath = "Assets/_Project/Prefabs/Enemies/RiotBruteMiniboss_Balanced.prefab";
    private const string MinibossPrefabPath = "Assets/_Project/Prefabs/Enemies/PrototypeMiniboss.prefab";
    private const string BossPrefabPath = "Assets/_Project/Prefabs/Boss/Experiment01Boss.prefab";
    private const string WardenBossPrefabPath = "Assets/_Project/Prefabs/Boss/WardenBoss.prefab";
    private const string BalancedWardenBossPrefabPath = "Assets/_Project/Prefabs/Boss/WardenBoss_Balanced.prefab";
    private const string EnemyProjectilePrefabPath = "Assets/_Project/Prefabs/Projectiles/EnemyProjectile.prefab";
    private const string SecurityLaserPrefabPath = "Assets/_Project/Prefabs/Hazards/SecurityLaser.prefab";
    private const string BalancedSecurityLaserPrefabPath = "Assets/_Project/Prefabs/Hazards/SecurityLaser_Balanced.prefab";
    private const string KeycardPickupPrefabPath = "Assets/_Project/Prefabs/Prison/KeycardPickup.prefab";
    private const string LockedGatePrefabPath = "Assets/_Project/Prefabs/Prison/LockedGate.prefab";
    private const string RewardPrefabPath = "Assets/_Project/Prefabs/Pickups/PrototypeReward.prefab";
    private const string HealthPickupPrefabPath = "Assets/_Project/Prefabs/Pickups/HealthPickup.prefab";
    private const string AmmoPickupPrefabPath = "Assets/_Project/Prefabs/Pickups/AmmoPickup.prefab";
    private const string MoneyPickupPrefabPath = "Assets/_Project/Prefabs/Pickups/MoneyPickup.prefab";
    private const string BalancedHealthPickupPrefabPath = "Assets/_Project/Prefabs/Pickups/HealthPickup_Balanced.prefab";
    private const string BalancedAmmoPickupPrefabPath = "Assets/_Project/Prefabs/Pickups/AmmoPickup_Balanced.prefab";
    private const string BalancedMoneyPickupPrefabPath = "Assets/_Project/Prefabs/Pickups/MoneyPickup_Balanced.prefab";
    private const string SMGPickupPrefabPath = "Assets/_Project/Prefabs/Pickups/SMGPickup.prefab";
    private const string ShotgunPickupPrefabPath = "Assets/_Project/Prefabs/Pickups/ShotgunPickup.prefab";
    private const string ShopHealthPrefabPath = "Assets/_Project/Prefabs/Shop/ShopItem_Health.prefab";
    private const string ShopAmmoPrefabPath = "Assets/_Project/Prefabs/Shop/ShopItem_Ammo.prefab";
    private const string ShopShotgunPrefabPath = "Assets/_Project/Prefabs/Shop/ShopItem_Shotgun.prefab";
    private const string BalancedShopHealthPrefabPath = "Assets/_Project/Prefabs/Shop/ShopItem_Health_Balanced.prefab";
    private const string BalancedShopAmmoPrefabPath = "Assets/_Project/Prefabs/Shop/ShopItem_Ammo_Balanced.prefab";
    private const string BalancedShopWeaponPrefabPath = "Assets/_Project/Prefabs/Shop/ShopItem_Weapon_Balanced.prefab";
    private const string PistolWeaponPath = "Assets/_Project/ScriptableObjects/Weapons/Pistol.asset";
    private const string SMGWeaponPath = "Assets/_Project/ScriptableObjects/Weapons/SMG.asset";
    private const string ShotgunWeaponPath = "Assets/_Project/ScriptableObjects/Weapons/Shotgun.asset";
    private const string ShopHealthDefinitionPath = "Assets/_Project/ScriptableObjects/Shop/Shop_HealthSmall.asset";
    private const string ShopAmmoDefinitionPath = "Assets/_Project/ScriptableObjects/Shop/Shop_AmmoPack.asset";
    private const string ShopShotgunDefinitionPath = "Assets/_Project/ScriptableObjects/Shop/Shop_Shotgun.asset";
    private const string BalancedShopHealthDefinitionPath = "Assets/_Project/ScriptableObjects/Shop/Shop_HealthSmall_Balanced.asset";
    private const string BalancedShopAmmoDefinitionPath = "Assets/_Project/ScriptableObjects/Shop/Shop_AmmoPack_Balanced.asset";
    private const string BalancedShopWeaponDefinitionPath = "Assets/_Project/ScriptableObjects/Shop/Shop_Weapon_Balanced.asset";
    private const string PrisonBalanceProfilePath = "Assets/_Project/ScriptableObjects/Balance/PrisonBalanceProfile_Normal.asset";
    private const string BuffMaxHealthPath = "Assets/_Project/ScriptableObjects/Buffs/Buff_MaxHealth.asset";
    private const string BuffHealPath = "Assets/_Project/ScriptableObjects/Buffs/Buff_Heal.asset";
    private const string BuffMoveSpeedPath = "Assets/_Project/ScriptableObjects/Buffs/Buff_MoveSpeed.asset";
    private const string BuffFireRatePath = "Assets/_Project/ScriptableObjects/Buffs/Buff_FireRate.asset";
    private const string BuffDamagePath = "Assets/_Project/ScriptableObjects/Buffs/Buff_Damage.asset";
    private const string BuffReloadPath = "Assets/_Project/ScriptableObjects/Buffs/Buff_Reload.asset";
    private const string BuffMaxAmmoPath = "Assets/_Project/ScriptableObjects/Buffs/Buff_MaxAmmo.asset";
    private const string BuffMoneyPath = "Assets/_Project/ScriptableObjects/Buffs/Buff_Money.asset";
    private const string GeneratedArtFolder = "Assets/_Project/Art/Generated";
    private const string PlayerSpritePath = GeneratedArtFolder + "/Player_Prototype.png";
    private const string EnemySpritePath = GeneratedArtFolder + "/Enemy_Prototype.png";
    private const string MinibossSpritePath = GeneratedArtFolder + "/Miniboss_Prototype.png";
    private const string BossSpritePath = GeneratedArtFolder + "/Boss_Experiment01.png";
    private const string BulletSpritePath = GeneratedArtFolder + "/Bullet_Prototype.png";
    private const string EnemyProjectileSpritePath = GeneratedArtFolder + "/EnemyProjectile.png";
    private const string WallSpritePath = GeneratedArtFolder + "/Wall_Prototype.png";
    private const string BossArenaWallSpritePath = GeneratedArtFolder + "/BossArenaWall.png";
    private const string CoverSpritePath = GeneratedArtFolder + "/Cover_Block.png";
    private const string RewardSpritePath = GeneratedArtFolder + "/Reward_Prototype.png";
    private const string HealthPickupSpritePath = GeneratedArtFolder + "/HealthPickup_Prototype.png";
    private const string AmmoPickupSpritePath = GeneratedArtFolder + "/AmmoPickup_Prototype.png";
    private const string MoneyPickupSpritePath = GeneratedArtFolder + "/MoneyPickup_Prototype.png";
    private const string PistolSpritePath = GeneratedArtFolder + "/Pistol_Prototype.png";
    private const string SMGSpritePath = GeneratedArtFolder + "/SMG_Prototype.png";
    private const string ShotgunSpritePath = GeneratedArtFolder + "/Shotgun_Prototype.png";
    private const string WeaponPickupSpritePath = GeneratedArtFolder + "/WeaponPickup_Prototype.png";
    private const string ShopHealthSpritePath = GeneratedArtFolder + "/ShopHealth_Prototype.png";
    private const string ShopAmmoSpritePath = GeneratedArtFolder + "/ShopAmmo_Prototype.png";
    private const string ShopWeaponSpritePath = GeneratedArtFolder + "/ShopWeapon_Prototype.png";
    private const string PrisonPlayerStartMarkerSpritePath = GeneratedArtFolder + "/Prison_PlayerStartMarker.png";
    private const string PrisonWallSpritePath = GeneratedArtFolder + "/PrisonWall.png";
    private const string PrisonBarsSpritePath = GeneratedArtFolder + "/PrisonBars.png";
    private const string PrisonFloorSpritePath = GeneratedArtFolder + "/PrisonFloor.png";
    private const string PrisonGuardSpritePath = GeneratedArtFolder + "/PrisonGuard.png";
    private const string PrisonRangedGuardSpritePath = GeneratedArtFolder + "/PrisonRangedGuard.png";
    private const string PrisonBruteMinibossSpritePath = GeneratedArtFolder + "/PrisonBruteMiniboss.png";
    private const string WardenBossSpritePath = GeneratedArtFolder + "/WardenBoss.png";
    private const string KeycardSpritePath = GeneratedArtFolder + "/Keycard.png";
    private const string LockedGateSpritePath = GeneratedArtFolder + "/LockedGate.png";
    private const string SecurityLaserSpritePath = GeneratedArtFolder + "/SecurityLaser.png";
    private const string PrisonCoverSpritePath = GeneratedArtFolder + "/PrisonCover.png";
    private const string AlarmLightSpritePath = GeneratedArtFolder + "/AlarmLight.png";
    private const string PlayerTag = "Player";
    private const string GeneratedAudioFolder = "Assets/_Project/Audio/Generated";
    private const string ShootAudioPath = GeneratedAudioFolder + "/Shoot.wav";
    private const string HitAudioPath = GeneratedAudioFolder + "/Hit.wav";
    private const string PickupAudioPath = GeneratedAudioFolder + "/Pickup.wav";
    private const string DoorAudioPath = GeneratedAudioFolder + "/Door.wav";
    private const string ShopAudioPath = GeneratedAudioFolder + "/Shop.wav";
    private const string BuffAudioPath = GeneratedAudioFolder + "/Buff.wav";
    private const string BossPhaseAudioPath = GeneratedAudioFolder + "/BossPhase.wav";
    private const string VictoryAudioPath = GeneratedAudioFolder + "/Victory.wav";
    private const string GameOverAudioPath = GeneratedAudioFolder + "/GameOver.wav";
    private const string LaserAudioPath = GeneratedAudioFolder + "/Laser.wav";
    private const string KeycardAudioPath = GeneratedAudioFolder + "/Keycard.wav";

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
        "Assets/_Project/Prefabs/Boss",
        "Assets/_Project/Prefabs/Projectiles",
        "Assets/_Project/Prefabs/Environment",
        "Assets/_Project/Prefabs/Hazards",
        "Assets/_Project/Prefabs/Prison",
        "Assets/_Project/Prefabs/UI",
        "Assets/_Project/Prefabs/Pickups",
        "Assets/_Project/Prefabs/Loot",
        "Assets/_Project/Prefabs/Buffs",
        "Assets/_Project/Prefabs/Rooms",
        "Assets/_Project/Prefabs/Shop",
        "Assets/_Project/Scenes",
        "Assets/_Project/ScriptableObjects",
        "Assets/_Project/ScriptableObjects/Weapons",
        "Assets/_Project/ScriptableObjects/Shop",
        "Assets/_Project/ScriptableObjects/Buffs",
        "Assets/_Project/ScriptableObjects/Balance",
        "Assets/_Project/Docs",
        "Assets/_Project/Scripts",
        "Assets/_Project/Scripts/Balance",
        "Assets/_Project/Scripts/Buffs",
        "Assets/_Project/Scripts/Boss",
        "Assets/_Project/Scripts/Audio",
        "Assets/_Project/Scripts/Core",
        "Assets/_Project/Scripts/Environment",
        "Assets/_Project/Scripts/Feedback",
        "Assets/_Project/Scripts/Level",
        "Assets/_Project/Scripts/Loot",
        "Assets/_Project/Scripts/Hazards",
        "Assets/_Project/Scripts/Prison",
        "Assets/_Project/Scripts/Projectiles",
        "Assets/_Project/Scripts/Resources",
        "Assets/_Project/Scripts/Shop",
        "Assets/_Project/Scripts/UI",
        "Assets/_Project/Scripts/Player",
        "Assets/_Project/Scripts/Weapons",
        "Assets/_Project/Scripts/Enemies",
        "Assets/_Project/Scripts/Rooms",
        "Assets/_Project/Scripts/Pickups",
        "Assets/_Project/Scripts/Menu",
        "Assets/_Project/Scripts/Camera",
        "Assets/_Project/Scripts/Editor",
        "Assets/_Project/Audio/Generated"
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

    [MenuItem("Tools/Cod Evadare/Create Prototype 0.5 Weapon Loot Scene")]
    public static void CreatePrototypeWeaponLootScene()
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
        Sprite weaponPickupSprite = AssetDatabase.LoadAssetAtPath<Sprite>(WeaponPickupSpritePath);

        GameObject root = new GameObject("Prototype_WeaponLoot");
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
        WeaponDefinition2D pistol = CreateWeaponDefinition(PistolWeaponPath, "Pistol", WeaponRarity2D.Common, bulletPrefab, 0.2f, 12, 1, 1f, 1, 12f, 2f, 1, 0f);
        WeaponDefinition2D smg = CreateWeaponDefinition(SMGWeaponPath, "SMG", WeaponRarity2D.Rare, bulletPrefab, 0.08f, 24, 1, 1.2f, 1, 13f, 2f, 1, 3f);
        WeaponDefinition2D shotgun = CreateWeaponDefinition(ShotgunWeaponPath, "Shotgun", WeaponRarity2D.Epic, bulletPrefab, 0.55f, 6, 1, 1.4f, 1, 11f, 1.2f, 5, 35f);
        GameObject smgPickup = CreateWeaponPickupPrefab("SMGPickup", SMGPickupPrefabPath, weaponPickupSprite, smg);
        GameObject shotgunPickup = CreateWeaponPickupPrefab("ShotgunPickup", ShotgunPickupPrefabPath, weaponPickupSprite, shotgun);

        AssignObjectReference(playerShooting, "firePoint", firePoint);
        AssignObjectReference(playerShooting, "bulletPrefab", bulletPrefab);
        AssignBool(playerShooting, "useAmmo", true);
        AssignObjectReference(playerShooting, "playerResources", playerResources);
        AssignObjectReference(playerShooting, "startingWeapon", pistol);
        AssignObjectReference(playerShooting, "equippedWeapon", pistol);

        CameraFollow2D cameraFollow = cameraObject.GetComponent<CameraFollow2D>();
        AssignObjectReference(cameraFollow, "target", player.transform);

        GameObject enemyPrefab = CreateEnemyPrefab(enemySprite);
        GameObject[] lootPrefabs =
        {
            CreateResourcePickupPrefab("HealthPickup", HealthPickupPrefabPath, healthPickupSprite, ResourcePickupType.Health, 2),
            CreateResourcePickupPrefab("AmmoPickup", AmmoPickupPrefabPath, ammoPickupSprite, ResourcePickupType.Ammo, 10),
            CreateResourcePickupPrefab("MoneyPickup", MoneyPickupPrefabPath, moneyPickupSprite, ResourcePickupType.Money, 25),
            shotgunPickup != null ? shotgunPickup : smgPickup
        };

        CreateWeaponLootRoom(root.transform, wallSprite, enemyPrefab, lootPrefabs);

        GameObject gameOverPanel = CreateWeaponLootUI(root.transform, playerHealth, playerResources, playerShooting);
        CreateGameSystems(root.transform, playerHealth, gameOverPanel);

        EditorSceneManager.SaveScene(scene, WeaponLootScenePath);
        AddSceneToBuildSettings(WeaponLootScenePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Created Prototype 0.5 weapon loot scene at {WeaponLootScenePath}.");
    }

    [MenuItem("Tools/Cod Evadare/Create Prototype 0.6 Shop Scene")]
    public static void CreatePrototypeShopScene()
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
        Sprite moneyPickupSprite = AssetDatabase.LoadAssetAtPath<Sprite>(MoneyPickupSpritePath);
        Sprite shopHealthSprite = AssetDatabase.LoadAssetAtPath<Sprite>(ShopHealthSpritePath);
        Sprite shopAmmoSprite = AssetDatabase.LoadAssetAtPath<Sprite>(ShopAmmoSpritePath);
        Sprite shopWeaponSprite = AssetDatabase.LoadAssetAtPath<Sprite>(ShopWeaponSpritePath);

        GameObject root = new GameObject("Prototype_Shop");
        GameObject cameraObject = CreateCamera(root.transform);
        CreateLighting(root.transform);

        GameObject player = CreatePlayer(root.transform, playerSprite, out Transform firePoint, out PlayerShooting2D playerShooting);
        player.transform.position = new Vector3(-4.5f, 0f, 0f);

        PlayerHealth2D playerHealth = player.AddComponent<PlayerHealth2D>();
        AssignInt(playerHealth, "maxHealth", 5);
        AssignFloat(playerHealth, "invincibilityDuration", 0.75f);
        AssignBool(playerHealth, "destroyOnDeath", false);

        PlayerResources2D playerResources = player.AddComponent<PlayerResources2D>();
        AssignInt(playerResources, "startingAmmo", 30);
        AssignInt(playerResources, "maxAmmo", 99);
        AssignInt(playerResources, "startingMoney", 100);

        GameObject bulletPrefab = GetOrCreateBulletPrefab(bulletSprite);
        WeaponDefinition2D pistol = CreateWeaponDefinition(PistolWeaponPath, "Pistol", WeaponRarity2D.Common, bulletPrefab, 0.2f, 12, 1, 1f, 1, 12f, 2f, 1, 0f);
        CreateWeaponDefinition(SMGWeaponPath, "SMG", WeaponRarity2D.Rare, bulletPrefab, 0.08f, 24, 1, 1.2f, 1, 13f, 2f, 1, 3f);
        WeaponDefinition2D shotgun = CreateWeaponDefinition(ShotgunWeaponPath, "Shotgun", WeaponRarity2D.Epic, bulletPrefab, 0.55f, 6, 1, 1.4f, 1, 11f, 1.2f, 5, 35f);

        AssignObjectReference(playerShooting, "firePoint", firePoint);
        AssignObjectReference(playerShooting, "bulletPrefab", bulletPrefab);
        AssignBool(playerShooting, "useAmmo", true);
        AssignObjectReference(playerShooting, "playerResources", playerResources);
        AssignObjectReference(playerShooting, "startingWeapon", pistol);
        AssignObjectReference(playerShooting, "equippedWeapon", pistol);

        CameraFollow2D cameraFollow = cameraObject.GetComponent<CameraFollow2D>();
        AssignObjectReference(cameraFollow, "target", player.transform);

        GameObject enemyPrefab = CreateEnemyPrefab(enemySprite);
        GameObject moneyPickup = CreateResourcePickupPrefab("MoneyPickup", MoneyPickupPrefabPath, moneyPickupSprite, ResourcePickupType.Money, 25);
        CreateShopCombatRoom(root.transform, wallSprite, enemyPrefab, moneyPickup);

        ShopItemDefinition2D healthShopItem = CreateShopItemDefinition(ShopHealthDefinitionPath, "Health +2", ShopItemType2D.Health, 25, 2, null, shopHealthSprite);
        ShopItemDefinition2D ammoShopItem = CreateShopItemDefinition(ShopAmmoDefinitionPath, "Ammo +15", ShopItemType2D.Ammo, 20, 15, null, shopAmmoSprite);
        ShopItemDefinition2D shotgunShopItem = CreateShopItemDefinition(ShopShotgunDefinitionPath, "Shotgun", ShopItemType2D.Weapon, 75, 0, shotgun, shopWeaponSprite);

        GameObject healthShopPrefab = CreateShopItemPrefab("ShopItem_Health", ShopHealthPrefabPath, shopHealthSprite, healthShopItem);
        GameObject ammoShopPrefab = CreateShopItemPrefab("ShopItem_Ammo", ShopAmmoPrefabPath, shopAmmoSprite, ammoShopItem);
        GameObject shotgunShopPrefab = CreateShopItemPrefab("ShopItem_Shotgun", ShopShotgunPrefabPath, shopWeaponSprite, shotgunShopItem);

        GameObject gameOverPanel = CreateShopSceneUI(root.transform, playerHealth, playerResources, playerShooting, out ShopUI2D shopUI);
        GameObject shopArea = CreateShopArea(root.transform, wallSprite, shopUI, healthShopPrefab, ammoShopPrefab, shotgunShopPrefab);
        shopArea.transform.SetSiblingIndex(4);
        CreateGameSystems(root.transform, playerHealth, gameOverPanel);

        EditorSceneManager.SaveScene(scene, ShopScenePath);
        AddSceneToBuildSettings(ShopScenePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Created Prototype 0.6 shop scene at {ShopScenePath}.");
    }

    [MenuItem("Tools/Cod Evadare/Create Prototype 0.7 Miniboss Buff Scene")]
    public static void CreatePrototypeMinibossBuffScene()
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
        Sprite minibossSprite = AssetDatabase.LoadAssetAtPath<Sprite>(MinibossSpritePath);
        Sprite bulletSprite = AssetDatabase.LoadAssetAtPath<Sprite>(BulletSpritePath);
        Sprite wallSprite = AssetDatabase.LoadAssetAtPath<Sprite>(WallSpritePath);

        GameObject root = new GameObject("Prototype_MinibossBuffs");
        GameObject cameraObject = CreateCamera(root.transform);
        CreateLighting(root.transform);

        GameObject player = CreatePlayer(root.transform, playerSprite, out Transform firePoint, out PlayerShooting2D playerShooting);
        player.transform.position = new Vector3(-4.5f, 0f, 0f);

        PlayerHealth2D playerHealth = player.AddComponent<PlayerHealth2D>();
        AssignInt(playerHealth, "maxHealth", 5);
        AssignFloat(playerHealth, "invincibilityDuration", 0.75f);
        AssignBool(playerHealth, "destroyOnDeath", false);

        PlayerResources2D playerResources = player.AddComponent<PlayerResources2D>();
        AssignInt(playerResources, "startingAmmo", 60);
        AssignInt(playerResources, "maxAmmo", 120);
        AssignInt(playerResources, "startingMoney", 50);

        PlayerBuffs2D playerBuffs = player.AddComponent<PlayerBuffs2D>();

        GameObject bulletPrefab = GetOrCreateBulletPrefab(bulletSprite);
        WeaponDefinition2D pistol = CreateWeaponDefinition(PistolWeaponPath, "Pistol", WeaponRarity2D.Common, bulletPrefab, 0.2f, 12, 1, 1f, 1, 12f, 2f, 1, 0f);

        AssignObjectReference(playerShooting, "firePoint", firePoint);
        AssignObjectReference(playerShooting, "bulletPrefab", bulletPrefab);
        AssignBool(playerShooting, "useAmmo", true);
        AssignObjectReference(playerShooting, "playerResources", playerResources);
        AssignObjectReference(playerShooting, "startingWeapon", pistol);
        AssignObjectReference(playerShooting, "equippedWeapon", pistol);

        CameraFollow2D cameraFollow = cameraObject.GetComponent<CameraFollow2D>();
        AssignObjectReference(cameraFollow, "target", player.transform);

        BuffDefinition2D[] buffPool = CreatePrototypeBuffDefinitions();
        GameObject gameOverPanel = CreateMinibossBuffUI(root.transform, playerHealth, playerResources, playerShooting, playerBuffs, out GameObject choicePanel, out Button[] choiceButtons, out Text[] choiceTexts);
        BuffChoiceController2D buffChoiceController = CreateMinibossGameSystems(root.transform, playerHealth, gameOverPanel, playerBuffs, buffPool, choicePanel, choiceButtons, choiceTexts);

        GameObject minibossPrefab = CreateMinibossPrefab(minibossSprite);
        CreateMinibossRoom(root.transform, wallSprite, minibossPrefab, buffChoiceController);
        CreateEventSystem(root.transform);

        EditorSceneManager.SaveScene(scene, MinibossBuffScenePath);
        AddSceneToBuildSettings(MinibossBuffScenePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Created Prototype 0.7 miniboss buff scene at {MinibossBuffScenePath}.");
    }

    [MenuItem("Tools/Cod Evadare/Create Prototype 0.8 Boss Fight Scene")]
    public static void CreatePrototypeBossFightScene()
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
        Sprite bulletSprite = AssetDatabase.LoadAssetAtPath<Sprite>(BulletSpritePath);
        Sprite wallSprite = AssetDatabase.LoadAssetAtPath<Sprite>(BossArenaWallSpritePath);
        Sprite bossSprite = AssetDatabase.LoadAssetAtPath<Sprite>(BossSpritePath);
        Sprite enemyProjectileSprite = AssetDatabase.LoadAssetAtPath<Sprite>(EnemyProjectileSpritePath);
        Sprite coverSprite = AssetDatabase.LoadAssetAtPath<Sprite>(CoverSpritePath);

        GameObject root = new GameObject("Prototype_BossFight");
        GameObject cameraObject = CreateCamera(root.transform);
        CreateLighting(root.transform);

        GameObject player = CreatePlayer(root.transform, playerSprite, out Transform firePoint, out PlayerShooting2D playerShooting);
        player.transform.position = new Vector3(-9f, 0f, 0f);

        PlayerHealth2D playerHealth = player.AddComponent<PlayerHealth2D>();
        AssignInt(playerHealth, "maxHealth", 6);
        AssignFloat(playerHealth, "invincibilityDuration", 0.75f);
        AssignBool(playerHealth, "destroyOnDeath", false);

        PlayerResources2D playerResources = player.AddComponent<PlayerResources2D>();
        AssignInt(playerResources, "startingAmmo", 90);
        AssignInt(playerResources, "maxAmmo", 150);
        AssignInt(playerResources, "startingMoney", 0);

        PlayerBuffs2D playerBuffs = player.AddComponent<PlayerBuffs2D>();

        GameObject bulletPrefab = GetOrCreateBulletPrefab(bulletSprite);
        WeaponDefinition2D smg = CreateWeaponDefinition(SMGWeaponPath, "SMG", WeaponRarity2D.Uncommon, bulletPrefab, 0.08f, 30, 1, 1.1f, 1, 14f, 2f, 1, 5f);
        CreateWeaponDefinition(ShotgunWeaponPath, "Shotgun", WeaponRarity2D.Rare, bulletPrefab, 0.55f, 6, 1, 1.35f, 1, 11f, 1.3f, 5, 32f);

        AssignObjectReference(playerShooting, "firePoint", firePoint);
        AssignObjectReference(playerShooting, "bulletPrefab", bulletPrefab);
        AssignBool(playerShooting, "useAmmo", true);
        AssignObjectReference(playerShooting, "playerResources", playerResources);
        AssignObjectReference(playerShooting, "startingWeapon", smg);
        AssignObjectReference(playerShooting, "equippedWeapon", smg);

        CameraFollow2D cameraFollow = cameraObject.GetComponent<CameraFollow2D>();
        AssignObjectReference(cameraFollow, "target", player.transform);

        GameObject enemyProjectilePrefab = CreateEnemyProjectilePrefab(enemyProjectileSprite);
        GameObject bossPrefab = CreateBossPrefab(bossSprite, enemyProjectilePrefab);

        GameObject gameOverPanel = CreateBossFightUI(root.transform, playerHealth, playerResources, playerShooting, playerBuffs, out GameObject victoryPanel, out Text victoryText);
        LevelEndController2D levelEndController = CreateBossFightGameSystems(root.transform, playerHealth, gameOverPanel, victoryPanel, victoryText);

        CreateBossRoom(root.transform, wallSprite, coverSprite, bossPrefab, levelEndController);
        CreateEventSystem(root.transform);

        EditorSceneManager.SaveScene(scene, BossFightScenePath);
        AddSceneToBuildSettings(BossFightScenePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Created Prototype 0.8 boss fight scene at {BossFightScenePath}.");
    }

    [MenuItem("Tools/Cod Evadare/Create Prototype 0.9 Full Laboratory Level")]
    public static void CreatePrototypeFullLaboratoryLevel()
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
        Sprite minibossSprite = AssetDatabase.LoadAssetAtPath<Sprite>(MinibossSpritePath);
        Sprite bossSprite = AssetDatabase.LoadAssetAtPath<Sprite>(BossSpritePath);
        Sprite bulletSprite = AssetDatabase.LoadAssetAtPath<Sprite>(BulletSpritePath);
        Sprite enemyProjectileSprite = AssetDatabase.LoadAssetAtPath<Sprite>(EnemyProjectileSpritePath);
        Sprite wallSprite = AssetDatabase.LoadAssetAtPath<Sprite>(WallSpritePath);
        Sprite bossWallSprite = AssetDatabase.LoadAssetAtPath<Sprite>(BossArenaWallSpritePath);
        Sprite coverSprite = AssetDatabase.LoadAssetAtPath<Sprite>(CoverSpritePath);
        Sprite healthPickupSprite = AssetDatabase.LoadAssetAtPath<Sprite>(HealthPickupSpritePath);
        Sprite ammoPickupSprite = AssetDatabase.LoadAssetAtPath<Sprite>(AmmoPickupSpritePath);
        Sprite moneyPickupSprite = AssetDatabase.LoadAssetAtPath<Sprite>(MoneyPickupSpritePath);
        Sprite weaponPickupSprite = AssetDatabase.LoadAssetAtPath<Sprite>(WeaponPickupSpritePath);
        Sprite shopHealthSprite = AssetDatabase.LoadAssetAtPath<Sprite>(ShopHealthSpritePath);
        Sprite shopAmmoSprite = AssetDatabase.LoadAssetAtPath<Sprite>(ShopAmmoSpritePath);
        Sprite shopWeaponSprite = AssetDatabase.LoadAssetAtPath<Sprite>(ShopWeaponSpritePath);

        GameObject root = new GameObject("Prototype_FullLaboratoryLevel");
        GameObject cameraObject = CreateCamera(root.transform);
        CreateLighting(root.transform);

        GameObject player = CreatePlayer(root.transform, playerSprite, out Transform firePoint, out PlayerShooting2D playerShooting);
        player.transform.position = new Vector3(-24f, 0f, 0f);

        PlayerHealth2D playerHealth = player.AddComponent<PlayerHealth2D>();
        AssignInt(playerHealth, "maxHealth", 6);
        AssignFloat(playerHealth, "invincibilityDuration", 0.75f);
        AssignBool(playerHealth, "destroyOnDeath", false);

        PlayerResources2D playerResources = player.AddComponent<PlayerResources2D>();
        AssignInt(playerResources, "startingAmmo", 90);
        AssignInt(playerResources, "maxAmmo", 150);
        AssignInt(playerResources, "startingMoney", 50);

        PlayerBuffs2D playerBuffs = player.AddComponent<PlayerBuffs2D>();

        GameObject bulletPrefab = GetOrCreateBulletPrefab(bulletSprite);
        WeaponDefinition2D pistol = CreateWeaponDefinition(PistolWeaponPath, "Pistol", WeaponRarity2D.Common, bulletPrefab, 0.2f, 12, 1, 1f, 1, 12f, 2f, 1, 0f);
        CreateWeaponDefinition(SMGWeaponPath, "SMG", WeaponRarity2D.Rare, bulletPrefab, 0.08f, 24, 1, 1.2f, 1, 13f, 2f, 1, 3f);
        WeaponDefinition2D shotgun = CreateWeaponDefinition(ShotgunWeaponPath, "Shotgun", WeaponRarity2D.Epic, bulletPrefab, 0.55f, 6, 1, 1.4f, 1, 11f, 1.2f, 5, 35f);

        AssignObjectReference(playerShooting, "firePoint", firePoint);
        AssignObjectReference(playerShooting, "bulletPrefab", bulletPrefab);
        AssignBool(playerShooting, "useAmmo", true);
        AssignObjectReference(playerShooting, "playerResources", playerResources);
        AssignObjectReference(playerShooting, "startingWeapon", pistol);
        AssignObjectReference(playerShooting, "equippedWeapon", pistol);

        CameraFollow2D cameraFollow = cameraObject.GetComponent<CameraFollow2D>();
        AssignObjectReference(cameraFollow, "target", player.transform);

        GameObject enemyPrefab = CreateEnemyPrefab(enemySprite);
        GameObject minibossPrefab = CreateMinibossPrefab(minibossSprite);
        GameObject enemyProjectilePrefab = CreateEnemyProjectilePrefab(enemyProjectileSprite);
        GameObject bossPrefab = CreateBossPrefab(bossSprite, enemyProjectilePrefab);

        GameObject healthPickup = CreateResourcePickupPrefab("HealthPickup", HealthPickupPrefabPath, healthPickupSprite, ResourcePickupType.Health, 2);
        GameObject ammoPickup = CreateResourcePickupPrefab("AmmoPickup", AmmoPickupPrefabPath, ammoPickupSprite, ResourcePickupType.Ammo, 25);
        GameObject moneyPickup = CreateResourcePickupPrefab("MoneyPickup", MoneyPickupPrefabPath, moneyPickupSprite, ResourcePickupType.Money, 25);
        GameObject shotgunPickup = CreateWeaponPickupPrefab("ShotgunPickup", ShotgunPickupPrefabPath, weaponPickupSprite, shotgun);

        ShopItemDefinition2D healthShopItem = CreateShopItemDefinition(ShopHealthDefinitionPath, "Health +2", ShopItemType2D.Health, 25, 2, null, shopHealthSprite);
        ShopItemDefinition2D ammoShopItem = CreateShopItemDefinition(ShopAmmoDefinitionPath, "Ammo +15", ShopItemType2D.Ammo, 20, 15, null, shopAmmoSprite);
        ShopItemDefinition2D shotgunShopItem = CreateShopItemDefinition(ShopShotgunDefinitionPath, "Shotgun", ShopItemType2D.Weapon, 75, 0, shotgun, shopWeaponSprite);
        GameObject healthShopPrefab = CreateShopItemPrefab("ShopItem_Health", ShopHealthPrefabPath, shopHealthSprite, healthShopItem);
        GameObject ammoShopPrefab = CreateShopItemPrefab("ShopItem_Ammo", ShopAmmoPrefabPath, shopAmmoSprite, ammoShopItem);
        GameObject shotgunShopPrefab = CreateShopItemPrefab("ShopItem_Weapon", ShopShotgunPrefabPath, shopWeaponSprite, shotgunShopItem);

        BuffDefinition2D[] buffPool = CreatePrototypeBuffDefinitions();
        GameObject gameOverPanel = CreateFullLaboratoryUI(
            root.transform,
            playerHealth,
            playerResources,
            playerShooting,
            playerBuffs,
            out ObjectiveUI2D objectiveUI,
            out ShopUI2D shopUI,
            out GameObject choicePanel,
            out Button[] choiceButtons,
            out Text[] choiceTexts,
            out GameObject victoryPanel,
            out Text victoryText);

        CreateFullLaboratoryGameSystems(
            root.transform,
            playerHealth,
            gameOverPanel,
            playerBuffs,
            buffPool,
            choicePanel,
            choiceButtons,
            choiceTexts,
            objectiveUI,
            victoryPanel,
            victoryText,
            out BuffChoiceController2D buffChoiceController,
            out LevelEndController2D levelEndController);

        CreateStartArea(root.transform, objectiveUI);
        CreateFullCombatRoom(
            root.transform,
            "Combat_Room_01",
            -16f,
            wallSprite,
            enemyPrefab,
            3,
            new[] { healthPickup, ammoPickup, moneyPickup },
            objectiveUI,
            "Clear the first laboratory chamber",
            "Collect supplies, then move to the shop");

        CreateFullShopArea(root.transform, -5f, wallSprite, shopUI, objectiveUI, healthShopPrefab, ammoShopPrefab, shotgunShopPrefab);

        CreateFullCombatRoom(
            root.transform,
            "Combat_Room_02",
            7f,
            wallSprite,
            enemyPrefab,
            4,
            new[] { healthPickup, ammoPickup, moneyPickup, shotgunPickup },
            objectiveUI,
            "Clear the second laboratory chamber",
            "Proceed to the miniboss room");

        CreateFullMinibossRoom(root.transform, 19f, wallSprite, minibossPrefab, buffChoiceController, objectiveUI);
        CreateFullBossRoom(root.transform, 34f, bossWallSprite, coverSprite, bossPrefab, levelEndController, objectiveUI);
        CreateEventSystem(root.transform);

        EditorSceneManager.SaveScene(scene, FullLaboratoryLevelScenePath);
        AddSceneToBuildSettings(FullLaboratoryLevelScenePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Created Prototype 0.9 full laboratory level at {FullLaboratoryLevelScenePath}.");
    }

    [MenuItem("Tools/Cod Evadare/Create Main Menu Scene")]
    public static void CreateMainMenuScene()
    {
        if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return;
        }

        CreateRequiredFolders();

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        SceneManager.SetActiveScene(scene);

        GameObject root = new GameObject("MainMenu");
        CreateMenuCamera(root.transform);
        CreateMainMenuUI(root.transform);
        CreateEventSystem(root.transform);

        EditorSceneManager.SaveScene(scene, MainMenuScenePath);
        AddSceneToBuildSettingsFirst(MainMenuScenePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Created main menu scene at {MainMenuScenePath}.");
    }

    [MenuItem("Tools/Cod Evadare/Create Prototype 1.0 Final Demo")]
    public static void CreatePrototypeFinalDemo()
    {
        if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return;
        }

        CreateRequiredFolders();
        GeneratePlaceholderSprites();
        GeneratePlaceholderAudio();
        EnsureTag(PlayerTag);

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        SceneManager.SetActiveScene(scene);

        Sprite playerSprite = AssetDatabase.LoadAssetAtPath<Sprite>(PlayerSpritePath);
        Sprite enemySprite = AssetDatabase.LoadAssetAtPath<Sprite>(EnemySpritePath);
        Sprite minibossSprite = AssetDatabase.LoadAssetAtPath<Sprite>(MinibossSpritePath);
        Sprite bossSprite = AssetDatabase.LoadAssetAtPath<Sprite>(BossSpritePath);
        Sprite bulletSprite = AssetDatabase.LoadAssetAtPath<Sprite>(BulletSpritePath);
        Sprite enemyProjectileSprite = AssetDatabase.LoadAssetAtPath<Sprite>(EnemyProjectileSpritePath);
        Sprite wallSprite = AssetDatabase.LoadAssetAtPath<Sprite>(WallSpritePath);
        Sprite bossWallSprite = AssetDatabase.LoadAssetAtPath<Sprite>(BossArenaWallSpritePath);
        Sprite coverSprite = AssetDatabase.LoadAssetAtPath<Sprite>(CoverSpritePath);
        Sprite healthPickupSprite = AssetDatabase.LoadAssetAtPath<Sprite>(HealthPickupSpritePath);
        Sprite ammoPickupSprite = AssetDatabase.LoadAssetAtPath<Sprite>(AmmoPickupSpritePath);
        Sprite moneyPickupSprite = AssetDatabase.LoadAssetAtPath<Sprite>(MoneyPickupSpritePath);
        Sprite weaponPickupSprite = AssetDatabase.LoadAssetAtPath<Sprite>(WeaponPickupSpritePath);
        Sprite shopHealthSprite = AssetDatabase.LoadAssetAtPath<Sprite>(ShopHealthSpritePath);
        Sprite shopAmmoSprite = AssetDatabase.LoadAssetAtPath<Sprite>(ShopAmmoSpritePath);
        Sprite shopWeaponSprite = AssetDatabase.LoadAssetAtPath<Sprite>(ShopWeaponSpritePath);

        GameObject root = new GameObject("Prototype_FinalDemo");
        GameObject cameraObject = CreateCamera(root.transform);
        SimpleCameraShake2D cameraShake = cameraObject.AddComponent<SimpleCameraShake2D>();
        AssignFloat(cameraShake, "defaultDuration", 0.12f);
        AssignFloat(cameraShake, "defaultMagnitude", 0.12f);
        CreateLighting(root.transform);

        GameObject player = CreatePlayer(root.transform, playerSprite, out Transform firePoint, out PlayerShooting2D playerShooting);
        player.transform.position = new Vector3(-24f, 0f, 0f);
        SpriteFlash2D playerFlash = player.AddComponent<SpriteFlash2D>();
        AssignObjectReference(playerFlash, "spriteRenderer", player.GetComponent<SpriteRenderer>());

        PlayerHealth2D playerHealth = player.AddComponent<PlayerHealth2D>();
        AssignInt(playerHealth, "maxHealth", 6);
        AssignFloat(playerHealth, "invincibilityDuration", 0.75f);
        AssignBool(playerHealth, "destroyOnDeath", false);

        PlayerResources2D playerResources = player.AddComponent<PlayerResources2D>();
        AssignInt(playerResources, "startingAmmo", 100);
        AssignInt(playerResources, "maxAmmo", 160);
        AssignInt(playerResources, "startingMoney", 50);

        PlayerBuffs2D playerBuffs = player.AddComponent<PlayerBuffs2D>();

        GameObject bulletPrefab = GetOrCreateBulletPrefab(bulletSprite);
        WeaponDefinition2D pistol = CreateWeaponDefinition(PistolWeaponPath, "Pistol", WeaponRarity2D.Common, bulletPrefab, 0.2f, 12, 1, 1f, 1, 12f, 2f, 1, 0f);
        CreateWeaponDefinition(SMGWeaponPath, "SMG", WeaponRarity2D.Rare, bulletPrefab, 0.08f, 24, 1, 1.2f, 1, 13f, 2f, 1, 3f);
        WeaponDefinition2D shotgun = CreateWeaponDefinition(ShotgunWeaponPath, "Shotgun", WeaponRarity2D.Epic, bulletPrefab, 0.55f, 6, 1, 1.4f, 1, 11f, 1.2f, 5, 35f);

        AssignObjectReference(playerShooting, "firePoint", firePoint);
        AssignObjectReference(playerShooting, "bulletPrefab", bulletPrefab);
        AssignBool(playerShooting, "useAmmo", true);
        AssignObjectReference(playerShooting, "playerResources", playerResources);
        AssignObjectReference(playerShooting, "startingWeapon", pistol);
        AssignObjectReference(playerShooting, "equippedWeapon", pistol);

        CameraFollow2D cameraFollow = cameraObject.GetComponent<CameraFollow2D>();
        AssignObjectReference(cameraFollow, "target", player.transform);

        GameObject enemyPrefab = CreateEnemyPrefab(enemySprite);
        GameObject minibossPrefab = CreateMinibossPrefab(minibossSprite);
        GameObject enemyProjectilePrefab = CreateEnemyProjectilePrefab(enemyProjectileSprite);
        GameObject bossPrefab = CreateBossPrefab(bossSprite, enemyProjectilePrefab);

        GameObject healthPickup = CreateResourcePickupPrefab("HealthPickup", HealthPickupPrefabPath, healthPickupSprite, ResourcePickupType.Health, 2);
        GameObject ammoPickup = CreateResourcePickupPrefab("AmmoPickup", AmmoPickupPrefabPath, ammoPickupSprite, ResourcePickupType.Ammo, 25);
        GameObject moneyPickup = CreateResourcePickupPrefab("MoneyPickup", MoneyPickupPrefabPath, moneyPickupSprite, ResourcePickupType.Money, 25);
        GameObject shotgunPickup = CreateWeaponPickupPrefab("ShotgunPickup", ShotgunPickupPrefabPath, weaponPickupSprite, shotgun);

        ShopItemDefinition2D healthShopItem = CreateShopItemDefinition(ShopHealthDefinitionPath, "Health +2", ShopItemType2D.Health, 25, 2, null, shopHealthSprite);
        ShopItemDefinition2D ammoShopItem = CreateShopItemDefinition(ShopAmmoDefinitionPath, "Ammo +15", ShopItemType2D.Ammo, 20, 15, null, shopAmmoSprite);
        ShopItemDefinition2D shotgunShopItem = CreateShopItemDefinition(ShopShotgunDefinitionPath, "Shotgun", ShopItemType2D.Weapon, 75, 0, shotgun, shopWeaponSprite);
        GameObject healthShopPrefab = CreateShopItemPrefab("ShopItem_Health", ShopHealthPrefabPath, shopHealthSprite, healthShopItem);
        GameObject ammoShopPrefab = CreateShopItemPrefab("ShopItem_Ammo", ShopAmmoPrefabPath, shopAmmoSprite, ammoShopItem);
        GameObject shotgunShopPrefab = CreateShopItemPrefab("ShopItem_Weapon", ShopShotgunPrefabPath, shopWeaponSprite, shotgunShopItem);

        BuffDefinition2D[] buffPool = CreatePrototypeBuffDefinitions();
        GameObject gameOverPanel = CreateFullLaboratoryUI(
            root.transform,
            playerHealth,
            playerResources,
            playerShooting,
            playerBuffs,
            out ObjectiveUI2D objectiveUI,
            out ShopUI2D shopUI,
            out GameObject choicePanel,
            out Button[] choiceButtons,
            out Text[] choiceTexts,
            out GameObject victoryPanel,
            out Text victoryText);

        AddFinalDemoUiOverlays(root.transform, out GameObject pausePanel, out DemoMessageUI2D demoMessageUI);

        CreateFinalDemoGameSystems(
            root.transform,
            playerHealth,
            gameOverPanel,
            playerBuffs,
            buffPool,
            choicePanel,
            choiceButtons,
            choiceTexts,
            objectiveUI,
            victoryPanel,
            victoryText,
            pausePanel,
            demoMessageUI,
            out BuffChoiceController2D buffChoiceController,
            out LevelEndController2D levelEndController);

        CreateStartArea(root.transform, objectiveUI);
        CreateFullCombatRoom(root.transform, "Combat_Room_01", -16f, wallSprite, enemyPrefab, 3, new[] { healthPickup, ammoPickup, moneyPickup }, objectiveUI, "Clear the first laboratory chamber", "Collect supplies, then move to the shop");
        CreateFullShopArea(root.transform, -5f, wallSprite, shopUI, objectiveUI, healthShopPrefab, ammoShopPrefab, shotgunShopPrefab);
        CreateFullCombatRoom(root.transform, "Combat_Room_02", 7f, wallSprite, enemyPrefab, 4, new[] { healthPickup, ammoPickup, moneyPickup, shotgunPickup }, objectiveUI, "Clear the second laboratory chamber", "Proceed to the miniboss room");
        CreateFullMinibossRoom(root.transform, 19f, wallSprite, minibossPrefab, buffChoiceController, objectiveUI);
        CreateFullBossRoom(root.transform, 34f, bossWallSprite, coverSprite, bossPrefab, levelEndController, objectiveUI);
        CreateEventSystem(root.transform);

        EditorSceneManager.SaveScene(scene, FinalDemoScenePath);
        AddSceneToBuildSettings(FinalDemoScenePath);

        if (File.Exists(ToAbsoluteAssetPath(MainMenuScenePath)))
        {
            AddSceneToBuildSettingsFirst(MainMenuScenePath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Created Prototype 1.0 final demo scene at {FinalDemoScenePath}.");
    }

    [MenuItem("Tools/Cod Evadare/Create Prototype 2.0 Prison Level")]
    public static void CreatePrototypePrisonLevel()
    {
        if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return;
        }

        CreateRequiredFolders();
        GeneratePlaceholderSprites();
        GeneratePlaceholderAudio();
        EnsureTag(PlayerTag);

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        SceneManager.SetActiveScene(scene);

        Sprite playerSprite = AssetDatabase.LoadAssetAtPath<Sprite>(PlayerSpritePath);
        Sprite bulletSprite = AssetDatabase.LoadAssetAtPath<Sprite>(BulletSpritePath);
        Sprite enemyProjectileSprite = AssetDatabase.LoadAssetAtPath<Sprite>(EnemyProjectileSpritePath);
        Sprite prisonWallSprite = AssetDatabase.LoadAssetAtPath<Sprite>(PrisonWallSpritePath);
        Sprite prisonBarsSprite = AssetDatabase.LoadAssetAtPath<Sprite>(PrisonBarsSpritePath);
        Sprite prisonGuardSprite = AssetDatabase.LoadAssetAtPath<Sprite>(PrisonGuardSpritePath);
        Sprite prisonRangedGuardSprite = AssetDatabase.LoadAssetAtPath<Sprite>(PrisonRangedGuardSpritePath);
        Sprite prisonBruteSprite = AssetDatabase.LoadAssetAtPath<Sprite>(PrisonBruteMinibossSpritePath);
        Sprite wardenSprite = AssetDatabase.LoadAssetAtPath<Sprite>(WardenBossSpritePath);
        Sprite keycardSprite = AssetDatabase.LoadAssetAtPath<Sprite>(KeycardSpritePath);
        Sprite lockedGateSprite = AssetDatabase.LoadAssetAtPath<Sprite>(LockedGateSpritePath);
        Sprite securityLaserSprite = AssetDatabase.LoadAssetAtPath<Sprite>(SecurityLaserSpritePath);
        Sprite prisonCoverSprite = AssetDatabase.LoadAssetAtPath<Sprite>(PrisonCoverSpritePath);
        Sprite healthPickupSprite = AssetDatabase.LoadAssetAtPath<Sprite>(HealthPickupSpritePath);
        Sprite ammoPickupSprite = AssetDatabase.LoadAssetAtPath<Sprite>(AmmoPickupSpritePath);
        Sprite moneyPickupSprite = AssetDatabase.LoadAssetAtPath<Sprite>(MoneyPickupSpritePath);
        Sprite weaponPickupSprite = AssetDatabase.LoadAssetAtPath<Sprite>(WeaponPickupSpritePath);
        Sprite shopHealthSprite = AssetDatabase.LoadAssetAtPath<Sprite>(ShopHealthSpritePath);
        Sprite shopAmmoSprite = AssetDatabase.LoadAssetAtPath<Sprite>(ShopAmmoSpritePath);
        Sprite shopWeaponSprite = AssetDatabase.LoadAssetAtPath<Sprite>(ShopWeaponSpritePath);

        GameObject root = new GameObject("Prototype_PrisonLevel");
        GameObject cameraObject = CreateCamera(root.transform);
        SimpleCameraShake2D cameraShake = cameraObject.AddComponent<SimpleCameraShake2D>();
        AssignFloat(cameraShake, "defaultDuration", 0.12f);
        AssignFloat(cameraShake, "defaultMagnitude", 0.12f);
        CreateLighting(root.transform);

        GameObject player = CreatePlayer(root.transform, playerSprite, out Transform firePoint, out PlayerShooting2D playerShooting);
        player.transform.position = new Vector3(-28f, 0f, 0f);
        SpriteFlash2D playerFlash = player.AddComponent<SpriteFlash2D>();
        AssignObjectReference(playerFlash, "spriteRenderer", player.GetComponent<SpriteRenderer>());

        PlayerHealth2D playerHealth = player.AddComponent<PlayerHealth2D>();
        AssignInt(playerHealth, "maxHealth", 6);
        AssignFloat(playerHealth, "invincibilityDuration", 0.75f);
        AssignBool(playerHealth, "destroyOnDeath", false);

        PlayerResources2D playerResources = player.AddComponent<PlayerResources2D>();
        AssignInt(playerResources, "startingAmmo", 90);
        AssignInt(playerResources, "maxAmmo", 150);
        AssignInt(playerResources, "startingMoney", 40);

        PlayerBuffs2D playerBuffs = player.AddComponent<PlayerBuffs2D>();
        PlayerKeyring2D playerKeyring = player.AddComponent<PlayerKeyring2D>();
        AssignInt(playerKeyring, "startingKeycards", 0);

        GameObject bulletPrefab = GetOrCreateBulletPrefab(bulletSprite);
        WeaponDefinition2D pistol = CreateWeaponDefinition(PistolWeaponPath, "Pistol", WeaponRarity2D.Common, bulletPrefab, 0.2f, 12, 1, 1f, 1, 12f, 2f, 1, 0f);
        WeaponDefinition2D smg = CreateWeaponDefinition(SMGWeaponPath, "SMG", WeaponRarity2D.Rare, bulletPrefab, 0.08f, 24, 1, 1.2f, 1, 13f, 2f, 1, 3f);
        WeaponDefinition2D shotgun = CreateWeaponDefinition(ShotgunWeaponPath, "Shotgun", WeaponRarity2D.Epic, bulletPrefab, 0.55f, 6, 1, 1.4f, 1, 11f, 1.2f, 5, 35f);

        AssignObjectReference(playerShooting, "firePoint", firePoint);
        AssignObjectReference(playerShooting, "bulletPrefab", bulletPrefab);
        AssignBool(playerShooting, "useAmmo", true);
        AssignObjectReference(playerShooting, "playerResources", playerResources);
        AssignObjectReference(playerShooting, "startingWeapon", pistol);
        AssignObjectReference(playerShooting, "equippedWeapon", pistol);

        CameraFollow2D cameraFollow = cameraObject.GetComponent<CameraFollow2D>();
        AssignObjectReference(cameraFollow, "target", player.transform);

        GameObject enemyProjectilePrefab = CreateEnemyProjectilePrefab(enemyProjectileSprite);
        GameObject prisonGuardPrefab = CreatePrisonGuardPrefab(prisonGuardSprite);
        GameObject prisonRangedGuardPrefab = CreatePrisonRangedGuardPrefab(prisonRangedGuardSprite, enemyProjectilePrefab);
        GameObject riotBrutePrefab = CreateRiotBrutePrefab(prisonBruteSprite);
        GameObject wardenBossPrefab = CreateWardenBossPrefab(wardenSprite, enemyProjectilePrefab);
        GameObject keycardPickupPrefab = CreateKeycardPickupPrefab(keycardSprite);
        GameObject lockedGatePrefab = CreateLockedGatePrefab(lockedGateSprite);
        GameObject securityLaserPrefab = CreateSecurityLaserPrefab(securityLaserSprite);

        GameObject healthPickup = CreateResourcePickupPrefab("HealthPickup", HealthPickupPrefabPath, healthPickupSprite, ResourcePickupType.Health, 2);
        GameObject ammoPickup = CreateResourcePickupPrefab("AmmoPickup", AmmoPickupPrefabPath, ammoPickupSprite, ResourcePickupType.Ammo, 25);
        GameObject moneyPickup = CreateResourcePickupPrefab("MoneyPickup", MoneyPickupPrefabPath, moneyPickupSprite, ResourcePickupType.Money, 25);
        GameObject shotgunPickup = CreateWeaponPickupPrefab("ShotgunPickup", ShotgunPickupPrefabPath, weaponPickupSprite, shotgun);

        ShopItemDefinition2D healthShopItem = CreateShopItemDefinition(ShopHealthDefinitionPath, "Health +2", ShopItemType2D.Health, 25, 2, null, shopHealthSprite);
        ShopItemDefinition2D ammoShopItem = CreateShopItemDefinition(ShopAmmoDefinitionPath, "Ammo +20", ShopItemType2D.Ammo, 20, 20, null, shopAmmoSprite);
        ShopItemDefinition2D weaponShopItem = CreateShopItemDefinition(ShopShotgunDefinitionPath, "SMG", ShopItemType2D.Weapon, 70, 0, smg, shopWeaponSprite);
        GameObject healthShopPrefab = CreateShopItemPrefab("ShopItem_Health", ShopHealthPrefabPath, shopHealthSprite, healthShopItem);
        GameObject ammoShopPrefab = CreateShopItemPrefab("ShopItem_Ammo", ShopAmmoPrefabPath, shopAmmoSprite, ammoShopItem);
        GameObject weaponShopPrefab = CreateShopItemPrefab("ShopItem_Weapon", ShopShotgunPrefabPath, shopWeaponSprite, weaponShopItem);

        BuffDefinition2D[] buffPool = CreatePrototypeBuffDefinitions();
        GameObject gameOverPanel = CreateFullLaboratoryUI(
            root.transform,
            playerHealth,
            playerResources,
            playerShooting,
            playerBuffs,
            out ObjectiveUI2D objectiveUI,
            out ShopUI2D shopUI,
            out GameObject choicePanel,
            out Button[] choiceButtons,
            out Text[] choiceTexts,
            out GameObject victoryPanel,
            out Text victoryText);

        AddPrisonKeycardUI(root.transform, playerKeyring);
        AddFinalDemoUiOverlays(root.transform, out GameObject pausePanel, out DemoMessageUI2D demoMessageUI);
        Text gatePromptText = root.transform.Find("UI/Canvas/DemoMessageText")?.GetComponent<Text>();

        CreatePrisonGameSystems(
            root.transform,
            playerHealth,
            gameOverPanel,
            playerBuffs,
            buffPool,
            choicePanel,
            choiceButtons,
            choiceTexts,
            objectiveUI,
            victoryPanel,
            victoryText,
            pausePanel,
            demoMessageUI,
            out BuffChoiceController2D buffChoiceController,
            out LevelEndController2D levelEndController);

        CreatePrisonStartCell(root.transform, prisonWallSprite, prisonBarsSprite, objectiveUI);
        CreatePrisonGuardRoom(root.transform, -18f, prisonWallSprite, prisonBarsSprite, prisonGuardPrefab, prisonRangedGuardPrefab, new[] { keycardPickupPrefab, ammoPickup, moneyPickup }, objectiveUI);
        CreatePrisonLockedGateArea(root.transform, -9f, lockedGatePrefab, gatePromptText, objectiveUI);
        CreatePrisonShopArea(root.transform, -1f, prisonWallSprite, shopUI, objectiveUI, healthShopPrefab, ammoShopPrefab, weaponShopPrefab);
        CreatePrisonYard(root.transform, 11f, prisonWallSprite, prisonCoverSprite, securityLaserPrefab, prisonGuardPrefab, prisonRangedGuardPrefab, new[] { healthPickup, ammoPickup, moneyPickup, shotgunPickup }, objectiveUI);
        CreatePrisonMinibossRoom(root.transform, 23f, prisonWallSprite, riotBrutePrefab, buffChoiceController, objectiveUI);
        CreatePrisonBossRoom(root.transform, 38f, prisonWallSprite, prisonCoverSprite, securityLaserPrefab, wardenBossPrefab, levelEndController, objectiveUI);
        CreateEventSystem(root.transform);

        EditorSceneManager.SaveScene(scene, PrisonLevelScenePath);
        AddSceneToBuildSettings(PrisonLevelScenePath);

        if (File.Exists(ToAbsoluteAssetPath(MainMenuScenePath)))
        {
            AddSceneToBuildSettingsFirst(MainMenuScenePath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Created Prototype 2.0 prison level at {PrisonLevelScenePath}.");
    }

    [MenuItem("Tools/Cod Evadare/Create Prototype 2.1 Balanced Prison Level")]
    public static void CreatePrototypeBalancedPrisonLevel()
    {
        if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return;
        }

        CreateRequiredFolders();
        GeneratePlaceholderSprites();
        GeneratePlaceholderAudio();
        EnsureTag(PlayerTag);

        PrisonBalanceProfile2D balance = GetOrCreatePrisonBalanceProfile();

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        SceneManager.SetActiveScene(scene);

        Sprite playerSprite = AssetDatabase.LoadAssetAtPath<Sprite>(PlayerSpritePath);
        Sprite bulletSprite = AssetDatabase.LoadAssetAtPath<Sprite>(BulletSpritePath);
        Sprite enemyProjectileSprite = AssetDatabase.LoadAssetAtPath<Sprite>(EnemyProjectileSpritePath);
        Sprite prisonWallSprite = AssetDatabase.LoadAssetAtPath<Sprite>(PrisonWallSpritePath);
        Sprite prisonBarsSprite = AssetDatabase.LoadAssetAtPath<Sprite>(PrisonBarsSpritePath);
        Sprite prisonGuardSprite = AssetDatabase.LoadAssetAtPath<Sprite>(PrisonGuardSpritePath);
        Sprite prisonRangedGuardSprite = AssetDatabase.LoadAssetAtPath<Sprite>(PrisonRangedGuardSpritePath);
        Sprite prisonBruteSprite = AssetDatabase.LoadAssetAtPath<Sprite>(PrisonBruteMinibossSpritePath);
        Sprite wardenSprite = AssetDatabase.LoadAssetAtPath<Sprite>(WardenBossSpritePath);
        Sprite keycardSprite = AssetDatabase.LoadAssetAtPath<Sprite>(KeycardSpritePath);
        Sprite lockedGateSprite = AssetDatabase.LoadAssetAtPath<Sprite>(LockedGateSpritePath);
        Sprite securityLaserSprite = AssetDatabase.LoadAssetAtPath<Sprite>(SecurityLaserSpritePath);
        Sprite prisonCoverSprite = AssetDatabase.LoadAssetAtPath<Sprite>(PrisonCoverSpritePath);
        Sprite alarmLightSprite = AssetDatabase.LoadAssetAtPath<Sprite>(AlarmLightSpritePath);
        Sprite healthPickupSprite = AssetDatabase.LoadAssetAtPath<Sprite>(HealthPickupSpritePath);
        Sprite ammoPickupSprite = AssetDatabase.LoadAssetAtPath<Sprite>(AmmoPickupSpritePath);
        Sprite moneyPickupSprite = AssetDatabase.LoadAssetAtPath<Sprite>(MoneyPickupSpritePath);
        Sprite weaponPickupSprite = AssetDatabase.LoadAssetAtPath<Sprite>(WeaponPickupSpritePath);
        Sprite shopHealthSprite = AssetDatabase.LoadAssetAtPath<Sprite>(ShopHealthSpritePath);
        Sprite shopAmmoSprite = AssetDatabase.LoadAssetAtPath<Sprite>(ShopAmmoSpritePath);
        Sprite shopWeaponSprite = AssetDatabase.LoadAssetAtPath<Sprite>(ShopWeaponSpritePath);

        GameObject root = new GameObject("Prototype_PrisonLevel_Balanced");
        GameObject cameraObject = CreateCamera(root.transform);
        SimpleCameraShake2D cameraShake = cameraObject.AddComponent<SimpleCameraShake2D>();
        AssignFloat(cameraShake, "defaultDuration", 0.1f);
        AssignFloat(cameraShake, "defaultMagnitude", 0.1f);
        CreateLighting(root.transform);

        GameObject player = CreatePlayer(root.transform, playerSprite, out Transform firePoint, out PlayerShooting2D playerShooting);
        player.transform.position = new Vector3(-28f, 0f, 0f);
        SpriteFlash2D playerFlash = player.AddComponent<SpriteFlash2D>();
        AssignObjectReference(playerFlash, "spriteRenderer", player.GetComponent<SpriteRenderer>());

        PlayerHealth2D playerHealth = player.AddComponent<PlayerHealth2D>();
        AssignInt(playerHealth, "maxHealth", balance.PlayerMaxHealth);
        AssignFloat(playerHealth, "invincibilityDuration", 0.75f);
        AssignBool(playerHealth, "destroyOnDeath", false);

        PlayerResources2D playerResources = player.AddComponent<PlayerResources2D>();
        AssignInt(playerResources, "startingAmmo", balance.StartingAmmo);
        AssignInt(playerResources, "maxAmmo", balance.MaxAmmo);
        AssignInt(playerResources, "startingMoney", balance.StartingMoney);

        PlayerBuffs2D playerBuffs = player.AddComponent<PlayerBuffs2D>();
        PlayerKeyring2D playerKeyring = player.AddComponent<PlayerKeyring2D>();
        AssignInt(playerKeyring, "startingKeycards", 0);

        GameObject bulletPrefab = GetOrCreateBulletPrefab(bulletSprite);
        WeaponDefinition2D pistol = CreateWeaponDefinition(PistolWeaponPath, "Pistol", WeaponRarity2D.Common, bulletPrefab, 0.2f, 12, 1, 1f, 1, 12f, 2f, 1, 0f);
        WeaponDefinition2D smg = CreateWeaponDefinition(SMGWeaponPath, "SMG", WeaponRarity2D.Rare, bulletPrefab, 0.08f, 24, 1, 1.2f, 1, 13f, 2f, 1, 3f);
        WeaponDefinition2D shotgun = CreateWeaponDefinition(ShotgunWeaponPath, "Shotgun", WeaponRarity2D.Epic, bulletPrefab, 0.55f, 6, 1, 1.4f, 1, 11f, 1.2f, 5, 35f);

        AssignObjectReference(playerShooting, "firePoint", firePoint);
        AssignObjectReference(playerShooting, "bulletPrefab", bulletPrefab);
        AssignBool(playerShooting, "useAmmo", true);
        AssignObjectReference(playerShooting, "playerResources", playerResources);
        AssignObjectReference(playerShooting, "startingWeapon", pistol);
        AssignObjectReference(playerShooting, "equippedWeapon", pistol);

        CameraFollow2D cameraFollow = cameraObject.GetComponent<CameraFollow2D>();
        AssignObjectReference(cameraFollow, "target", player.transform);

        GameObject enemyProjectilePrefab = CreateEnemyProjectilePrefab(enemyProjectileSprite);
        GameObject prisonGuardPrefab = CreateBalancedPrisonGuardPrefab(prisonGuardSprite, balance);
        GameObject prisonRangedGuardPrefab = CreateBalancedPrisonRangedGuardPrefab(prisonRangedGuardSprite, enemyProjectilePrefab, balance);
        GameObject riotBrutePrefab = CreateBalancedRiotBrutePrefab(prisonBruteSprite, balance);
        GameObject wardenBossPrefab = CreateBalancedWardenBossPrefab(wardenSprite, enemyProjectilePrefab, balance);
        GameObject keycardPickupPrefab = CreateKeycardPickupPrefab(keycardSprite);
        GameObject lockedGatePrefab = CreateLockedGatePrefab(lockedGateSprite);
        GameObject securityLaserPrefab = CreateBalancedSecurityLaserPrefab(securityLaserSprite, balance);

        GameObject healthPickup = CreateResourcePickupPrefab("HealthPickup_Balanced", BalancedHealthPickupPrefabPath, healthPickupSprite, ResourcePickupType.Health, balance.HealthPickupAmount);
        GameObject ammoPickup = CreateResourcePickupPrefab("AmmoPickup_Balanced", BalancedAmmoPickupPrefabPath, ammoPickupSprite, ResourcePickupType.Ammo, balance.AmmoPickupAmount);
        GameObject moneyPickup = CreateResourcePickupPrefab("MoneyPickup_Balanced", BalancedMoneyPickupPrefabPath, moneyPickupSprite, ResourcePickupType.Money, balance.MoneyPickupAmountSmall);
        GameObject shotgunPickup = CreateWeaponPickupPrefab("ShotgunPickup", ShotgunPickupPrefabPath, weaponPickupSprite, shotgun);

        ShopItemDefinition2D healthShopItem = CreateShopItemDefinition(BalancedShopHealthDefinitionPath, "Health +2", ShopItemType2D.Health, balance.ShopHealthPrice, balance.HealthPickupAmount, null, shopHealthSprite);
        ShopItemDefinition2D ammoShopItem = CreateShopItemDefinition(BalancedShopAmmoDefinitionPath, "Ammo +18", ShopItemType2D.Ammo, balance.ShopAmmoPrice, balance.AmmoPickupAmount, null, shopAmmoSprite);
        ShopItemDefinition2D weaponShopItem = CreateShopItemDefinition(BalancedShopWeaponDefinitionPath, "SMG", ShopItemType2D.Weapon, balance.ShopWeaponPrice, 0, smg, shopWeaponSprite);
        GameObject healthShopPrefab = CreateShopItemPrefab("ShopItem_Health_Balanced", BalancedShopHealthPrefabPath, shopHealthSprite, healthShopItem);
        GameObject ammoShopPrefab = CreateShopItemPrefab("ShopItem_Ammo_Balanced", BalancedShopAmmoPrefabPath, shopAmmoSprite, ammoShopItem);
        GameObject weaponShopPrefab = CreateShopItemPrefab("ShopItem_Weapon_Balanced", BalancedShopWeaponPrefabPath, shopWeaponSprite, weaponShopItem);

        BuffDefinition2D[] buffPool = CreatePrototypeBuffDefinitions();
        GameObject gameOverPanel = CreateFullLaboratoryUI(
            root.transform,
            playerHealth,
            playerResources,
            playerShooting,
            playerBuffs,
            out ObjectiveUI2D objectiveUI,
            out ShopUI2D shopUI,
            out GameObject choicePanel,
            out Button[] choiceButtons,
            out Text[] choiceTexts,
            out GameObject victoryPanel,
            out Text victoryText);

        AddPrisonKeycardUI(root.transform, playerKeyring);
        AddFinalDemoUiOverlays(root.transform, out GameObject pausePanel, out DemoMessageUI2D demoMessageUI);
        Text gatePromptText = root.transform.Find("UI/Canvas/DemoMessageText")?.GetComponent<Text>();

        CreatePrisonGameSystems(
            root.transform,
            playerHealth,
            gameOverPanel,
            playerBuffs,
            buffPool,
            choicePanel,
            choiceButtons,
            choiceTexts,
            objectiveUI,
            victoryPanel,
            victoryText,
            pausePanel,
            demoMessageUI,
            out BuffChoiceController2D buffChoiceController,
            out LevelEndController2D levelEndController,
            "Prison Escape",
            "Escape the Prison",
            "Find a keycard to unlock the security gate",
            "PRISON ESCAPED");

        CreateBalancedPrisonStartCell(root.transform, prisonWallSprite, prisonBarsSprite, objectiveUI);
        CreateBalancedPrisonGuardRoom(root.transform, -18f, prisonWallSprite, prisonBarsSprite, prisonCoverSprite, prisonGuardPrefab, prisonRangedGuardPrefab, new[] { keycardPickupPrefab, ammoPickup, moneyPickup }, objectiveUI);
        CreateBalancedPrisonLockedGateArea(root.transform, -9f, lockedGatePrefab, prisonWallSprite, keycardSprite, gatePromptText, objectiveUI);
        CreatePrisonShopArea(root.transform, -1f, prisonWallSprite, shopUI, objectiveUI, healthShopPrefab, ammoShopPrefab, weaponShopPrefab);
        CreateBalancedPrisonYard(root.transform, 11f, prisonWallSprite, prisonCoverSprite, securityLaserPrefab, prisonGuardPrefab, prisonRangedGuardPrefab, new[] { healthPickup, ammoPickup, moneyPickup, shotgunPickup }, objectiveUI);
        CreateBalancedPrisonMinibossRoom(root.transform, 23f, prisonWallSprite, prisonCoverSprite, riotBrutePrefab, buffChoiceController, objectiveUI);
        CreateBalancedPrisonBossRoom(root.transform, 38f, prisonWallSprite, prisonCoverSprite, securityLaserPrefab, wardenBossPrefab, levelEndController, objectiveUI);
        CreatePrisonAlarmLights(root.transform, alarmLightSprite);
        CreateEventSystem(root.transform);

        EditorSceneManager.SaveScene(scene, BalancedPrisonLevelScenePath);
        AddSceneToBuildSettings(BalancedPrisonLevelScenePath);

        if (File.Exists(ToAbsoluteAssetPath(MainMenuScenePath)))
        {
            AddSceneToBuildSettingsFirst(MainMenuScenePath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Created Prototype 2.1 balanced prison level at {BalancedPrisonLevelScenePath}.");
    }

    [MenuItem("Tools/Cod Evadare/Update Main Menu With Level 2")]
    public static void UpdateMainMenuWithLevel2()
    {
        CreateMainMenuScene();
        bool addedAnyPrisonScene = false;

        if (File.Exists(ToAbsoluteAssetPath(PrisonLevelScenePath)))
        {
            AddSceneToBuildSettings(PrisonLevelScenePath);
            addedAnyPrisonScene = true;
        }

        if (File.Exists(ToAbsoluteAssetPath(BalancedPrisonLevelScenePath)))
        {
            AddSceneToBuildSettings(BalancedPrisonLevelScenePath);
            addedAnyPrisonScene = true;
        }

        if (!addedAnyPrisonScene)
        {
            Debug.LogWarning($"Main menu was updated, but no prison scene exists yet. Run Tools/Cod Evadare/Create Prototype 2.0 Prison Level or Tools/Cod Evadare/Create Prototype 2.1 Balanced Prison Level before using the Level 2 buttons.");
        }
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
        WriteSpriteTexture(MinibossSpritePath, 64, CreateMinibossPixel);
        WriteSpriteTexture(BossSpritePath, 64, CreateBossPixel);
        WriteSpriteTexture(BulletSpritePath, 32, CreateBulletPixel);
        WriteSpriteTexture(EnemyProjectileSpritePath, 32, CreateEnemyProjectilePixel);
        WriteSpriteTexture(WallSpritePath, 64, CreateWallPixel);
        WriteSpriteTexture(BossArenaWallSpritePath, 64, CreateBossArenaWallPixel);
        WriteSpriteTexture(CoverSpritePath, 64, CreateCoverPixel);
        WriteSpriteTexture(RewardSpritePath, 64, CreateRewardPixel);
        WriteSpriteTexture(HealthPickupSpritePath, 64, CreateHealthPickupPixel);
        WriteSpriteTexture(AmmoPickupSpritePath, 64, CreateAmmoPickupPixel);
        WriteSpriteTexture(MoneyPickupSpritePath, 64, CreateMoneyPickupPixel);
        WriteSpriteTexture(PistolSpritePath, 64, CreatePistolPixel);
        WriteSpriteTexture(SMGSpritePath, 64, CreateSMGPixel);
        WriteSpriteTexture(ShotgunSpritePath, 64, CreateShotgunPixel);
        WriteSpriteTexture(WeaponPickupSpritePath, 64, CreateWeaponPickupPixel);
        WriteSpriteTexture(ShopHealthSpritePath, 64, CreateShopHealthPixel);
        WriteSpriteTexture(ShopAmmoSpritePath, 64, CreateShopAmmoPixel);
        WriteSpriteTexture(ShopWeaponSpritePath, 64, CreateShopWeaponPixel);
        WriteSpriteTexture(PrisonPlayerStartMarkerSpritePath, 64, CreatePrisonPlayerStartMarkerPixel);
        WriteSpriteTexture(PrisonWallSpritePath, 64, CreatePrisonWallPixel);
        WriteSpriteTexture(PrisonBarsSpritePath, 64, CreatePrisonBarsPixel);
        WriteSpriteTexture(PrisonFloorSpritePath, 64, CreatePrisonFloorPixel);
        WriteSpriteTexture(PrisonGuardSpritePath, 64, CreatePrisonGuardPixel);
        WriteSpriteTexture(PrisonRangedGuardSpritePath, 64, CreatePrisonRangedGuardPixel);
        WriteSpriteTexture(PrisonBruteMinibossSpritePath, 64, CreatePrisonBruteMinibossPixel);
        WriteSpriteTexture(WardenBossSpritePath, 64, CreateWardenBossPixel);
        WriteSpriteTexture(KeycardSpritePath, 64, CreateKeycardPixel);
        WriteSpriteTexture(LockedGateSpritePath, 64, CreateLockedGatePixel);
        WriteSpriteTexture(SecurityLaserSpritePath, 64, CreateSecurityLaserPixel);
        WriteSpriteTexture(PrisonCoverSpritePath, 64, CreatePrisonCoverPixel);
        WriteSpriteTexture(AlarmLightSpritePath, 64, CreateAlarmLightPixel);
    }

    private static void GeneratePlaceholderAudio()
    {
        WriteSineWave(ShootAudioPath, 620f, 0.08f, 0.35f);
        WriteSineWave(HitAudioPath, 180f, 0.12f, 0.45f);
        WriteSineWave(PickupAudioPath, 880f, 0.12f, 0.35f);
        WriteSineWave(DoorAudioPath, 120f, 0.18f, 0.4f);
        WriteSineWave(ShopAudioPath, 760f, 0.12f, 0.35f);
        WriteSineWave(BuffAudioPath, 1040f, 0.2f, 0.35f);
        WriteSineWave(BossPhaseAudioPath, 90f, 0.35f, 0.45f);
        WriteSineWave(VictoryAudioPath, 660f, 0.45f, 0.35f);
        WriteSineWave(GameOverAudioPath, 110f, 0.45f, 0.45f);
        WriteSineWave(LaserAudioPath, 1320f, 0.08f, 0.28f);
        WriteSineWave(KeycardAudioPath, 980f, 0.12f, 0.32f);

        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
    }

    private static void WriteSineWave(string assetPath, float frequency, float duration, float amplitude)
    {
        const int sampleRate = 44100;
        int sampleCount = Mathf.Max(1, Mathf.CeilToInt(sampleRate * Mathf.Max(0.01f, duration)));
        byte[] wavBytes = new byte[44 + sampleCount * 2];
        int byteRate = sampleRate * 2;
        int dataSize = sampleCount * 2;

        WriteAscii(wavBytes, 0, "RIFF");
        WriteInt32(wavBytes, 4, 36 + dataSize);
        WriteAscii(wavBytes, 8, "WAVE");
        WriteAscii(wavBytes, 12, "fmt ");
        WriteInt32(wavBytes, 16, 16);
        WriteInt16(wavBytes, 20, 1);
        WriteInt16(wavBytes, 22, 1);
        WriteInt32(wavBytes, 24, sampleRate);
        WriteInt32(wavBytes, 28, byteRate);
        WriteInt16(wavBytes, 32, 2);
        WriteInt16(wavBytes, 34, 16);
        WriteAscii(wavBytes, 36, "data");
        WriteInt32(wavBytes, 40, dataSize);

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleRate;
            float fade = 1f - (float)i / sampleCount;
            short sample = (short)(Mathf.Sin(t * frequency * Mathf.PI * 2f) * amplitude * fade * short.MaxValue);
            WriteInt16(wavBytes, 44 + i * 2, sample);
        }

        string absolutePath = ToAbsoluteAssetPath(assetPath);
        string directory = Path.GetDirectoryName(absolutePath);

        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllBytes(absolutePath, wavBytes);
        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
    }

    private static void WriteAscii(byte[] bytes, int startIndex, string text)
    {
        for (int i = 0; i < text.Length; i++)
        {
            bytes[startIndex + i] = (byte)text[i];
        }
    }

    private static void WriteInt16(byte[] bytes, int startIndex, int value)
    {
        byte[] valueBytes = BitConverter.GetBytes((short)value);
        Array.Copy(valueBytes, 0, bytes, startIndex, valueBytes.Length);
    }

    private static void WriteInt32(byte[] bytes, int startIndex, int value)
    {
        byte[] valueBytes = BitConverter.GetBytes(value);
        Array.Copy(valueBytes, 0, bytes, startIndex, valueBytes.Length);
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

    private static void CreateMenuCamera(Transform parent)
    {
        GameObject cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
        cameraObject.transform.SetParent(parent);
        cameraObject.transform.position = new Vector3(0f, 0f, -10f);
        cameraObject.tag = "MainCamera";

        Camera camera = cameraObject.GetComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 5f;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.04f, 0.05f, 0.06f);
    }

    private static void CreateMainMenuUI(Transform parent)
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

        GameObject mainPanel = CreateRectObject("MainPanel", canvasObject.transform);
        StretchRectTransform(mainPanel.GetComponent<RectTransform>(), Vector2.zero, Vector2.zero);

        GameObject titleObject = CreateRectObject("TitleText", mainPanel.transform);
        SetRectTransform(titleObject.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -105f), new Vector2(720f, 70f));
        Text titleText = titleObject.AddComponent<Text>();
        titleText.font = GetBuiltinUIFont();
        titleText.text = "COD: EVADARE";
        titleText.fontSize = 46;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = Color.white;
        titleText.raycastTarget = false;

        GameObject subtitleObject = CreateRectObject("SubtitleText", mainPanel.transform);
        SetRectTransform(subtitleObject.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -168f), new Vector2(600f, 40f));
        Text subtitleText = subtitleObject.AddComponent<Text>();
        subtitleText.font = GetBuiltinUIFont();
        subtitleText.text = "Laboratory Escape Demo";
        subtitleText.fontSize = 24;
        subtitleText.alignment = TextAnchor.MiddleCenter;
        subtitleText.color = new Color(0.72f, 0.9f, 1f, 1f);
        subtitleText.raycastTarget = false;

        GameObject controlsPanel = CreateRectObject("ControlsPanel", canvasObject.transform);
        StretchRectTransform(controlsPanel.GetComponent<RectTransform>(), Vector2.zero, Vector2.zero);

        GameObject menuSystems = new GameObject("MenuSystems");
        menuSystems.transform.SetParent(parent);
        GameObject controllerObject = new GameObject("MainMenuController");
        controllerObject.transform.SetParent(menuSystems.transform);
        MainMenuController2D controller = controllerObject.AddComponent<MainMenuController2D>();
        AssignString(controller, "demoSceneName", "Prototype_FinalDemo");
        AssignString(controller, "demoScenePath", FinalDemoScenePath);
        AssignString(controller, "prisonSceneName", "Prototype_PrisonLevel");
        AssignString(controller, "prisonScenePath", PrisonLevelScenePath);
        AssignString(controller, "balancedPrisonSceneName", "Prototype_PrisonLevel_Balanced");
        AssignString(controller, "balancedPrisonScenePath", BalancedPrisonLevelScenePath);
        AssignObjectReference(controller, "mainPanel", mainPanel);
        AssignObjectReference(controller, "controlsPanel", controlsPanel);

        Button playButton = CreateMenuButton(mainPanel.transform, "PlayButton", "Play Demo", new Vector2(0f, 56f));
        UnityEventTools.AddPersistentListener(playButton.onClick, controller.PlayDemo);

        Button prisonButton = CreateMenuButton(mainPanel.transform, "PrisonButton", "Play Level 2: Prison", new Vector2(0f, -10f));
        UnityEventTools.AddPersistentListener(prisonButton.onClick, controller.PlayPrisonLevel);

        Button balancedPrisonButton = CreateMenuButton(mainPanel.transform, "BalancedPrisonButton", "Play Level 2: Prison Balanced", new Vector2(0f, -76f));
        UnityEventTools.AddPersistentListener(balancedPrisonButton.onClick, controller.PlayBalancedPrisonLevel);

        Button controlsButton = CreateMenuButton(mainPanel.transform, "ControlsButton", "Controls", new Vector2(0f, -142f));
        UnityEventTools.AddPersistentListener(controlsButton.onClick, controller.ShowControls);

        Button quitButton = CreateMenuButton(mainPanel.transform, "QuitButton", "Quit", new Vector2(0f, -208f));
        UnityEventTools.AddPersistentListener(quitButton.onClick, controller.QuitGame);

        GameObject controlsTitleObject = CreateRectObject("ControlsTitleText", controlsPanel.transform);
        SetRectTransform(controlsTitleObject.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -90f), new Vector2(620f, 54f));
        Text controlsTitleText = controlsTitleObject.AddComponent<Text>();
        controlsTitleText.font = GetBuiltinUIFont();
        controlsTitleText.text = "Controls";
        controlsTitleText.fontSize = 36;
        controlsTitleText.alignment = TextAnchor.MiddleCenter;
        controlsTitleText.color = Color.white;
        controlsTitleText.raycastTarget = false;

        GameObject controlsBodyObject = CreateRectObject("ControlsBodyText", controlsPanel.transform);
        SetRectTransform(controlsBodyObject.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 24f), new Vector2(620f, 260f));
        Text controlsBodyText = controlsBodyObject.AddComponent<Text>();
        controlsBodyText.font = GetBuiltinUIFont();
        controlsBodyText.text = "WASD / Arrow Keys - Move\nMouse - Aim\nLeft Click - Shoot\nR - Reload\nE - Interact\nEscape - Pause";
        controlsBodyText.fontSize = 24;
        controlsBodyText.alignment = TextAnchor.MiddleCenter;
        controlsBodyText.color = Color.white;
        controlsBodyText.raycastTarget = false;

        Button backButton = CreateMenuButton(controlsPanel.transform, "BackButton", "Back", new Vector2(0f, -220f));
        UnityEventTools.AddPersistentListener(backButton.onClick, controller.ShowMainMenu);

        controlsPanel.SetActive(false);
    }

    private static Button CreateMenuButton(Transform parent, string name, string label, Vector2 anchoredPosition)
    {
        GameObject buttonObject = CreateRectObject(name, parent);
        SetRectTransform(buttonObject.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), anchoredPosition, new Vector2(380f, 54f));

        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.12f, 0.2f, 0.24f, 1f);

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;

        GameObject textObject = CreateRectObject("Text", buttonObject.transform);
        StretchRectTransform(textObject.GetComponent<RectTransform>(), new Vector2(8f, 6f), new Vector2(-8f, -6f));

        Text text = textObject.AddComponent<Text>();
        text.font = GetBuiltinUIFont();
        text.text = label;
        text.fontSize = 22;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.raycastTarget = false;

        return button;
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

        SpriteFlash2D flash = enemy.AddComponent<SpriteFlash2D>();
        AssignObjectReference(flash, "spriteRenderer", spriteRenderer);

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(enemy, EnemyPrefabPath);
        UnityEngine.Object.DestroyImmediate(enemy);

        if (prefab == null)
        {
            prefab = AssetDatabase.LoadAssetAtPath<GameObject>(EnemyPrefabPath);
        }

        return prefab;
    }

    private static GameObject CreateMinibossPrefab(Sprite sprite)
    {
        GameObject miniboss = new GameObject("PrototypeMiniboss");
        miniboss.transform.localScale = new Vector3(1.5f, 1.5f, 1f);

        SpriteRenderer spriteRenderer = miniboss.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = 10;

        Rigidbody2D body = miniboss.AddComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;
        body.freezeRotation = true;

        miniboss.AddComponent<BoxCollider2D>();

        EnemyHealth health = miniboss.AddComponent<EnemyHealth>();
        AssignInt(health, "maxHealth", 15);

        SimpleEnemyChaser2D chaser = miniboss.AddComponent<SimpleEnemyChaser2D>();
        AssignFloat(chaser, "moveSpeed", 1.1f);
        AssignFloat(chaser, "stopDistance", 1f);

        EnemyContactDamage2D contactDamage = miniboss.AddComponent<EnemyContactDamage2D>();
        AssignInt(contactDamage, "damage", 1);
        AssignFloat(contactDamage, "damageCooldown", 1f);

        MinibossMarker2D marker = miniboss.AddComponent<MinibossMarker2D>();
        AssignString(marker, "minibossName", "Prototype Miniboss");

        SpriteFlash2D flash = miniboss.AddComponent<SpriteFlash2D>();
        AssignObjectReference(flash, "spriteRenderer", spriteRenderer);

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(miniboss, MinibossPrefabPath);
        UnityEngine.Object.DestroyImmediate(miniboss);

        if (prefab == null)
        {
            prefab = AssetDatabase.LoadAssetAtPath<GameObject>(MinibossPrefabPath);
        }

        return prefab;
    }

    private static GameObject CreateEnemyProjectilePrefab(Sprite sprite)
    {
        GameObject projectile = new GameObject("EnemyProjectile");
        projectile.transform.localScale = new Vector3(0.22f, 0.22f, 1f);

        SpriteRenderer spriteRenderer = projectile.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = 18;

        Rigidbody2D body = projectile.AddComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        CircleCollider2D collider = projectile.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;

        EnemyProjectile2D projectileComponent = projectile.AddComponent<EnemyProjectile2D>();
        AssignFloat(projectileComponent, "speed", 7f);
        AssignFloat(projectileComponent, "lifetime", 4f);
        AssignInt(projectileComponent, "damage", 1);

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(projectile, EnemyProjectilePrefabPath);
        UnityEngine.Object.DestroyImmediate(projectile);

        if (prefab == null)
        {
            prefab = AssetDatabase.LoadAssetAtPath<GameObject>(EnemyProjectilePrefabPath);
        }

        return prefab;
    }

    private static GameObject CreateBossPrefab(Sprite sprite, GameObject enemyProjectilePrefab)
    {
        GameObject boss = new GameObject("Experiment-01");
        boss.transform.localScale = new Vector3(1.9f, 1.9f, 1f);

        SpriteRenderer spriteRenderer = boss.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = 10;

        Rigidbody2D body = boss.AddComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;
        body.freezeRotation = true;

        CircleCollider2D collider = boss.AddComponent<CircleCollider2D>();
        collider.radius = 0.52f;

        EnemyHealth health = boss.AddComponent<EnemyHealth>();
        AssignInt(health, "maxHealth", 50);

        EnemyContactDamage2D contactDamage = boss.AddComponent<EnemyContactDamage2D>();
        AssignInt(contactDamage, "damage", 1);
        AssignFloat(contactDamage, "damageCooldown", 1f);

        GameObject projectileSpawnPoint = new GameObject("ProjectileSpawnPoint");
        projectileSpawnPoint.transform.SetParent(boss.transform);
        projectileSpawnPoint.transform.localPosition = new Vector3(0.8f, 0f, 0f);
        projectileSpawnPoint.transform.localRotation = Quaternion.identity;
        projectileSpawnPoint.transform.localScale = Vector3.one;

        BossAttackController2D attackController = boss.AddComponent<BossAttackController2D>();
        AssignString(attackController, "bossName", "Experiment-01");
        AssignObjectReference(attackController, "projectileSpawnPoint", projectileSpawnPoint.transform);
        AssignObjectReference(attackController, "enemyProjectilePrefab", enemyProjectilePrefab);
        AssignFloat(attackController, "moveSpeed", 1.1f);
        AssignFloat(attackController, "stopDistance", 3f);
        AssignFloat(attackController, "attackInterval", 1.5f);
        AssignFloat(attackController, "phaseTwoHealthPercent", 0.5f);
        AssignFloat(attackController, "phaseTwoAttackIntervalMultiplier", 0.65f);
        AssignInt(attackController, "aimedShotDamage", 1);
        AssignInt(attackController, "radialShotDamage", 1);
        AssignFloat(attackController, "projectileSpeed", 7f);
        AssignFloat(attackController, "projectileLifetime", 4f);
        AssignInt(attackController, "phaseOneRadialProjectiles", 8);
        AssignInt(attackController, "phaseTwoRadialProjectiles", 14);
        AssignBool(attackController, "activateOnStart", true);

        BossMarker2D marker = boss.AddComponent<BossMarker2D>();
        AssignString(marker, "bossName", "Experiment-01");

        SpriteFlash2D flash = boss.AddComponent<SpriteFlash2D>();
        AssignObjectReference(flash, "spriteRenderer", spriteRenderer);

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(boss, BossPrefabPath);
        UnityEngine.Object.DestroyImmediate(boss);

        if (prefab == null)
        {
            prefab = AssetDatabase.LoadAssetAtPath<GameObject>(BossPrefabPath);
        }

        return prefab;
    }

    private static GameObject CreatePrisonGuardPrefab(Sprite sprite)
    {
        GameObject guard = new GameObject("PrisonGuard");
        guard.transform.localScale = new Vector3(0.85f, 0.85f, 1f);

        SpriteRenderer spriteRenderer = guard.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = 10;

        Rigidbody2D body = guard.AddComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;
        body.freezeRotation = true;

        guard.AddComponent<BoxCollider2D>();

        EnemyHealth health = guard.AddComponent<EnemyHealth>();
        AssignInt(health, "maxHealth", 3);

        SimpleEnemyChaser2D chaser = guard.AddComponent<SimpleEnemyChaser2D>();
        AssignFloat(chaser, "moveSpeed", 1.6f);
        AssignFloat(chaser, "stopDistance", 1f);

        EnemyContactDamage2D contactDamage = guard.AddComponent<EnemyContactDamage2D>();
        AssignInt(contactDamage, "damage", 1);
        AssignFloat(contactDamage, "damageCooldown", 1f);

        SpriteFlash2D flash = guard.AddComponent<SpriteFlash2D>();
        AssignObjectReference(flash, "spriteRenderer", spriteRenderer);

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(guard, PrisonGuardPrefabPath);
        UnityEngine.Object.DestroyImmediate(guard);
        return prefab != null ? prefab : AssetDatabase.LoadAssetAtPath<GameObject>(PrisonGuardPrefabPath);
    }

    private static GameObject CreatePrisonRangedGuardPrefab(Sprite sprite, GameObject enemyProjectilePrefab)
    {
        GameObject guard = new GameObject("PrisonRangedGuard");
        guard.transform.localScale = new Vector3(0.85f, 0.85f, 1f);

        SpriteRenderer spriteRenderer = guard.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = 10;

        Rigidbody2D body = guard.AddComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;
        body.freezeRotation = true;

        guard.AddComponent<BoxCollider2D>();

        EnemyHealth health = guard.AddComponent<EnemyHealth>();
        AssignInt(health, "maxHealth", 4);

        GameObject projectileSpawnPoint = new GameObject("ProjectileSpawnPoint");
        projectileSpawnPoint.transform.SetParent(guard.transform);
        projectileSpawnPoint.transform.localPosition = new Vector3(0.55f, 0f, 0f);
        projectileSpawnPoint.transform.localRotation = Quaternion.identity;
        projectileSpawnPoint.transform.localScale = Vector3.one;

        RangedEnemyShooter2D shooter = guard.AddComponent<RangedEnemyShooter2D>();
        AssignObjectReference(shooter, "projectileSpawnPoint", projectileSpawnPoint.transform);
        AssignObjectReference(shooter, "enemyProjectilePrefab", enemyProjectilePrefab);
        AssignFloat(shooter, "moveSpeed", 1.2f);
        AssignFloat(shooter, "preferredDistance", 4f);
        AssignFloat(shooter, "minimumDistance", 2f);
        AssignFloat(shooter, "attackInterval", 1.2f);
        AssignInt(shooter, "damage", 1);
        AssignFloat(shooter, "projectileSpeed", 7f);
        AssignFloat(shooter, "projectileLifetime", 3f);
        AssignBool(shooter, "activateOnStart", true);

        EnemyContactDamage2D contactDamage = guard.AddComponent<EnemyContactDamage2D>();
        AssignInt(contactDamage, "damage", 1);
        AssignFloat(contactDamage, "damageCooldown", 1.25f);

        SpriteFlash2D flash = guard.AddComponent<SpriteFlash2D>();
        AssignObjectReference(flash, "spriteRenderer", spriteRenderer);

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(guard, PrisonRangedGuardPrefabPath);
        UnityEngine.Object.DestroyImmediate(guard);
        return prefab != null ? prefab : AssetDatabase.LoadAssetAtPath<GameObject>(PrisonRangedGuardPrefabPath);
    }

    private static GameObject CreateRiotBrutePrefab(Sprite sprite)
    {
        GameObject brute = new GameObject("RiotBruteMiniboss");
        brute.transform.localScale = new Vector3(1.5f, 1.5f, 1f);

        SpriteRenderer spriteRenderer = brute.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = 10;

        Rigidbody2D body = brute.AddComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;
        body.freezeRotation = true;

        brute.AddComponent<BoxCollider2D>();

        EnemyHealth health = brute.AddComponent<EnemyHealth>();
        AssignInt(health, "maxHealth", 22);

        SimpleEnemyChaser2D chaser = brute.AddComponent<SimpleEnemyChaser2D>();
        AssignFloat(chaser, "moveSpeed", 1.2f);
        AssignFloat(chaser, "stopDistance", 1.1f);

        EnemyContactDamage2D contactDamage = brute.AddComponent<EnemyContactDamage2D>();
        AssignInt(contactDamage, "damage", 2);
        AssignFloat(contactDamage, "damageCooldown", 1.2f);

        MinibossMarker2D marker = brute.AddComponent<MinibossMarker2D>();
        AssignString(marker, "minibossName", "Riot Brute");

        SpriteFlash2D flash = brute.AddComponent<SpriteFlash2D>();
        AssignObjectReference(flash, "spriteRenderer", spriteRenderer);

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(brute, RiotBrutePrefabPath);
        UnityEngine.Object.DestroyImmediate(brute);
        return prefab != null ? prefab : AssetDatabase.LoadAssetAtPath<GameObject>(RiotBrutePrefabPath);
    }

    private static GameObject CreateWardenBossPrefab(Sprite sprite, GameObject enemyProjectilePrefab)
    {
        GameObject boss = new GameObject("The Warden");
        boss.transform.localScale = new Vector3(2f, 2f, 1f);

        SpriteRenderer spriteRenderer = boss.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = 10;

        Rigidbody2D body = boss.AddComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;
        body.freezeRotation = true;

        CircleCollider2D collider = boss.AddComponent<CircleCollider2D>();
        collider.radius = 0.52f;

        EnemyHealth health = boss.AddComponent<EnemyHealth>();
        AssignInt(health, "maxHealth", 60);

        EnemyContactDamage2D contactDamage = boss.AddComponent<EnemyContactDamage2D>();
        AssignInt(contactDamage, "damage", 1);
        AssignFloat(contactDamage, "damageCooldown", 1f);

        GameObject projectileSpawnPoint = new GameObject("ProjectileSpawnPoint");
        projectileSpawnPoint.transform.SetParent(boss.transform);
        projectileSpawnPoint.transform.localPosition = new Vector3(0.8f, 0f, 0f);
        projectileSpawnPoint.transform.localRotation = Quaternion.identity;
        projectileSpawnPoint.transform.localScale = Vector3.one;

        BossAttackController2D attackController = boss.AddComponent<BossAttackController2D>();
        AssignString(attackController, "bossName", "The Warden");
        AssignObjectReference(attackController, "projectileSpawnPoint", projectileSpawnPoint.transform);
        AssignObjectReference(attackController, "enemyProjectilePrefab", enemyProjectilePrefab);
        AssignFloat(attackController, "moveSpeed", 1f);
        AssignFloat(attackController, "stopDistance", 4f);
        AssignFloat(attackController, "attackInterval", 1.4f);
        AssignFloat(attackController, "phaseTwoHealthPercent", 0.5f);
        AssignFloat(attackController, "phaseTwoAttackIntervalMultiplier", 0.6f);
        AssignInt(attackController, "aimedShotDamage", 1);
        AssignInt(attackController, "radialShotDamage", 1);
        AssignFloat(attackController, "projectileSpeed", 7f);
        AssignFloat(attackController, "projectileLifetime", 4f);
        AssignInt(attackController, "phaseOneRadialProjectiles", 8);
        AssignInt(attackController, "phaseTwoRadialProjectiles", 16);
        AssignBool(attackController, "activateOnStart", true);

        BossMarker2D marker = boss.AddComponent<BossMarker2D>();
        AssignString(marker, "bossName", "The Warden");

        SpriteFlash2D flash = boss.AddComponent<SpriteFlash2D>();
        AssignObjectReference(flash, "spriteRenderer", spriteRenderer);

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(boss, WardenBossPrefabPath);
        UnityEngine.Object.DestroyImmediate(boss);
        return prefab != null ? prefab : AssetDatabase.LoadAssetAtPath<GameObject>(WardenBossPrefabPath);
    }

    private static GameObject CreateKeycardPickupPrefab(Sprite sprite)
    {
        GameObject keycard = new GameObject("KeycardPickup");
        keycard.transform.localScale = new Vector3(0.45f, 0.45f, 1f);

        SpriteRenderer spriteRenderer = keycard.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = 15;

        CircleCollider2D collider = keycard.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;

        KeycardPickup2D pickup = keycard.AddComponent<KeycardPickup2D>();
        AssignInt(pickup, "keycardAmount", 1);
        AssignBool(pickup, "destroyOnPickup", true);

        keycard.AddComponent<PickupBob2D>();

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(keycard, KeycardPickupPrefabPath);
        UnityEngine.Object.DestroyImmediate(keycard);
        return prefab != null ? prefab : AssetDatabase.LoadAssetAtPath<GameObject>(KeycardPickupPrefabPath);
    }

    private static GameObject CreateLockedGatePrefab(Sprite sprite)
    {
        GameObject gate = new GameObject("LockedGate");
        gate.transform.localScale = new Vector3(0.55f, 2.4f, 1f);

        SpriteRenderer spriteRenderer = gate.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = 8;

        BoxCollider2D blockingCollider = gate.AddComponent<BoxCollider2D>();
        blockingCollider.isTrigger = false;
        blockingCollider.size = new Vector2(1f, 1f);

        BoxCollider2D promptCollider = gate.AddComponent<BoxCollider2D>();
        promptCollider.isTrigger = true;
        promptCollider.size = new Vector2(4f, 1.5f);

        GameObject promptTrigger = new GameObject("GatePromptTrigger");
        promptTrigger.transform.SetParent(gate.transform);
        promptTrigger.transform.localPosition = Vector3.zero;
        promptTrigger.transform.localRotation = Quaternion.identity;
        promptTrigger.transform.localScale = Vector3.one;

        GameObject openVisual = new GameObject("OpenVisual");
        openVisual.transform.SetParent(gate.transform);
        openVisual.transform.localPosition = Vector3.zero;
        openVisual.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
        openVisual.transform.localScale = new Vector3(0.55f, 0.35f, 1f);
        SpriteRenderer openRenderer = openVisual.AddComponent<SpriteRenderer>();
        openRenderer.sprite = sprite;
        openRenderer.sortingOrder = 8;
        openRenderer.color = new Color(0.35f, 0.7f, 0.85f, 0.45f);
        openVisual.SetActive(false);

        LockedGate2D lockedGate = gate.AddComponent<LockedGate2D>();
        AssignInt(lockedGate, "requiredKeycards", 1);
        AssignBool(lockedGate, "consumeKeycard", true);
        AssignEnumByName(lockedGate, "interactKey", nameof(KeyCode.E));
        AssignObjectReference(lockedGate, "spriteRenderer", spriteRenderer);
        AssignObjectReference(lockedGate, "gateCollider", blockingCollider);
        AssignObjectReference(lockedGate, "openVisual", openVisual);
        AssignString(lockedGate, "lockedMessage", "Requires keycard");
        AssignString(lockedGate, "openMessage", "Gate opened");

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(gate, LockedGatePrefabPath);
        UnityEngine.Object.DestroyImmediate(gate);
        return prefab != null ? prefab : AssetDatabase.LoadAssetAtPath<GameObject>(LockedGatePrefabPath);
    }

    private static GameObject CreateSecurityLaserPrefab(Sprite sprite)
    {
        GameObject laser = new GameObject("SecurityLaser");
        laser.transform.localScale = new Vector3(0.25f, 2.8f, 1f);

        SpriteRenderer spriteRenderer = laser.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = 6;

        BoxCollider2D collider = laser.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;

        SecurityLaserHazard2D hazard = laser.AddComponent<SecurityLaserHazard2D>();
        AssignInt(hazard, "damage", 1);
        AssignFloat(hazard, "damageCooldown", 0.75f);
        AssignBool(hazard, "startsActive", true);
        AssignBool(hazard, "toggles", true);
        AssignFloat(hazard, "activeDuration", 2f);
        AssignFloat(hazard, "inactiveDuration", 1.5f);
        AssignObjectReference(hazard, "spriteRenderer", spriteRenderer);
        AssignObjectReference(hazard, "hazardCollider", collider);

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(laser, SecurityLaserPrefabPath);
        UnityEngine.Object.DestroyImmediate(laser);
        return prefab != null ? prefab : AssetDatabase.LoadAssetAtPath<GameObject>(SecurityLaserPrefabPath);
    }

    private static GameObject CreateBalancedPrisonGuardPrefab(Sprite sprite, PrisonBalanceProfile2D balance)
    {
        GameObject guard = new GameObject("PrisonGuard_Balanced");
        guard.transform.localScale = new Vector3(0.85f, 0.85f, 1f);

        SpriteRenderer spriteRenderer = guard.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = 10;

        Rigidbody2D body = guard.AddComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;
        body.freezeRotation = true;

        guard.AddComponent<BoxCollider2D>();

        EnemyHealth health = guard.AddComponent<EnemyHealth>();
        AssignInt(health, "maxHealth", balance.PrisonGuardHealth);

        SimpleEnemyChaser2D chaser = guard.AddComponent<SimpleEnemyChaser2D>();
        AssignFloat(chaser, "moveSpeed", balance.PrisonGuardMoveSpeed);
        AssignFloat(chaser, "stopDistance", 1f);

        EnemyContactDamage2D contactDamage = guard.AddComponent<EnemyContactDamage2D>();
        AssignInt(contactDamage, "damage", balance.PrisonGuardContactDamage);
        AssignFloat(contactDamage, "damageCooldown", 1f);

        SpriteFlash2D flash = guard.AddComponent<SpriteFlash2D>();
        AssignObjectReference(flash, "spriteRenderer", spriteRenderer);

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(guard, BalancedPrisonGuardPrefabPath);
        UnityEngine.Object.DestroyImmediate(guard);
        return prefab != null ? prefab : AssetDatabase.LoadAssetAtPath<GameObject>(BalancedPrisonGuardPrefabPath);
    }

    private static GameObject CreateBalancedPrisonRangedGuardPrefab(Sprite sprite, GameObject enemyProjectilePrefab, PrisonBalanceProfile2D balance)
    {
        GameObject guard = new GameObject("PrisonRangedGuard_Balanced");
        guard.transform.localScale = new Vector3(0.85f, 0.85f, 1f);

        SpriteRenderer spriteRenderer = guard.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = 10;

        Rigidbody2D body = guard.AddComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;
        body.freezeRotation = true;

        guard.AddComponent<BoxCollider2D>();

        EnemyHealth health = guard.AddComponent<EnemyHealth>();
        AssignInt(health, "maxHealth", balance.RangedGuardHealth);

        GameObject projectileSpawnPoint = new GameObject("ProjectileSpawnPoint");
        projectileSpawnPoint.transform.SetParent(guard.transform);
        projectileSpawnPoint.transform.localPosition = new Vector3(0.55f, 0f, 0f);
        projectileSpawnPoint.transform.localRotation = Quaternion.identity;
        projectileSpawnPoint.transform.localScale = Vector3.one;

        LineRenderer aimLine = guard.AddComponent<LineRenderer>();
        aimLine.enabled = false;
        aimLine.useWorldSpace = true;
        aimLine.positionCount = 2;
        aimLine.startWidth = 0.035f;
        aimLine.endWidth = 0.015f;
        aimLine.startColor = new Color(1f, 0.85f, 0.15f, 0.85f);
        aimLine.endColor = new Color(1f, 0.2f, 0.1f, 0.35f);
        aimLine.sortingOrder = 30;
        Material aimMaterial = GetOrCreateMaterial("Assets/_Project/Materials/AimTelegraph.mat", "Sprites/Default", new Color(1f, 0.65f, 0.1f, 1f));
        aimLine.sharedMaterial = aimMaterial;

        RangedEnemyShooter2D shooter = guard.AddComponent<RangedEnemyShooter2D>();
        AssignObjectReference(shooter, "projectileSpawnPoint", projectileSpawnPoint.transform);
        AssignObjectReference(shooter, "enemyProjectilePrefab", enemyProjectilePrefab);
        AssignFloat(shooter, "moveSpeed", balance.RangedGuardMoveSpeed);
        AssignFloat(shooter, "preferredDistance", balance.RangedGuardPreferredDistance);
        AssignFloat(shooter, "minimumDistance", balance.RangedGuardMinimumDistance);
        AssignFloat(shooter, "attackInterval", balance.RangedGuardAttackInterval);
        AssignInt(shooter, "damage", balance.RangedGuardDamage);
        AssignFloat(shooter, "projectileSpeed", balance.RangedGuardProjectileSpeed);
        AssignFloat(shooter, "projectileLifetime", 3f);
        AssignBool(shooter, "activateOnStart", true);
        AssignFloat(shooter, "aimTelegraphDuration", balance.RangedGuardAimTelegraphDuration);
        AssignObjectReference(shooter, "aimLine", aimLine);
        AssignBool(shooter, "useAimTelegraph", true);

        EnemyContactDamage2D contactDamage = guard.AddComponent<EnemyContactDamage2D>();
        AssignInt(contactDamage, "damage", 1);
        AssignFloat(contactDamage, "damageCooldown", 1.25f);

        SpriteFlash2D flash = guard.AddComponent<SpriteFlash2D>();
        AssignObjectReference(flash, "spriteRenderer", spriteRenderer);

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(guard, BalancedPrisonRangedGuardPrefabPath);
        UnityEngine.Object.DestroyImmediate(guard);
        return prefab != null ? prefab : AssetDatabase.LoadAssetAtPath<GameObject>(BalancedPrisonRangedGuardPrefabPath);
    }

    private static GameObject CreateBalancedRiotBrutePrefab(Sprite sprite, PrisonBalanceProfile2D balance)
    {
        GameObject brute = new GameObject("RiotBruteMiniboss_Balanced");
        brute.transform.localScale = new Vector3(1.5f, 1.5f, 1f);

        SpriteRenderer spriteRenderer = brute.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = 10;

        Rigidbody2D body = brute.AddComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;
        body.freezeRotation = true;

        brute.AddComponent<BoxCollider2D>();

        EnemyHealth health = brute.AddComponent<EnemyHealth>();
        AssignInt(health, "maxHealth", balance.RiotBruteHealth);

        SimpleEnemyChaser2D chaser = brute.AddComponent<SimpleEnemyChaser2D>();
        AssignFloat(chaser, "moveSpeed", balance.RiotBruteMoveSpeed);
        AssignFloat(chaser, "stopDistance", 1.1f);

        EnemyContactDamage2D contactDamage = brute.AddComponent<EnemyContactDamage2D>();
        AssignInt(contactDamage, "damage", balance.RiotBruteDamage);
        AssignFloat(contactDamage, "damageCooldown", balance.RiotBruteDamageCooldown);

        MinibossMarker2D marker = brute.AddComponent<MinibossMarker2D>();
        AssignString(marker, "minibossName", "Riot Brute");

        SpriteFlash2D flash = brute.AddComponent<SpriteFlash2D>();
        AssignObjectReference(flash, "spriteRenderer", spriteRenderer);

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(brute, BalancedRiotBrutePrefabPath);
        UnityEngine.Object.DestroyImmediate(brute);
        return prefab != null ? prefab : AssetDatabase.LoadAssetAtPath<GameObject>(BalancedRiotBrutePrefabPath);
    }

    private static GameObject CreateBalancedWardenBossPrefab(Sprite sprite, GameObject enemyProjectilePrefab, PrisonBalanceProfile2D balance)
    {
        GameObject boss = new GameObject("The Warden");
        boss.transform.localScale = new Vector3(2f, 2f, 1f);

        SpriteRenderer spriteRenderer = boss.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = 10;

        Rigidbody2D body = boss.AddComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;
        body.freezeRotation = true;

        CircleCollider2D collider = boss.AddComponent<CircleCollider2D>();
        collider.radius = 0.52f;

        EnemyHealth health = boss.AddComponent<EnemyHealth>();
        AssignInt(health, "maxHealth", balance.WardenHealth);

        EnemyContactDamage2D contactDamage = boss.AddComponent<EnemyContactDamage2D>();
        AssignInt(contactDamage, "damage", 1);
        AssignFloat(contactDamage, "damageCooldown", 1f);

        GameObject projectileSpawnPoint = new GameObject("ProjectileSpawnPoint");
        projectileSpawnPoint.transform.SetParent(boss.transform);
        projectileSpawnPoint.transform.localPosition = new Vector3(0.8f, 0f, 0f);
        projectileSpawnPoint.transform.localRotation = Quaternion.identity;
        projectileSpawnPoint.transform.localScale = Vector3.one;

        BossAttackController2D attackController = boss.AddComponent<BossAttackController2D>();
        AssignString(attackController, "bossName", "The Warden");
        AssignObjectReference(attackController, "projectileSpawnPoint", projectileSpawnPoint.transform);
        AssignObjectReference(attackController, "enemyProjectilePrefab", enemyProjectilePrefab);
        AssignFloat(attackController, "moveSpeed", balance.WardenMoveSpeed);
        AssignFloat(attackController, "stopDistance", balance.WardenStopDistance);
        AssignFloat(attackController, "attackInterval", balance.WardenAttackInterval);
        AssignFloat(attackController, "phaseTwoHealthPercent", 0.5f);
        AssignFloat(attackController, "phaseTwoAttackIntervalMultiplier", balance.WardenPhaseTwoMultiplier);
        AssignInt(attackController, "aimedShotDamage", balance.WardenProjectileDamage);
        AssignInt(attackController, "radialShotDamage", balance.WardenProjectileDamage);
        AssignFloat(attackController, "projectileSpeed", balance.WardenProjectileSpeed);
        AssignFloat(attackController, "projectileLifetime", 4f);
        AssignInt(attackController, "phaseOneRadialProjectiles", balance.WardenPhaseOneRadialProjectiles);
        AssignInt(attackController, "phaseTwoRadialProjectiles", balance.WardenPhaseTwoRadialProjectiles);
        AssignBool(attackController, "activateOnStart", true);

        BossMarker2D marker = boss.AddComponent<BossMarker2D>();
        AssignString(marker, "bossName", "The Warden");

        SpriteFlash2D flash = boss.AddComponent<SpriteFlash2D>();
        AssignObjectReference(flash, "spriteRenderer", spriteRenderer);

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(boss, BalancedWardenBossPrefabPath);
        UnityEngine.Object.DestroyImmediate(boss);
        return prefab != null ? prefab : AssetDatabase.LoadAssetAtPath<GameObject>(BalancedWardenBossPrefabPath);
    }

    private static GameObject CreateBalancedSecurityLaserPrefab(Sprite sprite, PrisonBalanceProfile2D balance)
    {
        GameObject laser = new GameObject("SecurityLaser_Balanced");
        laser.transform.localScale = new Vector3(0.22f, 2.4f, 1f);

        SpriteRenderer spriteRenderer = laser.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = 6;

        BoxCollider2D collider = laser.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;

        SecurityLaserHazard2D hazard = laser.AddComponent<SecurityLaserHazard2D>();
        AssignInt(hazard, "damage", balance.LaserDamage);
        AssignFloat(hazard, "damageCooldown", balance.LaserDamageCooldown);
        AssignBool(hazard, "startsActive", false);
        AssignBool(hazard, "toggles", true);
        AssignFloat(hazard, "activeDuration", balance.LaserActiveDuration);
        AssignFloat(hazard, "inactiveDuration", balance.LaserInactiveDuration);
        AssignFloat(hazard, "warningDuration", balance.LaserWarningDuration);
        AssignColor(hazard, "inactiveColor", new Color(1f, 0.08f, 0.04f, 0.18f));
        AssignColor(hazard, "warningColor", new Color(1f, 0.72f, 0.1f, 0.85f));
        AssignColor(hazard, "activeColor", new Color(1f, 0.08f, 0.04f, 0.95f));
        AssignBool(hazard, "useWarningBeforeActive", true);
        AssignObjectReference(hazard, "spriteRenderer", spriteRenderer);
        AssignObjectReference(hazard, "hazardCollider", collider);

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(laser, BalancedSecurityLaserPrefabPath);
        UnityEngine.Object.DestroyImmediate(laser);
        return prefab != null ? prefab : AssetDatabase.LoadAssetAtPath<GameObject>(BalancedSecurityLaserPrefabPath);
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

        reward.AddComponent<PickupBob2D>();

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

        pickupObject.AddComponent<PickupBob2D>();

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(pickupObject, prefabPath);
        UnityEngine.Object.DestroyImmediate(pickupObject);

        if (prefab == null)
        {
            prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        }

        return prefab;
    }

    private static WeaponDefinition2D CreateWeaponDefinition(
        string assetPath,
        string weaponName,
        WeaponRarity2D rarity,
        GameObject bulletPrefab,
        float fireCooldown,
        int magazineSize,
        int ammoPerShot,
        float reloadDuration,
        int projectileDamage,
        float projectileSpeed,
        float projectileLifetime,
        int projectilesPerShot,
        float spreadAngle)
    {
        WeaponDefinition2D weapon = AssetDatabase.LoadAssetAtPath<WeaponDefinition2D>(assetPath);

        if (weapon == null)
        {
            weapon = ScriptableObject.CreateInstance<WeaponDefinition2D>();
            AssetDatabase.CreateAsset(weapon, assetPath);
        }

        AssignString(weapon, "weaponName", weaponName);
        AssignEnum(weapon, "rarity", (int)rarity);
        AssignObjectReference(weapon, "bulletPrefab", bulletPrefab);
        AssignFloat(weapon, "fireCooldown", fireCooldown);
        AssignInt(weapon, "magazineSize", magazineSize);
        AssignInt(weapon, "ammoPerShot", ammoPerShot);
        AssignFloat(weapon, "reloadDuration", reloadDuration);
        AssignInt(weapon, "projectileDamage", projectileDamage);
        AssignFloat(weapon, "projectileSpeed", projectileSpeed);
        AssignFloat(weapon, "projectileLifetime", projectileLifetime);
        AssignInt(weapon, "projectilesPerShot", projectilesPerShot);
        AssignFloat(weapon, "spreadAngle", spreadAngle);

        EditorUtility.SetDirty(weapon);
        AssetDatabase.SaveAssets();
        return weapon;
    }

    private static BuffDefinition2D[] CreatePrototypeBuffDefinitions()
    {
        return new[]
        {
            CreateBuffDefinition(BuffMaxHealthPath, "Thick Skin", "+1 Max HP", BuffType2D.MaxHealth, 1, 1f, null),
            CreateBuffDefinition(BuffHealPath, "Emergency Medkit", "Heal 2 HP", BuffType2D.Heal, 2, 1f, null),
            CreateBuffDefinition(BuffMoveSpeedPath, "Adrenaline", "+15% Move Speed", BuffType2D.MoveSpeedMultiplier, 0, 1.15f, null),
            CreateBuffDefinition(BuffFireRatePath, "Trigger Discipline", "+20% Fire Rate", BuffType2D.FireRateMultiplier, 0, 1.2f, null),
            CreateBuffDefinition(BuffDamagePath, "High Caliber", "+1 Bullet Damage", BuffType2D.DamageBonus, 1, 1f, null),
            CreateBuffDefinition(BuffReloadPath, "Fast Hands", "+25% Reload Speed", BuffType2D.ReloadSpeedMultiplier, 0, 1.25f, null),
            CreateBuffDefinition(BuffMaxAmmoPath, "Extra Pockets", "+20 Max Reserve Ammo", BuffType2D.MaxAmmo, 20, 1f, null),
            CreateBuffDefinition(BuffMoneyPath, "Found Credits", "+50 Money", BuffType2D.MoneyBonus, 50, 1f, null)
        };
    }

    private static PrisonBalanceProfile2D GetOrCreatePrisonBalanceProfile()
    {
        PrisonBalanceProfile2D profile = AssetDatabase.LoadAssetAtPath<PrisonBalanceProfile2D>(PrisonBalanceProfilePath);

        if (profile == null)
        {
            profile = ScriptableObject.CreateInstance<PrisonBalanceProfile2D>();
            AssetDatabase.CreateAsset(profile, PrisonBalanceProfilePath);
        }

        AssignInt(profile, "playerMaxHealth", 6);
        AssignInt(profile, "startingAmmo", 100);
        AssignInt(profile, "maxAmmo", 160);
        AssignInt(profile, "startingMoney", 45);
        AssignInt(profile, "prisonGuardHealth", 3);
        AssignFloat(profile, "prisonGuardMoveSpeed", 1.55f);
        AssignInt(profile, "prisonGuardContactDamage", 1);
        AssignInt(profile, "rangedGuardHealth", 3);
        AssignFloat(profile, "rangedGuardMoveSpeed", 1.15f);
        AssignFloat(profile, "rangedGuardPreferredDistance", 4.5f);
        AssignFloat(profile, "rangedGuardMinimumDistance", 2.25f);
        AssignFloat(profile, "rangedGuardAttackInterval", 1.45f);
        AssignFloat(profile, "rangedGuardProjectileSpeed", 6.5f);
        AssignInt(profile, "rangedGuardDamage", 1);
        AssignFloat(profile, "rangedGuardAimTelegraphDuration", 0.25f);
        AssignInt(profile, "laserDamage", 1);
        AssignFloat(profile, "laserDamageCooldown", 0.9f);
        AssignFloat(profile, "laserWarningDuration", 0.45f);
        AssignFloat(profile, "laserActiveDuration", 1.65f);
        AssignFloat(profile, "laserInactiveDuration", 2.1f);
        AssignInt(profile, "riotBruteHealth", 20);
        AssignFloat(profile, "riotBruteMoveSpeed", 1.15f);
        AssignInt(profile, "riotBruteDamage", 2);
        AssignFloat(profile, "riotBruteDamageCooldown", 1.2f);
        AssignInt(profile, "wardenHealth", 58);
        AssignFloat(profile, "wardenMoveSpeed", 0.95f);
        AssignFloat(profile, "wardenStopDistance", 4f);
        AssignFloat(profile, "wardenAttackInterval", 1.45f);
        AssignFloat(profile, "wardenPhaseTwoMultiplier", 0.65f);
        AssignInt(profile, "wardenPhaseOneRadialProjectiles", 8);
        AssignInt(profile, "wardenPhaseTwoRadialProjectiles", 14);
        AssignInt(profile, "wardenProjectileDamage", 1);
        AssignFloat(profile, "wardenProjectileSpeed", 6.8f);
        AssignInt(profile, "shopHealthPrice", 20);
        AssignInt(profile, "shopAmmoPrice", 15);
        AssignInt(profile, "shopWeaponPrice", 70);
        AssignInt(profile, "moneyPickupAmountSmall", 20);
        AssignInt(profile, "ammoPickupAmount", 18);
        AssignInt(profile, "healthPickupAmount", 2);

        EditorUtility.SetDirty(profile);
        AssetDatabase.SaveAssets();
        return profile;
    }

    private static BuffDefinition2D CreateBuffDefinition(
        string assetPath,
        string displayName,
        string description,
        BuffType2D buffType,
        int amount,
        float multiplier,
        Sprite icon)
    {
        BuffDefinition2D buff = AssetDatabase.LoadAssetAtPath<BuffDefinition2D>(assetPath);

        if (buff == null)
        {
            buff = ScriptableObject.CreateInstance<BuffDefinition2D>();
            AssetDatabase.CreateAsset(buff, assetPath);
        }

        AssignString(buff, "displayName", displayName);
        AssignString(buff, "description", description);
        AssignEnum(buff, "buffType", (int)buffType);
        AssignInt(buff, "amount", amount);
        AssignFloat(buff, "multiplier", multiplier);
        AssignObjectReference(buff, "icon", icon);

        EditorUtility.SetDirty(buff);
        AssetDatabase.SaveAssets();
        return buff;
    }

    private static GameObject CreateWeaponPickupPrefab(string pickupName, string prefabPath, Sprite sprite, WeaponDefinition2D weapon)
    {
        GameObject pickupObject = new GameObject(pickupName);
        pickupObject.transform.localScale = new Vector3(0.55f, 0.55f, 1f);

        SpriteRenderer spriteRenderer = pickupObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = 15;

        CircleCollider2D collider = pickupObject.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;

        WeaponPickup2D pickup = pickupObject.AddComponent<WeaponPickup2D>();
        AssignObjectReference(pickup, "weapon", weapon);
        AssignBool(pickup, "destroyOnPickup", true);

        pickupObject.AddComponent<PickupBob2D>();

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(pickupObject, prefabPath);
        UnityEngine.Object.DestroyImmediate(pickupObject);

        if (prefab == null)
        {
            prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        }

        return prefab;
    }

    private static ShopItemDefinition2D CreateShopItemDefinition(
        string assetPath,
        string displayName,
        ShopItemType2D itemType,
        int price,
        int amount,
        WeaponDefinition2D weaponDefinition,
        Sprite icon)
    {
        ShopItemDefinition2D item = AssetDatabase.LoadAssetAtPath<ShopItemDefinition2D>(assetPath);

        if (item == null)
        {
            item = ScriptableObject.CreateInstance<ShopItemDefinition2D>();
            AssetDatabase.CreateAsset(item, assetPath);
        }

        AssignString(item, "displayName", displayName);
        AssignEnum(item, "itemType", (int)itemType);
        AssignInt(item, "price", price);
        AssignInt(item, "amount", amount);
        AssignObjectReference(item, "weaponDefinition", weaponDefinition);
        AssignObjectReference(item, "icon", icon);

        EditorUtility.SetDirty(item);
        AssetDatabase.SaveAssets();
        return item;
    }

    private static GameObject CreateShopItemPrefab(string itemName, string prefabPath, Sprite sprite, ShopItemDefinition2D itemDefinition)
    {
        GameObject itemObject = new GameObject(itemName);
        itemObject.transform.localScale = new Vector3(0.6f, 0.6f, 1f);

        SpriteRenderer spriteRenderer = itemObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = 15;

        CircleCollider2D collider = itemObject.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 0.65f;

        ShopItem2D shopItem = itemObject.AddComponent<ShopItem2D>();
        AssignObjectReference(shopItem, "itemDefinition", itemDefinition);
        AssignEnumByName(shopItem, "interactKey", nameof(KeyCode.E));
        AssignBool(shopItem, "singlePurchase", true);
        AssignObjectReference(shopItem, "spriteRenderer", spriteRenderer);

        itemObject.AddComponent<PickupBob2D>();

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(itemObject, prefabPath);
        UnityEngine.Object.DestroyImmediate(itemObject);

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

    private static void CreateWeaponLootRoom(Transform parent, Sprite wallSprite, GameObject enemyPrefab, GameObject[] lootPrefabs)
    {
        GameObject room = new GameObject("Room_01");
        room.transform.SetParent(parent);

        CreateRoomLoopWalls(room.transform, wallSprite, out DoorController2D leftDoor, out DoorController2D rightDoor);

        EnemySpawner2D enemySpawner = CreateEnemySpawner(room.transform, enemyPrefab);
        RoomLootSpawner2D lootSpawner = CreateWeaponRoomLootSpawner(room.transform, lootPrefabs);

        RoomController2D roomController = room.AddComponent<RoomController2D>();
        AssignObjectReferenceArray(roomController, "doors", new[] { leftDoor, rightDoor });
        AssignObjectReference(roomController, "enemySpawner", enemySpawner);
        AssignObjectReference(roomController, "lootSpawner", lootSpawner);
        AssignFloat(roomController, "doorCloseDelay", 0.5f);

        CreateRoomTrigger(room.transform, roomController);
    }

    private static void CreateShopCombatRoom(Transform parent, Sprite wallSprite, GameObject enemyPrefab, GameObject moneyPickupPrefab)
    {
        GameObject room = new GameObject("Room_01");
        room.transform.SetParent(parent);

        CreateRoomLoopWalls(room.transform, wallSprite, out DoorController2D leftDoor, out DoorController2D rightDoor);

        EnemySpawner2D enemySpawner = CreateEnemySpawner(room.transform, enemyPrefab);
        RoomLootSpawner2D lootSpawner = CreateShopRoomLootSpawner(room.transform, moneyPickupPrefab);

        RoomController2D roomController = room.AddComponent<RoomController2D>();
        AssignObjectReferenceArray(roomController, "doors", new[] { leftDoor, rightDoor });
        AssignObjectReference(roomController, "enemySpawner", enemySpawner);
        AssignObjectReference(roomController, "lootSpawner", lootSpawner);
        AssignFloat(roomController, "doorCloseDelay", 0.5f);

        CreateRoomTrigger(room.transform, roomController);
    }

    private static void CreateMinibossRoom(Transform parent, Sprite wallSprite, GameObject minibossPrefab, BuffChoiceController2D buffChoiceController)
    {
        GameObject room = new GameObject("Miniboss_Room");
        room.transform.SetParent(parent);

        CreateRoomLoopWalls(room.transform, wallSprite, out DoorController2D leftDoor, out DoorController2D rightDoor);

        EnemySpawner2D enemySpawner = CreateMinibossSpawner(room.transform, minibossPrefab);

        RoomController2D roomController = room.AddComponent<RoomController2D>();
        AssignObjectReferenceArray(roomController, "doors", new[] { leftDoor, rightDoor });
        AssignObjectReference(roomController, "enemySpawner", enemySpawner);
        AssignObjectReference(roomController, "buffChoiceController", buffChoiceController);
        AssignBool(roomController, "showBuffChoiceOnClear", true);
        AssignFloat(roomController, "doorCloseDelay", 0.5f);

        CreateRoomTrigger(room.transform, roomController);
    }

    private static void CreateBossRoom(Transform parent, Sprite wallSprite, Sprite coverSprite, GameObject bossPrefab, LevelEndController2D levelEndController)
    {
        GameObject room = new GameObject("Boss_Room");
        room.transform.SetParent(parent);

        CreateBossRoomWalls(room.transform, wallSprite, out DoorController2D leftDoor, out DoorController2D rightDoor);
        CreateBossCover(room.transform, coverSprite);

        EnemySpawner2D enemySpawner = CreateBossSpawner(room.transform, bossPrefab);

        RoomController2D roomController = room.AddComponent<RoomController2D>();
        AssignObjectReferenceArray(roomController, "doors", new[] { leftDoor, rightDoor });
        AssignObjectReference(roomController, "enemySpawner", enemySpawner);
        AssignObjectReference(roomController, "levelEndController", levelEndController);
        AssignBool(roomController, "showVictoryOnClear", true);
        AssignString(roomController, "victoryMessage", "LABORATORY CLEARED");
        AssignFloat(roomController, "doorCloseDelay", 0.5f);

        CreateBossRoomTrigger(room.transform, roomController);
    }

    private static void CreateBossRoomWalls(Transform parent, Sprite sprite, out DoorController2D leftDoor, out DoorController2D rightDoor)
    {
        GameObject walls = new GameObject("Walls");
        walls.transform.SetParent(parent);

        CreateWall(walls.transform, "Wall_Top", sprite, new Vector3(1f, 5f, 0f), new Vector3(18.5f, 0.4f, 1f));
        CreateWall(walls.transform, "Wall_Bottom", sprite, new Vector3(1f, -5f, 0f), new Vector3(18.5f, 0.4f, 1f));
        CreateBossWallWithDoorGap(walls.transform, "Wall_Left", sprite, -8f);
        CreateBossWallWithDoorGap(walls.transform, "Wall_Right", sprite, 10f);

        GameObject doors = new GameObject("Doors");
        doors.transform.SetParent(parent);

        leftDoor = CreateDoor(doors.transform, "Door_Left", sprite, new Vector3(-8f, 0f, 0f));
        rightDoor = CreateDoor(doors.transform, "Door_Right", sprite, new Vector3(10f, 0f, 0f));
    }

    private static void CreateBossWallWithDoorGap(Transform parent, string name, Sprite sprite, float x)
    {
        GameObject wall = new GameObject(name);
        wall.transform.SetParent(parent);

        CreateWallSegment(wall.transform, name + "_Upper", sprite, new Vector3(x, 3.6f, 0f), new Vector3(0.4f, 2.8f, 1f));
        CreateWallSegment(wall.transform, name + "_Lower", sprite, new Vector3(x, -3.6f, 0f), new Vector3(0.4f, 2.8f, 1f));
    }

    private static void CreateBossCover(Transform parent, Sprite sprite)
    {
        GameObject coverRoot = new GameObject("Cover");
        coverRoot.transform.SetParent(parent);

        CreateCoverObject(coverRoot.transform, "Cover_01", sprite, new Vector3(-1.5f, 2f, 0f));
        CreateCoverObject(coverRoot.transform, "Cover_02", sprite, new Vector3(-1.5f, -2f, 0f));
        CreateCoverObject(coverRoot.transform, "Cover_03", sprite, new Vector3(3f, 2f, 0f));
        CreateCoverObject(coverRoot.transform, "Cover_04", sprite, new Vector3(3f, -2f, 0f));
    }

    private static void CreateCoverObject(Transform parent, string name, Sprite sprite, Vector3 position)
    {
        GameObject cover = new GameObject(name);
        cover.transform.SetParent(parent);
        cover.transform.position = position;
        cover.transform.localScale = new Vector3(1.2f, 0.8f, 1f);

        SpriteRenderer spriteRenderer = cover.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = 4;

        cover.AddComponent<BoxCollider2D>();
        cover.AddComponent<CoverObject2D>();
    }

    private static EnemySpawner2D CreateBossSpawner(Transform parent, GameObject bossPrefab)
    {
        GameObject spawnerObject = new GameObject("EnemySpawner");
        spawnerObject.transform.SetParent(parent);

        Transform[] spawnPoints =
        {
            CreateMarker(spawnerObject.transform, "SpawnPoint_Boss", new Vector3(4f, 0f, 0f))
        };

        EnemySpawner2D spawner = spawnerObject.AddComponent<EnemySpawner2D>();
        AssignObjectReference(spawner, "enemyPrefab", bossPrefab);
        AssignObjectReferenceArray(spawner, "spawnPoints", spawnPoints);
        AssignInt(spawner, "enemyCount", 1);

        return spawner;
    }

    private static void CreateBossRoomTrigger(Transform parent, RoomController2D roomController)
    {
        GameObject triggerObject = new GameObject("RoomTrigger");
        triggerObject.transform.SetParent(parent);
        triggerObject.transform.position = new Vector3(1f, 0f, 0f);

        BoxCollider2D triggerCollider = triggerObject.AddComponent<BoxCollider2D>();
        triggerCollider.isTrigger = true;
        triggerCollider.size = new Vector2(16.8f, 8.6f);

        RoomTrigger2D trigger = triggerObject.AddComponent<RoomTrigger2D>();
        AssignObjectReference(trigger, "roomController", roomController);
    }

    private static void CreateStartArea(Transform parent, ObjectiveUI2D objectiveUI)
    {
        GameObject startArea = new GameObject("Start_Area");
        startArea.transform.SetParent(parent);
        startArea.transform.position = new Vector3(-24f, 0f, 0f);

        CreateObjectiveTrigger(
            startArea.transform,
            "ObjectiveTrigger_Start",
            new Vector3(-24f, 0f, 0f),
            new Vector2(5f, 5f),
            objectiveUI,
            "Escape the Laboratory",
            "WASD move | Mouse aim | Left click shoot | R reload | E interact",
            false);
    }

    private static void CreateFullCombatRoom(
        Transform parent,
        string roomName,
        float centerX,
        Sprite wallSprite,
        GameObject enemyPrefab,
        int enemyCount,
        GameObject[] lootPrefabs,
        ObjectiveUI2D objectiveUI,
        string objectiveOnActivate,
        string objectiveOnClear)
    {
        GameObject room = new GameObject(roomName);
        room.transform.SetParent(parent);
        room.transform.position = new Vector3(centerX, 0f, 0f);

        CreateFullRoomWalls(room.transform, wallSprite, centerX, 6.5f, 4f, out DoorController2D leftDoor, out DoorController2D rightDoor);

        Vector3[] spawnPositions = enemyCount > 3
            ? new[]
            {
                new Vector3(centerX - 1.8f, 1.5f, 0f),
                new Vector3(centerX + 0.2f, 0.2f, 0f),
                new Vector3(centerX + 1.9f, -1.4f, 0f),
                new Vector3(centerX + 2.4f, 1.4f, 0f)
            }
            : new[]
            {
                new Vector3(centerX - 1.6f, 1.4f, 0f),
                new Vector3(centerX + 0.8f, 0f, 0f),
                new Vector3(centerX - 1.6f, -1.4f, 0f)
            };

        EnemySpawner2D enemySpawner = CreateFullEnemySpawner(room.transform, enemyPrefab, spawnPositions, enemyCount);

        Vector3[] lootPositions = lootPrefabs != null && lootPrefabs.Length > 3
            ? new[]
            {
                new Vector3(centerX + 2.6f, 1.3f, 0f),
                new Vector3(centerX + 3.3f, 0.45f, 0f),
                new Vector3(centerX + 3.3f, -0.45f, 0f),
                new Vector3(centerX + 2.6f, -1.3f, 0f)
            }
            : new[]
            {
                new Vector3(centerX + 2.8f, 1.1f, 0f),
                new Vector3(centerX + 3.5f, 0f, 0f),
                new Vector3(centerX + 2.8f, -1.1f, 0f)
            };

        RoomLootSpawner2D lootSpawner = CreateFullLootSpawner(room.transform, lootPrefabs, lootPositions);

        RoomController2D roomController = room.AddComponent<RoomController2D>();
        AssignObjectReferenceArray(roomController, "doors", new[] { leftDoor, rightDoor });
        AssignObjectReference(roomController, "enemySpawner", enemySpawner);
        AssignObjectReference(roomController, "lootSpawner", lootSpawner);
        AssignObjectReference(roomController, "objectiveUI", objectiveUI);
        AssignString(roomController, "objectiveOnActivate", objectiveOnActivate);
        AssignString(roomController, "objectiveOnClear", objectiveOnClear);
        AssignFloat(roomController, "doorCloseDelay", 0.5f);

        CreateFullRoomTrigger(room.transform, centerX, 11.6f, 6.8f, roomController);
    }

    private static void CreateFullShopArea(
        Transform parent,
        float centerX,
        Sprite wallSprite,
        ShopUI2D shopUI,
        ObjectiveUI2D objectiveUI,
        GameObject healthShopPrefab,
        GameObject ammoShopPrefab,
        GameObject weaponShopPrefab)
    {
        GameObject shopArea = new GameObject("Shop_Area");
        shopArea.transform.SetParent(parent);
        shopArea.transform.position = new Vector3(centerX, 0f, 0f);

        BoxCollider2D shopAreaTrigger = shopArea.AddComponent<BoxCollider2D>();
        shopAreaTrigger.isTrigger = true;
        shopAreaTrigger.size = new Vector2(7.4f, 5.4f);

        shopArea.AddComponent<ShopArea2D>();

        GameObject shopWalls = new GameObject("ShopWalls");
        shopWalls.transform.SetParent(shopArea.transform);

        CreateWall(shopWalls.transform, "ShopWall_Top", wallSprite, new Vector3(centerX, 3f, 0f), new Vector3(8.2f, 0.35f, 1f));
        CreateWall(shopWalls.transform, "ShopWall_Bottom", wallSprite, new Vector3(centerX, -3f, 0f), new Vector3(8.2f, 0.35f, 1f));
        CreateWall(shopWalls.transform, "ShopWall_Left_Upper", wallSprite, new Vector3(centerX - 4f, 2f, 0f), new Vector3(0.35f, 2f, 1f));
        CreateWall(shopWalls.transform, "ShopWall_Left_Lower", wallSprite, new Vector3(centerX - 4f, -2f, 0f), new Vector3(0.35f, 2f, 1f));
        CreateWall(shopWalls.transform, "ShopWall_Right_Upper", wallSprite, new Vector3(centerX + 4f, 2f, 0f), new Vector3(0.35f, 2f, 1f));
        CreateWall(shopWalls.transform, "ShopWall_Right_Lower", wallSprite, new Vector3(centerX + 4f, -2f, 0f), new Vector3(0.35f, 2f, 1f));

        InstantiateShopItem(shopArea.transform, healthShopPrefab, "ShopItem_Health", new Vector3(centerX - 1.4f, 1.2f, 0f), shopUI);
        InstantiateShopItem(shopArea.transform, ammoShopPrefab, "ShopItem_Ammo", new Vector3(centerX, 0f, 0f), shopUI);
        InstantiateShopItem(shopArea.transform, weaponShopPrefab, "ShopItem_Weapon", new Vector3(centerX + 1.4f, -1.2f, 0f), shopUI);

        CreateObjectiveTrigger(
            shopArea.transform,
            "ObjectiveTrigger_Shop",
            new Vector3(centerX, 0f, 0f),
            new Vector2(7.4f, 5.4f),
            objectiveUI,
            "Shop: buy supplies or a weapon",
            "Press E near an item to buy it",
            false);
    }

    private static void CreateFullMinibossRoom(
        Transform parent,
        float centerX,
        Sprite wallSprite,
        GameObject minibossPrefab,
        BuffChoiceController2D buffChoiceController,
        ObjectiveUI2D objectiveUI)
    {
        GameObject room = new GameObject("Miniboss_Room");
        room.transform.SetParent(parent);
        room.transform.position = new Vector3(centerX, 0f, 0f);

        CreateFullRoomWalls(room.transform, wallSprite, centerX, 5f, 4f, out DoorController2D leftDoor, out DoorController2D rightDoor);
        EnemySpawner2D enemySpawner = CreateFullEnemySpawner(
            room.transform,
            minibossPrefab,
            new[] { new Vector3(centerX + 1f, 0f, 0f) },
            1,
            new[] { "SpawnPoint_Miniboss" });

        RoomController2D roomController = room.AddComponent<RoomController2D>();
        AssignObjectReferenceArray(roomController, "doors", new[] { leftDoor, rightDoor });
        AssignObjectReference(roomController, "enemySpawner", enemySpawner);
        AssignObjectReference(roomController, "buffChoiceController", buffChoiceController);
        AssignBool(roomController, "showBuffChoiceOnClear", true);
        AssignObjectReference(roomController, "objectiveUI", objectiveUI);
        AssignString(roomController, "objectiveOnActivate", "Defeat the miniboss");
        AssignString(roomController, "objectiveOnClear", "Choose a buff, then enter the boss arena");
        AssignFloat(roomController, "doorCloseDelay", 0.5f);

        CreateFullRoomTrigger(room.transform, centerX, 8.8f, 6.8f, roomController);
    }

    private static void CreateFullBossRoom(
        Transform parent,
        float centerX,
        Sprite wallSprite,
        Sprite coverSprite,
        GameObject bossPrefab,
        LevelEndController2D levelEndController,
        ObjectiveUI2D objectiveUI)
    {
        GameObject room = new GameObject("Boss_Room");
        room.transform.SetParent(parent);
        room.transform.position = new Vector3(centerX, 0f, 0f);

        CreateFullRoomWalls(room.transform, wallSprite, centerX, 9f, 5f, out DoorController2D leftDoor, out DoorController2D rightDoor);
        CreateFullBossCover(room.transform, coverSprite, centerX);

        EnemySpawner2D enemySpawner = CreateFullEnemySpawner(
            room.transform,
            bossPrefab,
            new[] { new Vector3(centerX + 2f, 0f, 0f) },
            1,
            new[] { "SpawnPoint_Boss" });

        RoomController2D roomController = room.AddComponent<RoomController2D>();
        AssignObjectReferenceArray(roomController, "doors", new[] { leftDoor, rightDoor });
        AssignObjectReference(roomController, "enemySpawner", enemySpawner);
        AssignObjectReference(roomController, "levelEndController", levelEndController);
        AssignBool(roomController, "showVictoryOnClear", true);
        AssignString(roomController, "victoryMessage", "LABORATORY CLEARED");
        AssignObjectReference(roomController, "objectiveUI", objectiveUI);
        AssignString(roomController, "objectiveOnActivate", "Defeat Experiment-01");
        AssignString(roomController, "objectiveOnClear", "Laboratory cleared");
        AssignFloat(roomController, "doorCloseDelay", 0.5f);

        CreateFullRoomTrigger(room.transform, centerX, 16.8f, 8.6f, roomController);
        CreateCameraZone(room.transform, new Vector3(centerX, 0f, 0f), new Vector2(17.4f, 9.2f), 8f);
    }

    private static void CreateFullRoomWalls(Transform parent, Sprite sprite, float centerX, float halfWidth, float halfHeight, out DoorController2D leftDoor, out DoorController2D rightDoor)
    {
        GameObject walls = new GameObject("Walls");
        walls.transform.SetParent(parent);

        CreateWall(walls.transform, "Wall_Top", sprite, new Vector3(centerX, halfHeight, 0f), new Vector3(halfWidth * 2f + 0.5f, 0.4f, 1f));
        CreateWall(walls.transform, "Wall_Bottom", sprite, new Vector3(centerX, -halfHeight, 0f), new Vector3(halfWidth * 2f + 0.5f, 0.4f, 1f));
        CreateFullWallWithDoorGap(walls.transform, "Wall_Left", sprite, centerX - halfWidth, halfHeight);
        CreateFullWallWithDoorGap(walls.transform, "Wall_Right", sprite, centerX + halfWidth, halfHeight);

        GameObject doors = new GameObject("Doors");
        doors.transform.SetParent(parent);

        leftDoor = CreateDoor(doors.transform, "Door_Left", sprite, new Vector3(centerX - halfWidth, 0f, 0f));
        rightDoor = CreateDoor(doors.transform, "Door_Right", sprite, new Vector3(centerX + halfWidth, 0f, 0f));
    }

    private static void CreateFullWallWithDoorGap(Transform parent, string name, Sprite sprite, float x, float halfHeight)
    {
        GameObject wall = new GameObject(name);
        wall.transform.SetParent(parent);

        const float doorHalfHeight = 1.4f;
        float segmentHeight = Mathf.Max(0.1f, halfHeight - doorHalfHeight);
        float segmentCenterY = doorHalfHeight + segmentHeight * 0.5f;

        CreateWallSegment(wall.transform, name + "_Upper", sprite, new Vector3(x, segmentCenterY, 0f), new Vector3(0.4f, segmentHeight, 1f));
        CreateWallSegment(wall.transform, name + "_Lower", sprite, new Vector3(x, -segmentCenterY, 0f), new Vector3(0.4f, segmentHeight, 1f));
    }

    private static EnemySpawner2D CreateFullEnemySpawner(Transform parent, GameObject enemyPrefab, Vector3[] spawnPositions, int enemyCount, string[] spawnNames = null)
    {
        GameObject spawnerObject = new GameObject("EnemySpawner");
        spawnerObject.transform.SetParent(parent);

        Transform[] spawnPoints = new Transform[spawnPositions.Length];

        for (int i = 0; i < spawnPositions.Length; i++)
        {
            string spawnName = spawnNames != null && i < spawnNames.Length && !string.IsNullOrWhiteSpace(spawnNames[i])
                ? spawnNames[i]
                : $"SpawnPoint_{i + 1:00}";

            spawnPoints[i] = CreateMarker(spawnerObject.transform, spawnName, spawnPositions[i]);
        }

        EnemySpawner2D spawner = spawnerObject.AddComponent<EnemySpawner2D>();
        AssignObjectReference(spawner, "enemyPrefab", enemyPrefab);
        AssignObjectReferenceArray(spawner, "spawnPoints", spawnPoints);
        AssignInt(spawner, "enemyCount", enemyCount);

        return spawner;
    }

    private static RoomLootSpawner2D CreateFullLootSpawner(Transform parent, GameObject[] lootPrefabs, Vector3[] lootPositions)
    {
        GameObject lootSpawnerObject = new GameObject("LootSpawner");
        lootSpawnerObject.transform.SetParent(parent);

        string[] names =
        {
            "LootSpawnPoint_Health",
            "LootSpawnPoint_Ammo",
            "LootSpawnPoint_Money",
            "LootSpawnPoint_Weapon"
        };

        Transform[] spawnPoints = new Transform[lootPositions.Length];

        for (int i = 0; i < lootPositions.Length; i++)
        {
            string spawnName = i < names.Length ? names[i] : $"LootSpawnPoint_{i + 1:00}";
            spawnPoints[i] = CreateMarker(lootSpawnerObject.transform, spawnName, lootPositions[i]);
        }

        RoomLootSpawner2D lootSpawner = lootSpawnerObject.AddComponent<RoomLootSpawner2D>();
        AssignObjectReferenceArray(lootSpawner, "lootPrefabs", lootPrefabs);
        AssignObjectReferenceArray(lootSpawner, "spawnPoints", spawnPoints);
        AssignBool(lootSpawner, "spawnAll", true);
        AssignInt(lootSpawner, "randomSpawnCount", lootPrefabs != null ? lootPrefabs.Length : 0);

        return lootSpawner;
    }

    private static void CreateFullRoomTrigger(Transform parent, float centerX, float width, float height, RoomController2D roomController)
    {
        GameObject triggerObject = new GameObject("RoomTrigger");
        triggerObject.transform.SetParent(parent);
        triggerObject.transform.position = new Vector3(centerX, 0f, 0f);

        BoxCollider2D triggerCollider = triggerObject.AddComponent<BoxCollider2D>();
        triggerCollider.isTrigger = true;
        triggerCollider.size = new Vector2(width, height);

        RoomTrigger2D trigger = triggerObject.AddComponent<RoomTrigger2D>();
        AssignObjectReference(trigger, "roomController", roomController);
    }

    private static void CreateObjectiveTrigger(
        Transform parent,
        string name,
        Vector3 position,
        Vector2 size,
        ObjectiveUI2D objectiveUI,
        string objectiveMessage,
        string hintMessage,
        bool triggerOnce)
    {
        GameObject triggerObject = new GameObject(name);
        triggerObject.transform.SetParent(parent);
        triggerObject.transform.position = position;

        BoxCollider2D triggerCollider = triggerObject.AddComponent<BoxCollider2D>();
        triggerCollider.isTrigger = true;
        triggerCollider.size = size;

        ObjectiveTrigger2D trigger = triggerObject.AddComponent<ObjectiveTrigger2D>();
        AssignObjectReference(trigger, "objectiveUI", objectiveUI);
        AssignString(trigger, "objectiveMessage", objectiveMessage);
        AssignString(trigger, "hintMessage", hintMessage);
        AssignBool(trigger, "triggerOnce", triggerOnce);
    }

    private static void CreateFullBossCover(Transform parent, Sprite sprite, float centerX)
    {
        GameObject coverRoot = new GameObject("Cover");
        coverRoot.transform.SetParent(parent);

        CreateCoverObject(coverRoot.transform, "Cover_01", sprite, new Vector3(centerX - 3f, 2f, 0f));
        CreateCoverObject(coverRoot.transform, "Cover_02", sprite, new Vector3(centerX - 3f, -2f, 0f));
        CreateCoverObject(coverRoot.transform, "Cover_03", sprite, new Vector3(centerX + 2f, 2f, 0f));
        CreateCoverObject(coverRoot.transform, "Cover_04", sprite, new Vector3(centerX + 2f, -2f, 0f));
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

    private static void CreatePrisonStartCell(Transform parent, Sprite wallSprite, Sprite barsSprite, ObjectiveUI2D objectiveUI)
    {
        GameObject startCell = new GameObject("Start_Cell");
        startCell.transform.SetParent(parent);
        startCell.transform.position = new Vector3(-28f, 0f, 0f);

        GameObject walls = new GameObject("Walls");
        walls.transform.SetParent(startCell.transform);
        CreateWall(walls.transform, "CellWall_Top", wallSprite, new Vector3(-28f, 3f, 0f), new Vector3(7.5f, 0.35f, 1f));
        CreateWall(walls.transform, "CellWall_Bottom", wallSprite, new Vector3(-28f, -3f, 0f), new Vector3(7.5f, 0.35f, 1f));
        CreateWall(walls.transform, "CellWall_Left", wallSprite, new Vector3(-31.8f, 0f, 0f), new Vector3(0.35f, 6.2f, 1f));

        GameObject cellBars = new GameObject("CellBars");
        cellBars.transform.SetParent(startCell.transform);
        CreateWallSegment(cellBars.transform, "CellBars_Upper", barsSprite, new Vector3(-24.4f, 2.15f, 0f), new Vector3(0.24f, 1.75f, 1f));
        CreateWallSegment(cellBars.transform, "CellBars_Lower", barsSprite, new Vector3(-24.4f, -2.15f, 0f), new Vector3(0.24f, 1.75f, 1f));

        CreateObjectiveTrigger(
            startCell.transform,
            "ObjectiveTrigger_Start",
            new Vector3(-28f, 0f, 0f),
            new Vector2(7f, 5.4f),
            objectiveUI,
            "Escape the Prison",
            "WASD move | Mouse aim | Left click shoot | R reload | E interact | Esc pause",
            false);
    }

    private static void CreatePrisonGuardRoom(
        Transform parent,
        float centerX,
        Sprite wallSprite,
        Sprite barsSprite,
        GameObject guardPrefab,
        GameObject rangedGuardPrefab,
        GameObject[] lootPrefabs,
        ObjectiveUI2D objectiveUI)
    {
        GameObject room = new GameObject("Guard_Room_01");
        room.transform.SetParent(parent);
        room.transform.position = new Vector3(centerX, 0f, 0f);

        CreateFullRoomWalls(room.transform, wallSprite, centerX, 6.5f, 4f, out DoorController2D leftDoor, out DoorController2D rightDoor);

        GameObject cellBars = new GameObject("CellBars");
        cellBars.transform.SetParent(room.transform);
        CreateWallSegment(cellBars.transform, "Bars_01", barsSprite, new Vector3(centerX - 3.3f, 2.2f, 0f), new Vector3(0.18f, 1.5f, 1f));
        CreateWallSegment(cellBars.transform, "Bars_02", barsSprite, new Vector3(centerX - 3.3f, -2.2f, 0f), new Vector3(0.18f, 1.5f, 1f));

        EnemySpawner2D enemySpawner = CreatePrisonMixedEnemySpawner(
            room.transform,
            new[] { guardPrefab, guardPrefab, rangedGuardPrefab },
            new[]
            {
                new Vector3(centerX - 1.8f, 1.4f, 0f),
                new Vector3(centerX + 0.2f, -1.2f, 0f),
                new Vector3(centerX + 2.2f, 1.1f, 0f)
            },
            new[] { "SpawnPoint_01", "SpawnPoint_02", "SpawnPoint_RangedGuard" });

        RoomLootSpawner2D lootSpawner = CreateNamedLootSpawner(
            room.transform,
            lootPrefabs,
            new[]
            {
                new Vector3(centerX + 2.8f, 1.2f, 0f),
                new Vector3(centerX + 3.4f, 0f, 0f),
                new Vector3(centerX + 2.8f, -1.2f, 0f)
            },
            new[] { "LootSpawnPoint_Keycard", "LootSpawnPoint_Ammo", "LootSpawnPoint_Money" });

        RoomController2D roomController = room.AddComponent<RoomController2D>();
        AssignObjectReferenceArray(roomController, "doors", new[] { leftDoor, rightDoor });
        AssignObjectReference(roomController, "enemySpawner", enemySpawner);
        AssignObjectReference(roomController, "lootSpawner", lootSpawner);
        AssignObjectReference(roomController, "objectiveUI", objectiveUI);
        AssignString(roomController, "objectiveOnActivate", "Clear the guard room");
        AssignString(roomController, "objectiveOnClear", "Collect the keycard and open the prison gate");
        AssignFloat(roomController, "doorCloseDelay", 0.6f);

        CreateFullRoomTrigger(room.transform, centerX, 11.6f, 6.8f, roomController);
        CreateCameraZone(room.transform, new Vector3(centerX, 0f, 0f), new Vector2(13f, 8.2f), 7f);
    }

    private static void CreatePrisonLockedGateArea(Transform parent, float centerX, GameObject lockedGatePrefab, Text gatePromptText, ObjectiveUI2D objectiveUI)
    {
        GameObject area = new GameObject("LockedGate_Area");
        area.transform.SetParent(parent);
        area.transform.position = new Vector3(centerX, 0f, 0f);

        GameObject gate = InstantiatePrefab(lockedGatePrefab, area.transform, "LockedGate", new Vector3(centerX, 0f, 0f));
        LockedGate2D lockedGate = gate != null ? gate.GetComponent<LockedGate2D>() : null;

        if (lockedGate != null)
        {
            AssignObjectReference(lockedGate, "optionalPromptText", gatePromptText);
        }

        CreateObjectiveTrigger(
            area.transform,
            "ObjectiveTrigger_LockedGate",
            new Vector3(centerX, 0f, 0f),
            new Vector2(5f, 5f),
            objectiveUI,
            "Open the locked gate",
            "Press E near the gate if you have a keycard",
            false);
    }

    private static void CreatePrisonShopArea(
        Transform parent,
        float centerX,
        Sprite wallSprite,
        ShopUI2D shopUI,
        ObjectiveUI2D objectiveUI,
        GameObject healthShopPrefab,
        GameObject ammoShopPrefab,
        GameObject weaponShopPrefab)
    {
        GameObject shopArea = new GameObject("Armory_Shop");
        shopArea.transform.SetParent(parent);
        shopArea.transform.position = new Vector3(centerX, 0f, 0f);

        BoxCollider2D shopAreaTrigger = shopArea.AddComponent<BoxCollider2D>();
        shopAreaTrigger.isTrigger = true;
        shopAreaTrigger.size = new Vector2(7.4f, 5.4f);

        shopArea.AddComponent<ShopArea2D>();

        GameObject shopWalls = new GameObject("ShopWalls");
        shopWalls.transform.SetParent(shopArea.transform);
        CreateWall(shopWalls.transform, "ShopWall_Top", wallSprite, new Vector3(centerX, 3f, 0f), new Vector3(8.2f, 0.35f, 1f));
        CreateWall(shopWalls.transform, "ShopWall_Bottom", wallSprite, new Vector3(centerX, -3f, 0f), new Vector3(8.2f, 0.35f, 1f));
        CreateWall(shopWalls.transform, "ShopWall_Left_Upper", wallSprite, new Vector3(centerX - 4f, 2f, 0f), new Vector3(0.35f, 2f, 1f));
        CreateWall(shopWalls.transform, "ShopWall_Left_Lower", wallSprite, new Vector3(centerX - 4f, -2f, 0f), new Vector3(0.35f, 2f, 1f));
        CreateWall(shopWalls.transform, "ShopWall_Right_Upper", wallSprite, new Vector3(centerX + 4f, 2f, 0f), new Vector3(0.35f, 2f, 1f));
        CreateWall(shopWalls.transform, "ShopWall_Right_Lower", wallSprite, new Vector3(centerX + 4f, -2f, 0f), new Vector3(0.35f, 2f, 1f));

        InstantiateShopItem(shopArea.transform, healthShopPrefab, "ShopItem_Health", new Vector3(centerX - 1.4f, 1.2f, 0f), shopUI);
        InstantiateShopItem(shopArea.transform, ammoShopPrefab, "ShopItem_Ammo", new Vector3(centerX, 0f, 0f), shopUI);
        InstantiateShopItem(shopArea.transform, weaponShopPrefab, "ShopItem_Weapon", new Vector3(centerX + 1.4f, -1.2f, 0f), shopUI);

        CreateObjectiveTrigger(
            shopArea.transform,
            "ObjectiveTrigger_ArmoryShop",
            new Vector3(centerX, 0f, 0f),
            new Vector2(7.4f, 5.4f),
            objectiveUI,
            "Armory: buy supplies if needed",
            "Press E near an item to buy it",
            false);
    }

    private static void CreatePrisonYard(
        Transform parent,
        float centerX,
        Sprite wallSprite,
        Sprite coverSprite,
        GameObject securityLaserPrefab,
        GameObject guardPrefab,
        GameObject rangedGuardPrefab,
        GameObject[] lootPrefabs,
        ObjectiveUI2D objectiveUI)
    {
        GameObject room = new GameObject("Prison_Yard");
        room.transform.SetParent(parent);
        room.transform.position = new Vector3(centerX, 0f, 0f);

        CreateFullRoomWalls(room.transform, wallSprite, centerX, 6.5f, 4f, out DoorController2D leftDoor, out DoorController2D rightDoor);
        CreatePrisonCover(room.transform, coverSprite, centerX, new[]
        {
            new Vector3(centerX - 2.3f, 1.8f, 0f),
            new Vector3(centerX - 2.3f, -1.8f, 0f),
            new Vector3(centerX + 2.2f, 1.2f, 0f),
            new Vector3(centerX + 2.2f, -1.2f, 0f)
        });
        CreatePrisonLasers(room.transform, securityLaserPrefab, new[]
        {
            new Vector3(centerX - 0.4f, 2.2f, 0f),
            new Vector3(centerX + 0.8f, -2.2f, 0f)
        });

        EnemySpawner2D enemySpawner = CreatePrisonMixedEnemySpawner(
            room.transform,
            new[] { guardPrefab, guardPrefab, rangedGuardPrefab, rangedGuardPrefab },
            new[]
            {
                new Vector3(centerX - 2.2f, 1.3f, 0f),
                new Vector3(centerX - 1f, -1.6f, 0f),
                new Vector3(centerX + 1.7f, 1.6f, 0f),
                new Vector3(centerX + 2.4f, -1.3f, 0f)
            },
            new[] { "SpawnPoint_01", "SpawnPoint_02", "SpawnPoint_03", "SpawnPoint_RangedGuard" });

        RoomLootSpawner2D lootSpawner = CreateNamedLootSpawner(
            room.transform,
            lootPrefabs,
            new[]
            {
                new Vector3(centerX + 2.6f, 1.5f, 0f),
                new Vector3(centerX + 3.3f, 0.5f, 0f),
                new Vector3(centerX + 3.3f, -0.5f, 0f),
                new Vector3(centerX + 2.6f, -1.5f, 0f)
            },
            new[] { "LootSpawnPoint_Health", "LootSpawnPoint_Ammo", "LootSpawnPoint_Money", "LootSpawnPoint_Weapon" });

        RoomController2D roomController = room.AddComponent<RoomController2D>();
        AssignObjectReferenceArray(roomController, "doors", new[] { leftDoor, rightDoor });
        AssignObjectReference(roomController, "enemySpawner", enemySpawner);
        AssignObjectReference(roomController, "lootSpawner", lootSpawner);
        AssignObjectReference(roomController, "objectiveUI", objectiveUI);
        AssignString(roomController, "objectiveOnActivate", "Clear the prison yard");
        AssignString(roomController, "objectiveOnClear", "Proceed to the riot miniboss");
        AssignFloat(roomController, "doorCloseDelay", 0.6f);

        CreateFullRoomTrigger(room.transform, centerX, 11.6f, 6.8f, roomController);
        CreateCameraZone(room.transform, new Vector3(centerX, 0f, 0f), new Vector2(13f, 8.2f), 7f);
    }

    private static void CreatePrisonMinibossRoom(
        Transform parent,
        float centerX,
        Sprite wallSprite,
        GameObject minibossPrefab,
        BuffChoiceController2D buffChoiceController,
        ObjectiveUI2D objectiveUI)
    {
        GameObject room = new GameObject("Miniboss_Room");
        room.transform.SetParent(parent);
        room.transform.position = new Vector3(centerX, 0f, 0f);

        CreateFullRoomWalls(room.transform, wallSprite, centerX, 5f, 4f, out DoorController2D leftDoor, out DoorController2D rightDoor);
        EnemySpawner2D enemySpawner = CreatePrisonMixedEnemySpawner(
            room.transform,
            new[] { minibossPrefab },
            new[] { new Vector3(centerX + 1f, 0f, 0f) },
            new[] { "SpawnPoint_Miniboss" });

        RoomController2D roomController = room.AddComponent<RoomController2D>();
        AssignObjectReferenceArray(roomController, "doors", new[] { leftDoor, rightDoor });
        AssignObjectReference(roomController, "enemySpawner", enemySpawner);
        AssignObjectReference(roomController, "buffChoiceController", buffChoiceController);
        AssignBool(roomController, "showBuffChoiceOnClear", true);
        AssignObjectReference(roomController, "objectiveUI", objectiveUI);
        AssignString(roomController, "objectiveOnActivate", "Defeat Riot Brute");
        AssignString(roomController, "objectiveOnClear", "Choose a buff, then confront the Warden");
        AssignFloat(roomController, "doorCloseDelay", 0.6f);

        CreateFullRoomTrigger(room.transform, centerX, 8.8f, 6.8f, roomController);
        CreateCameraZone(room.transform, new Vector3(centerX, 0f, 0f), new Vector2(10f, 8.2f), 7f);
    }

    private static void CreatePrisonBossRoom(
        Transform parent,
        float centerX,
        Sprite wallSprite,
        Sprite coverSprite,
        GameObject securityLaserPrefab,
        GameObject bossPrefab,
        LevelEndController2D levelEndController,
        ObjectiveUI2D objectiveUI)
    {
        GameObject room = new GameObject("Warden_Boss_Room");
        room.transform.SetParent(parent);
        room.transform.position = new Vector3(centerX, 0f, 0f);

        CreateFullRoomWalls(room.transform, wallSprite, centerX, 9f, 5f, out DoorController2D leftDoor, out DoorController2D rightDoor);
        CreatePrisonCover(room.transform, coverSprite, centerX, new[]
        {
            new Vector3(centerX - 3f, 2f, 0f),
            new Vector3(centerX - 3f, -2f, 0f),
            new Vector3(centerX + 1.7f, 2f, 0f),
            new Vector3(centerX + 1.7f, -2f, 0f)
        });
        CreatePrisonLasers(room.transform, securityLaserPrefab, new[]
        {
            new Vector3(centerX - 5.4f, 0f, 0f),
            new Vector3(centerX + 5.4f, 0f, 0f)
        });

        EnemySpawner2D enemySpawner = CreatePrisonMixedEnemySpawner(
            room.transform,
            new[] { bossPrefab },
            new[] { new Vector3(centerX + 2f, 0f, 0f) },
            new[] { "SpawnPoint_Boss" });

        RoomController2D roomController = room.AddComponent<RoomController2D>();
        AssignObjectReferenceArray(roomController, "doors", new[] { leftDoor, rightDoor });
        AssignObjectReference(roomController, "enemySpawner", enemySpawner);
        AssignObjectReference(roomController, "levelEndController", levelEndController);
        AssignBool(roomController, "showVictoryOnClear", true);
        AssignString(roomController, "victoryMessage", "PRISON ESCAPED");
        AssignObjectReference(roomController, "objectiveUI", objectiveUI);
        AssignString(roomController, "objectiveOnActivate", "Defeat the Warden");
        AssignString(roomController, "objectiveOnClear", "Prison escaped");
        AssignFloat(roomController, "doorCloseDelay", 0.6f);

        CreateFullRoomTrigger(room.transform, centerX, 16.8f, 8.6f, roomController);
        CreateCameraZone(room.transform, new Vector3(centerX, 0f, 0f), new Vector2(17.4f, 9.2f), 8f);
    }

    private static void CreateBalancedPrisonStartCell(Transform parent, Sprite wallSprite, Sprite barsSprite, ObjectiveUI2D objectiveUI)
    {
        CreatePrisonStartCell(parent, wallSprite, barsSprite, objectiveUI);

        Transform startCell = parent.Find("Start_Cell");

        if (startCell != null)
        {
            CreateObjectiveTrigger(
                startCell,
                "ObjectiveTrigger_BalancedStartHint",
                new Vector3(-28f, 0f, 0f),
                new Vector2(7f, 5.4f),
                objectiveUI,
                "Escape the Prison",
                "Find a keycard to unlock the security gate",
                false);
        }
    }

    private static void CreateBalancedPrisonGuardRoom(
        Transform parent,
        float centerX,
        Sprite wallSprite,
        Sprite barsSprite,
        Sprite coverSprite,
        GameObject guardPrefab,
        GameObject rangedGuardPrefab,
        GameObject[] lootPrefabs,
        ObjectiveUI2D objectiveUI)
    {
        GameObject room = new GameObject("Guard_Room_01");
        room.transform.SetParent(parent);
        room.transform.position = new Vector3(centerX, 0f, 0f);

        CreateFullRoomWalls(room.transform, wallSprite, centerX, 6.5f, 4f, out DoorController2D leftDoor, out DoorController2D rightDoor);

        GameObject cellBars = new GameObject("CellBars");
        cellBars.transform.SetParent(room.transform);
        CreateWallSegment(cellBars.transform, "Bars_01", barsSprite, new Vector3(centerX - 3.3f, 2.2f, 0f), new Vector3(0.18f, 1.5f, 1f));
        CreateWallSegment(cellBars.transform, "Bars_02", barsSprite, new Vector3(centerX - 3.3f, -2.2f, 0f), new Vector3(0.18f, 1.5f, 1f));
        CreatePrisonCover(room.transform, coverSprite, centerX, new[] { new Vector3(centerX - 4.1f, 0f, 0f) });

        EnemySpawner2D enemySpawner = CreatePrisonMixedEnemySpawner(
            room.transform,
            new[] { guardPrefab, guardPrefab, rangedGuardPrefab },
            new[]
            {
                new Vector3(centerX - 0.5f, 1.45f, 0f),
                new Vector3(centerX + 0.2f, -1.35f, 0f),
                new Vector3(centerX + 3.05f, 1.15f, 0f)
            },
            new[] { "SpawnPoint_01", "SpawnPoint_02", "SpawnPoint_RangedGuard" });

        RoomLootSpawner2D lootSpawner = CreateNamedLootSpawner(
            room.transform,
            lootPrefabs,
            new[]
            {
                new Vector3(centerX + 2.4f, 0.9f, 0f),
                new Vector3(centerX + 3.3f, 0f, 0f),
                new Vector3(centerX + 2.4f, -0.9f, 0f)
            },
            new[] { "LootSpawnPoint_Keycard", "LootSpawnPoint_Ammo", "LootSpawnPoint_Money" });

        RoomController2D roomController = room.AddComponent<RoomController2D>();
        AssignObjectReferenceArray(roomController, "doors", new[] { leftDoor, rightDoor });
        AssignObjectReference(roomController, "enemySpawner", enemySpawner);
        AssignObjectReference(roomController, "lootSpawner", lootSpawner);
        AssignObjectReference(roomController, "objectiveUI", objectiveUI);
        AssignString(roomController, "objectiveOnActivate", "Clear the guard room");
        AssignString(roomController, "objectiveOnClear", "Collect the keycard and open the security gate");
        AssignFloat(roomController, "doorCloseDelay", 0.75f);

        CreateFullRoomTrigger(room.transform, centerX, 11.4f, 6.6f, roomController);
        CreateCameraZone(room.transform, new Vector3(centerX, 0f, 0f), new Vector2(13f, 8.2f), 7f);
    }

    private static void CreateBalancedPrisonLockedGateArea(Transform parent, float centerX, GameObject lockedGatePrefab, Sprite wallSprite, Sprite keycardSprite, Text gatePromptText, ObjectiveUI2D objectiveUI)
    {
        CreatePrisonLockedGateArea(parent, centerX, lockedGatePrefab, gatePromptText, objectiveUI);

        Transform area = parent.Find("LockedGate_Area");

        if (area == null)
        {
            return;
        }

        Transform gateTransform = area.Find("LockedGate");

        if (gateTransform != null)
        {
            gateTransform.position = new Vector3(centerX, 0f, 0f);
            gateTransform.localScale = new Vector3(0.48f, 2.45f, 1f);
        }

        CreateBalancedGateFrame(area, wallSprite, centerX);

        GameObject marker = new GameObject("KeycardGateMarker");
        marker.transform.SetParent(area);
        marker.transform.position = new Vector3(centerX + 0.65f, 0f, 0f);
        marker.transform.localScale = new Vector3(0.32f, 0.32f, 1f);

        SpriteRenderer markerRenderer = marker.AddComponent<SpriteRenderer>();
        markerRenderer.sprite = keycardSprite;
        markerRenderer.sortingOrder = 20;

        LockedGate2D gate = area.Find("LockedGate")?.GetComponent<LockedGate2D>();

        if (gate != null)
        {
            AssignObjectReference(gate, "feedbackText", gatePromptText);
            AssignFloat(gate, "feedbackDuration", 2f);
        }
    }

    private static void CreateBalancedGateFrame(Transform parent, Sprite wallSprite, float centerX)
    {
        if (wallSprite == null)
        {
            return;
        }

        GameObject frame = new GameObject("GateFrame");
        frame.transform.SetParent(parent);

        CreateWallSegment(frame.transform, "GateCorridor_Top", wallSprite, new Vector3(centerX, 1.4f, 0f), new Vector3(6.6f, 0.25f, 1f));
        CreateWallSegment(frame.transform, "GateCorridor_Bottom", wallSprite, new Vector3(centerX, -1.4f, 0f), new Vector3(6.6f, 0.25f, 1f));
    }

    private static void CreateBalancedPrisonYard(
        Transform parent,
        float centerX,
        Sprite wallSprite,
        Sprite coverSprite,
        GameObject securityLaserPrefab,
        GameObject guardPrefab,
        GameObject rangedGuardPrefab,
        GameObject[] lootPrefabs,
        ObjectiveUI2D objectiveUI)
    {
        GameObject room = new GameObject("Prison_Yard");
        room.transform.SetParent(parent);
        room.transform.position = new Vector3(centerX, 0f, 0f);

        CreateFullRoomWalls(room.transform, wallSprite, centerX, 7f, 4.3f, out DoorController2D leftDoor, out DoorController2D rightDoor);
        CreatePrisonCover(room.transform, coverSprite, centerX, new[]
        {
            new Vector3(centerX - 3.4f, 1.8f, 0f),
            new Vector3(centerX - 2.5f, -1.8f, 0f),
            new Vector3(centerX + 1.1f, 1.9f, 0f),
            new Vector3(centerX + 2.5f, -1.5f, 0f)
        });
        CreatePrisonLasers(room.transform, securityLaserPrefab, new[]
        {
            new Vector3(centerX - 0.8f, 2.35f, 0f),
            new Vector3(centerX + 0.6f, -2.35f, 0f),
            new Vector3(centerX + 3.6f, 0f, 0f)
        });

        EnemySpawner2D enemySpawner = CreatePrisonMixedEnemySpawner(
            room.transform,
            new[] { guardPrefab, guardPrefab, rangedGuardPrefab, rangedGuardPrefab },
            new[]
            {
                new Vector3(centerX - 2.6f, 1.1f, 0f),
                new Vector3(centerX - 1.5f, -1.4f, 0f),
                new Vector3(centerX + 1.8f, 1.7f, 0f),
                new Vector3(centerX + 3f, -1.2f, 0f)
            },
            new[] { "SpawnPoint_01", "SpawnPoint_02", "SpawnPoint_03", "SpawnPoint_RangedGuard" });

        RoomLootSpawner2D lootSpawner = CreateNamedLootSpawner(
            room.transform,
            lootPrefabs,
            new[]
            {
                new Vector3(centerX + 2.7f, 1.5f, 0f),
                new Vector3(centerX + 3.6f, 0.5f, 0f),
                new Vector3(centerX + 3.6f, -0.5f, 0f),
                new Vector3(centerX + 2.7f, -1.5f, 0f)
            },
            new[] { "LootSpawnPoint_Health", "LootSpawnPoint_Ammo", "LootSpawnPoint_Money", "LootSpawnPoint_Weapon" });

        RoomController2D roomController = room.AddComponent<RoomController2D>();
        AssignObjectReferenceArray(roomController, "doors", new[] { leftDoor, rightDoor });
        AssignObjectReference(roomController, "enemySpawner", enemySpawner);
        AssignObjectReference(roomController, "lootSpawner", lootSpawner);
        AssignObjectReference(roomController, "objectiveUI", objectiveUI);
        AssignString(roomController, "objectiveOnActivate", "Survive the prison yard lockdown");
        AssignString(roomController, "objectiveOnClear", "Proceed to the Riot Brute");
        AssignFloat(roomController, "doorCloseDelay", 0.75f);

        CreateFullRoomTrigger(room.transform, centerX, 12.6f, 7.2f, roomController);
        CreateCameraZone(room.transform, new Vector3(centerX, 0f, 0f), new Vector2(14f, 8.6f), 7.3f);
    }

    private static void CreateBalancedPrisonMinibossRoom(
        Transform parent,
        float centerX,
        Sprite wallSprite,
        Sprite coverSprite,
        GameObject minibossPrefab,
        BuffChoiceController2D buffChoiceController,
        ObjectiveUI2D objectiveUI)
    {
        GameObject room = new GameObject("Miniboss_Room");
        room.transform.SetParent(parent);
        room.transform.position = new Vector3(centerX, 0f, 0f);

        CreateFullRoomWalls(room.transform, wallSprite, centerX, 5.5f, 4.2f, out DoorController2D leftDoor, out DoorController2D rightDoor);
        CreatePrisonCover(room.transform, coverSprite, centerX, new[]
        {
            new Vector3(centerX - 2f, 1.8f, 0f),
            new Vector3(centerX + 1.8f, -1.8f, 0f)
        });

        EnemySpawner2D enemySpawner = CreatePrisonMixedEnemySpawner(
            room.transform,
            new[] { minibossPrefab },
            new[] { new Vector3(centerX + 1.2f, 0f, 0f) },
            new[] { "SpawnPoint_Miniboss" });

        RoomController2D roomController = room.AddComponent<RoomController2D>();
        AssignObjectReferenceArray(roomController, "doors", new[] { leftDoor, rightDoor });
        AssignObjectReference(roomController, "enemySpawner", enemySpawner);
        AssignObjectReference(roomController, "buffChoiceController", buffChoiceController);
        AssignBool(roomController, "showBuffChoiceOnClear", true);
        AssignObjectReference(roomController, "objectiveUI", objectiveUI);
        AssignString(roomController, "objectiveOnActivate", "Defeat Riot Brute");
        AssignString(roomController, "objectiveOnClear", "Choose a buff, then confront the Warden");
        AssignFloat(roomController, "doorCloseDelay", 0.75f);

        CreateFullRoomTrigger(room.transform, centerX, 9.6f, 7.2f, roomController);
        CreateCameraZone(room.transform, new Vector3(centerX, 0f, 0f), new Vector2(11f, 8.4f), 7f);
    }

    private static void CreateBalancedPrisonBossRoom(
        Transform parent,
        float centerX,
        Sprite wallSprite,
        Sprite coverSprite,
        GameObject securityLaserPrefab,
        GameObject bossPrefab,
        LevelEndController2D levelEndController,
        ObjectiveUI2D objectiveUI)
    {
        GameObject room = new GameObject("Warden_Boss_Room");
        room.transform.SetParent(parent);
        room.transform.position = new Vector3(centerX, 0f, 0f);

        CreateFullRoomWalls(room.transform, wallSprite, centerX, 9f, 5f, out DoorController2D leftDoor, out DoorController2D rightDoor);
        CreatePrisonCover(room.transform, coverSprite, centerX, new[]
        {
            new Vector3(centerX - 3.8f, 2f, 0f),
            new Vector3(centerX - 3.8f, -2f, 0f),
            new Vector3(centerX + 0.9f, 2.1f, 0f),
            new Vector3(centerX + 0.9f, -2.1f, 0f)
        });
        CreatePrisonLasers(room.transform, securityLaserPrefab, new[]
        {
            new Vector3(centerX - 5.7f, 0f, 0f),
            new Vector3(centerX + 5.7f, 0f, 0f)
        });

        EnemySpawner2D enemySpawner = CreatePrisonMixedEnemySpawner(
            room.transform,
            new[] { bossPrefab },
            new[] { new Vector3(centerX + 2.2f, 0f, 0f) },
            new[] { "SpawnPoint_Boss" });

        RoomController2D roomController = room.AddComponent<RoomController2D>();
        AssignObjectReferenceArray(roomController, "doors", new[] { leftDoor, rightDoor });
        AssignObjectReference(roomController, "enemySpawner", enemySpawner);
        AssignObjectReference(roomController, "levelEndController", levelEndController);
        AssignBool(roomController, "showVictoryOnClear", true);
        AssignString(roomController, "victoryMessage", "PRISON ESCAPED");
        AssignObjectReference(roomController, "objectiveUI", objectiveUI);
        AssignString(roomController, "objectiveOnActivate", "Defeat the Warden");
        AssignString(roomController, "objectiveOnClear", "Prison escaped");
        AssignFloat(roomController, "doorCloseDelay", 0.75f);

        CreateFullRoomTrigger(room.transform, centerX, 16.8f, 8.6f, roomController);
        CreateCameraZone(room.transform, new Vector3(centerX, 0f, 0f), new Vector2(17.4f, 9.2f), 8f);
    }

    private static void CreatePrisonAlarmLights(Transform parent, Sprite alarmLightSprite)
    {
        GameObject atmosphereObject = new GameObject("PrisonAtmosphere");
        atmosphereObject.transform.SetParent(parent);

        Vector3[] positions =
        {
            new Vector3(-24f, 3.45f, 0f),
            new Vector3(-14f, 3.45f, 0f),
            new Vector3(7f, 3.75f, 0f),
            new Vector3(23f, 3.55f, 0f),
            new Vector3(38f, 4.25f, 0f)
        };

        SpriteRenderer[] alarmLights = new SpriteRenderer[positions.Length];

        for (int i = 0; i < positions.Length; i++)
        {
            GameObject lightObject = new GameObject($"AlarmLight_{i + 1:00}");
            lightObject.transform.SetParent(atmosphereObject.transform);
            lightObject.transform.position = positions[i];
            lightObject.transform.localScale = new Vector3(0.35f, 0.35f, 1f);

            SpriteRenderer renderer = lightObject.AddComponent<SpriteRenderer>();
            renderer.sprite = alarmLightSprite;
            renderer.sortingOrder = 25;
            alarmLights[i] = renderer;
        }

        PrisonAtmosphereController2D atmosphere = atmosphereObject.AddComponent<PrisonAtmosphereController2D>();
        AssignObjectReferenceArray(atmosphere, "alarmLights", alarmLights);
        AssignFloat(atmosphere, "pulseSpeed", 2f);
        AssignColor(atmosphere, "normalColor", new Color(0.25f, 0.06f, 0.04f, 0.45f));
        AssignColor(atmosphere, "alarmColor", new Color(1f, 0.08f, 0.04f, 1f));
        AssignBool(atmosphere, "alarmActive", true);
    }

    private static EnemySpawner2D CreatePrisonMixedEnemySpawner(Transform parent, GameObject[] enemyPrefabs, Vector3[] spawnPositions, string[] spawnNames)
    {
        GameObject spawnerObject = new GameObject("EnemySpawner");
        spawnerObject.transform.SetParent(parent);

        Transform[] spawnPoints = new Transform[spawnPositions.Length];

        for (int i = 0; i < spawnPositions.Length; i++)
        {
            string spawnName = spawnNames != null && i < spawnNames.Length && !string.IsNullOrWhiteSpace(spawnNames[i])
                ? spawnNames[i]
                : $"SpawnPoint_{i + 1:00}";

            spawnPoints[i] = CreateMarker(spawnerObject.transform, spawnName, spawnPositions[i]);
        }

        EnemySpawner2D spawner = spawnerObject.AddComponent<EnemySpawner2D>();
        AssignObjectReference(spawner, "enemyPrefab", enemyPrefabs != null && enemyPrefabs.Length > 0 ? enemyPrefabs[0] : null);
        AssignObjectReferenceArray(spawner, "enemyPrefabs", enemyPrefabs);
        AssignObjectReferenceArray(spawner, "spawnPoints", spawnPoints);
        AssignInt(spawner, "enemyCount", enemyPrefabs != null ? enemyPrefabs.Length : 0);

        return spawner;
    }

    private static RoomLootSpawner2D CreateNamedLootSpawner(Transform parent, GameObject[] lootPrefabs, Vector3[] lootPositions, string[] spawnNames)
    {
        GameObject lootSpawnerObject = new GameObject("LootSpawner");
        lootSpawnerObject.transform.SetParent(parent);

        Transform[] spawnPoints = new Transform[lootPositions.Length];

        for (int i = 0; i < lootPositions.Length; i++)
        {
            string spawnName = spawnNames != null && i < spawnNames.Length && !string.IsNullOrWhiteSpace(spawnNames[i])
                ? spawnNames[i]
                : $"LootSpawnPoint_{i + 1:00}";

            spawnPoints[i] = CreateMarker(lootSpawnerObject.transform, spawnName, lootPositions[i]);
        }

        RoomLootSpawner2D lootSpawner = lootSpawnerObject.AddComponent<RoomLootSpawner2D>();
        AssignObjectReferenceArray(lootSpawner, "lootPrefabs", lootPrefabs);
        AssignObjectReferenceArray(lootSpawner, "spawnPoints", spawnPoints);
        AssignBool(lootSpawner, "spawnAll", true);
        AssignInt(lootSpawner, "randomSpawnCount", lootPrefabs != null ? lootPrefabs.Length : 0);

        return lootSpawner;
    }

    private static void CreatePrisonCover(Transform parent, Sprite sprite, float centerX, Vector3[] positions)
    {
        GameObject coverRoot = new GameObject("Cover");
        coverRoot.transform.SetParent(parent);

        for (int i = 0; i < positions.Length; i++)
        {
            CreateCoverObject(coverRoot.transform, $"Cover_{i + 1:00}", sprite, positions[i]);
        }
    }

    private static void CreatePrisonLasers(Transform parent, GameObject securityLaserPrefab, Vector3[] positions)
    {
        GameObject lasersRoot = new GameObject("SecurityLasers");
        lasersRoot.transform.SetParent(parent);

        for (int i = 0; i < positions.Length; i++)
        {
            GameObject laser = InstantiatePrefab(securityLaserPrefab, lasersRoot.transform, $"Laser_{i + 1:00}", positions[i]);

            if (laser != null && i % 2 == 1)
            {
                laser.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            }
        }
    }

    private static GameObject InstantiatePrefab(GameObject prefab, Transform parent, string fallbackName, Vector3 position)
    {
        if (prefab == null)
        {
            GameObject fallback = new GameObject(fallbackName);
            fallback.transform.SetParent(parent);
            fallback.transform.position = position;
            return fallback;
        }

        GameObject instance = PrefabUtility.InstantiatePrefab(prefab, parent) as GameObject;

        if (instance == null)
        {
            instance = UnityEngine.Object.Instantiate(prefab, position, Quaternion.identity, parent);
        }

        instance.name = fallbackName;
        instance.transform.position = position;
        return instance;
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

    private static EnemySpawner2D CreateMinibossSpawner(Transform parent, GameObject minibossPrefab)
    {
        GameObject spawnerObject = new GameObject("EnemySpawner");
        spawnerObject.transform.SetParent(parent);

        Transform[] spawnPoints =
        {
            CreateMarker(spawnerObject.transform, "SpawnPoint_Miniboss", new Vector3(2.5f, 0f, 0f))
        };

        EnemySpawner2D spawner = spawnerObject.AddComponent<EnemySpawner2D>();
        AssignObjectReference(spawner, "enemyPrefab", minibossPrefab);
        AssignObjectReferenceArray(spawner, "spawnPoints", spawnPoints);
        AssignInt(spawner, "enemyCount", 1);

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

    private static RoomLootSpawner2D CreateWeaponRoomLootSpawner(Transform parent, GameObject[] lootPrefabs)
    {
        GameObject lootSpawnerObject = new GameObject("LootSpawner");
        lootSpawnerObject.transform.SetParent(parent);

        Transform[] spawnPoints =
        {
            CreateMarker(lootSpawnerObject.transform, "LootSpawnPoint_Health", new Vector3(3.8f, 1.3f, 0f)),
            CreateMarker(lootSpawnerObject.transform, "LootSpawnPoint_Ammo", new Vector3(4.5f, 0.45f, 0f)),
            CreateMarker(lootSpawnerObject.transform, "LootSpawnPoint_Money", new Vector3(4.5f, -0.45f, 0f)),
            CreateMarker(lootSpawnerObject.transform, "LootSpawnPoint_Weapon", new Vector3(3.8f, -1.3f, 0f))
        };

        RoomLootSpawner2D lootSpawner = lootSpawnerObject.AddComponent<RoomLootSpawner2D>();
        AssignObjectReferenceArray(lootSpawner, "lootPrefabs", lootPrefabs);
        AssignObjectReferenceArray(lootSpawner, "spawnPoints", spawnPoints);
        AssignBool(lootSpawner, "spawnAll", true);
        AssignInt(lootSpawner, "randomSpawnCount", 4);

        return lootSpawner;
    }

    private static RoomLootSpawner2D CreateShopRoomLootSpawner(Transform parent, GameObject moneyPickupPrefab)
    {
        GameObject lootSpawnerObject = new GameObject("LootSpawner");
        lootSpawnerObject.transform.SetParent(parent);

        Transform[] spawnPoints =
        {
            CreateMarker(lootSpawnerObject.transform, "LootSpawnPoint_Money", new Vector3(5.2f, 0f, 0f))
        };

        RoomLootSpawner2D lootSpawner = lootSpawnerObject.AddComponent<RoomLootSpawner2D>();
        AssignObjectReferenceArray(lootSpawner, "lootPrefabs", new[] { moneyPickupPrefab });
        AssignObjectReferenceArray(lootSpawner, "spawnPoints", spawnPoints);
        AssignBool(lootSpawner, "spawnAll", true);
        AssignInt(lootSpawner, "randomSpawnCount", 1);

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

    private static GameObject CreateShopArea(
        Transform parent,
        Sprite wallSprite,
        ShopUI2D shopUI,
        GameObject healthShopPrefab,
        GameObject ammoShopPrefab,
        GameObject weaponShopPrefab)
    {
        GameObject shopArea = new GameObject("Shop_Area");
        shopArea.transform.SetParent(parent);
        shopArea.transform.position = Vector3.zero;

        BoxCollider2D areaTrigger = shopArea.AddComponent<BoxCollider2D>();
        areaTrigger.isTrigger = true;
        areaTrigger.offset = new Vector2(11f, 0f);
        areaTrigger.size = new Vector2(6f, 6f);

        shopArea.AddComponent<ShopArea2D>();

        GameObject shopWalls = new GameObject("ShopWalls");
        shopWalls.transform.SetParent(shopArea.transform);

        CreateWall(shopWalls.transform, "ShopWall_Top", wallSprite, new Vector3(11f, 3f, 0f), new Vector3(6.2f, 0.35f, 1f));
        CreateWall(shopWalls.transform, "ShopWall_Bottom", wallSprite, new Vector3(11f, -3f, 0f), new Vector3(6.2f, 0.35f, 1f));
        CreateWall(shopWalls.transform, "ShopWall_Right", wallSprite, new Vector3(14f, 0f, 0f), new Vector3(0.35f, 6.2f, 1f));
        CreateWall(shopWalls.transform, "ShopWall_Left_Upper", wallSprite, new Vector3(8f, 2f, 0f), new Vector3(0.35f, 2f, 1f));
        CreateWall(shopWalls.transform, "ShopWall_Left_Lower", wallSprite, new Vector3(8f, -2f, 0f), new Vector3(0.35f, 2f, 1f));

        InstantiateShopItem(shopArea.transform, healthShopPrefab, "ShopItem_Health", new Vector3(10f, 1.5f, 0f), shopUI);
        InstantiateShopItem(shopArea.transform, ammoShopPrefab, "ShopItem_Ammo", new Vector3(11f, 0f, 0f), shopUI);
        InstantiateShopItem(shopArea.transform, weaponShopPrefab, "ShopItem_Weapon", new Vector3(12f, -1.5f, 0f), shopUI);

        return shopArea;
    }

    private static void InstantiateShopItem(Transform parent, GameObject prefab, string fallbackName, Vector3 position, ShopUI2D shopUI)
    {
        GameObject itemObject;

        if (prefab != null)
        {
            itemObject = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        }
        else
        {
            itemObject = new GameObject(fallbackName);
        }

        if (itemObject == null)
        {
            Debug.LogWarning($"Could not instantiate shop item '{fallbackName}'.");
            return;
        }

        itemObject.name = fallbackName;
        itemObject.transform.SetParent(parent);
        itemObject.transform.position = position;
        itemObject.transform.localRotation = Quaternion.identity;

        ShopItem2D shopItem = itemObject.GetComponent<ShopItem2D>();

        if (shopItem != null)
        {
            AssignObjectReference(shopItem, "shopUI", shopUI);
        }
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

    private static GameObject CreateWeaponLootUI(Transform parent, PlayerHealth2D playerHealth, PlayerResources2D playerResources, PlayerShooting2D playerShooting)
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
        SetRectTransform(resourcePanel.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(24f, -76f), new Vector2(430f, 92f));

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
        SetRectTransform(moneyTextObject.GetComponent<RectTransform>(), new Vector2(0f, 0.5f), new Vector2(1f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(-16f, 24f));

        Text moneyText = moneyTextObject.AddComponent<Text>();
        moneyText.font = GetBuiltinUIFont();
        moneyText.text = "Money: 0";
        moneyText.fontSize = 16;
        moneyText.alignment = TextAnchor.MiddleLeft;
        moneyText.color = Color.white;
        moneyText.raycastTarget = false;

        GameObject weaponTextObject = CreateRectObject("WeaponText", resourcePanel.transform);
        SetRectTransform(weaponTextObject.GetComponent<RectTransform>(), new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 8f), new Vector2(-16f, 24f));

        Text weaponText = weaponTextObject.AddComponent<Text>();
        weaponText.font = GetBuiltinUIFont();
        weaponText.text = "Weapon: Pistol [Common]";
        weaponText.fontSize = 16;
        weaponText.alignment = TextAnchor.MiddleLeft;
        weaponText.color = Color.white;
        weaponText.raycastTarget = false;

        ResourceUI2D resourceUI = resourcePanel.AddComponent<ResourceUI2D>();
        AssignObjectReference(resourceUI, "playerResources", playerResources);
        AssignObjectReference(resourceUI, "playerShooting", playerShooting);
        AssignObjectReference(resourceUI, "ammoText", ammoText);
        AssignObjectReference(resourceUI, "moneyText", moneyText);

        WeaponUI2D weaponUI = resourcePanel.AddComponent<WeaponUI2D>();
        AssignObjectReference(weaponUI, "playerShooting", playerShooting);
        AssignObjectReference(weaponUI, "weaponText", weaponText);

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

    private static GameObject CreateShopSceneUI(
        Transform parent,
        PlayerHealth2D playerHealth,
        PlayerResources2D playerResources,
        PlayerShooting2D playerShooting,
        out ShopUI2D shopUI)
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
        SetRectTransform(resourcePanel.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(24f, -76f), new Vector2(430f, 92f));

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
        SetRectTransform(moneyTextObject.GetComponent<RectTransform>(), new Vector2(0f, 0.5f), new Vector2(1f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(-16f, 24f));

        Text moneyText = moneyTextObject.AddComponent<Text>();
        moneyText.font = GetBuiltinUIFont();
        moneyText.text = "Money: 100";
        moneyText.fontSize = 16;
        moneyText.alignment = TextAnchor.MiddleLeft;
        moneyText.color = Color.white;
        moneyText.raycastTarget = false;

        GameObject weaponTextObject = CreateRectObject("WeaponText", resourcePanel.transform);
        SetRectTransform(weaponTextObject.GetComponent<RectTransform>(), new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 8f), new Vector2(-16f, 24f));

        Text weaponText = weaponTextObject.AddComponent<Text>();
        weaponText.font = GetBuiltinUIFont();
        weaponText.text = "Weapon: Pistol [Common]";
        weaponText.fontSize = 16;
        weaponText.alignment = TextAnchor.MiddleLeft;
        weaponText.color = Color.white;
        weaponText.raycastTarget = false;

        ResourceUI2D resourceUI = resourcePanel.AddComponent<ResourceUI2D>();
        AssignObjectReference(resourceUI, "playerResources", playerResources);
        AssignObjectReference(resourceUI, "playerShooting", playerShooting);
        AssignObjectReference(resourceUI, "ammoText", ammoText);
        AssignObjectReference(resourceUI, "moneyText", moneyText);

        WeaponUI2D weaponUI = resourcePanel.AddComponent<WeaponUI2D>();
        AssignObjectReference(weaponUI, "playerShooting", playerShooting);
        AssignObjectReference(weaponUI, "weaponText", weaponText);

        GameObject shopPanel = CreateRectObject("ShopPanel", canvasObject.transform);
        SetRectTransform(shopPanel.GetComponent<RectTransform>(), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 36f), new Vector2(460f, 74f));

        Image shopPanelImage = shopPanel.AddComponent<Image>();
        shopPanelImage.color = new Color(0f, 0f, 0f, 0.78f);

        GameObject shopPromptTextObject = CreateRectObject("ShopPromptText", shopPanel.transform);
        SetRectTransform(shopPromptTextObject.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -8f), new Vector2(-24f, 28f));

        Text shopPromptText = shopPromptTextObject.AddComponent<Text>();
        shopPromptText.font = GetBuiltinUIFont();
        shopPromptText.text = string.Empty;
        shopPromptText.fontSize = 17;
        shopPromptText.alignment = TextAnchor.MiddleCenter;
        shopPromptText.color = Color.white;
        shopPromptText.raycastTarget = false;

        GameObject shopMessageTextObject = CreateRectObject("ShopMessageText", shopPanel.transform);
        SetRectTransform(shopMessageTextObject.GetComponent<RectTransform>(), new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 8f), new Vector2(-24f, 28f));

        Text shopMessageText = shopMessageTextObject.AddComponent<Text>();
        shopMessageText.font = GetBuiltinUIFont();
        shopMessageText.text = string.Empty;
        shopMessageText.fontSize = 17;
        shopMessageText.alignment = TextAnchor.MiddleCenter;
        shopMessageText.color = new Color(1f, 0.86f, 0.2f, 1f);
        shopMessageText.raycastTarget = false;

        shopUI = canvasObject.AddComponent<ShopUI2D>();
        AssignObjectReference(shopUI, "shopPanel", shopPanel);
        AssignObjectReference(shopUI, "promptText", shopPromptText);
        AssignObjectReference(shopUI, "messageText", shopMessageText);
        AssignFloat(shopUI, "messageDuration", 2f);
        shopPanel.SetActive(false);

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

    private static GameObject CreateMinibossBuffUI(
        Transform parent,
        PlayerHealth2D playerHealth,
        PlayerResources2D playerResources,
        PlayerShooting2D playerShooting,
        PlayerBuffs2D playerBuffs,
        out GameObject choicePanel,
        out Button[] choiceButtons,
        out Text[] choiceTexts)
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
        SetRectTransform(resourcePanel.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(24f, -76f), new Vector2(460f, 120f));

        Image resourcePanelImage = resourcePanel.AddComponent<Image>();
        resourcePanelImage.color = new Color(0f, 0f, 0f, 0.65f);

        GameObject ammoTextObject = CreateRectObject("AmmoText", resourcePanel.transform);
        SetRectTransform(ammoTextObject.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -8f), new Vector2(-16f, 24f));

        Text ammoText = ammoTextObject.AddComponent<Text>();
        ammoText.font = GetBuiltinUIFont();
        ammoText.text = "Ammo: 12 / 12 | Reserve: 60 / 120";
        ammoText.fontSize = 16;
        ammoText.alignment = TextAnchor.MiddleLeft;
        ammoText.color = Color.white;
        ammoText.raycastTarget = false;

        GameObject moneyTextObject = CreateRectObject("MoneyText", resourcePanel.transform);
        SetRectTransform(moneyTextObject.GetComponent<RectTransform>(), new Vector2(0f, 0.67f), new Vector2(1f, 0.67f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(-16f, 24f));

        Text moneyText = moneyTextObject.AddComponent<Text>();
        moneyText.font = GetBuiltinUIFont();
        moneyText.text = "Money: 50";
        moneyText.fontSize = 16;
        moneyText.alignment = TextAnchor.MiddleLeft;
        moneyText.color = Color.white;
        moneyText.raycastTarget = false;

        GameObject weaponTextObject = CreateRectObject("WeaponText", resourcePanel.transform);
        SetRectTransform(weaponTextObject.GetComponent<RectTransform>(), new Vector2(0f, 0.34f), new Vector2(1f, 0.34f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(-16f, 24f));

        Text weaponText = weaponTextObject.AddComponent<Text>();
        weaponText.font = GetBuiltinUIFont();
        weaponText.text = "Weapon: Pistol [Common]";
        weaponText.fontSize = 16;
        weaponText.alignment = TextAnchor.MiddleLeft;
        weaponText.color = Color.white;
        weaponText.raycastTarget = false;

        GameObject buffStatusTextObject = CreateRectObject("BuffStatusText", resourcePanel.transform);
        SetRectTransform(buffStatusTextObject.GetComponent<RectTransform>(), new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 8f), new Vector2(-16f, 24f));

        Text buffStatusText = buffStatusTextObject.AddComponent<Text>();
        buffStatusText.font = GetBuiltinUIFont();
        buffStatusText.text = "Buffs: none";
        buffStatusText.fontSize = 16;
        buffStatusText.alignment = TextAnchor.MiddleLeft;
        buffStatusText.color = Color.white;
        buffStatusText.raycastTarget = false;

        ResourceUI2D resourceUI = resourcePanel.AddComponent<ResourceUI2D>();
        AssignObjectReference(resourceUI, "playerResources", playerResources);
        AssignObjectReference(resourceUI, "playerShooting", playerShooting);
        AssignObjectReference(resourceUI, "ammoText", ammoText);
        AssignObjectReference(resourceUI, "moneyText", moneyText);

        WeaponUI2D weaponUI = resourcePanel.AddComponent<WeaponUI2D>();
        AssignObjectReference(weaponUI, "playerShooting", playerShooting);
        AssignObjectReference(weaponUI, "weaponText", weaponText);

        BuffStatusUI2D buffStatusUI = resourcePanel.AddComponent<BuffStatusUI2D>();
        AssignObjectReference(buffStatusUI, "playerBuffs", playerBuffs);
        AssignObjectReference(buffStatusUI, "buffStatusText", buffStatusText);

        choicePanel = CreateRectObject("BuffChoicePanel", canvasObject.transform);
        SetRectTransform(choicePanel.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(620f, 330f));

        Image choicePanelImage = choicePanel.AddComponent<Image>();
        choicePanelImage.color = new Color(0f, 0f, 0f, 0.86f);

        GameObject buffTitleObject = CreateRectObject("BuffTitleText", choicePanel.transform);
        SetRectTransform(buffTitleObject.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -18f), new Vector2(-32f, 42f));

        Text buffTitleText = buffTitleObject.AddComponent<Text>();
        buffTitleText.font = GetBuiltinUIFont();
        buffTitleText.text = "Choose a Buff";
        buffTitleText.fontSize = 26;
        buffTitleText.alignment = TextAnchor.MiddleCenter;
        buffTitleText.color = Color.white;
        buffTitleText.raycastTarget = false;

        choiceButtons = new Button[3];
        choiceTexts = new Text[3];
        choiceButtons[0] = CreateBuffChoiceButton(choicePanel.transform, "BuffButton_01", new Vector2(0f, 62f), out choiceTexts[0]);
        choiceButtons[1] = CreateBuffChoiceButton(choicePanel.transform, "BuffButton_02", new Vector2(0f, -28f), out choiceTexts[1]);
        choiceButtons[2] = CreateBuffChoiceButton(choicePanel.transform, "BuffButton_03", new Vector2(0f, -118f), out choiceTexts[2]);
        choicePanel.SetActive(false);

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

    private static GameObject CreateBossFightUI(
        Transform parent,
        PlayerHealth2D playerHealth,
        PlayerResources2D playerResources,
        PlayerShooting2D playerShooting,
        PlayerBuffs2D playerBuffs,
        out GameObject victoryPanel,
        out Text victoryText)
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

        GameObject healthBackground = CreateRectObject("HealthBackground", healthPanel.transform);
        StretchRectTransform(healthBackground.GetComponent<RectTransform>(), Vector2.zero, Vector2.zero);
        Image healthBackgroundImage = healthBackground.AddComponent<Image>();
        healthBackgroundImage.color = new Color(0f, 0f, 0f, 0.65f);

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
        healthText.text = "HP: 6 / 6";
        healthText.fontSize = 18;
        healthText.alignment = TextAnchor.MiddleCenter;
        healthText.color = Color.white;
        healthText.raycastTarget = false;

        HealthUI2D healthUI = healthPanel.AddComponent<HealthUI2D>();
        AssignObjectReference(healthUI, "playerHealth", playerHealth);
        AssignObjectReference(healthUI, "healthFill", healthFill);
        AssignObjectReference(healthUI, "healthText", healthText);

        GameObject resourcePanel = CreateRectObject("ResourcePanel", canvasObject.transform);
        SetRectTransform(resourcePanel.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(24f, -76f), new Vector2(460f, 120f));

        Image resourcePanelImage = resourcePanel.AddComponent<Image>();
        resourcePanelImage.color = new Color(0f, 0f, 0f, 0.65f);

        GameObject ammoTextObject = CreateRectObject("AmmoText", resourcePanel.transform);
        SetRectTransform(ammoTextObject.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -8f), new Vector2(-16f, 24f));

        Text ammoText = ammoTextObject.AddComponent<Text>();
        ammoText.font = GetBuiltinUIFont();
        ammoText.text = "Ammo: 30 / 30 | Reserve: 90 / 150";
        ammoText.fontSize = 16;
        ammoText.alignment = TextAnchor.MiddleLeft;
        ammoText.color = Color.white;
        ammoText.raycastTarget = false;

        GameObject moneyTextObject = CreateRectObject("MoneyText", resourcePanel.transform);
        SetRectTransform(moneyTextObject.GetComponent<RectTransform>(), new Vector2(0f, 0.67f), new Vector2(1f, 0.67f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(-16f, 24f));

        Text moneyText = moneyTextObject.AddComponent<Text>();
        moneyText.font = GetBuiltinUIFont();
        moneyText.text = "Money: 0";
        moneyText.fontSize = 16;
        moneyText.alignment = TextAnchor.MiddleLeft;
        moneyText.color = Color.white;
        moneyText.raycastTarget = false;

        GameObject weaponTextObject = CreateRectObject("WeaponText", resourcePanel.transform);
        SetRectTransform(weaponTextObject.GetComponent<RectTransform>(), new Vector2(0f, 0.34f), new Vector2(1f, 0.34f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(-16f, 24f));

        Text weaponText = weaponTextObject.AddComponent<Text>();
        weaponText.font = GetBuiltinUIFont();
        weaponText.text = "Weapon: SMG [Uncommon]";
        weaponText.fontSize = 16;
        weaponText.alignment = TextAnchor.MiddleLeft;
        weaponText.color = Color.white;
        weaponText.raycastTarget = false;

        GameObject buffStatusTextObject = CreateRectObject("BuffStatusText", resourcePanel.transform);
        SetRectTransform(buffStatusTextObject.GetComponent<RectTransform>(), new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 8f), new Vector2(-16f, 24f));

        Text buffStatusText = buffStatusTextObject.AddComponent<Text>();
        buffStatusText.font = GetBuiltinUIFont();
        buffStatusText.text = "Buffs: none";
        buffStatusText.fontSize = 16;
        buffStatusText.alignment = TextAnchor.MiddleLeft;
        buffStatusText.color = Color.white;
        buffStatusText.raycastTarget = false;

        ResourceUI2D resourceUI = resourcePanel.AddComponent<ResourceUI2D>();
        AssignObjectReference(resourceUI, "playerResources", playerResources);
        AssignObjectReference(resourceUI, "playerShooting", playerShooting);
        AssignObjectReference(resourceUI, "ammoText", ammoText);
        AssignObjectReference(resourceUI, "moneyText", moneyText);

        WeaponUI2D weaponUI = resourcePanel.AddComponent<WeaponUI2D>();
        AssignObjectReference(weaponUI, "playerShooting", playerShooting);
        AssignObjectReference(weaponUI, "weaponText", weaponText);

        BuffStatusUI2D buffStatusUI = resourcePanel.AddComponent<BuffStatusUI2D>();
        AssignObjectReference(buffStatusUI, "playerBuffs", playerBuffs);
        AssignObjectReference(buffStatusUI, "buffStatusText", buffStatusText);

        GameObject bossHealthPanel = CreateRectObject("BossHealthPanel", canvasObject.transform);
        SetRectTransform(bossHealthPanel.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -24f), new Vector2(460f, 66f));

        Image bossPanelBackground = bossHealthPanel.AddComponent<Image>();
        bossPanelBackground.color = new Color(0f, 0f, 0f, 0.72f);

        GameObject bossNameTextObject = CreateRectObject("BossNameText", bossHealthPanel.transform);
        SetRectTransform(bossNameTextObject.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -6f), new Vector2(-16f, 24f));

        Text bossNameText = bossNameTextObject.AddComponent<Text>();
        bossNameText.font = GetBuiltinUIFont();
        bossNameText.text = "Experiment-01";
        bossNameText.fontSize = 18;
        bossNameText.alignment = TextAnchor.MiddleCenter;
        bossNameText.color = Color.white;
        bossNameText.raycastTarget = false;

        GameObject bossHealthBackground = CreateRectObject("BossHealthBackground", bossHealthPanel.transform);
        SetRectTransform(bossHealthBackground.GetComponent<RectTransform>(), new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 6f), new Vector2(-20f, 26f));

        Image bossHealthBackgroundImage = bossHealthBackground.AddComponent<Image>();
        bossHealthBackgroundImage.color = new Color(0.12f, 0.02f, 0.03f, 0.95f);

        GameObject bossHealthFillObject = CreateRectObject("BossHealthFill", bossHealthBackground.transform);
        StretchRectTransform(bossHealthFillObject.GetComponent<RectTransform>(), new Vector2(3f, 3f), new Vector2(-3f, -3f));

        Image bossHealthFill = bossHealthFillObject.AddComponent<Image>();
        bossHealthFill.color = new Color(0.9f, 0.14f, 0.18f, 0.95f);
        bossHealthFill.type = Image.Type.Filled;
        bossHealthFill.fillMethod = Image.FillMethod.Horizontal;
        bossHealthFill.fillOrigin = (int)Image.OriginHorizontal.Left;
        bossHealthFill.fillAmount = 1f;

        GameObject bossHealthTextObject = CreateRectObject("BossHealthText", bossHealthBackground.transform);
        StretchRectTransform(bossHealthTextObject.GetComponent<RectTransform>(), Vector2.zero, Vector2.zero);

        Text bossHealthText = bossHealthTextObject.AddComponent<Text>();
        bossHealthText.font = GetBuiltinUIFont();
        bossHealthText.text = "HP: 50 / 50";
        bossHealthText.fontSize = 15;
        bossHealthText.alignment = TextAnchor.MiddleCenter;
        bossHealthText.color = Color.white;
        bossHealthText.raycastTarget = false;

        BossHealthUI2D bossHealthUI = bossHealthPanel.AddComponent<BossHealthUI2D>();
        AssignObjectReference(bossHealthUI, "bossPanel", bossHealthPanel);
        AssignObjectReference(bossHealthUI, "healthFill", bossHealthFill);
        AssignObjectReference(bossHealthUI, "healthText", bossHealthText);
        AssignObjectReference(bossHealthUI, "bossNameText", bossNameText);
        bossHealthPanel.SetActive(false);

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

        victoryPanel = CreateRectObject("VictoryPanel", canvasObject.transform);
        SetRectTransform(victoryPanel.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(460f, 180f));

        Image victoryBackground = victoryPanel.AddComponent<Image>();
        victoryBackground.color = new Color(0f, 0f, 0f, 0.84f);

        GameObject victoryTextObject = CreateRectObject("VictoryText", victoryPanel.transform);
        StretchRectTransform(victoryTextObject.GetComponent<RectTransform>(), new Vector2(18f, 18f), new Vector2(-18f, -18f));

        victoryText = victoryTextObject.AddComponent<Text>();
        victoryText.font = GetBuiltinUIFont();
        victoryText.text = "LABORATORY CLEARED\nPress R to restart";
        victoryText.fontSize = 30;
        victoryText.alignment = TextAnchor.MiddleCenter;
        victoryText.color = Color.white;
        victoryText.raycastTarget = false;
        victoryPanel.SetActive(false);

        return gameOverPanel;
    }

    private static GameObject CreateFullLaboratoryUI(
        Transform parent,
        PlayerHealth2D playerHealth,
        PlayerResources2D playerResources,
        PlayerShooting2D playerShooting,
        PlayerBuffs2D playerBuffs,
        out ObjectiveUI2D objectiveUI,
        out ShopUI2D shopUI,
        out GameObject choicePanel,
        out Button[] choiceButtons,
        out Text[] choiceTexts,
        out GameObject victoryPanel,
        out Text victoryText)
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

        GameObject objectivePanel = CreateRectObject("ObjectivePanel", canvasObject.transform);
        SetRectTransform(objectivePanel.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -18f), new Vector2(520f, 72f));

        Image objectivePanelImage = objectivePanel.AddComponent<Image>();
        objectivePanelImage.color = new Color(0f, 0f, 0f, 0.65f);

        GameObject objectiveTextObject = CreateRectObject("ObjectiveText", objectivePanel.transform);
        SetRectTransform(objectiveTextObject.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -8f), new Vector2(-20f, 30f));

        Text objectiveText = objectiveTextObject.AddComponent<Text>();
        objectiveText.font = GetBuiltinUIFont();
        objectiveText.text = "Escape the Laboratory";
        objectiveText.fontSize = 18;
        objectiveText.alignment = TextAnchor.MiddleCenter;
        objectiveText.color = Color.white;
        objectiveText.raycastTarget = false;

        GameObject hintTextObject = CreateRectObject("HintText", objectivePanel.transform);
        SetRectTransform(hintTextObject.GetComponent<RectTransform>(), new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 8f), new Vector2(-20f, 28f));

        Text hintText = hintTextObject.AddComponent<Text>();
        hintText.font = GetBuiltinUIFont();
        hintText.text = "WASD move | Mouse aim | Left click shoot | R reload | E interact";
        hintText.fontSize = 14;
        hintText.alignment = TextAnchor.MiddleCenter;
        hintText.color = new Color(0.85f, 0.95f, 1f, 1f);
        hintText.raycastTarget = false;

        objectiveUI = objectivePanel.AddComponent<ObjectiveUI2D>();
        AssignObjectReference(objectiveUI, "objectiveText", objectiveText);
        AssignObjectReference(objectiveUI, "hintText", hintText);

        GameObject healthPanel = CreateRectObject("HealthPanel", canvasObject.transform);
        SetRectTransform(healthPanel.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(24f, -24f), new Vector2(220f, 42f));

        GameObject healthBackground = CreateRectObject("HealthBackground", healthPanel.transform);
        StretchRectTransform(healthBackground.GetComponent<RectTransform>(), Vector2.zero, Vector2.zero);
        Image healthBackgroundImage = healthBackground.AddComponent<Image>();
        healthBackgroundImage.color = new Color(0f, 0f, 0f, 0.65f);

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
        healthText.text = "HP: 6 / 6";
        healthText.fontSize = 18;
        healthText.alignment = TextAnchor.MiddleCenter;
        healthText.color = Color.white;
        healthText.raycastTarget = false;

        HealthUI2D healthUI = healthPanel.AddComponent<HealthUI2D>();
        AssignObjectReference(healthUI, "playerHealth", playerHealth);
        AssignObjectReference(healthUI, "healthFill", healthFill);
        AssignObjectReference(healthUI, "healthText", healthText);

        GameObject resourcePanel = CreateRectObject("ResourcePanel", canvasObject.transform);
        SetRectTransform(resourcePanel.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(24f, -76f), new Vector2(470f, 120f));

        Image resourcePanelImage = resourcePanel.AddComponent<Image>();
        resourcePanelImage.color = new Color(0f, 0f, 0f, 0.65f);

        GameObject ammoTextObject = CreateRectObject("AmmoText", resourcePanel.transform);
        SetRectTransform(ammoTextObject.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -8f), new Vector2(-16f, 24f));

        Text ammoText = ammoTextObject.AddComponent<Text>();
        ammoText.font = GetBuiltinUIFont();
        ammoText.text = "Ammo: 12 / 12 | Reserve: 90 / 150";
        ammoText.fontSize = 16;
        ammoText.alignment = TextAnchor.MiddleLeft;
        ammoText.color = Color.white;
        ammoText.raycastTarget = false;

        GameObject moneyTextObject = CreateRectObject("MoneyText", resourcePanel.transform);
        SetRectTransform(moneyTextObject.GetComponent<RectTransform>(), new Vector2(0f, 0.67f), new Vector2(1f, 0.67f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(-16f, 24f));

        Text moneyText = moneyTextObject.AddComponent<Text>();
        moneyText.font = GetBuiltinUIFont();
        moneyText.text = "Money: 50";
        moneyText.fontSize = 16;
        moneyText.alignment = TextAnchor.MiddleLeft;
        moneyText.color = Color.white;
        moneyText.raycastTarget = false;

        GameObject weaponTextObject = CreateRectObject("WeaponText", resourcePanel.transform);
        SetRectTransform(weaponTextObject.GetComponent<RectTransform>(), new Vector2(0f, 0.34f), new Vector2(1f, 0.34f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(-16f, 24f));

        Text weaponText = weaponTextObject.AddComponent<Text>();
        weaponText.font = GetBuiltinUIFont();
        weaponText.text = "Weapon: Pistol [Common]";
        weaponText.fontSize = 16;
        weaponText.alignment = TextAnchor.MiddleLeft;
        weaponText.color = Color.white;
        weaponText.raycastTarget = false;

        GameObject buffStatusTextObject = CreateRectObject("BuffStatusText", resourcePanel.transform);
        SetRectTransform(buffStatusTextObject.GetComponent<RectTransform>(), new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 8f), new Vector2(-16f, 24f));

        Text buffStatusText = buffStatusTextObject.AddComponent<Text>();
        buffStatusText.font = GetBuiltinUIFont();
        buffStatusText.text = "Buffs: none";
        buffStatusText.fontSize = 16;
        buffStatusText.alignment = TextAnchor.MiddleLeft;
        buffStatusText.color = Color.white;
        buffStatusText.raycastTarget = false;

        ResourceUI2D resourceUI = resourcePanel.AddComponent<ResourceUI2D>();
        AssignObjectReference(resourceUI, "playerResources", playerResources);
        AssignObjectReference(resourceUI, "playerShooting", playerShooting);
        AssignObjectReference(resourceUI, "ammoText", ammoText);
        AssignObjectReference(resourceUI, "moneyText", moneyText);

        WeaponUI2D weaponUI = resourcePanel.AddComponent<WeaponUI2D>();
        AssignObjectReference(weaponUI, "playerShooting", playerShooting);
        AssignObjectReference(weaponUI, "weaponText", weaponText);

        BuffStatusUI2D buffStatusUI = resourcePanel.AddComponent<BuffStatusUI2D>();
        AssignObjectReference(buffStatusUI, "playerBuffs", playerBuffs);
        AssignObjectReference(buffStatusUI, "buffStatusText", buffStatusText);

        GameObject shopPanel = CreateRectObject("ShopPanel", canvasObject.transform);
        SetRectTransform(shopPanel.GetComponent<RectTransform>(), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 36f), new Vector2(520f, 74f));

        Image shopPanelImage = shopPanel.AddComponent<Image>();
        shopPanelImage.color = new Color(0f, 0f, 0f, 0.78f);

        GameObject shopPromptTextObject = CreateRectObject("ShopPromptText", shopPanel.transform);
        SetRectTransform(shopPromptTextObject.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -8f), new Vector2(-24f, 28f));

        Text shopPromptText = shopPromptTextObject.AddComponent<Text>();
        shopPromptText.font = GetBuiltinUIFont();
        shopPromptText.text = string.Empty;
        shopPromptText.fontSize = 17;
        shopPromptText.alignment = TextAnchor.MiddleCenter;
        shopPromptText.color = Color.white;
        shopPromptText.raycastTarget = false;

        GameObject shopMessageTextObject = CreateRectObject("ShopMessageText", shopPanel.transform);
        SetRectTransform(shopMessageTextObject.GetComponent<RectTransform>(), new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 8f), new Vector2(-24f, 28f));

        Text shopMessageText = shopMessageTextObject.AddComponent<Text>();
        shopMessageText.font = GetBuiltinUIFont();
        shopMessageText.text = string.Empty;
        shopMessageText.fontSize = 17;
        shopMessageText.alignment = TextAnchor.MiddleCenter;
        shopMessageText.color = new Color(1f, 0.86f, 0.2f, 1f);
        shopMessageText.raycastTarget = false;

        shopUI = canvasObject.AddComponent<ShopUI2D>();
        AssignObjectReference(shopUI, "shopPanel", shopPanel);
        AssignObjectReference(shopUI, "promptText", shopPromptText);
        AssignObjectReference(shopUI, "messageText", shopMessageText);
        AssignFloat(shopUI, "messageDuration", 2f);
        shopPanel.SetActive(false);

        choicePanel = CreateRectObject("BuffChoicePanel", canvasObject.transform);
        SetRectTransform(choicePanel.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(620f, 330f));

        Image choicePanelImage = choicePanel.AddComponent<Image>();
        choicePanelImage.color = new Color(0f, 0f, 0f, 0.86f);

        GameObject buffTitleObject = CreateRectObject("BuffTitleText", choicePanel.transform);
        SetRectTransform(buffTitleObject.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -18f), new Vector2(-32f, 42f));

        Text buffTitleText = buffTitleObject.AddComponent<Text>();
        buffTitleText.font = GetBuiltinUIFont();
        buffTitleText.text = "Choose a Buff";
        buffTitleText.fontSize = 26;
        buffTitleText.alignment = TextAnchor.MiddleCenter;
        buffTitleText.color = Color.white;
        buffTitleText.raycastTarget = false;

        choiceButtons = new Button[3];
        choiceTexts = new Text[3];
        choiceButtons[0] = CreateBuffChoiceButton(choicePanel.transform, "BuffButton_01", new Vector2(0f, 62f), out choiceTexts[0]);
        choiceButtons[1] = CreateBuffChoiceButton(choicePanel.transform, "BuffButton_02", new Vector2(0f, -28f), out choiceTexts[1]);
        choiceButtons[2] = CreateBuffChoiceButton(choicePanel.transform, "BuffButton_03", new Vector2(0f, -118f), out choiceTexts[2]);
        choicePanel.SetActive(false);

        GameObject bossHealthPanel = CreateRectObject("BossHealthPanel", canvasObject.transform);
        SetRectTransform(bossHealthPanel.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -96f), new Vector2(460f, 66f));

        Image bossPanelBackground = bossHealthPanel.AddComponent<Image>();
        bossPanelBackground.color = new Color(0f, 0f, 0f, 0.72f);

        GameObject bossNameTextObject = CreateRectObject("BossNameText", bossHealthPanel.transform);
        SetRectTransform(bossNameTextObject.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -6f), new Vector2(-16f, 24f));

        Text bossNameText = bossNameTextObject.AddComponent<Text>();
        bossNameText.font = GetBuiltinUIFont();
        bossNameText.text = "Experiment-01";
        bossNameText.fontSize = 18;
        bossNameText.alignment = TextAnchor.MiddleCenter;
        bossNameText.color = Color.white;
        bossNameText.raycastTarget = false;

        GameObject bossHealthBackground = CreateRectObject("BossHealthBackground", bossHealthPanel.transform);
        SetRectTransform(bossHealthBackground.GetComponent<RectTransform>(), new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 6f), new Vector2(-20f, 26f));

        Image bossHealthBackgroundImage = bossHealthBackground.AddComponent<Image>();
        bossHealthBackgroundImage.color = new Color(0.12f, 0.02f, 0.03f, 0.95f);

        GameObject bossHealthFillObject = CreateRectObject("BossHealthFill", bossHealthBackground.transform);
        StretchRectTransform(bossHealthFillObject.GetComponent<RectTransform>(), new Vector2(3f, 3f), new Vector2(-3f, -3f));

        Image bossHealthFill = bossHealthFillObject.AddComponent<Image>();
        bossHealthFill.color = new Color(0.9f, 0.14f, 0.18f, 0.95f);
        bossHealthFill.type = Image.Type.Filled;
        bossHealthFill.fillMethod = Image.FillMethod.Horizontal;
        bossHealthFill.fillOrigin = (int)Image.OriginHorizontal.Left;
        bossHealthFill.fillAmount = 1f;

        GameObject bossHealthTextObject = CreateRectObject("BossHealthText", bossHealthBackground.transform);
        StretchRectTransform(bossHealthTextObject.GetComponent<RectTransform>(), Vector2.zero, Vector2.zero);

        Text bossHealthText = bossHealthTextObject.AddComponent<Text>();
        bossHealthText.font = GetBuiltinUIFont();
        bossHealthText.text = "HP: 50 / 50";
        bossHealthText.fontSize = 15;
        bossHealthText.alignment = TextAnchor.MiddleCenter;
        bossHealthText.color = Color.white;
        bossHealthText.raycastTarget = false;

        BossHealthUI2D bossHealthUI = bossHealthPanel.AddComponent<BossHealthUI2D>();
        AssignObjectReference(bossHealthUI, "bossPanel", bossHealthPanel);
        AssignObjectReference(bossHealthUI, "healthFill", bossHealthFill);
        AssignObjectReference(bossHealthUI, "healthText", bossHealthText);
        AssignObjectReference(bossHealthUI, "bossNameText", bossNameText);
        bossHealthPanel.SetActive(false);

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

        victoryPanel = CreateRectObject("VictoryPanel", canvasObject.transform);
        SetRectTransform(victoryPanel.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(460f, 180f));

        Image victoryBackground = victoryPanel.AddComponent<Image>();
        victoryBackground.color = new Color(0f, 0f, 0f, 0.84f);

        GameObject victoryTextObject = CreateRectObject("VictoryText", victoryPanel.transform);
        StretchRectTransform(victoryTextObject.GetComponent<RectTransform>(), new Vector2(18f, 18f), new Vector2(-18f, -18f));

        victoryText = victoryTextObject.AddComponent<Text>();
        victoryText.font = GetBuiltinUIFont();
        victoryText.text = "LABORATORY CLEARED\nPress R to restart";
        victoryText.fontSize = 30;
        victoryText.alignment = TextAnchor.MiddleCenter;
        victoryText.color = Color.white;
        victoryText.raycastTarget = false;
        victoryPanel.SetActive(false);

        return gameOverPanel;
    }

    private static void AddPrisonKeycardUI(Transform root, PlayerKeyring2D playerKeyring)
    {
        Transform resourcePanelTransform = root.Find("UI/Canvas/ResourcePanel");

        if (resourcePanelTransform == null)
        {
            Debug.LogWarning("Could not add keycard UI because ResourcePanel was not found.");
            return;
        }

        RectTransform resourcePanelRect = resourcePanelTransform.GetComponent<RectTransform>();

        if (resourcePanelRect != null)
        {
            resourcePanelRect.sizeDelta = new Vector2(470f, 148f);
        }

        RepositionResourceText(resourcePanelTransform, "AmmoText", new Vector2(0f, -8f), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f));
        RepositionResourceText(resourcePanelTransform, "MoneyText", Vector2.zero, new Vector2(0f, 0.72f), new Vector2(1f, 0.72f), new Vector2(0.5f, 0.5f));
        RepositionResourceText(resourcePanelTransform, "WeaponText", Vector2.zero, new Vector2(0f, 0.47f), new Vector2(1f, 0.47f), new Vector2(0.5f, 0.5f));
        RepositionResourceText(resourcePanelTransform, "BuffStatusText", Vector2.zero, new Vector2(0f, 0.23f), new Vector2(1f, 0.23f), new Vector2(0.5f, 0.5f));

        GameObject keycardTextObject = CreateRectObject("KeycardText", resourcePanelTransform);
        SetRectTransform(keycardTextObject.GetComponent<RectTransform>(), new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 8f), new Vector2(-16f, 24f));

        Text keycardText = keycardTextObject.AddComponent<Text>();
        keycardText.font = GetBuiltinUIFont();
        keycardText.text = "Keycards: 0";
        keycardText.fontSize = 16;
        keycardText.alignment = TextAnchor.MiddleLeft;
        keycardText.color = Color.white;
        keycardText.raycastTarget = false;

        KeycardUI2D keycardUI = resourcePanelTransform.gameObject.AddComponent<KeycardUI2D>();
        AssignObjectReference(keycardUI, "playerKeyring", playerKeyring);
        AssignObjectReference(keycardUI, "keycardText", keycardText);
    }

    private static void RepositionResourceText(Transform resourcePanelTransform, string childName, Vector2 anchoredPosition, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot)
    {
        Transform child = resourcePanelTransform.Find(childName);

        if (child == null)
        {
            return;
        }

        RectTransform rectTransform = child.GetComponent<RectTransform>();

        if (rectTransform != null)
        {
            SetRectTransform(rectTransform, anchorMin, anchorMax, pivot, anchoredPosition, new Vector2(-16f, 24f));
        }
    }

    private static void AddFinalDemoUiOverlays(Transform root, out GameObject pausePanel, out DemoMessageUI2D demoMessageUI)
    {
        Transform canvasTransform = root.Find("UI/Canvas");

        if (canvasTransform == null)
        {
            pausePanel = null;
            demoMessageUI = null;
            return;
        }

        pausePanel = CreateRectObject("PausePanel", canvasTransform);
        SetRectTransform(pausePanel.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(420f, 270f));

        Image pauseBackground = pausePanel.AddComponent<Image>();
        pauseBackground.color = new Color(0f, 0f, 0f, 0.86f);

        GameObject titleObject = CreateRectObject("PauseTitleText", pausePanel.transform);
        SetRectTransform(titleObject.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -22f), new Vector2(-24f, 48f));
        Text titleText = titleObject.AddComponent<Text>();
        titleText.font = GetBuiltinUIFont();
        titleText.text = "Paused";
        titleText.fontSize = 30;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = Color.white;
        titleText.raycastTarget = false;

        CreateMenuButton(pausePanel.transform, "ResumeButton", "Resume", new Vector2(0f, 38f));
        CreateMenuButton(pausePanel.transform, "RestartButton", "Restart", new Vector2(0f, -32f));
        CreateMenuButton(pausePanel.transform, "MainMenuButton", "Main Menu", new Vector2(0f, -102f));
        pausePanel.SetActive(false);

        GameObject messageObject = CreateRectObject("DemoMessageText", canvasTransform);
        SetRectTransform(messageObject.GetComponent<RectTransform>(), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 116f), new Vector2(640f, 34f));

        Text messageText = messageObject.AddComponent<Text>();
        messageText.font = GetBuiltinUIFont();
        messageText.text = string.Empty;
        messageText.fontSize = 18;
        messageText.alignment = TextAnchor.MiddleCenter;
        messageText.color = new Color(0.9f, 0.96f, 1f, 1f);
        messageText.raycastTarget = false;

        demoMessageUI = messageObject.AddComponent<DemoMessageUI2D>();
        AssignObjectReference(demoMessageUI, "messageText", messageText);
        AssignFloat(demoMessageUI, "defaultDuration", 2f);
    }

    private static Button CreateBuffChoiceButton(Transform parent, string name, Vector2 anchoredPosition, out Text buttonText)
    {
        GameObject buttonObject = CreateRectObject(name, parent);
        SetRectTransform(buttonObject.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), anchoredPosition, new Vector2(540f, 74f));

        Image buttonImage = buttonObject.AddComponent<Image>();
        buttonImage.color = new Color(0.16f, 0.18f, 0.22f, 1f);

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = buttonImage;

        GameObject textObject = CreateRectObject("Text", buttonObject.transform);
        StretchRectTransform(textObject.GetComponent<RectTransform>(), new Vector2(12f, 8f), new Vector2(-12f, -8f));

        buttonText = textObject.AddComponent<Text>();
        buttonText.font = GetBuiltinUIFont();
        buttonText.text = "Buff";
        buttonText.fontSize = 17;
        buttonText.alignment = TextAnchor.MiddleCenter;
        buttonText.color = Color.white;
        buttonText.raycastTarget = false;

        return button;
    }

    private static BuffChoiceController2D CreateMinibossGameSystems(
        Transform parent,
        PlayerHealth2D playerHealth,
        GameObject gameOverPanel,
        PlayerBuffs2D playerBuffs,
        BuffDefinition2D[] buffPool,
        GameObject choicePanel,
        Button[] choiceButtons,
        Text[] choiceTexts)
    {
        GameObject gameSystems = new GameObject("GameSystems");
        gameSystems.transform.SetParent(parent);

        GameObject gameOverControllerObject = new GameObject("GameOverController");
        gameOverControllerObject.transform.SetParent(gameSystems.transform);

        GameOverController2D gameOverController = gameOverControllerObject.AddComponent<GameOverController2D>();
        AssignObjectReference(gameOverController, "playerHealth", playerHealth);
        AssignObjectReference(gameOverController, "gameOverPanel", gameOverPanel);
        AssignString(gameOverController, "restartKey", "r");

        GameObject buffChoiceControllerObject = new GameObject("BuffChoiceController");
        buffChoiceControllerObject.transform.SetParent(gameSystems.transform);

        BuffChoiceController2D buffChoiceController = buffChoiceControllerObject.AddComponent<BuffChoiceController2D>();
        AssignObjectReference(buffChoiceController, "playerBuffs", playerBuffs);
        AssignObjectReferenceArray(buffChoiceController, "buffPool", buffPool);
        AssignObjectReference(buffChoiceController, "choicePanel", choicePanel);
        AssignObjectReferenceArray(buffChoiceController, "choiceButtons", choiceButtons);
        AssignObjectReferenceArray(buffChoiceController, "choiceTexts", choiceTexts);
        AssignBool(buffChoiceController, "pauseGameWhileChoosing", true);
        AssignInt(buffChoiceController, "choicesToShow", 3);

        return buffChoiceController;
    }

    private static void CreateEventSystem(Transform parent)
    {
        if (UnityEngine.Object.FindObjectOfType<EventSystem>() != null)
        {
            return;
        }

        GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        eventSystem.transform.SetParent(parent);
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

    private static LevelEndController2D CreateBossFightGameSystems(
        Transform parent,
        PlayerHealth2D playerHealth,
        GameObject gameOverPanel,
        GameObject victoryPanel,
        Text victoryText)
    {
        GameObject gameSystems = new GameObject("GameSystems");
        gameSystems.transform.SetParent(parent);

        GameObject gameOverControllerObject = new GameObject("GameOverController");
        gameOverControllerObject.transform.SetParent(gameSystems.transform);

        GameOverController2D gameOverController = gameOverControllerObject.AddComponent<GameOverController2D>();
        AssignObjectReference(gameOverController, "playerHealth", playerHealth);
        AssignObjectReference(gameOverController, "gameOverPanel", gameOverPanel);
        AssignString(gameOverController, "restartKey", "r");

        GameObject levelEndControllerObject = new GameObject("LevelEndController");
        levelEndControllerObject.transform.SetParent(gameSystems.transform);

        LevelEndController2D levelEndController = levelEndControllerObject.AddComponent<LevelEndController2D>();
        AssignObjectReference(levelEndController, "victoryPanel", victoryPanel);
        AssignObjectReference(levelEndController, "victoryText", victoryText);
        AssignString(levelEndController, "restartKey", "r");
        AssignBool(levelEndController, "pauseOnVictory", true);

        return levelEndController;
    }

    private static void CreateFullLaboratoryGameSystems(
        Transform parent,
        PlayerHealth2D playerHealth,
        GameObject gameOverPanel,
        PlayerBuffs2D playerBuffs,
        BuffDefinition2D[] buffPool,
        GameObject choicePanel,
        Button[] choiceButtons,
        Text[] choiceTexts,
        ObjectiveUI2D objectiveUI,
        GameObject victoryPanel,
        Text victoryText,
        out BuffChoiceController2D buffChoiceController,
        out LevelEndController2D levelEndController)
    {
        GameObject gameSystems = new GameObject("GameSystems");
        gameSystems.transform.SetParent(parent);

        GameObject gameOverControllerObject = new GameObject("GameOverController");
        gameOverControllerObject.transform.SetParent(gameSystems.transform);

        GameOverController2D gameOverController = gameOverControllerObject.AddComponent<GameOverController2D>();
        AssignObjectReference(gameOverController, "playerHealth", playerHealth);
        AssignObjectReference(gameOverController, "gameOverPanel", gameOverPanel);
        AssignString(gameOverController, "restartKey", "r");

        GameObject levelEndControllerObject = new GameObject("LevelEndController");
        levelEndControllerObject.transform.SetParent(gameSystems.transform);

        levelEndController = levelEndControllerObject.AddComponent<LevelEndController2D>();
        AssignObjectReference(levelEndController, "victoryPanel", victoryPanel);
        AssignObjectReference(levelEndController, "victoryText", victoryText);
        AssignString(levelEndController, "restartKey", "r");
        AssignBool(levelEndController, "pauseOnVictory", true);

        GameObject levelFlowControllerObject = new GameObject("LevelFlowController");
        levelFlowControllerObject.transform.SetParent(gameSystems.transform);

        LevelFlowController2D levelFlowController = levelFlowControllerObject.AddComponent<LevelFlowController2D>();
        AssignObjectReference(levelFlowController, "objectiveUI", objectiveUI);
        AssignObjectReference(levelFlowController, "levelEndController", levelEndController);
        AssignString(levelFlowController, "levelName", "Laboratory");
        AssignString(levelFlowController, "startingObjective", "Escape the Laboratory");
        AssignString(levelFlowController, "controlsHint", "WASD move | Mouse aim | Left click shoot | R reload | E interact");

        GameObject buffChoiceControllerObject = new GameObject("BuffChoiceController");
        buffChoiceControllerObject.transform.SetParent(gameSystems.transform);

        buffChoiceController = buffChoiceControllerObject.AddComponent<BuffChoiceController2D>();
        AssignObjectReference(buffChoiceController, "playerBuffs", playerBuffs);
        AssignObjectReferenceArray(buffChoiceController, "buffPool", buffPool);
        AssignObjectReference(buffChoiceController, "choicePanel", choicePanel);
        AssignObjectReferenceArray(buffChoiceController, "choiceButtons", choiceButtons);
        AssignObjectReferenceArray(buffChoiceController, "choiceTexts", choiceTexts);
        AssignBool(buffChoiceController, "pauseGameWhileChoosing", true);
        AssignInt(buffChoiceController, "choicesToShow", 3);
    }

    private static void CreateFinalDemoGameSystems(
        Transform parent,
        PlayerHealth2D playerHealth,
        GameObject gameOverPanel,
        PlayerBuffs2D playerBuffs,
        BuffDefinition2D[] buffPool,
        GameObject choicePanel,
        Button[] choiceButtons,
        Text[] choiceTexts,
        ObjectiveUI2D objectiveUI,
        GameObject victoryPanel,
        Text victoryText,
        GameObject pausePanel,
        DemoMessageUI2D demoMessageUI,
        out BuffChoiceController2D buffChoiceController,
        out LevelEndController2D levelEndController,
        string levelName = "Prison Escape",
        string startingObjective = "Escape the Prison",
        string controlsHint = "WASD move | Mouse aim | Left click shoot | R reload | E interact | Esc pause",
        string victoryMessage = "PRISON ESCAPED")
    {
        GameObject gameSystems = new GameObject("GameSystems");
        gameSystems.transform.SetParent(parent);

        GameObject gameOverControllerObject = new GameObject("GameOverController");
        gameOverControllerObject.transform.SetParent(gameSystems.transform);

        GameOverController2D gameOverController = gameOverControllerObject.AddComponent<GameOverController2D>();
        AssignObjectReference(gameOverController, "playerHealth", playerHealth);
        AssignObjectReference(gameOverController, "gameOverPanel", gameOverPanel);
        AssignString(gameOverController, "restartKey", "r");

        GameObject levelEndControllerObject = new GameObject("LevelEndController");
        levelEndControllerObject.transform.SetParent(gameSystems.transform);

        levelEndController = levelEndControllerObject.AddComponent<LevelEndController2D>();
        AssignObjectReference(levelEndController, "victoryPanel", victoryPanel);
        AssignObjectReference(levelEndController, "victoryText", victoryText);
        AssignString(levelEndController, "restartKey", "r");
        AssignBool(levelEndController, "pauseOnVictory", true);

        GameObject levelFlowControllerObject = new GameObject("LevelFlowController");
        levelFlowControllerObject.transform.SetParent(gameSystems.transform);

        LevelFlowController2D levelFlowController = levelFlowControllerObject.AddComponent<LevelFlowController2D>();
        AssignObjectReference(levelFlowController, "objectiveUI", objectiveUI);
        AssignObjectReference(levelFlowController, "levelEndController", levelEndController);
        AssignString(levelFlowController, "levelName", "Laboratory");
        AssignString(levelFlowController, "startingObjective", "Escape the Laboratory");
        AssignString(levelFlowController, "controlsHint", "WASD move | Mouse aim | Left click shoot | R reload | E interact");

        GameObject buffChoiceControllerObject = new GameObject("BuffChoiceController");
        buffChoiceControllerObject.transform.SetParent(gameSystems.transform);

        buffChoiceController = buffChoiceControllerObject.AddComponent<BuffChoiceController2D>();
        AssignObjectReference(buffChoiceController, "playerBuffs", playerBuffs);
        AssignObjectReferenceArray(buffChoiceController, "buffPool", buffPool);
        AssignObjectReference(buffChoiceController, "choicePanel", choicePanel);
        AssignObjectReferenceArray(buffChoiceController, "choiceButtons", choiceButtons);
        AssignObjectReferenceArray(buffChoiceController, "choiceTexts", choiceTexts);
        AssignBool(buffChoiceController, "pauseGameWhileChoosing", true);
        AssignInt(buffChoiceController, "choicesToShow", 3);

        GameObject pauseControllerObject = new GameObject("PauseMenuController");
        pauseControllerObject.transform.SetParent(gameSystems.transform);

        PauseMenuController2D pauseController = pauseControllerObject.AddComponent<PauseMenuController2D>();
        AssignObjectReference(pauseController, "pausePanel", pausePanel);
        AssignString(pauseController, "mainMenuSceneName", "MainMenu");
        AssignEnumByName(pauseController, "pauseKey", nameof(KeyCode.Escape));
        WirePauseButton(pausePanel, "ResumeButton", pauseController.Resume);
        WirePauseButton(pausePanel, "RestartButton", pauseController.RestartScene);
        WirePauseButton(pausePanel, "MainMenuButton", pauseController.ReturnToMainMenu);

        GameObject audioObject = new GameObject("DemoAudioManager");
        audioObject.transform.SetParent(gameSystems.transform);

        AudioSource audioSource = audioObject.AddComponent<AudioSource>();
        DemoAudioManager2D audioManager = audioObject.AddComponent<DemoAudioManager2D>();
        AssignObjectReference(audioManager, "sfxSource", audioSource);
        AssignObjectReference(audioManager, "shootClip", AssetDatabase.LoadAssetAtPath<AudioClip>(ShootAudioPath));
        AssignObjectReference(audioManager, "hitClip", AssetDatabase.LoadAssetAtPath<AudioClip>(HitAudioPath));
        AssignObjectReference(audioManager, "pickupClip", AssetDatabase.LoadAssetAtPath<AudioClip>(PickupAudioPath));
        AssignObjectReference(audioManager, "doorClip", AssetDatabase.LoadAssetAtPath<AudioClip>(DoorAudioPath));
        AssignObjectReference(audioManager, "shopClip", AssetDatabase.LoadAssetAtPath<AudioClip>(ShopAudioPath));
        AssignObjectReference(audioManager, "buffClip", AssetDatabase.LoadAssetAtPath<AudioClip>(BuffAudioPath));
        AssignObjectReference(audioManager, "bossPhaseClip", AssetDatabase.LoadAssetAtPath<AudioClip>(BossPhaseAudioPath));
        AssignObjectReference(audioManager, "victoryClip", AssetDatabase.LoadAssetAtPath<AudioClip>(VictoryAudioPath));
        AssignObjectReference(audioManager, "gameOverClip", AssetDatabase.LoadAssetAtPath<AudioClip>(GameOverAudioPath));
        AssignFloat(audioManager, "volume", 0.6f);

        _ = demoMessageUI;
    }

    private static void CreatePrisonGameSystems(
        Transform parent,
        PlayerHealth2D playerHealth,
        GameObject gameOverPanel,
        PlayerBuffs2D playerBuffs,
        BuffDefinition2D[] buffPool,
        GameObject choicePanel,
        Button[] choiceButtons,
        Text[] choiceTexts,
        ObjectiveUI2D objectiveUI,
        GameObject victoryPanel,
        Text victoryText,
        GameObject pausePanel,
        DemoMessageUI2D demoMessageUI,
        out BuffChoiceController2D buffChoiceController,
        out LevelEndController2D levelEndController,
        string levelName = "Prison Escape",
        string startingObjective = "Escape the Prison",
        string controlsHint = "WASD move | Mouse aim | Left click shoot | R reload | E interact | Esc pause",
        string victoryMessage = "PRISON ESCAPED")
    {
        GameObject gameSystems = new GameObject("GameSystems");
        gameSystems.transform.SetParent(parent);

        GameObject gameOverControllerObject = new GameObject("GameOverController");
        gameOverControllerObject.transform.SetParent(gameSystems.transform);

        GameOverController2D gameOverController = gameOverControllerObject.AddComponent<GameOverController2D>();
        AssignObjectReference(gameOverController, "playerHealth", playerHealth);
        AssignObjectReference(gameOverController, "gameOverPanel", gameOverPanel);
        AssignString(gameOverController, "restartKey", "r");

        GameObject levelEndControllerObject = new GameObject("LevelEndController");
        levelEndControllerObject.transform.SetParent(gameSystems.transform);

        levelEndController = levelEndControllerObject.AddComponent<LevelEndController2D>();
        AssignObjectReference(levelEndController, "victoryPanel", victoryPanel);
        AssignObjectReference(levelEndController, "victoryText", victoryText);
        AssignString(levelEndController, "restartKey", "r");
        AssignBool(levelEndController, "pauseOnVictory", true);

        if (victoryText != null)
        {
            victoryText.text = $"{victoryMessage}\nPress R to restart";
        }

        GameObject levelFlowControllerObject = new GameObject("LevelFlowController");
        levelFlowControllerObject.transform.SetParent(gameSystems.transform);

        LevelFlowController2D levelFlowController = levelFlowControllerObject.AddComponent<LevelFlowController2D>();
        AssignObjectReference(levelFlowController, "objectiveUI", objectiveUI);
        AssignObjectReference(levelFlowController, "levelEndController", levelEndController);
        AssignString(levelFlowController, "levelName", levelName);
        AssignString(levelFlowController, "startingObjective", startingObjective);
        AssignString(levelFlowController, "controlsHint", controlsHint);

        GameObject buffChoiceControllerObject = new GameObject("BuffChoiceController");
        buffChoiceControllerObject.transform.SetParent(gameSystems.transform);

        buffChoiceController = buffChoiceControllerObject.AddComponent<BuffChoiceController2D>();
        AssignObjectReference(buffChoiceController, "playerBuffs", playerBuffs);
        AssignObjectReferenceArray(buffChoiceController, "buffPool", buffPool);
        AssignObjectReference(buffChoiceController, "choicePanel", choicePanel);
        AssignObjectReferenceArray(buffChoiceController, "choiceButtons", choiceButtons);
        AssignObjectReferenceArray(buffChoiceController, "choiceTexts", choiceTexts);
        AssignBool(buffChoiceController, "pauseGameWhileChoosing", true);
        AssignInt(buffChoiceController, "choicesToShow", 3);

        GameObject pauseControllerObject = new GameObject("PauseMenuController");
        pauseControllerObject.transform.SetParent(gameSystems.transform);

        PauseMenuController2D pauseController = pauseControllerObject.AddComponent<PauseMenuController2D>();
        AssignObjectReference(pauseController, "pausePanel", pausePanel);
        AssignString(pauseController, "mainMenuSceneName", "MainMenu");
        AssignEnumByName(pauseController, "pauseKey", nameof(KeyCode.Escape));
        WirePauseButton(pausePanel, "ResumeButton", pauseController.Resume);
        WirePauseButton(pausePanel, "RestartButton", pauseController.RestartScene);
        WirePauseButton(pausePanel, "MainMenuButton", pauseController.ReturnToMainMenu);

        GameObject audioObject = new GameObject("DemoAudioManager");
        audioObject.transform.SetParent(gameSystems.transform);

        AudioSource audioSource = audioObject.AddComponent<AudioSource>();
        DemoAudioManager2D audioManager = audioObject.AddComponent<DemoAudioManager2D>();
        AssignObjectReference(audioManager, "sfxSource", audioSource);
        AssignObjectReference(audioManager, "shootClip", AssetDatabase.LoadAssetAtPath<AudioClip>(ShootAudioPath));
        AssignObjectReference(audioManager, "hitClip", AssetDatabase.LoadAssetAtPath<AudioClip>(HitAudioPath));
        AssignObjectReference(audioManager, "pickupClip", AssetDatabase.LoadAssetAtPath<AudioClip>(PickupAudioPath));
        AssignObjectReference(audioManager, "doorClip", AssetDatabase.LoadAssetAtPath<AudioClip>(DoorAudioPath));
        AssignObjectReference(audioManager, "shopClip", AssetDatabase.LoadAssetAtPath<AudioClip>(ShopAudioPath));
        AssignObjectReference(audioManager, "buffClip", AssetDatabase.LoadAssetAtPath<AudioClip>(BuffAudioPath));
        AssignObjectReference(audioManager, "bossPhaseClip", AssetDatabase.LoadAssetAtPath<AudioClip>(BossPhaseAudioPath));
        AssignObjectReference(audioManager, "victoryClip", AssetDatabase.LoadAssetAtPath<AudioClip>(VictoryAudioPath));
        AssignObjectReference(audioManager, "gameOverClip", AssetDatabase.LoadAssetAtPath<AudioClip>(GameOverAudioPath));
        AssignObjectReference(audioManager, "laserClip", AssetDatabase.LoadAssetAtPath<AudioClip>(LaserAudioPath));
        AssignObjectReference(audioManager, "keycardClip", AssetDatabase.LoadAssetAtPath<AudioClip>(KeycardAudioPath));
        AssignFloat(audioManager, "volume", 0.6f);

        GameObject demoMessageObject = new GameObject("DemoMessageUI");
        demoMessageObject.transform.SetParent(gameSystems.transform);
        _ = demoMessageObject;
        _ = demoMessageUI;
    }

    private static void WirePauseButton(GameObject pausePanel, string buttonName, UnityEngine.Events.UnityAction action)
    {
        if (pausePanel == null || action == null)
        {
            return;
        }

        Transform buttonTransform = pausePanel.transform.Find(buttonName);

        if (buttonTransform == null)
        {
            return;
        }

        Button button = buttonTransform.GetComponent<Button>();

        if (button != null)
        {
            UnityEventTools.AddPersistentListener(button.onClick, action);
        }
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

    private static Material GetOrCreateMaterial(string assetPath, string shaderName, Color color)
    {
        Material material = AssetDatabase.LoadAssetAtPath<Material>(assetPath);

        if (material == null)
        {
            Shader shader = Shader.Find(shaderName);
            material = new Material(shader != null ? shader : Shader.Find("Sprites/Default"));
            AssetDatabase.CreateAsset(material, assetPath);
        }

        material.color = color;
        EditorUtility.SetDirty(material);
        AssetDatabase.SaveAssets();
        return material;
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

    private static Color CreateMinibossPixel(int x, int y, int size)
    {
        Color fill = new Color(0.58f, 0.08f, 0.78f, 1f);
        Color outline = new Color(0.12f, 0.01f, 0.2f, 1f);
        Color basePixel = CirclePixel(x, y, size, fill, outline);

        if (basePixel.a <= 0f)
        {
            return basePixel;
        }

        float center = size * 0.5f;
        bool core = Mathf.Abs(x + 0.5f - center) < size * 0.12f && Mathf.Abs(y + 0.5f - center) < size * 0.26f;
        bool horns = (y > size * 0.62f) && (Mathf.Abs(x + 0.5f - center) > size * 0.22f) && (Mathf.Abs(x + 0.5f - center) < size * 0.36f);

        if (core)
        {
            return new Color(1f, 0.45f, 0.95f, 1f);
        }

        return horns ? new Color(0.9f, 0.9f, 1f, 1f) : basePixel;
    }

    private static Color CreateBossPixel(int x, int y, int size)
    {
        Color fill = new Color(0.58f, 0.1f, 0.14f, 1f);
        Color outline = new Color(0.08f, 0.02f, 0.03f, 1f);
        Color basePixel = CirclePixel(x, y, size, fill, outline);

        if (basePixel.a <= 0f)
        {
            return basePixel;
        }

        float center = size * 0.5f;
        float dx = Mathf.Abs(x + 0.5f - center);
        float dy = Mathf.Abs(y + 0.5f - center);
        bool core = dx < size * 0.12f && dy < size * 0.3f;
        bool vialBand = dy < size * 0.08f && dx < size * 0.32f;
        bool eye = y > size * 0.54f && dx > size * 0.12f && dx < size * 0.28f;

        if (core || vialBand)
        {
            return new Color(0.2f, 1f, 0.82f, 1f);
        }

        return eye ? new Color(1f, 0.85f, 0.2f, 1f) : basePixel;
    }

    private static Color CreateBulletPixel(int x, int y, int size)
    {
        Color fill = new Color(1f, 0.86f, 0.12f, 1f);
        Color outline = new Color(0.85f, 0.42f, 0.04f, 1f);
        return CirclePixel(x, y, size, fill, outline);
    }

    private static Color CreateEnemyProjectilePixel(int x, int y, int size)
    {
        Color fill = new Color(0.2f, 0.95f, 0.78f, 1f);
        Color outline = new Color(0.02f, 0.3f, 0.26f, 1f);
        Color basePixel = CirclePixel(x, y, size, fill, outline);

        if (basePixel.a <= 0f)
        {
            return basePixel;
        }

        float center = size * 0.5f;
        bool spark = Mathf.Abs(x + 0.5f - center) < size * 0.08f || Mathf.Abs(y + 0.5f - center) < size * 0.08f;
        return spark ? Color.white : basePixel;
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

    private static Color CreateBossArenaWallPixel(int x, int y, int size)
    {
        bool border = x < 4 || y < 4 || x >= size - 4 || y >= size - 4;
        bool stripe = (x * 2 + y) % 18 < 9;

        if (border)
        {
            return new Color(0.04f, 0.08f, 0.1f, 1f);
        }

        return stripe
            ? new Color(0.32f, 0.42f, 0.46f, 1f)
            : new Color(0.22f, 0.31f, 0.35f, 1f);
    }

    private static Color CreateCoverPixel(int x, int y, int size)
    {
        bool body = x >= 6 && x < size - 6 && y >= 10 && y < size - 10;
        bool border = x < 10 || y < 14 || x >= size - 10 || y >= size - 14;
        bool stripe = (x + y) % 14 < 7;

        if (!body)
        {
            return Color.clear;
        }

        if (border)
        {
            return new Color(0.08f, 0.12f, 0.14f, 1f);
        }

        return stripe
            ? new Color(0.42f, 0.52f, 0.56f, 1f)
            : new Color(0.28f, 0.38f, 0.42f, 1f);
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

    private static Color CreatePistolPixel(int x, int y, int size)
    {
        bool barrel = x >= 24 && x < 54 && y >= 34 && y < 42;
        bool body = x >= 18 && x < 36 && y >= 24 && y < 38;
        bool grip = x >= 20 && x < 30 && y >= 12 && y < 28;
        bool outline = (barrel || body || grip) && (x % 8 == 0 || y % 8 == 0);

        if (!barrel && !body && !grip)
        {
            return Color.clear;
        }

        return outline ? new Color(0.08f, 0.08f, 0.09f, 1f) : new Color(0.55f, 0.6f, 0.65f, 1f);
    }

    private static Color CreateSMGPixel(int x, int y, int size)
    {
        bool barrel = x >= 18 && x < 56 && y >= 35 && y < 42;
        bool body = x >= 12 && x < 42 && y >= 25 && y < 38;
        bool magazine = x >= 28 && x < 36 && y >= 12 && y < 27;
        bool stock = x >= 6 && x < 16 && y >= 27 && y < 35;

        if (!barrel && !body && !magazine && !stock)
        {
            return Color.clear;
        }

        if (x < 10 || y < 14 || x > 52)
        {
            return new Color(0.1f, 0.1f, 0.12f, 1f);
        }

        return new Color(0.32f, 0.55f, 0.85f, 1f);
    }

    private static Color CreateShotgunPixel(int x, int y, int size)
    {
        bool barrel = x >= 14 && x < 58 && y >= 36 && y < 43;
        bool lowerBarrel = x >= 16 && x < 56 && y >= 29 && y < 34;
        bool stock = x >= 6 && x < 22 && y >= 24 && y < 38;
        bool grip = x >= 24 && x < 32 && y >= 14 && y < 30;

        if (!barrel && !lowerBarrel && !stock && !grip)
        {
            return Color.clear;
        }

        if (stock || grip)
        {
            return new Color(0.45f, 0.24f, 0.08f, 1f);
        }

        return new Color(0.75f, 0.62f, 0.32f, 1f);
    }

    private static Color CreateWeaponPickupPixel(int x, int y, int size)
    {
        Color basePixel = CirclePixel(x, y, size, new Color(0.52f, 0.22f, 0.9f, 1f), new Color(0.16f, 0.04f, 0.32f, 1f));

        if (basePixel.a <= 0f)
        {
            return basePixel;
        }

        bool weaponMark = (x >= 18 && x < 48 && y >= 34 && y < 40) || (x >= 22 && x < 32 && y >= 20 && y < 36);
        return weaponMark ? Color.white : basePixel;
    }

    private static Color CreateShopHealthPixel(int x, int y, int size)
    {
        Color basePixel = CirclePixel(x, y, size, new Color(0.1f, 0.7f, 0.28f, 1f), new Color(0.02f, 0.22f, 0.08f, 1f));

        if (basePixel.a <= 0f)
        {
            return basePixel;
        }

        bool cross = (x >= 28 && x < 36 && y >= 18 && y < 46) || (y >= 28 && y < 36 && x >= 18 && x < 46);
        bool priceDot = x >= 45 && x < 51 && y >= 12 && y < 18;
        return cross || priceDot ? Color.white : basePixel;
    }

    private static Color CreateShopAmmoPixel(int x, int y, int size)
    {
        Color basePixel = CirclePixel(x, y, size, new Color(0.95f, 0.55f, 0.08f, 1f), new Color(0.42f, 0.18f, 0.02f, 1f));

        if (basePixel.a <= 0f)
        {
            return basePixel;
        }

        bool ammoBox = x >= 19 && x < 45 && y >= 24 && y < 42;
        bool stripe = ammoBox && (x + y) % 10 < 5;
        return stripe ? new Color(1f, 0.9f, 0.2f, 1f) : basePixel;
    }

    private static Color CreateShopWeaponPixel(int x, int y, int size)
    {
        Color basePixel = CirclePixel(x, y, size, new Color(0.55f, 0.24f, 0.92f, 1f), new Color(0.16f, 0.04f, 0.34f, 1f));

        if (basePixel.a <= 0f)
        {
            return basePixel;
        }

        bool weapon = (x >= 16 && x < 50 && y >= 35 && y < 41) || (x >= 24 && x < 33 && y >= 18 && y < 36);
        bool priceDot = x >= 45 && x < 51 && y >= 12 && y < 18;
        return weapon || priceDot ? Color.white : basePixel;
    }

    private static Color CreatePrisonPlayerStartMarkerPixel(int x, int y, int size)
    {
        float center = size * 0.5f;
        float distance = Mathf.Abs(x + 0.5f - center) + Mathf.Abs(y + 0.5f - center);

        if (distance > size * 0.36f)
        {
            return Color.clear;
        }

        return distance > size * 0.29f
            ? new Color(0.08f, 0.1f, 0.14f, 1f)
            : new Color(0.38f, 0.55f, 0.7f, 1f);
    }

    private static Color CreatePrisonWallPixel(int x, int y, int size)
    {
        bool border = x < 4 || y < 4 || x >= size - 4 || y >= size - 4;
        bool seam = x % 18 < 2 || y % 18 < 2;
        bool stripe = (x * 2 + y) % 20 < 10;

        if (border)
        {
            return new Color(0.04f, 0.06f, 0.08f, 1f);
        }

        if (seam)
        {
            return new Color(0.12f, 0.16f, 0.2f, 1f);
        }

        return stripe
            ? new Color(0.25f, 0.31f, 0.36f, 1f)
            : new Color(0.18f, 0.24f, 0.3f, 1f);
    }

    private static Color CreatePrisonBarsPixel(int x, int y, int size)
    {
        bool verticalBar = x % 16 < 5;
        bool horizontalRivet = y < 7 || y >= size - 7 || Mathf.Abs(y - size * 0.5f) < 3f;

        if (!verticalBar && !horizontalRivet)
        {
            return Color.clear;
        }

        bool highlight = verticalBar && x % 16 == 1;
        return highlight
            ? new Color(0.58f, 0.68f, 0.74f, 1f)
            : new Color(0.25f, 0.35f, 0.42f, 1f);
    }

    private static Color CreatePrisonFloorPixel(int x, int y, int size)
    {
        bool tileLine = x % 16 == 0 || y % 16 == 0;

        if (tileLine)
        {
            return new Color(0.1f, 0.13f, 0.16f, 1f);
        }

        return (x + y) % 18 < 9
            ? new Color(0.16f, 0.2f, 0.23f, 1f)
            : new Color(0.13f, 0.17f, 0.2f, 1f);
    }

    private static Color CreatePrisonGuardPixel(int x, int y, int size)
    {
        Color basePixel = CirclePixel(x, y, size, new Color(0.25f, 0.38f, 0.54f, 1f), new Color(0.05f, 0.08f, 0.12f, 1f));

        if (basePixel.a <= 0f)
        {
            return basePixel;
        }

        bool visor = y > size * 0.54f && y < size * 0.62f && x > size * 0.25f && x < size * 0.75f;
        bool badge = x > size * 0.58f && x < size * 0.68f && y > size * 0.34f && y < size * 0.46f;

        if (visor)
        {
            return new Color(0.06f, 0.08f, 0.1f, 1f);
        }

        return badge ? new Color(0.95f, 0.78f, 0.16f, 1f) : basePixel;
    }

    private static Color CreatePrisonRangedGuardPixel(int x, int y, int size)
    {
        Color basePixel = CirclePixel(x, y, size, new Color(0.18f, 0.48f, 0.52f, 1f), new Color(0.03f, 0.09f, 0.11f, 1f));

        if (basePixel.a <= 0f)
        {
            return basePixel;
        }

        bool rifle = x > size * 0.58f && x < size * 0.92f && y > size * 0.45f && y < size * 0.55f;
        bool scope = x > size * 0.45f && x < size * 0.58f && y > size * 0.56f && y < size * 0.64f;

        if (rifle || scope)
        {
            return new Color(0.04f, 0.04f, 0.05f, 1f);
        }

        return basePixel;
    }

    private static Color CreatePrisonBruteMinibossPixel(int x, int y, int size)
    {
        Color basePixel = CirclePixel(x, y, size, new Color(0.7f, 0.28f, 0.12f, 1f), new Color(0.16f, 0.05f, 0.02f, 1f));

        if (basePixel.a <= 0f)
        {
            return basePixel;
        }

        float center = size * 0.5f;
        bool stripe = Mathf.Abs(x + 0.5f - center) < size * 0.09f || Mathf.Abs(y + 0.5f - center) < size * 0.09f;
        bool shoulder = y > size * 0.58f && Mathf.Abs(x + 0.5f - center) > size * 0.24f;

        if (stripe)
        {
            return new Color(0.95f, 0.88f, 0.18f, 1f);
        }

        return shoulder ? new Color(0.1f, 0.12f, 0.14f, 1f) : basePixel;
    }

    private static Color CreateWardenBossPixel(int x, int y, int size)
    {
        Color basePixel = CirclePixel(x, y, size, new Color(0.12f, 0.16f, 0.28f, 1f), new Color(0.02f, 0.03f, 0.06f, 1f));

        if (basePixel.a <= 0f)
        {
            return basePixel;
        }

        float center = size * 0.5f;
        float dx = Mathf.Abs(x + 0.5f - center);
        bool cap = y > size * 0.62f && dx < size * 0.34f;
        bool badge = dx < size * 0.1f && y > size * 0.36f && y < size * 0.52f;
        bool eyes = y > size * 0.52f && y < size * 0.58f && dx > size * 0.1f && dx < size * 0.24f;

        if (cap)
        {
            return new Color(0.04f, 0.06f, 0.12f, 1f);
        }

        if (badge)
        {
            return new Color(0.95f, 0.78f, 0.14f, 1f);
        }

        return eyes ? new Color(0.9f, 0.12f, 0.1f, 1f) : basePixel;
    }

    private static Color CreateKeycardPixel(int x, int y, int size)
    {
        bool body = x >= 10 && x < size - 10 && y >= 18 && y < size - 18;

        if (!body)
        {
            return Color.clear;
        }

        bool border = x < 14 || x >= size - 14 || y < 22 || y >= size - 22;
        bool chip = x >= 18 && x < 29 && y >= 29 && y < 40;
        bool stripe = x >= 34 && x < size - 16 && y >= 34 && y < 39;

        if (border)
        {
            return new Color(0.1f, 0.16f, 0.18f, 1f);
        }

        if (chip)
        {
            return new Color(0.95f, 0.78f, 0.18f, 1f);
        }

        return stripe ? Color.white : new Color(0.12f, 0.82f, 0.92f, 1f);
    }

    private static Color CreateLockedGatePixel(int x, int y, int size)
    {
        bool verticalBar = x % 14 < 5;
        bool frame = x < 7 || x >= size - 7 || y < 7 || y >= size - 7;
        bool lockBody = x >= 25 && x < 39 && y >= 24 && y < 38;

        if (lockBody)
        {
            return new Color(0.95f, 0.72f, 0.08f, 1f);
        }

        if (frame || verticalBar)
        {
            return new Color(0.34f, 0.44f, 0.5f, 1f);
        }

        return Color.clear;
    }

    private static Color CreateSecurityLaserPixel(int x, int y, int size)
    {
        float center = size * 0.5f;
        bool core = Mathf.Abs(x + 0.5f - center) < size * 0.08f;
        bool glow = Mathf.Abs(x + 0.5f - center) < size * 0.18f;
        bool emitter = y < 7 || y >= size - 7;

        if (emitter)
        {
            return new Color(0.18f, 0.2f, 0.22f, 1f);
        }

        if (core)
        {
            return new Color(1f, 0.08f, 0.06f, 1f);
        }

        return glow ? new Color(1f, 0.05f, 0.05f, 0.45f) : Color.clear;
    }

    private static Color CreatePrisonCoverPixel(int x, int y, int size)
    {
        bool body = x >= 8 && x < size - 8 && y >= 8 && y < size - 8;
        bool border = x < 12 || y < 12 || x >= size - 12 || y >= size - 12;
        bool warningStripe = body && ((x + y) % 18 < 7);

        if (!body)
        {
            return Color.clear;
        }

        if (border)
        {
            return new Color(0.05f, 0.08f, 0.1f, 1f);
        }

        return warningStripe
            ? new Color(0.52f, 0.58f, 0.18f, 1f)
            : new Color(0.22f, 0.28f, 0.3f, 1f);
    }

    private static Color CreateAlarmLightPixel(int x, int y, int size)
    {
        Color basePixel = CirclePixel(x, y, size, new Color(0.85f, 0.05f, 0.03f, 0.9f), new Color(0.18f, 0.02f, 0.02f, 1f));

        if (basePixel.a <= 0f)
        {
            return basePixel;
        }

        float center = size * 0.5f;
        bool highlight = x < center && y > center && Vector2.Distance(new Vector2(x, y), new Vector2(center - 8f, center + 8f)) < 9f;
        return highlight ? new Color(1f, 0.55f, 0.35f, 1f) : basePixel;
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

    private static void AddSceneToBuildSettingsFirst(string scenePath)
    {
        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        scenes.RemoveAll(scene => scene.path == scenePath);
        scenes.Insert(0, new EditorBuildSettingsScene(scenePath, true));
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

    private static void AssignColor(UnityEngine.Object target, string propertyName, Color value)
    {
        SerializedProperty property = FindSerializedProperty(target, propertyName);

        if (property == null)
        {
            return;
        }

        property.colorValue = value;
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
