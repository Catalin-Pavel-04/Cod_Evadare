using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;

    private int currentHealth;
    private bool isDead;

    private void Awake()
    {
        currentHealth = Mathf.Max(1, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
        {
            return;
        }

        int clampedDamage = Mathf.Max(0, damage);

        if (clampedDamage == 0)
        {
            Debug.Log($"{name} ignored non-positive damage.", this);
            return;
        }

        currentHealth = Mathf.Max(0, currentHealth - clampedDamage);
        Debug.Log($"{name} took {clampedDamage} damage. Health: {currentHealth}/{maxHealth}.", this);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        Debug.Log($"{name} destroyed.", this);
        Destroy(gameObject);
    }

    private void OnValidate()
    {
        maxHealth = Mathf.Max(1, maxHealth);
    }
}
