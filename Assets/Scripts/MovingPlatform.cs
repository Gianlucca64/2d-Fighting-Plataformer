
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Movimiento")]
    public Vector2 moveDirection = Vector2.right;
    public float moveSpeed = 3f;

    [Header("Detección")]
    public Transform wallCheck;
    public float wallDistance = 0.2f;
    public LayerMask groundLayer;

    Vector3 startPosition;

    bool activated = false;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if (!activated)
            return;

        bool wallDetected = Physics2D.Raycast(
            wallCheck.position,
            moveDirection.normalized,
            wallDistance,
            groundLayer);

        if (wallDetected)
        {
            activated = false;
            return;
        }

        transform.Translate(
            moveDirection.normalized *
            moveSpeed *
            Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController player =
            collision.gameObject.GetComponent<PlayerController>();

        if (player != null)
        {
            activated = true;
        }
    }

    //--------------------------------
    // Reiniciar plataforma
    //--------------------------------
    public void ResetPlatform()
    {
        transform.position = startPosition;
        activated = false;
    }

    //--------------------------------
    // Gizmos
    //--------------------------------
    void OnDrawGizmos()
    {
        if (wallCheck != null)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawLine(
                wallCheck.position,
                wallCheck.position +
                (Vector3)moveDirection.normalized *
                wallDistance);
        }
    }
}
// NOTA poner estos numeros en para cambiar hacia donde se mueve la plataforma
// (1, 0)   = derecha
// (-1, 0)  = izquierda
// (0, 1)   = arriba
// (0, -1)  = abajo


