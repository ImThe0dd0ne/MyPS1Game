using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public int attackDamage = 25;
    public float attackRange = 2f;
    public float attackCooldown = 0.5f;
    public LayerMask enemyLayer;

    [Header("Sword Reference")]
    public Transform swordTransform; // Drag your sword object here
    public Transform attackPoint; // Empty GameObject at the tip of sword

    [Header("Animation")]
    public Animator animator;

    [Header("Debug")]
    public bool showDebugGizmos = true;

    private bool canAttack = true;

    private void Start()
    {
        // Auto-find components if not assigned
        if (!animator) animator = GetComponent<Animator>();

        // Auto-find sword if not assigned
        if (!swordTransform)
        {
            Transform foundSword = transform.Find("Sword");
            if (foundSword) swordTransform = foundSword;
        }

        // Create attack point if it doesn't exist
        if (!attackPoint && swordTransform)
        {
            GameObject attackObj = new GameObject("AttackPoint");
            attackPoint = attackObj.transform;
            attackPoint.SetParent(swordTransform);
            attackPoint.localPosition = new Vector3(0, 0, 0.5f); // Adjust based on sword size
        }
    }

    private void Update()
    {
        // Left Mouse Button or specific key to attack
        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        canAttack = false;

        // Play attack animation
        if (animator)
        {
            animator.SetTrigger("Attack");
        }

        // Wait a bit for animation to reach the swing moment
        yield return new WaitForSeconds(0.2f);

        // Detect enemies in range
        DetectAndDamageEnemies();

        // Cooldown
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private void DetectAndDamageEnemies()
    {
        if (!attackPoint) return;

        // Find all enemies in attack range
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            // Check if enemy has the EnemyAI script
            EnemyAI enemyScript = enemy.GetComponent<EnemyAI>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(attackDamage);
                Debug.Log($"Hit {enemy.name} for {attackDamage} damage!");
            }
        }

        if (hitEnemies.Length > 0)
        {
            Debug.Log($"Hit {hitEnemies.Length} enemies!");
        }
    }

    // Visualize attack range in Scene view
    private void OnDrawGizmosSelected()
    {
        if (showDebugGizmos && attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}

