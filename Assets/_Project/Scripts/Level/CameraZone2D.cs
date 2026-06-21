using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CameraZone2D : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float orthographicSize = 6.5f;

    private void Awake()
    {
        ConfigureCollider();
    }

    private void Reset()
    {
        ConfigureCollider();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null || !other.CompareTag("Player"))
        {
            return;
        }

        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if (targetCamera != null)
        {
            targetCamera.orthographicSize = orthographicSize;
        }
    }

    private void ConfigureCollider()
    {
        Collider2D triggerCollider = GetComponent<Collider2D>();

        if (triggerCollider != null)
        {
            triggerCollider.isTrigger = true;
        }
    }

    private void OnValidate()
    {
        orthographicSize = Mathf.Max(0.1f, orthographicSize);
    }
}
