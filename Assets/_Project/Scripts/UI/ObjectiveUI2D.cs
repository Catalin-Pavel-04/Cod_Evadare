using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveUI2D : MonoBehaviour
{
    [SerializeField] private Text objectiveText;
    [SerializeField] private Text hintText;

    private Coroutine temporaryHintRoutine;
    private string persistentHint = string.Empty;

    public void SetObjective(string objective)
    {
        if (objectiveText != null)
        {
            objectiveText.text = objective ?? string.Empty;
        }
    }

    public void SetHint(string hint)
    {
        if (hintText != null)
        {
            hintText.text = hint ?? string.Empty;
        }

        persistentHint = hint ?? string.Empty;
    }

    public void ClearHint()
    {
        if (hintText != null)
        {
            hintText.text = string.Empty;
        }

        persistentHint = string.Empty;
    }

    public void ShowTemporaryHint(string hint, float duration)
    {
        if (temporaryHintRoutine != null)
        {
            StopCoroutine(temporaryHintRoutine);
        }

        temporaryHintRoutine = StartCoroutine(TemporaryHintRoutine(hint, duration));
    }

    private IEnumerator TemporaryHintRoutine(string hint, float duration)
    {
        if (hintText != null)
        {
            hintText.text = hint ?? string.Empty;
        }

        yield return new WaitForSeconds(Mathf.Max(0f, duration));

        if (hintText != null)
        {
            hintText.text = persistentHint;
        }

        temporaryHintRoutine = null;
    }
}
