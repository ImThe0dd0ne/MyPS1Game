using UnityEngine;

public class SwordParticleTrail : MonoBehaviour
{
    [Header("Sword Damage")]
    [SerializeField] private int damage = 25;
    [SerializeField] private float knockbackForce = 4f;
    [SerializeField] private bool isAttacking = false;

    [Header("Particle Trail")]
    [SerializeField] private ParticleSystem tipParticles;
    [SerializeField] private ParticleSystem baseParticles;

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
        EnableTrail();
    }

    public void DisableDamage()
    {
        isAttacking = false;
        DisableTrail();
    }

    private void EnableTrail()
    {
        if (tipParticles != null)
        {
            var emission = tipParticles.emission;
            emission.enabled = true;
        }

        if (baseParticles != null)
        {
            var emission = baseParticles.emission;
            emission.enabled = true;
        }
    }

    private void DisableTrail()
    {
        if (tipParticles != null)
        {
            var emission = tipParticles.emission;
            emission.enabled = false;
        }

        if (baseParticles != null)
        {
            var emission = baseParticles.emission;
            emission.enabled = false;
        }
    }
}
