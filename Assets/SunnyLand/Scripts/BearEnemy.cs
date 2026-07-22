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
    public GameObject attackVisual;

    float attackTimer;

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

        rb.velocity = new Vector2(
            direction * moveSpeed,
            rb.velocity.y);

        if (direction > 0)
        {
            movingRight = true;
            transform.localScale =
                new Vector3(1, 1, 1);
        }
        else
        {
            movingRight = false;
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

            StartCoroutine(
                ShowAttackVisual());

            player.TakeDamage(
                attackDamage,
                transform.position);
        }
    }

    System.Collections.IEnumerator ShowAttackVisual()
    {
        attackVisual.SetActive(true);

        yield return new WaitForSeconds(0.15f);

        attackVisual.SetActive(false);
    }
}