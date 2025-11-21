using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

public class SetupImpAnimator : MonoBehaviour
{
    [MenuItem("Tools/Setup Imp Animator Controller")]
    public static void CreateAnimatorController()
    {
        string controllerPath = "Assets/Imp/Animations/ImpAnimator.controller";
        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
        
        if (controller == null)
        {
            Debug.LogError("Animator Controller not found at: " + controllerPath);
            return;
        }

        AnimationClip idle = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Imp/Animations/Idle.anim");
        AnimationClip move = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Imp/Animations/Move.anim");
        AnimationClip attack1 = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Imp/Animations/Attack1.anim");
        AnimationClip takeDamage = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Imp/Animations/Take Damage.anim");
        AnimationClip dead = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Imp/Animations/Dead.anim");

        if (idle == null || move == null || attack1 == null || dead == null)
        {
            Debug.LogError("Some animation clips are missing!");
            return;
        }

        controller.layers[0].stateMachine.states = new ChildAnimatorState[0];
        controller.layers[0].stateMachine.anyStateTransitions = new AnimatorStateTransition[0];
        controller.layers[0].stateMachine.entryTransitions = new AnimatorTransition[0];

        controller.parameters = new AnimatorControllerParameter[0];

        controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
        controller.AddParameter("Attack", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Die", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("TakeDamage", AnimatorControllerParameterType.Trigger);

        var rootStateMachine = controller.layers[0].stateMachine;

        var idleState = rootStateMachine.AddState("Idle", new Vector3(300, 0, 0));
        idleState.motion = idle;

        var moveState = rootStateMachine.AddState("Move", new Vector3(300, 100, 0));
        moveState.motion = move;

        var attackState = rootStateMachine.AddState("Attack", new Vector3(300, 200, 0));
        attackState.motion = attack1;

        var deadState = rootStateMachine.AddState("Dead", new Vector3(300, 300, 0));
        deadState.motion = dead;

        if (takeDamage != null)
        {
            var takeDamageState = rootStateMachine.AddState("Take Damage", new Vector3(300, 400, 0));
            takeDamageState.motion = takeDamage;

            var damageTrans = rootStateMachine.AddAnyStateTransition(takeDamageState);
            damageTrans.AddCondition(AnimatorConditionMode.If, 0, "TakeDamage");
            damageTrans.duration = 0.1f;
            damageTrans.hasExitTime = false;

            var damageToIdle = takeDamageState.AddTransition(idleState);
            damageToIdle.hasExitTime = true;
            damageToIdle.exitTime = 0.9f;
            damageToIdle.duration = 0.1f;
        }

        rootStateMachine.defaultState = idleState;

        var idleToMove = idleState.AddTransition(moveState);
        idleToMove.AddCondition(AnimatorConditionMode.Greater, 0.1f, "Speed");
        idleToMove.hasExitTime = false;
        idleToMove.duration = 0.2f;

        var moveToIdle = moveState.AddTransition(idleState);
        moveToIdle.AddCondition(AnimatorConditionMode.Less, 0.1f, "Speed");
        moveToIdle.hasExitTime = false;
        moveToIdle.duration = 0.2f;

        var idleToAttack = idleState.AddTransition(attackState);
        idleToAttack.AddCondition(AnimatorConditionMode.If, 0, "Attack");
        idleToAttack.hasExitTime = false;
        idleToAttack.duration = 0.1f;

        var moveToAttack = moveState.AddTransition(attackState);
        moveToAttack.AddCondition(AnimatorConditionMode.If, 0, "Attack");
        moveToAttack.hasExitTime = false;
        moveToAttack.duration = 0.1f;

        var attackToIdle = attackState.AddTransition(idleState);
        attackToIdle.hasExitTime = true;
        attackToIdle.exitTime = 0.9f;
        attackToIdle.duration = 0.2f;

        var anyToDead = rootStateMachine.AddAnyStateTransition(deadState);
        anyToDead.AddCondition(AnimatorConditionMode.If, 0, "Die");
        anyToDead.duration = 0.2f;
        anyToDead.hasExitTime = false;

        EditorUtility.SetDirty(controller);
        AssetDatabase.SaveAssets();

        Debug.Log("✅ Imp Animator Controller setup complete!");
        Debug.Log("   States: Idle, Move, Attack, Dead, Take Damage");
        Debug.Log("   Parameters: Speed (float), Attack (trigger), Die (trigger), TakeDamage (trigger)");
        
        AssignToImpPrefab(controller);
    }

    private static void AssignToImpPrefab(AnimatorController controller)
    {
        string prefabPath = "Assets/Prefabs/Imp.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        
        if (prefab == null)
        {
            Debug.LogError("Imp prefab not found at: " + prefabPath);
            return;
        }

        string path = AssetDatabase.GetAssetPath(prefab);
        GameObject prefabContents = PrefabUtility.LoadPrefabContents(path);
        
        Animator animator = prefabContents.GetComponentInChildren<Animator>();
        if (animator != null)
        {
            animator.runtimeAnimatorController = controller;
            animator.applyRootMotion = false;
            animator.updateMode = AnimatorUpdateMode.Normal;
            animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
            
            PrefabUtility.SaveAsPrefabAsset(prefabContents, path);
            Debug.Log("✅ Animator Controller assigned to Imp prefab!");
        }
        else
        {
            Debug.LogWarning("⚠️ No Animator component found on Imp prefab!");
        }
        
        PrefabUtility.UnloadPrefabContents(prefabContents);
    }

    [MenuItem("Tools/Check Imp Animator")]
    public static void CheckAnimator()
    {
        Debug.Log("═══════════════════════════════════════════");
        Debug.Log("      IMP ANIMATOR STATUS CHECK");
        Debug.Log("═══════════════════════════════════════════");
        
        string prefabPath = "Assets/Prefabs/Imp.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        
        if (prefab == null)
        {
            Debug.LogError("❌ Imp prefab not found!");
            return;
        }

        Animator animator = prefab.GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogError("❌ No Animator component found!");
            return;
        }

        Debug.Log("✅ Animator component: Found");
        
        if (animator.runtimeAnimatorController != null)
        {
            Debug.Log("✅ Controller: " + AssetDatabase.GetAssetPath(animator.runtimeAnimatorController));
            
            AnimatorController controller = animator.runtimeAnimatorController as AnimatorController;
            if (controller != null)
            {
                Debug.Log("   Parameters:");
                foreach (var param in controller.parameters)
                {
                    Debug.Log("   - " + param.name + " (" + param.type + ")");
                }
                
                Debug.Log("   States:");
                foreach (var state in controller.layers[0].stateMachine.states)
                {
                    Debug.Log("   - " + state.state.name);
                }
            }
        }
        else
        {
            Debug.LogWarning("⚠️ Controller: NULL (not assigned!)");
        }

        Debug.Log("   Apply Root Motion: " + animator.applyRootMotion);
        Debug.Log("   Update Mode: " + animator.updateMode);
        
        Debug.Log("═══════════════════════════════════════════");
    }
}
