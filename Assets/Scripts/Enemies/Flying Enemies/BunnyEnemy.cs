using UnityEngine;

public class BunnyEnemy : FlyingEnemy
{
    enum BunnyState
    {
        Patrol,
        Chase,
        WindUp,
        Dash,
        Return,
        Cooldown
    }

    [Header("Detección")]
    public float detectRange = 6f;

    [Header("Persecución")]
    public float chaseRange = 2.5f;

    [Header("Embestida")]
    public float dashSpeed = 10f;
    public float dashDuration = 0.5f;

    [Header("Preparación")]
    public float windUpTime = 0.25f;
    public float windUpSpeed = 2f;

    [Header("Cooldown")]
    public float cooldownTime = 1f;

    [Header("Patrulla")]
    public float patrolHeight = 1f;
    public float patrolSpeed = 2f;

    BunnyState currentState;

    Vector2 dashStartPosition;
    Vector2 dashDirection;

    float stateTimer;
    float windUpTimer;

    protected override void Start()
    {
        base.Start();

        currentState = BunnyState.Patrol;
    }

    void FixedUpdate()
    {
        if (player == null)
            return;

        switch (currentState)
        {
            case BunnyState.Patrol:
                Patrol();
                break;

            case BunnyState.Chase:
                ChasePlayer();
                break;

            case BunnyState.WindUp:
                WindUp();
                break;

            case BunnyState.Dash:
                Dash();
                break;

            case BunnyState.Return:
                ReturnToStart();
                break;

            case BunnyState.Cooldown:
                Cooldown();
                break;
        }
    }

    // --------------------------------
    // PATRULLA
    // --------------------------------

    void Patrol()
    {
        float yOffset =
            Mathf.Sin(Time.time * patrolSpeed)
            * patrolHeight;

        transform.position =
            new Vector2(
                startPosition.x,
                startPosition.y + yOffset);

        float distance =
            Vector2.Distance(
                transform.position,
                player.transform.position);

        if (distance <= detectRange)
        {
            currentState = BunnyState.Chase;
        }
    }

    // --------------------------------
    // PERSEGUIR
    // --------------------------------

    void ChasePlayer()
    {
        FlyTowards(player.transform.position);

        float distance =
            Vector2.Distance(
                transform.position,
                player.transform.position);

        if (distance <= chaseRange)
        {
            dashStartPosition =
                transform.position;

            dashDirection =
                (player.transform.position -
                transform.position).normalized;

            windUpTimer = windUpTime;

            currentState =
                BunnyState.WindUp;
        }
    }

    // --------------------------------
    // PREPARAR EMBESTIDA
    // --------------------------------

    void WindUp()
    {
        // Se aleja un poco del jugador
        rb.velocity =
            -dashDirection * windUpSpeed;

        windUpTimer -= Time.fixedDeltaTime;

        if (windUpTimer <= 0)
        {
            stateTimer = dashDuration;

            currentState =
                BunnyState.Dash;
        }
    }

    // --------------------------------
    // EMBESTIDA
    // --------------------------------

    void Dash()
    {
        rb.velocity =
            dashDirection *
            dashSpeed;

        stateTimer -=
            Time.fixedDeltaTime;

        if (stateTimer <= 0)
        {
            StopFlying();

            currentState =
                BunnyState.Return;
        }
    }

    // --------------------------------
    // VOLVER
    // --------------------------------

    void ReturnToStart()
    {
        FlyTowards(
            dashStartPosition);

        float distance =
            Vector2.Distance(
                transform.position,
                dashStartPosition);

        if (distance <= 0.2f)
        {
            StopFlying();

            stateTimer =
                cooldownTime;

            currentState =
                BunnyState.Cooldown;
        }
    }
    // --------------------------------
    // RESETEO DE CHASE
    // --------------------------------
    protected override void OnEnemyReset()
    {
        base.OnEnemyReset();

        currentState = BunnyState.Patrol;

        dashDirection = Vector2.zero;

        stateTimer = 0f;
        windUpTimer = 0f;

        StopFlying();
    }

    // --------------------------------
    // DESCANSO
    // --------------------------------

    void Cooldown()
    {
        StopFlying();

        stateTimer -=
            Time.fixedDeltaTime;

        if (stateTimer <= 0)
        {
            currentState =
                BunnyState.Chase;
        }
    }
}