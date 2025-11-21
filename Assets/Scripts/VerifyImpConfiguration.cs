using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using UnityEditor.Animations;

public class VerifyImpConfiguration : MonoBehaviour
{
    [MenuItem("Tools/📋 Verify Imp Configuration")]
    public static void VerifyImp()
    {
        Debug.Log("╔═══════════════════════════════════════════════════════════╗");
        Debug.Log("║           📋 IMP CONFIGURATION VERIFICATION              ║");
        Debug.Log("╚═══════════════════════════════════════════════════════════╝\n");
        
        string impPrefabPath = "Assets/Prefabs/Imp.prefab";
        GameObject impPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(impPrefabPath);
        
        if (impPrefab == null)
        {
            Debug.LogError("❌ Imp prefab not found!");
            return;
        }
        
        int passedChecks = 0;
        int totalChecks = 0;
        
        Debug.Log("🔍 CHECKING PREFAB COMPONENTS:\n");
        
        totalChecks++;
        NavMeshAgent agent = impPrefab.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            Debug.Log($"✅ NavMeshAgent found");
            Debug.Log($"   - speed: {agent.speed}");
            Debug.Log($"   - radius: {agent.radius}");
            Debug.Log($"   - height: {agent.height}");
            Debug.Log($"   - baseOffset: {agent.baseOffset}");
            Debug.Log($"   - updatePosition: {agent.updatePosition}");
            Debug.Log($"   - updateRotation: {agent.updateRotation}");
            passedChecks++;
        }
        else
        {
            Debug.LogError("❌ NavMeshAgent MISSING!");
        }
        
        totalChecks++;
        Animator animator = impPrefab.GetComponent<Animator>();
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            Debug.Log($"\n✅ Animator found with controller");
            AnimatorController controller = animator.runtimeAnimatorController as AnimatorController;
            if (controller != null)
            {
                Debug.Log($"   - Parameters: {controller.parameters.Length}");
                foreach (var param in controller.parameters)
                {
                    Debug.Log($"     • {param.name} ({param.type})");
                }
            }
            passedChecks++;
        }
        else
        {
            Debug.LogError("❌ Animator or Controller MISSING!");
        }
        
        totalChecks++;
        ImpEnemy impScript = impPrefab.GetComponent<ImpEnemy>();
        if (impScript != null)
        {
            Debug.Log($"\n✅ ImpEnemy script found");
            Debug.Log($"   - maxHealth: {impScript.maxHealth}");
            Debug.Log($"   - attackDamage: {impScript.attackDamage}");
            Debug.Log($"   - attackRange: {impScript.attackRange}");
            Debug.Log($"   - fireballPrefab: {(impScript.fireballPrefab != null ? impScript.fireballPrefab.name : "NULL")}");
            passedChecks++;
        }
        else
        {
            Debug.LogError("❌ ImpEnemy script MISSING!");
        }
        
        totalChecks++;
        CapsuleCollider collider = impPrefab.GetComponent<CapsuleCollider>();
        if (collider != null)
        {
            Debug.Log($"\n✅ CapsuleCollider found");
            Debug.Log($"   - center: {collider.center}");
            Debug.Log($"   - radius: {collider.radius}");
            Debug.Log($"   - height: {collider.height}");
            Debug.Log($"   - isTrigger: {collider.isTrigger}");
            passedChecks++;
        }
        else
        {
            Debug.LogError("❌ CapsuleCollider MISSING!");
        }
        
        totalChecks++;
        ImpHeightFixer heightFixer = impPrefab.GetComponent<ImpHeightFixer>();
        if (heightFixer != null)
        {
            Debug.Log($"\n✅ ImpHeightFixer found (will prevent ground clipping)");
            passedChecks++;
        }
        else
        {
            Debug.LogWarning("⚠️ ImpHeightFixer not found (Imp may clip through ground)");
        }
        
        totalChecks++;
        if (impPrefab.tag == "Enemy")
        {
            Debug.Log($"\n✅ Tag set to 'Enemy'");
            passedChecks++;
        }
        else
        {
            Debug.LogError($"❌ Tag is '{impPrefab.tag}' instead of 'Enemy'");
        }
        
        totalChecks++;
        if (impPrefab.layer == LayerMask.NameToLayer("Enemy"))
        {
            Debug.Log($"✅ Layer set to 'Enemy' ({impPrefab.layer})");
            passedChecks++;
        }
        else
        {
            Debug.LogError($"❌ Layer is {impPrefab.layer} instead of Enemy");
        }
        
        Debug.Log($"\n🔍 CHECKING FIREBALL PREFAB:\n");
        
        totalChecks++;
        GameObject fireballPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Goblin_Character/Prefab/Fireball.prefab");
        if (fireballPrefab != null)
        {
            Debug.Log("✅ Fireball prefab found");
            Transform sphere = fireballPrefab.transform.Find("Sphere");
            if (sphere != null)
            {
                MeshRenderer renderer = sphere.GetComponent<MeshRenderer>();
                if (renderer != null && renderer.sharedMaterial != null)
                {
                    Debug.Log($"   - Material assigned: {renderer.sharedMaterial.name}");
                    passedChecks++;
                }
                else
                {
                    Debug.LogWarning("⚠️ Fireball Sphere has no material (will be invisible)");
                }
            }
        }
        else
        {
            Debug.LogError("❌ Fireball prefab not found!");
        }
        
        Debug.Log($"\n🔍 CHECKING NAVMESH:\n");
        
        totalChecks++;
        var triangulation = NavMesh.CalculateTriangulation();
        if (triangulation.vertices.Length > 0)
        {
            Debug.Log($"✅ NavMesh is BAKED ({triangulation.vertices.Length} vertices)");
            passedChecks++;
        }
        else
        {
            Debug.LogError("❌ NavMesh is NOT BAKED! Go to Window → AI → Navigation → Bake");
        }
        
        Debug.Log("\n╔═══════════════════════════════════════════════════════════╗");
        Debug.Log($"║  VERIFICATION RESULT: {passedChecks}/{totalChecks} CHECKS PASSED");
        Debug.Log("╚═══════════════════════════════════════════════════════════╝\n");
        
        if (passedChecks == totalChecks)
        {
            Debug.Log("🎉 ALL CHECKS PASSED! Imp should work correctly!");
        }
        else if (passedChecks >= totalChecks - 2)
        {
            Debug.LogWarning("⚠️ Most checks passed, but some issues remain");
        }
        else
        {
            Debug.LogError("❌ Multiple issues found! Run the COMPLETE IMP FIX again");
        }
    }
}
