using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine.AI;

public class COMPLETE_IMP_FIX : MonoBehaviour
{
    [MenuItem("Tools/🔥 COMPLETE IMP FIX - Click Here!")]
    public static void FixEverything()
    {
        Debug.Log("╔═══════════════════════════════════════════════════════════╗");
        Debug.Log("║         COMPLETE IMP FIX - FIXING ALL ISSUES             ║");
        Debug.Log("╚═══════════════════════════════════════════════════════════╝");
        
        FixAnimatorController();
        FixPrefabSetup();
        FixMaterial();
        CleanupMissingScripts();
        
        Debug.Log("╔═══════════════════════════════════════════════════════════╗");
        Debug.Log("║                  ✅ ALL FIXES COMPLETE!                   ║");
        Debug.Log("╚═══════════════════════════════════════════════════════════╝");
        Debug.Log("Next: Test in Play mode!");
    }

    private static void FixAnimatorController()
    {
        Debug.Log("\n--- FIXING ANIMATOR CONTROLLER ---");
        
        string controllerPath = "Assets/Imp/Animations/ImpAnimator.controller";
        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
        
        if (controller == null)
        {
            Debug.LogError("❌ ImpAnimator.controller not found!");
            return;
        }

        bool needsRebuild = false;
        if (!HasParameter(controller, "Speed")) needsRebuild = true;
        if (!HasParameter(controller, "Attack")) needsRebuild = true;
        if (!HasParameter(controller, "Die")) needsRebuild = true;

        if (needsRebuild)
        {
            Debug.Log("⚠️ Animator Controller missing parameters - rebuilding...");
            
            while (controller.parameters.Length > 0)
            {
                controller.RemoveParameter(0);
            }
            
            controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
            controller.AddParameter("Attack", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Die", AnimatorControllerParameterType.Trigger);
            
            Debug.Log("✅ Added parameters: Speed (float), Attack (trigger), Die (trigger)");
            
            AnimationClip idleClip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Imp/Animations/Idle.anim");
            AnimationClip moveClip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Imp/Animations/Move.anim");
            AnimationClip attackClip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Imp/Animations/Attack1.anim");
            AnimationClip deadClip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Imp/Animations/Dead.anim");
            
            if (controller.layers.Length > 0)
            {
                AnimatorStateMachine rootStateMachine = controller.layers[0].stateMachine;
                
                foreach (var state in rootStateMachine.states)
                {
                    rootStateMachine.RemoveState(state.state);
                }
                
                AnimatorState idleState = rootStateMachine.AddState("Idle");
                AnimatorState moveState = rootStateMachine.AddState("Move");
                AnimatorState attackState = rootStateMachine.AddState("Attack");
                AnimatorState deadState = rootStateMachine.AddState("Dead");
                
                if (idleClip) idleState.motion = idleClip;
                if (moveClip) moveState.motion = moveClip;
                if (attackClip) attackState.motion = attackClip;
                if (deadClip) deadState.motion = deadClip;
                
                rootStateMachine.defaultState = idleState;
                
                var idleToMove = idleState.AddTransition(moveState);
                idleToMove.hasExitTime = false;
                idleToMove.AddCondition(AnimatorConditionMode.Greater, 0.1f, "Speed");
                
                var moveToIdle = moveState.AddTransition(idleState);
                moveToIdle.hasExitTime = false;
                moveToIdle.AddCondition(AnimatorConditionMode.Less, 0.1f, "Speed");
                
                var idleToAttack = idleState.AddTransition(attackState);
                idleToAttack.hasExitTime = false;
                idleToAttack.AddCondition(AnimatorConditionMode.If, 0, "Attack");
                
                var moveToAttack = moveState.AddTransition(attackState);
                moveToAttack.hasExitTime = false;
                moveToAttack.AddCondition(AnimatorConditionMode.If, 0, "Attack");
                
                var attackToIdle = attackState.AddTransition(idleState);
                attackToIdle.hasExitTime = true;
                attackToIdle.exitTime = 0.9f;
                
                var anyToDead = rootStateMachine.AddAnyStateTransition(deadState);
                anyToDead.AddCondition(AnimatorConditionMode.If, 0, "Die");
                anyToDead.hasExitTime = false;
                
                Debug.Log("✅ Created animation states and transitions");
            }
            
            EditorUtility.SetDirty(controller);
            AssetDatabase.SaveAssets();
            Debug.Log("✅ Animator Controller saved");
        }
        else
        {
            Debug.Log("✅ Animator Controller already has all required parameters");
        }
    }

    private static bool HasParameter(AnimatorController controller, string paramName)
    {
        foreach (var param in controller.parameters)
        {
            if (param.name == paramName) return true;
        }
        return false;
    }

    private static void FixPrefabSetup()
    {
        Debug.Log("\n--- FIXING PREFAB SETUP ---");
        
        string prefabPath = "Assets/Prefabs/Imp.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        
        if (prefab == null)
        {
            Debug.LogError("❌ Imp prefab not found!");
            return;
        }

        string path = AssetDatabase.GetAssetPath(prefab);
        GameObject prefabContents = PrefabUtility.LoadPrefabContents(path);
        
        prefabContents.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        Debug.Log("✅ Scale: 0.4");

        NavMeshAgent agent = prefabContents.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.radius = 0.35f;
            agent.height = 1.2f;
            agent.baseOffset = 0.3f;
            Debug.Log("✅ NavMeshAgent - radius: 0.35, height: 1.2, baseOffset: 0.3 (raised higher to prevent ground clipping)");
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
        Debug.Log("✅ CapsuleCollider configured (matches model size at scale 0.4)");

        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>("Assets/Imp/Animations/ImpAnimator.controller");
        Animator animator = prefabContents.GetComponentInChildren<Animator>();
        if (animator != null && controller != null)
        {
            animator.runtimeAnimatorController = controller;
            animator.applyRootMotion = false;
            animator.updateMode = AnimatorUpdateMode.Normal;
            Debug.Log("✅ Animator configured");
        }

        Material impMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Imp/Materials/Imp Red Material.mat");
        if (impMaterial != null)
        {
            Renderer[] renderers = prefabContents.GetComponentsInChildren<Renderer>();
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

        PrefabUtility.SaveAsPrefabAsset(prefabContents, path);
        PrefabUtility.UnloadPrefabContents(prefabContents);
        
        Debug.Log("✅ Prefab saved");
    }

    private static void FixMaterial()
    {
        Debug.Log("\n--- FIXING MATERIAL ---");
        
        string materialPath = "Assets/Imp/Materials/Imp Red Material.mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        
        if (mat == null)
        {
            Debug.LogError("❌ Material not found!");
            return;
        }

        Shader urpLit = Shader.Find("Universal Render Pipeline/Lit");
        if (urpLit != null)
        {
            mat.shader = urpLit;
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
            Debug.Log($"✅ Texture assigned: {colorTex.name}");
        }
        else
        {
            mat.SetColor("_BaseColor", new Color(0.8f, 0.2f, 0.2f, 1f));
            Debug.Log("⚠️ Texture not found, using red color");
        }

        EditorUtility.SetDirty(mat);
        AssetDatabase.SaveAssets();
        Debug.Log("✅ Material saved");
    }

    private static void CleanupMissingScripts()
    {
        Debug.Log("\n--- CLEANING UP MISSING SCRIPTS ---");
        
        string prefabPath = "Assets/Prefabs/Imp.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        
        if (prefab == null) return;

        string path = AssetDatabase.GetAssetPath(prefab);
        GameObject prefabContents = PrefabUtility.LoadPrefabContents(path);
        
        int removedCount = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(prefabContents);
        
        if (removedCount > 0)
        {
            Debug.Log($"✅ Removed {removedCount} missing script(s)");
            PrefabUtility.SaveAsPrefabAsset(prefabContents, path);
        }
        else
        {
            Debug.Log("✅ No missing scripts found");
        }
        
        PrefabUtility.UnloadPrefabContents(prefabContents);
    }
}
