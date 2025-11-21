using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

public class FinalImpFix : MonoBehaviour
{
    [MenuItem("Tools/🔥 FINAL IMP FIX - Run This Now!")]
    public static void FixImpCompletely()
    {
        Debug.Log("═══════════════════════════════════════════");
        Debug.Log("       RUNNING COMPLETE IMP FIX");
        Debug.Log("═══════════════════════════════════════════");
        
        FixImpPrefab();
        FixImpMaterial();
        FixFireballMaterial();
        
        Debug.Log("═══════════════════════════════════════════");
        Debug.Log("✅ IMP COMPLETELY FIXED!");
        Debug.Log("═══════════════════════════════════════════");
        Debug.Log("Next steps:");
        Debug.Log("1. Make sure NavMesh is baked (Window → AI → Navigation → Bake)");
        Debug.Log("2. Test in Play mode");
        Debug.Log("═══════════════════════════════════════════");
    }

    private static void FixImpPrefab()
    {
        string prefabPath = "Assets/Prefabs/Imp.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        
        if (prefab == null)
        {
            Debug.LogError("❌ Imp prefab not found at: " + prefabPath);
            return;
        }

        string path = AssetDatabase.GetAssetPath(prefab);
        GameObject prefabContents = PrefabUtility.LoadPrefabContents(path);
        
        prefabContents.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        Debug.Log("✅ Scale set to 0.4");

        UnityEngine.AI.NavMeshAgent agent = prefabContents.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
        {
            agent.radius = 0.35f;
            agent.height = 1.2f;
            agent.baseOffset = 0.1f;
            Debug.Log("✅ NavMeshAgent configured (radius: 0.35, height: 1.2, baseOffset: 0.1)");
        }

        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>("Assets/Imp/Animations/ImpAnimator.controller");
        Animator animator = prefabContents.GetComponentInChildren<Animator>();
        if (animator != null && controller != null)
        {
            animator.runtimeAnimatorController = controller;
            animator.applyRootMotion = false;
            animator.updateMode = AnimatorUpdateMode.Normal;
            Debug.Log("✅ Animator Controller assigned and configured");
        }

        Renderer[] renderers = prefabContents.GetComponentsInChildren<Renderer>();
        Material impMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Imp/Materials/Imp Red Material.mat");
        
        if (impMaterial != null)
        {
            foreach (Renderer renderer in renderers)
            {
                if (renderer.name.Contains("Imp") || renderer.name.Contains("Body") || renderer.gameObject == prefabContents)
                {
                    Material[] mats = renderer.sharedMaterials;
                    if (mats.Length > 0)
                    {
                        mats[0] = impMaterial;
                        renderer.sharedMaterials = mats;
                        Debug.Log($"✅ Assigned material to {renderer.name}");
                    }
                }
            }
        }

        PrefabUtility.SaveAsPrefabAsset(prefabContents, path);
        PrefabUtility.UnloadPrefabContents(prefabContents);
        
        Debug.Log("✅ Imp prefab saved");
    }

    private static void FixImpMaterial()
    {
        string materialPath = "Assets/Imp/Materials/Imp Red Material.mat";
        Material impMaterial = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        
        if (impMaterial == null)
        {
            Debug.LogError("❌ Imp material not found at: " + materialPath);
            return;
        }

        Shader urpLitShader = Shader.Find("Universal Render Pipeline/Lit");
        if (urpLitShader == null)
        {
            Debug.LogError("❌ URP Lit shader not found!");
            return;
        }

        impMaterial.shader = urpLitShader;

        Texture2D colorTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Imp/Textures/Imp.Color.Complete.png");
        if (colorTexture == null)
        {
            colorTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Imp/Textures/ImpColorBrownComplet.png");
        }

        if (colorTexture != null)
        {
            impMaterial.SetTexture("_BaseMap", colorTexture);
            impMaterial.SetColor("_BaseColor", Color.white);
            Debug.Log("✅ Imp material - Assigned texture: " + colorTexture.name);
        }
        else
        {
            impMaterial.SetColor("_BaseColor", new Color(0.8f, 0.2f, 0.2f, 1f));
            Debug.Log("⚠️ Texture not found, set red color instead");
        }

        Texture2D normalMap = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Imp/Textures/Imp.normal.png");
        if (normalMap != null)
        {
            impMaterial.SetTexture("_BumpMap", normalMap);
            impMaterial.EnableKeyword("_NORMALMAP");
        }

        EditorUtility.SetDirty(impMaterial);
        AssetDatabase.SaveAssets();
        
        Debug.Log("✅ Imp material fixed and saved");
    }

    private static void FixFireballMaterial()
    {
        string materialPath = "Assets/Materials/Fireball_Material.mat";
        Material fireballMat = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        
        if (fireballMat == null)
        {
            Shader urpLitShader = Shader.Find("Universal Render Pipeline/Lit");
            if (urpLitShader == null)
            {
                Debug.LogError("❌ URP Lit shader not found for fireball!");
                return;
            }

            fireballMat = new Material(urpLitShader);
            fireballMat.SetColor("_BaseColor", new Color(1f, 0.4f, 0f, 1f));
            fireballMat.SetColor("_EmissionColor", new Color(1f, 0.5f, 0f) * 2f);
            fireballMat.EnableKeyword("_EMISSION");
            fireballMat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;

            AssetDatabase.CreateAsset(fireballMat, materialPath);
            Debug.Log("✅ Fireball material created");
        }

        string fireballPrefabPath = "Assets/Goblin_Character/Prefab/Fireball.prefab";
        GameObject fireballPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(fireballPrefabPath);
        
        if (fireballPrefab == null)
        {
            Debug.LogWarning("⚠️ Fireball prefab not found at: " + fireballPrefabPath);
            return;
        }

        string path = AssetDatabase.GetAssetPath(fireballPrefab);
        GameObject prefabContents = PrefabUtility.LoadPrefabContents(path);
        
        Renderer[] renderers = prefabContents.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material[] mats = renderer.sharedMaterials;
            if (mats.Length > 0)
            {
                mats[0] = fireballMat;
                renderer.sharedMaterials = mats;
            }
        }
        
        PrefabUtility.SaveAsPrefabAsset(prefabContents, path);
        PrefabUtility.UnloadPrefabContents(prefabContents);
        
        Debug.Log("✅ Fireball material assigned to prefab");
    }
}
