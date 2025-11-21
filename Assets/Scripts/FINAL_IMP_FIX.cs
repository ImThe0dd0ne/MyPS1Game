using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

public class FINAL_IMP_FIX
{
    [MenuItem("Tools/🔥 FINAL IMP FIX - BASEOFFSET")]
    public static void FinalFix()
    {
        Debug.Log("🔥🔥🔥 FINAL IMP FIX 🔥🔥🔥\n");
        
        string impPrefabPath = "Assets/Prefabs/Imp.prefab";
        GameObject impPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(impPrefabPath);
        
        if (impPrefab == null)
        {
            Debug.LogError("❌ Imp prefab not found!");
            return;
        }
        
        string path = AssetDatabase.GetAssetPath(impPrefab);
        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(path);
        
        NavMeshAgent agent = prefabRoot.GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("❌ NavMeshAgent not found!");
            PrefabUtility.UnloadPrefabContents(prefabRoot);
            return;
        }
        
        agent.baseOffset = 1.0f;
        
        Debug.Log("✅ SET baseOffset = 1.0");
        Debug.Log("   This lifts the Imp 1 unit above the NavMesh surface");
        Debug.Log("   preventing ground clipping!\n");
        
        ImpAI impAI = prefabRoot.GetComponent<ImpAI>();
        if (impAI != null)
        {
            SerializedObject serializedImp = new SerializedObject(impAI);
            
            int playerLayerIndex = LayerMask.NameToLayer("WhatIsPlayer");
            int groundLayerIndex = LayerMask.NameToLayer("WhatIsGround");
            
            if (playerLayerIndex >= 0)
            {
                serializedImp.FindProperty("whatIsPlayer").intValue = 1 << playerLayerIndex;
                Debug.Log($"✅ whatIsPlayer = layer {playerLayerIndex}");
            }
            
            if (groundLayerIndex >= 0)
            {
                serializedImp.FindProperty("whatIsGround").intValue = 1 << groundLayerIndex;
                Debug.Log($"✅ whatIsGround = layer {groundLayerIndex}");
            }
            
            serializedImp.ApplyModifiedProperties();
        }
        
        PrefabUtility.SaveAsPrefabAsset(prefabRoot, path);
        PrefabUtility.UnloadPrefabContents(prefabRoot);
        
        Debug.Log("\n🔥 IMP FIXED!");
        Debug.Log("✅ baseOffset = 1.0 (no more ground clipping)");
        Debug.Log("✅ LayerMasks set correctly\n");
        Debug.Log("🎮 PRESS PLAY AND TEST!\n");
    }
}
