using System;
using System.Collections;
using UnityEngine;

public class PlayerShooting2D : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float fireCooldown = 0.15f;
    [SerializeField] private bool useAmmo;
    [SerializeField] private int ammoPerShot = 1;
    [SerializeField] private PlayerResources2D playerResources;
    [SerializeField] private int magazineSize = 12;
    [SerializeField] private int currentMagazineAmmo = 12;
    [SerializeField] private float reloadDuration = 1f;
    [SerializeField] private KeyCode reloadKey = KeyCode.R;
    [SerializeField] private bool autoReloadWhenEmpty;

    public event Action<int, int> MagazineChanged;
    public event Action<bool> ReloadStateChanged;

    public int CurrentMagazineAmmo => currentMagazineAmmo;
    public int MagazineSize => magazineSize;
    public bool IsReloading { get; private set; }

    private float nextFireTime;
    private bool loggedMissingFirePoint;
    private bool loggedMissingBulletPrefab;
    private bool loggedMissingBulletComponent;
    private bool loggedMissingPlayerResources;
    private bool loggedOutOfAmmo;
    private bool loggedNoReserveAmmo;
    private Coroutine reloadRoutine;

    private void Awake()
    {
        magazineSize = Mathf.Max(1, magazineSize);
        currentMagazineAmmo = Mathf.Clamp(currentMagazineAmmo, 0, magazineSize);

        if (useAmmo && playerResources == null)
        {
            playerResources = GetComponent<PlayerResources2D>();
        }
    }

    private void Update()
    {
        if (useAmmo && Input.GetKeyDown(reloadKey))
        {
            TryStartReload();
        }

        if (!Input.GetMouseButton(0))
        {
            loggedOutOfAmmo = false;
            return;
        }

        if (Time.time < nextFireTime)
        {
            return;
        }

        nextFireTime = Time.time + fireCooldown;
        Shoot();
    }

    private void OnDisable()
    {
        if (reloadRoutine != null)
        {
            StopCoroutine(reloadRoutine);
            reloadRoutine = null;
        }

        if (IsReloading)
        {
            IsReloading = false;
            ReloadStateChanged?.Invoke(false);
        }
    }

    private void Shoot()
    {
        if (firePoint == null)
        {
            LogWarningOnce(ref loggedMissingFirePoint, "PlayerShooting2D cannot shoot because firePoint is not assigned.");
            return;
        }

        if (bulletPrefab == null)
        {
            LogWarningOnce(ref loggedMissingBulletPrefab, "PlayerShooting2D cannot shoot because bulletPrefab is not assigned.");
            return;
        }

        if (!CanSpendAmmo())
        {
            return;
        }

        GameObject bulletObject = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet2D bullet = bulletObject.GetComponent<Bullet2D>();

        if (bullet == null)
        {
            LogWarningOnce(ref loggedMissingBulletComponent, $"The bullet prefab '{bulletPrefab.name}' is missing a Bullet2D component.");
            Destroy(bulletObject);
            return;
        }

        bullet.Fire(firePoint.right);
    }

    private bool CanSpendAmmo()
    {
        if (!useAmmo)
        {
            return true;
        }

        if (IsReloading)
        {
            return false;
        }

        int shotCost = Mathf.Max(0, ammoPerShot);

        if (shotCost == 0)
        {
            return true;
        }

        if (currentMagazineAmmo < shotCost)
        {
            HandleEmptyMagazine();
            return false;
        }

        currentMagazineAmmo -= shotCost;
        MagazineChanged?.Invoke(currentMagazineAmmo, magazineSize);
        loggedOutOfAmmo = false;
        loggedNoReserveAmmo = false;
        return true;
    }

    private void HandleEmptyMagazine()
    {
        if (autoReloadWhenEmpty && TryStartReload())
        {
            return;
        }

        if (!loggedOutOfAmmo)
        {
            Debug.Log("Magazine is empty. Press R to reload.", this);
            loggedOutOfAmmo = true;
        }
    }

    private bool TryStartReload()
    {
        if (!useAmmo || IsReloading)
        {
            return false;
        }

        if (currentMagazineAmmo >= magazineSize)
        {
            return false;
        }

        if (playerResources == null)
        {
            playerResources = GetComponent<PlayerResources2D>();
        }

        if (playerResources == null)
        {
            LogWarningOnce(ref loggedMissingPlayerResources, "PlayerShooting2D cannot reload because PlayerResources2D is missing.");
            return false;
        }

        if (playerResources.CurrentReserveAmmo <= 0)
        {
            LogNoReserveAmmo();
            return false;
        }

        reloadRoutine = StartCoroutine(ReloadRoutine());
        return true;
    }

    private IEnumerator ReloadRoutine()
    {
        IsReloading = true;
        ReloadStateChanged?.Invoke(true);
        Debug.Log("Started reload.", this);

        if (reloadDuration > 0f)
        {
            yield return new WaitForSeconds(reloadDuration);
        }

        int missingAmmo = Mathf.Max(0, magazineSize - currentMagazineAmmo);
        int loadedAmmo = playerResources != null ? playerResources.SpendAmmoUpTo(missingAmmo) : 0;

        if (loadedAmmo > 0)
        {
            currentMagazineAmmo = Mathf.Clamp(currentMagazineAmmo + loadedAmmo, 0, magazineSize);
            MagazineChanged?.Invoke(currentMagazineAmmo, magazineSize);
        }

        IsReloading = false;
        reloadRoutine = null;
        loggedNoReserveAmmo = false;
        loggedOutOfAmmo = false;
        ReloadStateChanged?.Invoke(false);
        Debug.Log($"Reload complete. Magazine: {currentMagazineAmmo}/{magazineSize}.", this);
    }

    private void LogNoReserveAmmo()
    {
        if (loggedNoReserveAmmo)
        {
            return;
        }

        Debug.Log("No reserve ammo.", this);
        loggedNoReserveAmmo = true;
    }

    private void LogWarningOnce(ref bool alreadyLogged, string message)
    {
        if (alreadyLogged)
        {
            return;
        }

        Debug.LogWarning(message, this);
        alreadyLogged = true;
    }

    private void OnValidate()
    {
        ammoPerShot = Mathf.Max(0, ammoPerShot);
        magazineSize = Mathf.Max(1, magazineSize);
        currentMagazineAmmo = Mathf.Clamp(currentMagazineAmmo, 0, magazineSize);
        reloadDuration = Mathf.Max(0f, reloadDuration);
    }
}
