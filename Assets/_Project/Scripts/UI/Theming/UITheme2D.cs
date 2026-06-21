using UnityEngine;

[CreateAssetMenu(menuName = "Cod Evadare/UI/UI Theme 2D")]
public class UITheme2D : ScriptableObject
{
    [SerializeField] private string themeName = "Cod Evadare";
    [SerializeField] private Color backgroundColor = new Color(0.02f, 0.025f, 0.03f, 1f);
    [SerializeField] private Color panelColor = new Color(0.05f, 0.065f, 0.08f, 0.94f);
    [SerializeField] private Color primaryAccent = new Color(0.0f, 0.82f, 0.92f, 1f);
    [SerializeField] private Color secondaryAccent = new Color(1f, 0.58f, 0.14f, 1f);
    [SerializeField] private Color warningColor = new Color(1f, 0.72f, 0.16f, 1f);
    [SerializeField] private Color dangerColor = new Color(0.94f, 0.12f, 0.18f, 1f);
    [SerializeField] private Color successColor = new Color(0.18f, 0.85f, 0.35f, 1f);
    [SerializeField] private Sprite panelSprite;
    [SerializeField] private Sprite buttonSprite;
    [SerializeField] private Sprite buttonPressedSprite;
    [SerializeField] private Sprite cardSprite;
    [SerializeField] private Sprite resourceSlotSprite;
    [SerializeField] private Sprite healthBarBackgroundSprite;
    [SerializeField] private Sprite healthBarFillSprite;
    [SerializeField] private Sprite bossBarBackgroundSprite;
    [SerializeField] private Sprite bossBarFillSprite;

    public string ThemeName => themeName;
    public Color BackgroundColor => backgroundColor;
    public Color PanelColor => panelColor;
    public Color PrimaryAccent => primaryAccent;
    public Color SecondaryAccent => secondaryAccent;
    public Color WarningColor => warningColor;
    public Color DangerColor => dangerColor;
    public Color SuccessColor => successColor;
    public Sprite PanelSprite => panelSprite;
    public Sprite ButtonSprite => buttonSprite;
    public Sprite ButtonPressedSprite => buttonPressedSprite;
    public Sprite CardSprite => cardSprite;
    public Sprite ResourceSlotSprite => resourceSlotSprite;
    public Sprite HealthBarBackgroundSprite => healthBarBackgroundSprite;
    public Sprite HealthBarFillSprite => healthBarFillSprite;
    public Sprite BossBarBackgroundSprite => bossBarBackgroundSprite;
    public Sprite BossBarFillSprite => bossBarFillSprite;
}
