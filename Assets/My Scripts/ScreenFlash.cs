using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFlash : MonoBehaviour
{
    public static ScreenFlash Instance { get; private set; }

    [Header("Settings")]
    public float flashDuration = 0.1f;
    public Color hitFlashColor = new Color(1f, 0f, 0f, 0.3f);

    private Image flashImage;
    private Canvas canvas;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        CreateFlashOverlay();
    }

    private void CreateFlashOverlay()
    {
        GameObject canvasObj = new GameObject("ScreenFlashCanvas");
        canvasObj.transform.SetParent(transform);
        
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        GameObject imageObj = new GameObject("FlashImage");
        imageObj.transform.SetParent(canvasObj.transform, false);
        
        flashImage = imageObj.AddComponent<Image>();
        flashImage.color = new Color(1, 1, 1, 0);
        
        RectTransform rt = flashImage.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;

        canvasObj.SetActive(true);
    }

    public void Flash(Color? color = null, float? duration = null)
    {
        if (flashImage == null) return;

        StopAllCoroutines();
        StartCoroutine(FlashRoutine(color ?? hitFlashColor, duration ?? flashDuration));
    }

    private IEnumerator FlashRoutine(Color color, float duration)
    {
        flashImage.color = color;
        
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(color.a, 0f, elapsed / duration);
            flashImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        flashImage.color = new Color(color.r, color.g, color.b, 0f);
    }
}
