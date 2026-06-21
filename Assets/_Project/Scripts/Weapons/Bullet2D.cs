using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet2D : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private int damage = 1;

    private Rigidbody2D body;
    private bool lifetimeStarted;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        body.gravityScale = 0f;
    }

    private void Start()
    {
        StartLifetimeTimer();
    }

    public void Configure(int newDamage, float newSpeed, float newLifetime)
    {
        damage = Mathf.Max(0, newDamage);
        speed = Mathf.Max(0f, newSpeed);
        lifetime = Mathf.Max(0.01f, newLifetime);

        if (lifetimeStarted)
        {
            StartLifetimeTimer();
        }
    }

    public void Fire(Vector2 direction)
    {
        if (body == null)
        {
            body = GetComponent<Rigidbody2D>();
        }

        if (direction.sqrMagnitude < 0.0001f)
        {
            direction = transform.right;
        }

        body.velocity = direction.normalized * speed;
    }

    private void StartLifetimeTimer()
    {
        CancelInvoke(nameof(DestroySelf));
        lifetimeStarted = true;
        Invoke(nameof(DestroySelf), lifetime);
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null || other.CompareTag("Player"))
        {
            return;
        }

        EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();

        if (enemyHealth == null)
        {
            enemyHealth = other.GetComponentInParent<EnemyHealth>();
        }

        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        if (other.isTrigger)
        {
            return;
        }

        Destroy(gameObject);
    }
}
