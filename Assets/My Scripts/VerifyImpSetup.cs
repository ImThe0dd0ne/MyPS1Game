using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine.AI;

public class VerifyImpSetup : MonoBehaviour
{
    [MenuItem("Tools/✅ Verify Imp Setup")]
    public static void VerifySetup()
    {
        Debug.Log("╔═══════════════════════════════════════════════════════════╗");
        Debug.Log("║              IMP SETUP VERIFICATION REPORT               ║");
        Debug.Log("╚═══════════════════════════════════════════════════════════╝");
        
        bool allGood = true;
        
        allGood &= CheckAnimatorController();
        allGood &= CheckPrefab();
        allGood &= CheckMaterial();
        allGood &= CheckNavMesh();
        
        Debug.Log("\n╔═══════════════════════════════════════════════════════════╗");
        if (allGood)
        {
            Debug.Log("║            ✅✅✅ ALL CHECKS PASSED! ✅✅✅              ║");
            Debug.Log("║                Imp should work perfectly!                ║");
        }
        else
        {
            Debug.Log("║         ⚠️ SOME ISSUES FOUND - See details above         ║");
            Debug.Log("║       Run: Tools → 🔥 COMPLETE IMP FIX - Click Here!    ║");
        }
        Debug.Log("╚═══════════════════════════════════════════════════════════╝");
    }

    private static bool CheckAnimatorController()
    {
        Debug.Log("\n[1] ANIMATOR CONTROLLER CHECK");
        Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        
        string controllerPath = "Assets/Imp/Animations/ImpAnimator.controller";
        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
        
        if (controller == null)
        {
            Debug.LogError("❌ ImpAnimator.controller NOT FOUND!");
            return false;
        }
        
        Debug.Log("✅ Controller exists");
        
        bool hasSpeed = false;
        bool hasAttack = false;
        bool hasDie = false;
        
        foreach (var param in controller.parameters)
        {
            if (param.name == "Speed" && param.type == AnimatorControllerParameterType.Float)
            {
                hasSpeed = true;
                Debug.Log("   ✅ Speed (float)");
            }
            if (param.name == "Attack" && param.type == AnimatorControllerParameterType.Trigger)
            {
                hasAttack = true;
                Debug.Log("   ✅ Attack (trigger)");
            }
            if (param.name == "Die" && param.type == AnimatorControllerParameterType.Trigger)
            {
                hasDie = true;
                Debug.Log("   ✅ Die (trigger)");
            }
        }
        
        if (!hasSpeed)
        {
            Debug.LogError("   ❌ MISSING: Speed parameter!");
            return false;
        }
        if (!hasAttack)
        {
            Debug.LogError("   ❌ MISSING: Attack parameter!");
            return false;
        }
        if (!hasDie)
        {
            Debug.LogError("   ❌ MISSING: Die parameter!");
            return false;
        }
        
        Debug.Log("✅ All required parameters present");
        return true;
    }

    private static bool CheckPrefab()
    {
        Debug.Log("\n[2] PREFAB CHECK");
        Debug.Log("━━━━━━━━━━━━━━━━");
        
        string prefabPath = "Assets/Prefabs/Imp.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        
        if (prefab == null)
        {
            Debug.LogError("❌ Imp prefab NOT FOUND!");
            return false;
        }
        
        Debug.Log("✅ Prefab exists");
        Debug.Log($"   Scale: {prefab.transform.localScale}");
        
        bool allGood = true;
        
        NavMeshAgent agent = prefab.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            Debug.Log($"✅ NavMeshAgent present");
            Debug.Log($"   - Radius: {agent.radius}");
            Debug.Log($"   - Height: {agent.height}");
            Debug.Log($"   - BaseOffset: {agent.baseOffset}");
            
            if (agent.baseOffset < 0.1f)
            {
                Debug.LogWarning("   ⚠️ BaseOffset is low - Imp may sink into ground!");
                Debug.LogWarning("      Recommended: 0.3 or higher");
                allGood = false;
            }
        }
        else
        {
            Debug.LogError("❌ NavMeshAgent MISSING!");
            allGood = false;
        }
        
