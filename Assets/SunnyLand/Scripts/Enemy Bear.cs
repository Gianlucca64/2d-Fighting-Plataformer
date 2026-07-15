using UnityEngine;

public class BearEnemy : Enemy
{
    [Header("Movimiento")]
    public float moveSpeed = 1.5f;

    [Header("Detección")]
    public Transform wallCheck;
    public Transform edgeCheck;

    [Header("Detección")]
    public float detectRange = 5f;

    public float wallDistance = 0.2f;
    public float edgeDistance = 0.5f;

    public LayerMask obstacleLayer;

    [Header("Ataque")]
    public float attackRange = 1.5f;
    public float attackCooldown = 1.2f;
    public int attackDamage = 1;

    bool movingRight = true;
    float attackTimer;

    PlayerController player;

    protected override void Start()
    {
        base.Start();

        player = FindObjectOfType<PlayerController>();
    }

    void FixedUpdate()
    {
        if (player == null)
            return;

        float distance =
            Vector2.Distance(
                transform.position,
                player.transform.position);

        if (distance <= attackRange)
        {
            AttackBehaviour();
        }
        else if (distance <= detectRange)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
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

    void ChasePlayer()
    {
        if (isKnockedBack)
            return;

        float direction =
            player.transform.position.x >
            transform.position.x
            ? 1f
            : -1f;

        rb.velocity = new Vector2(
            direction * moveSpeed,
            rb.velocity.y);

        if (direction > 0)
        {
            transform.localScale =
                new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale =
                new Vector3(-1, 1, 1);
        }
    }
    void AttackBehaviour()
    {
        rb.velocity =
            new Vector2(
                0,
                rb.velocity.y);

        attackTimer += Time.fixedDeltaTime;

        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0;

            Debug.Log("Bear atacó");

            player.TakeDamage(
                attackDamage,
                transform.position);
        }
    }

    void Flip()
    {
        movingRight = !movingRight;

        Vector3 scale = transform.localScale;

        scale.x *= -1;

        transform.localScale = scale;
    }
}