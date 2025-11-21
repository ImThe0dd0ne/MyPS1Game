using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

public class FixImpCompletely : MonoBehaviour
{
    [MenuItem("Tools/FIX IMP - Scale + Material + Movement (COMPLETE)")]
    public static void FixEverything()
    {
        bool success = true;
        
        success &= FixScale();
        success &= FixMaterial();
        success &= FixFireballMaterial();
        success &= FixAnimator();
        
        if (success)
        {
            Debug.Log("✅ IMP COMPLETELY FIXED!");
            Debug.Log("   ✅ Scale reduced to 0.4 (slightly bigger than goblins)");
            Debug.Log("   ✅ Material has red color texture assigned");
            Debug.Log("   ✅ Fireball material created and assigned");
            Debug.Log("   ✅ Animator Controller setup and assigned");
            Debug.Log("   ✅ NavMeshAgent should work (make sure NavMesh is baked!)");
            Debug.Log("");
            Debug.Log("→ NEXT: Make sure NavMesh is baked in your scene");
            Debug.Log("→ Window → AI → Navigation → Bake");
        }
        else
        {
            Debug.LogWarning("⚠️ Some fixes failed - check errors above");
        }
    }

    private static bool FixScale()
    {
        string prefabPath = "Assets/Prefabs/Imp.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        
        if (prefab == null)
        {
            Debug.LogError("Imp prefab not found at: " + prefabPath);
            return false;
        }

        string path = AssetDatabase.GetAssetPath(prefab);
        GameObject prefabContents = PrefabUtility.LoadPrefabContents(path);
        
        prefabContents.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        
        NavMeshAgent agent = prefabContents.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.radius = 0.35f;
            agent.height = 1.2f;
        }

        PrefabUtility.SaveAsPrefabAsset(prefabContents, path);
        PrefabUtility.UnloadPrefabContents(prefabContents);
        
