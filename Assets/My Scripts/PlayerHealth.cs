using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    private bool isDead = false;
    public AudioClip deathSound;
    private AudioSource audioSource;
    public Transform respawnPoint;
    public float respawnDelay = 3f;

    void Start()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();
        
        if (CombatUI.Instance != null)
        {
            CombatUI.Instance.UpdateHealth(currentHealth, maxHealth);
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log("The player took damage. Current health: " + currentHealth);
        
        if (CombatUI.Instance != null)
        {
            CombatUI.Instance.UpdateHealth(currentHealth, maxHealth);
        }

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
        
        if (CombatUI.Instance != null)
        {
            CombatUI.Instance.UpdateHealth(currentHealth, maxHealth);
        }
    }

    void Die()
    {
        if (isDead) return; // Prevent multiple death calls

        PlayerStats stats = GetComponent<PlayerStats>();
        if (stats != null) stats.OnDeath();

        isDead = true;
        Debug.Log("Player has died!");

        // Play death animation
        Animator playerAnimator = GetComponent<Animator>();
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Die");
        }

        // Play death sound
        Debug.Log($"Trying to play death sound... audioSource={audioSource}, deathSound={(deathSound ? deathSound.name : "None")}");

        if (audioSource && deathSound)
        {
            audioSource.PlayOneShot(deathSound);
            Debug.Log("PlayOneShot called!");
        }
        else
        {
            Debug.LogWarning("❌ Could not play death sound — missing AudioSource or AudioClip.");
        }


        // Disable attack script immediately
        var playerAttack = GetComponent<PlayerAttack>();
        if (playerAttack) playerAttack.enabled = false;

        // Disable movement (but NOT CharacterController yet)
        // If you have a movement script, disable it here

        // Respawn after delay
        StartCoroutine(RespawnAfterDelay());
    }

    private IEnumerator RespawnAfterDelay()
    {
        Debug.Log($"Respawning in {respawnDelay} seconds...");
        yield return new WaitForSeconds(respawnDelay);

        Respawn();
    }

    private void Respawn()
    {
        Debug.Log("Player respawning...");

        // Disable CharacterController FIRST
        var controller = GetComponent<CharacterController>();
        bool hadController = false;
        if (controller != null)
        {
            controller.enabled = false;
            hadController = true;
        }

        // Find and move to respawn point
        GameObject respawnObj = GameObject.Find("RespawnPoint");
        if (respawnObj != null)
        {
            transform.position = respawnObj.transform.position;
            transform.rotation = respawnObj.transform.rotation;
            Debug.Log("Respawned at RespawnPoint: " + respawnObj.transform.position);
        }
        else
        {
            transform.position = new Vector3(0, 1, 0);
            transform.rotation = Quaternion.identity;
            Debug.Log("Respawned at default position (0, 1, 0)");
        }

        // Reset health and state
        currentHealth = maxHealth;
        isDead = false;

        if (hadController && controller != null)
        {
            controller.enabled = true;
        }

        Animator playerAnimator = GetComponent<Animator>();
        if (playerAnimator != null)
        {
            playerAnimator.Rebind();
            playerAnimator.Update(0f);
            playerAnimator.SetFloat("Speed", 0f);
        }

        var playerAttack = GetComponent<PlayerAttack>();
        if (playerAttack) playerAttack.enabled = true;
        
        if (CombatUI.Instance != null)
        {
            CombatUI.Instance.UpdateHealth(currentHealth, maxHealth);
        }

        Debug.Log("Respawn complete!");
    }

    void Update()
    {
        // Press K to test death animation
        if (Input.GetKeyDown(KeyCode.K))
        {
            Animator playerAnimator = GetComponent<Animator>();
            if (playerAnimator != null)
            {
                playerAnimator.SetFloat("Speed", 0f);
                playerAnimator.SetTrigger("Die");
                Debug.Log("Manual death test!");
            }
        }
    }
}