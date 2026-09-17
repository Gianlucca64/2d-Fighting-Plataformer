using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Sprite")]
    public Sprite checkpointOn;

    SpriteRenderer spriteRenderer;
    bool activated = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player =
            other.GetComponent<PlayerController>();

        if (player != null)
        {
            player.respawnPoint = transform;

            player.RestoreHealth();

            if (!activated)
            {
                activated = true;

                if (checkpointOn != null)
                    spriteRenderer.sprite = checkpointOn;
            }
        }
    }
}