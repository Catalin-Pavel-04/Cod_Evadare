using UnityEngine;

public enum BuffType2D
{
    MaxHealth,
    Heal,
    MoveSpeedMultiplier,
    FireRateMultiplier,
    DamageBonus,
    ReloadSpeedMultiplier,
    MaxAmmo,
    MoneyBonus
}

[CreateAssetMenu(fileName = "BuffDefinition2D", menuName = "Cod Evadare/Buff Definition 2D")]
public class BuffDefinition2D : ScriptableObject
{
    [SerializeField] private string displayName = "Buff";
    [SerializeField] private string description = "Buff description";
    [SerializeField] private BuffType2D buffType;
    [SerializeField] private int amount = 1;
    [SerializeField] private float multiplier = 1.15f;
    [SerializeField] private Sprite icon;

    public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? "Buff" : displayName;
    public string Description => string.IsNullOrWhiteSpace(description) ? "Buff description" : description;
    public BuffType2D BuffType => buffType;
    public int Amount => amount;
    public float Multiplier => multiplier;
    public Sprite Icon => icon;

    private void OnValidate()
    {
        amount = Mathf.Max(0, amount);
        multiplier = Mathf.Max(0f, multiplier);
    }
}
