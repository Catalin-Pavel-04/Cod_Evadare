using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DamageZone2D : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float damageCooldown = 1f;
    [SerializeField] private bool active = true;

    private float nextDamageTime;

    private void Awake()
    {
        ConfigureCollider();
    }

    private void Reset()
    {
        ConfigureCollider();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!active || Time.time < nextDamageTime || other == null || !other.CompareTag("Player"))
        {
            return;
        }

        PlayerHealth2D playerHealth = other.GetComponent<PlayerHealth2D>();

        if (playerHealth == null)
        {
            playerHealth = other.GetComponentInParent<PlayerHealth2D>();
        }

        if (playerHealth == null)
        {
            return;
        }

        playerHealth.TakeDamage(damage);
        nextDamageTime = Time.time + Mathf.Max(0.05f, damageCooldown);
    }

    public void SetActive(bool isActive)
    {
        active = isActive;
    }

    private void ConfigureCollider()
    {
        Collider2D zoneCollider = GetComponent<Collider2D>();

        if (zoneCollider != null)
        {
            zoneCollider.isTrigger = true;
        }
    }

    private void OnValidate()
    {
        damage = Mathf.Max(0, damage);
        damageCooldown = Mathf.Max(0.05f, damageCooldown);
    }
}
