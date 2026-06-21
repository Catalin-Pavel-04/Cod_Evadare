using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyProjectile2D : MonoBehaviour
{
    [SerializeField] private float speed = 7f;
    [SerializeField] private float lifetime = 4f;
    [SerializeField] private int damage = 1;
    [SerializeField] private bool destroyOnHit = true;

    private Rigidbody2D body;
    private GameObject owner;
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

    public void SetOwner(GameObject ownerObject)
    {
        owner = ownerObject;
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
        if (other == null || IsOwnerOrOwnerChild(other))
        {
            return;
        }

        if (other.GetComponentInParent<EnemyHealth>() != null)
        {
            return;
        }

        PlayerHealth2D playerHealth = other.GetComponent<PlayerHealth2D>();

        if (playerHealth == null)
        {
            playerHealth = other.GetComponentInParent<PlayerHealth2D>();
        }

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);

            if (destroyOnHit)
            {
                Destroy(gameObject);
            }

            return;
        }

        if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }

    private bool IsOwnerOrOwnerChild(Collider2D other)
    {
        if (owner == null)
        {
            return false;
        }

        Transform otherTransform = other.transform;
        return otherTransform == owner.transform || otherTransform.IsChildOf(owner.transform);
    }

    private void OnValidate()
    {
        speed = Mathf.Max(0f, speed);
        lifetime = Mathf.Max(0.01f, lifetime);
        damage = Mathf.Max(0, damage);
    }
}
