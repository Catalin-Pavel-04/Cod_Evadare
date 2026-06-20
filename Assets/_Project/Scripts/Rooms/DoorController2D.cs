using UnityEngine;

public class DoorController2D : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D doorCollider;

    private void Awake()
    {
        CacheComponents();
    }

    public void OpenDoor()
    {
        CacheComponents();

        if (doorCollider != null)
        {
            doorCollider.enabled = false;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        Debug.Log($"Door '{name}' opened.", this);
    }

    public void CloseDoor()
    {
        CacheComponents();

        if (doorCollider != null)
        {
            doorCollider.enabled = true;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }

        Debug.Log($"Door '{name}' closed.", this);
    }

    private void CacheComponents()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (doorCollider == null)
        {
            doorCollider = GetComponent<Collider2D>();
        }
    }
}
