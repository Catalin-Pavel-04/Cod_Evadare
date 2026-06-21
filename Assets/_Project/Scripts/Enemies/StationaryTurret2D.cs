using System.Collections;
using UnityEngine;

public class StationaryTurret2D : MonoBehaviour
{
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private GameObject enemyProjectilePrefab;
    [SerializeField] private float detectionRange = 8f;
    [SerializeField] private float attackInterval = 1f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float projectileSpeed = 8f;
    [SerializeField] private float projectileLifetime = 3f;
    [SerializeField] private bool useAimTelegraph = true;
    [SerializeField] private float aimTelegraphDuration = 0.25f;
    [SerializeField] private LineRenderer aimLine;

    private Transform target;
    private EnemyHealth enemyHealth;
    private float nextAttackTime;
    private bool isTelegraphing;
    private bool loggedMissingProjectilePrefab;

    private void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        HideAimLine();
    }

    private void Start()
    {
        FindTargetIfMissing();
        nextAttackTime = Time.time + Mathf.Max(0.05f, attackInterval);
    }

    private void Update()
    {
        if (IsDead())
        {
            HideAimLine();
            return;
        }

        FindTargetIfMissing();

        if (target == null || isTelegraphing || Time.time < nextAttackTime || !IsTargetInRange())
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

    private IEnumerator AimTelegraphRoutine()
    {
        isTelegraphing = true;
        float endTime = Time.time + Mathf.Max(0f, aimTelegraphDuration);

        while (Time.time < endTime)
        {
            if (target == null || IsDead() || !IsTargetInRange())
            {
                HideAimLine();
                isTelegraphing = false;
                yield break;
            }

            ShowAimLine();
            yield return null;
        }

        HideAimLine();
        ShootAtTarget();
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

        GameObject projectileObject = Instantiate(enemyProjectilePrefab, spawnPosition, Quaternion.FromToRotation(Vector3.right, direction));
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

    private bool IsTargetInRange()
    {
        return target != null && Vector2.Distance(transform.position, target.position) <= detectionRange;
    }

    private void ShowAimLine()
    {
        if (aimLine == null)
        {
            return;
        }

        Vector3 spawnPosition = projectileSpawnPoint != null ? projectileSpawnPoint.position : transform.position;
        aimLine.enabled = true;
        aimLine.positionCount = 2;
        aimLine.SetPosition(0, spawnPosition);
        aimLine.SetPosition(1, target != null ? target.position : spawnPosition + transform.right);
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
        detectionRange = Mathf.Max(0f, detectionRange);
        attackInterval = Mathf.Max(0.05f, attackInterval);
        damage = Mathf.Max(0, damage);
        projectileSpeed = Mathf.Max(0f, projectileSpeed);
        projectileLifetime = Mathf.Max(0.01f, projectileLifetime);
        aimTelegraphDuration = Mathf.Max(0f, aimTelegraphDuration);
    }
}
