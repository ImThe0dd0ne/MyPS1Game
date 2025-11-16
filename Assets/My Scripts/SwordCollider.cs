using UnityEngine;

public class SwordCollider : MonoBehaviour
{
    [Header("Sword Damage")]
    [SerializeField] private int damage = 25;

    [Header("Trail Reference")]
    [SerializeField] private SwordBladeTrail bladeTrail; // assign in inspector

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

    // Automatically called when collider becomes enabled
    private void OnEnable()
    {
        StartSwing();
    }

    // Automatically called when collider becomes disabled
    private void OnDisable()
    {
        EndSwing();
    }

    private void StartSwing()
    {
        isAttacking = true;

        if (bladeTrail != null)
            bladeTrail.EnableTrail();
    }

    private void EndSwing()
    {
        isAttacking = false;

        if (bladeTrail != null)
            bladeTrail.DisableTrail();
    }
}
