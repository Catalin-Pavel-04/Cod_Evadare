using System.Collections;
using UnityEngine;

public class SimpleCameraShake2D : MonoBehaviour
{
    [SerializeField] private float defaultDuration = 0.12f;
    [SerializeField] private float defaultMagnitude = 0.12f;

    private CameraFollow2D cameraFollow;
    private Coroutine shakeRoutine;

    private void Awake()
    {
        cameraFollow = GetComponent<CameraFollow2D>();
    }

    public void ShakeDefault()
    {
        Shake(defaultDuration, defaultMagnitude);
    }

    public void Shake(float duration, float magnitude)
    {
        if (shakeRoutine != null)
        {
            StopCoroutine(shakeRoutine);
        }

        shakeRoutine = StartCoroutine(ShakeRoutine(Mathf.Max(0f, duration), Mathf.Max(0f, magnitude)));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Vector2 randomOffset = Random.insideUnitCircle * magnitude;
            SetShakeOffset(new Vector3(randomOffset.x, randomOffset.y, 0f));
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        SetShakeOffset(Vector3.zero);
        shakeRoutine = null;
    }

    private void SetShakeOffset(Vector3 offset)
    {
        if (cameraFollow != null)
        {
            cameraFollow.SetShakeOffset(offset);
        }
    }

    private void OnDisable()
    {
        SetShakeOffset(Vector3.zero);
    }

    private void OnValidate()
    {
        defaultDuration = Mathf.Max(0f, defaultDuration);
        defaultMagnitude = Mathf.Max(0f, defaultMagnitude);
    }
}
