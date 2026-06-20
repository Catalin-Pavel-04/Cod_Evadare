using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SimpleEnemyChaser2D : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float stopDistance = 1f;

    private Rigidbody2D body;
    private Transform player;
    private bool loggedMissingPlayerTag;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.freezeRotation = true;
    }

    private void Start()
    {
        TryFindPlayer();
    }

    private void FixedUpdate()
    {
        if (player == null)
        {
            TryFindPlayer();
            return;
        }

        Vector2 toPlayer = (Vector2)player.position - body.position;
        float distance = toPlayer.magnitude;

        if (distance <= stopDistance || distance < 0.0001f)
        {
            return;
        }

        float moveDistance = Mathf.Min(moveSpeed * Time.fixedDeltaTime, distance - stopDistance);
        Vector2 nextPosition = body.position + toPlayer.normalized * moveDistance;
        body.MovePosition(nextPosition);
    }

    private void TryFindPlayer()
    {
        try
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            player = playerObject != null ? playerObject.transform : null;
        }
        catch (UnityException)
        {
            if (!loggedMissingPlayerTag)
            {
                Debug.LogWarning("SimpleEnemyChaser2D could not find the Player tag.", this);
                loggedMissingPlayerTag = true;
            }
        }
    }

    private void OnValidate()
    {
        moveSpeed = Mathf.Max(0f, moveSpeed);
        stopDistance = Mathf.Max(0f, stopDistance);
    }
}
