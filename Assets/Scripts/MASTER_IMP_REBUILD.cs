using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using UnityEditor.Animations;
using System.IO;

public class MASTER_IMP_REBUILD : MonoBehaviour
{
    [MenuItem("Tools/🎯 MASTER IMP REBUILD - COMPLETE SOLUTION")]
    public static void RebuildImpFromScratch()
    {
        Debug.Log("╔═══════════════════════════════════════════════════════════╗");
        Debug.Log("║         🎯 MASTER IMP REBUILD - FINAL SOLUTION           ║");
        Debug.Log("╚═══════════════════════════════════════════════════════════╝\n");
        
        bool deletedScripts = DeleteAllImpFixScripts();
        
        if (deletedScripts)
        {
            Debug.Log("\n⚠️  Scripts deleted. Unity needs to recompile.");
            Debug.Log("⚠️  Please run this command AGAIN after compilation finishes!\n");
            return;
        }
        
        FixFireballMaterial();
        RebuildImpPrefab();
        
        Debug.Log("\n╔═══════════════════════════════════════════════════════════╗");
        Debug.Log("║              ✅ IMP COMPLETELY REBUILT!                  ║");
        Debug.Log("╚═══════════════════════════════════════════════════════════╝");
        Debug.Log("\n🎮 PRESS PLAY AND TEST WITH B KEY!");
        Debug.Log("The Imp should now:");
        Debug.Log("  ✅ Spawn correctly (not in ground)");
        Debug.Log("  ✅ Move toward player");
        Debug.Log("  ✅ Shoot orange fireballs");
        Debug.Log("  ✅ Take damage and die");
        Debug.Log("  ✅ Give 15 XP when killed\n");
    }
    
    private static bool DeleteAllImpFixScripts()
    {
        Debug.Log("[STEP 1/3] Deleting all Imp fix/debug scripts...\n");
        
        string[] scriptsToDelete = new string[]
        {
            "/Assets/My Scripts/COMPLETE_IMP_FIX.cs",
            "/Assets/My Scripts/CleanImpPrefab.cs",
            "/Assets/My Scripts/CreateFireballMaterial.cs",
            "/Assets/My Scripts/DiagnoseImpIssues.cs",
            "/Assets/My Scripts/FINAL_IMP_MOVEMENT_FIX.cs",
            "/Assets/My Scripts/FIREBALL_SETUP_COMPLETE_GUIDE.cs",
            "/Assets/My Scripts/FIX_FIREBALL_1_CLICK.cs",
            "/Assets/My Scripts/FinalImpFix.cs",
            "/Assets/My Scripts/FixImpCompletely.cs",
            "/Assets/My Scripts/FixImpMaterialShader.cs",
            "/Assets/My Scripts/IMP_ANIMATOR_SETUP_GUIDE.cs",
            "/Assets/My Scripts/IMP_EMERGENCY_FIX_NOW.cs",
            "/Assets/My Scripts/IMP_FINAL_SOLUTION.cs",
            "/Assets/My Scripts/IMP_NOT_MOVING_FIX.cs",
            "/Assets/My Scripts/IMP_QUICK_FIX_GUIDE.cs",
            "/Assets/My Scripts/ImpEnemy.cs",
            "/Assets/My Scripts/ImpMovementDebug.cs",
            "/Assets/My Scripts/ImpRuntimeDebug.cs",
            "/Assets/My Scripts/ImpSpawnFix.cs",
            "/Assets/My Scripts/READ_ME_IMP_FIX.cs",
            "/Assets/My Scripts/SetupImpAnimator.cs",
            "/Assets/My Scripts/ULTIMATE_IMP_FIX.cs",
            "/Assets/My Scripts/VerifyImpSetup.cs",
            "/Assets/Scripts/COMPLETE_IMP_FIX_MASTER.cs",
            "/Assets/Scripts/EMERGENCY_CLEANUP_ALL_IMP_FIXES.cs",
            "/Assets/Scripts/ImpHeightFixer.cs",
            "/Assets/Scripts/ImpPositionFixer.cs",
            "/Assets/Scripts/NUCLEAR_IMP_FIX.cs",
            "/Assets/Scripts/REBUILD_IMP_COMPLETELY.cs",
            "/Assets/Scripts/SimpleImpDiagnostic.cs",
            "/Assets/Scripts/VerifyImpConfiguration.cs"
        };
        
        int deletedCount = 0;
        foreach (string scriptPath in scriptsToDelete)
        {
            if (File.Exists(scriptPath))
            {
                File.Delete(scriptPath);
                File.Delete(scriptPath + ".meta");
                deletedCount++;
                Debug.Log($"  ❌ Deleted: {Path.GetFileName(scriptPath)}");
            }
        }
        
        if (deletedCount > 0)
        {
            AssetDatabase.Refresh();
            Debug.Log($"\n✅ Deleted {deletedCount} old fix scripts");
            return true;
        }
        
        Debug.Log($"\n✅ No old scripts to delete (already cleaned)\n");
        return false;
    }
    
