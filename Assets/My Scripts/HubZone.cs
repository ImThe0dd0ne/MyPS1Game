using UnityEngine;

public class HubZone : MonoBehaviour
{
    public static HubZone Instance { get; private set; }

    [Header("Hub Settings")]
    public Transform hubCenter;
    public float hubRadius = 15f;
    public bool showGizmo = true;

    [Header("References")]
    private Transform player;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        if (hubCenter == null)
        {
            hubCenter = transform;
        }
    }

    private void Update()
    {
        if (player == null) return;

        bool inHub = IsPlayerInHub();

        if (GameManager.Instance != null)
        {
            if (inHub && GameManager.Instance.isInArena)
            {
                OnPlayerEnterHub();
            }
        }
    }

    public bool IsPlayerInHub()
    {
        if (player == null || hubCenter == null) return false;

        float distance = Vector3.Distance(player.position, hubCenter.position);
        return distance <= hubRadius;
    }

    private void OnPlayerEnterHub()
    {
        Debug.Log("Player returned to Hub - Arena run ended");

        ArenaManager arena = FindFirstObjectByType<ArenaManager>();
        if (arena != null)
        {
            arena.StopArena();
        }
    }

    private void OnDrawGizmos()
    {
        if (!showGizmo || hubCenter == null) return;

        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.DrawSphere(hubCenter.position, hubRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(hubCenter.position, hubRadius);
    }
}
