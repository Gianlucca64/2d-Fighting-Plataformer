using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 8f;

    Vector2 moveDirection;
    int damage;

    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        Destroy(gameObject, 5f);
    }

    public void Initialize(
        Vector2 direction,
        int projectileDamage)
    {
        moveDirection = direction.normalized;
        damage = projectileDamage;
    }

    void FixedUpdate()
    {
        rb.velocity =
            moveDirection * speed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player =
            other.GetComponent<PlayerController>();

        if (player != null)
        {
            player.TakeDamage(
                damage,
                transform.position);

            Destroy(gameObject);
        }

        if (other.gameObject.layer ==
            LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}