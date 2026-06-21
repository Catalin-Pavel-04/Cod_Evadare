using UnityEngine;
using UnityEngine.UI;

public class LockedGate2D : MonoBehaviour
{
    [SerializeField] private int requiredKeycards = 1;
    [SerializeField] private bool consumeKeycard = true;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D gateCollider;
    [SerializeField] private Text optionalPromptText;
    [SerializeField] private string lockedMessage = "Requires keycard";
    [SerializeField] private string openMessage = "Gate opened";

    private GameObject nearbyPlayer;
    private bool isOpen;
    private bool loggedMissingKeyring;

    private void Awake()
    {
        CacheComponents();
        HidePrompt();
    }

    private void Update()
    {
        if (isOpen || nearbyPlayer == null)
        {
            return;
        }

        if (Input.GetKeyDown(interactKey))
        {
            TryOpenGate();
        }
    }

    public void OpenGate()
    {
        if (isOpen)
        {
            return;
        }

        isOpen = true;
        CacheComponents();

        if (gateCollider != null)
        {
            gateCollider.enabled = false;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        HidePrompt();
        PlayDoorFeedback();
        Debug.Log(openMessage, this);
    }

    private void TryOpenGate()
    {
        PlayerKeyring2D keyring = nearbyPlayer != null ? nearbyPlayer.GetComponent<PlayerKeyring2D>() : null;

        if (keyring == null && nearbyPlayer != null)
        {
            keyring = nearbyPlayer.GetComponentInParent<PlayerKeyring2D>();
        }

        if (keyring == null)
        {
            if (!loggedMissingKeyring)
            {
                Debug.LogWarning($"{name} cannot open because the player is missing PlayerKeyring2D.", this);
                loggedMissingKeyring = true;
            }

            ShowPrompt(lockedMessage);
            return;
        }

        int clampedRequired = Mathf.Max(0, requiredKeycards);

        if (!keyring.HasKeycard(clampedRequired))
        {
            ShowPrompt(lockedMessage);
            Debug.Log(lockedMessage, this);
            return;
        }

        if (consumeKeycard && !keyring.TrySpendKeycard(clampedRequired))
        {
            ShowPrompt(lockedMessage);
            Debug.Log(lockedMessage, this);
            return;
        }

        OpenGate();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isOpen || other == null || !other.CompareTag("Player"))
        {
            return;
        }

        nearbyPlayer = other.gameObject;
        ShowPrompt("Press E to open gate");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == null || nearbyPlayer == null || other.gameObject != nearbyPlayer)
        {
            return;
        }

        nearbyPlayer = null;
        HidePrompt();
    }

    private void CacheComponents()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (gateCollider == null)
        {
            Collider2D[] colliders = GetComponents<Collider2D>();

            foreach (Collider2D candidate in colliders)
            {
                if (candidate != null && !candidate.isTrigger)
                {
                    gateCollider = candidate;
                    return;
                }
            }

            if (colliders.Length > 0)
            {
                gateCollider = colliders[0];
            }
        }
    }

    private void ShowPrompt(string message)
    {
        if (optionalPromptText == null)
        {
            return;
        }

        optionalPromptText.gameObject.SetActive(true);
        optionalPromptText.text = message ?? string.Empty;
    }

    private void HidePrompt()
    {
        if (optionalPromptText != null)
        {
            optionalPromptText.text = string.Empty;
            optionalPromptText.gameObject.SetActive(false);
        }
    }

    private void PlayDoorFeedback()
    {
        DemoAudioManager2D audioManager = FindObjectOfType<DemoAudioManager2D>();

        if (audioManager != null)
        {
            audioManager.PlayDoor();
        }
    }

    private void OnValidate()
    {
        requiredKeycards = Mathf.Max(0, requiredKeycards);
    }
}