        CapsuleCollider collider = prefab.GetComponent<CapsuleCollider>();
        if (collider != null)
        {
            Debug.Log($"✅ CapsuleCollider present");
            Debug.Log($"   - Center: {collider.center}");
            Debug.Log($"   - Radius: {collider.radius}");
            Debug.Log($"   - Height: {collider.height}");
        }
        else
        {
            Debug.LogWarning("⚠️ CapsuleCollider MISSING - Imp may not collide correctly!");
            allGood = false;
        }
        
        ImpEnemy impScript = prefab.GetComponent<ImpEnemy>();
        if (impScript != null)
        {
            Debug.Log("✅ ImpEnemy script attached");
        }
        else
        {
            Debug.LogError("❌ ImpEnemy script MISSING!");
            allGood = false;
        }
        
        Animator animator = prefab.GetComponentInChildren<Animator>();
        if (animator != null)
        {
            if (animator.runtimeAnimatorController != null)
            {
                Debug.Log($"✅ Animator Controller assigned: {animator.runtimeAnimatorController.name}");
                Debug.Log($"   - Apply Root Motion: {animator.applyRootMotion} (should be false)");
                
                if (animator.applyRootMotion)
                {
                    Debug.LogWarning("   ⚠️ Apply Root Motion should be FALSE!");
                    allGood = false;
                }
            }
            else
            {
                Debug.LogError("❌ Animator Controller NOT ASSIGNED!");
                allGood = false;
            }
        }
        else
        {
            Debug.LogError("❌ Animator MISSING!");
            allGood = false;
        }
        
        Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>();
        int materialsFound = 0;
        int nullMaterials = 0;
        
        foreach (Renderer renderer in renderers)
        {
            foreach (Material mat in renderer.sharedMaterials)
            {
                if (mat != null)
                {
                    materialsFound++;
                }
                else
                {
                    nullMaterials++;
                }
            }
        }
        
        if (nullMaterials > 0)
        {
            Debug.LogError($"❌ Found {nullMaterials} NULL material(s)!");
            allGood = false;
        }
        else if (materialsFound > 0)
        {
            Debug.Log($"✅ All materials assigned ({materialsFound} material(s))");
        }
        
        return allGood;
    }

    private static bool CheckMaterial()
    {
        Debug.Log("\n[3] MATERIAL CHECK");
        Debug.Log("━━━━━━━━━━━━━━━━━━");
        
        string materialPath = "Assets/Imp/Materials/Imp Red Material.mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        
        if (mat == null)
        {
            Debug.LogError("❌ Imp Red Material NOT FOUND!");
            return false;
        }
        
        Debug.Log("✅ Material exists");
        Debug.Log($"   Shader: {mat.shader.name}");
        
        bool allGood = true;
        
        if (!mat.shader.name.Contains("Universal Render Pipeline"))
        {
            Debug.LogWarning("⚠️ Shader is not URP - should use Universal Render Pipeline/Lit");
            allGood = false;
        }
        
        Texture baseMap = mat.GetTexture("_BaseMap");
        if (baseMap != null)
        {
            Debug.Log($"✅ BaseMap texture assigned: {baseMap.name}");
        }
        else
        {
            Debug.LogWarning("⚠️ BaseMap NOT ASSIGNED - Imp will appear grey/white!");
            Debug.LogWarning("   This is likely why the Imp has no texture!");
            allGood = false;
        }
        
        return allGood;
    }

    private static bool CheckNavMesh()
    {
        Debug.Log("\n[4] NAVMESH CHECK");
        Debug.Log("━━━━━━━━━━━━━━━━━");
        
        var triangulation = UnityEngine.AI.NavMesh.CalculateTriangulation();
        if (triangulation.vertices.Length > 0)
        {
            Debug.Log($"✅ NavMesh is BAKED ({triangulation.vertices.Length} vertices)");
            Debug.Log("   Imps can move!");
            return true;
        }
        else
        {
            Debug.LogError("❌ NavMesh NOT BAKED!");
            Debug.LogError("   Imps CANNOT move without NavMesh!");
            Debug.LogError("   → Window → AI → Navigation → Bake");
            return false;
        }
    }
}
