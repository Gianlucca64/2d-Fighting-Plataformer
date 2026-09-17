using UnityEngine;

public class MushroomBounce : MonoBehaviour
{
    public float bounceForce = 15f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        Rigidbody2D rb =
            other.GetComponent<Rigidbody2D>();

        if (rb == null)
            return;

        rb.velocity = new Vector2(
            rb.velocity.x,
            bounceForce);
    }
}