using UnityEngine;

public enum ResourcePickupType
{
    Health,
    Ammo,
    Money
}

[RequireComponent(typeof(Collider2D))]
public class ResourcePickup2D : MonoBehaviour
{
    [SerializeField] private ResourcePickupType pickupType;
    [SerializeField] private int amount = 1;
    [SerializeField] private bool destroyOnCollect = true;

    private bool loggedMissingComponent;

    private void Awake()
    {
        ConfigureCollider();
    }

    private void Reset()
    {
        ConfigureCollider();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null || !other.CompareTag("Player"))
        {
            return;
        }

        if (!TryCollect(other.gameObject))
        {
            return;
        }

        Debug.Log($"Player collected {pickupType} pickup for {Mathf.Max(0, amount)}.", this);
        PlayPickupFeedback();

        if (destroyOnCollect)
        {
            Destroy(gameObject);
        }
    }

    private bool TryCollect(GameObject player)
    {
        int clampedAmount = Mathf.Max(0, amount);

        switch (pickupType)
        {
            case ResourcePickupType.Health:
                PlayerHealth2D health = player.GetComponent<PlayerHealth2D>();

                if (health == null)
                {
                    LogMissingComponent("PlayerHealth2D");
                    return false;
                }

                health.Heal(clampedAmount);
                return true;

            case ResourcePickupType.Ammo:
                PlayerResources2D ammoResources = player.GetComponent<PlayerResources2D>();

                if (ammoResources == null)
                {
                    LogMissingComponent("PlayerResources2D");
                    return false;
                }

                ammoResources.AddAmmo(clampedAmount);
                return true;

            case ResourcePickupType.Money:
                PlayerResources2D moneyResources = player.GetComponent<PlayerResources2D>();

                if (moneyResources == null)
                {
                    LogMissingComponent("PlayerResources2D");
                    return false;
                }

                moneyResources.AddMoney(clampedAmount);
                return true;
        }

        return false;
    }

    private void ConfigureCollider()
    {
        Collider2D pickupCollider = GetComponent<Collider2D>();

        if (pickupCollider != null)
        {
            pickupCollider.isTrigger = true;
        }
    }

    private void LogMissingComponent(string componentName)
    {
        if (loggedMissingComponent)
        {
            return;
        }

        Debug.LogWarning($"{name} could not be collected because the player is missing {componentName}.", this);
        loggedMissingComponent = true;
    }

    private void PlayPickupFeedback()
    {
        DemoAudioManager2D audioManager = FindObjectOfType<DemoAudioManager2D>();

        if (audioManager != null)
        {
            audioManager.PlayPickup();
        }
    }

    private void OnValidate()
    {
        amount = Mathf.Max(0, amount);
    }
}
