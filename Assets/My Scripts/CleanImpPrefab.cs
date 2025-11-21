using UnityEngine;
using UnityEditor;

public class CleanImpPrefab : MonoBehaviour
{
    [MenuItem("Tools/Clean Imp Prefab - Remove Missing Scripts")]
    public static void CleanPrefab()
    {
        string prefabPath = "Assets/Prefabs/Imp.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        
        if (prefab == null)
        {
            Debug.LogError("Imp prefab not found!");
            return;
        }
        
        Debug.Log("╔═══════════════════════════════════════════════════════════╗");
        Debug.Log("║         CLEANING IMP PREFAB                              ║");
        Debug.Log("╚═══════════════════════════════════════════════════════════╝");
        
        string path = AssetDatabase.GetAssetPath(prefab);
        GameObject prefabContents = PrefabUtility.LoadPrefabContents(path);
        
        int removed = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(prefabContents);
        Debug.Log($"Removed {removed} missing script(s) from root GameObject");
        
        int totalRemoved = removed;
        
        foreach (Transform child in prefabContents.GetComponentsInChildren<Transform>(true))
        {
            int childRemoved = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(child.gameObject);
            if (childRemoved > 0)
            {
                Debug.Log($"  Removed {childRemoved} from: {child.name}");
                totalRemoved += childRemoved;
            }
        }
        
        Debug.Log($"\n✅ Total missing scripts removed: {totalRemoved}");
        
        if (prefabContents.GetComponent<ImpMovementDebug>() == null)
        {
            prefabContents.AddComponent<ImpMovementDebug>();
            Debug.Log("✅ Added ImpMovementDebug component");
        }
        
        PrefabUtility.SaveAsPrefabAsset(prefabContents, path);
        PrefabUtility.UnloadPrefabContents(prefabContents);
        
        Debug.Log("✅ Prefab cleaned and saved!");
        Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Debug.Log("Test in Play mode now - check console for detailed movement analysis");
    }
}
