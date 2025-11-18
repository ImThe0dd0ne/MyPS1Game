using UnityEngine;
using System.Collections;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void DoHitstop(float duration)
    {
        if (Time.timeScale == 0f) return;
        StartCoroutine(HitstopCoroutine(duration));
    }

    private IEnumerator HitstopCoroutine(float duration)
    {
        float originalTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = originalTimeScale;
    }

    public void SlowMotion(float scale, float duration)
    {
        StartCoroutine(SlowMotionCoroutine(scale, duration));
    }

    private IEnumerator SlowMotionCoroutine(float scale, float duration)
    {
        Time.timeScale = scale;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }
}
