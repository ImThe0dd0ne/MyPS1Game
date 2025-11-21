using UnityEngine;
using UnityEngine.AI;

public class ImpPositionFixer : MonoBehaviour
{
    private NavMeshAgent agent;
    private bool isFixed = false;
    
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    
    private void Start()
    {
        if (agent != null && !isFixed)
        {
            Vector3 pos = transform.position;
            pos.y += 1.5f;
            
            if (agent.isOnNavMesh)
            {
                agent.Warp(pos);
            }
            else
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(pos, out hit, 10f, NavMesh.AllAreas))
                {
                    agent.Warp(hit.position);
                }
            }
            
            isFixed = true;
        }
    }
}
