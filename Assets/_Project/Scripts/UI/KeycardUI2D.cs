using UnityEngine;
using UnityEngine.UI;

public class KeycardUI2D : MonoBehaviour
{
    [SerializeField] private PlayerKeyring2D playerKeyring;
    [SerializeField] private Text keycardText;

    private void Start()
    {
        if (playerKeyring == null)
        {
            playerKeyring = FindObjectOfType<PlayerKeyring2D>();
        }

        if (playerKeyring != null)
        {
            playerKeyring.KeycardsChanged += Refresh;
            Refresh(playerKeyring.CurrentKeycards);
        }
        else
        {
            Refresh(0);
        }
    }

    private void Refresh(int currentKeycards)
    {
        if (keycardText != null)
        {
            keycardText.text = $"Keycards: {Mathf.Max(0, currentKeycards)}";
        }
    }

    private void OnDestroy()
    {
        if (playerKeyring != null)
        {
            playerKeyring.KeycardsChanged -= Refresh;
        }
    }
}
