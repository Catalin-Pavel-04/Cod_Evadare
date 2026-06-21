using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ShopItem2D : MonoBehaviour
{
    [SerializeField] private ShopItemDefinition2D itemDefinition;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private bool singlePurchase = true;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private ShopUI2D shopUI;

    private GameObject nearbyPlayer;
    private bool loggedMissingDefinition;
    private bool loggedMissingResources;
    private bool loggedMissingHealth;
    private bool loggedMissingShooting;
    private bool purchased;

    private void Awake()
    {
        ConfigureComponents();
    }

    private void Reset()
    {
        ConfigureComponents();
    }

    private void Update()
    {
        if (nearbyPlayer == null || purchased)
        {
            return;
        }

        if (Input.GetKeyDown(interactKey))
        {
            TryBuy();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null || !other.CompareTag("Player") || purchased)
        {
            return;
        }

        nearbyPlayer = other.gameObject;

        if (shopUI != null)
        {
            shopUI.ShowPrompt(itemDefinition, interactKey);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == null || nearbyPlayer == null || other.gameObject != nearbyPlayer)
        {
            return;
        }

        nearbyPlayer = null;

        if (shopUI != null)
        {
            shopUI.HidePrompt();
        }
    }

    private void TryBuy()
    {
        if (itemDefinition == null)
        {
            LogWarningOnce(ref loggedMissingDefinition, $"{name} cannot be purchased because no ShopItemDefinition2D is assigned.");
            ShowMessage("Shop item is missing");
            return;
        }

        PlayerResources2D resources = nearbyPlayer.GetComponent<PlayerResources2D>();

        if (resources == null)
        {
            LogWarningOnce(ref loggedMissingResources, $"Cannot buy {itemDefinition.DisplayName} because the player is missing PlayerResources2D.");
            ShowMessage("Missing player resources");
            return;
        }

        if (!CanApplyItem(nearbyPlayer))
        {
            return;
        }

        if (!resources.TrySpendMoney(itemDefinition.Price))
        {
            ShowMessage("Not enough money");
            return;
        }

        ApplyItem(nearbyPlayer, resources);
        ShowMessage($"Bought: {itemDefinition.DisplayName}");
        Debug.Log($"Player bought '{itemDefinition.DisplayName}' for ${itemDefinition.Price}.", this);
        PlayShopFeedback();

        if (singlePurchase)
        {
            purchased = true;
            nearbyPlayer = null;

            if (shopUI != null)
            {
                shopUI.HidePrompt();
            }

            gameObject.SetActive(false);
        }
    }

    private bool CanApplyItem(GameObject player)
    {
        switch (itemDefinition.ItemType)
        {
            case ShopItemType2D.Health:
                if (player.GetComponent<PlayerHealth2D>() == null)
                {
                    LogWarningOnce(ref loggedMissingHealth, $"Cannot buy {itemDefinition.DisplayName} because the player is missing PlayerHealth2D.");
                    ShowMessage("Missing player health");
                    return false;
                }

                return true;

            case ShopItemType2D.Ammo:
                return true;

            case ShopItemType2D.Weapon:
                if (itemDefinition.WeaponDefinition == null)
                {
                    LogWarningOnce(ref loggedMissingDefinition, $"Cannot buy {itemDefinition.DisplayName} because no weapon definition is assigned.");
                    ShowMessage("Weapon is missing");
                    return false;
                }

                if (player.GetComponent<PlayerShooting2D>() == null)
                {
                    LogWarningOnce(ref loggedMissingShooting, $"Cannot buy {itemDefinition.DisplayName} because the player is missing PlayerShooting2D.");
                    ShowMessage("Missing player weapon system");
                    return false;
                }

                return true;
        }

        return false;
    }

    private void ApplyItem(GameObject player, PlayerResources2D resources)
    {
        switch (itemDefinition.ItemType)
        {
            case ShopItemType2D.Health:
                PlayerHealth2D health = player.GetComponent<PlayerHealth2D>();

                if (health != null)
                {
                    health.Heal(itemDefinition.Amount);
                }

                break;

            case ShopItemType2D.Ammo:
                resources.AddAmmo(itemDefinition.Amount);
                break;

            case ShopItemType2D.Weapon:
                PlayerShooting2D shooting = player.GetComponent<PlayerShooting2D>();

                if (shooting != null && itemDefinition.WeaponDefinition != null)
                {
                    shooting.EquipWeapon(itemDefinition.WeaponDefinition, true);
                }

                break;
        }
    }

    private void ConfigureComponents()
    {
        Collider2D itemCollider = GetComponent<Collider2D>();

        if (itemCollider != null)
        {
            itemCollider.isTrigger = true;
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (itemDefinition != null && itemDefinition.Icon != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = itemDefinition.Icon;
        }
    }

    private void ShowMessage(string message)
    {
        if (shopUI != null)
        {
            shopUI.ShowMessage(message);
        }
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

    private void PlayShopFeedback()
    {
        DemoAudioManager2D audioManager = FindObjectOfType<DemoAudioManager2D>();

        if (audioManager != null)
        {
            audioManager.PlayShop();
        }
    }
}
