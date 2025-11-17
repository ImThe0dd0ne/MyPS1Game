using UnityEngine;

public class SwordCollider : MonoBehaviour
{
    [Header("Sword Damage")]
    [SerializeField] private int damage = 25;

    private bool isAttacking = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!isAttacking) return;

        EnemyAI enemy = other.GetComponent<EnemyAI>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
    }

    // Call this to start the attack (enable damage)
    public void EnableDamage()
    {
        isAttacking = true;
    }

    // Call this to end the attack (disable damage)
    public void DisableDamage()
    {
        isAttacking = false;
    }

    // Optional: automatically enable/disable on enable/disable
    private void OnEnable()
    {
        // Uncomment if you want collider to automatically start attack
        // EnableDamage();
    }

    private void OnDisable()
    {
        // Uncomment if you want collider to automatically stop attack
        // DisableDamage();
    }
}
