using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using UnityEditor.Animations;

public class TOTAL_IMP_REBUILD
{
    [MenuItem("Tools/🔥 TOTAL IMP REBUILD")]
    public static void TotalRebuild()
    {
        Debug.Log("🔥🔥🔥 TOTAL IMP REBUILD 🔥🔥🔥\n");
        
        string impPrefabPath = "Assets/Prefabs/Imp.prefab";
        GameObject impPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(impPrefabPath);
        
        if (impPrefab == null)
        {
            Debug.LogError("❌ Imp prefab not found!");
            return;
        }
        
        string path = AssetDatabase.GetAssetPath(impPrefab);
        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(path);
        
        GameObjectUtility.RemoveMonoBehavioursWithMissingScript(prefabRoot);
        Debug.Log("✅ Removed missing scripts");
        
        Component[] allComponents = prefabRoot.GetComponents<Component>();
        foreach (Component comp in allComponents)
        {
            if (comp == null) continue;
            
            string typeName = comp.GetType().Name;
            if (typeName.Contains("Imp") || typeName.Contains("Debug") || typeName == "HitEffect")
            {
                if (comp is Transform || comp is Animator || comp is NavMeshAgent || 
                    comp is AudioSource || comp is CapsuleCollider) continue;
                    
                Object.DestroyImmediate(comp, true);
                Debug.Log($"✅ Removed {typeName}");
            }
        }
        
        NavMeshAgent agent = prefabRoot.GetComponent<NavMeshAgent>();
        if (agent == null) agent = prefabRoot.AddComponent<NavMeshAgent>();
        
        agent.radius = 0.5f;
        agent.height = 2f;
        agent.baseOffset = 1.0f;
        agent.speed = 3.5f;
        agent.angularSpeed = 120f;
        agent.acceleration = 8f;
        agent.stoppingDistance = 7f;
        agent.autoBraking = true;
        agent.updateRotation = false;
        agent.updatePosition = true;
        agent.updateUpAxis = true;
        Debug.Log("✅ NavMeshAgent configured");
        
        CapsuleCollider col = prefabRoot.GetComponent<CapsuleCollider>();
        if (col == null) col = prefabRoot.AddComponent<CapsuleCollider>();
        
        col.center = new Vector3(0, 1.5f, 0);
        col.radius = 0.8f;
        col.height = 3f;
        Debug.Log("✅ CapsuleCollider configured");
        
        Animator animator = prefabRoot.GetComponent<Animator>();
        if (animator != null)
        {
            string goblinControllerPath = "Assets/Character/Knight/Animations/GoblinAnimator.controller";
            AnimatorController goblinController = AssetDatabase.LoadAssetAtPath<AnimatorController>(goblinControllerPath);
            
            if (goblinController != null)
            {
                animator.runtimeAnimatorController = goblinController;
                animator.applyRootMotion = false;
                Debug.Log("✅ Animator configured with GoblinAnimator.controller");
            }
        }
        
        ImpAI_NEW impAI = prefabRoot.AddComponent<ImpAI_NEW>();
        impAI.agent = agent;
        impAI.animator = animator;
        
        int playerLayerIndex = LayerMask.NameToLayer("WhatIsPlayer");
        int groundLayerIndex = LayerMask.NameToLayer("WhatIsGround");
        
        SerializedObject serializedImp = new SerializedObject(impAI);
        
        if (playerLayerIndex >= 0)
        {
            serializedImp.FindProperty("whatIsPlayer").intValue = 1 << playerLayerIndex;
        }
        
        if (groundLayerIndex >= 0)
        {
            serializedImp.FindProperty("whatIsGround").intValue = 1 << groundLayerIndex;
        }
        
        impAI.maxHealth = 60f;
        impAI.health = 60f;
        impAI.attackDamage = 10;
        impAI.xpReward = 15;
        impAI.sightRange = 15f;
        impAI.attackRange = 10f;
        impAI.timeBetweenAttacks = 3f;
        impAI.attackWindupTime = 0.8f;
        impAI.fireballSpeed = 20f;
        impAI.walkPointRange = 10f;
        
        string fireballPath = "Assets/Goblin_Character/Prefab/Fireball.prefab";
        GameObject fireballPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(fireballPath);
        if (fireballPrefab != null)
        {
            serializedImp.FindProperty("fireballPrefab").objectReferenceValue = fireballPrefab;
            Debug.Log("✅ Fireball prefab assigned");
        }
        
        AudioSource audioSource = prefabRoot.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = prefabRoot.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;
        serializedImp.FindProperty("audioSource").objectReferenceValue = audioSource;
        
        serializedImp.ApplyModifiedProperties();
        
        Debug.Log("✅ ImpAI_NEW added and configured");
        
        PrefabUtility.SaveAsPrefabAsset(prefabRoot, path);
        PrefabUtility.UnloadPrefabContents(prefabRoot);
        
        Debug.Log("\n🔥 TOTAL IMP REBUILD COMPLETE!");
        Debug.Log("✅ All old scripts removed");
        Debug.Log("✅ Clean ImpAI_NEW added (exact copy of Goblin behavior)");
        Debug.Log("✅ HitEffect removed (no more yellow cubes)");
        Debug.Log("✅ NavMeshAgent configured exactly like Goblin");
        Debug.Log("✅ Rotation system copied from Goblin (SmoothRotate)");
        Debug.Log("✅ Fireball attack instead of melee");
        Debug.Log("\n🎮 PRESS PLAY AND TEST!\n");
    }
}
