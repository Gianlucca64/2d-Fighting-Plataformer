using UnityEngine;
using System.Collections;

public class BoxCollectible : MonoBehaviour
{
    [Header("Destino")]
    public Transform targetPoint;

    [Header("Plataforma")]
    public GameObject platformPrefab;

    [Header("Animación")]
    public float moveDuration = 0.5f;

    bool collected = false;

    Collider2D boxCollider;
    SpriteRenderer spriteRenderer;

    void Start()
    {
        boxCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (collected)
            return;

        if (!other.CompareTag("Player"))
            return;

        collected = true;

        boxCollider.enabled = false;

        StartCoroutine(MoveToTarget());
    }

    IEnumerator MoveToTarget()
    {
        Vector3 startPosition = transform.position;

        float timer = 0f;

        while (timer < moveDuration)
        {
            timer += Time.deltaTime;

            float t = timer / moveDuration;

            transform.position =
                Vector3.Lerp(
                    startPosition,
                    targetPoint.position,
                    t);

            yield return null;
        }

        transform.position = targetPoint.position;

        if (platformPrefab != null)
        {
            Instantiate(
                platformPrefab,
                targetPoint.position,
                Quaternion.identity);
        }

        Destroy(gameObject);
    }
}