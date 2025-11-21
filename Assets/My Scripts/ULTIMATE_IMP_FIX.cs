using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine.AI;

public class ULTIMATE_IMP_FIX : MonoBehaviour
{
    [MenuItem("Tools/🚨 ULTIMATE IMP FIX - Nuclear Option")]
    public static void NuclearFix()
    {
        Debug.Log("╔═══════════════════════════════════════════════════════════╗");
        Debug.Log("║         🚨 ULTIMATE IMP FIX - NUCLEAR OPTION 🚨          ║");
        Debug.Log("║              Analyzing EVERYTHING...                     ║");
        Debug.Log("╚═══════════════════════════════════════════════════════════╝\n");
        
        AnalyzeAndFixAnimator();
        AnalyzeAndFixPrefab();
        AnalyzeAndFixMaterial();
        AnalyzeLayerMasks();
        
        Debug.Log("\n╔═══════════════════════════════════════════════════════════╗");
        Debug.Log("║              ✅ NUCLEAR FIX COMPLETE!                    ║");
        Debug.Log("╚═══════════════════════════════════════════════════════════╝");
        Debug.Log("\n🔥 CRITICAL NEXT STEPS:");
        Debug.Log("1. Test in Play mode NOW");
        Debug.Log("2. If still stuck: Check console for 'agent.isOnNavMesh = false' message");
        Debug.Log("3. Make sure NavMesh is baked and covers spawn areas!\n");
    }

