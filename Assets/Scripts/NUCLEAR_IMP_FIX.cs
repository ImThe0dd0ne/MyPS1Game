using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using UnityEditor.Animations;

public class NUCLEAR_IMP_FIX : MonoBehaviour
{
    [MenuItem("Tools/🚀 NUCLEAR IMP FIX - REMOVE ALL INTERFERENCE")]
    public static void NuclearFix()
    {
        Debug.Log("╔═══════════════════════════════════════════════════════════╗");
        Debug.Log("║          🚀 NUCLEAR IMP FIX - FINAL SOLUTION             ║");
        Debug.Log("╚═══════════════════════════════════════════════════════════╝\n");
        
        string impPrefabPath = "Assets/Prefabs/Imp.prefab";
        GameObject impPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(impPrefabPath);
        
        if (impPrefab == null)
        {
            Debug.LogError("❌ Imp prefab not found!");
            return;
        }
        
        string path = AssetDatabase.GetAssetPath(impPrefab);
        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(path);
        
        Debug.Log("[1/8] REMOVING ALL INTERFERING COMPONENTS...");
        
        Component[] componentsToRemove = prefabRoot.GetComponents<Component>();
        foreach (Component comp in componentsToRemove)
        {
            if (comp is Transform) continue;
            if (comp is Animator) continue;
            if (comp is NavMeshAgent) continue;
            if (comp is ImpEnemy) continue;
            if (comp is CapsuleCollider) continue;
            
            string componentType = comp.GetType().Name;
            if (componentType.Contains("Debug") || componentType.Contains("Fix") || componentType.Contains("Runtime") || componentType.Contains("Spawn") || componentType.Contains("Movement"))
            {
                Debug.Log($"  ❌ Removing interfering component: {componentType}");
                Object.DestroyImmediate(comp, true);
            }
        }
        
        Debug.Log("✅ Removed all interfering components\n");
        
        Debug.Log("[2/8] Checking Rigidbody...");
        Rigidbody rb = prefabRoot.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Debug.Log("  ⚠️ Found Rigidbody - checking if it interferes...");
            if (!rb.isKinematic)
            {
                Debug.Log("  ❌ Rigidbody is NOT kinematic - THIS IS THE PROBLEM!");
                Debug.Log("  🔧 Setting Rigidbody.isKinematic = true");
                rb.isKinematic = true;
            }
            else
            {
                Debug.Log("  ✅ Rigidbody is kinematic (good)");
            }
        }
        else
        {
            Debug.Log("  ✅ No Rigidbody found (good)\n");
        }
        
