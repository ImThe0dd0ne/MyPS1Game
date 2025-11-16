using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;

    [Header("Stats")]
    public float health = 100f;

    [Header("Patrol Settings")]
    public float walkPointRange = 10f;
    private Vector3 walkPoint;
    private bool walkPointSet;

    [Header("Combat Settings")]
    public float sightRange = 15f;
    public float attackRange = 2.5f;
    public float timeBetweenAttacks = 2f;
    private bool alreadyAttacked;
    public float dodgeChance = 0.2f;

    [Header("Projectile (Optional)")]
    public GameObject projectile;
    public Transform attackPoint;

    [Header("Animation")]
    public Animator animator;

    [Header("Audio")]
    public AudioSource audioSource;      
    public AudioClip attackSound;         


    [Header("Debug")]
    public bool showDebugLogs = false;

    private enum State { Patrol, Chase, Attack }
    private State currentState = State.Patrol;

    private void Awake()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!animator) animator = GetComponent<Animator>();
        if (!player)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj) player = playerObj.transform;
        }

        // Configure agent
        agent.updateRotation = false;
        agent.stoppingDistance = attackRange * 0.8f;

        if (animator) animator.applyRootMotion = false;

        if (showDebugLogs)
        {
            if (!player) UnityEngine.Debug.LogError("Player not found!");
            if (!animator) UnityEngine.Debug.LogWarning("Animator missing!");
        }
    }

    private void Update()
    {
        if (!player || !agent.isOnNavMesh) return;

        bool playerInSight = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        bool playerInAttack = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        // Handle state changes
        if (!playerInSight && !playerInAttack) SwitchState(State.Patrol);
        else if (playerInSight && !playerInAttack) SwitchState(State.Chase);
        else if (playerInAttack) SwitchState(State.Attack);

        // Run logic per state
        switch (currentState)
        {
            case State.Patrol: Patrol(); break;
            case State.Chase: Chase(); break;
            case State.Attack: Attack(); break;
        }

        UpdateAnimation();
        SmoothRotate();
    }

    private void SwitchState(State newState)
    {
        if (currentState == newState) return;
        if (showDebugLogs) UnityEngine.Debug.Log($"State: {currentState} → {newState}");
        currentState = newState;
    }

    // ---------------- PATROL ----------------
    private void Patrol()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
        {
            agent.isStopped = false;
            agent.SetDestination(walkPoint);

            float distance = Vector3.Distance(transform.position, walkPoint);
            if (distance < 1f)
            {
                walkPointSet = false;
                if (showDebugLogs) UnityEngine.Debug.Log("Reached patrol point");
            }
        }
    }

    private void SearchWalkPoint()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomPoint = transform.position + new Vector3(
                UnityEngine.Random.Range(-walkPointRange, walkPointRange),
                0,
                UnityEngine.Random.Range(-walkPointRange, walkPointRange)
            );

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 3f, NavMesh.AllAreas))
            {
                walkPoint = hit.position;
                walkPointSet = true;
                if (showDebugLogs) UnityEngine.Debug.Log($"New patrol point: {walkPoint}");
                return;
            }
        }
    }

    // ---------------- CHASE ----------------
    private void Chase()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    // ---------------- ATTACK ----------------
    private void Attack()
    {
        agent.isStopped = true;

        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion lookRot = Quaternion.LookRotation(direction);
            lookRot *= Quaternion.Euler(0, 180f, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 10f);
        }

        if (animator) animator.SetTrigger("Attack");

        
        if (audioSource && attackSound)
        {
            audioSource.PlayOneShot(attackSound);
        }


        if (!alreadyAttacked)
        {
            if (animator) animator.SetTrigger("Attack");

            PlayerHealth playerhealth = player.GetComponent<PlayerHealth>();
            if (playerhealth != null)
            {
                playerhealth.TakeDamage(20);
                if (showDebugLogs) UnityEngine.Debug.Log("Goblin hit player for 20 damage!");
            }

            if (projectile && attackPoint)
            {
                Rigidbody rb = Instantiate(projectile, attackPoint.position, attackPoint.rotation).GetComponent<Rigidbody>();
                rb.AddForce(transform.forward * 25f, ForceMode.Impulse);
            }

            if (showDebugLogs) UnityEngine.Debug.Log("Attacking!");

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);

            if (UnityEngine.Random.value < dodgeChance)
            {
                Vector3 dodgeDir = (transform.position - player.position).normalized;
                agent.Move(dodgeDir * 2f);
            }
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void UpdateAnimation()
    {
        if (!animator) return;

        float currentSpeed = agent.velocity.magnitude;
        float normalizedSpeed = Mathf.Clamp01(currentSpeed / agent.speed);
        animator.SetFloat("Speed", normalizedSpeed);

        if (showDebugLogs && Time.frameCount % 60 == 0)
            UnityEngine.Debug.Log($"Blend Tree Speed: {normalizedSpeed:F2}, Actual Speed: {currentSpeed:F2}");
    }

    private void SmoothRotate()
    {
        if (currentState == State.Attack) return;
        if (agent.velocity.sqrMagnitude < 0.1f) return;

        Vector3 direction = agent.velocity.normalized;

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion lookRot = Quaternion.LookRotation(direction);
            lookRot *= Quaternion.Euler(0, 180f, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 8f);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0) Destroy(gameObject);
    }

    private void OnGUI()
    {
        if (showDebugLogs && agent != null)
        {
            float currentSpeed = agent.velocity.magnitude;
            float normalizedSpeed = Mathf.Clamp01(currentSpeed / agent.speed);

            GUI.Box(new Rect(10, 10, 300, 100), "BLEND TREE DEBUG");
            GUI.Label(new Rect(20, 40, 280, 20), $"Speed Parameter: {normalizedSpeed:F2}");
            GUI.Label(new Rect(20, 60, 280, 20), $"Actual Velocity: {currentSpeed:F2}");
            GUI.Label(new Rect(20, 80, 280, 20), $"State: {currentState}");

            if (GUI.Button(new Rect(150, 40, 120, 30), "TEST WALK"))
            {
                animator.SetFloat("Speed", 1f);
                UnityEngine.Debug.Log("Manual: Speed = 1 (Walk)");
            }

            if (GUI.Button(new Rect(150, 75, 120, 30), "TEST IDLE"))
            {
                animator.SetFloat("Speed", 0f);
                UnityEngine.Debug.Log("Manual: Speed = 0 (Idle)");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        if (walkPointSet)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(walkPoint, 0.4f);
        }
    }
}
