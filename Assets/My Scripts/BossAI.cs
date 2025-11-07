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

    [Header("Movement")]
    public float runSpeed = 6f;
    public float jumpAttackRange = 7f;
    public float attackRange = 3.5f;
    public float jumpHeight = 3f;
    public float jumpSpeed = 12f;          // horizontal speed for jump
    public float minJumpTravelTime = 0.35f;
    public float maxJumpTravelTime = 1.2f;

    [Header("Animation")]
    public string paramSpeed = "Speed";
    public string trigSwipe = "Swipe";
    public string trigJump = "JumpAttack";

    [Header("Behavior Chance")]
    [Range(0f, 1f)] public float jumpChance = 0.35f; // chance to jump

    private enum AIState { Chasing, JumpAttacking, Attacking, Dead }
    private AIState state = AIState.Chasing;
    private bool isPerformingAction = false;

    void Start()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();
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

        // Always face player
        Vector3 dir = (player.position - transform.position);
        dir.y = 0;
        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 8f * Time.deltaTime);
        }

        if (isPerformingAction) return;

        // Behavior selection
        if (distance <= attackRange)
        {
            StartCoroutine(SwipeRoutine());
        }
        else if (distance <= jumpAttackRange)
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

        // Animator blend
        if (animator != null && animator.HasParameter(paramSpeed))
            animator.SetFloat(paramSpeed, agent.velocity.magnitude);
    }

    IEnumerator SwipeRoutine()
    {
        state = AIState.Attacking;
        isPerformingAction = true; // general action flag

        // Re-check distance to player
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > attackRange)
        {
            state = AIState.Chasing;
            agent.isStopped = false;
            isPerformingAction = false;
            yield break;
        }

        // Stop movement
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        // Trigger animation
        if (animator != null)
            animator.SetTrigger(trigSwipe);

        // Wait for animation duration
        float swipeDuration = 1.2f; // match your swipe animation length
        yield return new WaitForSeconds(swipeDuration);

        // Resume chasing
        agent.isStopped = false;
        agent.SetDestination(player.position);
        state = AIState.Chasing;
        isPerformingAction = false;
    }


    IEnumerator JumpAttackRoutine()
    {
        state = AIState.JumpAttacking;
        isPerformingAction = true;

        // Stop agent movement but keep it enabled
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        // Start jump animation
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

            // Smooth vertical arc (sinusoidal)
            float height = Mathf.Sin(t * Mathf.PI) * jumpHeight;

            // Horizontal lerp
            Vector3 horiz = Vector3.Lerp(start, target, t);
            transform.position = new Vector3(horiz.x, start.y + height, horiz.z);

            // Gradually face player
            Vector3 lookDir = (player.position - transform.position);
            lookDir.y = 0;
            if (lookDir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(lookDir.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
            }

            yield return null;
        }

        // Snap precisely on ground plane at landing
        transform.position = new Vector3(target.x, start.y, target.z);

        // Resume chasing
        agent.isStopped = false;
        agent.SetDestination(player.position);

        isPerformingAction = false;
        state = AIState.Chasing;
    }

}

// Animator extension helper
public static class AnimatorExtensions
{
    public static bool HasParameter(this Animator animator, string paramName)
    {
        if (animator == null) return false;
        foreach (var p in animator.parameters)
            if (p.name == paramName) return true;
        return false;
    }
}
