using UnityEngine;
using UnityEngine.UI;

public class SpiderBoss : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 12;
    int currentHealth;

    [Header("Contacto")]
    public int contactDamage = 1;

    [Header("Barra de vida")]
    public Slider healthBar;

    [Header("Embestida")]
    public Transform dashStartPoint;
    public Transform dashEndPoint;
    public float dashSpeed = 12f;

    [Header("Stun")]
    public float stunDuration = 3f;

    bool isDashing = false;

    bool isStunned = false;

    bool canBeStunned = true;

    bool isRecovering = false;

    [Header("Recuperación")]
    public float retreatDistance = 1.5f;
    public float retreatSpeed = 4f;
    public float retreatWaitTime = 0.5f;

    Vector2 retreatTarget;
    float retreatTimer;
    bool goingToEnd = true;
    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        if (dashStartPoint != null)
            transform.position = dashStartPoint.position;

        Invoke(nameof(StartDash), 2f);
    }


    void Update()
    {
        if (isStunned)
            return;

        if (isRecovering)
        {
            Retreat();
            return;
        }

        if (isDashing)
            Dash();
    }
    void Retreat()
    {
        if (retreatTimer > 0)
        {
            retreatTimer -= Time.deltaTime;
            return;
        }

        transform.position = Vector2.MoveTowards(
            transform.position,
            retreatTarget,
            retreatSpeed * Time.deltaTime);

        if (Vector2.Distance(
    transform.position,
    retreatTarget) < 0.05f)
        {
            isRecovering = false;

            Invoke(nameof(StartDash), retreatWaitTime);
        }
    }

    void Dash()
    {
        Transform target =
            goingToEnd ? dashEndPoint : dashStartPoint;

        if (target == null)
            return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            dashSpeed * Time.deltaTime);

        if (Vector2.Distance(
            transform.position,
            target.position) < 0.05f)
        {
            EndDash();

            goingToEnd = !goingToEnd;
        }
    }

    public void StartDash()
    {
        if (isStunned)
            return;

        if (isRecovering)
            return;

        canBeStunned = true;

        isDashing = true;
    }

    public bool IsDashing()
    {
        return isDashing;
    }
    void EndDash()
    {
        isDashing = false;
    }

    public void Stun()
    {
        if (isStunned)
            return;

        if (!canBeStunned)
            return;

        canBeStunned = false;

        isStunned = true;
        isDashing = false;

        rb.velocity = Vector2.zero;

        CancelInvoke(nameof(EndStun));
        Invoke(nameof(EndStun), stunDuration);
    }

    void EndStun()
    {
        isStunned = false;

        StartRetreat();
    }
    void StartRetreat()
    {
        isRecovering = true;

        float direction =
            goingToEnd ? 1f : -1f;

        retreatTarget =
            (Vector2)transform.position
            - new Vector2(direction * retreatDistance, 0f);

        retreatTimer = retreatWaitTime;
    }
    public void TakeBossDamage(int damage)
    {
        if (!isStunned)
            return;

        currentHealth -= damage;

        if (healthBar != null)
            healthBar.value = currentHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    public void TryTakeDamage(int damage)
    {
        if (!isStunned)
            return;

        TakeBossDamage(damage);
    }

    void Die()
    {
        gameObject.SetActive(false);
    }

    public bool IsStunned()
    {
        return isStunned;
    }
}
