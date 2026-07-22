using UnityEngine;

public class FlyingEnemy : Enemy
{
    [Header("Vuelo")]
    public float flySpeed = 3f;

    protected override void Start()
    {
        base.Start();

        rb.gravityScale = 0f;
    }

    protected void FlyTowards(Vector2 target)
    {
        if (isKnockedBack)
            return;

        Vector2 dir =
            (target -
            (Vector2)transform.position).normalized;

        rb.velocity = dir * flySpeed;
    }

    protected void StopFlying()
    {
        rb.velocity = Vector2.zero;
    }
}