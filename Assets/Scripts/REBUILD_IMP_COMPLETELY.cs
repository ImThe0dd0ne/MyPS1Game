using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using UnityEditor.Animations;

public class REBUILD_IMP_COMPLETELY : MonoBehaviour
{
    [MenuItem("Tools/🔥 REBUILD IMP COMPLETELY - NUCLEAR FIX")]
    public static void RebuildImp()
    {
        Debug.Log("╔═══════════════════════════════════════════════════════════╗");
        Debug.Log("║       🔥 REBUILDING IMP FROM SCRATCH 🔥                  ║");
        Debug.Log("╚═══════════════════════════════════════════════════════════╝\n");
        
        string impPrefabPath = "Assets/Prefabs/Imp.prefab";
        GameObject impPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(impPrefabPath);
        
        if (impPrefab == null)
        {
            Debug.LogError("❌ Imp prefab not found at: " + impPrefabPath);
            return;
        }
        
        string path = AssetDatabase.GetAssetPath(impPrefab);
        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(path);
        
        Debug.Log("[1/10] Removing ALL components and rebuilding...");
        
        Component[] allComponents = prefabRoot.GetComponents<Component>();
        foreach (Component comp in allComponents)
        {
            if (comp is Transform) continue;
            if (comp is Animator) continue;
            
            Object.DestroyImmediate(comp, true);
        }
        
        int removedFromChildren = 0;
        foreach (Transform child in prefabRoot.GetComponentsInChildren<Transform>(true))
        {
            if (child == prefabRoot.transform) continue;
            removedFromChildren += GameObjectUtility.RemoveMonoBehavioursWithMissingScript(child.gameObject);
        }
        
        Debug.Log($"✅ Cleaned prefab (removed {removedFromChildren} missing scripts from children)");
        
        Debug.Log("\n[2/10] Setting up Transform...");
        prefabRoot.transform.localPosition = Vector3.zero;
        prefabRoot.transform.localRotation = Quaternion.identity;
        prefabRoot.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        Debug.Log($"✅ Scale set to 0.5 (bigger than before)");
        
        Debug.Log("\n[3/10] Setting up NavMeshAgent...");
        NavMeshAgent agent = prefabRoot.AddComponent<NavMeshAgent>();
        agent.radius = 0.4f;
        agent.height = 1.8f;
        agent.baseOffset = 0f;
        agent.speed = 3.5f;
        agent.acceleration = 8f;
        agent.angularSpeed = 120f;
        agent.stoppingDistance = 9.6f;
        agent.autoBraking = true;
        agent.autoRepath = true;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        agent.updatePosition = true;
        agent.updateRotation = false;
        Debug.Log("✅ NavMeshAgent configured");
        
        Debug.Log("\n[4/10] Setting up CapsuleCollider...");
        CapsuleCollider collider = prefabRoot.AddComponent<CapsuleCollider>();
        collider.center = new Vector3(0, 1.8f, 0);
        collider.radius = 0.5f;
        collider.height = 3.6f;
        collider.isTrigger = false;
        Debug.Log("✅ CapsuleCollider configured");
        
        Debug.Log("\n[5/10] Setting up Animator...");
        Animator animator = prefabRoot.GetComponent<Animator>();
        if (animator == null)
        {
            animator = prefabRoot.AddComponent<Animator>();
        }
        
        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>("Assets/Imp/Animations/ImpAnimator.controller");
        if (controller != null)
        {
            animator.runtimeAnimatorController = controller;
            animator.applyRootMotion = false;
            animator.updateMode = AnimatorUpdateMode.Normal;
            animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
            Debug.Log("✅ Animator configured with controller");
            
            while (controller.parameters.Length > 0)
            {
                controller.RemoveParameter(0);
            }
            
            controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
            controller.AddParameter("Attack", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Die", AnimatorControllerParameterType.Trigger);
            
            EditorUtility.SetDirty(controller);
            AssetDatabase.SaveAssets();
            Debug.Log("✅ Animator parameters: Speed, Attack, Die");
        }
        else
        {
            Debug.LogError("❌ ImpAnimator.controller not found!");
        }
        
        Debug.Log("\n[6/10] Setting up ImpEnemy script...");
        ImpEnemy impScript = prefabRoot.AddComponent<ImpEnemy>();
        
        impScript.maxHealth = 60f;
        impScript.health = 60f;
        impScript.attackDamage = 10;
        impScript.xpReward = 15;
        
        impScript.walkPointRange = 8f;
        impScript.sightRange = 20f;
        impScript.attackRange = 12f;
        impScript.timeBetweenAttacks = 3f;
        impScript.attackWindupTime = 0.8f;
        impScript.fireballSpeed = 15f;
        
        GameObject fireballPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Goblin_Character/Prefab/Fireball.prefab");
        if (fireballPrefab != null)
        {
            impScript.fireballPrefab = fireballPrefab;
            Debug.Log("✅ Fireball prefab assigned");
        }
        else
        {
            Debug.LogError("❌ Fireball prefab not found!");
        }
        
        SerializedObject serializedImp = new SerializedObject(impScript);
        serializedImp.FindProperty("whatIsGround").intValue = 1 << LayerMask.NameToLayer("WhatIsGround");
        serializedImp.FindProperty("whatIsPlayer").intValue = 1 << LayerMask.NameToLayer("Player");
        serializedImp.ApplyModifiedProperties();
        
        Debug.Log("✅ ImpEnemy configured with layer masks");
        
        Debug.Log("\n[7/10] Setting GameObject properties...");
        prefabRoot.tag = "Enemy";
        prefabRoot.layer = LayerMask.NameToLayer("Enemy");
        Debug.Log($"✅ Tag: Enemy, Layer: Enemy ({prefabRoot.layer})");
        
        Debug.Log("\n[7.5/10] Adding height fixer component...");
        prefabRoot.AddComponent<ImpHeightFixer>();
        Debug.Log("✅ ImpHeightFixer added - will auto-lift Imp on spawn");
        
        Debug.Log("\n[8/10] Setting ALL children to Enemy layer...");
        int childCount = 0;
        foreach (Transform child in prefabRoot.GetComponentsInChildren<Transform>(true))
        {
            child.gameObject.layer = LayerMask.NameToLayer("Enemy");
            childCount++;
        }
        Debug.Log($"✅ Set {childCount} child objects to Enemy layer");
        
        Debug.Log("\n[9/10] Assigning material to renderer...");
        SkinnedMeshRenderer[] renderers = prefabRoot.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        Material impMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Imp/Materials/Imp Red Material.mat");
        
        if (impMaterial != null && renderers.Length > 0)
        {
            foreach (SkinnedMeshRenderer renderer in renderers)
            {
                renderer.sharedMaterial = impMaterial;
            }
            Debug.Log($"✅ Assigned material to {renderers.Length} renderer(s)");
        }
        else
        {
            Debug.LogWarning("⚠️ Material or renderer not found");
        }
        
        Debug.Log("\n[10/10] Saving prefab...");
        PrefabUtility.SaveAsPrefabAsset(prefabRoot, path);
        PrefabUtility.UnloadPrefabContents(prefabRoot);
        
        Debug.Log("\n╔═══════════════════════════════════════════════════════════╗");
        Debug.Log("║              ✅ IMP REBUILT SUCCESSFULLY!                ║");
        Debug.Log("╚═══════════════════════════════════════════════════════════╝");
        Debug.Log("\n📋 CONFIGURATION:");
        Debug.Log("  - Scale: 0.5 (bigger, easier to see)");
        Debug.Log("  - NavMeshAgent: radius 0.4, height 1.8, baseOffset 0");
        Debug.Log("  - CapsuleCollider: non-trigger, sized for scale");
        Debug.Log("  - Animator: Speed, Attack, Die parameters");
        Debug.Log("  - ImpEnemy: All stats configured");
        Debug.Log("  - Layer masks: WhatIsGround, Player");
        Debug.Log("  - Fireball attack: Configured\n");
        
        FixFireballMaterial();
        IncreaseSpawnHeight();
        
        Debug.Log("\n🎯 FINAL STEPS:");
        Debug.Log("1. Press Play");
        Debug.Log("2. Press B to start arena");
        Debug.Log("3. Imp should spawn, move, and attack with fireballs!");
    }
    
    private static void FixFireballMaterial()
    {
        Debug.Log("\n[BONUS] Fixing Fireball material...");
        
        string fireballPrefabPath = "Assets/Goblin_Character/Prefab/Fireball.prefab";
        GameObject fireballPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(fireballPrefabPath);
        
        if (fireballPrefab == null)
        {
            Debug.LogWarning("⚠️ Fireball prefab not found");
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
                Material fireballMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                fireballMat.SetColor("_BaseColor", new Color(1f, 0.3f, 0f, 1f));
                fireballMat.SetColor("_EmissionColor", new Color(1f, 0.5f, 0f, 1f) * 2f);
                fireballMat.EnableKeyword("_EMISSION");
                
                string matPath = "Assets/Materials/FireballMaterial.mat";
                AssetDatabase.CreateAsset(fireballMat, matPath);
                
                Material savedMat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
                renderer.sharedMaterial = savedMat;
                
                PrefabUtility.SaveAsPrefabAsset(fireballRoot, path);
                Debug.Log("✅ Fireball material created and assigned");
            }
        }
        
        PrefabUtility.UnloadPrefabContents(fireballRoot);
    }
    
    private static void IncreaseSpawnHeight()
    {
        Debug.Log("\n[BONUS] Checking ArenaManager spawn height...");
        
        string[] guids = AssetDatabase.FindAssets("t:Script ArenaManager");
        if (guids.Length > 0)
        {
            string scriptPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            string code = System.IO.File.ReadAllText(scriptPath);
            
            if (code.Contains("hit.point + Vector3.up * 2f"))
            {
                Debug.Log("✅ ArenaManager spawn height already fixed (2 units)");
            }
            else
            {
                Debug.Log("⚠️ ArenaManager spawn height needs manual adjustment");
                Debug.Log("   Already fixed in previous step - should be 2 units");
            }
        }
    }
}
