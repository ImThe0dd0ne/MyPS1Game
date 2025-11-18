using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Base Stats")]
    public int baseMaxHealth = 100;
    public int baseDamage = 25;
    public float baseMoveSpeed = 6.5f;

    [Header("Current Stats (Runtime)")]
    public int currentMaxHealth;
    public int currentHealth;
    public int currentDamage;
    public float currentMoveSpeed;

    [Header("Multipliers from Upgrades")]
    private float healthMultiplier = 1f;
    private float damageMultiplier = 1f;
    private float moveSpeedMultiplier = 1f;
    private float xpMultiplier = 1f;

    [Header("References")]
    private PlayerHealth playerHealth;
    private ThirdPersonPlayer movement;
    private PlayerAttack playerAttack;

    private void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        movement = GetComponent<ThirdPersonPlayer>();
        playerAttack = GetComponent<PlayerAttack>();

        RecalculateStats();

        if (playerHealth != null)
        {
            currentHealth = currentMaxHealth;
        }
    }

    public void RecalculateStats()
    {
        int permanentHealthBonus = GameManager.Instance ? GameManager.Instance.permanentHealthBonus : 0;
        int permanentDamageBonus = GameManager.Instance ? GameManager.Instance.permanentDamageBonus : 0;

        currentMaxHealth = Mathf.RoundToInt((baseMaxHealth + permanentHealthBonus) * healthMultiplier);
        currentDamage = Mathf.RoundToInt((baseDamage + permanentDamageBonus) * damageMultiplier);
        currentMoveSpeed = baseMoveSpeed * moveSpeedMultiplier;

        if (playerHealth != null)
        {
            playerHealth.maxHealth = currentMaxHealth;
        }

        if (movement != null)
        {
            movement.moveSpeed = currentMoveSpeed;
        }

        if (playerAttack != null)
        {
            for (int i = 0; i < playerAttack.comboDamage.Length; i++)
            {
                playerAttack.comboDamage[i] = Mathf.RoundToInt((baseDamage + permanentDamageBonus) * damageMultiplier * (1f + i * 0.4f));
            }
        }

        Debug.Log($"Stats recalculated - HP: {currentMaxHealth}, Damage: {currentDamage}, Speed: {currentMoveSpeed:F1}");
    }

    public void ApplyUpgrade(TemporaryUpgrade upgrade)
    {
        switch (upgrade.type)
        {
            case TemporaryUpgrade.UpgradeType.MaxHealth:
                healthMultiplier += upgrade.value;
                if (playerHealth != null)
                {
                    int healthIncrease = Mathf.RoundToInt(baseMaxHealth * upgrade.value);
                    currentHealth += healthIncrease;
                }
                break;

            case TemporaryUpgrade.UpgradeType.Damage:
                damageMultiplier += upgrade.value;
                break;

            case TemporaryUpgrade.UpgradeType.MoveSpeed:
                moveSpeedMultiplier += upgrade.value;
                break;

            case TemporaryUpgrade.UpgradeType.XPGain:
                xpMultiplier += upgrade.value;
                break;
        }

        RecalculateStats();
    }

    public void ResetTemporaryUpgrades()
    {
        healthMultiplier = 1f;
        damageMultiplier = 1f;
        moveSpeedMultiplier = 1f;
        xpMultiplier = 1f;

        RecalculateStats();
    }

    public int GainXP(int baseXP)
    {
        int actualXP = Mathf.RoundToInt(baseXP * xpMultiplier);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GainXP(actualXP);
        }

        return actualXP;
    }

    public void OnDeath()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerDeath();
        }

        ResetTemporaryUpgrades();
    }
}
