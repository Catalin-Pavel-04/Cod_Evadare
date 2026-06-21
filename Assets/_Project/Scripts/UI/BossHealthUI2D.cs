using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI2D : MonoBehaviour
{
    [SerializeField] private EnemyHealth bossHealth;
    [SerializeField] private GameObject bossPanel;
    [SerializeField] private Image healthFill;
    [SerializeField] private Text healthText;
    [SerializeField] private Text bossNameText;

    private RectTransform healthFillRect;

    private void Awake()
    {
        ConfigureHealthFill();
    }

    private void Start()
    {
        if (bossHealth != null)
        {
            SetBoss(bossHealth, GetBossName(bossHealth));
            return;
        }

        SetPanelVisible(false);
    }

    public void SetBoss(EnemyHealth newBossHealth, string bossName = "Boss")
    {
        if (bossHealth != null)
        {
            bossHealth.HealthChanged -= Refresh;
            bossHealth.Died -= HandleBossDied;
        }

        bossHealth = newBossHealth;

        if (bossHealth == null)
        {
            SetPanelVisible(false);
            return;
        }

        bossHealth.HealthChanged += Refresh;
        bossHealth.Died += HandleBossDied;

        if (bossNameText != null)
        {
            bossNameText.text = string.IsNullOrWhiteSpace(bossName) ? "Boss" : bossName;
        }

        SetPanelVisible(true);
        Refresh(bossHealth.CurrentHealth, bossHealth.MaxHealth);
    }

    private void Refresh(int currentHealth, int maxHealth)
    {
        int safeMaxHealth = Mathf.Max(1, maxHealth);
        int safeCurrentHealth = Mathf.Clamp(currentHealth, 0, safeMaxHealth);

        if (healthFill != null)
        {
            SetHealthFillAmount((float)safeCurrentHealth / safeMaxHealth);
        }

        if (healthText != null)
        {
            healthText.text = $"HP: {safeCurrentHealth} / {safeMaxHealth}";
        }
    }

    private void HandleBossDied(EnemyHealth deadBoss)
    {
        int maxHealth = deadBoss != null ? deadBoss.MaxHealth : 1;
        Refresh(0, maxHealth);
    }

    private void SetPanelVisible(bool isVisible)
    {
        if (bossPanel != null)
        {
            bossPanel.SetActive(isVisible);
        }
    }

    private string GetBossName(EnemyHealth health)
    {
        if (health == null)
        {
            return "Boss";
        }

        BossMarker2D marker = health.GetComponent<BossMarker2D>();
        return marker != null ? marker.BossName : "Boss";
    }

    private void ConfigureHealthFill()
    {
        if (healthFill == null)
        {
            return;
        }

        healthFill.type = Image.Type.Filled;
        healthFill.fillMethod = Image.FillMethod.Horizontal;
        healthFill.fillOrigin = (int)Image.OriginHorizontal.Left;
        healthFillRect = healthFill.rectTransform;
    }

    private void SetHealthFillAmount(float amount)
    {
        ConfigureHealthFill();

        if (healthFill == null)
        {
            return;
        }

        float clampedAmount = Mathf.Clamp01(amount);
        healthFill.fillAmount = clampedAmount;
        healthFill.enabled = clampedAmount > 0.001f;

        if (healthFillRect == null)
        {
            return;
        }

        healthFillRect.anchorMin = Vector2.zero;
        healthFillRect.anchorMax = new Vector2(clampedAmount, 1f);
        healthFillRect.offsetMin = Vector2.zero;
        healthFillRect.offsetMax = Vector2.zero;
    }

    private void OnDestroy()
    {
        if (bossHealth != null)
        {
            bossHealth.HealthChanged -= Refresh;
            bossHealth.Died -= HandleBossDied;
        }
    }

    private void OnValidate()
    {
        ConfigureHealthFill();
    }
}
