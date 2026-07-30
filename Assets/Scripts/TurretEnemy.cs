using UnityEngine;

public class TurretEnemy : Enemy
{
    [Header("Disparo")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    public float detectRange = 8f;
    public float fireCooldown = 1.5f;
   
    float fireTimer;
    Vector3 originalScale;

    protected override void Start()
    {
        base.Start();

        originalScale = transform.localScale;
    }
    void FixedUpdate()
    {
        if (player == null)
            return;

        float distance =
            Vector2.Distance(
                transform.position,
                player.transform.position);

        if (distance <= detectRange)
        {
            AimAtPlayer();

            fireTimer += Time.fixedDeltaTime;

            if (fireTimer >= fireCooldown)
            {
                fireTimer = 0;

                Shoot();
            }
        }
    }

    void AimAtPlayer()
    {
        Vector3 scale = originalScale;

        if (player.transform.position.x <
            transform.position.x)
        {
            scale.x *= -1;
        }

        transform.localScale = scale;
    }

    void Shoot()
    {
        GameObject bullet =
            Instantiate(
                projectilePrefab,
                firePoint.position,
                Quaternion.identity);

        Projectile projectile =
            bullet.GetComponent<Projectile>();

        if (projectile != null)
        {
            Vector2 direction =
                (player.transform.position -
                firePoint.position).normalized;

            projectile.Initialize(
                direction,
                contactDamage);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            detectRange);
    }
}