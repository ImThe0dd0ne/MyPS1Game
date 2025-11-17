using UnityEngine;

public class SwordParticleTrail : MonoBehaviour
{
    [Header("Sword Damage")]
    [SerializeField] private int damage = 25;
    [SerializeField] private bool isAttacking = false;

    [Header("Particle Trail")]
    [SerializeField] private ParticleSystem tipParticles;
    [SerializeField] private ParticleSystem baseParticles;

    private void OnTriggerEnter(Collider other)
    {
        if (!isAttacking) return;

        EnemyAI enemy = other.GetComponent<EnemyAI>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
    }

    // Called via Animation Events or Attack Script
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
