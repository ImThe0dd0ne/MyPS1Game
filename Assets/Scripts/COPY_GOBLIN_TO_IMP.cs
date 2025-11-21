using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using UnityEditor.Animations;

public class COPY_GOBLIN_TO_IMP
{
    [MenuItem("Tools/🔥 COPY GOBLIN VALUES TO IMP")]
    public static void CopyGoblinToImp()
    {
        Debug.Log("🔥🔥🔥 COPYING WORKING GOBLIN CONFIG TO IMP 🔥🔥🔥\n");
        
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
        Debug.Log("✅ Removed missing scripts\n");
        
        prefabRoot.transform.localScale = Vector3.one * 0.3f;
        prefabRoot.transform.localPosition = Vector3.zero;
        prefabRoot.tag = "Enemy";
        prefabRoot.layer = LayerMask.NameToLayer("Enemy");
        Debug.Log("✅ Scale = 0.3 (Goblin size)");
        Debug.Log("✅ Tag = Enemy, Layer = Enemy\n");
        
        Rigidbody rb = prefabRoot.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Object.DestroyImmediate(rb, true);
            Debug.Log("✅ Removed Rigidbody\n");
        }
        
        NavMeshAgent agent = prefabRoot.GetComponent<NavMeshAgent>();
        if (agent == null) agent = prefabRoot.AddComponent<NavMeshAgent>();
        
        agent.radius = 0.5f;
        agent.height = 2f;
        agent.baseOffset = 0f;
        agent.speed = 3.5f;
        agent.acceleration = 8f;
        agent.angularSpeed = 120f;
        agent.stoppingDistance = 10f;
        agent.autoBraking = true;
        agent.autoRepath = true;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        agent.updatePosition = true;
        agent.updateRotation = false;
        
        Debug.Log("✅ NavMeshAgent (EXACT GOBLIN VALUES):");
        Debug.Log($"   radius = {agent.radius}");
        Debug.Log($"   height = {agent.height}");
        Debug.Log($"   baseOffset = {agent.baseOffset}");
        Debug.Log($"   speed = {agent.speed}");
        Debug.Log($"   stoppingDistance = {agent.stoppingDistance}");
        Debug.Log($"   updatePosition = {agent.updatePosition}");
        Debug.Log($"   updateRotation = {agent.updateRotation}\n");
        
        CapsuleCollider col = prefabRoot.GetComponent<CapsuleCollider>();
        if (col == null) col = prefabRoot.AddComponent<CapsuleCollider>();
        
        col.center = new Vector3(0, 1.5f, 0);
        col.radius = 0.8f;
        col.height = 3f;
        col.isTrigger = false;
        
        Debug.Log("✅ CapsuleCollider (EXACT GOBLIN VALUES):");
        Debug.Log($"   center = {col.center}");
        Debug.Log($"   radius = {col.radius}");
        Debug.Log($"   height = {col.height}\n");
        
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
                Debug.Log("✅ Animator Controller: Speed, Attack, Die\n");
            }
        }
        
        ImpAI impAI = prefabRoot.GetComponent<ImpAI>();
        if (impAI == null) impAI = prefabRoot.AddComponent<ImpAI>();
        
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
        }
        
        SerializedObject serializedImp = new SerializedObject(impAI);
        serializedImp.FindProperty("whatIsGround").intValue = 1 << LayerMask.NameToLayer("WhatIsGround");
        serializedImp.FindProperty("whatIsPlayer").intValue = 1 << LayerMask.NameToLayer("Player");
        serializedImp.ApplyModifiedProperties();
        
        Debug.Log("✅ ImpAI component configured:");
        Debug.Log($"   Health = {impAI.maxHealth}");
        Debug.Log($"   Damage = {impAI.attackDamage}");
        Debug.Log($"   XP Reward = {impAI.xpReward}");
        Debug.Log($"   Attack Range = {impAI.attackRange}");
        Debug.Log($"   Fireball Speed = {impAI.fireballSpeed}\n");
        
        PrefabUtility.SaveAsPrefabAsset(prefabRoot, path);
        PrefabUtility.UnloadPrefabContents(prefabRoot);
        
        Debug.Log("🔥🔥🔥 IMP NOW USES EXACT GOBLIN VALUES! 🔥🔥🔥");
        Debug.Log("⚡ NavMesh Agent + Collider match Goblin perfectly");
        Debug.Log("⚡ Scale = 0.3 (Goblin size)\n");
        Debug.Log("🎮 PRESS PLAY AND TEST!\n");
    }
}
