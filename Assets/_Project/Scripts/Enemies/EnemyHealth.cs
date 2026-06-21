using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;

    private int currentHealth;
    private bool isDead;

    public event Action<int, int> HealthChanged;
    public event Action<EnemyHealth> Died;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => isDead;

    private void Awake()
    {
        currentHealth = Mathf.Max(1, maxHealth);
        HealthChanged?.Invoke(currentHealth, maxHealth);
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
        HealthChanged?.Invoke(currentHealth, maxHealth);
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
        Died?.Invoke(this);
        Destroy(gameObject);
    }

    private void OnValidate()
    {
        int previousMaxHealth = maxHealth;
        maxHealth = Mathf.Max(1, maxHealth);

        if (Application.isPlaying && previousMaxHealth != maxHealth)
        {
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            HealthChanged?.Invoke(currentHealth, maxHealth);
        }
    }
}
