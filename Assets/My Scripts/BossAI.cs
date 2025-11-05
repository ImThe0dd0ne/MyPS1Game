using UnityEngine;
using UnityEngine.AI;

public class BossAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public NavMeshAgent agent;

    [Header("Detection")]
    public float detectionRadius = 8f;
    public float attackRange = 4f;

    [Header("Movement")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float wanderRadius = 5f;
    public float minWanderTime = 3f;
    public float maxWanderTime = 8f;

    [Header("Combat")]
    public float attackCooldown = 2f;

    // States
    private enum AIState { Wandering, Chasing, Attacking, Dead }
    private AIState currentState = AIState.Wandering;

    // Components
    private Animator animator;

    // Wandering
    private Vector3 wanderCenter;
    private Vector3 wanderTarget;
    private float wanderTimer;
    private float currentWanderTime;

    // Combat
    private float nextAttackTime;

    void Start()
    {
        animator = GetComponent<Animator>();

        // Get NavMeshAgent if not assigned
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
            if (agent == null)
            {
                Debug.LogError("BOSS AI: No NavMeshAgent found! Please add NavMeshAgent component.");
                return;
            }
        }

        // Manual player assignment check
        if (player == null)
        {
            Debug.LogError("BOSS AI: No player assigned! Please drag player into Player field.");
            return;
        }

        // Setup NavMeshAgent
        agent.speed = walkSpeed;
        agent.stoppingDistance = attackRange - 0.5f;
        agent.angularSpeed = 120f;
        agent.acceleration = 8f;

        // Initialize wandering
        wanderCenter = transform.position;
        SetNewWanderTarget();

        Debug.Log("Golem AI Started: Wandering Mode | Player: " + player.name);
        Debug.Log("NavMeshAgent: " + agent.name + " | Speed: " + agent.speed);
    }

    void Update()
    {
        if (player == null)
        {
            Debug.LogWarning("BOSS AI: Player reference lost!");
            return;
        }

        if (agent == null)
        {
            Debug.LogWarning("BOSS AI: NavMeshAgent reference lost!");
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Debug distance occasionally
        if (Time.frameCount % 60 == 0)
        {
            Debug.Log("Distance to player: " + distanceToPlayer + " | State: " + currentState + " | Agent Speed: " + agent.velocity.magnitude);
        }

        // State Machine
        switch (currentState)
        {
            case AIState.Wandering:
                UpdateWandering(distanceToPlayer);
                break;
            case AIState.Chasing:
                UpdateChasing(distanceToPlayer);
                break;
            case AIState.Attacking:
                UpdateAttacking(distanceToPlayer);
                break;
            case AIState.Dead:
                break;
        }

        // Update animator speed
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    void UpdateWandering(float distanceToPlayer)
    {
        // Check if player enters detection radius
        if (distanceToPlayer <= detectionRadius)
        {
            currentState = AIState.Chasing;
            agent.isStopped = true;
            animator.SetTrigger("Roar");
            
            Debug.Log("PLAYER DETECTED! Distance: " + distanceToPlayer + " | Switching to chase mode");
            return;
        }





        // Continue wandering
        wanderTimer += Time.deltaTime;

        if (!agent.pathPending && agent.remainingDistance <= 0.5f || wanderTimer >= currentWanderTime)
        {
            SetNewWanderTarget();
        }
    }

    void UpdateChasing(float distanceToPlayer)
    {
        // Player left detection radius
        if (distanceToPlayer > detectionRadius * 1.2f)
        {
            currentState = AIState.Wandering;
            SetNewWanderTarget();
            Debug.Log("Player lost. Returning to wandering");
            return;
        }

        // In attack range
        if (distanceToPlayer <= attackRange)
        {
            currentState = AIState.Attacking;
            agent.isStopped = true;
            animator.SetFloat("Speed", 0);
            Debug.Log("In attack range!");
            return;
        }

        // Chase player
        agent.isStopped = false;
        agent.speed = runSpeed;
        agent.SetDestination(player.position);

        // Face player
        Vector3 lookDirection = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(lookDirection);
    }

    void UpdateAttacking(float distanceToPlayer)
    {
        // Player moved out of attack range
        if (distanceToPlayer > attackRange * 1.1f)
        {
            currentState = AIState.Chasing;
            agent.isStopped = false;
            Debug.Log("Player out of range, resuming chase");
            return;
        }

        // Face player
        Vector3 lookDirection = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(lookDirection);

        // Attack if cooldown ready
        if (Time.time >= nextAttackTime)
        {
            PerformAttack();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    void SetNewWanderTarget()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += wanderCenter;
        randomDirection.y = transform.position.y;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1))
        {
            wanderTarget = hit.position;
            agent.SetDestination(wanderTarget);
            agent.speed = walkSpeed;
            agent.isStopped = false;

            currentWanderTime = Random.Range(minWanderTime, maxWanderTime);
            wanderTimer = 0f;

            Debug.Log("New wander target: " + wanderTarget);
        }
    }

    void PerformAttack()
    {
        int attackType = Random.Range(0, 3);

        switch (attackType)
        {
            case 0:
            case 1:
                animator.SetTrigger("Attack");
                break;
            case 2:
                animator.SetTrigger("JumpAtta");
                break;
        }

        Debug.Log("Boss attacking! Type: " + attackType);
    }

    public void Die()
    {
        currentState = AIState.Dead;
        animator.SetTrigger("Die");
        agent.isStopped = true;
        enabled = false;
    }

    public void EndRoar()
    {
        agent.isStopped = false;
        Debug.Log("Roar finished — resuming chase!");
    }


    void OnDrawGizmosSelected()
    {
        // Detection radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Current target line
        if (Application.isPlaying)
        {
            if (currentState == AIState.Wandering)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, wanderTarget);
            }
            else if (currentState == AIState.Chasing && player != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, player.position);
            }
        }
    }
}