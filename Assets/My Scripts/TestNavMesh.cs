using UnityEngine;
using UnityEngine.AI;

public class NavMeshTest : MonoBehaviour
{
    NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        Debug.Log("Agent enabled: " + agent.enabled);
        Debug.Log("Agent isOnNavMesh: " + agent.isOnNavMesh);
        Debug.Log("Agent hasPath: " + agent.hasPath);

        // Try to move to a simple position
        Vector3 targetPos = transform.position + Vector3.forward * 5f;
        agent.SetDestination(targetPos);

        Debug.Log("Set destination to: " + targetPos);
    }

    void Update()
    {
        if (agent != null)
        {
            Debug.Log("Velocity: " + agent.velocity.magnitude);
            Debug.Log("Remaining distance: " + agent.remainingDistance);
            Debug.Log("Path status: " + agent.pathStatus);
        }
    }
}