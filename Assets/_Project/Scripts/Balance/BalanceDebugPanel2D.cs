using UnityEngine;
using UnityEngine.UI;

public class BalanceDebugPanel2D : MonoBehaviour
{
    [SerializeField] private Text debugText;
    [SerializeField] private PlayerHealth2D playerHealth;
    [SerializeField] private PlayerResources2D playerResources;
    [SerializeField] private PlayerKeyring2D playerKeyring;
    [SerializeField] private bool showDebug;

    private void Update()
    {
        if (debugText == null)
        {
            return;
        }

        if (!showDebug)
        {
            debugText.text = string.Empty;
            debugText.gameObject.SetActive(false);
            return;
        }

        CacheMissingReferences();
        debugText.gameObject.SetActive(true);

        int currentHealth = playerHealth != null ? playerHealth.CurrentHealth : 0;
        int maxHealth = playerHealth != null ? playerHealth.MaxHealth : 0;
        int ammo = playerResources != null ? playerResources.CurrentAmmo : 0;
        int maxAmmo = playerResources != null ? playerResources.MaxAmmo : 0;
        int money = playerResources != null ? playerResources.CurrentMoney : 0;
        int keycards = playerKeyring != null ? playerKeyring.CurrentKeycards : 0;

        debugText.text = $"HP {currentHealth}/{maxHealth} | Ammo {ammo}/{maxAmmo} | Money {money} | Keycards {keycards}";
    }

    private void CacheMissingReferences()
    {
        if (playerHealth == null)
        {
            playerHealth = FindObjectOfType<PlayerHealth2D>();
        }

        if (playerResources == null)
        {
            playerResources = FindObjectOfType<PlayerResources2D>();
        }

        if (playerKeyring == null)
        {
            playerKeyring = FindObjectOfType<PlayerKeyring2D>();
        }
    }
}
