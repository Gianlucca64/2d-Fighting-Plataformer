using UnityEngine;

public class SpiderBossAttackHitbox : MonoBehaviour
{
    public SpiderBoss boss;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (boss == null)
            return;

        if (!boss.IsDashing())
            return;

        PlayerController player =
            other.GetComponent<PlayerController>();

        if (player != null)
        {
            player.TakeDamage(
                boss.contactDamage,
                transform.position);
        }
    }
}