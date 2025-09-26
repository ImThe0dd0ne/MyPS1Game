using UnityEngine;
using Debug = UnityEngine.Debug;

public class EnemyChase : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float stopDistance = 1.5f;
    public float rotationSpeed = 5f;

    [Header("Combat Settings")]
    public float attackDistance = 1f;
    public float attackCooldown = 2f;

    [Header("Health")]
    public int health = 100;

    private Transform player;
    private Rigidbody rb;
    private Animator animator;
    private float lastAttackTime;
    private bool isDead = false;

    void Start()
    {
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            UnityEngine.Debug.LogError("Rigidbody component missing! Please add a Rigidbody to " + gameObject.name);
        }

        // Get animator and disable root motion
        animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.applyRootMotion = false;
        }

        // Find player by tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            UnityEngine.Debug.LogError("Player not found! Make sure Player GameObject is tagged as 'Player'.");
        }
    }

    void Update()
    {
        // Exit early if no player found, no rigidbody, or dead
        if (player == null || rb == null || isDead) return;

        // Check if should die
        if (health <= 0 && !isDead)
        {
            Die();
            return;
        }

        // Calculate distance to player
        float distance = Vector3.Distance(transform.position, player.position);

        // Debug output (remove these lines once working)
        UnityEngine.Debug.Log($"Distance to player: {distance}, Stop Distance: {stopDistance}");

        // Check if close enough to attack
        if (distance <= attackDistance && Time.time - lastAttackTime > attackCooldown)
        {
            Attack();
        }
        // Move toward player if outside stop distance
        else if (distance > stopDistance)
        {
            UnityEngine.Debug.Log("Should be moving!");
            MoveTowardPlayer(distance);
        }
        else
        {
            // Close to player but not attacking - just idle
            if (animator != null)
            {
                animator.SetBool("isMoving", false);
                animator.SetBool("isAttacking", false);
            }
        }
    }

    void MoveTowardPlayer(float distance)
    {
        // Set moving animation
        if (animator != null)
        {
            animator.SetBool("isMoving", true);
            animator.SetBool("isAttacking", false);
        }

        // Calculate direction to player (only X and Z for ground movement)
        Vector3 direction = (player.position - transform.position);
        direction.y = 0; // Keep on ground level
        direction = direction.normalized;

        // Rotate towards player
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        // Move using transform (since rb.MovePosition wasn't working)
        Vector3 movement = direction * moveSpeed * Time.deltaTime;
        UnityEngine.Debug.Log($"Moving by: {movement}");
        transform.position += movement;
    }

    void Attack()
    {
        UnityEngine.Debug.Log("Attacking player!");

        if (animator != null)
        {
            animator.SetBool("isMoving", false);
            animator.SetBool("isAttacking", true);
        }

        lastAttackTime = Time.time;

        // Add actual attack logic here (damage player, etc.)
        // For example: player.GetComponent<PlayerHealth>()?.TakeDamage(10);
    }

    void Die()
    {
        UnityEngine.Debug.Log("Goblin died!");
        isDead = true;

        if (animator != null)
        {
            animator.SetBool("isMoving", false);
            animator.SetBool("isAttacking", false);
            animator.SetBool("isDead", true);
        }

        // Optional: Disable movement components
        if (rb != null) rb.isKinematic = true;

        // Optional: Destroy after delay
        // Destroy(gameObject, 5f);
    }

    // Public method to take damage (call from other scripts)
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;
        UnityEngine.Debug.Log($"Goblin took {damage} damage. Health: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    // Draw gizmos in scene view for debugging
    void OnDrawGizmosSelected()
    {
        // Draw stop distance
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stopDistance);

        // Draw attack distance
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
}