#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameplayVisualArtPassBuilder
{
    private const string GeneratedArtFolder = "Assets/_Project/Art/Generated";
    private const string DocumentationPath = "Assets/_Project/Docs/GAMEPLAY_VISUAL_ART_PASS_2_0.md";

    private static readonly Color Transparent = new Color(0f, 0f, 0f, 0f);
    private static readonly Color Outline = Rgb(7, 10, 13);
    private static readonly Color MetalDark = Rgb(36, 49, 58);
    private static readonly Color MetalMid = Rgb(73, 95, 108);
    private static readonly Color Cyan = Rgb(0, 216, 224);
    private static readonly Color Blue = Rgb(55, 145, 255);
    private static readonly Color Orange = Rgb(255, 149, 33);
    private static readonly Color Red = Rgb(230, 37, 49);
    private static readonly Color Green = Rgb(42, 220, 88);
    private static readonly Color Purple = Rgb(147, 67, 255);
    private static readonly Color Gold = Rgb(255, 190, 54);

    private struct SpriteSpec
    {
        public readonly string Path;
        public readonly VisualKind Kind;
        public readonly Color Primary;
        public readonly Color Secondary;
        public readonly Color Accent;
        public readonly int Size;

        public SpriteSpec(string path, VisualKind kind, Color primary, Color secondary, Color accent, int size = 128)
        {
            Path = path;
            Kind = kind;
            Primary = primary;
            Secondary = secondary;
            Accent = accent;
            Size = size;
        }
    }

    private enum VisualKind
    {
        Player,
        Enemy,
        RangedEnemy,
        Brute,
        Boss,
        PickupHealth,
        PickupAmmo,
        PickupMoney,
        PickupKeycard,
        Weapon,
        Bullet,
        Projectile,
        Wall,
        Floor,
        Door,
        Cover,
        Hazard,
        Turret,
        Light,
        ShopIcon,
        Reward
    }

    [MenuItem("Tools/Cod Evadare/Art/Build Gameplay Visual Art Pass")]
    public static void BuildGameplayVisualArtPass()
    {
        Directory.CreateDirectory(ToAbsolutePath(GeneratedArtFolder));

        foreach (SpriteSpec spec in GetSpriteSpecs())
        {
            if (!File.Exists(ToAbsolutePath(spec.Path)))
            {
                WriteSprite(spec);
            }

            ConfigureSpriteImport(spec.Path, spec.Size);
        }

        WriteDocumentation();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Applied gameplay visual art pass 2.0 import settings. Missing generated sprites were recreated with fallback procedural art.");
    }

    [MenuItem("Tools/Cod Evadare/Art/Apply Mobile Art Scale To Open Scene")]
    public static void ApplyMobileArtScaleToOpenScene()
    {
        Scene scene = SceneManager.GetActiveScene();

        if (!scene.IsValid())
        {
            Debug.LogWarning("Cannot apply mobile art scale because there is no valid active scene.");
            return;
        }

        int changedCount = 0;

        foreach (GameObject root in scene.GetRootGameObjects())
        {
            SpriteRenderer[] renderers = root.GetComponentsInChildren<SpriteRenderer>(true);

            foreach (SpriteRenderer renderer in renderers)
            {
                if (renderer == null || renderer.sprite == null || !ShouldUseMobileTileRendering(renderer.gameObject.name))
                {
                    continue;
                }

                Undo.RecordObject(renderer.transform, "Apply mobile art scale");
                Undo.RecordObject(renderer, "Apply mobile art scale");

                Vector2 targetSize = GetCurrentWorldLikeSize(renderer);

                if (IsChunkyCollisionTile(renderer.gameObject.name))
                {
                    targetSize = GetChunkyTileSize(targetSize);
                }

                renderer.transform.localScale = Vector3.one;
                renderer.drawMode = SpriteDrawMode.Tiled;
                renderer.size = targetSize;

                BoxCollider2D collider = renderer.GetComponent<BoxCollider2D>();

                if (collider != null)
                {
                    Undo.RecordObject(collider, "Apply mobile art scale");
                    collider.size = targetSize;
                }

                changedCount++;
            }
        }

        if (changedCount > 0)
        {
            EditorSceneManager.MarkSceneDirty(scene);
        }

        Debug.Log($"Applied mobile art scale to {changedCount} renderers in scene '{scene.name}'. Save the scene if the layout looks correct.");
    }

    private static IEnumerable<SpriteSpec> GetSpriteSpecs()
    {
        yield return Spec("Player_Prototype.png", VisualKind.Player, Blue, Cyan, Orange);
        yield return Spec("Bullet_Prototype.png", VisualKind.Bullet, Gold, Orange, Color.white, 64);
        yield return Spec("EnemyProjectile.png", VisualKind.Projectile, Red, Purple, Color.white, 64);

        yield return Spec("Enemy_Prototype.png", VisualKind.Enemy, Red, Rgb(100, 24, 32), Orange);
        yield return Spec("LabEnemy.png", VisualKind.Enemy, Green, Rgb(18, 90, 65), Cyan);
        yield return Spec("LabRangedEnemy.png", VisualKind.RangedEnemy, Cyan, Rgb(20, 76, 82), Green);
        yield return Spec("LabMiniboss.png", VisualKind.Brute, Green, Rgb(24, 96, 65), Cyan, 128);
        yield return Spec("PrisonGuard.png", VisualKind.Enemy, Rgb(98, 126, 144), Rgb(38, 50, 62), Orange);
        yield return Spec("PrisonRangedGuard.png", VisualKind.RangedEnemy, Rgb(118, 150, 170), Rgb(38, 50, 62), Orange);
        yield return Spec("RiotBruteMiniboss.png", VisualKind.Brute, Rgb(132, 142, 150), Rgb(42, 48, 55), Orange, 128);
        yield return Spec("PrisonBruteMiniboss.png", VisualKind.Brute, Rgb(132, 142, 150), Rgb(42, 48, 55), Orange, 128);
        yield return Spec("ZombieWalker.png", VisualKind.Enemy, Green, Rgb(53, 82, 42), Rgb(156, 255, 58));
        yield return Spec("ZombieRunner.png", VisualKind.Enemy, Rgb(90, 220, 62), Rgb(50, 80, 40), Red);
        yield return Spec("ZombieSpitter.png", VisualKind.RangedEnemy, Rgb(116, 238, 74), Rgb(38, 92, 45), Rgb(178, 255, 65));
        yield return Spec("ExploderZombie.png", VisualKind.Brute, Rgb(120, 255, 72), Rgb(56, 86, 42), Orange);
        yield return Spec("SecurityBot.png", VisualKind.Enemy, Blue, Rgb(30, 42, 80), Cyan);
        yield return Spec("DroneEnemy.png", VisualKind.RangedEnemy, Blue, Rgb(30, 42, 80), Purple);
        yield return Spec("LaserTurret.png", VisualKind.Turret, MetalMid, MetalDark, Red);
        yield return Spec("ShadowCrawler.png", VisualKind.Enemy, Purple, Rgb(34, 18, 48), Red);
        yield return Spec("HauntedNurse.png", VisualKind.RangedEnemy, Rgb(220, 220, 210), Rgb(94, 38, 64), Red);
        yield return Spec("GhostEnemy.png", VisualKind.Enemy, Rgb(120, 200, 255), Rgb(60, 74, 106), Purple);

        yield return Spec("Miniboss_Prototype.png", VisualKind.Brute, Purple, Rgb(48, 26, 72), Cyan, 128);
        yield return Spec("ReactorMiniboss.png", VisualKind.Brute, Blue, Rgb(24, 44, 76), Cyan, 128);
        yield return Spec("NecromancerMiniboss.png", VisualKind.Brute, Purple, Rgb(42, 20, 58), Green, 128);
        yield return Spec("SurgeonMiniboss.png", VisualKind.Brute, Red, Rgb(82, 30, 42), Rgb(220, 220, 210), 128);

        yield return Spec("Boss_Experiment01.png", VisualKind.Boss, Red, Cyan, Green, 160);
        yield return Spec("Experiment01Boss.png", VisualKind.Boss, Red, Cyan, Green, 160);
        yield return Spec("WardenBoss.png", VisualKind.Boss, Rgb(134, 144, 154), Rgb(32, 40, 48), Orange, 160);
        yield return Spec("AICoreBoss.png", VisualKind.Boss, Blue, Purple, Cyan, 160);
        yield return Spec("AbominationBoss.png", VisualKind.Boss, Green, Rgb(70, 34, 40), Red, 160);
        yield return Spec("NightmareDoctorBoss.png", VisualKind.Boss, Rgb(210, 210, 205), Rgb(62, 22, 48), Red, 160);

        yield return Spec("HealthPickup_Prototype.png", VisualKind.PickupHealth, Green, Rgb(18, 92, 46), Color.white);
        yield return Spec("AmmoPickup_Prototype.png", VisualKind.PickupAmmo, Blue, Rgb(22, 42, 78), Gold);
        yield return Spec("MoneyPickup_Prototype.png", VisualKind.PickupMoney, Gold, Rgb(96, 72, 18), Color.white);
        yield return Spec("Pickup_Health.png", VisualKind.PickupHealth, Green, Rgb(18, 92, 46), Color.white);
        yield return Spec("Pickup_Ammo.png", VisualKind.PickupAmmo, Blue, Rgb(22, 42, 78), Gold);
        yield return Spec("Pickup_Money.png", VisualKind.PickupMoney, Gold, Rgb(96, 72, 18), Color.white);
        yield return Spec("Keycard.png", VisualKind.PickupKeycard, Gold, Orange, Cyan);
        yield return Spec("Reward_Prototype.png", VisualKind.Reward, Purple, Gold, Cyan);

        yield return Spec("Pistol_Prototype.png", VisualKind.Weapon, MetalMid, MetalDark, Cyan);
        yield return Spec("SMG_Prototype.png", VisualKind.Weapon, Blue, MetalDark, Cyan);
        yield return Spec("Shotgun_Prototype.png", VisualKind.Weapon, Orange, MetalDark, Gold);
        yield return Spec("ShopHealth_Prototype.png", VisualKind.ShopIcon, Green, Rgb(18, 92, 46), Color.white);
        yield return Spec("ShopAmmo_Prototype.png", VisualKind.ShopIcon, Blue, Rgb(22, 42, 78), Gold);
        yield return Spec("ShopWeapon_Prototype.png", VisualKind.ShopIcon, Orange, MetalDark, Cyan);
        yield return Spec("ShopIcon_Health.png", VisualKind.ShopIcon, Green, Rgb(18, 92, 46), Color.white);
        yield return Spec("ShopIcon_Ammo.png", VisualKind.ShopIcon, Blue, Rgb(22, 42, 78), Gold);
        yield return Spec("ShopIcon_Weapon.png", VisualKind.ShopIcon, Orange, MetalDark, Cyan);

        yield return Spec("LabWall.png", VisualKind.Wall, Rgb(38, 74, 74), Rgb(16, 36, 38), Cyan);
        yield return Spec("LabFloor.png", VisualKind.Floor, Rgb(12, 26, 27), Rgb(22, 46, 47), Cyan);
        yield return Spec("PrisonWall.png", VisualKind.Wall, Rgb(74, 88, 96), Rgb(34, 42, 48), Orange);
        yield return Spec("PrisonFloor.png", VisualKind.Floor, Rgb(31, 37, 42), Rgb(48, 55, 62), Orange);
        yield return Spec("PrisonBars.png", VisualKind.Wall, Rgb(82, 96, 104), Rgb(24, 30, 36), Blue);
        yield return Spec("PrisonCover.png", VisualKind.Cover, Rgb(78, 82, 74), Rgb(35, 38, 34), Orange);
        yield return Spec("CityWall.png", VisualKind.Wall, Rgb(68, 78, 72), Rgb(26, 32, 30), Green);
        yield return Spec("StreetFloor.png", VisualKind.Floor, Rgb(24, 27, 28), Rgb(42, 46, 48), Green);
        yield return Spec("SciFiWall.png", VisualKind.Wall, Rgb(34, 48, 86), Rgb(14, 20, 42), Purple);
        yield return Spec("SciFiFloor.png", VisualKind.Floor, Rgb(12, 16, 36), Rgb(25, 34, 68), Blue);
        yield return Spec("HospitalWall.png", VisualKind.Wall, Rgb(72, 58, 68), Rgb(34, 22, 34), Red);
        yield return Spec("HospitalFloor.png", VisualKind.Floor, Rgb(30, 24, 30), Rgb(54, 42, 52), Red);
        yield return Spec("Wall_Prototype.png", VisualKind.Wall, MetalMid, MetalDark, Cyan);
        yield return Spec("BossArenaWall.png", VisualKind.Wall, Rgb(48, 34, 82), Rgb(20, 14, 40), Purple);
        yield return Spec("Cover_Block.png", VisualKind.Cover, MetalMid, MetalDark, Orange);
        yield return Spec("Door.png", VisualKind.Door, MetalMid, MetalDark, Cyan);
        yield return Spec("PlasmaDoor.png", VisualKind.Door, Blue, Rgb(14, 20, 42), Purple);
        yield return Spec("LockedGate.png", VisualKind.Door, Rgb(80, 92, 98), Rgb(20, 25, 29), Gold);
        yield return Spec("SecurityLaser.png", VisualKind.Hazard, Red, Rgb(48, 10, 16), Orange);
        yield return Spec("ElectricFloor.png", VisualKind.Hazard, Blue, Rgb(12, 20, 48), Cyan);
        yield return Spec("ToxicPuddle.png", VisualKind.Hazard, Green, Rgb(22, 76, 28), Rgb(154, 255, 42));
        yield return Spec("ToxicSlime.png", VisualKind.Hazard, Green, Rgb(22, 76, 28), Rgb(154, 255, 42));
        yield return Spec("BloodHazard.png", VisualKind.Hazard, Red, Rgb(80, 10, 20), Purple);
        yield return Spec("AlarmLight.png", VisualKind.Light, Red, Rgb(80, 14, 20), Orange);
        yield return Spec("FlickerLight.png", VisualKind.Light, Gold, Rgb(80, 64, 20), Color.white);
    }

    private static SpriteSpec Spec(string fileName, VisualKind kind, Color primary, Color secondary, Color accent, int size = 128)
    {
        return new SpriteSpec($"{GeneratedArtFolder}/{fileName}", kind, primary, secondary, accent, size);
    }

    private static void WriteSprite(SpriteSpec spec)
    {
        Texture2D texture = new Texture2D(spec.Size, spec.Size, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Bilinear;

        for (int y = 0; y < spec.Size; y++)
        {
            for (int x = 0; x < spec.Size; x++)
            {
                texture.SetPixel(x, y, GetPixel(spec, x, y));
            }
        }

        texture.Apply();
        Directory.CreateDirectory(Path.GetDirectoryName(ToAbsolutePath(spec.Path)) ?? ToAbsolutePath(GeneratedArtFolder));
        File.WriteAllBytes(ToAbsolutePath(spec.Path), texture.EncodeToPNG());
        Object.DestroyImmediate(texture);
    }

    private static Color GetPixel(SpriteSpec spec, int x, int y)
    {
        float size = spec.Size;
        float nx = (x + 0.5f) / size;
        float ny = (y + 0.5f) / size;
        Vector2 point = new Vector2(nx - 0.5f, ny - 0.5f);

        switch (spec.Kind)
        {
            case VisualKind.Player:
                return DrawPlayer(point, spec.Primary, spec.Secondary, spec.Accent);
            case VisualKind.Enemy:
                return DrawActor(point, spec.Primary, spec.Secondary, spec.Accent, false, false);
            case VisualKind.RangedEnemy:
                return DrawActor(point, spec.Primary, spec.Secondary, spec.Accent, true, false);
            case VisualKind.Brute:
                return DrawActor(point, spec.Primary, spec.Secondary, spec.Accent, true, true);
            case VisualKind.Boss:
                return DrawBoss(point, spec.Primary, spec.Secondary, spec.Accent);
            case VisualKind.PickupHealth:
                return DrawPickup(point, spec.Primary, spec.Secondary, spec.Accent, 0);
            case VisualKind.PickupAmmo:
                return DrawPickup(point, spec.Primary, spec.Secondary, spec.Accent, 1);
            case VisualKind.PickupMoney:
                return DrawPickup(point, spec.Primary, spec.Secondary, spec.Accent, 2);
            case VisualKind.PickupKeycard:
                return DrawPickup(point, spec.Primary, spec.Secondary, spec.Accent, 3);
            case VisualKind.Reward:
                return DrawPickup(point, spec.Primary, spec.Secondary, spec.Accent, 4);
            case VisualKind.Weapon:
                return DrawWeapon(point, spec.Primary, spec.Secondary, spec.Accent);
            case VisualKind.Bullet:
            case VisualKind.Projectile:
                return DrawProjectile(point, spec.Primary, spec.Secondary, spec.Accent);
            case VisualKind.Wall:
                return DrawWall(nx, ny, spec.Primary, spec.Secondary, spec.Accent);
            case VisualKind.Floor:
                return DrawFloor(nx, ny, spec.Primary, spec.Secondary, spec.Accent);
            case VisualKind.Door:
                return DrawDoor(nx, ny, spec.Primary, spec.Secondary, spec.Accent);
            case VisualKind.Cover:
                return DrawCover(nx, ny, spec.Primary, spec.Secondary, spec.Accent);
            case VisualKind.Hazard:
                return DrawHazard(point, spec.Primary, spec.Secondary, spec.Accent);
            case VisualKind.Turret:
                return DrawTurret(point, spec.Primary, spec.Secondary, spec.Accent);
            case VisualKind.Light:
                return DrawLight(point, spec.Primary, spec.Secondary, spec.Accent);
            case VisualKind.ShopIcon:
                return DrawPickup(point, spec.Primary, spec.Secondary, spec.Accent, 4);
            default:
                return Transparent;
        }
    }

    private static Color DrawPlayer(Vector2 p, Color body, Color glow, Color weapon)
    {
        float d = p.magnitude;
        bool shadow = InEllipse(p + new Vector2(0f, -0.05f), 0.34f, 0.24f);
        bool torso = InEllipse(p, 0.24f, 0.31f);
        bool visor = InRect(p, -0.1f, 0.12f, 0.1f, 0.25f);
        bool gun = InRect(p, 0.14f, -0.045f, 0.42f, 0.045f) || InRect(p, 0.31f, -0.025f, 0.46f, 0.025f);
        bool stripe = Mathf.Abs(p.x) < 0.035f && Mathf.Abs(p.y) < 0.28f;

        if (gun)
        {
            return weapon;
        }

        if (visor)
        {
            return glow;
        }

        if (stripe)
        {
            return Color.Lerp(body, glow, 0.7f);
        }

        if (torso)
        {
            return Color.Lerp(body, glow, Mathf.Clamp01(0.2f + d));
        }

        if (shadow)
        {
            return Outline;
        }

        return Transparent;
    }

    private static Color DrawActor(Vector2 p, Color body, Color dark, Color accent, bool ranged, bool brute)
    {
        float scale = brute ? 1.18f : 1f;
        bool outline = InEllipse(p, 0.31f * scale, 0.31f * scale);
        bool torso = InEllipse(p, 0.25f * scale, 0.25f * scale);
        bool eye = InEllipse(p + new Vector2(-0.07f, -0.05f), 0.045f * scale, 0.06f * scale) || InEllipse(p + new Vector2(0.07f, -0.05f), 0.045f * scale, 0.06f * scale);
        bool weapon = ranged && (InRect(p, 0.12f, -0.035f, 0.43f, 0.035f) || InRect(p, 0.3f, -0.02f, 0.48f, 0.02f));
        bool armor = Mathf.Abs(p.x) < 0.08f && Mathf.Abs(p.y) < 0.24f;

        if (weapon)
        {
            return accent;
        }

        if (eye)
        {
            return accent;
        }

        if (armor)
        {
            return Color.Lerp(body, accent, 0.45f);
        }

        if (torso)
        {
            return body;
        }

        if (outline)
        {
            return dark;
        }

        return Transparent;
    }

    private static Color DrawBoss(Vector2 p, Color body, Color accentA, Color accentB)
    {
        float d = p.magnitude;
        bool core = InEllipse(p, 0.26f, 0.26f);
        bool shell = InEllipse(p, 0.38f, 0.38f);
        bool horns = InRect(p, -0.42f, 0.1f, -0.24f, 0.2f) || InRect(p, 0.24f, 0.1f, 0.42f, 0.2f) || InRect(p, -0.1f, -0.46f, 0.1f, -0.32f);
        bool cross = Mathf.Abs(p.x) < 0.06f || Mathf.Abs(p.y) < 0.06f;
        bool ring = Mathf.Abs(d - 0.34f) < 0.025f;

        if (horns)
        {
            return accentA;
        }

        if (cross && core)
        {
            return accentB;
        }

        if (core)
        {
            return Color.Lerp(body, accentB, 0.35f);
        }

        if (ring)
        {
            return accentA;
        }

        if (shell)
        {
            return body;
        }

        return Transparent;
    }

    private static Color DrawPickup(Vector2 p, Color primary, Color secondary, Color accent, int type)
    {
        bool baseCircle = InEllipse(p, 0.3f, 0.3f);
        bool outline = InEllipse(p, 0.36f, 0.36f);
        bool symbol = false;

        switch (type)
        {
            case 0:
                symbol = Mathf.Abs(p.x) < 0.045f && Mathf.Abs(p.y) < 0.18f || Mathf.Abs(p.y) < 0.045f && Mathf.Abs(p.x) < 0.18f;
                break;
            case 1:
                symbol = InRect(p, -0.15f, -0.18f, 0.15f, 0.18f) && (Mathf.Abs(p.x) > 0.1f || Mathf.Abs(p.y) > 0.12f);
                break;
            case 2:
                symbol = Mathf.Abs(p.magnitude - 0.13f) < 0.035f || Mathf.Abs(p.x) < 0.035f && Mathf.Abs(p.y) < 0.18f;
                break;
            case 3:
                symbol = InRect(p, -0.2f, -0.1f, 0.2f, 0.1f) || InRect(p, 0.08f, -0.2f, 0.18f, -0.08f);
                break;
            default:
                symbol = Mathf.Abs(p.x) + Mathf.Abs(p.y) < 0.22f;
                break;
        }

        if (symbol)
        {
            return accent;
        }

        if (baseCircle)
        {
            return primary;
        }

        if (outline)
        {
            return secondary;
        }

        return Transparent;
    }

    private static Color DrawWeapon(Vector2 p, Color primary, Color secondary, Color accent)
    {
        bool body = InRect(p, -0.32f, -0.07f, 0.24f, 0.07f);
        bool barrel = InRect(p, 0.2f, -0.035f, 0.45f, 0.035f);
        bool grip = InRect(p, -0.16f, -0.28f, -0.04f, -0.04f);
        bool muzzle = InEllipse(p + new Vector2(-0.45f, 0f), 0.06f, 0.06f);

        if (muzzle)
        {
            return accent;
        }

        if (body || barrel)
        {
            return primary;
        }

        if (grip)
        {
            return secondary;
        }

        return Transparent;
    }

    private static Color DrawProjectile(Vector2 p, Color primary, Color secondary, Color accent)
    {
        float d = p.magnitude;
        bool core = InEllipse(p, 0.18f, 0.1f);
        bool trail = p.x < 0.15f && p.x > -0.42f && Mathf.Abs(p.y) < 0.06f + (p.x + 0.42f) * 0.12f;

        if (core)
        {
            return accent;
        }

        if (trail)
        {
            return primary;
        }

        if (d < 0.28f)
        {
            return new Color(secondary.r, secondary.g, secondary.b, 0.55f);
        }

        return Transparent;
    }

    private static Color DrawWall(float nx, float ny, Color primary, Color secondary, Color accent)
    {
        bool border = nx < 0.06f || ny < 0.06f || nx > 0.94f || ny > 0.94f;
        bool seam = Mathf.Abs((nx * 5f) % 1f - 0.5f) < 0.025f || Mathf.Abs((ny * 4f) % 1f - 0.5f) < 0.025f;
        bool diagonal = Mathf.Abs((nx + ny) % 0.22f) < 0.012f;

        if (border)
        {
            return secondary;
        }

        if (seam)
        {
            return Color.Lerp(primary, accent, 0.45f);
        }

        if (diagonal)
        {
            return Color.Lerp(primary, accent, 0.18f);
        }

        return Color.Lerp(primary, secondary, ny * 0.35f);
    }

    private static Color DrawFloor(float nx, float ny, Color primary, Color secondary, Color accent)
    {
        bool grid = Mathf.Abs((nx * 6f) % 1f) < 0.018f || Mathf.Abs((ny * 6f) % 1f) < 0.018f;
        bool scuff = Mathf.Abs(Mathf.Sin(nx * 31f + ny * 17f)) < 0.06f;

        if (grid)
        {
            return Color.Lerp(primary, accent, 0.25f);
        }

        if (scuff)
        {
            return Color.Lerp(primary, secondary, 0.4f);
        }

        return Color.Lerp(primary, secondary, ny * 0.22f);
    }

    private static Color DrawDoor(float nx, float ny, Color primary, Color secondary, Color accent)
    {
        bool frame = nx < 0.14f || nx > 0.86f || ny < 0.08f || ny > 0.92f;
        bool bars = Mathf.Abs((nx * 5f) % 1f - 0.5f) < 0.04f;
        bool light = nx > 0.58f && nx < 0.68f && ny > 0.35f && ny < 0.65f;

        if (light)
        {
            return accent;
        }

        if (frame)
        {
            return secondary;
        }

        if (bars)
        {
            return Color.Lerp(primary, accent, 0.35f);
        }

        return primary;
    }

    private static Color DrawCover(float nx, float ny, Color primary, Color secondary, Color accent)
    {
        bool border = nx < 0.08f || nx > 0.92f || ny < 0.08f || ny > 0.92f;
        bool stripes = Mathf.Abs((nx + ny) % 0.22f) < 0.04f;

        if (border)
        {
            return secondary;
        }

        if (stripes)
        {
            return accent;
        }

        return primary;
    }

    private static Color DrawHazard(Vector2 p, Color primary, Color secondary, Color accent)
    {
        float d = p.magnitude;
        bool pool = InEllipse(p, 0.38f, 0.24f);
        bool glow = InEllipse(p, 0.48f, 0.32f);
        bool stripe = Mathf.Abs((p.x + p.y) % 0.16f) < 0.025f;

        if (pool && stripe)
        {
            return accent;
        }

        if (pool)
        {
            return primary;
        }

        if (glow)
        {
            return new Color(secondary.r, secondary.g, secondary.b, Mathf.Clamp01(0.45f - d * 0.5f));
        }

        return Transparent;
    }

    private static Color DrawTurret(Vector2 p, Color primary, Color secondary, Color accent)
    {
        bool basePart = InEllipse(p, 0.28f, 0.28f);
        bool barrel = InRect(p, 0.1f, -0.045f, 0.46f, 0.045f);
        bool eye = InEllipse(p, 0.09f, 0.09f);

        if (barrel)
        {
            return accent;
        }

        if (eye)
        {
            return Color.white;
        }

        if (basePart)
        {
            return primary;
        }

        if (InEllipse(p, 0.35f, 0.35f))
        {
            return secondary;
        }

        return Transparent;
    }

    private static Color DrawLight(Vector2 p, Color primary, Color secondary, Color accent)
    {
        float d = p.magnitude;

        if (d < 0.16f)
        {
            return accent;
        }

        if (d < 0.28f)
        {
            return primary;
        }

        if (d < 0.42f)
        {
            return new Color(secondary.r, secondary.g, secondary.b, 0.5f);
        }

        return Transparent;
    }

    private static bool InEllipse(Vector2 p, float rx, float ry)
    {
        if (rx <= 0f || ry <= 0f)
        {
            return false;
        }

        return (p.x * p.x) / (rx * rx) + (p.y * p.y) / (ry * ry) <= 1f;
    }

    private static bool InRect(Vector2 p, float left, float bottom, float right, float top)
    {
        return p.x >= left && p.x <= right && p.y >= bottom && p.y <= top;
    }

    private static bool ShouldUseMobileTileRendering(string objectName)
    {
        if (string.IsNullOrEmpty(objectName))
        {
            return false;
        }

        string lowerName = objectName.ToLowerInvariant();
        return lowerName.Contains("wall")
            || lowerName.Contains("floor")
            || lowerName.Contains("door")
            || lowerName.Contains("gate")
            || lowerName.Contains("bars")
            || lowerName.Contains("corridor");
    }

    private static bool IsChunkyCollisionTile(string objectName)
    {
        if (string.IsNullOrEmpty(objectName))
        {
            return false;
        }

        string lowerName = objectName.ToLowerInvariant();
        return lowerName.Contains("wall")
            || lowerName.Contains("door")
            || lowerName.Contains("gate")
            || lowerName.Contains("bars")
            || lowerName.Contains("corridor");
    }

    private static Vector2 GetCurrentWorldLikeSize(SpriteRenderer renderer)
    {
        BoxCollider2D collider = renderer.GetComponent<BoxCollider2D>();
        Vector3 localScale = renderer.transform.localScale;

        if (collider != null)
        {
            return new Vector2(
                Mathf.Abs(collider.size.x * localScale.x),
                Mathf.Abs(collider.size.y * localScale.y));
        }

        if (renderer.sprite != null)
        {
            Bounds spriteBounds = renderer.sprite.bounds;
            return new Vector2(
                Mathf.Abs(spriteBounds.size.x * localScale.x),
                Mathf.Abs(spriteBounds.size.y * localScale.y));
        }

        return Vector2.one;
    }

    private static Vector2 GetChunkyTileSize(Vector2 size)
    {
        float width = Mathf.Max(0.05f, Mathf.Abs(size.x));
        float height = Mathf.Max(0.05f, Mathf.Abs(size.y));

        if (width >= height)
        {
            height = Mathf.Max(0.6f, height);
        }
        else
        {
            width = Mathf.Max(0.6f, width);
        }

        return new Vector2(width, height);
    }

    private static void ConfigureSpriteImport(string assetPath, int spritePixelsPerUnit)
    {
        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceSynchronousImport);
        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;

        if (importer == null)
        {
            return;
        }

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        TextureImporterSettings settings = new TextureImporterSettings();
        importer.ReadTextureSettings(settings);
        settings.spriteMeshType = SpriteMeshType.FullRect;
        importer.SetTextureSettings(settings);
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.filterMode = FilterMode.Bilinear;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.spritePixelsPerUnit = Mathf.Max(1, spritePixelsPerUnit);
        importer.SaveAndReimport();
    }

    private static void WriteDocumentation()
    {
        string text =
            "# Cod: Evadare - Gameplay Visual Art Pass 2.0\n\n" +
            "This pass refreshes generated gameplay sprites in-place so existing prefab and scene references keep their Unity GUIDs.\n\n" +
            "Version 2.0 uses larger source sprites, smoother import settings, stronger silhouettes, richer environment panels, clearer pickups/projectiles, and corrected pixels-per-unit so bigger art does not stretch the map.\n\n" +
            "Run `Tools/Cod Evadare/Art/Build Gameplay Visual Art Pass` from Unity to apply import settings. Existing polished PNGs are preserved; missing generated sprites are recreated with fallback procedural art.\n\n" +
            "Run `Tools/Cod Evadare/Art/Apply Mobile Art Scale To Open Scene` on an open generated scene to convert existing walls, floors, doors, gates, bars, and corridor pieces to tiled rendering with corrected sizes. Save the scene after checking the layout.\n\n" +
            "Generated assets are under `Assets/_Project/Art/Generated` and include richer player, enemy, pickup, weapon, projectile, wall, floor, door, cover, hazard, and boss sprites.\n\n" +
            "Manual verification: open the current level scenes and confirm actor silhouettes, walls, doors, pickups, hazards, and projectiles are readable in Play Mode.\n";

        Directory.CreateDirectory(ToAbsolutePath("Assets/_Project/Docs"));
        File.WriteAllText(ToAbsolutePath(DocumentationPath), text);
        AssetDatabase.ImportAsset(DocumentationPath);
    }

    private static string ToAbsolutePath(string assetPath)
    {
        return Path.Combine(Directory.GetCurrentDirectory(), assetPath).Replace('\\', Path.DirectorySeparatorChar);
    }

    private static Color Rgb(int r, int g, int b)
    {
        return new Color(r / 255f, g / 255f, b / 255f, 1f);
    }
}
#endif
