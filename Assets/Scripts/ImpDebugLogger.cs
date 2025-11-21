using UnityEngine;
using UnityEngine.AI;

public class ImpDebugLogger : MonoBehaviour
{
    private NavMeshAgent agent;
    private ImpAI impAI;
    private float lastLogTime;
    
    private void Start()
    {
        if (!gameObject.name.Contains("Imp")) return;
        
        agent = GetComponent<NavMeshAgent>();
        impAI = GetComponent<ImpAI>();
        
        Debug.Log($"🔍 IMP SPAWNED: {gameObject.name}");
        Debug.Log($"   Position: {transform.position}");
        Debug.Log($"   Scale: {transform.localScale}");
        Debug.Log($"   Active: {gameObject.activeSelf}");
        
        if (agent != null)
        {
            Debug.Log($"   ✅ NavMeshAgent:");
            Debug.Log($"      - isOnNavMesh: {agent.isOnNavMesh}");
            Debug.Log($"      - enabled: {agent.enabled}");
            Debug.Log($"      - radius: {agent.radius}");
            Debug.Log($"      - height: {agent.height}");
            Debug.Log($"      - baseOffset: {agent.baseOffset}");
        }
        else
        {
            Debug.LogError($"   ❌ NO NavMeshAgent!");
        }
        
        if (impAI != null)
        {
            Debug.Log($"   ✅ ImpAI script found");
        }
        else
        {
            Debug.LogError($"   ❌ NO ImpAI script!");
        }
        
        Component[] allComponents = GetComponents<Component>();
        Debug.Log($"   Components: {allComponents.Length}");
        foreach (Component comp in allComponents)
        {
            if (comp == null)
                Debug.Log($"      - MISSING SCRIPT (null)");
            else
                Debug.Log($"      - {comp.GetType().Name}");
        }
        
        lastLogTime = Time.time;
    }
    
    private void Update()
    {
        if (!gameObject.name.Contains("Imp")) return;
        if (Time.time - lastLogTime < 2f) return;
        
        lastLogTime = Time.time;
        
        if (agent != null && impAI != null)
        {
            Debug.Log($"🎯 IMP UPDATE ({gameObject.name}):");
            Debug.Log($"   Position: {transform.position}");
            Debug.Log($"   isOnNavMesh: {agent.isOnNavMesh}");
            Debug.Log($"   velocity: {agent.velocity.magnitude:F3}");
            Debug.Log($"   hasPath: {agent.hasPath}");
            Debug.Log($"   pathStatus: {agent.pathStatus}");
            Debug.Log($"   isStopped: {agent.isStopped}");
            Debug.Log($"   destination: {agent.destination}");
            Debug.Log($"   remainingDistance: {agent.remainingDistance:F3}");
            
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                float distToPlayer = Vector3.Distance(transform.position, playerObj.transform.position);
                Debug.Log($"   🎮 Player distance: {distToPlayer:F2}");
                Debug.Log($"   🎮 Player layer: {playerObj.layer} ({LayerMask.LayerToName(playerObj.layer)})");
                
                System.Reflection.FieldInfo whatIsPlayerField = typeof(ImpAI).GetField("whatIsPlayer", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                if (whatIsPlayerField != null)
                {
                    LayerMask whatIsPlayer = (LayerMask)whatIsPlayerField.GetValue(impAI);
                    Debug.Log($"   🔍 whatIsPlayer LayerMask: {whatIsPlayer.value}");
                    
                    bool canSeePlayer = ((1 << playerObj.layer) & whatIsPlayer.value) != 0;
                    Debug.Log($"   🔍 Can detect player layer: {canSeePlayer}");
                }
            }
        }
    }
}
