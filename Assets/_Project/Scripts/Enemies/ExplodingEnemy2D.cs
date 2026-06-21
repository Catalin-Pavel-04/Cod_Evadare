using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ExplodingEnemy2D : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1.8f;
    [SerializeField] private float explosionRadius = 1.6f;
    [SerializeField] private int explosionDamage = 2;
    [SerializeField] private float triggerDistance = 1.1f;
    [SerializeField] private GameObject explosionVisualPrefab;

    private Rigidbody2D body;
    private Transform target;
    private EnemyHealth enemyHealth;
    private bool hasExploded;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.freezeRotation = true;
        enemyHealth = GetComponent<EnemyHealth>();

        if (enemyHealth != null)
        {
            enemyHealth.Died += HandleEnemyDied;
        }
    }

    private void Start()
    {
        FindTargetIfMissing();
    }

    private void FixedUpdate()
    {
        if (hasExploded)
        {
            return;
        }

        FindTargetIfMissing();

        if (target == null)
        {
            return;
        }

        Vector2 currentPosition = body.position;
        Vector2 toTarget = (Vector2)target.position - currentPosition;

        if (toTarget.sqrMagnitude <= triggerDistance * triggerDistance)
        {
            Explode();
            return;
        }

        if (toTarget.sqrMagnitude < 0.0001f)
        {
            return;
        }

        body.MovePosition(currentPosition + toTarget.normalized * moveSpeed * Time.fixedDeltaTime);
    }

    private void HandleEnemyDied(EnemyHealth deadEnemy)
    {
        Explode();
    }

    private void Explode()
    {
        if (hasExploded)
        {
            return;
        }

        hasExploded = true;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, Mathf.Max(0f, explosionRadius));

        foreach (Collider2D hit in hits)
        {
            if (hit == null)
            {
                continue;
            }

            PlayerHealth2D playerHealth = hit.GetComponent<PlayerHealth2D>();

            if (playerHealth == null)
            {
                playerHealth = hit.GetComponentInParent<PlayerHealth2D>();
            }

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(explosionDamage);
            }
        }

        if (explosionVisualPrefab != null)
        {
            Instantiate(explosionVisualPrefab, transform.position, Quaternion.identity);
        }

        DemoAudioManager2D audioManager = FindObjectOfType<DemoAudioManager2D>();

        if (audioManager != null)
        {
            audioManager.PlayExplosion();
        }

        Destroy(gameObject);
    }

    private void FindTargetIfMissing()
    {
        if (target != null)
        {
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            target = player.transform;
        }
    }

    private void OnDestroy()
    {
        if (enemyHealth != null)
        {
            enemyHealth.Died -= HandleEnemyDied;
        }
    }

    private void OnValidate()
    {
        moveSpeed = Mathf.Max(0f, moveSpeed);
        explosionRadius = Mathf.Max(0f, explosionRadius);
        explosionDamage = Mathf.Max(0, explosionDamage);
        triggerDistance = Mathf.Max(0f, triggerDistance);
    }
}
