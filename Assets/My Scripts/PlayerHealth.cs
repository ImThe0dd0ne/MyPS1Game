using UnityEngine;
using System.Collections;
public Transform respawnPoint;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    // ADD THESE VARIABLES
    private bool isDead = false;
    public AudioClip deathSound;
    private AudioSource audioSource;
    public Transform respawnPoint; // Drag your respawn point here
    public float respawnDelay = 3f;

    void Start()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log("The player took damage. Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log("The player has healed. Current health: " + currentHealth);
    }

    void Die()
    {
        isDead = true;
        Debug.Log("Player has died!");

        // Play death animation
        Animator playerAnimator = GetComponent<Animator>();
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Die");
        }

        // Play death sound
        if (audioSource && deathSound)
        {
            audioSource.PlayOneShot(deathSound);
        }

        // Disable player controls
        var controller = GetComponent<CharacterController>();
        if (controller) controller.enabled = false;

        // Disable attack script
        var playerAttack = GetComponent<PlayerAttack>();
        if (playerAttack) playerAttack.enabled = false;

        // Respawn after delay
        StartCoroutine(RespawnAfterDelay());
    }

    // ADD THIS METHOD
    private IEnumerator RespawnAfterDelay()
    {
        Debug.Log($"Respawning in {respawnDelay} seconds...");
        yield return new WaitForSeconds(respawnDelay);

        Respawn();
    }

    // ADD THIS METHOD
    private void Respawn()
    {
        Debug.Log("Player respawned!");

        // Reset health
        currentHealth = maxHealth;
        isDead = false;

        // Move to respawn point
        if (respawnPoint != null)
        {
            transform.position = respawnPoint.position;
            transform.rotation = respawnPoint.rotation;
        }

        // Reset animator
        Animator playerAnimator = GetComponent<Animator>();
        if (playerAnimator != null)
        {
            playerAnimator.SetFloat("Speed", 0f);
            playerAnimator.Rebind();
        }

        // Re-enable controls
        var controller = GetComponent<CharacterController>();
        if (controller) controller.enabled = true;

        // Re-enable attack
        var playerAttack = GetComponent<PlayerAttack>();
        if (playerAttack) playerAttack.enabled = true;
    }
}