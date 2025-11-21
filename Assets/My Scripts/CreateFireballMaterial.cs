using UnityEngine;
using UnityEditor;

public class CreateFireballMaterial : MonoBehaviour
{
    [MenuItem("Tools/Create Fireball Material (Auto)")]
    public static void CreateMaterial()
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
            return;
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
        AssetDatabase.Refresh();

        Debug.Log("✅ Fireball Material created at: " + materialPath);
        Debug.Log("→ Now assign it to: Fireball prefab → Sphere → MeshRenderer");
        
        Selection.activeObject = fireballMat;
        EditorGUIUtility.PingObject(fireballMat);
    }

    [MenuItem("Tools/Auto-Assign Fireball Material")]
    public static void AssignToFireball()
    {
        string materialPath = "Assets/Materials/Fireball_Material.mat";
        string fireballPrefabPath = "Assets/Goblin_Character/Prefab/Fireball.prefab";

        Material fireballMat = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        if (fireballMat == null)
        {
            Debug.LogError("Fireball Material not found! Create it first using 'Tools → Create Fireball Material'");
            return;
        }

        GameObject fireballPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(fireballPrefabPath);
        if (fireballPrefab == null)
        {
            Debug.LogError("Fireball prefab not found at: " + fireballPrefabPath);
            return;
        }

        Transform sphereTransform = fireballPrefab.transform.Find("Sphere");
        if (sphereTransform == null)
        {
            Debug.LogError("Sphere child not found in Fireball prefab!");
            return;
        }

        MeshRenderer sphereRenderer = sphereTransform.GetComponent<MeshRenderer>();
        if (sphereRenderer == null)
        {
            Debug.LogError("MeshRenderer not found on Sphere!");
            return;
        }

        string prefabPath = AssetDatabase.GetAssetPath(fireballPrefab);
        GameObject prefabContents = PrefabUtility.LoadPrefabContents(prefabPath);
        
        Transform sphereInPrefab = prefabContents.transform.Find("Sphere");
        if (sphereInPrefab != null)
        {
            MeshRenderer renderer = sphereInPrefab.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = fireballMat;
                PrefabUtility.SaveAsPrefabAsset(prefabContents, prefabPath);
                Debug.Log("✅ Fireball Material assigned to Sphere!");
                Debug.Log("✅ Fireball is ready to use!");
            }
        }

        PrefabUtility.UnloadPrefabContents(prefabContents);
        
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<GameObject>(fireballPrefabPath);
    }
}
