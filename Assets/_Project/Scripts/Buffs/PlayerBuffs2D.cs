using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuffs2D : MonoBehaviour
{
    public event Action<BuffDefinition2D> BuffApplied;

    public int AppliedBuffCount => appliedBuffNames.Count;
    public IReadOnlyList<string> AppliedBuffNames => appliedBuffNames;
    public string LastAppliedBuffName => appliedBuffNames.Count > 0 ? appliedBuffNames[appliedBuffNames.Count - 1] : string.Empty;

    private readonly List<string> appliedBuffNames = new List<string>();

    private PlayerHealth2D health;
    private PlayerMovement2D movement;
    private PlayerShooting2D shooting;
    private PlayerResources2D resources;

    private bool loggedMissingHealth;
    private bool loggedMissingMovement;
    private bool loggedMissingShooting;
    private bool loggedMissingResources;

    private void Awake()
    {
        CacheComponents();
    }

    public void ApplyBuff(BuffDefinition2D buff)
    {
        if (buff == null)
        {
            return;
        }

        CacheComponents();

        if (!TryApplyBuff(buff))
        {
            return;
        }

        appliedBuffNames.Add(buff.DisplayName);
        BuffApplied?.Invoke(buff);
        Debug.Log($"Applied buff: {buff.DisplayName}", this);
    }

    private bool TryApplyBuff(BuffDefinition2D buff)
    {
        switch (buff.BuffType)
        {
            case BuffType2D.MaxHealth:
                if (health == null)
                {
                    LogMissingComponent(ref loggedMissingHealth, "PlayerHealth2D");
                    return false;
                }

                health.AddMaxHealth(buff.Amount, true);
                return true;

            case BuffType2D.Heal:
                if (health == null)
                {
                    LogMissingComponent(ref loggedMissingHealth, "PlayerHealth2D");
                    return false;
                }

                health.Heal(buff.Amount);
                return true;

            case BuffType2D.MoveSpeedMultiplier:
                if (movement == null)
                {
                    LogMissingComponent(ref loggedMissingMovement, "PlayerMovement2D");
                    return false;
                }

                movement.AddMoveSpeedMultiplier(buff.Multiplier);
                return true;

            case BuffType2D.FireRateMultiplier:
                if (shooting == null)
                {
                    LogMissingComponent(ref loggedMissingShooting, "PlayerShooting2D");
                    return false;
                }

                shooting.AddFireRateMultiplier(buff.Multiplier);
                return true;

            case BuffType2D.DamageBonus:
                if (shooting == null)
                {
                    LogMissingComponent(ref loggedMissingShooting, "PlayerShooting2D");
                    return false;
                }

                shooting.AddProjectileDamageBonus(buff.Amount);
                return true;

            case BuffType2D.ReloadSpeedMultiplier:
                if (shooting == null)
                {
                    LogMissingComponent(ref loggedMissingShooting, "PlayerShooting2D");
                    return false;
                }

                shooting.AddReloadSpeedMultiplier(buff.Multiplier);
                return true;

            case BuffType2D.MaxAmmo:
                if (resources == null)
                {
                    LogMissingComponent(ref loggedMissingResources, "PlayerResources2D");
                    return false;
                }

                resources.AddMaxAmmo(buff.Amount, true);
                return true;

            case BuffType2D.MoneyBonus:
                if (resources == null)
                {
                    LogMissingComponent(ref loggedMissingResources, "PlayerResources2D");
                    return false;
                }

                resources.AddMoney(buff.Amount);
                return true;
        }

        return false;
    }

    private void CacheComponents()
    {
        if (health == null)
        {
            health = GetComponent<PlayerHealth2D>();
        }

        if (movement == null)
        {
            movement = GetComponent<PlayerMovement2D>();
        }

        if (shooting == null)
        {
            shooting = GetComponent<PlayerShooting2D>();
        }

        if (resources == null)
        {
            resources = GetComponent<PlayerResources2D>();
        }
    }

    private void LogMissingComponent(ref bool alreadyLogged, string componentName)
    {
        if (alreadyLogged)
        {
            return;
        }

        Debug.LogWarning($"Cannot apply buff because the player is missing {componentName}.", this);
        alreadyLogged = true;
    }
}
