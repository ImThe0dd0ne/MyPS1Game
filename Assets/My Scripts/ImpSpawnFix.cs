using UnityEngine;
using UnityEngine.AI;

public class ImpSpawnFix : MonoBehaviour
{
    private void Start()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        
        if (agent != null)
        {
            Vector3 currentPos = transform.position;
            
            if (agent.isOnNavMesh)
            {
                agent.Warp(currentPos + Vector3.up * 1.5f);
                Debug.Log($"<color=green>✅ Imp spawned and warped up 1.5 units to prevent ground clipping!</color>");
            }
            else
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(currentPos, out hit, 10f, NavMesh.AllAreas))
                {
                    Vector3 fixedPos = hit.position + Vector3.up * 1.5f;
                    agent.Warp(fixedPos);
                    Debug.Log($"<color=green>✅ Imp found NavMesh and warped to: {fixedPos}</color>");
                }
                else
                {
                    Debug.LogError($"<color=red>❌ Imp at {currentPos} - NO NAVMESH FOUND within 10 units!</color>");
                    Debug.LogError("<color=red>NAVMESH IS NOT BAKED OR SPAWN AREA NOT COVERED!</color>");
                }
            }
        }
    }
}
