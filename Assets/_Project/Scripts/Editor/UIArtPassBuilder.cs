#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class UIArtPassBuilder
{
    private const string UiRoot = "Assets/_Project/Art/Generated/UI";
    private const string IconFolder = UiRoot + "/Icons";
    private const string PanelFolder = UiRoot + "/Panels";
    private const string ButtonFolder = UiRoot + "/Buttons";
    private const string BarFolder = UiRoot + "/Bars";
    private const string CardFolder = UiRoot + "/Cards";
    private const string ThumbnailFolder = UiRoot + "/LevelThumbnails";
    private const string ThemeFolder = "Assets/_Project/ScriptableObjects/UI";
    private const string ShowcaseScenePath = "Assets/_Project/Scenes/Game/UI_Showcase.unity";
    private const string UiDocPath = "Assets/_Project/Docs/UI_ART_PASS_1_0.md";
    private const string ReadmePath = "README_GAME_COMPLETE.md";

    private static readonly Color Background = Rgb(7, 10, 13);
    private static readonly Color Panel = Rgb(15, 24, 31);
    private static readonly Color PanelLight = Rgb(28, 44, 55);
    private static readonly Color Teal = Rgb(0, 216, 224);
    private static readonly Color Cyan = Rgb(91, 239, 255);
    private static readonly Color Orange = Rgb(255, 150, 39);
    private static readonly Color Red = Rgb(238, 34, 56);
    private static readonly Color Green = Rgb(41, 218, 91);
    private static readonly Color Blue = Rgb(73, 145, 255);
    private static readonly Color Purple = Rgb(171, 83, 255);
    private static readonly Color Gold = Rgb(255, 188, 54);
    private static readonly Color Gray = Rgb(178, 192, 202);

    private static readonly string[] RequiredFolders =
    {
        "Assets/_Project",
        "Assets/_Project/Art",
        "Assets/_Project/Art/Generated",
        UiRoot,
        IconFolder,
        PanelFolder,
        ButtonFolder,
        BarFolder,
        CardFolder,
        ThumbnailFolder,
        "Assets/_Project/Prefabs",
        "Assets/_Project/Prefabs/UI",
        "Assets/_Project/Prefabs/UI/HUD",
        "Assets/_Project/Prefabs/UI/Menus",
        "Assets/_Project/Prefabs/UI/Cards",
        "Assets/_Project/Scenes",
        "Assets/_Project/Scenes/Game",
        "Assets/_Project/Scenes/Levels",
        "Assets/_Project/Scripts",
        "Assets/_Project/Scripts/UI",
        "Assets/_Project/Scripts/UI/Theming",
        "Assets/_Project/Scripts/UI/Animation",
        "Assets/_Project/Scripts/Menu",
        "Assets/_Project/Scripts/Editor",
        "Assets/_Project/ScriptableObjects",
        ThemeFolder,
        "Assets/_Project/Docs"
    };

    private static readonly string[] CandidateScenes =
    {
        "Assets/_Project/Scenes/Game/MainMenu.unity",
        "Assets/_Project/Scenes/Game/LevelSelect.unity",
        "Assets/_Project/Scenes/Levels/Level_01_Laboratory.unity",
        "Assets/_Project/Scenes/Levels/Level_02_Prison.unity",
        "Assets/_Project/Scenes/Levels/Level_03_ZombieCity.unity",
        "Assets/_Project/Scenes/Levels/Level_04_SciFiBase.unity",
        "Assets/_Project/Scenes/Levels/Level_05_HorrorHospital.unity",
        "Assets/_Project/Scenes/Prototype_FinalDemo.unity",
        "Assets/_Project/Scenes/Prototype_PrisonLevel_Balanced.unity"
    };

    private static readonly string[] ExistingScripts =
    {
        "HealthUI2D",
        "ResourceUI2D",
        "WeaponUI2D",
        "KeycardUI2D",
        "BuffStatusUI2D",
        "ObjectiveUI2D",
        "BossHealthUI2D",
        "ShopUI2D",
        "MainMenuController2D",
        "LevelSelectController2D",
        "PauseMenuController2D",
        "GameOverController2D",
        "LevelEndController2D",
        "PlayerShooting2D",
        "PlayerHealth2D",
        "PlayerResources2D",
        "BuffChoiceController2D"
    };

    private static readonly string[] RequiredScenes =
    {
        "Assets/_Project/Scenes/Game/MainMenu.unity",
        "Assets/_Project/Scenes/Game/LevelSelect.unity",
        "Assets/_Project/Scenes/Levels/Level_01_Laboratory.unity",
        "Assets/_Project/Scenes/Levels/Level_02_Prison.unity",
        "Assets/_Project/Scenes/Levels/Level_03_ZombieCity.unity",
        "Assets/_Project/Scenes/Levels/Level_04_SciFiBase.unity",
        "Assets/_Project/Scenes/Levels/Level_05_HorrorHospital.unity",
        "Assets/_Project/Scenes/Prototype_FinalDemo.unity",
        "Assets/_Project/Scenes/Prototype_PrisonLevel_Balanced.unity"
    };

    private sealed class AuditResult
    {
        public readonly List<string> ExistingPaths = new List<string>();
        public readonly List<string> MissingPaths = new List<string>();
        public readonly List<string> ExistingScripts = new List<string>();
        public readonly List<string> MissingScripts = new List<string>();
        public readonly List<string> ExistingScenes = new List<string>();
        public readonly List<string> MissingScenes = new List<string>();
        public readonly List<string> UpgradedScenes = new List<string>();
        public readonly List<string> SkippedScenes = new List<string>();
    }

    [MenuItem("Tools/Cod Evadare/UI/Build UI Art Pack")]
    public static void BuildUiArtPack()
    {
        CreateRequiredFolders();
        GenerateSprites();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Built Cod: Evadare UI art pack.");
    }

    [MenuItem("Tools/Cod Evadare/UI/Build UI Theme Assets")]
    public static void BuildUiThemeAssets()
    {
        CreateRequiredFolders();
        GenerateSprites();
        CreateThemeAssets();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Built Cod: Evadare UI theme assets.");
    }

    [MenuItem("Tools/Cod Evadare/UI/Build UI Showcase Scene")]
    public static void BuildUiShowcaseScene()
    {
        CreateRequiredFolders();
        GenerateSprites();
        CreateThemeAssets();
        CreateShowcaseScene();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Built UI showcase scene at {ShowcaseScenePath}.");
    }

    [MenuItem("Tools/Cod Evadare/UI/Apply UI Art Pass To Existing Scenes")]
    public static void ApplyUiArtPassToExistingScenes()
    {
        CreateRequiredFolders();
        GenerateSprites();
        CreateThemeAssets();
        AuditResult audit = AuditRepository();
        ApplyToExistingScenes(audit);
        WriteDocumentation(audit);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Applied UI art pass to {audit.UpgradedScenes.Count} scene(s). Skipped {audit.SkippedScenes.Count} scene(s).");
    }

    [MenuItem("Tools/Cod Evadare/UI/Build Complete UI Art Pass")]
    public static void BuildCompleteUiArtPass()
    {
        CreateRequiredFolders();
        GenerateSprites();
        CreateThemeAssets();
        CreateShowcaseScene();
        AuditResult audit = AuditRepository();
        ApplyToExistingScenes(audit);
        WriteDocumentation(audit);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Built complete UI art pass. Upgraded {audit.UpgradedScenes.Count} scene(s), skipped {audit.SkippedScenes.Count} scene(s).");
    }

    private static void CreateRequiredFolders()
    {
        foreach (string folder in RequiredFolders)
        {
            Directory.CreateDirectory(ToAbsolutePath(folder));
        }

        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
    }

    private static AuditResult AuditRepository()
    {
        AuditResult audit = new AuditResult();
        string[] paths =
        {
            "Assets/_Project",
            "Assets/_Project/Scripts",
            "Assets/_Project/Scripts/UI",
            "Assets/_Project/Scripts/Player",
            "Assets/_Project/Scripts/Weapons",
            "Assets/_Project/Scripts/Rooms",
            "Assets/_Project/Scripts/Shop",
            "Assets/_Project/Scripts/Buffs",
            "Assets/_Project/Scripts/Menu",
            "Assets/_Project/Scripts/Editor",
            "Assets/_Project/Scenes",
            "Assets/_Project/Scenes/Game",
            "Assets/_Project/Scenes/Levels",
            "Assets/_Project/Prefabs",
            "Assets/_Project/ScriptableObjects",
            "Assets/_Project/Art/Generated"
        };

        foreach (string path in paths)
        {
            AddAuditPath(audit.ExistingPaths, audit.MissingPaths, path);
        }

        foreach (string script in ExistingScripts)
        {
            string[] guids = AssetDatabase.FindAssets($"{script} t:Script", new[] { "Assets/_Project/Scripts" });
            (guids.Length > 0 ? audit.ExistingScripts : audit.MissingScripts).Add(script);
        }

        foreach (string scene in RequiredScenes)
        {
            AddAuditPath(audit.ExistingScenes, audit.MissingScenes, scene);
        }

        return audit;
    }

    private static void AddAuditPath(List<string> existing, List<string> missing, string assetPath)
    {
        if (File.Exists(ToAbsolutePath(assetPath)) || Directory.Exists(ToAbsolutePath(assetPath)))
        {
            existing.Add(assetPath);
        }
        else
        {
            missing.Add(assetPath);
        }
    }

    private static void GenerateSprites()
    {
        GeneratePanels();
        GenerateButtons();
        GenerateBars();
        GenerateCards();
        GenerateHudFrames();
        GenerateIcons();
        GenerateWeaponIcons();
        GenerateBuffIcons();
        GenerateLevelThumbnails();
        WriteSprite($"{UiRoot}/UI_Crosshair.png", 64, 64, SpriteKind.Crosshair, Color.clear, Teal, Color.white, Vector4.zero);
    }

    private static void GeneratePanels()
    {
        WriteSprite($"{PanelFolder}/UI_Panel_Dark.png", 256, 128, SpriteKind.Panel, Panel, Teal, PanelLight, new Vector4(18, 18, 18, 18));
        WriteSprite($"{PanelFolder}/UI_Panel_Dark_Header.png", 256, 128, SpriteKind.HeaderPanel, Panel, Teal, PanelLight, new Vector4(18, 18, 18, 18));
        WriteSprite($"{PanelFolder}/UI_Panel_Teal_Frame.png", 256, 128, SpriteKind.Panel, Panel, Teal, Cyan, new Vector4(18, 18, 18, 18));
        WriteSprite($"{PanelFolder}/UI_Panel_Warning_Frame.png", 256, 128, SpriteKind.Panel, Panel, Orange, Gold, new Vector4(18, 18, 18, 18));
        WriteSprite($"{PanelFolder}/UI_Panel_Danger_Frame.png", 256, 128, SpriteKind.Panel, Panel, Red, Orange, new Vector4(18, 18, 18, 18));
        WriteSprite($"{PanelFolder}/UI_Panel_Transparent_Backdrop.png", 256, 128, SpriteKind.Backdrop, new Color(0f, 0f, 0f, 0.55f), Teal, Color.clear, new Vector4(6, 6, 6, 6));
        WriteSprite($"{PanelFolder}/UI_Modal_Backdrop.png", 256, 128, SpriteKind.Backdrop, new Color(0f, 0f, 0f, 0.78f), Color.clear, Color.clear, new Vector4(6, 6, 6, 6));
    }

    private static void GenerateButtons()
    {
        WriteSprite($"{ButtonFolder}/UI_Button_Normal.png", 256, 80, SpriteKind.Button, Rgb(20, 43, 52), Teal, Cyan, new Vector4(18, 18, 18, 18));
        WriteSprite($"{ButtonFolder}/UI_Button_Hover.png", 256, 80, SpriteKind.Button, Rgb(26, 62, 72), Cyan, Teal, new Vector4(18, 18, 18, 18));
        WriteSprite($"{ButtonFolder}/UI_Button_Pressed.png", 256, 80, SpriteKind.Button, Rgb(10, 31, 38), Teal, Color.white, new Vector4(18, 18, 18, 18));
        WriteSprite($"{ButtonFolder}/UI_Button_Disabled.png", 256, 80, SpriteKind.Button, Rgb(30, 34, 38), Rgb(82, 92, 98), Gray, new Vector4(18, 18, 18, 18));
        WriteSprite($"{ButtonFolder}/UI_Button_Danger.png", 256, 80, SpriteKind.Button, Rgb(55, 18, 25), Red, Orange, new Vector4(18, 18, 18, 18));
        WriteSprite($"{ButtonFolder}/UI_Button_Confirm.png", 256, 80, SpriteKind.Button, Rgb(16, 51, 31), Green, Teal, new Vector4(18, 18, 18, 18));
    }

    private static void GenerateBars()
    {
        WriteSprite($"{BarFolder}/UI_Bar_Background.png", 256, 40, SpriteKind.Bar, Rgb(14, 18, 23), Rgb(56, 70, 80), PanelLight, new Vector4(14, 14, 14, 14));
        WriteSprite($"{BarFolder}/UI_Bar_Health_Fill.png", 256, 40, SpriteKind.FillBar, Green, Rgb(85, 255, 132), Color.white, new Vector4(8, 8, 8, 8));
        WriteSprite($"{BarFolder}/UI_Bar_Ammo_Fill.png", 256, 40, SpriteKind.FillBar, Teal, Cyan, Color.white, new Vector4(8, 8, 8, 8));
        WriteSprite($"{BarFolder}/UI_Bar_Boss_Background.png", 320, 42, SpriteKind.Bar, Rgb(16, 10, 14), Red, PanelLight, new Vector4(14, 14, 14, 14));
        WriteSprite($"{BarFolder}/UI_Bar_Boss_Fill.png", 320, 42, SpriteKind.FillBar, Red, Rgb(255, 76, 95), Orange, new Vector4(8, 8, 8, 8));
        WriteSprite($"{BarFolder}/UI_Bar_Experience_Fill.png", 256, 40, SpriteKind.FillBar, Blue, Purple, Cyan, new Vector4(8, 8, 8, 8));
    }

    private static void GenerateCards()
    {
        WriteSprite($"{CardFolder}/UI_Card_Common.png", 256, 160, SpriteKind.Card, Panel, Gray, Color.white, new Vector4(20, 20, 20, 20));
        WriteSprite($"{CardFolder}/UI_Card_Uncommon.png", 256, 160, SpriteKind.Card, Panel, Green, Color.white, new Vector4(20, 20, 20, 20));
        WriteSprite($"{CardFolder}/UI_Card_Rare.png", 256, 160, SpriteKind.Card, Panel, Blue, Cyan, new Vector4(20, 20, 20, 20));
        WriteSprite($"{CardFolder}/UI_Card_Epic.png", 256, 160, SpriteKind.Card, Panel, Purple, Cyan, new Vector4(20, 20, 20, 20));
        WriteSprite($"{CardFolder}/UI_Card_Legendary.png", 256, 160, SpriteKind.Card, Panel, Gold, Orange, new Vector4(20, 20, 20, 20));
        WriteSprite($"{CardFolder}/UI_Card_Shop.png", 256, 160, SpriteKind.Card, Panel, Orange, Teal, new Vector4(20, 20, 20, 20));
        WriteSprite($"{CardFolder}/UI_Card_Buff.png", 256, 160, SpriteKind.Card, Panel, Purple, Green, new Vector4(20, 20, 20, 20));
        WriteSprite($"{CardFolder}/UI_Card_Level_Unlocked.png", 256, 160, SpriteKind.Card, Panel, Teal, Green, new Vector4(20, 20, 20, 20));
        WriteSprite($"{CardFolder}/UI_Card_Level_Locked.png", 256, 160, SpriteKind.Card, Rgb(18, 21, 25), Rgb(74, 82, 90), Red, new Vector4(20, 20, 20, 20));
    }

    private static void GenerateHudFrames()
    {
        WriteSprite($"{PanelFolder}/UI_HUD_ResourceSlot.png", 180, 60, SpriteKind.HudSlot, Panel, Teal, PanelLight, new Vector4(16, 16, 16, 16));
        WriteSprite($"{PanelFolder}/UI_HUD_WeaponSlot.png", 260, 76, SpriteKind.HudSlot, Panel, Orange, PanelLight, new Vector4(16, 16, 16, 16));
        WriteSprite($"{PanelFolder}/UI_HUD_ObjectivePanel.png", 420, 86, SpriteKind.HudSlot, Panel, Teal, Green, new Vector4(16, 16, 16, 16));
        WriteSprite($"{PanelFolder}/UI_HUD_BuffSlot.png", 180, 60, SpriteKind.HudSlot, Panel, Purple, PanelLight, new Vector4(16, 16, 16, 16));
        WriteSprite($"{PanelFolder}/UI_HUD_KeycardSlot.png", 180, 60, SpriteKind.HudSlot, Panel, Gold, PanelLight, new Vector4(16, 16, 16, 16));
    }

    private static void GenerateIcons()
    {
        string[] icons =
        {
            "Heart", "Ammo", "Magazine", "Money", "Keycard", "Weapon", "Buff", "Shop", "Boss", "Reload",
            "Objective", "Warning", "Locked", "Unlocked", "Play", "Back", "Settings", "Pause", "Victory", "GameOver"
        };

        foreach (string icon in icons)
        {
            WriteSprite($"{IconFolder}/Icon_{icon}.png", 64, 64, SpriteKind.Icon, IconPrimary(icon), IconSecondary(icon), Color.white, Vector4.zero, icon);
        }
    }

    private static void GenerateWeaponIcons()
    {
        string[] icons = { "Pistol", "SMG", "Shotgun", "AssaultRifle", "Revolver", "PlasmaRifle", "ArcaneStaff" };

        foreach (string icon in icons)
        {
            WriteSprite($"{IconFolder}/Icon_{icon}.png", 80, 80, SpriteKind.WeaponIcon, Teal, Orange, Color.white, Vector4.zero, icon);
        }
    }

    private static void GenerateBuffIcons()
    {
        string[] icons = { "MaxHealth", "Heal", "MoveSpeed", "FireRate", "Damage", "Reload", "MaxAmmo", "Money" };

        foreach (string icon in icons)
        {
            WriteSprite($"{IconFolder}/Icon_Buff_{icon}.png", 80, 80, SpriteKind.BuffIcon, Purple, Green, Color.white, Vector4.zero, icon);
        }
    }

    private static void GenerateLevelThumbnails()
    {
        WriteSprite($"{ThumbnailFolder}/Thumbnail_Laboratory.png", 320, 180, SpriteKind.Thumbnail, Rgb(10, 34, 36), Teal, Green, Vector4.zero, "Laboratory");
        WriteSprite($"{ThumbnailFolder}/Thumbnail_Prison.png", 320, 180, SpriteKind.Thumbnail, Rgb(26, 34, 42), Rgb(96, 116, 130), Orange, Vector4.zero, "Prison");
        WriteSprite($"{ThumbnailFolder}/Thumbnail_ZombieCity.png", 320, 180, SpriteKind.Thumbnail, Rgb(14, 30, 17), Green, Rgb(154, 255, 42), Vector4.zero, "ZombieCity");
        WriteSprite($"{ThumbnailFolder}/Thumbnail_SciFiBase.png", 320, 180, SpriteKind.Thumbnail, Rgb(13, 20, 47), Blue, Purple, Vector4.zero, "SciFiBase");
        WriteSprite($"{ThumbnailFolder}/Thumbnail_HorrorHospital.png", 320, 180, SpriteKind.Thumbnail, Rgb(34, 12, 22), Red, Purple, Vector4.zero, "HorrorHospital");
    }

    private static Color IconPrimary(string icon)
    {
        switch (icon)
        {
            case "Heart":
            case "GameOver":
            case "Boss":
                return Red;
            case "Money":
            case "Keycard":
            case "Victory":
                return Gold;
            case "Warning":
                return Orange;
            case "Unlocked":
                return Green;
            case "Locked":
                return Gray;
            default:
                return Teal;
        }
    }

    private static Color IconSecondary(string icon)
    {
        switch (icon)
        {
            case "Buff":
            case "Settings":
                return Purple;
            case "Weapon":
            case "Shop":
                return Orange;
            case "Ammo":
            case "Magazine":
                return Blue;
            default:
                return PanelLight;
        }
    }

    private static void CreateThemeAssets()
    {
        Sprite panelSprite = LoadSprite($"{PanelFolder}/UI_Panel_Teal_Frame.png");
        Sprite buttonSprite = LoadSprite($"{ButtonFolder}/UI_Button_Normal.png");
        Sprite buttonPressedSprite = LoadSprite($"{ButtonFolder}/UI_Button_Pressed.png");
        Sprite cardSprite = LoadSprite($"{CardFolder}/UI_Card_Rare.png");
        Sprite resourceSlot = LoadSprite($"{PanelFolder}/UI_HUD_ResourceSlot.png");
        Sprite barBackground = LoadSprite($"{BarFolder}/UI_Bar_Background.png");
        Sprite healthFill = LoadSprite($"{BarFolder}/UI_Bar_Health_Fill.png");
        Sprite bossBackground = LoadSprite($"{BarFolder}/UI_Bar_Boss_Background.png");
        Sprite bossFill = LoadSprite($"{BarFolder}/UI_Bar_Boss_Fill.png");

        CreateTheme("Theme_Global", "Global", Background, Panel, Teal, Orange, Orange, Red, Green, panelSprite, buttonSprite, buttonPressedSprite, cardSprite, resourceSlot, barBackground, healthFill, bossBackground, bossFill);
        CreateTheme("Theme_Laboratory", "Laboratory", Rgb(5, 18, 20), Panel, Teal, Green, Orange, Red, Green, panelSprite, buttonSprite, buttonPressedSprite, LoadSprite($"{CardFolder}/UI_Card_Uncommon.png"), resourceSlot, barBackground, healthFill, bossBackground, bossFill);
        CreateTheme("Theme_Prison", "Prison", Rgb(18, 24, 30), Panel, Rgb(120, 154, 174), Orange, Orange, Red, Green, panelSprite, buttonSprite, buttonPressedSprite, LoadSprite($"{CardFolder}/UI_Card_Shop.png"), resourceSlot, barBackground, healthFill, bossBackground, bossFill);
        CreateTheme("Theme_ZombieCity", "Zombie City", Rgb(8, 22, 10), Panel, Green, Rgb(154, 255, 42), Orange, Red, Green, panelSprite, buttonSprite, buttonPressedSprite, LoadSprite($"{CardFolder}/UI_Card_Uncommon.png"), resourceSlot, barBackground, healthFill, bossBackground, bossFill);
        CreateTheme("Theme_SciFiBase", "Sci-Fi Base", Rgb(8, 11, 32), Panel, Blue, Purple, Orange, Red, Green, panelSprite, buttonSprite, buttonPressedSprite, LoadSprite($"{CardFolder}/UI_Card_Epic.png"), resourceSlot, barBackground, healthFill, bossBackground, bossFill);
        CreateTheme("Theme_HorrorHospital", "Horror Hospital", Rgb(24, 7, 14), Panel, Red, Purple, Orange, Red, Green, panelSprite, buttonSprite, buttonPressedSprite, LoadSprite($"{CardFolder}/UI_Card_Danger.png") ?? LoadSprite($"{CardFolder}/UI_Card_Epic.png"), resourceSlot, barBackground, healthFill, bossBackground, bossFill);
    }

    private static void CreateTheme(
        string assetName,
        string displayName,
        Color backgroundColor,
        Color panelColor,
        Color primaryAccent,
        Color secondaryAccent,
        Color warningColor,
        Color dangerColor,
        Color successColor,
        Sprite panelSprite,
        Sprite buttonSprite,
        Sprite buttonPressedSprite,
        Sprite cardSprite,
        Sprite resourceSlotSprite,
        Sprite healthBarBackgroundSprite,
        Sprite healthBarFillSprite,
        Sprite bossBarBackgroundSprite,
        Sprite bossBarFillSprite)
    {
        string assetPath = $"{ThemeFolder}/{assetName}.asset";
        UITheme2D theme = AssetDatabase.LoadAssetAtPath<UITheme2D>(assetPath);

        if (theme == null)
        {
            theme = ScriptableObject.CreateInstance<UITheme2D>();
            AssetDatabase.CreateAsset(theme, assetPath);
        }

        SerializedObject serializedTheme = new SerializedObject(theme);
        SetProperty(serializedTheme, "themeName", displayName);
        SetProperty(serializedTheme, "backgroundColor", backgroundColor);
        SetProperty(serializedTheme, "panelColor", panelColor);
        SetProperty(serializedTheme, "primaryAccent", primaryAccent);
        SetProperty(serializedTheme, "secondaryAccent", secondaryAccent);
        SetProperty(serializedTheme, "warningColor", warningColor);
        SetProperty(serializedTheme, "dangerColor", dangerColor);
        SetProperty(serializedTheme, "successColor", successColor);
        SetProperty(serializedTheme, "panelSprite", panelSprite);
        SetProperty(serializedTheme, "buttonSprite", buttonSprite);
        SetProperty(serializedTheme, "buttonPressedSprite", buttonPressedSprite);
        SetProperty(serializedTheme, "cardSprite", cardSprite);
        SetProperty(serializedTheme, "resourceSlotSprite", resourceSlotSprite);
        SetProperty(serializedTheme, "healthBarBackgroundSprite", healthBarBackgroundSprite);
        SetProperty(serializedTheme, "healthBarFillSprite", healthBarFillSprite);
        SetProperty(serializedTheme, "bossBarBackgroundSprite", bossBarBackgroundSprite);
        SetProperty(serializedTheme, "bossBarFillSprite", bossBarFillSprite);
        serializedTheme.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(theme);
    }

    private static void CreateShowcaseScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        SceneManager.SetActiveScene(scene);

        GameObject root = new GameObject("UI_Showcase");
        CreateUiCamera(root.transform);
        Canvas canvas = CreateCanvas(root.transform);

        Image backdrop = CreateImage(canvas.transform, "Backdrop", Vector2.zero, new Vector2(1920f, 1080f), LoadSprite($"{PanelFolder}/UI_Modal_Backdrop.png"), Image.Type.Sliced);
        backdrop.color = Color.white;

        Text title = CreateText(canvas.transform, "Title", "COD: EVADARE UI KIT", new Vector2(0f, 470f), new Vector2(900f, 70f), 42, TextAnchor.MiddleCenter);
        title.color = Cyan;

        CreatePanelSample(canvas.transform);
        CreateButtonSample(canvas.transform);
        CreateHudSample(canvas.transform);
        CreateCardsSample(canvas.transform);
        CreateBossSample(canvas.transform);
        CreateModalSample(canvas.transform, "GameOverSample", "GAME OVER", "Press R to restart", new Vector2(-430f, -330f), Red);
        CreateModalSample(canvas.transform, "VictorySample", "LEVEL CLEARED", "Press R to restart", new Vector2(430f, -330f), Green);
        CreateCrosshairSample(canvas.transform);
        CreateEventSystem(root.transform);

        EditorSceneManager.SaveScene(scene, ShowcaseScenePath);
    }

    private static void CreatePanelSample(Transform parent)
    {
        Image panel = CreateImage(parent, "TitlePanel", new Vector2(-610f, 295f), new Vector2(500f, 170f), LoadSprite($"{PanelFolder}/UI_Panel_Teal_Frame.png"), Image.Type.Sliced);
        CreateText(panel.transform, "Title", "Dark tactical panels", new Vector2(0f, 34f), new Vector2(440f, 42f), 24, TextAnchor.MiddleCenter);
        Text body = CreateText(panel.transform, "Body", "High contrast / generated sprites / sliced frames", new Vector2(0f, -24f), new Vector2(440f, 54f), 18, TextAnchor.MiddleCenter);
        body.color = Rgb(210, 229, 236);
    }

    private static void CreateButtonSample(Transform parent)
    {
        CreateButton(parent, "NormalButton", "NORMAL", new Vector2(0f, 315f), LoadSprite($"{ButtonFolder}/UI_Button_Normal.png"));
        CreateButton(parent, "DangerButton", "DANGER", new Vector2(0f, 235f), LoadSprite($"{ButtonFolder}/UI_Button_Danger.png"));
        CreateButton(parent, "ConfirmButton", "CONFIRM", new Vector2(0f, 155f), LoadSprite($"{ButtonFolder}/UI_Button_Confirm.png"));
    }

    private static void CreateHudSample(Transform parent)
    {
        Image healthPanel = CreateImage(parent, "HealthPanel", new Vector2(-610f, 82f), new Vector2(500f, 120f), LoadSprite($"{PanelFolder}/UI_HUD_ResourceSlot.png"), Image.Type.Sliced);
        CreateIcon(healthPanel.transform, "HeartIcon", "Icon_Heart", new Vector2(-205f, 20f), 42f);
        CreateText(healthPanel.transform, "HealthText", "HP 6 / 6", new Vector2(-72f, 28f), new Vector2(200f, 28f), 20, TextAnchor.MiddleLeft);
        CreateImage(healthPanel.transform, "HealthBarBackground", new Vector2(90f, -24f), new Vector2(280f, 28f), LoadSprite($"{BarFolder}/UI_Bar_Background.png"), Image.Type.Sliced);
        CreateImage(healthPanel.transform, "HealthFill", new Vector2(90f, -24f), new Vector2(250f, 18f), LoadSprite($"{BarFolder}/UI_Bar_Health_Fill.png"), Image.Type.Sliced);

        Image ammoPanel = CreateImage(parent, "AmmoPanel", new Vector2(-610f, -66f), new Vector2(500f, 120f), LoadSprite($"{PanelFolder}/UI_HUD_WeaponSlot.png"), Image.Type.Sliced);
        CreateIcon(ammoPanel.transform, "AmmoIcon", "Icon_Ammo", new Vector2(-205f, 20f), 42f);
        CreateText(ammoPanel.transform, "AmmoText", "Ammo 12 / 90", new Vector2(18f, 28f), new Vector2(340f, 30f), 20, TextAnchor.MiddleLeft);
        CreateText(ammoPanel.transform, "WeaponText", "SMG [Rare]", new Vector2(18f, -24f), new Vector2(340f, 30f), 20, TextAnchor.MiddleLeft);
    }

    private static void CreateCardsSample(Transform parent)
    {
        string[] cards = { "Common", "Uncommon", "Rare", "Epic", "Legendary" };

        for (int i = 0; i < cards.Length; i++)
        {
            string rarity = cards[i];
            Image card = CreateImage(parent, $"Card_{rarity}", new Vector2(420f + i * 130f, 250f), new Vector2(118f, 150f), LoadSprite($"{CardFolder}/UI_Card_{rarity}.png"), Image.Type.Sliced);
            CreateText(card.transform, "Name", rarity, new Vector2(0f, -48f), new Vector2(104f, 34f), 15, TextAnchor.MiddleCenter);
            CreateIcon(card.transform, "WeaponIcon", i == 4 ? "Icon_Revolver" : "Icon_Weapon", new Vector2(0f, 24f), 48f);
        }

        string[] levels = { "Laboratory", "Prison", "ZombieCity", "SciFiBase", "HorrorHospital" };

        for (int i = 0; i < levels.Length; i++)
        {
            Image thumbnail = CreateImage(parent, $"Thumbnail_{levels[i]}", new Vector2(-520f + i * 260f, -205f), new Vector2(230f, 130f), LoadSprite($"{ThumbnailFolder}/Thumbnail_{levels[i]}.png"), Image.Type.Simple);
            CreateText(thumbnail.transform, "Label", levels[i].Replace("ZombieCity", "Zombie City").Replace("SciFiBase", "Sci-Fi Base"), new Vector2(0f, -46f), new Vector2(210f, 30f), 16, TextAnchor.MiddleCenter);
        }
    }

    private static void CreateBossSample(Transform parent)
    {
        Image bossPanel = CreateImage(parent, "BossHealthPanel", new Vector2(510f, 35f), new Vector2(640f, 126f), LoadSprite($"{PanelFolder}/UI_Panel_Danger_Frame.png"), Image.Type.Sliced);
        CreateIcon(bossPanel.transform, "BossIcon", "Icon_Boss", new Vector2(-282f, 20f), 44f);
        Text bossName = CreateText(bossPanel.transform, "BossName", "EXPERIMENT-01", new Vector2(0f, 34f), new Vector2(520f, 34f), 24, TextAnchor.MiddleCenter);
        bossName.color = Color.white;
        CreateImage(bossPanel.transform, "BossHealthBackground", new Vector2(28f, -24f), new Vector2(540f, 32f), LoadSprite($"{BarFolder}/UI_Bar_Boss_Background.png"), Image.Type.Sliced);
        CreateImage(bossPanel.transform, "BossHealthFill", new Vector2(28f, -24f), new Vector2(390f, 22f), LoadSprite($"{BarFolder}/UI_Bar_Boss_Fill.png"), Image.Type.Sliced);
        CreateText(bossPanel.transform, "BossHealthText", "HP 50 / 50", new Vector2(28f, -24f), new Vector2(260f, 28f), 16, TextAnchor.MiddleCenter);
    }

    private static void CreateModalSample(Transform parent, string name, string title, string subtitle, Vector2 position, Color accent)
    {
        Image panel = CreateImage(parent, name, position, new Vector2(360f, 170f), LoadSprite($"{PanelFolder}/UI_Panel_Dark_Header.png"), Image.Type.Sliced);
        panel.color = Color.white;
        Text titleText = CreateText(panel.transform, "Title", title, new Vector2(0f, 30f), new Vector2(320f, 44f), 28, TextAnchor.MiddleCenter);
        titleText.color = accent;
        CreateText(panel.transform, "Subtitle", subtitle, new Vector2(0f, -32f), new Vector2(320f, 36f), 18, TextAnchor.MiddleCenter);
    }

    private static void CreateCrosshairSample(Transform parent)
    {
        CreateImage(parent, "CrosshairSample", new Vector2(790f, -60f), new Vector2(64f, 64f), LoadSprite($"{UiRoot}/UI_Crosshair.png"), Image.Type.Simple);
    }

    private static void ApplyToExistingScenes(AuditResult audit)
    {
        bool hasGameplayScripts = audit.ExistingScripts.Contains("PlayerShooting2D") && audit.ExistingScripts.Contains("PlayerHealth2D");

        foreach (string scenePath in CandidateScenes)
        {
            if (!File.Exists(ToAbsolutePath(scenePath)))
            {
                audit.SkippedScenes.Add($"{scenePath} (missing)");
                Debug.Log($"UI art pass skipped missing scene: {scenePath}");
                continue;
            }

            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            Canvas[] canvases = UnityEngine.Object.FindObjectsOfType<Canvas>(true);

            if (canvases.Length == 0)
            {
                if (hasGameplayScripts)
                {
                    CreateCanvas(null);
                    canvases = UnityEngine.Object.FindObjectsOfType<Canvas>(true);
                }
                else
                {
                    audit.SkippedScenes.Add($"{scenePath} (no Canvas and gameplay scripts missing)");
                    continue;
                }
            }

            ThemeColors colors = GetThemeColors(scenePath);

            foreach (Canvas canvas in canvases)
            {
                ConfigureCanvas(canvas);
                StyleCanvas(canvas, colors);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            audit.UpgradedScenes.Add(scenePath);
        }
    }

    private static void StyleCanvas(Canvas canvas, ThemeColors colors)
    {
        foreach (Image image in canvas.GetComponentsInChildren<Image>(true))
        {
            StyleImage(image, colors);
        }

        foreach (Text text in canvas.GetComponentsInChildren<Text>(true))
        {
            StyleText(text, colors);
        }

        foreach (Button button in canvas.GetComponentsInChildren<Button>(true))
        {
            StyleButton(button);
        }

        UIThemeApplier2D applier = canvas.GetComponent<UIThemeApplier2D>();

        if (applier == null)
        {
            applier = canvas.gameObject.AddComponent<UIThemeApplier2D>();
        }

        SetSerializedReference(applier, "theme", LoadTheme(colors.ThemeAssetName));
    }

    private static void StyleImage(Image image, ThemeColors colors)
    {
        if (image == null)
        {
            return;
        }

        string lowerName = image.name.ToLowerInvariant();
        bool isButton = image.GetComponent<Button>() != null || image.GetComponentInParent<Button>() != null;

        if (lowerName.Contains("boss") && lowerName.Contains("fill"))
        {
            AssignImageSprite(image, $"{BarFolder}/UI_Bar_Boss_Fill.png", Image.Type.Sliced);
            return;
        }

        if (lowerName.Contains("boss") && (lowerName.Contains("background") || lowerName.Contains("bar")))
        {
            AssignImageSprite(image, $"{BarFolder}/UI_Bar_Boss_Background.png", Image.Type.Sliced);
            return;
        }

        if (lowerName.Contains("health") && lowerName.Contains("fill"))
        {
            AssignImageSprite(image, $"{BarFolder}/UI_Bar_Health_Fill.png", Image.Type.Sliced);
            return;
        }

        if ((lowerName.Contains("ammo") || lowerName.Contains("reload")) && lowerName.Contains("fill"))
        {
            AssignImageSprite(image, $"{BarFolder}/UI_Bar_Ammo_Fill.png", Image.Type.Sliced);
            return;
        }

        if (lowerName.Contains("fill"))
        {
            AssignImageSprite(image, $"{BarFolder}/UI_Bar_Experience_Fill.png", Image.Type.Sliced);
            return;
        }

        if (lowerName.Contains("background") && lowerName.Contains("bar"))
        {
            AssignImageSprite(image, $"{BarFolder}/UI_Bar_Background.png", Image.Type.Sliced);
            return;
        }

        if (isButton)
        {
            AssignImageSprite(image, $"{ButtonFolder}/UI_Button_Normal.png", Image.Type.Sliced);
            image.color = Color.white;
            return;
        }

        if (lowerName.Contains("card") || lowerName.Contains("choice"))
        {
            AssignImageSprite(image, $"{CardFolder}/UI_Card_Buff.png", Image.Type.Sliced);
            image.color = Color.white;
            return;
        }

        if (lowerName.Contains("panel") || lowerName.Contains("modal") || lowerName.Contains("backdrop"))
        {
            AssignImageSprite(image, lowerName.Contains("gameover") ? $"{PanelFolder}/UI_Panel_Danger_Frame.png" : $"{PanelFolder}/UI_Panel_Teal_Frame.png", Image.Type.Sliced);
            image.color = Color.white;
            return;
        }

        if (lowerName.Contains("icon"))
        {
            image.color = colors.Primary;
        }
    }

    private static void StyleText(Text text, ThemeColors colors)
    {
        if (text == null)
        {
            return;
        }

        text.font = GetBuiltinFont();
        text.resizeTextForBestFit = false;
        text.color = Color.white;

        string lowerName = text.name.ToLowerInvariant();
        string lowerValue = text.text != null ? text.text.ToLowerInvariant() : string.Empty;

        if (lowerName.Contains("title") || lowerValue.Contains("cod: evadare"))
        {
            text.color = colors.Primary;
        }
        else if (lowerName.Contains("hint") || lowerName.Contains("objective"))
        {
            text.color = Rgb(205, 237, 242);
        }
        else if (lowerValue.Contains("game over"))
        {
            text.color = Red;
        }
        else if (lowerValue.Contains("cleared") || lowerValue.Contains("escaped") || lowerValue.Contains("victory"))
        {
            text.color = Green;
        }
    }

    private static void StyleButton(Button button)
    {
        if (button == null)
        {
            return;
        }

        Image targetImage = button.targetGraphic as Image;

        if (targetImage != null)
        {
            AssignImageSprite(targetImage, $"{ButtonFolder}/UI_Button_Normal.png", Image.Type.Sliced);
        }

        SpriteState state = button.spriteState;
        state.highlightedSprite = LoadSprite($"{ButtonFolder}/UI_Button_Hover.png");
        state.pressedSprite = LoadSprite($"{ButtonFolder}/UI_Button_Pressed.png");
        state.disabledSprite = LoadSprite($"{ButtonFolder}/UI_Button_Disabled.png");
        button.spriteState = state;

        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = Color.white;
        colors.pressedColor = Color.white;
        colors.disabledColor = new Color(1f, 1f, 1f, 0.42f);
        button.colors = colors;

        if (button.GetComponent<UIHoverSound2D>() == null)
        {
            button.gameObject.AddComponent<UIHoverSound2D>();
        }
    }

    private static void AssignImageSprite(Image image, string spritePath, Image.Type type)
    {
        Sprite sprite = LoadSprite(spritePath);

        if (sprite == null || image == null)
        {
            return;
        }

        image.sprite = sprite;

        if (image.type != Image.Type.Filled)
        {
            image.type = type;
        }

        image.color = Color.white;
    }

    private static void CreateUiCamera(Transform parent)
    {
        GameObject cameraObject = new GameObject("Main Camera");
        cameraObject.transform.SetParent(parent);
        cameraObject.transform.position = new Vector3(0f, 0f, -10f);
        Camera camera = cameraObject.AddComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = Background;
        camera.orthographic = true;
        camera.orthographicSize = 5f;
        cameraObject.tag = "MainCamera";
    }

    private static Canvas CreateCanvas(Transform parent)
    {
        GameObject canvasObject = new GameObject("Canvas");

        if (parent != null)
        {
            canvasObject.transform.SetParent(parent);
        }

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<GraphicRaycaster>();
        ConfigureCanvas(canvas);
        return canvas;
    }

    private static void ConfigureCanvas(Canvas canvas)
    {
        if (canvas == null)
        {
            return;
        }

        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();

        if (scaler == null)
        {
            scaler = canvas.gameObject.AddComponent<CanvasScaler>();
        }

        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;
    }

    private static Image CreateImage(Transform parent, string name, Vector2 anchoredPosition, Vector2 size, Sprite sprite, Image.Type type)
    {
        GameObject gameObject = CreateRectObject(name, parent, anchoredPosition, size);
        Image image = gameObject.AddComponent<Image>();
        image.sprite = sprite;
        image.type = type;
        image.color = Color.white;
        return image;
    }

    private static void CreateIcon(Transform parent, string name, string iconName, Vector2 anchoredPosition, float size)
    {
        CreateImage(parent, name, anchoredPosition, new Vector2(size, size), LoadSprite($"{IconFolder}/{iconName}.png"), Image.Type.Simple);
    }

    private static Button CreateButton(Transform parent, string name, string label, Vector2 anchoredPosition, Sprite sprite)
    {
        Image image = CreateImage(parent, name, anchoredPosition, new Vector2(260f, 60f), sprite, Image.Type.Sliced);
        Button button = image.gameObject.AddComponent<Button>();
        button.targetGraphic = image;
        Text text = CreateText(image.transform, "Text", label, Vector2.zero, new Vector2(230f, 42f), 21, TextAnchor.MiddleCenter);
        text.color = Color.white;
        StyleButton(button);
        return button;
    }

    private static Text CreateText(Transform parent, string name, string text, Vector2 anchoredPosition, Vector2 size, int fontSize, TextAnchor alignment)
    {
        GameObject gameObject = CreateRectObject(name, parent, anchoredPosition, size);
        Text textComponent = gameObject.AddComponent<Text>();
        textComponent.font = GetBuiltinFont();
        textComponent.text = text;
        textComponent.fontSize = fontSize;
        textComponent.alignment = alignment;
        textComponent.color = Color.white;
        textComponent.raycastTarget = false;
        return textComponent;
    }

    private static GameObject CreateRectObject(string name, Transform parent, Vector2 anchoredPosition, Vector2 size)
    {
        GameObject gameObject = new GameObject(name);
        gameObject.transform.SetParent(parent, false);
        RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = size;
        return gameObject;
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

    private enum SpriteKind
    {
        Panel,
        HeaderPanel,
        Backdrop,
        Button,
        Bar,
        FillBar,
        Card,
        HudSlot,
        Icon,
        WeaponIcon,
        BuffIcon,
        Thumbnail,
        Crosshair
    }

    private static void WriteSprite(string assetPath, int width, int height, SpriteKind kind, Color primary, Color secondary, Color accent, Vector4 border, string label = "")
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Bilinear;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                texture.SetPixel(x, y, GetPixel(kind, x, y, width, height, primary, secondary, accent, label));
            }
        }

        texture.Apply();
        Directory.CreateDirectory(Path.GetDirectoryName(ToAbsolutePath(assetPath)) ?? ToAbsolutePath(UiRoot));
        File.WriteAllBytes(ToAbsolutePath(assetPath), texture.EncodeToPNG());
        UnityEngine.Object.DestroyImmediate(texture);
        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceSynchronousImport);

        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;

        if (importer == null)
        {
            return;
        }

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.filterMode = FilterMode.Bilinear;
        importer.spritePixelsPerUnit = 100f;
        importer.spriteBorder = border;
        importer.SaveAndReimport();
    }

    private static Color GetPixel(SpriteKind kind, int x, int y, int width, int height, Color primary, Color secondary, Color accent, string label)
    {
        float nx = width > 1 ? (float)x / (width - 1) : 0f;
        float ny = height > 1 ? (float)y / (height - 1) : 0f;

        switch (kind)
        {
            case SpriteKind.Panel:
            case SpriteKind.HeaderPanel:
            case SpriteKind.Card:
            case SpriteKind.HudSlot:
                return DrawFramedRect(x, y, width, height, primary, secondary, accent, kind == SpriteKind.HeaderPanel);
            case SpriteKind.Backdrop:
                return primary;
            case SpriteKind.Button:
                return DrawButton(x, y, width, height, primary, secondary, accent);
            case SpriteKind.Bar:
                return DrawFramedRect(x, y, width, height, primary, secondary, accent, false);
            case SpriteKind.FillBar:
                return Color.Lerp(primary, secondary, nx) * (0.88f + ny * 0.22f);
            case SpriteKind.Icon:
                return DrawIcon(x, y, width, height, primary, secondary, accent, label);
            case SpriteKind.WeaponIcon:
                return DrawWeaponIcon(x, y, width, height, primary, secondary, accent, label);
            case SpriteKind.BuffIcon:
                return DrawBuffIcon(x, y, width, height, primary, secondary, accent, label);
            case SpriteKind.Thumbnail:
                return DrawThumbnail(x, y, width, height, primary, secondary, accent, label);
            case SpriteKind.Crosshair:
                return DrawCrosshair(x, y, width, height, secondary, accent);
            default:
                return Color.Lerp(primary, secondary, nx * ny);
        }
    }

    private static Color DrawFramedRect(int x, int y, int width, int height, Color fill, Color frame, Color accent, bool header)
    {
        bool border = x < 4 || y < 4 || x >= width - 4 || y >= height - 4;
        bool innerBorder = x == 8 || y == 8 || x == width - 9 || y == height - 9;

        if (border)
        {
            return frame;
        }

        if (innerBorder)
        {
            return Color.Lerp(frame, accent, 0.35f);
        }

        if (header && y > height - 28)
        {
            return Color.Lerp(frame, fill, 0.35f);
        }

        float gradient = (float)y / Mathf.Max(1, height - 1);
        Color result = Color.Lerp(fill, Color.Lerp(fill, frame, 0.2f), gradient);

        if ((x + y) % 23 == 0)
        {
            result = Color.Lerp(result, accent, 0.08f);
        }

        return result;
    }

    private static Color DrawButton(int x, int y, int width, int height, Color fill, Color frame, Color accent)
    {
        bool border = x < 3 || y < 3 || x >= width - 3 || y >= height - 3;
        bool slash = (x + y) % 36 < 3 && y < 18;

        if (border)
        {
            return frame;
        }

        Color result = Color.Lerp(fill, Color.Lerp(fill, accent, 0.25f), (float)y / Mathf.Max(1, height - 1));
        return slash ? Color.Lerp(result, accent, 0.32f) : result;
    }

    private static Color DrawIcon(int x, int y, int width, int height, Color primary, Color secondary, Color accent, string label)
    {
        Vector2 center = new Vector2(width * 0.5f, height * 0.5f);
        Vector2 point = new Vector2(x, y);
        float distance = Vector2.Distance(point, center);
        float radius = Mathf.Min(width, height) * 0.38f;
        bool ring = Mathf.Abs(distance - radius) < 2.2f;
        bool inside = distance < radius * 0.76f;

        if (label == "Heart")
        {
            float dx = Mathf.Abs(x - center.x) / radius;
            float dy = (y - center.y) / radius;
            inside = Mathf.Pow(dx, 2f) + Mathf.Pow(dy + 0.18f, 2f) < 0.5f || (dy < 0f && Mathf.Abs(dx) < 0.72f + dy);
        }
        else if (label == "Play")
        {
            inside = x > width * 0.34f && x < width * 0.72f && Mathf.Abs(y - center.y) < (x - width * 0.34f) * 0.72f;
        }
        else if (label == "Back")
        {
            inside = Mathf.Abs(y - center.y) < 4f && x > width * 0.28f && x < width * 0.74f || Mathf.Abs((x - width * 0.28f) + (y - center.y)) < 4f && x < width * 0.45f || Mathf.Abs((x - width * 0.28f) - (y - center.y)) < 4f && x < width * 0.45f;
        }
        else if (label == "Pause")
        {
            inside = (x > width * 0.34f && x < width * 0.44f || x > width * 0.56f && x < width * 0.66f) && y > height * 0.28f && y < height * 0.72f;
        }
        else if (label == "Locked" || label == "Unlocked")
        {
            inside = x > width * 0.28f && x < width * 0.72f && y > height * 0.28f && y < height * 0.58f;
            ring = ring || (Mathf.Abs(distance - radius * 0.54f) < 2f && y > height * 0.48f);
        }
        else if (label == "Warning")
        {
            inside = y > height * 0.22f && y < height * 0.76f && Mathf.Abs(x - center.x) < (y - height * 0.22f) * 0.45f;
        }

        if (inside)
        {
            return primary;
        }

        if (ring)
        {
            return secondary;
        }

        return Color.clear;
    }

    private static Color DrawWeaponIcon(int x, int y, int width, int height, Color primary, Color secondary, Color accent, string label)
    {
        bool body = y > height * 0.48f && y < height * 0.58f && x > width * 0.18f && x < width * 0.78f;
        bool grip = x > width * 0.35f && x < width * 0.48f && y > height * 0.22f && y < height * 0.5f;
        bool barrel = y > height * 0.54f && y < height * 0.64f && x > width * 0.64f && x < width * 0.9f;
        bool accentPart = label.Contains("Plasma") ? Mathf.Abs(Vector2.Distance(new Vector2(x, y), new Vector2(width * 0.55f, height * 0.56f)) - 12f) < 2f : false;

        if (accentPart)
        {
            return accent;
        }

        if (body || grip || barrel)
        {
            return primary;
        }

        return Color.clear;
    }

    private static Color DrawBuffIcon(int x, int y, int width, int height, Color primary, Color secondary, Color accent, string label)
    {
        Vector2 center = new Vector2(width * 0.5f, height * 0.5f);
        float distance = Vector2.Distance(new Vector2(x, y), center);
        bool ring = Mathf.Abs(distance - width * 0.28f) < 3f;
        bool plus = Mathf.Abs(x - center.x) < 4f && Mathf.Abs(y - center.y) < height * 0.22f || Mathf.Abs(y - center.y) < 4f && Mathf.Abs(x - center.x) < width * 0.22f;
        bool bolt = label.Contains("Fire") || label.Contains("Damage") ? x > center.x - 6f && x < center.x + 10f && y > height * 0.2f && y < height * 0.8f && (x + y) % 14 < 7 : false;

        if (bolt)
        {
            return accent;
        }

        if (plus)
        {
            return secondary;
        }

        if (ring)
        {
            return primary;
        }

        return Color.clear;
    }

    private static Color DrawThumbnail(int x, int y, int width, int height, Color background, Color primary, Color accent, string label)
    {
        float nx = (float)x / Mathf.Max(1, width - 1);
        float ny = (float)y / Mathf.Max(1, height - 1);
        Color result = Color.Lerp(background, Color.Lerp(background, primary, 0.35f), ny);

        if (label == "Laboratory")
        {
            if (x % 58 < 3 || y % 44 < 3)
            {
                result = Color.Lerp(result, primary, 0.65f);
            }
        }
        else if (label == "Prison")
        {
            if (x % 36 < 5)
            {
                result = Color.Lerp(result, primary, 0.65f);
            }

            if ((x + y) % 52 < 6 && y < 48)
            {
                result = Color.Lerp(result, accent, 0.5f);
            }
        }
        else if (label == "ZombieCity")
        {
            if (y < 42 && x % 42 < 24)
            {
                result = Color.Lerp(result, primary, 0.5f);
            }

            if ((x * y) % 97 < 3)
            {
                result = accent;
            }
        }
        else if (label == "SciFiBase")
        {
            if (Mathf.Abs(Mathf.Sin(nx * 20f + ny * 8f)) < 0.08f)
            {
                result = Color.Lerp(result, primary, 0.8f);
            }
        }
        else if (label == "HorrorHospital")
        {
            if (Mathf.Abs(x - width * 0.5f) < 5f || Mathf.Abs(y - height * 0.5f) < 5f)
            {
                result = Color.Lerp(result, accent, 0.65f);
            }
        }

        return result;
    }

    private static Color DrawCrosshair(int x, int y, int width, int height, Color primary, Color accent)
    {
        Vector2 center = new Vector2(width * 0.5f, height * 0.5f);
        float distance = Vector2.Distance(new Vector2(x, y), center);
        bool ring = Mathf.Abs(distance - width * 0.28f) < 1.6f;
        bool horizontal = Mathf.Abs(y - center.y) < 1.5f && Mathf.Abs(x - center.x) > 9f && Mathf.Abs(x - center.x) < 27f;
        bool vertical = Mathf.Abs(x - center.x) < 1.5f && Mathf.Abs(y - center.y) > 9f && Mathf.Abs(y - center.y) < 27f;

        if (ring || horizontal || vertical)
        {
            return primary;
        }

        if (distance < 2f)
        {
            return accent;
        }

        return Color.clear;
    }

    private static void WriteDocumentation(AuditResult audit)
    {
        string markdown = BuildDocumentation(audit);
        File.WriteAllText(ToAbsolutePath(UiDocPath), markdown);
        AssetDatabase.ImportAsset(UiDocPath);

        if (File.Exists(ToAbsolutePath(ReadmePath)))
        {
            string readme = File.ReadAllText(ToAbsolutePath(ReadmePath));
            const string marker = "\n## UI Art Pass 1.0\n";
            int markerIndex = readme.IndexOf(marker, StringComparison.Ordinal);

            string section = marker + "\nRun `Tools/Cod Evadare/UI/Build Complete UI Art Pass` to regenerate the generated UI sprites, theme assets, showcase scene, and safe scene styling pass.\n\nShowcase scene: `Assets/_Project/Scenes/Game/UI_Showcase.unity`\n";

            if (markerIndex >= 0)
            {
                readme = readme.Substring(0, markerIndex) + section;
            }
            else
            {
                readme += section;
            }

            File.WriteAllText(ToAbsolutePath(ReadmePath), readme);
            AssetDatabase.ImportAsset(ReadmePath);
        }
        else
        {
            File.WriteAllText(ToAbsolutePath("README_UI_ART_PASS.md"), BuildStandaloneReadme());
            AssetDatabase.ImportAsset("README_UI_ART_PASS.md");
        }
    }

    private static string BuildDocumentation(AuditResult audit)
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendLine("# Cod: Evadare - UI Art Pass 1.0");
        builder.AppendLine();
        builder.AppendLine("Unity version: 2022.3.62f3 LTS");
        builder.AppendLine();
        builder.AppendLine("## Repository Audit");
        AppendList(builder, "Existing paths", audit.ExistingPaths);
        AppendList(builder, "Missing paths", audit.MissingPaths);
        AppendList(builder, "Existing scripts", audit.ExistingScripts);
        AppendList(builder, "Missing scripts", audit.MissingScripts);
        AppendList(builder, "Existing scenes", audit.ExistingScenes);
        AppendList(builder, "Missing scenes", audit.MissingScenes);
        builder.AppendLine("## Style Direction");
        builder.AppendLine();
        builder.AppendLine("Dark tactical sci-fi UI with teal/cyan primary accents, orange warnings, red danger states, green success states, framed panels, generated icons, rarity cards, and level-themed color accents.");
        builder.AppendLine();
        builder.AppendLine("## Generated Assets");
        builder.AppendLine();
        builder.AppendLine("- Sprites: `Assets/_Project/Art/Generated/UI`");
        builder.AppendLine("- Icons: `Assets/_Project/Art/Generated/UI/Icons`");
        builder.AppendLine("- Theme assets: `Assets/_Project/ScriptableObjects/UI`");
        builder.AppendLine("- Showcase scene: `Assets/_Project/Scenes/Game/UI_Showcase.unity`");
        builder.AppendLine();
        AppendList(builder, "Scenes upgraded", audit.UpgradedScenes);
        AppendList(builder, "Scenes skipped", audit.SkippedScenes);
        builder.AppendLine("## Run");
        builder.AppendLine();
        builder.AppendLine("Use `Tools/Cod Evadare/UI/Build Complete UI Art Pass`.");
        builder.AppendLine();
        builder.AppendLine("## Inspect");
        builder.AppendLine();
        builder.AppendLine("Open `Assets/_Project/Scenes/Game/UI_Showcase.unity` and inspect the generated UI kit.");
        builder.AppendLine();
        builder.AppendLine("## Manual Test Checklist");
        builder.AppendLine();
        builder.AppendLine("- Main menu buttons still load the correct campaign scenes.");
        builder.AppendLine("- Level select still respects locked/unlocked states.");
        builder.AppendLine("- HUD text remains readable in all five gameplay scenes.");
        builder.AppendLine("- Shop, buff choice, boss health, pause, game over, and victory panels still function.");
        builder.AppendLine("- Crosshair is only enabled where intentionally assigned.");
        builder.AppendLine();
        builder.AppendLine("## Known Limitations");
        builder.AppendLine();
        builder.AppendLine("This pass uses generated placeholder UI sprites and broad scene styling by object names. Final bespoke layout polish still requires Play Mode review.");
        return builder.ToString();
    }

    private static string BuildStandaloneReadme()
    {
        return "# Cod: Evadare - UI Art Pass 1.0\n\nOpen the project in Unity 2022.3.62f3 LTS and run `Tools/Cod Evadare/UI/Build Complete UI Art Pass`.\n\nGenerated UI assets are under `Assets/_Project/Art/Generated/UI`.\n\nInspect `Assets/_Project/Scenes/Game/UI_Showcase.unity`.\n";
    }

    private static void AppendList(StringBuilder builder, string title, List<string> values)
    {
        builder.AppendLine($"### {title}");
        builder.AppendLine();

        if (values == null || values.Count == 0)
        {
            builder.AppendLine("- None");
        }
        else
        {
            foreach (string value in values)
            {
                builder.AppendLine($"- `{value}`");
            }
        }

        builder.AppendLine();
    }

    private static Sprite LoadSprite(string assetPath)
    {
        return AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
    }

    private static UITheme2D LoadTheme(string assetName)
    {
        return AssetDatabase.LoadAssetAtPath<UITheme2D>($"{ThemeFolder}/{assetName}.asset");
    }

    private static Font GetBuiltinFont()
    {
        return Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
    }

    private static string ToAbsolutePath(string assetPath)
    {
        return Path.Combine(Directory.GetCurrentDirectory(), assetPath).Replace('\\', Path.DirectorySeparatorChar);
    }

    private static Color Rgb(int r, int g, int b)
    {
        return new Color(r / 255f, g / 255f, b / 255f, 1f);
    }

    private static void SetProperty(SerializedObject serializedObject, string propertyName, string value)
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);

        if (property != null)
        {
            property.stringValue = value;
        }
    }

    private static void SetProperty(SerializedObject serializedObject, string propertyName, Color value)
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);

        if (property != null)
        {
            property.colorValue = value;
        }
    }

    private static void SetProperty(SerializedObject serializedObject, string propertyName, UnityEngine.Object value)
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);

        if (property != null)
        {
            property.objectReferenceValue = value;
        }
    }

    private static void SetSerializedReference(UnityEngine.Object target, string propertyName, UnityEngine.Object value)
    {
        if (target == null)
        {
            return;
        }

        SerializedObject serializedObject = new SerializedObject(target);
        SerializedProperty property = serializedObject.FindProperty(propertyName);

        if (property != null)
        {
            property.objectReferenceValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
        }
    }

    private readonly struct ThemeColors
    {
        public readonly Color Primary;
        public readonly string ThemeAssetName;

        public ThemeColors(Color primary, string themeAssetName)
        {
            Primary = primary;
            ThemeAssetName = themeAssetName;
        }
    }

    private static ThemeColors GetThemeColors(string scenePath)
    {
        if (scenePath.Contains("Prison"))
        {
            return new ThemeColors(Rgb(120, 154, 174), "Theme_Prison");
        }

        if (scenePath.Contains("Zombie"))
        {
            return new ThemeColors(Green, "Theme_ZombieCity");
        }

        if (scenePath.Contains("SciFi"))
        {
            return new ThemeColors(Blue, "Theme_SciFiBase");
        }

        if (scenePath.Contains("Horror"))
        {
            return new ThemeColors(Red, "Theme_HorrorHospital");
        }

        return new ThemeColors(Teal, "Theme_Laboratory");
    }
}
#endif
