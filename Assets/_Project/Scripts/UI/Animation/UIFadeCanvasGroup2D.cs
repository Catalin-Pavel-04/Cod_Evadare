using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIFadeCanvasGroup2D : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 0.18f;

    private CanvasGroup canvasGroup;
    private Coroutine fadeRoutine;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Show()
    {
        FadeTo(1f, true);
    }

    public void Hide()
    {
        FadeTo(0f, false);
    }

    public void ShowInstant()
    {
        SetVisible(1f, true);
    }

    public void HideInstant()
    {
        SetVisible(0f, false);
    }

    private void FadeTo(float targetAlpha, bool interactable)
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        if (canvasGroup == null)
        {
            return;
        }

        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }

        fadeRoutine = StartCoroutine(FadeRoutine(targetAlpha, interactable));
    }

    private IEnumerator FadeRoutine(float targetAlpha, bool interactable)
    {
        float startAlpha = canvasGroup.alpha;
        float duration = Mathf.Max(0f, fadeDuration);
        float elapsed = 0f;

        if (targetAlpha > 0f)
        {
            gameObject.SetActive(true);
        }

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = duration > 0f ? Mathf.Clamp01(elapsed / duration) : 1f;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }

        SetVisible(targetAlpha, interactable);
        fadeRoutine = null;
    }

    private void SetVisible(float alpha, bool interactable)
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        if (canvasGroup == null)
        {
            return;
        }

        canvasGroup.alpha = alpha;
        canvasGroup.interactable = interactable;
        canvasGroup.blocksRaycasts = interactable;

        if (alpha <= 0f)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnValidate()
    {
        fadeDuration = Mathf.Max(0f, fadeDuration);
    }
}
