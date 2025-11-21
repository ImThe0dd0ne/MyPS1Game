using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using UnityEditor.Animations;

public class IMP_PRODUCTION_FIX
{
    [MenuItem("Tools/🎯 IMP PRODUCTION FIX")]
    public static void ProductionFix()
    {
        Debug.Log("🎯 IMP PRODUCTION FIX\n");
        
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
        
        ImpDebugLogger debugLogger = prefabRoot.GetComponent<ImpDebugLogger>();
        if (debugLogger != null)
        {
            Object.DestroyImmediate(debugLogger, true);
            Debug.Log("✅ Removed ImpDebugLogger");
        }
        
        HitEffect hitEffect = prefabRoot.GetComponent<HitEffect>();
        if (hitEffect != null)
        {
            SerializedObject so = new SerializedObject(hitEffect);
            so.FindProperty("showDebugCubes").boolValue = false;
            so.ApplyModifiedProperties();
            Debug.Log("✅ Disabled HitEffect debug cubes");
        }
        
        ImpAI impAI = prefabRoot.GetComponent<ImpAI>();
        if (impAI != null)
        {
            impAI.sightRange = 1000f;
            impAI.attackRange = 15f;
            impAI.timeBetweenAttacks = 3f;
            impAI.fireballSpeed = 20f;
            
            Debug.Log($"✅ ImpAI ranges:");
            Debug.Log($"   sightRange = {impAI.sightRange} (MASSIVE for testing)");
            Debug.Log($"   attackRange = {impAI.attackRange}");
            Debug.Log($"   fireballSpeed = {impAI.fireballSpeed}");
            
            SerializedObject serializedImp = new SerializedObject(impAI);
            
            int playerLayerIndex = LayerMask.NameToLayer("WhatIsPlayer");
            int groundLayerIndex = LayerMask.NameToLayer("WhatIsGround");
            
            if (playerLayerIndex >= 0)
            {
                serializedImp.FindProperty("whatIsPlayer").intValue = 1 << playerLayerIndex;
            }
            
            if (groundLayerIndex >= 0)
            {
                serializedImp.FindProperty("whatIsGround").intValue = 1 << groundLayerIndex;
            }
            
            serializedImp.ApplyModifiedProperties();
        }
        
        Animator animator = prefabRoot.GetComponent<Animator>();
        if (animator != null)
        {
            string goblinControllerPath = "Assets/Character/Knight/Animations/GoblinAnimator.controller";
            AnimatorController goblinController = AssetDatabase.LoadAssetAtPath<AnimatorController>(goblinControllerPath);
            
            if (goblinController != null)
            {
                animator.runtimeAnimatorController = goblinController;
                Debug.Log("✅ Set Animator controller to GoblinAnimator.controller");
            }
            else
            {
                Debug.LogWarning("⚠️ GoblinAnimator.controller not found at: " + goblinControllerPath);
            }
        }
        
        NavMeshAgent agent = prefabRoot.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.updateRotation = false;
            Debug.Log($"✅ NavMeshAgent: updateRotation = false (ImpAI handles rotation)");
        }
        
        PrefabUtility.SaveAsPrefabAsset(prefabRoot, path);
        PrefabUtility.UnloadPrefabContents(prefabRoot);
        
        Debug.Log("\n🎯 IMP PRODUCTION FIX COMPLETE!");
        Debug.Log("✅ Removed debug scripts");
        Debug.Log("✅ Disabled yellow debug cubes");
        Debug.Log("✅ Set animator controller");
        Debug.Log("✅ Increased sight range to 1000 units (temporary)");
        Debug.Log("\n🎮 PRESS PLAY AND TEST!\n");
    }
}
