using UnityEngine;

public class PlatformCarry : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController player =
            collision.gameObject.GetComponent<PlayerController>();

        if (player != null)
        {
            Vector3 playerScale =
                player.transform.localScale;

            player.transform.SetParent(transform, true);

            player.transform.localScale =
                playerScale;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        PlayerController player =
            collision.gameObject.GetComponent<PlayerController>();

        if (player != null)
        {
            Vector3 playerScale =
                player.transform.localScale;

            player.transform.SetParent(null, true);

            player.transform.localScale =
                playerScale;
        }
    }
}