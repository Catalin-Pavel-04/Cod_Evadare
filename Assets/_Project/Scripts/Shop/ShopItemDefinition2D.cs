using UnityEngine;

public enum ShopItemType2D
{
    Health,
    Ammo,
    Weapon
}

[CreateAssetMenu(fileName = "ShopItemDefinition2D", menuName = "Cod Evadare/Shop Item Definition 2D")]
public class ShopItemDefinition2D : ScriptableObject
{
    [SerializeField] private string displayName = "Shop Item";
    [SerializeField] private ShopItemType2D itemType;
    [SerializeField] private int price = 25;
    [SerializeField] private int amount = 1;
    [SerializeField] private WeaponDefinition2D weaponDefinition;
    [SerializeField] private Sprite icon;

    public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? "Shop Item" : displayName;
    public ShopItemType2D ItemType => itemType;
    public int Price => price;
    public int Amount => amount;
    public WeaponDefinition2D WeaponDefinition => weaponDefinition;
    public Sprite Icon => icon;

    private void OnValidate()
    {
        price = Mathf.Max(0, price);
        amount = Mathf.Max(0, amount);
    }
}
