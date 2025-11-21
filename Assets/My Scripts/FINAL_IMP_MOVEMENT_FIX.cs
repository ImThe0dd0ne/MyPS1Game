using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

public class FINAL_IMP_MOVEMENT_FIX : MonoBehaviour
{
    [MenuItem("Tools/🚀 FINAL IMP MOVEMENT FIX")]
    public static void ApplyFinalFix()
    {
        Debug.Log("╔═══════════════════════════════════════════════════════════╗");
        Debug.Log("║          🚀 FINAL IMP MOVEMENT FIX 🚀                    ║");
        Debug.Log("╚═══════════════════════════════════════════════════════════╝\n");
        
        string prefabPath = "Assets/Prefabs/Imp.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        
        if (prefab == null)
        {
            Debug.LogError("❌ Imp prefab not found!");
            return;
        }
        
        string path = AssetDatabase.GetAssetPath(prefab);
        GameObject prefabContents = PrefabUtility.LoadPrefabContents(path);
        
        Debug.Log("[1] Removing missing scripts...");
        int totalRemoved = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(prefabContents);
        foreach (Transform child in prefabContents.GetComponentsInChildren<Transform>(true))
        {
            totalRemoved += GameObjectUtility.RemoveMonoBehavioursWithMissingScript(child.gameObject);
        }
        Debug.Log($"✅ Removed {totalRemoved} missing script(s)");
        
        Debug.Log("\n[2] Checking NavMeshAgent configuration...");
        NavMeshAgent agent = prefabContents.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = 3.5f;
            agent.acceleration = 8f;
            agent.angularSpeed = 120f;
            agent.radius = 0.35f;
            agent.height = 1.2f;
            agent.baseOffset = 0f;
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            agent.avoidancePriority = 50;
            agent.autoBraking = true;
            agent.autoRepath = true;
            agent.areaMask = -1;
            
            agent.updatePosition = true;
            agent.updateRotation = false;
            
            Debug.Log("✅ NavMeshAgent fully configured:");
            Debug.Log($"   speed: {agent.speed}");
            Debug.Log($"   acceleration: {agent.acceleration}");
            Debug.Log($"   updatePosition: {agent.updatePosition} ← CRITICAL!");
            Debug.Log($"   updateRotation: {agent.updateRotation}");
            Debug.Log($"   obstacleAvoidanceType: {agent.obstacleAvoidanceType}");
        }
        else
        {
            Debug.LogError("❌ NavMeshAgent not found!");
        }
        
        Debug.Log("\n[3] Checking Rigidbody...");
        Rigidbody rb = prefabContents.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Debug.LogWarning($"⚠️ Rigidbody found! This can interfere with NavMeshAgent!");
            Debug.LogWarning($"   isKinematic: {rb.isKinematic}");
            
            if (!rb.isKinematic)
            {
                Debug.LogWarning("   Setting to kinematic...");
                rb.isKinematic = true;
            }
        }
        else
        {
            Debug.Log("✅ No Rigidbody (good for NavMeshAgent)");
        }
        
        Debug.Log("\n[4] Adding movement debug component...");
        if (prefabContents.GetComponent<ImpMovementDebug>() == null)
        {
            prefabContents.AddComponent<ImpMovementDebug>();
            Debug.Log("✅ Added ImpMovementDebug");
        }
        else
        {
            Debug.Log("✅ ImpMovementDebug already present");
        }
        
        Debug.Log("\n[5] Checking colliders...");
        Collider[] colliders = prefabContents.GetComponents<Collider>();
        Debug.Log($"Found {colliders.Length} collider(s) on root:");
        foreach (Collider col in colliders)
        {
            Debug.Log($"   - {col.GetType().Name}: isTrigger={col.isTrigger}");
        }
        
        CapsuleCollider capsule = prefabContents.GetComponent<CapsuleCollider>();
        if (capsule != null)
        {
            capsule.isTrigger = false;
            Debug.Log("✅ CapsuleCollider is NOT trigger");
        }
        
        PrefabUtility.SaveAsPrefabAsset(prefabContents, path);
        PrefabUtility.UnloadPrefabContents(prefabContents);
        
        Debug.Log("\n╔═══════════════════════════════════════════════════════════╗");
        Debug.Log("║                  ✅ FIX COMPLETE!                        ║");
        Debug.Log("╚═══════════════════════════════════════════════════════════╝");
        Debug.Log("\n🔥 TEST NOW:");
        Debug.Log("1. Press Play");
        Debug.Log("2. Press B to start arena");
        Debug.Log("3. Watch console for detailed movement analysis");
        Debug.Log("4. Look for 'desiredVelocity' and 'velocity' values\n");
    }
}
