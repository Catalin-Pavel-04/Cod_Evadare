using UnityEngine;
using UnityEngine.UI;

public class ResourceUI2D : MonoBehaviour
{
    [SerializeField] private PlayerResources2D playerResources;
    [SerializeField] private PlayerShooting2D playerShooting;
    [SerializeField] private Text ammoText;
    [SerializeField] private Text moneyText;

    private int currentReserveAmmo;
    private int maxReserveAmmo;
    private int currentMoney;
    private int currentMagazineAmmo;
    private int magazineSize;
    private bool isReloading;

    private void Start()
    {
        if (playerResources == null)
        {
            playerResources = FindObjectOfType<PlayerResources2D>();
        }

        if (playerShooting == null)
        {
            playerShooting = FindObjectOfType<PlayerShooting2D>();
        }

        if (playerResources != null)
        {
            playerResources.ResourcesChanged += HandleResourcesChanged;
            currentReserveAmmo = playerResources.CurrentReserveAmmo;
            maxReserveAmmo = playerResources.MaxReserveAmmo;
            currentMoney = playerResources.CurrentMoney;
        }

        if (playerShooting != null)
        {
            playerShooting.MagazineChanged += HandleMagazineChanged;
            playerShooting.ReloadStateChanged += HandleReloadStateChanged;
            currentMagazineAmmo = playerShooting.CurrentMagazineAmmo;
            magazineSize = playerShooting.MagazineSize;
            isReloading = playerShooting.IsReloading;
        }

        Refresh();
    }

    private void OnDestroy()
    {
        if (playerResources != null)
        {
            playerResources.ResourcesChanged -= HandleResourcesChanged;
        }

        if (playerShooting != null)
        {
            playerShooting.MagazineChanged -= HandleMagazineChanged;
            playerShooting.ReloadStateChanged -= HandleReloadStateChanged;
        }
    }

    private void HandleResourcesChanged(int reserveAmmo, int reserveMax, int money)
    {
        currentReserveAmmo = reserveAmmo;
        maxReserveAmmo = reserveMax;
        currentMoney = money;
        Refresh();
    }

    private void HandleMagazineChanged(int magazineAmmo, int maxMagazineAmmo)
    {
        currentMagazineAmmo = magazineAmmo;
        magazineSize = maxMagazineAmmo;
        Refresh();
    }

    private void HandleReloadStateChanged(bool reloading)
    {
        isReloading = reloading;
        Refresh();
    }

    private void Refresh()
    {
        if (ammoText != null)
        {
            ammoText.text = BuildAmmoText();
        }

        if (moneyText != null)
        {
            moneyText.text = $"Money: {currentMoney}";
        }
    }

    private string BuildAmmoText()
    {
        if (playerShooting == null)
        {
            return $"Ammo: {currentReserveAmmo} / {maxReserveAmmo}";
        }

        if (isReloading)
        {
            return $"Reloading... {currentMagazineAmmo} / {magazineSize} | Reserve: {currentReserveAmmo} / {maxReserveAmmo}";
        }

        return $"Ammo: {currentMagazineAmmo} / {magazineSize} | Reserve: {currentReserveAmmo} / {maxReserveAmmo}";
    }
}
