using UnityEngine;

[CreateAssetMenu(menuName = "Cod Evadare/Balance/Prison Balance Profile 2D")]
public class PrisonBalanceProfile2D : ScriptableObject
{
    [Header("Player")]
    [SerializeField] private int playerMaxHealth = 6;
    [SerializeField] private int startingAmmo = 100;
    [SerializeField] private int maxAmmo = 160;
    [SerializeField] private int startingMoney = 45;

    [Header("Normal Prison Guard")]
    [SerializeField] private int prisonGuardHealth = 3;
    [SerializeField] private float prisonGuardMoveSpeed = 1.55f;
    [SerializeField] private int prisonGuardContactDamage = 1;

    [Header("Ranged Guard")]
    [SerializeField] private int rangedGuardHealth = 3;
    [SerializeField] private float rangedGuardMoveSpeed = 1.15f;
    [SerializeField] private float rangedGuardPreferredDistance = 4.5f;
    [SerializeField] private float rangedGuardMinimumDistance = 2.25f;
    [SerializeField] private float rangedGuardAttackInterval = 1.45f;
    [SerializeField] private float rangedGuardProjectileSpeed = 6.5f;
    [SerializeField] private int rangedGuardDamage = 1;
    [SerializeField] private float rangedGuardAimTelegraphDuration = 0.25f;

    [Header("Security Laser")]
    [SerializeField] private int laserDamage = 1;
    [SerializeField] private float laserDamageCooldown = 0.9f;
    [SerializeField] private float laserWarningDuration = 0.45f;
    [SerializeField] private float laserActiveDuration = 1.65f;
    [SerializeField] private float laserInactiveDuration = 2.1f;

    [Header("Riot Brute Miniboss")]
    [SerializeField] private int riotBruteHealth = 20;
    [SerializeField] private float riotBruteMoveSpeed = 1.15f;
    [SerializeField] private int riotBruteDamage = 2;
    [SerializeField] private float riotBruteDamageCooldown = 1.2f;

    [Header("Warden Boss")]
    [SerializeField] private int wardenHealth = 58;
    [SerializeField] private float wardenMoveSpeed = 0.95f;
    [SerializeField] private float wardenStopDistance = 4f;
    [SerializeField] private float wardenAttackInterval = 1.45f;
    [SerializeField] private float wardenPhaseTwoMultiplier = 0.65f;
    [SerializeField] private int wardenPhaseOneRadialProjectiles = 8;
    [SerializeField] private int wardenPhaseTwoRadialProjectiles = 14;
    [SerializeField] private int wardenProjectileDamage = 1;
    [SerializeField] private float wardenProjectileSpeed = 6.8f;

    [Header("Economy")]
    [SerializeField] private int shopHealthPrice = 20;
    [SerializeField] private int shopAmmoPrice = 15;
    [SerializeField] private int shopWeaponPrice = 70;
    [SerializeField] private int moneyPickupAmountSmall = 20;
    [SerializeField] private int ammoPickupAmount = 18;
    [SerializeField] private int healthPickupAmount = 2;

    public int PlayerMaxHealth => playerMaxHealth;
    public int StartingAmmo => startingAmmo;
    public int MaxAmmo => maxAmmo;
    public int StartingMoney => startingMoney;
    public int PrisonGuardHealth => prisonGuardHealth;
    public float PrisonGuardMoveSpeed => prisonGuardMoveSpeed;
    public int PrisonGuardContactDamage => prisonGuardContactDamage;
    public int RangedGuardHealth => rangedGuardHealth;
    public float RangedGuardMoveSpeed => rangedGuardMoveSpeed;
    public float RangedGuardPreferredDistance => rangedGuardPreferredDistance;
    public float RangedGuardMinimumDistance => rangedGuardMinimumDistance;
    public float RangedGuardAttackInterval => rangedGuardAttackInterval;
    public float RangedGuardProjectileSpeed => rangedGuardProjectileSpeed;
    public int RangedGuardDamage => rangedGuardDamage;
    public float RangedGuardAimTelegraphDuration => rangedGuardAimTelegraphDuration;
    public int LaserDamage => laserDamage;
    public float LaserDamageCooldown => laserDamageCooldown;
    public float LaserWarningDuration => laserWarningDuration;
    public float LaserActiveDuration => laserActiveDuration;
    public float LaserInactiveDuration => laserInactiveDuration;
    public int RiotBruteHealth => riotBruteHealth;
    public float RiotBruteMoveSpeed => riotBruteMoveSpeed;
    public int RiotBruteDamage => riotBruteDamage;
    public float RiotBruteDamageCooldown => riotBruteDamageCooldown;
    public int WardenHealth => wardenHealth;
    public float WardenMoveSpeed => wardenMoveSpeed;
    public float WardenStopDistance => wardenStopDistance;
    public float WardenAttackInterval => wardenAttackInterval;
    public float WardenPhaseTwoMultiplier => wardenPhaseTwoMultiplier;
    public int WardenPhaseOneRadialProjectiles => wardenPhaseOneRadialProjectiles;
    public int WardenPhaseTwoRadialProjectiles => wardenPhaseTwoRadialProjectiles;
    public int WardenProjectileDamage => wardenProjectileDamage;
    public float WardenProjectileSpeed => wardenProjectileSpeed;
    public int ShopHealthPrice => shopHealthPrice;
    public int ShopAmmoPrice => shopAmmoPrice;
    public int ShopWeaponPrice => shopWeaponPrice;
    public int MoneyPickupAmountSmall => moneyPickupAmountSmall;
    public int AmmoPickupAmount => ammoPickupAmount;
    public int HealthPickupAmount => healthPickupAmount;

