using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArenaManager : MonoBehaviour
{
    [Header("Wave Settings")]
    public int currentWave = 0;
    public int enemiesPerWave = 3;
    public float waveScaling = 1.3f;
    public float timeBetweenWaves = 5f;

    [Header("Spawning")]
    public GameObject[] enemyPrefabs;
    public Transform player;
    public float spawnRadius = 20f;
    public float minSpawnDistance = 10f;
    public LayerMask groundLayer;

    [Header("Boss Settings")]
    public GameObject bossObject;
    public int bossWaveInterval = 5;
    public Transform bossSpawnPoint;

    [Header("State")]
    public bool arenaActive = false;
    private int enemiesAlive = 0;
    private bool waitingForNextWave = false;
    private bool bossActive = false;

    [Header("Upgrades")]
    public TemporaryUpgrade[] availableUpgrades;
    private bool upgradePanelOpen = false;

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        if (bossObject != null)
        {
            bossObject.SetActive(false);
        }

        InitializeUpgrades();
    }

    private void InitializeUpgrades()
    {
        availableUpgrades = new TemporaryUpgrade[]
        {
            new TemporaryUpgrade { upgradeName = "Vitality Boost", type = TemporaryUpgrade.UpgradeType.MaxHealth, value = 0.2f },
            new TemporaryUpgrade { upgradeName = "Power Surge", type = TemporaryUpgrade.UpgradeType.Damage, value = 0.25f },
            new TemporaryUpgrade { upgradeName = "Swift Strikes", type = TemporaryUpgrade.UpgradeType.AttackSpeed, value = 0.15f },
            new TemporaryUpgrade { upgradeName = "Momentum", type = TemporaryUpgrade.UpgradeType.MoveSpeed, value = 0.15f },
            new TemporaryUpgrade { upgradeName = "Soul Collector", type = TemporaryUpgrade.UpgradeType.XPGain, value = 0.3f },
        };
    }

    public void StartArena()
    {
        arenaActive = true;
        currentWave = 0;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.EnterArena();
        }

        Debug.Log("Arena started!");
        StartNextWave();
    }

    public void StopArena()
    {
        arenaActive = false;
        currentWave = 0;

        StopAllCoroutines();

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }

        if (bossObject != null && bossActive)
        {
            bossObject.SetActive(false);
            bossActive = false;

            BossAI bossAI = bossObject.GetComponent<BossAI>();
            if (bossAI != null)
            {
                bossAI.enabled = false;
            }
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ExitArena();
        }

        Debug.Log("Arena stopped!");
    }

    private void StartNextWave()
    {
        if (!arenaActive) return;

        currentWave++;

        if (currentWave % bossWaveInterval == 0 && bossObject != null)
        {
            StartBossWave();
        }
        else
        {
            StartRegularWave();
        }
    }

    private void StartBossWave()
    {
        Debug.Log($"🔥 BOSS WAVE {currentWave}! THE GOLEM AWAKENS! 🔥");

        Vector3 spawnPos = bossSpawnPoint != null ? bossSpawnPoint.position : player.position + new Vector3(15f, 0f, 0f);

        bossObject.transform.position = spawnPos;
        bossObject.SetActive(true);
        bossActive = true;

        BossAI bossAI = bossObject.GetComponent<BossAI>();
        if (bossAI != null)
        {
            bossAI.enabled = true;
            enemiesAlive = 1;
        }

        waitingForNextWave = false;
    }

    private void StartRegularWave()
    {
        int enemiesToSpawn = Mathf.RoundToInt(enemiesPerWave * Mathf.Pow(waveScaling, currentWave - 1));
        Debug.Log($"Starting Wave {currentWave} - Spawning {enemiesToSpawn} enemies");
        StartCoroutine(SpawnWave(enemiesToSpawn));
    }

    private IEnumerator SpawnWave(int count)
    {
        enemiesAlive = count;

        for (int i = 0; i < count; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(0.5f);
        }

        waitingForNextWave = false;
    }

    private void SpawnEnemy()
    {
        if (player == null || enemyPrefabs.Length == 0) return;

        Vector3 spawnPos = GetRandomSpawnPosition();

        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.player = player;

            float healthScale = 1f + (currentWave - 1) * 0.2f;
            enemyAI.health *= healthScale;
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        for (int i = 0; i < 20; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle.normalized;
            float distance = Random.Range(minSpawnDistance, spawnRadius);

            Vector3 offset = new Vector3(randomCircle.x, 0, randomCircle.y) * distance;
            Vector3 spawnPos = player.position + offset;

            if (Physics.Raycast(spawnPos + Vector3.up * 5f, Vector3.down, out RaycastHit hit, 10f, groundLayer))
            {
                return hit.point + Vector3.up * 0.5f;
            }
        }

        return player.position + new Vector3(minSpawnDistance, 0, 0);
    }

    public void OnEnemyKilled()
    {
        enemiesAlive--;

        Debug.Log($"Enemy killed! {enemiesAlive} remaining in wave {currentWave}");

        if (enemiesAlive <= 0 && !waitingForNextWave && arenaActive)
        {
            if (bossActive)
            {
                bossObject.SetActive(false);
                bossActive = false;
                Debug.Log("💀 BOSS DEFEATED! Wave complete!");
            }

            waitingForNextWave = true;
            StartCoroutine(WaitForNextWave());
        }
    }

    private IEnumerator WaitForNextWave()
    {
        Debug.Log($"Wave {currentWave} complete! Next wave in {timeBetweenWaves} seconds...");
        yield return new WaitForSeconds(timeBetweenWaves);

        if (arenaActive)
        {
            StartNextWave();
        }
    }

    public void ShowUpgradeSelection()
    {
        if (upgradePanelOpen) return;

        upgradePanelOpen = true;
        Time.timeScale = 0f;

        Debug.Log("=== LEVEL UP! Choose an upgrade: ===");
        TemporaryUpgrade[] choices = GetRandomUpgrades(3);

        for (int i = 0; i < choices.Length; i++)
        {
            Debug.Log($"{i + 1}. {choices[i].upgradeName} (+{choices[i].value * 100}% {choices[i].type})");
        }

        Debug.Log("Press 1, 2, or 3 to choose (temporary - for testing)");

        StartCoroutine(WaitForUpgradeChoice(choices));
    }

    private TemporaryUpgrade[] GetRandomUpgrades(int count)
    {
        List<TemporaryUpgrade> shuffled = new List<TemporaryUpgrade>(availableUpgrades);

        for (int i = 0; i < shuffled.Count; i++)
        {
            int randomIndex = Random.Range(i, shuffled.Count);
            TemporaryUpgrade temp = shuffled[i];
            shuffled[i] = shuffled[randomIndex];
            shuffled[randomIndex] = temp;
        }

        TemporaryUpgrade[] result = new TemporaryUpgrade[count];
        for (int i = 0; i < count && i < shuffled.Count; i++)
        {
            result[i] = shuffled[i];
        }

        return result;
    }

    private IEnumerator WaitForUpgradeChoice(TemporaryUpgrade[] choices)
    {
        while (upgradePanelOpen)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            {
                SelectUpgrade(choices[0]);
                break;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            {
                SelectUpgrade(choices[1]);
                break;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
            {
                SelectUpgrade(choices[2]);
                break;
            }

            yield return null;
        }
    }

    private void SelectUpgrade(TemporaryUpgrade upgrade)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddTemporaryUpgrade(upgrade);
        }

        upgradePanelOpen = false;
        Time.timeScale = 1f;

        Debug.Log($"Selected: {upgrade.upgradeName}");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B) && !arenaActive)
        {
            StartArena();
        }

        if (Input.GetKeyDown(KeyCode.N) && arenaActive)
        {
            StopArena();
        }
    }
}
