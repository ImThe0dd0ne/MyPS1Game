using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using UnityEditor.Animations;

public class FIX_IMP_NOW_EMERGENCY
{
    [MenuItem("Tools/⚡ EMERGENCY FIX IMP NOW")]
    public static void FixImpNow()
    {
        Debug.Log("⚡⚡⚡ EMERGENCY IMP FIX STARTING ⚡⚡⚡\n");
        
        string impPrefabPath = "Assets/Prefabs/Imp.prefab";
        GameObject impPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(impPrefabPath);
        
        if (impPrefab == null)
        {
            Debug.LogError("❌ Imp prefab not found at Assets/Prefabs/Imp.prefab!");
            return;
        }
        
        string path = AssetDatabase.GetAssetPath(impPrefab);
        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(path);
        
        Debug.Log("📋 BEFORE FIX - Components on Imp:");
        Component[] allBefore = prefabRoot.GetComponents<Component>();
        foreach (Component comp in allBefore)
        {
            if (comp == null)
            {
                Debug.Log("  ⚠️ MISSING SCRIPT (null component)");
            }
            else
            {
                Debug.Log($"  - {comp.GetType().Name}");
            }
        }
        
        GameObjectUtility.RemoveMonoBehavioursWithMissingScript(prefabRoot);
        Debug.Log("\n✅ Removed missing scripts");
        
        prefabRoot.transform.localScale = Vector3.one;
        prefabRoot.transform.localPosition = Vector3.zero;
        Debug.Log("✅ Scale = 1, Position = 0");
        
        NavMeshAgent agent = prefabRoot.GetComponent<NavMeshAgent>();
        if (agent == null) agent = prefabRoot.AddComponent<NavMeshAgent>();
        agent.radius = 0.5f;
        agent.height = 2f;
        agent.baseOffset = 1f;
        agent.speed = 3.5f;
        agent.stoppingDistance = 10f;
        agent.updatePosition = true;
        agent.updateRotation = false;
        Debug.Log("✅ NavMeshAgent: radius=0.5, height=2, baseOffset=1 (LIFTS OFF GROUND)");
        
        CapsuleCollider col = prefabRoot.GetComponent<CapsuleCollider>();
        if (col == null) col = prefabRoot.AddComponent<CapsuleCollider>();
        col.center = new Vector3(0, 1f, 0);
        col.radius = 0.5f;
        col.height = 2f;
        col.isTrigger = false;
        Debug.Log("✅ CapsuleCollider: center=(0,1,0), radius=0.5, height=2");
        
        Rigidbody rb = prefabRoot.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Object.DestroyImmediate(rb, true);
            Debug.Log("✅ Removed Rigidbody (blocks NavMesh)");
        }
        
        Animator anim = prefabRoot.GetComponent<Animator>();
        if (anim != null)
        {
            AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>("Assets/Imp/Animations/ImpAnimator.controller");
            if (controller != null)
            {
                anim.runtimeAnimatorController = controller;
                anim.applyRootMotion = false;
                
                bool hasSpeed = false;
                bool hasAttack = false;
                bool hasDie = false;
                
                foreach (var param in controller.parameters)
                {
                    if (param.name == "Speed") hasSpeed = true;
                    if (param.name == "Attack") hasAttack = true;
                    if (param.name == "Die") hasDie = true;
                }
                
                if (!hasSpeed) controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
                if (!hasAttack) controller.AddParameter("Attack", AnimatorControllerParameterType.Trigger);
                if (!hasDie) controller.AddParameter("Die", AnimatorControllerParameterType.Trigger);
                
                EditorUtility.SetDirty(controller);
                AssetDatabase.SaveAssets();
                Debug.Log("✅ Animator: Speed, Attack, Die parameters");
            }
        }
        
        ImpAI impAI = prefabRoot.GetComponent<ImpAI>();
        if (impAI == null)
        {
            impAI = prefabRoot.AddComponent<ImpAI>();
            Debug.Log("✅ Added ImpAI component");
        }
        
        impAI.maxHealth = 60f;
        impAI.health = 60f;
        impAI.attackDamage = 10;
        impAI.xpReward = 15;
        impAI.sightRange = 20f;
        impAI.attackRange = 12f;
        impAI.timeBetweenAttacks = 3f;
        impAI.fireballSpeed = 15f;
        
        GameObject fireballPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Goblin_Character/Prefab/Fireball.prefab");
        if (fireballPrefab != null)
        {
            impAI.fireballPrefab = fireballPrefab;
            Debug.Log("✅ Fireball prefab assigned");
        }
        
        SerializedObject serializedImp = new SerializedObject(impAI);
        serializedImp.FindProperty("whatIsGround").intValue = 1 << LayerMask.NameToLayer("WhatIsGround");
        serializedImp.FindProperty("whatIsPlayer").intValue = 1 << LayerMask.NameToLayer("Player");
        serializedImp.ApplyModifiedProperties();
        
        prefabRoot.tag = "Enemy";
        prefabRoot.layer = LayerMask.NameToLayer("Enemy");
        
        Debug.Log("\n📋 AFTER FIX - Components on Imp:");
        Component[] allAfter = prefabRoot.GetComponents<Component>();
        foreach (Component comp in allAfter)
        {
            Debug.Log($"  ✅ {comp.GetType().Name}");
        }
        
        PrefabUtility.SaveAsPrefabAsset(prefabRoot, path);
        PrefabUtility.UnloadPrefabContents(prefabRoot);
        
        Debug.Log("\n⚡⚡⚡ IMP PREFAB FIXED! ⚡⚡⚡");
        Debug.Log("🎯 baseOffset = 1 means Imp floats 1 unit above NavMesh");
        Debug.Log("🎯 This prevents ground clipping!");
        Debug.Log("\n👉 NOW CHECK ARENA MANAGER:");
        Debug.Log("   Select ArenaManager in hierarchy");
        Debug.Log("   Make sure Imp prefab is in Enemy Prefabs array\n");
    }
}
