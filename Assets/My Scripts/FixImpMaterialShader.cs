using UnityEngine;
using UnityEditor;

public class FixImpMaterialShader : MonoBehaviour
{
    [MenuItem("Tools/Fix Imp Material (URP)")]
    public static void FixMaterial()
    {
        string materialPath = "Assets/Imp/Materials/Imp Red Material.mat";
        Material impMaterial = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        
        if (impMaterial == null)
        {
            Debug.LogError("Could not find Imp Red Material at: " + materialPath);
            return;
        }

        Texture albedoTex = null;
        Texture normalTex = impMaterial.GetTexture("_BumpMap");
        Texture roughnessTex = impMaterial.GetTexture("_MetallicGlossMap");
        Color baseColor = impMaterial.HasProperty("_Color") ? impMaterial.GetColor("_Color") : Color.white;

        Shader urpShader = Shader.Find("Universal Render Pipeline/Lit");
        if (urpShader == null)
        {
            Debug.LogError("URP Lit shader not found! Make sure URP is properly installed.");
            return;
        }

        impMaterial.shader = urpShader;

        if (albedoTex != null)
            impMaterial.SetTexture("_BaseMap", albedoTex);
        
        impMaterial.SetColor("_BaseColor", baseColor);

        if (normalTex != null)
        {
            impMaterial.SetTexture("_BumpMap", normalTex);
            impMaterial.EnableKeyword("_NORMALMAP");
        }

        if (roughnessTex != null)
        {
            impMaterial.SetTexture("_MetallicGlossMap", roughnessTex);
        }

        impMaterial.SetFloat("_Smoothness", 0.5f);
        impMaterial.SetFloat("_Metallic", 0f);

        EditorUtility.SetDirty(impMaterial);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("✅ Imp Material fixed! It should no longer be pink.");
        Selection.activeObject = impMaterial;
    }
}
