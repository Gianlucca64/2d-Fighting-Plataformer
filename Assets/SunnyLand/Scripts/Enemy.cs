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
    int currentHealth;

    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        currentHealth = maxHealth;
    }

    //--------------------------------
    // RECIBIR DAÑO
    //--------------------------------
    public void TakeDamage(
        int damage,
        Vector2 knockbackDirection)
    {
        currentHealth -= damage;

        rb.velocity = Vector2.zero;

        rb.AddForce(
            knockbackDirection *
            knockbackForce,
            ForceMode2D.Impulse);

        StartCoroutine(HitFlash());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    //--------------------------------
    // DAÑO POR CONTACTO
    //--------------------------------
    void OnCollisionStay2D(Collision2D collision)
    {
        PlayerController player =
            collision.gameObject
            .GetComponent<PlayerController>();

        if (player != null)
        {
            player.TakeDamage(
                contactDamage,
                transform.position);
        }
    }

    //Parpadeo//
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
    //--------------------------------
    // MORIR
    //--------------------------------
    void Die()
    {
        Destroy(gameObject);
    }
}