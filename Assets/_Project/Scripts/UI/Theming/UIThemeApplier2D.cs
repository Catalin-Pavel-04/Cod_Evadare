using UnityEngine;
using UnityEngine.UI;

public class UIThemeApplier2D : MonoBehaviour
{
    [SerializeField] private UITheme2D theme;
    [SerializeField] private Image[] panelImages;
    [SerializeField] private Image[] buttonImages;
    [SerializeField] private Image[] accentImages;
    [SerializeField] private Text[] accentTexts;

    private void Start()
    {
        ApplyTheme();
    }

    public void ApplyTheme()
    {
        if (theme == null)
        {
            return;
        }

        ApplyImages(panelImages, theme.PanelSprite, theme.PanelColor, Image.Type.Sliced);
        ApplyImages(buttonImages, theme.ButtonSprite, Color.white, Image.Type.Sliced);
        ApplyImages(accentImages, null, theme.PrimaryAccent, Image.Type.Simple);

        if (accentTexts == null)
        {
            return;
        }

        foreach (Text text in accentTexts)
        {
            if (text != null)
            {
                text.color = theme.PrimaryAccent;
            }
        }
    }

    private static void ApplyImages(Image[] images, Sprite sprite, Color color, Image.Type imageType)
    {
        if (images == null)
        {
            return;
        }

        foreach (Image image in images)
        {
            if (image == null)
            {
                continue;
            }

            if (sprite != null)
            {
                image.sprite = sprite;
                image.type = imageType;
            }

            image.color = color;
        }
    }
}
