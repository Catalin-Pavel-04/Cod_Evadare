using UnityEngine;
using UnityEngine.UI;

public class HealthUI2D : MonoBehaviour
{
    [SerializeField] private PlayerHealth2D playerHealth;
    [SerializeField] private Image healthFill;
    [SerializeField] private Text healthText;

    private Sprite runtimeFillSprite;

    private void Awake()
    {
        ResolveMissingReferences();
        ConfigureHealthFill();
    }

    private void Start()
    {
        if (playerHealth == null)
        {
            playerHealth = FindObjectOfType<PlayerHealth2D>();
        }

        if (playerHealth == null)
        {
            Refresh(0, 1);
            return;
        }

        playerHealth.HealthChanged += Refresh;
        Refresh(playerHealth.CurrentHealth, playerHealth.MaxHealth);
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.HealthChanged -= Refresh;
        }

        if (runtimeFillSprite != null)
        {
            Destroy(runtimeFillSprite);
        }
    }

    private void Refresh(int current, int max)
    {
        int safeMax = Mathf.Max(1, max);

        if (healthFill != null)
        {
            healthFill.fillAmount = Mathf.Clamp01((float)current / safeMax);
        }

        if (healthText != null)
        {
            healthText.text = $"HP: {current} / {safeMax}";
        }
    }

    private void ResolveMissingReferences()
    {
        if (healthFill == null)
        {
            Transform fillTransform = transform.Find("HealthFill");
            healthFill = fillTransform != null ? fillTransform.GetComponent<Image>() : null;
        }

        if (healthText == null)
        {
            Transform textTransform = transform.Find("HealthText");
            healthText = textTransform != null ? textTransform.GetComponent<Text>() : null;
        }
    }

    private void ConfigureHealthFill()
    {
        if (healthFill == null)
        {
            return;
        }

        if (healthFill.sprite == null)
        {
            Texture2D texture = Texture2D.whiteTexture;
            runtimeFillSprite = Sprite.Create(
                texture,
                new Rect(0f, 0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                100f);
            runtimeFillSprite.name = "RuntimeHealthFillSprite";
            healthFill.sprite = runtimeFillSprite;
        }

        healthFill.type = Image.Type.Filled;
        healthFill.fillMethod = Image.FillMethod.Horizontal;
        healthFill.fillOrigin = (int)Image.OriginHorizontal.Left;
    }
}
