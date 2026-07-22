using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;

    [Header("Suelo")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Ataques")]
    public Transform sideAttackPoint;
    public Transform upAttackPoint;
    public Transform pogoAttackPoint;
    public GameObject sideAttackVisual;
    public GameObject upAttackVisual;
    public GameObject pogoAttackVisual;

    public float attackRadius = 0.5f;
    public int attackDamage = 1;

    [Header("Vida")]
    public int maxHealth = 5;
    private int currentHealth;

    [Header("UI")]
    public Slider healthBar;

    [Header("Daño")]
    public float knockbackForce = 8f;
    public float iFrameDuration = 1f;

    bool canBeHit = true;
    bool isKnockedBack = false;

    [Header("Respawn")]
    public Transform respawnPoint;
    public float voidY = -20f;

    Rigidbody2D rb;

    bool isGrounded;
    float moveInput;
    public enum AttackDirection
    {
        Side,
        Up,
        Down
    }

    AttackDirection currentAttackDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        currentHealth = maxHealth;

        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
        sideAttackVisual.SetActive(false);
        upAttackVisual.SetActive(false);
        pogoAttackVisual.SetActive(false);
    }
    System.Collections.IEnumerator ShowAttackVisual(
    GameObject visual,
    float duration)
    {
        visual.SetActive(true);

        yield return new WaitForSeconds(duration);

        visual.SetActive(false);
    }

    void Update()
    {
        //--------------------
        // Movimiento
        //--------------------
        moveInput = 0;

        if (Input.GetKey(KeyCode.LeftArrow))
            moveInput = -1;

        if (Input.GetKey(KeyCode.RightArrow))
            moveInput = 1;

        //--------------------
        // Girar
        //--------------------
        if (moveInput > 0)
            transform.localScale = new Vector3(1, 1, 1);

        if (moveInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        //--------------------
        // Suelo
        //--------------------
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer);
        if (Input.GetKey(KeyCode.UpArrow))
        {
            currentAttackDirection = AttackDirection.Up;
        }
        else if (Input.GetKey(KeyCode.DownArrow) && !isGrounded)
        {
            currentAttackDirection = AttackDirection.Down;
        }
        else
        {
            currentAttackDirection = AttackDirection.Side;
        }

        //--------------------
        // Salto
        //--------------------
        if (Input.GetKeyDown(KeyCode.Z) && isGrounded)
        {
            rb.velocity =
                new Vector2(rb.velocity.x, jumpForce);
        }

        //--------------------
        // Ataques
        //--------------------
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                UpAttack();
            }
            else if (Input.GetKey(KeyCode.DownArrow) && !isGrounded)
            {
                PogoAttack();
            }
            else
            {
                SideAttack();
            }
        }

        //--------------------
        // Vacío
        //--------------------
        if (transform.position.y < voidY)
        {
            Respawn();
        }
        //---- visual del Ataque
        if (currentAttackDirection == AttackDirection.Side)
        {
            Debug.DrawLine(
                transform.position,
                sideAttackPoint.position,
                Color.yellow);
        }

        if (currentAttackDirection == AttackDirection.Up)
        {
            Debug.DrawLine(
                transform.position,
                upAttackPoint.position,
                Color.yellow);
        }

        if (currentAttackDirection == AttackDirection.Down)
        {
            Debug.DrawLine(
                transform.position,
                pogoAttackPoint.position,
                Color.yellow);
        }
    }

    void FixedUpdate()
    {
        if (isKnockedBack)
            return;

        rb.velocity =
            new Vector2(
                moveInput * moveSpeed,
                rb.velocity.y);
    }

    //--------------------------------
    // ATAQUE LATERAL
    //--------------------------------
    void SideAttack()
    {
        StartCoroutine(
           ShowAttackVisual(
               sideAttackVisual,
                       0.1f));
        Collider2D[] hits =
            Physics2D.OverlapCircleAll(
                sideAttackPoint.position,
                attackRadius);

        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();

            if (enemy != null)
            {
                Vector2 dir =
                    (hit.transform.position -
                    transform.position).normalized;

                enemy.TakeDamage(attackDamage, dir);
            }
        }
    }

    //--------------------------------
    // ATAQUE ARRIBA
    //--------------------------------
    void UpAttack()
    {
        StartCoroutine(
    ShowAttackVisual(
        upAttackVisual,
        0.1f));
        Collider2D[] hits =
            Physics2D.OverlapCircleAll(
                upAttackPoint.position,
                attackRadius);

        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(attackDamage, Vector2.up);
            }
        }
    }

    //--------------------------------
    // POGO
    //--------------------------------
    void PogoAttack()
    {
        StartCoroutine(
    ShowAttackVisual(
        pogoAttackVisual,
        0.1f));
        bool hitEnemy = false;

        Collider2D[] hits =
            Physics2D.OverlapCircleAll(
                pogoAttackPoint.position,
                attackRadius);

        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();

            if (enemy != null)
            {
                hitEnemy = true;

                enemy.TakeDamage(
                    attackDamage,
                    Vector2.down);
            }

            PogoObject pogo =
                hit.GetComponent<PogoObject>();

            if (pogo != null)
            {
                hitEnemy = true;
            }
        }

        if (hitEnemy)
        {
            rb.velocity = new Vector2(
                rb.velocity.x,
                jumpForce);
        }
    }

    //--------------------------------
    // RECIBIR DAÑO
    //--------------------------------
    public void TakeDamage(
    int damage,
    Vector2 sourcePosition)
    {
        Debug.Log("TakeDamage iniciado");

        Debug.Log("HealthBar = " + healthBar);
        Debug.Log("RespawnPoint = " + respawnPoint);
        Debug.Log("RB = " + rb);

        if (!canBeHit)
            return;

        currentHealth -= damage;

        healthBar.value = currentHealth;

        Vector2 knockbackDir;

        if (transform.position.x > sourcePosition.x)
        {
            knockbackDir = new Vector2(1f, 0.5f);
        }
        else
        {
            knockbackDir = new Vector2(-1f, 0.5f);
        }

        knockbackDir.Normalize();

        rb.velocity = Vector2.zero;
        isKnockedBack = true;
        rb.AddForce(
            knockbackDir * knockbackForce,
            ForceMode2D.Impulse);

        if (currentHealth <= 0)
        {
            Respawn();
        }

        StartCoroutine(IFrames());
        System.Collections.IEnumerator KnockbackRecovery()
        {
            yield return new WaitForSeconds(0.2f);

            isKnockedBack = false;
        }
        StartCoroutine(KnockbackRecovery());
    }

    //--------------------------------
    // I-FRAMES
    //--------------------------------
    System.Collections.IEnumerator IFrames()
    {
        canBeHit = false;

        SpriteRenderer sr =
            GetComponent<SpriteRenderer>();

        float timer = 0;

        while (timer < iFrameDuration)
        {
            sr.enabled = !sr.enabled;

            yield return new WaitForSeconds(0.1f);

            timer += 0.1f;
        }

        sr.enabled = true;

        canBeHit = true;
    }

    //--------------------------------
    // RESPAWN
    //--------------------------------
    void Respawn()
    {
        currentHealth = maxHealth;

        healthBar.value = currentHealth;

        transform.position =
            respawnPoint.position;

        rb.velocity = Vector2.zero;
    }

    //--------------------------------
    // GIZMOS
    //--------------------------------
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (sideAttackPoint != null)
            Gizmos.DrawWireSphere(
                sideAttackPoint.position,
                attackRadius);

        if (upAttackPoint != null)
            Gizmos.DrawWireSphere(
                upAttackPoint.position,
                attackRadius);

        if (pogoAttackPoint != null)
            Gizmos.DrawWireSphere(
                pogoAttackPoint.position,
                attackRadius);

        Gizmos.color = Color.green;

        if (groundCheck != null)
            Gizmos.DrawWireSphere(
                groundCheck.position,
                groundCheckRadius);
        //--------------------------------
        // Dirección de ataque actual
        //--------------------------------

        if (Application.isPlaying)
        {
            Gizmos.color = Color.yellow;

            switch (currentAttackDirection)
            {
                case AttackDirection.Side:

                    Gizmos.DrawLine(
                        transform.position,
                        sideAttackPoint.position);

                    break;

                case AttackDirection.Up:

                    Gizmos.DrawLine(
                        transform.position,
                        upAttackPoint.position);

                    break;

                case AttackDirection.Down:

                    Gizmos.DrawLine(
                        transform.position,
                        pogoAttackPoint.position);

                    break;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<PlatformEffector2D>())// Si salimos de la zona de activación del collider con el componente Platform Effector 2D
        {
            other.isTrigger = false;// Deshabilitar la propiedad isTrigger
        }
    }
}