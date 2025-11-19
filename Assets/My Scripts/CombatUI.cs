using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatUI : MonoBehaviour
{
    public static CombatUI Instance { get; private set; }

    [Header("Health Bar")]
    public Image healthBarFill;
    public TextMeshProUGUI healthText;
    public Color healthColorHigh = Color.green;
    public Color healthColorMid = Color.yellow;
    public Color healthColorLow = Color.red;

    [Header("Combo Display")]
    public TextMeshProUGUI comboText;
    public float comboDisplayTime = 2f;
    private float comboTimer = 0f;

    [Header("Wave Info")]
    public TextMeshProUGUI waveText;

    [Header("XP/Level")]
    public Image xpBarFill;
    public TextMeshProUGUI levelText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (comboText != null)
            comboText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (comboTimer > 0f)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f && comboText != null)
            {
                comboText.gameObject.SetActive(false);
            }
        }
    }

    public void UpdateHealth(int current, int max)
    {
        if (healthBarFill != null)
        {
            float fillAmount = (float)current / max;
            healthBarFill.fillAmount = fillAmount;

            if (fillAmount > 0.6f)
                healthBarFill.color = healthColorHigh;
            else if (fillAmount > 0.3f)
                healthBarFill.color = healthColorMid;
            else
                healthBarFill.color = healthColorLow;
        }

        if (healthText != null)
        {
            healthText.text = $"{current}/{max}";
        }
    }

    public void ShowCombo(int comboCount)
    {
        if (comboText != null)
        {
            comboText.gameObject.SetActive(true);
            
            string comboLabel = comboCount switch
            {
                1 => "HIT!",
                2 => "COMBO!",
                3 => "FINISH!",
                _ => $"x{comboCount} COMBO!"
            };
            
            comboText.text = comboLabel;
            comboText.fontSize = 36 + (comboCount * 8);
            
            Color comboColor = comboCount switch
            {
                1 => new Color(1f, 1f, 1f, 1f),
                2 => new Color(1f, 0.8f, 0.2f, 1f),
                3 => new Color(1f, 0.3f, 0.3f, 1f),
                _ => new Color(1f, 0f, 0f, 1f)
            };
            
            comboText.color = comboColor;
            comboTimer = comboDisplayTime;
        }
    }

    public void ResetCombo()
    {
        comboText.gameObject.SetActive(false);
        comboText.text = "";
    }


    public void UpdateWave(int waveNumber, int enemiesRemaining)
    {
        if (waveText != null)
        {
            waveText.text = $"Wave {waveNumber}\nEnemies: {enemiesRemaining}";
        }
    }

    public void UpdateXP(int current, int max, int level)
    {
        if (xpBarFill != null)
        {
            xpBarFill.fillAmount = (float)current / max;
        }

        if (levelText != null)
        {
            levelText.text = $"Level {level}";
        }
    }
}
