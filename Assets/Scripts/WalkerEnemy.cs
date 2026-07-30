using UnityEngine;

public class WalkerEnemy : Enemy
{
    [Header("Movimiento")]
    public float moveSpeed = 2f;

    [Header("Detección")]
    public Transform wallCheck;
    public Transform edgeCheck;

    public float wallDistance = 0.2f;
    public float edgeDistance = 0.5f;

    public LayerMask obstacleLayer;

    protected bool movingRight = true;

    protected virtual void Patrol()
    {
        if (isKnockedBack)
            return;

        float direction = movingRight ? 1f : -1f;

        rb.velocity = new Vector2(
            direction * moveSpeed,
            rb.velocity.y);

        bool wallDetected = Physics2D.Raycast(
            wallCheck.position,
            Vector2.right * direction,
            wallDistance,
            obstacleLayer);

        bool groundDetected = Physics2D.Raycast(
            edgeCheck.position,
            Vector2.down,
            edgeDistance);

        if (wallDetected || !groundDetected)
        {
            Flip();
        }
    }

    protected void Flip()
    {
        movingRight = !movingRight;

        Vector3 scale = transform.localScale;

        scale.x *= -1;

        transform.localScale = scale;
    }

    protected bool HasGroundAhead()
    {
        return Physics2D.Raycast(
            edgeCheck.position,
            Vector2.down,
            edgeDistance);
    }

    void OnDrawGizmos()
    {
        if (wallCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(wallCheck.position, 0.1f);
        }

        if (edgeCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(edgeCheck.position, 0.1f);
        }
    }
}