    private static void FixFireballMaterial()
    {
        Debug.Log("[STEP 2/3] Fixing Fireball material...\n");
        
        string fireballPrefabPath = "Assets/Goblin_Character/Prefab/Fireball.prefab";
        GameObject fireballPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(fireballPrefabPath);
        
        if (fireballPrefab == null)
        {
            Debug.LogError("❌ Fireball prefab not found!");
            return;
        }
        
        string path = AssetDatabase.GetAssetPath(fireballPrefab);
        GameObject fireballRoot = PrefabUtility.LoadPrefabContents(path);
        
        Transform sphere = fireballRoot.transform.Find("Sphere");
        if (sphere != null)
        {
            MeshRenderer renderer = sphere.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                Material fireballMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/FireballMaterial.mat");
                if (fireballMat == null)
                {
                    fireballMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                    fireballMat.SetColor("_BaseColor", new Color(1f, 0.4f, 0f, 1f));
                    fireballMat.SetColor("_EmissionColor", new Color(2f, 0.8f, 0f, 1f));
                    fireballMat.EnableKeyword("_EMISSION");
                    fireballMat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                    
                    if (!Directory.Exists("Assets/Materials"))
                    {
                        Directory.CreateDirectory("Assets/Materials");
                    }
                    
                    string matPath = "Assets/Materials/FireballMaterial.mat";
                    AssetDatabase.CreateAsset(fireballMat, matPath);
                    fireballMat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
                }
                
                renderer.sharedMaterial = fireballMat;
                PrefabUtility.SaveAsPrefabAsset(fireballRoot, path);
                Debug.Log("✅ Fireball material: Orange with emission\n");
            }
        }
        
        PrefabUtility.UnloadPrefabContents(fireballRoot);
    }
    
    private static void RebuildImpPrefab()
    {
        Debug.Log("[STEP 3/3] Rebuilding Imp prefab...\n");
        
        string impPrefabPath = "Assets/Prefabs/Imp.prefab";
        GameObject impPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(impPrefabPath);
        
        if (impPrefab == null)
        {
            Debug.LogError("❌ Imp prefab not found!");
            return;
        }
        
        string path = AssetDatabase.GetAssetPath(impPrefab);
        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(path);
        
        Component[] allComponents = prefabRoot.GetComponents<Component>();
        foreach (Component comp in allComponents)
        {
            if (comp == null) continue;
            if (comp is Transform) continue;
            if (comp is Animator) continue;
            if (comp is NavMeshAgent) continue;
            
            string typeName = comp.GetType().Name;
            if (typeName.Contains("Imp") || typeName.Contains("Debug") || typeName.Contains("Fix") || typeName.Contains("Diagnostic"))
            {
                Debug.Log($"  ❌ Removing: {typeName}");
                Object.DestroyImmediate(comp, true);
            }
        }
        Debug.Log("  🧹 Cleaned debug/fix components");
        
        prefabRoot.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        prefabRoot.tag = "Enemy";
        prefabRoot.layer = LayerMask.NameToLayer("Enemy");
        Debug.Log("  📏 Scale: 0.5, Tag: Enemy, Layer: Enemy");
        
        NavMeshAgent agent = prefabRoot.GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            agent = prefabRoot.AddComponent<NavMeshAgent>();
        }
        agent.radius = 0.5f;
        agent.height = 2.5f;
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
        Debug.Log("  🚶 NavMeshAgent configured");
        
        CapsuleCollider collider = prefabRoot.GetComponent<CapsuleCollider>();
        if (collider == null)
        {
            collider = prefabRoot.AddComponent<CapsuleCollider>();
        }
        collider.center = new Vector3(0, 2f, 0);
        collider.radius = 0.6f;
        collider.height = 4f;
        collider.isTrigger = false;
        Debug.Log("  📦 CapsuleCollider configured");
        
        Animator animator = prefabRoot.GetComponent<Animator>();
        if (animator != null)
        {
            AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>("Assets/Imp/Animations/ImpAnimator.controller");
            if (controller != null)
            {
                animator.runtimeAnimatorController = controller;
                animator.applyRootMotion = false;
                
                while (controller.parameters.Length > 0)
                {
                    controller.RemoveParameter(0);
                }
                
                controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
                controller.AddParameter("Attack", AnimatorControllerParameterType.Trigger);
                controller.AddParameter("Die", AnimatorControllerParameterType.Trigger);
                
                EditorUtility.SetDirty(controller);
                AssetDatabase.SaveAssets();
                Debug.Log("  🎭 Animator: Speed, Attack, Die parameters");
            }
        }
        
        ImpAI impAI = prefabRoot.GetComponent<ImpAI>();
        if (impAI == null)
        {
            impAI = prefabRoot.AddComponent<ImpAI>();
        }
        impAI.maxHealth = 60f;
        impAI.health = 60f;
        impAI.attackDamage = 10;
        impAI.xpReward = 15;
        impAI.walkPointRange = 8f;
        impAI.sightRange = 20f;
        impAI.attackRange = 12f;
        impAI.timeBetweenAttacks = 3f;
        impAI.attackWindupTime = 0.8f;
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
        
        Debug.Log("  🧠 ImpAI script configured");
        
        foreach (Transform child in prefabRoot.GetComponentsInChildren<Transform>(true))
        {
            child.gameObject.layer = LayerMask.NameToLayer("Enemy");
        }
        Debug.Log("  🎨 All children set to Enemy layer");
        
        SkinnedMeshRenderer[] renderers = prefabRoot.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        Material impMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Imp/Materials/Imp Red Material.mat");
        
        if (impMaterial != null && renderers.Length > 0)
        {
            foreach (SkinnedMeshRenderer renderer in renderers)
            {
                renderer.sharedMaterial = impMaterial;
            }
            Debug.Log($"  🎨 Material assigned to {renderers.Length} renderer(s)");
        }
        
        PrefabUtility.SaveAsPrefabAsset(prefabRoot, path);
        PrefabUtility.UnloadPrefabContents(prefabRoot);
        
        Debug.Log("\n✅ Imp prefab rebuilt successfully!");
    }
}
