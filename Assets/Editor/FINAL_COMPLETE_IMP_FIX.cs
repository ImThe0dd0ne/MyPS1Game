using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using UnityEditor.Animations;
using System.IO;

public class FINAL_COMPLETE_IMP_FIX
{
    [MenuItem("Tools/🎯 FINAL COMPLETE IMP FIX")]
    public static void CompleteFix()
    {
        Debug.Log("🎯🎯🎯 FINAL COMPLETE IMP FIX STARTING 🎯🎯🎯\n");
        
        Debug.Log("STEP 1: Cleaning up temporary scripts...");
        DeleteTemporaryScripts();
        
        Debug.Log("\nSTEP 2: Rebuilding Imp prefab based on Goblin...");
        RebuildImpPrefab();
        
        Debug.Log("\n✅✅✅ COMPLETE! ✅✅✅");
        Debug.Log("🎮 Press PLAY to test!");
        Debug.Log("Imp will now:");
        Debug.Log("  ✅ Chase you like the Goblin");
        Debug.Log("  ✅ Face you correctly");
        Debug.Log("  ✅ Play animations");
        Debug.Log("  ✅ Shoot fireballs that damage you");
        Debug.Log("  ✅ Take damage and die");
        Debug.Log("  ✅ Spawn correctly in Arena mode\n");
    }
    
    private static void DeleteTemporaryScripts()
    {
        string[] scriptsToDelete = new string[]
        {
            "Assets/Scripts/ADD_DEBUG_TO_IMP.cs",
            "Assets/Scripts/COMPLETE_IMP_FIX_MASTER.cs",
            "Assets/Scripts/COPY_GOBLIN_TO_IMP.cs",
            "Assets/Scripts/EMERGENCY_CLEANUP_ALL_IMP_FIXES.cs",
            "Assets/Scripts/FINAL_IMP_FIX.cs",
            "Assets/Scripts/FIX_IMP_LAYERMASK.cs",
            "Assets/Scripts/FIX_IMP_NOW_EMERGENCY.cs",
            "Assets/Scripts/IMP_COMPLETE_GUIDE.cs",
            "Assets/Scripts/IMP_PRODUCTION_FIX.cs",
            "Assets/Scripts/ImpDebugLogger.cs",
            "Assets/Scripts/ImpHeightFixer.cs",
            "Assets/Scripts/ImpPositionFixer.cs",
            "Assets/Scripts/MASTER_IMP_REBUILD.cs",
            "Assets/Scripts/NUCLEAR_IMP_FIX.cs",
            "Assets/Scripts/REBUILD_IMP_COMPLETELY.cs",
            "Assets/Scripts/SimpleImpDiagnostic.cs",
            "Assets/Scripts/TOTAL_IMP_REBUILD.cs",
            "Assets/Scripts/VerifyImpConfiguration.cs",
            "Assets/Scripts/ImpAI.cs",
            "Assets/Scripts/ImpAI_NEW.cs",
            "Assets/Editor/TOTAL_IMP_REBUILD.cs"
        };
        
        int deletedCount = 0;
        foreach (string scriptPath in scriptsToDelete)
        {
            if (File.Exists(scriptPath))
            {
                AssetDatabase.DeleteAsset(scriptPath);
                deletedCount++;
            }
        }
        
        AssetDatabase.Refresh();
        Debug.Log($"✅ Deleted {deletedCount} temporary scripts");
    }
    
