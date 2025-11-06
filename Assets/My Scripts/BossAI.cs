using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Debug = UnityEngine.Debug;

public class BossAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public NavMeshAgent agent;

    [Header("Detection")]
    public float detectionRadius = 10f;
    public float attackRange = 3.5f;

    [Header("Movement")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float wanderRadius = 5f;
    public float minWanderTime = 3f;
    public float maxWanderTime = 7f;
    public float turnSpeed = 6f;

    [Header("Combat")]
    public float attackCooldown = 2f;
    public float roarDuration = 2f;

    [Header("Jump Attack Settings")]
    public float jumpHeight = 8f;
    public float jumpDuration = 1.5f;
    public float maxJumpDistance = 15f;
    public float jumpAnticipationTime = 0.8f;
    public float jumpAttackMinDistance = 6f;
    [Range(0f, 1f)] public float jumpAttackChance = 0.3f; // 30% chance to jump when conditions are met

    private enum AIState { Wandering, Roaring, Chasing, Attacking, Jumping, Dead }
    private AIState currentState = AIState.Wandering;

    private Animator animator;
    private Vector3 wanderCenter;
    private Vector3 wanderTarget;
    private float wanderTimer;
    private float currentWanderTime;
    private float nextAttackTime;
    private float roarTimer;

    // Jump attack variables
    private Vector3 jumpTarget;
    private bool isJumping = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (!agent) agent = GetComponent<NavMeshAgent>();

        if (!player)
        {
            player = GameObject.FindWithTag("Player")?.transform;
            if (!player)
            {
                Debug.LogError("BossAI: No player found!");
                enabled = false;
                return;
            }
        }

        agent.speed = walkSpeed;
        agent.stoppingDistance = attackRange - 0.3f;
        agent.updateRotation = false;

        wanderCenter = transform.position;
        SetNewWanderTarget();

        Debug.Log("BossAI initialized and wandering.");
    }

    void Update()
    {
        if (currentState == AIState.Dead || isJumping) return;

        float dist = Vector3.Distance(transform.position, player.position);

        // Debug distance info (less frequent to reduce spam)
        if (Time.frameCount % 120 == 0) // Log every 2 seconds
        {
            Debug.Log($"Distance to player: {dist:F1} | State: {currentState} | CanJump: {CanJumpAttack(dist)}");
        }

        switch (currentState)
        {
            case AIState.Wandering:
                UpdateWander(dist);
                break;
            case AIState.Roaring:
                UpdateRoar(dist);
                break;
            case AIState.Chasing:
                UpdateChase(dist);
                break;
            case AIState.Attacking:
                UpdateAttack(dist);
                break;
            case AIState.Jumping:
                // handled by coroutine
                break;
        }

        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    // ---------------------- STATES ---------------------- //

    void UpdateWander(float distance)
    {
        if (distance <= detectionRadius)
        {
            StartRoar();
            return;
        }

        wanderTimer += Time.deltaTime;
        bool reached = !agent.pathPending && agent.remainingDistance <= 0.5f;

        if (reached || wanderTimer >= currentWanderTime)
            SetNewWanderTarget();

        // Rotate toward wander direction
        if (agent.hasPath)
        {
            Vector3 dir = agent.steeringTarget - transform.position;
            dir.y = 0;
            if (dir.sqrMagnitude > 0.01f)
            {
                Quaternion lookRot = Quaternion.LookRotation(dir.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * turnSpeed);
            }
        }
    }

    void UpdateRoar(float distance)
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        SmoothLookAt(player.position);

        roarTimer += Time.deltaTime;
        if (roarTimer >= roarDuration)
        {
            currentState = AIState.Chasing;
            agent.isStopped = false;
            Debug.Log("Roar finished → chasing player.");
        }
    }

    void UpdateChase(float distance)
    {
        if (distance > detectionRadius * 1.3f)
        {
            currentState = AIState.Wandering;
            SetNewWanderTarget();
            Debug.Log("Player lost → back to wander.");
            return;
        }

        // Check for jump attack opportunity while chasing
        if (CanJumpAttack(distance) && Time.time >= nextAttackTime && ShouldJumpAttack())
        {
            StartJumpAttack();
            return;
        }

        if (distance <= attackRange)
        {
            currentState = AIState.Attacking;
            agent.isStopped = true;
            animator.SetFloat("Speed", 0);
            return;
        }

        agent.isStopped = false;
        agent.speed = runSpeed;
        agent.SetDestination(player.position);
        SmoothLookAt(player.position);
    }

    void UpdateAttack(float distance)
    {
        SmoothLookAt(player.position);

        // Check for jump attack opportunity
        if (CanJumpAttack(distance) && Time.time >= nextAttackTime && ShouldJumpAttack())
        {
            StartJumpAttack();
            return;
        }

        // if player got too far, resume chase
        if (distance > attackRange * 1.5f)
        {
            currentState = AIState.Chasing;
            agent.isStopped = false;
            return;
        }

        // wait for cooldown
        if (Time.time < nextAttackTime) return;

        // Normal attack
        animator.SetTrigger("Attack");
        nextAttackTime = Time.time + attackCooldown;
        Debug.Log("Normal attack triggered");
    }

    bool CanJumpAttack(float distance)
    {
        return distance >= jumpAttackMinDistance && distance <= maxJumpDistance && !isJumping;
    }

    bool ShouldJumpAttack()
    {
        // Random chance to jump based on jumpAttackChance
        bool shouldJump = UnityEngine.Random.value <= jumpAttackChance;
        if (shouldJump)
        {
            Debug.Log($"🎲 Jump attack rolled: {UnityEngine.Random.value:F2} <= {jumpAttackChance} = YES");
        }
        else
        {
            Debug.Log($"🎲 Jump attack rolled: {UnityEngine.Random.value:F2} <= {jumpAttackChance} = NO (normal attack)");
        }
        return shouldJump;
    }

    // ---------------------- JUMP ATTACK ---------------------- //

    void StartJumpAttack()
    {
        if (isJumping) return;

        Debug.Log($"🚀 STARTING JUMP ATTACK! Distance: {Vector3.Distance(transform.position, player.position):F1}");
        currentState = AIState.Jumping;
        StartCoroutine(JumpAttack());
        nextAttackTime = Time.time + attackCooldown * 1.5f;
    }

    IEnumerator JumpAttack()
    {
        isJumping = true;
        agent.isStopped = true;

        // Use separate animation trigger for jump
        animator.SetTrigger("JumpAttack");

        // Calculate jump target with prediction
        Vector3 playerVelocity = Vector3.zero;
        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            playerVelocity = playerRb.linearVelocity;
        }

        // Predict where player will be
        jumpTarget = player.position + playerVelocity * jumpAnticipationTime;
        jumpTarget.y = transform.position.y;

        // Clamp jump distance
        float distanceToTarget = Vector3.Distance(transform.position, jumpTarget);
        if (distanceToTarget > maxJumpDistance)
        {
            Vector3 direction = (jumpTarget - transform.position).normalized;
            jumpTarget = transform.position + direction * maxJumpDistance;
        }

        Debug.Log($"🎯 Jump target calculated: {jumpTarget} | Distance: {distanceToTarget:F1}");

        // Anticipation phase - boss prepares to jump
        float anticipationElapsed = 0f;
        Vector3 lookTarget = jumpTarget;

        while (anticipationElapsed < jumpAnticipationTime)
        {
            anticipationElapsed += Time.deltaTime;
            SmoothLookAt(lookTarget);
            yield return null;
        }

        // Launch phase - disable agent and perform the jump
        bool wasAgentEnabled = agent.enabled;
        agent.enabled = false;

        Vector3 startPos = transform.position;
        float jumpElapsed = 0f;

        Debug.Log("🚀 LAUNCHING!");

        while (jumpElapsed < jumpDuration)
        {
            jumpElapsed += Time.deltaTime;
            float t = jumpElapsed / jumpDuration;

            // More dramatic curve using quadratic easing
            float horizontalT = 1f - Mathf.Pow(1f - t, 2f); // Ease out
            Vector3 horizontalPos = Vector3.Lerp(startPos, jumpTarget, horizontalT);

            // Higher arc with more pronounced curve
            float verticalT = Mathf.Sin(t * Mathf.PI);
            horizontalPos.y += verticalT * jumpHeight;

            transform.position = horizontalPos;

            // Always look at the direction of movement
            Vector3 moveDirection = (jumpTarget - startPos).normalized;
            if (moveDirection.sqrMagnitude > 0.1f)
            {
                Quaternion lookRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed * 2f);
            }

            yield return null;
        }

        // Land at exact target position
        transform.position = new Vector3(jumpTarget.x, transform.position.y, jumpTarget.z);

        // Impact effects
        Debug.Log("💥 BOSS LANDS WITH IMPACT!");

        // Recovery phase
        yield return new WaitForSeconds(0.5f);

        // Re-enable navigation
        agent.enabled = wasAgentEnabled;
        if (agent.enabled)
        {
            agent.Warp(transform.position);
            agent.isStopped = false;
        }

        // Return to chasing
        currentState = AIState.Chasing;
        isJumping = false;

        Debug.Log("Jump attack completed!");
    }

    // ---------------------- HELPERS ---------------------- //

    void SetNewWanderTarget()
    {
        Vector3 randomDir = UnityEngine.Random.insideUnitSphere * wanderRadius;
        randomDir += wanderCenter;
        randomDir.y = transform.position.y;

        if (NavMesh.SamplePosition(randomDir, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
        {
            wanderTarget = hit.position;
            agent.SetDestination(wanderTarget);
            agent.isStopped = false;
            agent.speed = walkSpeed;

            wanderTimer = 0f;
            currentWanderTime = UnityEngine.Random.Range(minWanderTime, maxWanderTime);
        }
    }

    void StartRoar()
    {
        currentState = AIState.Roaring;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        roarTimer = 0f;
        animator.SetTrigger("Roar");
        Debug.Log("Player detected → roaring!");
    }

    void SmoothLookAt(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude < 0.001f) return;
        Quaternion rot = Quaternion.LookRotation(dir.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * turnSpeed);
    }

    // ---------------------- Death ---------------------- //

    public void Die()
    {
        currentState = AIState.Dead;
        agent.isStopped = true;
        animator.SetTrigger("Die");
        enabled = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Draw jump range
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, maxJumpDistance);

        // Draw jump min distance
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, jumpAttackMinDistance);
    }
}