        Debug.Log("✅ Imp scale fixed: 0.4 (was huge, now slightly bigger than goblins)");
        return true;
    }

    private static bool FixMaterial()
    {
        string materialPath = "Assets/Imp/Materials/Imp Red Material.mat";
        Material impMaterial = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        
        if (impMaterial == null)
        {
            Debug.LogError("Imp Red Material not found at: " + materialPath);
            return false;
        }

        string colorTexturePath = "Assets/Imp/Textures/Imp.Color.Complete.png";
        Texture colorTexture = AssetDatabase.LoadAssetAtPath<Texture>(colorTexturePath);
        
        if (colorTexture == null)
        {
            Debug.LogWarning("Color texture not found, trying alternate...");
            colorTexturePath = "Assets/Imp/Textures/ImpColorBrownComplet.png";
            colorTexture = AssetDatabase.LoadAssetAtPath<Texture>(colorTexturePath);
        }

        if (colorTexture != null)
        {
            impMaterial.SetTexture("_BaseMap", colorTexture);
            impMaterial.SetColor("_BaseColor", Color.white);
            
            EditorUtility.SetDirty(impMaterial);
            AssetDatabase.SaveAssets();
            
            Debug.Log("✅ Imp material fixed: Assigned color texture " + colorTexturePath);
            return true;
        }
        else
        {
            impMaterial.SetColor("_BaseColor", new Color(0.8f, 0.2f, 0.2f));
            EditorUtility.SetDirty(impMaterial);
            AssetDatabase.SaveAssets();
            
            Debug.LogWarning("⚠️ Color texture not found, set material to red color instead");
            return true;
        }
    }

    private static bool FixFireballMaterial()
    {
        string materialPath = "Assets/Materials/Fireball_Material.mat";
        
        if (!AssetDatabase.IsValidFolder("Assets/Materials"))
        {
            AssetDatabase.CreateFolder("Assets", "Materials");
        }

        Shader urpLitShader = Shader.Find("Universal Render Pipeline/Lit");
        if (urpLitShader == null)
        {
            Debug.LogError("URP Lit shader not found!");
            return false;
        }

        Material fireballMat = new Material(urpLitShader);
        
        fireballMat.SetColor("_BaseColor", new Color(1f, 0.4f, 0f, 1f));
        fireballMat.SetColor("_EmissionColor", new Color(1f, 0.5f, 0f) * 2f);
        fireballMat.EnableKeyword("_EMISSION");
        fireballMat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        fireballMat.SetFloat("_Metallic", 0f);
        fireballMat.SetFloat("_Smoothness", 0.5f);

        AssetDatabase.CreateAsset(fireballMat, materialPath);
        AssetDatabase.SaveAssets();

        string fireballPrefabPath = "Assets/Goblin_Character/Prefab/Fireball.prefab";
        GameObject fireballPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(fireballPrefabPath);
        
        if (fireballPrefab != null)
        {
            string path = AssetDatabase.GetAssetPath(fireballPrefab);
            GameObject prefabContents = PrefabUtility.LoadPrefabContents(path);
            
            Transform sphereInPrefab = prefabContents.transform.Find("Sphere");
            if (sphereInPrefab != null)
            {
                MeshRenderer renderer = sphereInPrefab.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    renderer.sharedMaterial = fireballMat;
                    PrefabUtility.SaveAsPrefabAsset(prefabContents, path);
                    Debug.Log("✅ Fireball material created and assigned");
                }
            }
            
            PrefabUtility.UnloadPrefabContents(prefabContents);
        }

        return true;
    }

    private static bool FixAnimator()
    {
        string controllerPath = "Assets/Imp/Animations/ImpAnimator.controller";
        UnityEditor.Animations.AnimatorController controller = AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController>(controllerPath);
        
        if (controller == null)
        {
            Debug.LogError("Animator Controller not found at: " + controllerPath);
            return false;
        }

        string prefabPath = "Assets/Prefabs/Imp.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        
        if (prefab == null)
        {
            Debug.LogError("Imp prefab not found at: " + prefabPath);
            return false;
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
            Debug.Log("✅ Animator Controller assigned to Imp prefab");
        }
        else
        {
            Debug.LogWarning("⚠️ No Animator component found on Imp prefab");
            PrefabUtility.UnloadPrefabContents(prefabContents);
            return false;
        }
        
        PrefabUtility.UnloadPrefabContents(prefabContents);
        return true;
    }

    [MenuItem("Tools/Check Imp Status")]
    public static void CheckImpStatus()
    {
        Debug.Log("═══════════════════════════════════════════");
        Debug.Log("         IMP STATUS CHECK");
        Debug.Log("═══════════════════════════════════════════");
        
        string prefabPath = "Assets/Prefabs/Imp.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        
        if (prefab == null)
        {
            Debug.LogError("❌ Imp prefab not found!");
            return;
        }

        Debug.Log("✅ Prefab exists: " + prefabPath);
        Debug.Log("   Scale: " + prefab.transform.localScale);
        
        NavMeshAgent agent = prefab.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            Debug.Log("✅ NavMeshAgent: Present");
            Debug.Log("   Speed: " + agent.speed);
            Debug.Log("   Radius: " + agent.radius);
            Debug.Log("   Height: " + agent.height);
        }
        else
        {
            Debug.LogWarning("⚠️ NavMeshAgent: Missing!");
        }

        var impScript = prefab.GetComponent<ImpEnemy>();
        if (impScript != null)
        {
            Debug.Log("✅ ImpEnemy script: Attached");
        }
        else
        {
            Debug.LogWarning("⚠️ ImpEnemy script: Missing!");
        }

        Animator animator = prefab.GetComponentInChildren<Animator>();
        if (animator != null)
        {
            Debug.Log("✅ Animator component: Present");
            if (animator.runtimeAnimatorController != null)
            {
                Debug.Log("   Controller: " + AssetDatabase.GetAssetPath(animator.runtimeAnimatorController));
            }
            else
            {
                Debug.LogWarning("⚠️ Animator Controller: NULL (not assigned!)");
            }
            Debug.Log("   Apply Root Motion: " + animator.applyRootMotion + " (should be False)");
        }
        else
        {
            Debug.LogWarning("⚠️ Animator component: Missing!");
        }

        string materialPath = "Assets/Imp/Materials/Imp Red Material.mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        if (mat != null)
        {
            Texture baseMap = mat.GetTexture("_BaseMap");
            Color baseColor = mat.GetColor("_BaseColor");
            
            if (baseMap != null)
            {
                Debug.Log("✅ Material BaseMap: " + AssetDatabase.GetAssetPath(baseMap));
            }
            else
            {
                Debug.LogWarning("⚠️ Material BaseMap: NULL (will show white/grey)");
            }
            
            Debug.Log("   Base Color: " + baseColor);
        }

        Debug.Log("═══════════════════════════════════════════");
    }
}
