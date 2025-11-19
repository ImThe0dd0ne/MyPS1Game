using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatUIBuilder : MonoBehaviour
{
    
    [ContextMenu("Build Combat UI")]
    public void BuildCombatUI()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("CombatCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        CombatUI combatUI = canvas.gameObject.GetComponent<CombatUI>();
        if (combatUI == null)
        {
            combatUI = canvas.gameObject.AddComponent<CombatUI>();
        }

        CreateHealthBar(canvas.transform, combatUI);
        CreateComboText(canvas.transform, combatUI);
        CreateWaveText(canvas.transform, combatUI);
        CreateXPBar(canvas.transform, combatUI);

        Debug.Log("✅ Combat UI Built Successfully! Check your Canvas.");
    }

    private void CreateHealthBar(Transform parent, CombatUI combatUI)
    {
        GameObject healthBarBG = new GameObject("HealthBarBackground");
        healthBarBG.transform.SetParent(parent, false);
        
        RectTransform bgRect = healthBarBG.AddComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0.02f, 0.96f);
        bgRect.anchorMax = new Vector2(0.02f, 0.96f);
        bgRect.pivot = new Vector2(0, 0.5f);
        bgRect.sizeDelta = new Vector2(180, 16);
        
        Image bgImage = healthBarBG.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);

        GameObject healthBarFill = new GameObject("HealthBarFill");
        healthBarFill.transform.SetParent(healthBarBG.transform, false);
        
        RectTransform fillRect = healthBarFill.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.sizeDelta = new Vector2(-2, -2);
        
        Image fillImage = healthBarFill.AddComponent<Image>();
        fillImage.color = Color.green;
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;
        
        combatUI.healthBarFill = fillImage;

        GameObject healthText = new GameObject("HealthText");
        healthText.transform.SetParent(healthBarBG.transform, false);
        
        RectTransform textRect = healthText.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        
        TextMeshProUGUI tmp = healthText.AddComponent<TextMeshProUGUI>();
        tmp.text = "100/100";
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontSize = 12;
        tmp.color = Color.white;
        tmp.fontStyle = FontStyles.Bold;
        
        combatUI.healthText = tmp;
    }

    private void CreateComboText(Transform parent, CombatUI combatUI)
    {
        GameObject comboObj = new GameObject("ComboText");
        comboObj.transform.SetParent(parent, false);
        
        RectTransform rect = comboObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.65f);
        rect.anchorMax = new Vector2(0.5f, 0.65f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(300, 80);
        
        TextMeshProUGUI tmp = comboObj.AddComponent<TextMeshProUGUI>();
        tmp.text = "COMBO x3!";
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontSize = 36;
        tmp.color = new Color(1f, 0.8f, 0f, 1f);
        tmp.fontStyle = FontStyles.Bold;
        tmp.outlineWidth = 0.25f;
        tmp.outlineColor = Color.black;
        
        combatUI.comboText = tmp;
        comboObj.SetActive(false);
    }

    private void CreateWaveText(Transform parent, CombatUI combatUI)
    {
        GameObject waveObj = new GameObject("WaveText");
        waveObj.transform.SetParent(parent, false);
        
        RectTransform rect = waveObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.98f, 0.96f);
        rect.anchorMax = new Vector2(0.98f, 0.96f);
        rect.pivot = new Vector2(1f, 0.5f);
        rect.sizeDelta = new Vector2(150, 60);
        
        TextMeshProUGUI tmp = waveObj.AddComponent<TextMeshProUGUI>();
        tmp.text = "Wave 1\nEnemies: 5";
        tmp.alignment = TextAlignmentOptions.Right;
        tmp.fontSize = 16;
        tmp.color = Color.white;
        tmp.fontStyle = FontStyles.Bold;
        tmp.outlineWidth = 0.15f;
        tmp.outlineColor = Color.black;
        
        combatUI.waveText = tmp;
    }

    private void CreateXPBar(Transform parent, CombatUI combatUI)
    {
        GameObject xpBarBG = new GameObject("XPBarBackground");
        xpBarBG.transform.SetParent(parent, false);
        
        RectTransform bgRect = xpBarBG.AddComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0.5f, 0.03f);
        bgRect.anchorMax = new Vector2(0.5f, 0.03f);
        bgRect.pivot = new Vector2(0.5f, 0);
        bgRect.sizeDelta = new Vector2(300, 14);
        
        Image bgImage = xpBarBG.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);

        GameObject xpBarFill = new GameObject("XPBarFill");
        xpBarFill.transform.SetParent(xpBarBG.transform, false);
        
        RectTransform fillRect = xpBarFill.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.sizeDelta = new Vector2(-2, -2);
        
        Image fillImage = xpBarFill.AddComponent<Image>();
        fillImage.color = new Color(0.3f, 0.6f, 1f, 1f);
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;
        
        combatUI.xpBarFill = fillImage;

        GameObject levelText = new GameObject("LevelText");
        levelText.transform.SetParent(xpBarBG.transform, false);
        
        RectTransform textRect = levelText.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        
        TextMeshProUGUI tmp = levelText.AddComponent<TextMeshProUGUI>();
        tmp.text = "Level 1";
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontSize = 11;
        tmp.color = Color.white;
        tmp.fontStyle = FontStyles.Bold;
        
        combatUI.levelText = tmp;
    }
}
