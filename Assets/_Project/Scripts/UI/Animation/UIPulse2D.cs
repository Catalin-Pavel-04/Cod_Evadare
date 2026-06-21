using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIPulse2D : MonoBehaviour
{
    [SerializeField] private float scaleAmount = 0.06f;
    [SerializeField] private float speed = 3f;

    private RectTransform rectTransform;
    private Vector3 baseScale;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        baseScale = rectTransform != null ? rectTransform.localScale : Vector3.one;
    }

    private void OnEnable()
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }

        baseScale = rectTransform != null ? rectTransform.localScale : Vector3.one;
    }

    private void Update()
    {
        if (rectTransform == null)
        {
            return;
        }

        float pulse = 1f + Mathf.Sin(Time.unscaledTime * Mathf.Max(0f, speed)) * Mathf.Max(0f, scaleAmount);
        rectTransform.localScale = baseScale * pulse;
    }

    private void OnDisable()
    {
        if (rectTransform != null)
        {
            rectTransform.localScale = baseScale;
        }
    }
}
