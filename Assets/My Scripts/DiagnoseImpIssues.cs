using UnityEngine;
using UnityEditor;

public class DiagnoseImpIssues : MonoBehaviour
{
    [MenuItem("Tools/📋 Diagnose Imp Issues")]
    public static void RunDiagnostics()
    {
        Debug.Log("═══════════════════════════════════════════════════════════");
        Debug.Log("                   IMP DIAGNOSTIC REPORT");
        Debug.Log("═══════════════════════════════════════════════════════════");
        
        CheckPrefab();
        CheckMaterial();
        CheckFireball();
        CheckNavMesh();
        
        Debug.Log("═══════════════════════════════════════════════════════════");
        Debug.Log("                   DIAGNOSTIC COMPLETE");
        Debug.Log("═══════════════════════════════════════════════════════════");
    }

    private static void CheckPrefab()
    {
        Debug.Log("\n--- IMP PREFAB CHECK ---");
        
        string prefabPath = "Assets/Prefabs/Imp.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        
        if (prefab == null)
        {
            Debug.LogError("❌ Imp prefab NOT FOUND at: " + prefabPath);
            return;
        }
        
        Debug.Log("✅ Imp prefab exists");
        Debug.Log($"   Scale: {prefab.transform.localScale}");
        
        UnityEngine.AI.NavMeshAgent agent = prefab.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
        {
            Debug.Log("✅ NavMeshAgent present");
            Debug.Log($"   - Radius: {agent.radius}");
            Debug.Log($"   - Height: {agent.height}");
            Debug.Log($"   - BaseOffset: {agent.baseOffset}");
            Debug.Log($"   - Speed: {agent.speed}");
        }
        else
        {
            Debug.LogError("❌ NavMeshAgent MISSING!");
        }
        
        ImpEnemy impScript = prefab.GetComponent<ImpEnemy>();
        if (impScript != null)
        {
            Debug.Log("✅ ImpEnemy script attached");
        }
        else
        {
            Debug.LogError("❌ ImpEnemy script MISSING!");
        }
        
        Animator animator = prefab.GetComponentInChildren<Animator>();
        if (animator != null)
        {
            Debug.Log("✅ Animator present");
            if (animator.runtimeAnimatorController != null)
            {
                Debug.Log($"   - Controller: {animator.runtimeAnimatorController.name}");
            }
            else
            {
                Debug.LogWarning("⚠️ Animator Controller NOT ASSIGNED!");
            }
            Debug.Log($"   - Apply Root Motion: {animator.applyRootMotion} (should be FALSE)");
        }
        else
        {
            Debug.LogWarning("⚠️ Animator MISSING!");
        }
        
        Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>();
        Debug.Log($"   Renderers found: {renderers.Length}");
        foreach (Renderer renderer in renderers)
        {
            Debug.Log($"   - {renderer.name}:");
            if (renderer.sharedMaterials.Length > 0)
            {
                foreach (Material mat in renderer.sharedMaterials)
                {
                    if (mat != null)
                    {
                        Debug.Log($"     Material: {mat.name}");
                    }
                    else
                    {
                        Debug.LogError($"     ❌ NULL MATERIAL on {renderer.name}!");
                    }
                }
            }
            else
            {
                Debug.LogWarning($"     ⚠️ No materials on {renderer.name}");
            }
        }
    }

    private static void CheckMaterial()
    {
        Debug.Log("\n--- IMP MATERIAL CHECK ---");
        
        string materialPath = "Assets/Imp/Materials/Imp Red Material.mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        
        if (mat == null)
        {
            Debug.LogError("❌ Imp Red Material NOT FOUND!");
            return;
        }
        
        Debug.Log("✅ Imp Red Material exists");
        Debug.Log($"   Shader: {mat.shader.name}");
        
        Texture baseMap = mat.GetTexture("_BaseMap");
        if (baseMap != null)
        {
            Debug.Log($"✅ BaseMap assigned: {baseMap.name}");
        }
        else
        {
            Debug.LogWarning("⚠️ BaseMap NOT ASSIGNED (this causes grey appearance!)");
            Color baseColor = mat.GetColor("_BaseColor");
            Debug.Log($"   BaseColor: {baseColor}");
        }
        
        Texture normalMap = mat.GetTexture("_BumpMap");
        if (normalMap != null)
        {
            Debug.Log($"   Normal Map: {normalMap.name}");
        }
    }

    private static void CheckFireball()
    {
        Debug.Log("\n--- FIREBALL CHECK ---");
        
        string fireballPrefabPath = "Assets/Goblin_Character/Prefab/Fireball.prefab";
        GameObject fireball = AssetDatabase.LoadAssetAtPath<GameObject>(fireballPrefabPath);
        
        if (fireball == null)
        {
            Debug.LogError("❌ Fireball prefab NOT FOUND!");
            return;
        }
        
        Debug.Log("✅ Fireball prefab exists");
        
        Renderer[] renderers = fireball.GetComponentsInChildren<Renderer>();
        bool hasMaterial = false;
        foreach (Renderer renderer in renderers)
        {
            if (renderer.sharedMaterials.Length > 0 && renderer.sharedMaterials[0] != null)
            {
                Debug.Log($"✅ Fireball has material: {renderer.sharedMaterials[0].name}");
                hasMaterial = true;
            }
        }
        
        if (!hasMaterial)
        {
            Debug.LogWarning("⚠️ Fireball has NO MATERIAL (will be invisible!)");
        }
    }

    private static void CheckNavMesh()
    {
        Debug.Log("\n--- NAVMESH CHECK ---");
        
        if (UnityEngine.AI.NavMesh.GetSettingsCount() > 0)
        {
            Debug.Log("✅ NavMesh settings exist");
            
            var triangulation = UnityEngine.AI.NavMesh.CalculateTriangulation();
            if (triangulation.vertices.Length > 0)
            {
                Debug.Log($"✅ NavMesh is BAKED ({triangulation.vertices.Length} vertices)");
            }
            else
            {
                Debug.LogWarning("⚠️ NavMesh NOT BAKED! Imps cannot move without NavMesh!");
                Debug.LogWarning("   → Go to: Window → AI → Navigation → Bake");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ No NavMesh settings found");
        }
    }
}
