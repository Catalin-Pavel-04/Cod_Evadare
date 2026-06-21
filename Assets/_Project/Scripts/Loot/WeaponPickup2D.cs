using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WeaponPickup2D : MonoBehaviour
{
    [SerializeField] private WeaponDefinition2D weapon;
    [SerializeField] private bool destroyOnPickup = true;

    private bool loggedMissingWeapon;
    private bool loggedMissingShooting;

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

        if (weapon == null)
        {
            LogWarningOnce(ref loggedMissingWeapon, $"{name} cannot equip a weapon because no WeaponDefinition2D is assigned.");
            return;
        }

        PlayerShooting2D playerShooting = other.GetComponent<PlayerShooting2D>();

        if (playerShooting == null)
        {
            LogWarningOnce(ref loggedMissingShooting, $"{name} cannot equip '{weapon.WeaponName}' because the player is missing PlayerShooting2D.");
            return;
        }

        playerShooting.EquipWeapon(weapon, true);
        Debug.Log($"Player picked up weapon: {weapon.WeaponName} ({weapon.Rarity}).", this);
        PlayPickupFeedback();

        if (destroyOnPickup)
        {
            Destroy(gameObject);
        }
    }

    private void ConfigureCollider()
    {
        Collider2D pickupCollider = GetComponent<Collider2D>();

        if (pickupCollider != null)
        {
            pickupCollider.isTrigger = true;
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

    private void PlayPickupFeedback()
    {
        DemoAudioManager2D audioManager = FindObjectOfType<DemoAudioManager2D>();

        if (audioManager != null)
        {
            audioManager.PlayPickup();
        }
    }
}
