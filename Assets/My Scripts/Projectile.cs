using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 10;
    public GameObject owner;
    public GameObject impactEffectPrefab;
    public AudioClip impactSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == owner) return;

        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            SpawnImpactEffect();
            Destroy(gameObject);
        }
        else if (other.CompareTag("Ground") || other.gameObject.layer == LayerMask.NameToLayer("WhatIsGround"))
        {
            SpawnImpactEffect();
            Destroy(gameObject);
        }
    }

    private void SpawnImpactEffect()
    {
        if (impactEffectPrefab != null)
        {
            GameObject effect = Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }

        if (impactSound != null)
        {
            AudioSource.PlayClipAtPoint(impactSound, transform.position);
        }
    }
}
