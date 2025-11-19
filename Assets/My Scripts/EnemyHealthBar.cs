using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("Settings")]
    public Vector3 offset = new Vector3(0, 2.5f, 0);
    public Vector2 barSize = new Vector2(1.5f, 0.15f);
    public Color healthColorHigh = Color.green;
    public Color healthColorLow = Color.red;

    private Canvas canvas;
    private Image backgroundImage;
    private Image fillImage;
    private RectTransform canvasRect;

    private void Start()
    {
        CreateHealthBar();
    }

    private void CreateHealthBar()
    {
        GameObject canvasObj = new GameObject("HealthBarCanvas");
        canvasObj.transform.SetParent(transform);
        canvasObj.transform.localPosition = offset;

        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;

        canvasRect = canvas.GetComponent<RectTransform>();
        canvasRect.sizeDelta = barSize;
        canvasRect.localScale = Vector3.one * 0.01f;

        GameObject backgroundObj = new GameObject("Background");
        backgroundObj.transform.SetParent(canvasObj.transform, false);
        backgroundImage = backgroundObj.AddComponent<Image>();
        backgroundImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        
        RectTransform bgRect = backgroundImage.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;

        GameObject fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(backgroundObj.transform, false);
        fillImage = fillObj.AddComponent<Image>();
        fillImage.color = healthColorHigh;
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;
        
        RectTransform fillRect = fillImage.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.sizeDelta = new Vector2(-4, -4);
    }

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        if (fillImage == null) return;

        float fillAmount = currentHealth / maxHealth;
        fillImage.fillAmount = fillAmount;

        fillImage.color = Color.Lerp(healthColorLow, healthColorHigh, fillAmount);

        if (fillAmount <= 0)
        {
            canvas.gameObject.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        if (canvas != null && Camera.main != null)
        {
            canvas.transform.LookAt(Camera.main.transform);
            canvas.transform.Rotate(0, 180, 0);
        }
    }
}
