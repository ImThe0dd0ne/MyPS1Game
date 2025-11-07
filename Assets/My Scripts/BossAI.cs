using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BossAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public NavMeshAgent agent;
    public Animator animator;

    [Header("Movement")]
    public float runSpeed = 6f;
    public float attackRange = 3.5f;
    public float jumpAttackRange = 7f;
    public float jumpHeight = 3f;
    public float jumpSpeed = 12f;
    public float minJumpTravelTime = 0.35f;
    public float maxJumpTravelTime = 1.2f;

    [Header("Combat")]
    public float timeBetweenAttacks = 2f;
    public int attackDamage = 30;
    public GameObject projectile;      // Optional projectile
    public Transform attackPoint;

    [Header("Animation")]
    public string paramSpeed = "Speed";
    public string trigSwipe = "Swipe";
    public string trigJump = "JumpAttack";

    [Header("Behavior Chance")]
    [Range(0f, 1f)] public float jumpChance = 0.35f;

    [Header("Jump Recovery")]
    public float landingRecoveryTime = 0.5f;

    [Header("Debug")]
    public bool showDebugLogs = false;

    private enum AIState { Chasing, JumpAttacking, Attacking, Dead }
    private AIState state = AIState.Chasing;

    private bool isPerformingAction = false;
    private bool isJumping = false;
    private bool alreadyAttacked = false;

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
    }

    void Update()
    {
        if (state == AIState.Dead || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Always face the player
        Vector3 dir = player.position - transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir.normalized), 8f * Time.deltaTime);

        // Block all actions if performing something
        if (isPerformingAction) return;

        if (!isJumping && distance <= attackRange)
        {
            StartCoroutine(AttackRoutine());
        }
        else if (!isJumping && distance <= jumpAttackRange)
        {
            if (Random.value < jumpChance)
                StartCoroutine(JumpAttackRoutine());
            else
            {
                agent.isStopped = false;
                agent.speed = runSpeed;
                agent.SetDestination(player.position);
            }
        }
        else
        {
            agent.isStopped = false;
            agent.speed = runSpeed;
            agent.SetDestination(player.position);
        }

        if (animator != null)
            animator.SetFloat(paramSpeed, agent.velocity.magnitude);
    }

    // ----------------------- SWIPE / ATTACK ----------------------- //
    IEnumerator AttackRoutine()
    {
        isPerformingAction = true;
        state = AIState.Attacking;

        // Stop agent
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        // Trigger animation
        if (animator != null)
            animator.SetTrigger(trigSwipe);

        // Damage player
        if (!alreadyAttacked)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
                if (showDebugLogs) Debug.Log($"Boss hit player for {attackDamage} damage!");
            }

            if (projectile && attackPoint)
            {
                Rigidbody rb = Instantiate(projectile, attackPoint.position, attackPoint.rotation).GetComponent<Rigidbody>();
                rb.AddForce(transform.forward * 25f, ForceMode.Impulse);
            }

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }

        float swipeDuration = animator != null
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

    // ----------------------- JUMP ATTACK ----------------------- //
    IEnumerator JumpAttackRoutine()
    {
        state = AIState.JumpAttacking;
        isPerformingAction = true;
        isJumping = true;

        // Stop agent during jump
        agent.isStopped = true;
        agent.enabled = false;

        // Trigger jump animation
        if (animator != null)
            animator.SetTrigger(trigJump);

        Vector3 start = transform.position;
        Vector3 target = player.position;
        start.y = transform.position.y;
        target.y = transform.position.y;

        float distance = Vector3.Distance(start, target);
        float travelTime = Mathf.Clamp(distance / jumpSpeed, minJumpTravelTime, maxJumpTravelTime);

        float elapsed = 0f;

        while (elapsed < travelTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / travelTime);

            // Vertical arc
            float height = Mathf.Sin(t * Mathf.PI) * jumpHeight;

            // Horizontal lerp
            Vector3 horiz = Vector3.Lerp(start, target, t);
            transform.position = new Vector3(horiz.x, start.y + height, horiz.z);

            // Face player while airborne
            Vector3 lookDir = player.position - transform.position;
            lookDir.y = 0;
            if (lookDir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(lookDir.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
            }

            yield return null;
        }

        // Snap to final position
        transform.position = new Vector3(target.x, start.y, target.z);

        // Small landing recovery to prevent instant swipe
        yield return new WaitForSeconds(landingRecoveryTime);

        // Re-enable agent
        agent.enabled = true;
        agent.Warp(transform.position);
        agent.isStopped = false;

        isJumping = false;
        isPerformingAction = false;
        state = AIState.Chasing;
    }

    // ----------------------- DAMAGE ----------------------- //
    public void TakeDamage(int damage)
    {
        // You can add health logic here
        if (showDebugLogs) Debug.Log($"Boss took {damage} damage!");
    }
}
