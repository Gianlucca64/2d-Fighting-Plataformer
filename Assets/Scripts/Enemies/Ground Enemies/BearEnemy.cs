using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class BearEnemy : WalkerEnemy
{
    [Header("Detección")]
    public float detectRange = 5f;

    [Header("Ataque")]
    public float attackRange = 1.5f;
    public float attackCooldown = 1.2f;
    public int attackDamage = 1;

    [Header("Visual Ataque")]
    Animator anim;
    float attackTimer;

    protected override void Start()
    {
        base.Start();

        anim = GetComponent<Animator>();
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

    protected override void Patrol()
    {
        anim.SetBool("Run", true);

        base.Patrol();
    }

    void ChasePlayer()
    {
        if (isKnockedBack)
            return;

        if (!HasGroundAhead())
        {
            rb.velocity = Vector2.zero;
            return;
        }

        float direction =
            player.transform.position.x >
            transform.position.x
            ? 1f
            : -1f;

        anim.SetBool("Run", true);

        rb.velocity = new Vector2(
            direction * moveSpeed,
            rb.velocity.y);

        if (direction > 0)
        {
            movingRight = true;
            transform.localScale =
                new Vector3(-1, 1, 1);
        }
        else
        {
            movingRight = false;
            transform.localScale =
                new Vector3(1, 1, 1);
        }
    }

    void AttackBehaviour()
    {
        anim.SetBool("Run", false);

        rb.velocity =
            new Vector2(
                0,
                rb.velocity.y);

        attackTimer += Time.fixedDeltaTime;

        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0;

            anim.SetTrigger("Attack");
        }
    }
    public void DealDamage()
    {
        if (player == null)
            return;

        float distance = Vector2.Distance(
            transform.position,
            player.transform.position);

        if (distance <= attackRange)
        {
            player.TakeDamage(
                attackDamage,
                transform.position);
        }
    }
}
