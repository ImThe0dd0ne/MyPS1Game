using UnityEngine;
using UnityEngine.AI;

public class SimpleImpDiagnostic : MonoBehaviour
{
    private NavMeshAgent agent;
    private float checkTime = 0f;
    
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        Debug.Log($"═══ IMP SPAWN DIAGNOSTIC: {gameObject.name} ═══");
        Debug.Log($"Position: {transform.position}");
        Debug.Log($"Scale: {transform.localScale}");
        
        Debug.Log($"\n🔍 CHECKING COMPONENTS:");
        
        Component[] allComponents = GetComponents<Component>();
        foreach (Component comp in allComponents)
        {
            Debug.Log($"  - {comp.GetType().Name}");
        }
        
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            Debug.LogError($"⚠️⚠️⚠️ RIGIDBODY FOUND! isKinematic: {rb.isKinematic}");
            if (!rb.isKinematic)
            {
                Debug.LogError("❌❌❌ RIGIDBODY IS NOT KINEMATIC - THIS BREAKS NAVMESH!");
                Debug.Log("🔧 Setting to kinematic NOW...");
                rb.isKinematic = true;
            }
        }
        
        if (agent != null)
        {
            Debug.Log($"\n🔍 NAVMESH AGENT:");
            Debug.Log($"  - enabled: {agent.enabled}");
            Debug.Log($"  - isOnNavMesh: {agent.isOnNavMesh}");
            Debug.Log($"  - updatePosition: {agent.updatePosition}");
            Debug.Log($"  - speed: {agent.speed}");
            Debug.Log($"  - baseOffset: {agent.baseOffset}");
        }
        
        Debug.Log($"════════════════════════════════════════\n");
    }
    
    private void Update()
    {
        if (agent == null) return;
        
        checkTime += Time.deltaTime;
        if (checkTime >= 2f)
        {
            checkTime = 0f;
            
            if (agent.hasPath && agent.velocity.magnitude < 0.01f && !agent.isStopped)
            {
                Debug.LogWarning($"⚠️ {gameObject.name} has path but NOT MOVING!");
                Debug.Log($"  velocity: {agent.velocity}");
                Debug.Log($"  desiredVelocity: {agent.desiredVelocity}");
                Debug.Log($"  remainingDistance: {agent.remainingDistance}");
                
                Rigidbody rb = GetComponent<Rigidbody>();
                if (rb != null && !rb.isKinematic)
                {
                    Debug.LogError("❌ NON-KINEMATIC RIGIDBODY IS BLOCKING MOVEMENT!");
                }
            }
        }
    }
}
