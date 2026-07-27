using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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

    public float attackRadius = 0.5f;
    public int attackDamage = 1;

    [Header("Animación de Ataque")]
    public float attackDuration = 0.43f;

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
    public bool isPogoAttacking { get; private set; }

    [Header("Respawn")]
    public Transform respawnPoint;
    public float voidY = -20f;

    Rigidbody2D rb;
    Animator anim;

    bool isGrounded;
    float moveInput;
    bool canAttack = true;

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
        anim = GetComponent<Animator>();

        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        if (anim == null)
        {
            Debug.LogError("El jugador no tiene Animator");
        }
    }

    void Update()
    {
        //--------------------
        // Movimiento e Inputs
        //--------------------
        moveInput = 0;

        if (Input.GetKey(KeyCode.LeftArrow))
            moveInput = -1;

        if (Input.GetKey(KeyCode.RightArrow))
            moveInput = 1;

        // Girar
        if (moveInput > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        // Suelo
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(
                groundCheck.position,
                groundCheckRadius,
                groundLayer);
        }

        // Dirección de Ataque
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

        // Salto
        if (Input.GetKeyDown(KeyCode.Z) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        //--------------------
        // Ejecución de Ataques
        //--------------------
        if (Input.GetKeyDown(KeyCode.X))
        {
            StartAttack(currentAttackDirection);
        }

        // Vacío
        if (transform.position.y < voidY)
        {
            Respawn();
        }

        //--------------------
        // Debug Visual
        //--------------------
        if (currentAttackDirection == AttackDirection.Side && sideAttackPoint != null)
        {
            Debug.DrawLine(transform.position, sideAttackPoint.position, Color.yellow);
        }
        else if (currentAttackDirection == AttackDirection.Up && upAttackPoint != null)
        {
            Debug.DrawLine(transform.position, upAttackPoint.position, Color.yellow);
        }
        else if (currentAttackDirection == AttackDirection.Down && pogoAttackPoint != null)
        {
            Debug.DrawLine(transform.position, pogoAttackPoint.position, Color.yellow);
        }

        //--------------------
        // Actualización del Animator (Sigue ejecutándose siempre)
        //--------------------
        if (anim != null)
        {
            anim.SetBool("Run", Mathf.Abs(rb.velocity.x) > 0.1f);
            anim.SetBool("Grounded", isGrounded);
            anim.SetFloat("Y", rb.velocity.y);
        }
        Debug.Log(anim.GetCurrentAnimatorStateInfo(0).IsName("Zorro_SideAttack"));
    }

    void FixedUpdate()
    {
        if (isKnockedBack)
            return;

        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    //--------------------------------
    // ATAQUE LATERAL
    //--------------------------------
    void StartAttack(AttackDirection direction)
    {
        if (!canAttack)
            return;

        canAttack = false;

        currentAttackDirection = direction;

        if (direction == AttackDirection.Down)
            isPogoAttacking = true;

        anim.Play("Zorro_SideAttack");
        StartCoroutine(AttackRoutine());
    }


    public void SideAttackHit()
    {
        switch (currentAttackDirection)
        {
            case AttackDirection.Side:
                DealSideDamage();
                break;

            case AttackDirection.Up:
                DealUpDamage();
                break;

            case AttackDirection.Down:
                DealPogoDamage();
                break;
        }
    }

    IEnumerator AttackRoutine()
    {
        // Espera hasta que el Animator realmente entre en SideAttack
        while (!anim.GetCurrentAnimatorStateInfo(0).IsName("Zorro_SideAttack"))
            yield return null;

        // Espera hasta que la animación termine completamente
        while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            yield return null;

        anim.SetBool("IsAttacking", false);

        isPogoAttacking = false;

        canAttack = true;
    }
    void DealSideDamage()
    {
        if (sideAttackPoint == null)
            return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(sideAttackPoint.position, attackRadius);

        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();

            if (enemy != null)
            {
                Vector2 dir = (hit.transform.position - transform.position).normalized;
                enemy.TakeDamage(attackDamage, dir);
            }
        }
    }
    //--------------------------------
    // ATAQUE ARRIBA
    //--------------------------------
    void DealUpDamage()
    {
        

        if (upAttackPoint == null) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(upAttackPoint.position, attackRadius);

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
    void DealPogoDamage()
    {
        

        if (pogoAttackPoint == null) return;

        bool hitEnemy = false;
        Collider2D[] hits = Physics2D.OverlapCircleAll(pogoAttackPoint.position, attackRadius);

        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                hitEnemy = true;
                enemy.TakeDamage(attackDamage, Vector2.down);
            }

            PogoObject pogo = hit.GetComponent<PogoObject>();
            if (pogo != null)
            {
                hitEnemy = true;
            }
        }

        if (hitEnemy)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    System.Collections.IEnumerator PogoWindow()
    {
        isPogoAttacking = true;
        yield return new WaitForSeconds(0.2f);
        isPogoAttacking = false;
    }

    //--------------------------------
    // RECIBIR DAÑO
    //--------------------------------
    public void TakeDamage(int damage, Vector2 sourcePosition)
    {
        if (!canBeHit)
            return;

        currentHealth -= damage;

        if (healthBar != null)
            healthBar.value = currentHealth;

        Vector2 knockbackDir = transform.position.x > sourcePosition.x ? new Vector2(1f, 0.5f) : new Vector2(-1f, 0.5f);
        knockbackDir.Normalize();

        rb.velocity = Vector2.zero;
        isKnockedBack = true;
        rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);

        if (currentHealth <= 0)
        {
            Respawn();
        }

        StartCoroutine(IFrames());
        StartCoroutine(KnockbackRecovery());
    }

    System.Collections.IEnumerator KnockbackRecovery()
    {
        yield return new WaitForSeconds(0.2f);
        isKnockedBack = false;
    }

    //--------------------------------
    // I-FRAMES
    //--------------------------------
    System.Collections.IEnumerator IFrames()
    {
        canBeHit = false;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        float timer = 0;

        while (timer < iFrameDuration)
        {
            if (sr != null) sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(0.1f);
            timer += 0.1f;
        }

        if (sr != null) sr.enabled = true;
        canBeHit = true;
    }

    //--------------------------------
    // RESPAWN
    //--------------------------------
    void Respawn()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
            healthBar.value = currentHealth;

        if (respawnPoint != null)
            transform.position = respawnPoint.position;

        rb.velocity = Vector2.zero;

        MovingPlatform[] platforms = FindObjectsOfType<MovingPlatform>();
        foreach (MovingPlatform platform in platforms)
        {
            platform.ResetPlatform();
        }
    }

    //--------------------------------
    // GIZMOS
    //--------------------------------
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (sideAttackPoint != null) Gizmos.DrawWireSphere(sideAttackPoint.position, attackRadius);
        if (upAttackPoint != null) Gizmos.DrawWireSphere(upAttackPoint.position, attackRadius);
        if (pogoAttackPoint != null) Gizmos.DrawWireSphere(pogoAttackPoint.position, attackRadius);

        Gizmos.color = Color.green;

        if (groundCheck != null) Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        if (Application.isPlaying)
        {
            Gizmos.color = Color.yellow;

            switch (currentAttackDirection)
            {
                case AttackDirection.Side:
                    if (sideAttackPoint != null) Gizmos.DrawLine(transform.position, sideAttackPoint.position);
                    break;
                case AttackDirection.Up:
                    if (upAttackPoint != null) Gizmos.DrawLine(transform.position, upAttackPoint.position);
                    break;
                case AttackDirection.Down:
                    if (pogoAttackPoint != null) Gizmos.DrawLine(transform.position, pogoAttackPoint.position);
                    break;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<PlatformEffector2D>())
        {
            other.isTrigger = false;
        }
    }
}