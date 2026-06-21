using UnityEngine;
using UnityEngine.EventSystems;

public class UIHoverSound2D : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    private DemoAudioManager2D audioManager;

    public void OnPointerEnter(PointerEventData eventData)
    {
        EnsureAudioManager();

        if (audioManager != null)
        {
            audioManager.PlayPickup();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        EnsureAudioManager();

        if (audioManager != null)
        {
            audioManager.PlayShop();
        }
    }

    private void EnsureAudioManager()
    {
        if (audioManager == null)
        {
            audioManager = FindObjectOfType<DemoAudioManager2D>();
        }
    }
}
