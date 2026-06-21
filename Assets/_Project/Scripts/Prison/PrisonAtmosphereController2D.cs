using UnityEngine;

public class PrisonAtmosphereController2D : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] alarmLights;
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private Color normalColor = new Color(0.25f, 0.1f, 0.08f, 0.6f);
    [SerializeField] private Color alarmColor = new Color(1f, 0.08f, 0.04f, 1f);
    [SerializeField] private bool alarmActive = true;

    private void Update()
    {
        if (alarmLights == null || alarmLights.Length == 0)
        {
            return;
        }

        Color targetColor = normalColor;

        if (alarmActive)
        {
            float t = (Mathf.Sin(Time.time * Mathf.Max(0.01f, pulseSpeed)) + 1f) * 0.5f;
            targetColor = Color.Lerp(normalColor, alarmColor, t);
        }

        foreach (SpriteRenderer alarmLight in alarmLights)
        {
            if (alarmLight != null)
            {
                alarmLight.color = targetColor;
            }
        }
    }

    private void OnValidate()
    {
        pulseSpeed = Mathf.Max(0.01f, pulseSpeed);
    }
}