        Debug.Log("[3/8] Configuring NavMeshAgent...");
        NavMeshAgent agent = prefabRoot.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.radius = 0.35f;
            agent.height = 1.5f;
            agent.baseOffset = 0.1f;
            agent.speed = 3.5f;
            agent.acceleration = 8f;
            agent.angularSpeed = 120f;
            agent.stoppingDistance = 9.6f;
            agent.autoBraking = true;
            agent.autoRepath = true;
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            agent.updatePosition = true;
            agent.updateRotation = false;
            Debug.Log($"✅ NavMeshAgent configured:");
            Debug.Log($"   - updatePosition: {agent.updatePosition} (CRITICAL!)");
            Debug.Log($"   - speed: {agent.speed}");
            Debug.Log($"   - baseOffset: {agent.baseOffset}\n");
        }
        else
        {
            Debug.LogError("❌ NavMeshAgent not found!\n");
        }
        
        Debug.Log("[4/8] Configuring CapsuleCollider...");
        CapsuleCollider collider = prefabRoot.GetComponent<CapsuleCollider>();
        if (collider == null)
        {
            collider = prefabRoot.AddComponent<CapsuleCollider>();
            Debug.Log("  ➕ Added CapsuleCollider");
        }
        collider.center = new Vector3(0, 1.5f, 0);
        collider.radius = 0.4f;
        collider.height = 3f;
        collider.isTrigger = false;
        Debug.Log($"✅ CapsuleCollider configured (non-trigger)\n");
        
        Debug.Log("[5/8] Configuring Animator...");
        Animator animator = prefabRoot.GetComponent<Animator>();
        if (animator != null)
        {
            AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>("Assets/Imp/Animations/ImpAnimator.controller");
            if (controller != null)
            {
                animator.runtimeAnimatorController = controller;
                animator.applyRootMotion = false;
                
                bool hasSpeed = false;
                bool hasAttack = false;
                bool hasDie = false;
                
                foreach (var param in controller.parameters)
                {
                    if (param.name == "Speed") hasSpeed = true;
                    if (param.name == "Attack") hasAttack = true;
                    if (param.name == "Die") hasDie = true;
                }
                
                if (!hasSpeed || !hasAttack || !hasDie)
                {
                    Debug.Log("  🔧 Adding missing animator parameters...");
                    while (controller.parameters.Length > 0)
                    {
                        controller.RemoveParameter(0);
                    }
                    controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
                    controller.AddParameter("Attack", AnimatorControllerParameterType.Trigger);
                    controller.AddParameter("Die", AnimatorControllerParameterType.Trigger);
                    EditorUtility.SetDirty(controller);
                    AssetDatabase.SaveAssets();
                }
                Debug.Log("✅ Animator configured with Speed, Attack, Die parameters\n");
            }
        }
        
        Debug.Log("[6/8] Configuring ImpEnemy script...");
        ImpEnemy impScript = prefabRoot.GetComponent<ImpEnemy>();
        if (impScript != null)
        {
            GameObject fireballPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Goblin_Character/Prefab/Fireball.prefab");
            if (fireballPrefab != null)
            {
                impScript.fireballPrefab = fireballPrefab;
            }
            
            SerializedObject serializedImp = new SerializedObject(impScript);
            serializedImp.FindProperty("whatIsGround").intValue = 1 << LayerMask.NameToLayer("WhatIsGround");
            serializedImp.FindProperty("whatIsPlayer").intValue = 1 << LayerMask.NameToLayer("Player");
            serializedImp.ApplyModifiedProperties();
            
            Debug.Log("✅ ImpEnemy configured\n");
        }
        
        Debug.Log("[7/8] Setting scale and layers...");
        prefabRoot.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        prefabRoot.tag = "Enemy";
        prefabRoot.layer = LayerMask.NameToLayer("Enemy");
        
        foreach (Transform child in prefabRoot.GetComponentsInChildren<Transform>(true))
        {
            child.gameObject.layer = LayerMask.NameToLayer("Enemy");
        }
        Debug.Log("✅ Scale 0.4, all layers set to Enemy\n");
        
        Debug.Log("[8/8] Saving prefab...");
        PrefabUtility.SaveAsPrefabAsset(prefabRoot, path);
        PrefabUtility.UnloadPrefabContents(prefabRoot);
        
        Debug.Log("\n╔═══════════════════════════════════════════════════════════╗");
        Debug.Log("║              ✅ NUCLEAR FIX COMPLETE!                    ║");
        Debug.Log("╚═══════════════════════════════════════════════════════════╝");
        Debug.Log("\n📋 WHAT WAS FIXED:");
        Debug.Log("  ✅ Removed ALL interfering debug/fix components");
        Debug.Log("  ✅ Fixed Rigidbody (if present)");
        Debug.Log("  ✅ NavMeshAgent updatePosition = true");
        Debug.Log("  ✅ CapsuleCollider (non-trigger)");
        Debug.Log("  ✅ Animator parameters");
        Debug.Log("  ✅ ImpEnemy script");
        Debug.Log("  ✅ Layers and scale\n");
        
        FixFireball();
        
        Debug.Log("\n🎯 NOW TEST:");
        Debug.Log("  1. Press Play");
        Debug.Log("  2. Press B to start arena");
        Debug.Log("  3. Imp should MOVE and ATTACK!\n");
    }
    
    private static void FixFireball()
    {
        Debug.Log("[BONUS] Fixing Fireball prefab...");
        
        string fireballPrefabPath = "Assets/Goblin_Character/Prefab/Fireball.prefab";
        GameObject fireballPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(fireballPrefabPath);
        
        if (fireballPrefab != null)
        {
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
                        fireballMat.SetColor("_BaseColor", new Color(1f, 0.3f, 0f, 1f));
                        fireballMat.SetColor("_EmissionColor", new Color(1f, 0.5f, 0f, 1f) * 2f);
                        fireballMat.EnableKeyword("_EMISSION");
                        
                        string matPath = "Assets/Materials/FireballMaterial.mat";
                        AssetDatabase.CreateAsset(fireballMat, matPath);
                        fireballMat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
                    }
                    
                    renderer.sharedMaterial = fireballMat;
                    PrefabUtility.SaveAsPrefabAsset(fireballRoot, path);
                    Debug.Log("✅ Fireball material fixed\n");
                }
            }
            
            PrefabUtility.UnloadPrefabContents(fireballRoot);
        }
    }
}
