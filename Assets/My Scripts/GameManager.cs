using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player Persistent Data")]
    public int playerLevel = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 100;

    [Header("Temporary Upgrades (Lost on Death)")]
    public List<TemporaryUpgrade> temporaryUpgrades = new List<TemporaryUpgrade>();

    [Header("Permanent Stats (From Dungeon)")]
    public int permanentHealthBonus = 0;
    public int permanentDamageBonus = 0;

    [Header("Currency")]
    public int souls = 0;

    [Header("Game State")]
    public GameMode currentMode = GameMode.Hub;
    public bool isInArena = false;

    [Header("References")]
    public Transform hubSpawnPoint;

    public enum GameMode
    {
        Hub,
        Arena,
        Dungeon
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void GainXP(int amount)
    {
        currentXP += amount;
        Debug.Log($"Gained {amount} XP. Total: {currentXP}/{xpToNextLevel}");

        while (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentXP -= xpToNextLevel;
        playerLevel++;
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.2f);

        Debug.Log($"LEVEL UP! Now level {playerLevel}");

        ArenaManager arenaManager = FindFirstObjectByType<ArenaManager>();
        if (arenaManager != null)
        {
            arenaManager.ShowUpgradeSelection();
        }
    }

    public void AddTemporaryUpgrade(TemporaryUpgrade upgrade)
    {
        temporaryUpgrades.Add(upgrade);
        Debug.Log($"Added temporary upgrade: {upgrade.upgradeName}");

        PlayerStats playerStats = FindFirstObjectByType<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.ApplyUpgrade(upgrade);
        }
    }

    public void ClearTemporaryUpgrades()
    {
        temporaryUpgrades.Clear();
        Debug.Log("Cleared all temporary upgrades (player died)");
    }

    public void OnPlayerDeath()
    {
        Debug.Log("Player died - returning to Hub and clearing temporary upgrades");

        ClearTemporaryUpgrades();

        playerLevel = 1;
        currentXP = 0;
        xpToNextLevel = 100;

        isInArena = false;
        currentMode = GameMode.Hub;
    }

    public void EnterArena()
    {
        currentMode = GameMode.Arena;
        isInArena = true;
        Debug.Log("Entered Arena mode");
    }

    public void ExitArena()
    {
        currentMode = GameMode.Hub;
        isInArena = false;
        Debug.Log("Exited Arena mode - returned to Hub");
    }

    public void AddSouls(int amount)
    {
        souls += amount;
        Debug.Log($"Gained {amount} souls. Total: {souls}");
    }

    public bool SpendSouls(int amount)
    {
        if (souls >= amount)
        {
            souls -= amount;
            Debug.Log($"Spent {amount} souls. Remaining: {souls}");
            return true;
        }

        Debug.Log($"Not enough souls! Need {amount}, have {souls}");
        return false;
    }
}

[System.Serializable]
public class TemporaryUpgrade
{
    public string upgradeName;
    public UpgradeType type;
    public float value;

    public enum UpgradeType
    {
        MaxHealth,
        Damage,
        MoveSpeed,
        AttackSpeed,
        XPGain
    }
}
