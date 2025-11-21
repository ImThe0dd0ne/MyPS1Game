using UnityEngine;
using UnityEditor;

public class ADD_DEBUG_TO_IMP
{
    [MenuItem("Tools/📊 ADD DEBUG LOGGER TO IMP")]
    public static void AddDebugLogger()
    {
        string impPrefabPath = "Assets/Prefabs/Imp.prefab";
        GameObject impPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(impPrefabPath);
        
        if (impPrefab == null)
        {
            Debug.LogError("❌ Imp prefab not found!");
            return;
        }
        
        string path = AssetDatabase.GetAssetPath(impPrefab);
        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(path);
        
        ImpDebugLogger existing = prefabRoot.GetComponent<ImpDebugLogger>();
        if (existing == null)
        {
            prefabRoot.AddComponent<ImpDebugLogger>();
            Debug.Log("✅ Added ImpDebugLogger to Imp prefab");
        }
        else
        {
            Debug.Log("✅ ImpDebugLogger already on Imp prefab");
        }
        
        PrefabUtility.SaveAsPrefabAsset(prefabRoot, path);
        PrefabUtility.UnloadPrefabContents(prefabRoot);
        
        Debug.Log("\n🎮 PRESS PLAY AND START ARENA (B key)");
        Debug.Log("📊 Watch the console for detailed Imp spawn info!\n");
    }
}
