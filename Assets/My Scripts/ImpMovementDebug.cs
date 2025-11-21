using UnityEngine;
using UnityEngine.AI;

public class ImpMovementDebug : MonoBehaviour
{
    private NavMeshAgent agent;
    private Vector3 lastPosition;
    private float checkInterval = 1f;
    private float timer = 0f;
    
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        lastPosition = transform.position;
        
        if (agent != null)
        {
            Debug.Log("╔══════════════════════════════════════════════════════════╗");
            Debug.Log("║         IMP MOVEMENT DEBUG - DETAILED ANALYSIS          ║");
            Debug.Log("╚══════════════════════════════════════════════════════════╝");
            Debug.Log($"<color=cyan>Agent Speed: {agent.speed}</color>");
            Debug.Log($"<color=cyan>Agent Acceleration: {agent.acceleration}</color>");
            Debug.Log($"<color=cyan>Agent Angular Speed: {agent.angularSpeed}</color>");
            Debug.Log($"<color=cyan>Agent Update Position: {agent.updatePosition}</color>");
            Debug.Log($"<color=cyan>Agent Update Rotation: {agent.updateRotation}</color>");
            Debug.Log($"<color=cyan>Agent Obstacle Avoidance: {agent.obstacleAvoidanceType}</color>");
            Debug.Log($"<color=cyan>Agent Area Mask: {agent.areaMask}</color>");
            Debug.Log($"<color=cyan>Agent Auto Braking: {agent.autoBraking}</color>");
            Debug.Log($"<color=cyan>Agent Radius: {agent.radius}</color>");
            Debug.Log($"<color=cyan>Agent Height: {agent.height}</color>");
            Debug.Log($"<color=cyan>Agent Base Offset: {agent.baseOffset}</color>");
            
            MonoBehaviour[] allScripts = GetComponents<MonoBehaviour>();
            Debug.Log($"\n<color=yellow>All Components on Imp ({allScripts.Length}):</color>");
            foreach (var script in allScripts)
            {
                if (script != null)
                {
                    Debug.Log($"  ✅ {script.GetType().Name} (enabled: {script.enabled})");
                }
                else
                {
                    Debug.LogError($"  ❌ NULL/MISSING SCRIPT!");
                }
            }
        }
    }
    
    private void Update()
    {
        if (agent == null) return;
        
        timer += Time.deltaTime;
        
        if (timer >= checkInterval)
        {
            timer = 0f;
            
            Vector3 currentPos = transform.position;
            float distanceMoved = Vector3.Distance(currentPos, lastPosition);
            
            Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Debug.Log($"<color=cyan>MOVEMENT CHECK (1 second):</color>");
            Debug.Log($"  Position: {currentPos}");
            Debug.Log($"  Distance moved: {distanceMoved:F3} units");
            Debug.Log($"  Agent velocity: {agent.velocity} (magnitude: {agent.velocity.magnitude:F3})");
            Debug.Log($"  Agent desiredVelocity: {agent.desiredVelocity} (magnitude: {agent.desiredVelocity.magnitude:F3})");
            Debug.Log($"  Agent speed: {agent.speed}");
            Debug.Log($"  Agent enabled: {agent.enabled}");
            Debug.Log($"  Agent isOnNavMesh: {agent.isOnNavMesh}");
            Debug.Log($"  Agent isStopped: {agent.isStopped}");
            Debug.Log($"  Agent hasPath: {agent.hasPath}");
            
            if (agent.hasPath)
            {
                Debug.Log($"  Path status: {agent.pathStatus}");
                Debug.Log($"  Remaining distance: {agent.remainingDistance:F2}");
                Debug.Log($"  Destination: {agent.destination}");
                
                if (agent.path != null && agent.path.corners.Length > 0)
                {
                    Debug.Log($"  Path corners: {agent.path.corners.Length}");
                    Debug.Log($"  Next corner: {agent.path.corners[0]}");
                }
            }
            
            if (distanceMoved < 0.01f && agent.hasPath && !agent.isStopped)
            {
                Debug.LogError("❌❌❌ AGENT HAS PATH BUT NOT MOVING!");
                Debug.LogError("Checking for issues...");
                
                if (agent.speed <= 0)
                {
                    Debug.LogError($"  → Agent speed is {agent.speed}! Setting to 3.5");
                    agent.speed = 3.5f;
                }
                
                if (agent.remainingDistance < 0.1f)
                {
                    Debug.LogWarning("  → Agent is already at destination!");
                }
                
                ImpEnemy impScript = GetComponent<ImpEnemy>();
                if (impScript != null)
                {
                    Debug.Log($"  → ImpEnemy script found, enabled: {impScript.enabled}");
                    if (impScript.player == null)
                    {
                        Debug.LogError("  → ImpEnemy.player is NULL!");
                    }
                }
                
                Debug.LogError("  → Attempting to force movement...");
                agent.isStopped = false;
                agent.speed = 3.5f;
                
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                {
                    agent.SetDestination(playerObj.transform.position);
                    Debug.Log($"  → Set new destination to player: {playerObj.transform.position}");
                }
            }
            
            lastPosition = currentPos;
        }
    }
}
