using UnityEngine;
using System.Collections;

public class TimeManager : MonoBehaviour
{
    // Singleton instance
    public static TimeManager Instance { get; private set; }

    private void Awake()
    {
        // Setup singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: keeps it between scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate
        }
    }

    public void DoHitstop(float duration)
    {
        if (!gameObject.activeInHierarchy) return;
        StartCoroutine(HitstopRoutine(duration));
    }

    private IEnumerator HitstopRoutine(float t)
    {
        float prev = Time.timeScale;
        Time.timeScale = 0.02f;
        yield return new WaitForSecondsRealtime(t);
        Time.timeScale = prev;
    }
}