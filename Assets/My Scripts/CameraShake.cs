using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [Header("Settings")]
    public float traumaReduction = 1.5f;

    private float trauma = 0f;
    private Camera mainCamera;
    private Vector3 originalPosition;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        mainCamera = Camera.main;
        if (mainCamera != null)
            originalPosition = mainCamera.transform.localPosition;
    }

    private void Update()
    {
        if (trauma > 0)
        {
            trauma = Mathf.Max(0f, trauma - traumaReduction * Time.deltaTime);
            ApplyShake();
        }
        else if (mainCamera != null)
        {
            mainCamera.transform.localPosition = originalPosition;
        }
    }

    public void ShakeCamera(float intensity, float duration)
    {
        trauma = Mathf.Min(1f, trauma + intensity);
    }

    private void ApplyShake()
    {
        if (mainCamera == null) return;

        float shake = trauma * trauma;

        float offsetX = Mathf.PerlinNoise(Time.time * 25f, 0f) * 2f - 1f;
        float offsetY = Mathf.PerlinNoise(0f, Time.time * 25f) * 2f - 1f;

        Vector3 offset = new Vector3(offsetX, offsetY, 0f) * shake * 0.3f;
        mainCamera.transform.localPosition = originalPosition + offset;
    }
}