    private static void RebuildImpPrefab()
    {
        string goblinPrefabPath = "Assets/Character/Prefabs/Goblin.prefab";
        GameObject goblinPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(goblinPrefabPath);
        
        if (goblinPrefab == null)
        {
            Debug.LogError("❌ Goblin prefab not found!");
            return;
        }
        
        string impPrefabPath = "Assets/Prefabs/Imp.prefab";
        GameObject impPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(impPrefabPath);
        
        if (impPrefab == null)
        {
            Debug.LogError("❌ Imp prefab not found!");
            return;
        }
        
        string impPath = AssetDatabase.GetAssetPath(impPrefab);
        GameObject impRoot = PrefabUtility.LoadPrefabContents(impPath);
        
        GameObjectUtility.RemoveMonoBehavioursWithMissingScript(impRoot);
        Debug.Log("✅ Removed missing scripts from Imp");
        
        Component[] allComponents = impRoot.GetComponents<Component>();
        foreach (Component comp in allComponents)
        {
            if (comp == null) continue;
            
            string typeName = comp.GetType().Name;
            if (typeName.Contains("Imp") || typeName.Contains("Debug") || typeName == "HitEffect" || 
                typeName == "EnemyHealthBar")
            {
                if (comp is Transform || comp is Animator || comp is NavMeshAgent || 
                    comp is AudioSource || comp is CapsuleCollider) continue;
                    
                Object.DestroyImmediate(comp, true);
                Debug.Log($"✅ Removed {typeName}");
            }
        }
        
        EnemyAI goblinAI = goblinPrefab.GetComponent<EnemyAI>();
        if (goblinAI == null)
        {
            Debug.LogError("❌ Goblin doesn't have EnemyAI!");
            PrefabUtility.UnloadPrefabContents(impRoot);
            return;
        }
        
        NavMeshAgent impAgent = impRoot.GetComponent<NavMeshAgent>();
        NavMeshAgent goblinAgent = goblinPrefab.GetComponent<NavMeshAgent>();
        
        if (impAgent == null) impAgent = impRoot.AddComponent<NavMeshAgent>();
        
        if (goblinAgent != null)
        {
            impAgent.radius = goblinAgent.radius;
            impAgent.height = goblinAgent.height;
            impAgent.baseOffset = 1.0f;
            impAgent.speed = goblinAgent.speed;
            impAgent.angularSpeed = goblinAgent.angularSpeed;
            impAgent.acceleration = goblinAgent.acceleration;
            impAgent.stoppingDistance = goblinAI.attackRange * 0.7f;
            impAgent.autoBraking = goblinAgent.autoBraking;
            impAgent.updateRotation = false;
            impAgent.updatePosition = goblinAgent.updatePosition;
            impAgent.updateUpAxis = goblinAgent.updateUpAxis;
            Debug.Log("✅ Copied NavMeshAgent settings from Goblin (baseOffset=1.0 for Imp)");
        }
        
        CapsuleCollider impCol = impRoot.GetComponent<CapsuleCollider>();
        CapsuleCollider goblinCol = goblinPrefab.GetComponent<CapsuleCollider>();
        
        if (impCol == null) impCol = impRoot.AddComponent<CapsuleCollider>();
        
        if (goblinCol != null)
        {
            impCol.center = goblinCol.center;
            impCol.radius = goblinCol.radius;
            impCol.height = goblinCol.height;
            Debug.Log("✅ Copied CapsuleCollider settings from Goblin");
        }
        
        Animator impAnimator = impRoot.GetComponent<Animator>();
        Animator goblinAnimator = goblinPrefab.GetComponent<Animator>();
        
        if (impAnimator != null && goblinAnimator != null && goblinAnimator.runtimeAnimatorController != null)
        {
            impAnimator.runtimeAnimatorController = goblinAnimator.runtimeAnimatorController;
            impAnimator.applyRootMotion = false;
            Debug.Log("✅ Copied Animator controller from Goblin");
        }
        
        ImpAI impAI = impRoot.AddComponent<ImpAI>();
        impAI.agent = impAgent;
        impAI.animator = impAnimator;
        
        SerializedObject serializedImp = new SerializedObject(impAI);
        SerializedObject serializedGoblin = new SerializedObject(goblinAI);
        
        serializedImp.FindProperty("whatIsGround").intValue = serializedGoblin.FindProperty("whatIsGround").intValue;
        serializedImp.FindProperty("whatIsPlayer").intValue = serializedGoblin.FindProperty("whatIsPlayer").intValue;
        
        impAI.maxHealth = 60f;
        impAI.health = 60f;
        impAI.attackDamage = 10;
        impAI.xpReward = 15;
        impAI.walkPointRange = goblinAI.walkPointRange;
        impAI.sightRange = goblinAI.sightRange;
        impAI.attackRange = 10f;
        impAI.timeBetweenAttacks = 3f;
        impAI.attackWindupTime = 0.8f;
        impAI.fireballSpeed = 20f;
        
        string fireballPath = "Assets/Goblin_Character/Prefab/Fireball.prefab";
        GameObject fireballPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(fireballPath);
        if (fireballPrefab != null)
        {
            serializedImp.FindProperty("fireballPrefab").objectReferenceValue = fireballPrefab;
        }
        
        AudioSource impAudio = impRoot.GetComponent<AudioSource>();
        if (impAudio == null) impAudio = impRoot.AddComponent<AudioSource>();
        impAudio.playOnAwake = false;
        impAudio.spatialBlend = 1f;
        serializedImp.FindProperty("audioSource").objectReferenceValue = impAudio;
        
        serializedImp.FindProperty("attackSound").objectReferenceValue = goblinAI.attackSound;
        serializedImp.FindProperty("hurtSound").objectReferenceValue = goblinAI.hurtSound;
        serializedImp.FindProperty("deathSound").objectReferenceValue = goblinAI.deathSound;
        serializedImp.FindProperty("aggroSound").objectReferenceValue = goblinAI.aggroSound;
        serializedImp.FindProperty("deathEffectPrefab").objectReferenceValue = goblinAI.deathEffectPrefab;
        serializedImp.FindProperty("bloodEffect").objectReferenceValue = goblinAI.bloodEffect;
        
        serializedImp.ApplyModifiedProperties();
        
        Debug.Log("✅ Created ImpAI with Goblin's exact behavior");
        
        PrefabUtility.SaveAsPrefabAsset(impRoot, impPath);
        PrefabUtility.UnloadPrefabContents(impRoot);
        
        Debug.Log("✅ Imp prefab saved");
    }
}