    private static void AnalyzeAndFixAnimator()
    {
        Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Debug.Log("[1] ANIMATOR CONTROLLER ANALYSIS");
        Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        
        string controllerPath = "Assets/Imp/Animations/ImpAnimator.controller";
        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
        
        if (controller == null)
        {
            Debug.LogError("❌ CRITICAL: ImpAnimator.controller NOT FOUND!");
            return;
        }
        
        Debug.Log("✅ Controller found");
        
        bool hasSpeed = false;
        bool hasAttack = false;
        bool hasDie = false;
        
        Debug.Log($"Current parameters ({controller.parameters.Length}):");
        foreach (var param in controller.parameters)
        {
            Debug.Log($"   - {param.name} ({param.type})");
            if (param.name == "Speed") hasSpeed = true;
            if (param.name == "Attack") hasAttack = true;
            if (param.name == "Die") hasDie = true;
        }
        
        if (!hasSpeed || !hasAttack || !hasDie)
        {
            Debug.LogWarning("⚠️ MISSING PARAMETERS - Fixing...");
            
            while (controller.parameters.Length > 0)
            {
                controller.RemoveParameter(0);
            }
            
            controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
            controller.AddParameter("Attack", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Die", AnimatorControllerParameterType.Trigger);
            
            Debug.Log("✅ Added: Speed (float), Attack (trigger), Die (trigger)");
            
            AnimationClip idle = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Imp/Animations/Idle.anim");
            AnimationClip move = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Imp/Animations/Move.anim");
            AnimationClip attack = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Imp/Animations/Attack1.anim");
            AnimationClip dead = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Imp/Animations/Dead.anim");
            
            if (controller.layers.Length > 0)
            {
                AnimatorStateMachine sm = controller.layers[0].stateMachine;
                
                var states = new System.Collections.Generic.List<ChildAnimatorState>(sm.states);
                foreach (var state in states)
                {
                    sm.RemoveState(state.state);
                }
                
                var idleState = sm.AddState("Idle");
                var moveState = sm.AddState("Move");
                var attackState = sm.AddState("Attack");
                var deadState = sm.AddState("Dead");
                
                if (idle) idleState.motion = idle;
                if (move) moveState.motion = move;
                if (attack) attackState.motion = attack;
                if (dead) deadState.motion = dead;
                
                sm.defaultState = idleState;
                
                var t1 = idleState.AddTransition(moveState);
                t1.hasExitTime = false;
                t1.AddCondition(AnimatorConditionMode.Greater, 0.1f, "Speed");
                
                var t2 = moveState.AddTransition(idleState);
                t2.hasExitTime = false;
                t2.AddCondition(AnimatorConditionMode.Less, 0.1f, "Speed");
                
                var t3 = idleState.AddTransition(attackState);
                t3.hasExitTime = false;
                t3.AddCondition(AnimatorConditionMode.If, 0, "Attack");
                
                var t4 = moveState.AddTransition(attackState);
                t4.hasExitTime = false;
                t4.AddCondition(AnimatorConditionMode.If, 0, "Attack");
                
                var t5 = attackState.AddTransition(idleState);
                t5.hasExitTime = true;
                t5.exitTime = 0.9f;
                
                var t6 = sm.AddAnyStateTransition(deadState);
                t6.AddCondition(AnimatorConditionMode.If, 0, "Die");
                
                Debug.Log("✅ Created states and transitions");
            }
            
            EditorUtility.SetDirty(controller);
            AssetDatabase.SaveAssets();
        }
        else
        {
            Debug.Log("✅ All parameters present");
        }
    }

    private static void AnalyzeAndFixPrefab()
    {
        Debug.Log("\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Debug.Log("[2] PREFAB ANALYSIS & FIX");
        Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        
        string prefabPath = "Assets/Prefabs/Imp.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        
        if (prefab == null)
        {
            Debug.LogError("❌ CRITICAL: Imp prefab NOT FOUND!");
            return;
        }
        
        string path = AssetDatabase.GetAssetPath(prefab);
        GameObject prefabContents = PrefabUtility.LoadPrefabContents(path);
        
        Debug.Log($"Scale: {prefabContents.transform.localScale}");
        prefabContents.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        Debug.Log("✅ Scale set to 0.4");
        
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        Debug.Log($"Enemy layer index: {enemyLayer}");
        
        if (enemyLayer != -1)
        {
            SetLayerRecursively(prefabContents, enemyLayer);
            Debug.Log("✅ Set layer to Enemy on all children");
        }
        else
        {
            Debug.LogError("❌ Enemy layer doesn't exist!");
        }
        
        NavMeshAgent agent = prefabContents.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.radius = 0.35f;
            agent.height = 1.2f;
            agent.baseOffset = 0f;
            agent.speed = 3.5f;
            agent.acceleration = 8f;
            agent.angularSpeed = 120f;
            agent.stoppingDistance = 9.6f;
            agent.autoBraking = true;
            agent.updateRotation = false;
            
            Debug.Log("✅ NavMeshAgent configured:");
            Debug.Log($"   - radius: {agent.radius}");
            Debug.Log($"   - height: {agent.height}");
            Debug.Log($"   - baseOffset: {agent.baseOffset} (spawn fix will handle height)");
            Debug.Log($"   - stoppingDistance: {agent.stoppingDistance}");
        }
        else
        {
            Debug.LogError("❌ NavMeshAgent MISSING!");
        }
        
        CapsuleCollider capsule = prefabContents.GetComponent<CapsuleCollider>();
        if (capsule == null)
        {
            capsule = prefabContents.AddComponent<CapsuleCollider>();
            Debug.Log("✅ Added CapsuleCollider");
        }
        capsule.center = new Vector3(0, 1.5f, 0);
        capsule.radius = 0.5f;
        capsule.height = 3f;
        Debug.Log("✅ CapsuleCollider configured");
        
        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>("Assets/Imp/Animations/ImpAnimator.controller");
        Animator animator = prefabContents.GetComponentInChildren<Animator>();
        if (animator != null && controller != null)
        {
            animator.runtimeAnimatorController = controller;
            animator.applyRootMotion = false;
            animator.updateMode = AnimatorUpdateMode.Normal;
            Debug.Log("✅ Animator configured");
        }
        else
        {
            Debug.LogError("❌ Animator or Controller missing!");
        }
        
        ImpEnemy impScript = prefabContents.GetComponent<ImpEnemy>();
        if (impScript != null)
        {
            Debug.Log("✅ ImpEnemy script found");
            
            Debug.Log("Checking layer masks...");
            SerializedObject so = new SerializedObject(prefabContents);
            
            var impComponent = so.FindProperty("m_Component");
            for (int i = 0; i < impComponent.arraySize; i++)
            {
                var comp = impComponent.GetArrayElementAtIndex(i).objectReferenceValue;
                if (comp is ImpEnemy)
                {
                    SerializedObject impSo = new SerializedObject(comp);
                    
                    SerializedProperty whatIsPlayerProp = impSo.FindProperty("whatIsPlayer");
                    SerializedProperty whatIsGroundProp = impSo.FindProperty("whatIsGround");
                    
                    int playerLayer = LayerMask.NameToLayer("Player");
                    int groundLayer = LayerMask.NameToLayer("WhatIsGround");
                    
                    if (playerLayer != -1)
                    {
                        whatIsPlayerProp.intValue = 1 << playerLayer;
                        Debug.Log($"✅ whatIsPlayer = Player layer ({playerLayer})");
                    }
                    
                    if (groundLayer != -1)
                    {
                        whatIsGroundProp.intValue = 1 << groundLayer;
                        Debug.Log($"✅ whatIsGround = WhatIsGround layer ({groundLayer})");
                    }
                    
                    impSo.ApplyModifiedProperties();
                }
            }
        }
        
        Material impMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Imp/Materials/Imp Red Material.mat");
        if (impMaterial != null)
        {
            Renderer[] renderers = prefabContents.GetComponentsInChildren<Renderer>();
            Debug.Log($"Found {renderers.Length} renderers");
            
            foreach (Renderer renderer in renderers)
            {
                Material[] mats = renderer.sharedMaterials;
                for (int i = 0; i < mats.Length; i++)
                {
                    mats[i] = impMaterial;
                }
                renderer.sharedMaterials = mats;
            }
            Debug.Log("✅ Material assigned to all renderers");
        }
        
        int removedScripts = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(prefabContents);
        if (removedScripts > 0)
        {
            Debug.Log($"✅ Removed {removedScripts} missing script(s)");
        }
        
        if (prefabContents.GetComponent<ImpRuntimeDebug>() == null)
        {
            prefabContents.AddComponent<ImpRuntimeDebug>();
            Debug.Log("✅ Added ImpRuntimeDebug component for diagnostics");
        }
        
        if (prefabContents.GetComponent<ImpSpawnFix>() == null)
        {
            prefabContents.AddComponent<ImpSpawnFix>();
            Debug.Log("✅ Added ImpSpawnFix component - will auto-lift Imp 1.5 units on spawn!");
        }
        
        PrefabUtility.SaveAsPrefabAsset(prefabContents, path);
        PrefabUtility.UnloadPrefabContents(prefabContents);
        
        Debug.Log("✅ Prefab saved");
    }

    private static void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    private static void AnalyzeAndFixMaterial()
    {
        Debug.Log("\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Debug.Log("[3] MATERIAL ANALYSIS");
        Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        
        string materialPath = "Assets/Imp/Materials/Imp Red Material.mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        
        if (mat == null)
        {
            Debug.LogError("❌ Material NOT FOUND!");
            return;
        }
        
        Debug.Log($"Shader: {mat.shader.name}");
        
        Shader urpLit = Shader.Find("Universal Render Pipeline/Lit");
        if (urpLit != null)
        {
            mat.shader = urpLit;
            Debug.Log("✅ Shader set to URP/Lit");
        }
        
        Texture2D colorTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Imp/Textures/Imp.Color.Complete.png");
        if (colorTex == null)
        {
            colorTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Imp/Textures/ImpColorBrownComplet.png");
        }
        
        if (colorTex != null)
        {
            mat.SetTexture("_BaseMap", colorTex);
            mat.SetColor("_BaseColor", Color.white);
            Debug.Log($"✅ Texture: {colorTex.name}");
        }
        else
        {
            mat.SetColor("_BaseColor", new Color(0.8f, 0.2f, 0.2f, 1f));
            Debug.LogWarning("⚠️ Texture not found, using red color");
        }
        
        EditorUtility.SetDirty(mat);
        AssetDatabase.SaveAssets();
    }

    private static void AnalyzeLayerMasks()
    {
        Debug.Log("\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Debug.Log("[4] LAYER CONFIGURATION");
        Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        
        int playerLayer = LayerMask.NameToLayer("Player");
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        int groundLayer = LayerMask.NameToLayer("WhatIsGround");
        
        Debug.Log($"Player layer: {playerLayer} {(playerLayer == -1 ? "❌ MISSING!" : "✅")}");
        Debug.Log($"Enemy layer: {enemyLayer} {(enemyLayer == -1 ? "❌ MISSING!" : "✅")}");
        Debug.Log($"WhatIsGround layer: {groundLayer} {(groundLayer == -1 ? "❌ MISSING!" : "✅")}");
        
        if (playerLayer == -1 || enemyLayer == -1 || groundLayer == -1)
        {
            Debug.LogError("❌ CRITICAL: Missing required layers!");
            Debug.LogError("   Add these in: Edit → Project Settings → Tags and Layers");
        }
    }
}
