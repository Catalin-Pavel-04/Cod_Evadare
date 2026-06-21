using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIShake2D : MonoBehaviour
{
    [SerializeField] private float duration = 0.18f;
    [SerializeField] private float strength = 8f;

    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private Coroutine shakeRoutine;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform != null ? rectTransform.anchoredPosition : Vector2.zero;
    }

    public void Shake()
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }

        if (rectTransform == null)
        {
            return;
        }

        if (shakeRoutine != null)
        {
            StopCoroutine(shakeRoutine);
            rectTransform.anchoredPosition = originalPosition;
        }

        shakeRoutine = StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        originalPosition = rectTransform.anchoredPosition;
        float elapsed = 0f;
        float safeDuration = Mathf.Max(0f, duration);

        while (elapsed < safeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            rectTransform.anchoredPosition = originalPosition + Random.insideUnitCircle * Mathf.Max(0f, strength);
            yield return null;
        }

        rectTransform.anchoredPosition = originalPosition;
        shakeRoutine = null;
    }

    private void OnDisable()
    {
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = originalPosition;
        }
    }
}
