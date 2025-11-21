using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

public class FIX_IMP_LAYERMASK
{
    [MenuItem("Tools/⚡ FIX IMP LAYERMASK")]
    public static void FixLayerMask()
    {
        Debug.Log("⚡ FIXING IMP LAYERMASK...\n");
        
        string impPrefabPath = "Assets/Prefabs/Imp.prefab";
        GameObject impPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(impPrefabPath);
        
        if (impPrefab == null)
        {
            Debug.LogError("❌ Imp prefab not found!");
            return;
        }
        
        string path = AssetDatabase.GetAssetPath(impPrefab);
        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(path);
        
        ImpAI impAI = prefabRoot.GetComponent<ImpAI>();
        if (impAI == null)
        {
            Debug.LogError("❌ ImpAI component not found!");
            PrefabUtility.UnloadPrefabContents(prefabRoot);
            return;
        }
        
        SerializedObject serializedImp = new SerializedObject(impAI);
        
        int groundLayerIndex = LayerMask.NameToLayer("WhatIsGround");
        int playerLayerIndex = LayerMask.NameToLayer("WhatIsPlayer");
        
        if (groundLayerIndex == -1)
        {
            Debug.LogError("❌ Layer 'WhatIsGround' not found!");
        }
        else
        {
            serializedImp.FindProperty("whatIsGround").intValue = 1 << groundLayerIndex;
            Debug.Log($"✅ whatIsGround = layer {groundLayerIndex} (WhatIsGround)");
        }
        
        if (playerLayerIndex == -1)
        {
            Debug.LogError("❌ Layer 'WhatIsPlayer' not found!");
        }
        else
        {
            serializedImp.FindProperty("whatIsPlayer").intValue = 1 << playerLayerIndex;
            Debug.Log($"✅ whatIsPlayer = layer {playerLayerIndex} (WhatIsPlayer)");
        }
        
        impAI.sightRange = 20f;
        impAI.attackRange = 12f;
        
        Debug.Log($"✅ sightRange = {impAI.sightRange}");
        Debug.Log($"✅ attackRange = {impAI.attackRange}");
        
        serializedImp.ApplyModifiedProperties();
        
        PrefabUtility.SaveAsPrefabAsset(prefabRoot, path);
        PrefabUtility.UnloadPrefabContents(prefabRoot);
        
        Debug.Log("\n⚡ IMP LAYERMASK FIXED!");
        Debug.Log("🎮 Press Play and test!\n");
    }
}
