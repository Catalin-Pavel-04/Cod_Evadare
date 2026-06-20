using UnityEngine;

public class PlayerShooting2D : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float fireCooldown = 0.15f;

    private float nextFireTime;
    private bool loggedMissingFirePoint;
    private bool loggedMissingBulletPrefab;
    private bool loggedMissingBulletComponent;

    private void Update()
    {
        if (!Input.GetMouseButton(0))
        {
            return;
        }

        if (Time.time < nextFireTime)
        {
            return;
        }

        nextFireTime = Time.time + fireCooldown;
        Shoot();
    }

    private void Shoot()
    {
        if (firePoint == null)
        {
            LogWarningOnce(ref loggedMissingFirePoint, "PlayerShooting2D cannot shoot because firePoint is not assigned.");
            return;
        }

        if (bulletPrefab == null)
        {
            LogWarningOnce(ref loggedMissingBulletPrefab, "PlayerShooting2D cannot shoot because bulletPrefab is not assigned.");
            return;
        }

        GameObject bulletObject = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet2D bullet = bulletObject.GetComponent<Bullet2D>();

        if (bullet == null)
        {
            LogWarningOnce(ref loggedMissingBulletComponent, $"The bullet prefab '{bulletPrefab.name}' is missing a Bullet2D component.");
            Destroy(bulletObject);
            return;
        }

        bullet.Fire(firePoint.right);
    }

    private void LogWarningOnce(ref bool alreadyLogged, string message)
    {
        if (alreadyLogged)
        {
            return;
        }

        Debug.LogWarning(message, this);
        alreadyLogged = true;
    }
}
