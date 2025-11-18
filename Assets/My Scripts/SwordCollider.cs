using UnityEngine;

public class SwordCollider : MonoBehaviour
{
    [Header("Sword Damage")]
    [SerializeField] private int damage = 25;
    [SerializeField] private float knockbackForce = 4f;

    private bool isAttacking = false;
    private Transform playerTransform;

    private void Start()
    {
        playerTransform = GetComponentInParent<PlayerAttack>()?.transform;
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isAttacking) return;

        EnemyAI enemy = other.GetComponent<EnemyAI>();
        if (enemy != null && playerTransform != null)
        {
            Vector3 hitDirection = (other.transform.position - playerTransform.position).normalized;
            enemy.TakeDamage(damage, hitDirection, knockbackForce);
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

    private void OnDisable()
    {
        DisableDamage();
    }
}
