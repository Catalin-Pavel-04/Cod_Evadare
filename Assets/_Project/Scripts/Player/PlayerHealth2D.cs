using System;
using System.Collections;
using UnityEngine;

public class PlayerHealth2D : MonoBehaviour
{
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private float invincibilityDuration = 0.75f;
    [SerializeField] private bool destroyOnDeath;

    public event Action<int, int> HealthChanged;
    public event Action PlayerDied;

    public int CurrentHealth { get; private set; }
    public int MaxHealth => maxHealth;
    public bool IsDead { get; private set; }
    public bool IsInvincible { get; private set; }

    private Rigidbody2D body;
    private SpriteRenderer spriteRenderer;
    private PlayerMovement2D movement;
    private PlayerShooting2D shooting;
    private Coroutine invincibilityRoutine;
    private Color originalSpriteColor;
    private bool originalSpriteEnabled = true;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        movement = GetComponent<PlayerMovement2D>();
        shooting = GetComponent<PlayerShooting2D>();

        if (spriteRenderer != null)
        {
            originalSpriteColor = spriteRenderer.color;
            originalSpriteEnabled = spriteRenderer.enabled;
        }

        ResetHealth();
    }

    public void TakeDamage(int damage)
    {
        if (IsDead || IsInvincible)
        {
            return;
        }

        int clampedDamage = Mathf.Max(0, damage);

        if (clampedDamage == 0)
        {
            return;
        }

        int previousHealth = CurrentHealth;
        CurrentHealth = Mathf.Max(0, CurrentHealth - clampedDamage);

        if (CurrentHealth != previousHealth)
        {
            HealthChanged?.Invoke(CurrentHealth, maxHealth);
        }

        Debug.Log($"Player took {clampedDamage} damage. Health: {CurrentHealth}/{maxHealth}.", this);

        if (CurrentHealth <= 0)
        {
            Die();
            return;
        }

        StartInvincibility();
    }

    public void Heal(int amount)
    {
        if (IsDead)
        {
            return;
        }

        int clampedAmount = Mathf.Max(0, amount);

        if (clampedAmount == 0)
        {
            return;
        }

        int previousHealth = CurrentHealth;
        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + clampedAmount);

        if (CurrentHealth != previousHealth)
        {
            HealthChanged?.Invoke(CurrentHealth, maxHealth);
        }
    }

    public void AddMaxHealth(int amount, bool healByIncrease = true)
    {
        int clampedAmount = Mathf.Max(0, amount);

        if (clampedAmount == 0)
        {
            return;
        }

        maxHealth += clampedAmount;

        if (healByIncrease && !IsDead)
        {
            CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + clampedAmount);
        }

        HealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

    public void ResetHealth()
    {
        if (invincibilityRoutine != null)
        {
            StopCoroutine(invincibilityRoutine);
            invincibilityRoutine = null;
        }

        maxHealth = Mathf.Max(1, maxHealth);
        CurrentHealth = maxHealth;
        IsDead = false;
        IsInvincible = false;

        if (movement != null)
        {
            movement.enabled = true;
        }

        if (shooting != null)
        {
            shooting.enabled = true;
        }

        RestoreSprite();
        HealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

    private void StartInvincibility()
    {
        if (invincibilityDuration <= 0f)
        {
            return;
        }

        if (invincibilityRoutine != null)
        {
            StopCoroutine(invincibilityRoutine);
        }

        invincibilityRoutine = StartCoroutine(InvincibilityFrames());
    }

    private IEnumerator InvincibilityFrames()
    {
        IsInvincible = true;

        float elapsed = 0f;
        const float flickerInterval = 0.08f;

        while (elapsed < invincibilityDuration && !IsDead)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
            }

            yield return new WaitForSeconds(flickerInterval);
            elapsed += flickerInterval;
        }

        IsInvincible = false;
        invincibilityRoutine = null;
        RestoreSprite();
    }

    private void Die()
    {
        if (IsDead)
        {
            return;
        }

        IsDead = true;
        IsInvincible = false;

        if (invincibilityRoutine != null)
        {
            StopCoroutine(invincibilityRoutine);
            invincibilityRoutine = null;
        }

        if (movement != null)
        {
            movement.enabled = false;
        }

        if (shooting != null)
        {
            shooting.enabled = false;
        }

        if (body != null)
        {
            body.velocity = Vector2.zero;
        }

        RestoreSprite();
        Debug.Log("Player died.", this);
        PlayerDied?.Invoke();

        if (destroyOnDeath)
        {
            Destroy(gameObject);
        }
    }

    private void RestoreSprite()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        spriteRenderer.enabled = originalSpriteEnabled;
        spriteRenderer.color = originalSpriteColor;
    }

    private void OnValidate()
    {
        maxHealth = Mathf.Max(1, maxHealth);
        invincibilityDuration = Mathf.Max(0f, invincibilityDuration);
    }
}
