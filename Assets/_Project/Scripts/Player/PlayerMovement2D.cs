using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement2D : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D body;
    private Camera mainCamera;
    private Vector2 movementInput;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.freezeRotation = true;

        CacheMainCamera();
    }

    private void Update()
    {
        movementInput = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical"));

        if (movementInput.sqrMagnitude > 1f)
        {
            movementInput.Normalize();
        }

        RotateTowardMouse();
    }

    private void FixedUpdate()
    {
        Vector2 nextPosition = body.position + movementInput * moveSpeed * Time.fixedDeltaTime;
        body.MovePosition(nextPosition);
    }

    private void CacheMainCamera()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void RotateTowardMouse()
    {
        if (mainCamera == null)
        {
            CacheMainCamera();
        }

        if (mainCamera == null)
        {
            return;
        }

        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 aimDirection = (Vector2)mouseWorldPosition - body.position;

        if (aimDirection.sqrMagnitude < 0.0001f)
        {
            return;
        }

        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
