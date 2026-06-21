using UnityEngine;

public class EnemyContactDamage2D : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float damageCooldown = 1f;

    private float nextDamageTime;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision == null)
        {
            return;
        }

        if (!TryDamagePlayer(collision.collider))
        {
            TryDamagePlayer(collision.otherCollider);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryDamagePlayer(other);
    }

    private bool TryDamagePlayer(Collider2D other)
    {
        if (other == null || !other.CompareTag("Player"))
        {
            return false;
        }

        if (Time.time < nextDamageTime)
        {
            return true;
        }

        PlayerHealth2D playerHealth = other.GetComponent<PlayerHealth2D>();

        if (playerHealth == null)
        {
            return true;
        }

        nextDamageTime = Time.time + damageCooldown;
        playerHealth.TakeDamage(damage);
        return true;
    }

    private void OnValidate()
    {
        damage = Mathf.Max(0, damage);
        damageCooldown = Mathf.Max(0f, damageCooldown);
    }
}
