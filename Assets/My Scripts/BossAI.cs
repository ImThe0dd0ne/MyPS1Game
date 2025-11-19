using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class BossAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public NavMeshAgent agent;
    public Animator animator;

    [Header("Health")]
    public int maxHealth = 500;
    private int currentHealth;

    [Header("Movement")]
    public float runSpeed = 6f;
    public float attackRange = 3.5f;
    public float jumpAttackRange = 10f;
    public float jumpHeight = 4f;
    public float jumpSpeed = 15f;
    public float minJumpTravelTime = 0.6f;
    public float maxJumpTravelTime = 1.5f;

    [Header("Combat")]
    public float timeBetweenAttacks = 3f;
    public float jumpAttackCooldown = 5f;
    public int attackDamage = 30;
    public int jumpAttackDamage = 45;
    public float jumpAOERadius = 4f;
    public GameObject projectile;
    public Transform attackPoint;

    [Header("Animation")]
    public string paramSpeed = "Speed";
    public string trigSwipe = "Swipe";
    public string trigJump = "JumpAttack";

    [Header("Behavior")]
    [Range(0f, 1f)] public float jumpChance = 0.4f;
    public float landingRecoveryTime = 1f;

    [Header("Rewards")]
    public int xpReward = 200;
    public int soulReward = 100;

    [Header("Debug")]
    public bool showDebugLogs = false;

    private enum AIState { Chasing, JumpAttacking, Attacking, Dead }
    private AIState state = AIState.Chasing;

    private bool isPerformingAction = false;
    private bool alreadyAttacked = false;
    private float lastJumpTime = -999f;

    private HubZone hubZone;
    private EnemyHealthBar healthBar;

    void Start()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!player)
            player = GameObject.FindWithTag("Player")?.transform;

        agent.speed = runSpeed;
        agent.stoppingDistance = attackRange - 0.2f;
        agent.angularSpeed = 360f;
        agent.acceleration = 20f;
        agent.autoBraking = false;

        hubZone = FindFirstObjectByType<HubZone>();
        currentHealth = maxHealth;
        
        healthBar = gameObject.AddComponent<EnemyHealthBar>();
        if (healthBar != null)
        {
            healthBar.offset = new Vector3(0, 3.5f, 0);
            healthBar.barSize = new Vector2(2.5f, 0.2f);
        }

        if (showDebugLogs)
            Debug.Log($"Boss spawned with {maxHealth} HP");
    }

    void Update()
    {
        if (state == AIState.Dead || player == null) return;

        if (hubZone != null && hubZone.IsPlayerInHub())
        {
            agent.isStopped = true;
            if (animator != null)
                animator.SetFloat(paramSpeed, 0f);
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        Vector3 dir = player.position - transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude > 0.001f && !isPerformingAction)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir.normalized), 8f * Time.deltaTime);

        if (isPerformingAction) return;

        if (distance <= attackRange)
        {
            StartCoroutine(AttackRoutine());
        }
        else if (distance <= jumpAttackRange && CanJumpAttack())
        {
            if (UnityEngine.Random.value < jumpChance)
            {
                StartCoroutine(JumpAttackRoutine());
            }
            else
            {
                ChasePlayer();
            }
        }
        else
        {
            ChasePlayer();
        }

        if (animator != null)
            animator.SetFloat(paramSpeed, agent.velocity.magnitude);
    }

    private bool CanJumpAttack()
    {
        return (Time.time - lastJumpTime) >= jumpAttackCooldown;
    }

    private void ChasePlayer()
    {
        agent.isStopped = false;
        agent.speed = runSpeed;
        if (player != null)
            agent.SetDestination(player.position);
    }

    IEnumerator AttackRoutine()
    {
        isPerformingAction = true;
        state = AIState.Attacking;

        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        if (animator != null)
            animator.SetTrigger(trigSwipe);

        if (!alreadyAttacked)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
                if (showDebugLogs)
                    Debug.Log($"Boss swipe hit for {attackDamage} damage!");
            }

            if (projectile && attackPoint)
            {
                Rigidbody rb = Instantiate(projectile, attackPoint.position, attackPoint.rotation).GetComponent<Rigidbody>();
                rb.AddForce(transform.forward * 25f, ForceMode.Impulse);
            }

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }

        float swipeDuration = (animator != null)
            ? animator.GetCurrentAnimatorStateInfo(0).length
            : 1.2f;

        yield return new WaitForSeconds(swipeDuration);

        agent.isStopped = false;
        state = AIState.Chasing;
        isPerformingAction = false;
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    IEnumerator JumpAttackRoutine()
    {
        state = AIState.JumpAttacking;
        isPerformingAction = true;
        lastJumpTime = Time.time;

        agent.isStopped = true;
        agent.enabled = false;

        if (animator != null)
            animator.SetTrigger(trigJump);

        Vector3 start = transform.position;
        Vector3 targetPosition = player.position;
        start.y = transform.position.y;
        targetPosition.y = transform.position.y;

        Vector3 directionToTarget = (targetPosition - start).normalized;
        transform.rotation = Quaternion.LookRotation(directionToTarget);

        if (showDebugLogs)
            Debug.Log($"🦘 LEAP ATTACK! Target locked at {targetPosition}");

        float distance = Vector3.Distance(start, targetPosition);
        float travelTime = Mathf.Clamp(distance / jumpSpeed, minJumpTravelTime, maxJumpTravelTime);
        float elapsed = 0f;

        while (elapsed < travelTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / travelTime);
            float height = Mathf.Sin(t * Mathf.PI) * jumpHeight;
            Vector3 horiz = Vector3.Lerp(start, targetPosition, t);
            transform.position = new Vector3(horiz.x, start.y + height, horiz.z);

            yield return null;
        }

        transform.position = new Vector3(targetPosition.x, start.y, targetPosition.z);

        Collider[] hits = Physics.OverlapSphere(transform.position, jumpAOERadius);
        foreach (var c in hits)
        {
            if (c.CompareTag("Player"))
            {
                PlayerHealth playerHealth = c.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(jumpAttackDamage);
                    if (showDebugLogs)
                        Debug.Log($"💥 Boss leap attack hit for {jumpAttackDamage} damage!");
                }
            }
        }

        yield return new WaitForSeconds(landingRecoveryTime);

        agent.enabled = true;
        agent.Warp(transform.position);
        agent.isStopped = false;

        isPerformingAction = false;
        state = AIState.Chasing;
    }

    public void TakeDamage(int damage)
    {
        if (state == AIState.Dead) return;

        currentHealth -= damage;
        
        if (healthBar != null)
        {
            healthBar.UpdateHealth(currentHealth, maxHealth);
        }

        if (showDebugLogs)
            Debug.Log($"Boss took {damage} damage! Health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        state = AIState.Dead;
        isPerformingAction = true;
        agent.isStopped = true;
        agent.enabled = false;

        if (animator != null)
            animator.SetTrigger("Die");

        Debug.Log("💀 BOSS DEFEATED!");

        PlayerStats playerStats = FindFirstObjectByType<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.GainXP(xpReward);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.souls += soulReward;
        }

        Destroy(gameObject, 3f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, jumpAttackRange);

        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
        Gizmos.DrawSphere(transform.position, jumpAOERadius);
    }
}
