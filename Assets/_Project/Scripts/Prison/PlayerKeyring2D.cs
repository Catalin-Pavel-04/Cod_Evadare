using System;
using UnityEngine;

public class PlayerKeyring2D : MonoBehaviour
{
    [SerializeField] private int startingKeycards;

    public event Action<int> KeycardsChanged;

    public int CurrentKeycards { get; private set; }

    private void Awake()
    {
        CurrentKeycards = Mathf.Max(0, startingKeycards);
        KeycardsChanged?.Invoke(CurrentKeycards);
    }

    public void AddKeycard(int amount = 1)
    {
        int clampedAmount = Mathf.Max(0, amount);

        if (clampedAmount == 0)
        {
            return;
        }

        CurrentKeycards += clampedAmount;
        KeycardsChanged?.Invoke(CurrentKeycards);
        Debug.Log($"Collected {clampedAmount} keycard(s). Current keycards: {CurrentKeycards}.", this);
    }

    public bool TrySpendKeycard(int amount = 1)
    {
        int clampedAmount = Mathf.Max(0, amount);

        if (clampedAmount == 0)
        {
            return true;
        }

        if (CurrentKeycards < clampedAmount)
        {
            return false;
        }

        CurrentKeycards -= clampedAmount;
        KeycardsChanged?.Invoke(CurrentKeycards);
        Debug.Log($"Spent {clampedAmount} keycard(s). Current keycards: {CurrentKeycards}.", this);
        return true;
    }

    public bool HasKeycard(int amount = 1)
    {
        int clampedAmount = Mathf.Max(0, amount);
        return CurrentKeycards >= clampedAmount;
    }

    private void OnValidate()
    {
        startingKeycards = Mathf.Max(0, startingKeycards);
    }
}
