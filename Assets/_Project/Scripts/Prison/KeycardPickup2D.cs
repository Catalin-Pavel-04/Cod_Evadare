using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class KeycardPickup2D : MonoBehaviour
{
    [SerializeField] private int keycardAmount = 1;
    [SerializeField] private bool destroyOnPickup = true;

    private bool loggedMissingKeyring;

    private void Awake()
    {
        ConfigureCollider();
    }

    private void Reset()
    {
        ConfigureCollider();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null || !other.CompareTag("Player"))
        {
            return;
        }

        PlayerKeyring2D keyring = other.GetComponent<PlayerKeyring2D>();

        if (keyring == null)
        {
            keyring = other.GetComponentInParent<PlayerKeyring2D>();
        }

        if (keyring == null)
        {
            if (!loggedMissingKeyring)
            {
                Debug.LogWarning($"{name} cannot be collected because the player is missing PlayerKeyring2D.", this);
                loggedMissingKeyring = true;
            }

            return;
        }

        keyring.AddKeycard(keycardAmount);
        PlayPickupFeedback();
        ShowObjectiveHint("Keycard collected");
        Debug.Log($"Player collected keycard pickup '{name}' for {Mathf.Max(0, keycardAmount)} keycard(s).", this);

        if (destroyOnPickup)
        {
            Destroy(gameObject);
        }
    }

    private void ConfigureCollider()
    {
        Collider2D pickupCollider = GetComponent<Collider2D>();

        if (pickupCollider != null)
        {
            pickupCollider.isTrigger = true;
        }
    }

    private void PlayPickupFeedback()
    {
        DemoAudioManager2D audioManager = FindObjectOfType<DemoAudioManager2D>();

        if (audioManager != null)
        {
            audioManager.PlayKeycard();
        }
    }

    private void ShowObjectiveHint(string message)
    {
        ObjectiveUI2D objectiveUI = FindObjectOfType<ObjectiveUI2D>();

        if (objectiveUI != null)
        {
            objectiveUI.ShowTemporaryHint(message, 2f);
        }
    }

    private void OnValidate()
    {
        keycardAmount = Mathf.Max(0, keycardAmount);
    }
}