    private void OnValidate()
    {
        playerMaxHealth = Mathf.Max(1, playerMaxHealth);
        startingAmmo = Mathf.Max(0, startingAmmo);
        maxAmmo = Mathf.Max(startingAmmo, maxAmmo);
        startingMoney = Mathf.Max(0, startingMoney);

        prisonGuardHealth = Mathf.Max(1, prisonGuardHealth);
        prisonGuardMoveSpeed = Mathf.Max(0f, prisonGuardMoveSpeed);
        prisonGuardContactDamage = Mathf.Max(0, prisonGuardContactDamage);

        rangedGuardHealth = Mathf.Max(1, rangedGuardHealth);
        rangedGuardMoveSpeed = Mathf.Max(0f, rangedGuardMoveSpeed);
        rangedGuardPreferredDistance = Mathf.Max(0f, rangedGuardPreferredDistance);
        rangedGuardMinimumDistance = Mathf.Clamp(rangedGuardMinimumDistance, 0f, rangedGuardPreferredDistance);
        rangedGuardAttackInterval = Mathf.Max(0.05f, rangedGuardAttackInterval);
        rangedGuardProjectileSpeed = Mathf.Max(0f, rangedGuardProjectileSpeed);
        rangedGuardDamage = Mathf.Max(0, rangedGuardDamage);
        rangedGuardAimTelegraphDuration = Mathf.Max(0f, rangedGuardAimTelegraphDuration);

        laserDamage = Mathf.Max(0, laserDamage);
        laserDamageCooldown = Mathf.Max(0f, laserDamageCooldown);
        laserWarningDuration = Mathf.Max(0f, laserWarningDuration);
        laserActiveDuration = Mathf.Max(0.05f, laserActiveDuration);
        laserInactiveDuration = Mathf.Max(0.05f, laserInactiveDuration);

        riotBruteHealth = Mathf.Max(1, riotBruteHealth);
        riotBruteMoveSpeed = Mathf.Max(0f, riotBruteMoveSpeed);
        riotBruteDamage = Mathf.Max(0, riotBruteDamage);
        riotBruteDamageCooldown = Mathf.Max(0f, riotBruteDamageCooldown);

        wardenHealth = Mathf.Max(1, wardenHealth);
        wardenMoveSpeed = Mathf.Max(0f, wardenMoveSpeed);
        wardenStopDistance = Mathf.Max(0f, wardenStopDistance);
        wardenAttackInterval = Mathf.Max(0.05f, wardenAttackInterval);
        wardenPhaseTwoMultiplier = Mathf.Max(0.05f, wardenPhaseTwoMultiplier);
        wardenPhaseOneRadialProjectiles = Mathf.Max(1, wardenPhaseOneRadialProjectiles);
        wardenPhaseTwoRadialProjectiles = Mathf.Max(1, wardenPhaseTwoRadialProjectiles);
        wardenProjectileDamage = Mathf.Max(0, wardenProjectileDamage);
        wardenProjectileSpeed = Mathf.Max(0f, wardenProjectileSpeed);

        shopHealthPrice = Mathf.Max(0, shopHealthPrice);
        shopAmmoPrice = Mathf.Max(0, shopAmmoPrice);
        shopWeaponPrice = Mathf.Max(0, shopWeaponPrice);
        moneyPickupAmountSmall = Mathf.Max(0, moneyPickupAmountSmall);
        ammoPickupAmount = Mathf.Max(0, ammoPickupAmount);
        healthPickupAmount = Mathf.Max(0, healthPickupAmount);
    }
}
