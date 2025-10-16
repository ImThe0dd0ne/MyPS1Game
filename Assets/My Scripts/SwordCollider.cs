using UnityEngine;

public class SwordCollider : MonoBehaviour
{
    [Header("Sword Damage")]
    public int damage = 25;
    [SerializeField] private bool isAttacking = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!isAttacking) return;

        EnemyAI enemy = other.GetComponent<EnemyAI>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            // Debug logging removed to fix the conflict
        }
    }

    public void EnableDamage()
    {
        isAttacking = true;
    }

    public void DisableDamage()
    {
        isAttacking = false;
    }
}