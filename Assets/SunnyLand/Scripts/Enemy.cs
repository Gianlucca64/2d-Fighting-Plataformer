using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 3;

    [Header("Contacto")]
    public int contactDamage = 1;

    [Header("Knockback")]
    public float knockbackForce = 5f;
    public float hitFlashTime = 0.1f;

    protected int currentHealth;
    protected Rigidbody2D rb;
    protected PlayerController player;

    protected bool isKnockedBack = false;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        player = FindObjectOfType<PlayerController>();

        currentHealth = maxHealth;
    }

    public void TakeDamage(
        int damage,
        Vector2 knockbackDirection)
    {
        currentHealth -= damage;

        rb.velocity = Vector2.zero;

        isKnockedBack = true;

        rb.AddForce(
            knockbackDirection * knockbackForce,
            ForceMode2D.Impulse);

        StartCoroutine(KnockbackRecovery());
        StartCoroutine(HitFlash());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    System.Collections.IEnumerator KnockbackRecovery()
    {
        yield return new WaitForSeconds(0.2f);

        isKnockedBack = false;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        PlayerController player =
            collision.gameObject.GetComponent<PlayerController>();

        if (player != null)
        {
            player.TakeDamage(
                contactDamage,
                transform.position);
        }
    }

    System.Collections.IEnumerator HitFlash()
    {
        SpriteRenderer sr =
            GetComponent<SpriteRenderer>();

        if (sr == null)
            yield break;

        sr.enabled = false;

        yield return new WaitForSeconds(hitFlashTime);

        sr.enabled = true;
    }

    void Die()
    {
        Destroy(gameObject);
    }
}