using UnityEngine;
using UnityEngine.UI;

public class WeaponUI2D : MonoBehaviour
{
    [SerializeField] private PlayerShooting2D playerShooting;
    [SerializeField] private Text weaponText;

    private void Start()
    {
        if (playerShooting == null)
        {
            playerShooting = FindObjectOfType<PlayerShooting2D>();
        }

        if (playerShooting != null)
        {
            playerShooting.WeaponChanged += Refresh;
            Refresh(playerShooting.EquippedWeapon);
            return;
        }

        Refresh(null);
    }

    private void OnDestroy()
    {
        if (playerShooting != null)
        {
            playerShooting.WeaponChanged -= Refresh;
        }
    }

    private void Refresh(WeaponDefinition2D weapon)
    {
        if (weaponText == null)
        {
            return;
        }

        weaponText.text = weapon != null
            ? $"Weapon: {weapon.WeaponName} [{weapon.Rarity}]"
            : "Weapon: Default";
    }
}
