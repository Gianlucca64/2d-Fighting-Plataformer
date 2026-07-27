using UnityEngine;

public class Spike : PogoObject
{
    public int damage = 1;

    void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController player =
            collision.gameObject.GetComponent<PlayerController>();

        if (player == null)
            return;

        if (player.isPogoAttacking)
            return;

        player.TakeDamage(
            damage,
            transform.position);
    }
    void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log("SPIKE STAY");

        PlayerController player =
            collision.gameObject.GetComponent<PlayerController>();

        if (player == null)
            return;

        Debug.Log("Pogo: " + player.isPogoAttacking);

        if (player.isPogoAttacking)
            return;

        player.TakeDamage(
            damage,
            transform.position);
    }
}