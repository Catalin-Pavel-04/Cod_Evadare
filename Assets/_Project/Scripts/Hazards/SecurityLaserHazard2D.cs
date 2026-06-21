using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SecurityLaserHazard2D : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float damageCooldown = 0.75f;
    [SerializeField] private bool startsActive = true;
    [SerializeField] private bool toggles = true;
    [SerializeField] private float activeDuration = 2f;
    [SerializeField] private float inactiveDuration = 1.5f;
    [SerializeField] private float warningDuration = 0.45f;
    [SerializeField] private Color inactiveColor = new Color(1f, 0.1f, 0.08f, 0.18f);
    [SerializeField] private Color warningColor = new Color(1f, 0.65f, 0.05f, 0.75f);
    [SerializeField] private Color activeColor = new Color(1f, 0.1f, 0.08f, 0.95f);
    [SerializeField] private bool useWarningBeforeActive = true;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D hazardCollider;

    private float nextDamageTime;
    private bool isActive;
    private Coroutine toggleRoutine;
    private static float nextLaserHintTime;

    private void Awake()
    {
        CacheComponents();

        if (!toggles)
        {
            SetActiveState(startsActive);
            return;
        }

        SetInactiveState();
    }

    private void OnEnable()
    {
        if (toggles && toggleRoutine == null)
        {
            toggleRoutine = StartCoroutine(ToggleRoutine());
        }
    }

    private void OnDisable()
    {
        if (toggleRoutine != null)
        {
            StopCoroutine(toggleRoutine);
            toggleRoutine = null;
        }
    }

    private void Reset()
    {
        CacheComponents();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!isActive || other == null || !other.CompareTag("Player") || Time.time < nextDamageTime)
        {
            return;
        }

        PlayerHealth2D playerHealth = other.GetComponent<PlayerHealth2D>();

        if (playerHealth == null)
        {
            playerHealth = other.GetComponentInParent<PlayerHealth2D>();
        }

        if (playerHealth == null)
        {
            return;
        }

        playerHealth.TakeDamage(damage);
        nextDamageTime = Time.time + Mathf.Max(0f, damageCooldown);
        PlayLaserFeedback();
    }

    private IEnumerator ToggleRoutine()
    {
        while (true)
        {
            SetInactiveState();
            yield return new WaitForSeconds(Mathf.Max(0.05f, inactiveDuration));

            if (useWarningBeforeActive && warningDuration > 0f)
            {
                SetWarningState();
                yield return new WaitForSeconds(Mathf.Max(0.01f, warningDuration));
            }

            SetActiveState(true);
            yield return new WaitForSeconds(Mathf.Max(0.05f, activeDuration));
        }
    }

    private void SetInactiveState()
    {
        isActive = false;
        CacheComponents();

        if (hazardCollider != null)
        {
            hazardCollider.enabled = false;
            hazardCollider.isTrigger = true;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.color = inactiveColor;
        }
    }

    private void SetWarningState()
    {
        isActive = false;
        CacheComponents();

        if (hazardCollider != null)
        {
            hazardCollider.enabled = false;
            hazardCollider.isTrigger = true;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.color = warningColor;
        }

        ShowLaserWarningHint();
    }

    private void SetActiveState(bool active)
    {
        isActive = active;
        CacheComponents();

        if (hazardCollider != null)
        {
            hazardCollider.enabled = active;
            hazardCollider.isTrigger = true;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.color = active ? activeColor : inactiveColor;
        }
    }

    private void CacheComponents()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (hazardCollider == null)
        {
            hazardCollider = GetComponent<Collider2D>();
        }

        if (hazardCollider != null)
        {
            hazardCollider.isTrigger = true;
        }
    }

    private void PlayLaserFeedback()
    {
        DemoAudioManager2D audioManager = FindObjectOfType<DemoAudioManager2D>();

        if (audioManager != null)
        {
            audioManager.PlayLaser();
        }
    }

    private void ShowLaserWarningHint()
    {
        if (Time.time < nextLaserHintTime)
        {
            return;
        }

        ObjectiveUI2D objectiveUI = FindObjectOfType<ObjectiveUI2D>();

        if (objectiveUI != null)
        {
            objectiveUI.ShowTemporaryHint("Warning: security laser active", 1.25f);
            nextLaserHintTime = Time.time + 3f;
        }
    }

    private void OnValidate()
    {
        damage = Mathf.Max(0, damage);
        damageCooldown = Mathf.Max(0f, damageCooldown);
        activeDuration = Mathf.Max(0.05f, activeDuration);
        inactiveDuration = Mathf.Max(0.05f, inactiveDuration);
        warningDuration = Mathf.Max(0f, warningDuration);
    }
}
