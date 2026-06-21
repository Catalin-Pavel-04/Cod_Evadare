using UnityEngine;

public enum WeaponRarity2D
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

[CreateAssetMenu(fileName = "WeaponDefinition2D", menuName = "Cod Evadare/Weapon Definition 2D")]
public class WeaponDefinition2D : ScriptableObject
{
    [SerializeField] private string weaponName = "Pistol";
    [SerializeField] private WeaponRarity2D rarity = WeaponRarity2D.Common;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float fireCooldown = 0.2f;
    [SerializeField] private int magazineSize = 12;
    [SerializeField] private int ammoPerShot = 1;
    [SerializeField] private float reloadDuration = 1f;
    [SerializeField] private int projectileDamage = 1;
    [SerializeField] private float projectileSpeed = 12f;
    [SerializeField] private float projectileLifetime = 2f;
    [SerializeField] private int projectilesPerShot = 1;
    [SerializeField] private float spreadAngle;

    public string WeaponName => string.IsNullOrWhiteSpace(weaponName) ? "Unnamed Weapon" : weaponName;
    public WeaponRarity2D Rarity => rarity;
    public GameObject BulletPrefab => bulletPrefab;
    public float FireCooldown => fireCooldown;
    public int MagazineSize => magazineSize;
    public int AmmoPerShot => ammoPerShot;
    public float ReloadDuration => reloadDuration;
    public int ProjectileDamage => projectileDamage;
    public float ProjectileSpeed => projectileSpeed;
    public float ProjectileLifetime => projectileLifetime;
    public int ProjectilesPerShot => projectilesPerShot;
    public float SpreadAngle => spreadAngle;

    private void OnValidate()
    {
        fireCooldown = Mathf.Max(0f, fireCooldown);
        magazineSize = Mathf.Max(1, magazineSize);
        ammoPerShot = Mathf.Max(0, ammoPerShot);
        reloadDuration = Mathf.Max(0f, reloadDuration);
        projectileDamage = Mathf.Max(0, projectileDamage);
        projectileSpeed = Mathf.Max(0f, projectileSpeed);
        projectileLifetime = Mathf.Max(0.01f, projectileLifetime);
        projectilesPerShot = Mathf.Max(1, projectilesPerShot);
        spreadAngle = Mathf.Max(0f, spreadAngle);
    }
}
