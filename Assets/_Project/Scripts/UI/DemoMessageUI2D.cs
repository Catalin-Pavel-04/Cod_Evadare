using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DemoMessageUI2D : MonoBehaviour
{
    [SerializeField] private Text messageText;
    [SerializeField] private float defaultDuration = 2f;

    private Coroutine messageRoutine;

    private void Start()
    {
        ClearMessage();
    }

    public void ShowMessage(string message)
    {
        ShowMessage(message, defaultDuration);
    }

    public void ShowMessage(string message, float duration)
    {
        if (messageText == null)
        {
            return;
        }

        if (messageRoutine != null)
        {
            StopCoroutine(messageRoutine);
        }

        messageText.text = message ?? string.Empty;
        messageRoutine = StartCoroutine(ClearAfterDelay(Mathf.Max(0f, duration)));
    }

    public void ClearMessage()
    {
        if (messageRoutine != null)
        {
            StopCoroutine(messageRoutine);
            messageRoutine = null;
        }

        if (messageText != null)
        {
            messageText.text = string.Empty;
        }
    }

    private IEnumerator ClearAfterDelay(float duration)
    {
        yield return new WaitForSecondsRealtime(duration);
        messageRoutine = null;
        ClearMessage();
    }

    private void OnValidate()
    {
        defaultDuration = Mathf.Max(0f, defaultDuration);
    }
}
