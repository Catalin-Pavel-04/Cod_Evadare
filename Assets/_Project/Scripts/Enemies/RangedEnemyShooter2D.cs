using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RangedEnemyShooter2D : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private GameObject enemyProjectilePrefab;
    [SerializeField] private float moveSpeed = 1.2f;
    [SerializeField] private float preferredDistance = 4f;
    [SerializeField] private float minimumDistance = 2f;
    [SerializeField] private float attackInterval = 1.25f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float projectileSpeed = 7f;
    [SerializeField] private float projectileLifetime = 3f;
    [SerializeField] private bool activateOnStart = true;
    [SerializeField] private float aimTelegraphDuration = 0.25f;
    [SerializeField] private LineRenderer aimLine;
    [SerializeField] private bool useAimTelegraph = true;

    private Rigidbody2D body;
    private EnemyHealth enemyHealth;
    private bool isActive;
    private bool isTelegraphing;
    private bool loggedMissingProjectilePrefab;
    private float nextAttackTime;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.freezeRotation = true;
        enemyHealth = GetComponent<EnemyHealth>();
        HideAimLine();
    }

    private void Start()
    {
        FindTargetIfMissing();
        isActive = activateOnStart;
        nextAttackTime = Time.time + Mathf.Max(0.1f, attackInterval);
        HideAimLine();
    }

    private void Update()
    {
        if (!isActive || IsDead())
        {
            return;
        }

        FindTargetIfMissing();

        if (target == null || isTelegraphing || Time.time < nextAttackTime)
        {
            return;
        }

        if (useAimTelegraph && aimTelegraphDuration > 0f)
        {
            StartCoroutine(AimTelegraphRoutine());
            return;
        }

        ShootAtTarget();
        nextAttackTime = Time.time + Mathf.Max(0.05f, attackInterval);
    }

    private void FixedUpdate()
    {
        if (!isActive || IsDead() || target == null)
        {
            return;
        }

        Vector2 currentPosition = body.position;
        Vector2 toTarget = (Vector2)target.position - currentPosition;
        float distance = toTarget.magnitude;

        if (distance < 0.0001f)
        {
            body.velocity = Vector2.zero;
            return;
        }

        Vector2 moveDirection = Vector2.zero;

        if (distance > preferredDistance)
        {
            moveDirection = toTarget.normalized;
        }
        else if (distance < minimumDistance)
        {
            moveDirection = -toTarget.normalized;
        }

        if (moveDirection == Vector2.zero)
        {
            body.velocity = Vector2.zero;
            return;
        }

        Vector2 nextPosition = currentPosition + moveDirection * moveSpeed * Time.fixedDeltaTime;
        body.MovePosition(nextPosition);
    }

    private IEnumerator AimTelegraphRoutine()
    {
        isTelegraphing = true;
        float endTime = Time.time + Mathf.Max(0f, aimTelegraphDuration);

        while (Time.time < endTime)
        {
            if (!isActive || IsDead() || target == null)
            {
                HideAimLine();
                isTelegraphing = false;
                yield break;
            }

            ShowAimLine();
            yield return null;
        }

        HideAimLine();

        if (isActive && !IsDead() && target != null)
        {
            ShootAtTarget();
        }

        nextAttackTime = Time.time + Mathf.Max(0.05f, attackInterval);
        isTelegraphing = false;
    }

    private void ShootAtTarget()
    {
        if (enemyProjectilePrefab == null)
        {
            if (!loggedMissingProjectilePrefab)
            {
                Debug.LogWarning($"{name} cannot shoot because enemyProjectilePrefab is missing.", this);
                loggedMissingProjectilePrefab = true;
            }

            return;
        }

        Vector3 spawnPosition = projectileSpawnPoint != null ? projectileSpawnPoint.position : transform.position;
        Vector2 direction = target != null ? ((Vector2)target.position - (Vector2)spawnPosition).normalized : transform.right;

        if (direction.sqrMagnitude < 0.0001f)
        {
            direction = transform.right;
        }

        Quaternion rotation = Quaternion.FromToRotation(Vector3.right, direction);
        GameObject projectileObject = Instantiate(enemyProjectilePrefab, spawnPosition, rotation);
        EnemyProjectile2D projectile = projectileObject.GetComponent<EnemyProjectile2D>();

        if (projectile == null)
        {
            Debug.LogWarning($"{name} spawned a projectile prefab without EnemyProjectile2D.", this);
            Destroy(projectileObject);
            return;
        }

        projectile.SetOwner(gameObject);
        projectile.Configure(damage, projectileSpeed, projectileLifetime);
        projectile.Fire(direction);
    }

    private void ShowAimLine()
    {
        if (aimLine == null)
        {
            return;
        }

        Vector3 spawnPosition = projectileSpawnPoint != null ? projectileSpawnPoint.position : transform.position;
        Vector3 targetPosition = target != null ? target.position : spawnPosition + transform.right;

        aimLine.enabled = true;
        aimLine.positionCount = 2;
        aimLine.SetPosition(0, spawnPosition);
        aimLine.SetPosition(1, targetPosition);
    }

    private void HideAimLine()
    {
        if (aimLine != null)
        {
            aimLine.enabled = false;
        }
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

    private bool IsDead()
    {
        return enemyHealth != null && enemyHealth.IsDead;
    }

    private void OnValidate()
    {
        moveSpeed = Mathf.Max(0f, moveSpeed);
        preferredDistance = Mathf.Max(0f, preferredDistance);
        minimumDistance = Mathf.Max(0f, minimumDistance);
        attackInterval = Mathf.Max(0.05f, attackInterval);
        damage = Mathf.Max(0, damage);
        projectileSpeed = Mathf.Max(0f, projectileSpeed);
        projectileLifetime = Mathf.Max(0.01f, projectileLifetime);
        aimTelegraphDuration = Mathf.Max(0f, aimTelegraphDuration);
    }
}
