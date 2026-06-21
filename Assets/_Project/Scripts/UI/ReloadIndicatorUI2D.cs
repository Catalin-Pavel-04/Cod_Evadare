using UnityEngine;
using UnityEngine.UI;

public class ReloadIndicatorUI2D : MonoBehaviour
{
    [SerializeField] private PlayerShooting2D playerShooting;
    [SerializeField] private Image reloadFill;
    [SerializeField] private Text reloadText;

    private void OnEnable()
    {
        ResolvePlayerShooting();

        if (playerShooting != null)
        {
            playerShooting.ReloadStateChanged += HandleReloadStateChanged;
        }

        Refresh();
    }

    private void Update()
    {
        Refresh();
    }

    private void OnDisable()
    {
        if (playerShooting != null)
        {
            playerShooting.ReloadStateChanged -= HandleReloadStateChanged;
        }
    }

    private void HandleReloadStateChanged(bool isReloading)
    {
        Refresh();
    }

    private void Refresh()
    {
        ResolvePlayerShooting();

        bool isReloading = playerShooting != null && playerShooting.IsReloading;

        if (reloadFill != null)
        {
            reloadFill.gameObject.SetActive(isReloading);
            reloadFill.fillAmount = isReloading ? 1f : 0f;
        }

        if (reloadText != null)
        {
            reloadText.gameObject.SetActive(isReloading);
            reloadText.text = isReloading ? "Reloading..." : string.Empty;
        }
    }

    private void ResolvePlayerShooting()
    {
        if (playerShooting == null)
        {
            playerShooting = FindObjectOfType<PlayerShooting2D>();
        }
    }
}
