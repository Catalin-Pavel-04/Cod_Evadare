using System;
using UnityEngine;

public class PlayerResources2D : MonoBehaviour
{
    [SerializeField] private int startingAmmo = 30;
    [SerializeField] private int maxAmmo = 99;
    [SerializeField] private int startingMoney;

    public event Action<int, int, int> ResourcesChanged;

    public int CurrentAmmo { get; private set; }
    public int MaxAmmo => maxAmmo;
    public int CurrentReserveAmmo => CurrentAmmo;
    public int MaxReserveAmmo => maxAmmo;
    public int CurrentMoney { get; private set; }

    private void Awake()
    {
        ResetResources();
    }

    public bool TrySpendAmmo(int amount)
    {
        int clampedAmount = Mathf.Max(0, amount);

        if (clampedAmount == 0)
        {
            return true;
        }

        if (CurrentAmmo < clampedAmount)
        {
            return false;
        }

        CurrentAmmo -= clampedAmount;
        ResourcesChanged?.Invoke(CurrentAmmo, maxAmmo, CurrentMoney);
        return true;
    }

    public int SpendAmmoUpTo(int requestedAmount)
    {
        int clampedAmount = Mathf.Max(0, requestedAmount);

        if (clampedAmount == 0 || CurrentAmmo <= 0)
        {
            return 0;
        }

        int spentAmount = Mathf.Min(CurrentAmmo, clampedAmount);
        CurrentAmmo -= spentAmount;
        ResourcesChanged?.Invoke(CurrentAmmo, maxAmmo, CurrentMoney);
        return spentAmount;
    }

    public bool CanAfford(int cost)
    {
        int clampedCost = Mathf.Max(0, cost);
        return CurrentMoney >= clampedCost;
    }

    public bool TrySpendMoney(int amount)
    {
        int clampedAmount = Mathf.Max(0, amount);

        if (clampedAmount == 0)
        {
            return true;
        }

        if (CurrentMoney < clampedAmount)
        {
            return false;
        }

        CurrentMoney -= clampedAmount;
        ResourcesChanged?.Invoke(CurrentAmmo, maxAmmo, CurrentMoney);
        return true;
    }

    public void AddAmmo(int amount)
    {
        int clampedAmount = Mathf.Max(0, amount);

        if (clampedAmount == 0)
        {
            return;
        }

        int previousAmmo = CurrentAmmo;
        CurrentAmmo = Mathf.Min(maxAmmo, CurrentAmmo + clampedAmount);

        if (CurrentAmmo != previousAmmo)
        {
            ResourcesChanged?.Invoke(CurrentAmmo, maxAmmo, CurrentMoney);
        }
    }

    public void AddMaxAmmo(int amount, bool addAmmoToo = true)
    {
        int clampedAmount = Mathf.Max(0, amount);

        if (clampedAmount == 0)
        {
            return;
        }

        maxAmmo += clampedAmount;

        if (addAmmoToo)
        {
            CurrentAmmo = Mathf.Min(maxAmmo, CurrentAmmo + clampedAmount);
        }

        ResourcesChanged?.Invoke(CurrentAmmo, maxAmmo, CurrentMoney);
    }

    public void AddMoney(int amount)
    {
        int clampedAmount = Mathf.Max(0, amount);

        if (clampedAmount == 0)
        {
            return;
        }

        int previousMoney = CurrentMoney;
        CurrentMoney = Mathf.Max(0, CurrentMoney + clampedAmount);

        if (CurrentMoney != previousMoney)
        {
            ResourcesChanged?.Invoke(CurrentAmmo, maxAmmo, CurrentMoney);
        }
    }

    public void ResetResources()
    {
        maxAmmo = Mathf.Max(0, maxAmmo);
        CurrentAmmo = Mathf.Clamp(startingAmmo, 0, maxAmmo);
        CurrentMoney = Mathf.Max(0, startingMoney);
        ResourcesChanged?.Invoke(CurrentAmmo, maxAmmo, CurrentMoney);
    }

    private void OnValidate()
    {
        maxAmmo = Mathf.Max(0, maxAmmo);
        startingAmmo = Mathf.Clamp(startingAmmo, 0, maxAmmo);
        startingMoney = Mathf.Max(0, startingMoney);
    }
}
