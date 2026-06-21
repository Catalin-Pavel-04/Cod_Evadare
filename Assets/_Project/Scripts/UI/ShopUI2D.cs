using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI2D : MonoBehaviour
{
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Text promptText;
    [SerializeField] private Text messageText;
    [SerializeField] private float messageDuration = 2f;

    private Coroutine messageRoutine;
    private bool promptActive;
    private bool messageActive;

    public void ShowPrompt(ShopItemDefinition2D item, KeyCode key)
    {
        promptActive = true;

        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
        }

        if (promptText != null)
        {
            promptText.text = item != null
                ? $"Press {key} to buy {item.DisplayName} - ${item.Price}"
                : $"Press {key} to buy";
        }
    }

    public void HidePrompt()
    {
        promptActive = false;

        if (promptText != null)
        {
            promptText.text = string.Empty;
        }

        UpdatePanelState();
    }

    public void ShowMessage(string message)
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
        }

        if (messageText != null)
        {
            messageText.text = message;
        }

        if (messageRoutine != null)
        {
            StopCoroutine(messageRoutine);
        }

        messageRoutine = StartCoroutine(ClearMessageAfterDelay());
    }

    private IEnumerator ClearMessageAfterDelay()
    {
        messageActive = true;
        yield return new WaitForSeconds(messageDuration);

        messageActive = false;
        messageRoutine = null;

        if (messageText != null)
        {
            messageText.text = string.Empty;
        }

        UpdatePanelState();
    }

    private void UpdatePanelState()
    {
        if (shopPanel != null && !promptActive && !messageActive)
        {
            shopPanel.SetActive(false);
        }
    }

    private void OnValidate()
    {
        messageDuration = Mathf.Max(0f, messageDuration);
    }
}
