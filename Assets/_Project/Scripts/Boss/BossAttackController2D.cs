using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BossAttackController2D : MonoBehaviour
{
    [SerializeField] private string bossName = "Experiment-01";
    [SerializeField] private Transform target;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private GameObject enemyProjectilePrefab;
    [SerializeField] private float moveSpeed = 1.2f;
    [SerializeField] private float stopDistance = 3f;
    [SerializeField] private float attackInterval = 1.5f;
    [SerializeField] private float phaseTwoHealthPercent = 0.5f;
    [SerializeField] private float phaseTwoAttackIntervalMultiplier = 0.65f;
    [SerializeField] private int aimedShotDamage = 1;
    [SerializeField] private int radialShotDamage = 1;
    [SerializeField] private float projectileSpeed = 7f;
    [SerializeField] private float projectileLifetime = 4f;
    [SerializeField] private int phaseOneRadialProjectiles = 8;
    [SerializeField] private int phaseTwoRadialProjectiles = 14;
    [SerializeField] private bool activateOnStart = true;

    private Rigidbody2D body;
    private EnemyHealth enemyHealth;
    private bool isActive;
    private bool isPhaseTwo;
    private bool useRadialNext;
    private bool loggedMissingProjectilePrefab;
    private float nextAttackTime;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.freezeRotation = true;

        enemyHealth = GetComponent<EnemyHealth>();
    }

    private void Start()
    {
        FindTargetIfMissing();
        RegisterBossHealthUI();

        if (enemyHealth != null)
        {
            enemyHealth.Died += HandleBossDied;
        }

        if (activateOnStart)
        {
            ActivateBoss();
        }
    }

    private void Update()
    {
        if (!isActive || IsBossDead())
        {
            return;
        }

        FindTargetIfMissing();
        CheckPhaseTwo();

        if (Time.time >= nextAttackTime)
        {
            PerformAttack();
            nextAttackTime = Time.time + GetCurrentAttackInterval();
        }
    }

    private void FixedUpdate()
    {
        if (!isActive || IsBossDead() || target == null)
        {
            return;
        }

        Vector2 currentPosition = body.position;
        Vector2 targetPosition = target.position;
        Vector2 toTarget = targetPosition - currentPosition;

        if (toTarget.sqrMagnitude <= stopDistance * stopDistance)
        {
            body.velocity = Vector2.zero;
            return;
        }

        Vector2 nextPosition = currentPosition + toTarget.normalized * moveSpeed * Time.fixedDeltaTime;
        body.MovePosition(nextPosition);
    }

    public void ActivateBoss()
    {
        if (IsBossDead())
        {
            return;
        }

        isActive = true;
        nextAttackTime = Time.time + Mathf.Max(0.1f, GetCurrentAttackInterval());
    }

    public void DeactivateBoss()
    {
        isActive = false;

        if (body != null)
        {
            body.velocity = Vector2.zero;
        }
    }

    private void PerformAttack()
    {
        if (enemyProjectilePrefab == null)
        {
            if (!loggedMissingProjectilePrefab)
            {
                Debug.LogWarning($"{bossName} cannot attack because enemyProjectilePrefab is missing.", this);
                loggedMissingProjectilePrefab = true;
            }

            return;
        }

        if (useRadialNext)
        {
            FireRadialBurst();
        }
        else
        {
            FireAimedShot();
        }

        useRadialNext = !useRadialNext;
    }

    private void FireAimedShot()
    {
        Vector2 direction = target != null
            ? (target.position - GetProjectileSpawnPosition()).normalized
            : transform.right;

        SpawnProjectile(direction, aimedShotDamage);
    }

    private void FireRadialBurst()
    {
        int projectileCount = isPhaseTwo ? phaseTwoRadialProjectiles : phaseOneRadialProjectiles;
        projectileCount = Mathf.Max(1, projectileCount);

        for (int i = 0; i < projectileCount; i++)
        {
            float angle = 360f * i / projectileCount;
            Vector2 direction = Quaternion.Euler(0f, 0f, angle) * Vector2.right;
            SpawnProjectile(direction, radialShotDamage);
        }
    }

    private void SpawnProjectile(Vector2 direction, int projectileDamage)
    {
        Vector3 spawnPosition = GetProjectileSpawnPosition();
        Quaternion rotation = direction.sqrMagnitude > 0.0001f
            ? Quaternion.FromToRotation(Vector3.right, direction.normalized)
            : Quaternion.identity;

        GameObject projectileObject = Instantiate(enemyProjectilePrefab, spawnPosition, rotation);
        EnemyProjectile2D projectile = projectileObject.GetComponent<EnemyProjectile2D>();

        if (projectile == null)
        {
            Debug.LogWarning($"{bossName} spawned a projectile prefab without EnemyProjectile2D.", this);
            Destroy(projectileObject);
            return;
        }

        projectile.SetOwner(gameObject);
        projectile.Configure(projectileDamage, projectileSpeed, projectileLifetime);
        projectile.Fire(direction);
    }

    private Vector3 GetProjectileSpawnPosition()
    {
        return projectileSpawnPoint != null ? projectileSpawnPoint.position : transform.position;
    }

    private void CheckPhaseTwo()
    {
        if (isPhaseTwo || enemyHealth == null || enemyHealth.MaxHealth <= 0)
        {
            return;
        }

        float phaseTwoThreshold = enemyHealth.MaxHealth * phaseTwoHealthPercent;

        if (enemyHealth.CurrentHealth <= phaseTwoThreshold)
        {
            isPhaseTwo = true;
            Debug.Log($"{bossName} entered phase 2", this);
        }
    }

    private float GetCurrentAttackInterval()
    {
        float interval = Mathf.Max(0.05f, attackInterval);

        if (isPhaseTwo)
        {
            interval *= Mathf.Max(0.05f, phaseTwoAttackIntervalMultiplier);
        }

        return interval;
    }

    private bool IsBossDead()
    {
        return enemyHealth != null && enemyHealth.IsDead;
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

    private void RegisterBossHealthUI()
    {
        if (enemyHealth == null)
        {
            return;
        }

        BossHealthUI2D bossHealthUI = FindBossHealthUI();

        if (bossHealthUI != null)
        {
            bossHealthUI.SetBoss(enemyHealth, bossName);
        }
    }

    private BossHealthUI2D FindBossHealthUI()
    {
        BossHealthUI2D bossHealthUI = FindObjectOfType<BossHealthUI2D>();

        if (bossHealthUI != null)
        {
            return bossHealthUI;
        }

        BossHealthUI2D[] allBossHealthUis = Resources.FindObjectsOfTypeAll<BossHealthUI2D>();
        return allBossHealthUis != null && allBossHealthUis.Length > 0 ? allBossHealthUis[0] : null;
    }

    private void HandleBossDied(EnemyHealth deadBoss)
    {
        DeactivateBoss();
    }

    private void OnDestroy()
    {
        if (enemyHealth != null)
        {
            enemyHealth.Died -= HandleBossDied;
        }
    }

    private void OnValidate()
    {
        moveSpeed = Mathf.Max(0f, moveSpeed);
        stopDistance = Mathf.Max(0f, stopDistance);
        attackInterval = Mathf.Max(0.05f, attackInterval);
        phaseTwoHealthPercent = Mathf.Clamp01(phaseTwoHealthPercent);
        phaseTwoAttackIntervalMultiplier = Mathf.Max(0.05f, phaseTwoAttackIntervalMultiplier);
        aimedShotDamage = Mathf.Max(0, aimedShotDamage);
        radialShotDamage = Mathf.Max(0, radialShotDamage);
        projectileSpeed = Mathf.Max(0f, projectileSpeed);
        projectileLifetime = Mathf.Max(0.01f, projectileLifetime);
        phaseOneRadialProjectiles = Mathf.Max(1, phaseOneRadialProjectiles);
        phaseTwoRadialProjectiles = Mathf.Max(1, phaseTwoRadialProjectiles);
    }
}
