using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;

    [Header("Enemy Stats")]
    public float health = 100f;

    [Header("Patrolling")]
    public float walkPointRange = 10f;
    private Vector3 walkPoint;
    private bool walkPointSet;

    [Header("Combat")]
    public float sightRange = 15f;
    public float chaseRange = 15f;
    public float attackRange = 2.5f;
    public float timeBetweenAttacks = 2f;
    private bool alreadyAttacked;

    public GameObject projectile;
    public Transform attackPoint;

    [Header("Animation")]
    public Animator animator;

    private void Awake()
    {
        player = GameObject.Find("Player")?.transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (animator != null)
            animator.applyRootMotion = false; // Movement handled by NavMeshAgent

        if (agent != null)
        {
            agent.speed = 3.5f;
            agent.acceleration = 8f;
            agent.angularSpeed = 200f;
            agent.stoppingDistance = attackRange * 0.8f;
            agent.updateRotation = false; // We’ll handle rotation manually
            agent.isStopped = false;
        }
    }

    private void Update()
    {
        if (player == null) return;

        // Check for sight and attack ranges
        bool playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        bool playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        // Decide state
        if (!playerInSightRange && !playerInAttackRange)
            Patroling();
        else if (playerInSightRange && !playerInAttackRange)
            ChasePlayer();
        else if (playerInAttackRange)
            AttackPlayer();

        // Update animation speed
        if (animator != null)
        {
            float speed = agent.velocity.magnitude;
            animator.SetFloat("Speed", speed); // Blend tree for idle/walk/run
        }

        // Smoothly rotate toward movement direction
        if (agent.velocity.sqrMagnitude > 0.1f && !playerInAttackRange)
        {
            Quaternion rot = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10f);
        }
    }

    // --- PATROLLING ---
    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet && agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        Vector3 potentialPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(potentialPoint + Vector3.up, -Vector3.up, 3f, whatIsGround))
        {
            walkPoint = potentialPoint;
            walkPointSet = true;
        }
    }

    // --- CHASING ---
    private void ChasePlayer()
    {
        if (!agent.isOnNavMesh) return;

        agent.isStopped = false;
        agent.SetDestination(player.position);

        // Face player while chasing
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;
        if (dir != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 8f);
        }
    }

    // --- ATTACKING ---
    private void AttackPlayer()
    {
        if (!agent.isOnNavMesh) return;

        agent.isStopped = true;

        // Face the player
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;
        if (dir != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 10f);
        }

        if (!alreadyAttacked)
        {
            if (animator != null)
                animator.SetTrigger("Attack");

            if (projectile != null && attackPoint != null)
            {
                Rigidbody rb = Instantiate(projectile, attackPoint.position, attackPoint.rotation).GetComponent<Rigidbody>();
                rb.AddForce(transform.forward * 30f, ForceMode.Impulse);
            }

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;

        // Resume chasing if player moves away
        if (Vector3.Distance(transform.position, player.position) > attackRange)
            agent.isStopped = false;
    }

    // --- DAMAGE & DEATH ---
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    // --- DEBUG VISUALS ---
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
