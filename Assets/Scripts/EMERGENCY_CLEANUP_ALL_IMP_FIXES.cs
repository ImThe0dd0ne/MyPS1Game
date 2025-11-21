using UnityEngine;
using UnityEditor;

public class EMERGENCY_CLEANUP_ALL_IMP_FIXES : MonoBehaviour
{
    [MenuItem("Tools/🧹 EMERGENCY: Clean All Imp Debug Scripts")]
    public static void CleanAll()
    {
        Debug.Log("╔═══════════════════════════════════════════════════════════╗");
        Debug.Log("║     🧹 EMERGENCY CLEANUP - REMOVING ALL DEBUG SCRIPTS    ║");
        Debug.Log("╚═══════════════════════════════════════════════════════════╝\n");
        
        string impPrefabPath = "Assets/Prefabs/Imp.prefab";
        GameObject impPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(impPrefabPath);
        
        if (impPrefab == null)
        {
            Debug.LogError("❌ Imp prefab not found!");
            return;
        }
        
        string path = AssetDatabase.GetAssetPath(impPrefab);
        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(path);
        
        int removedCount = 0;
        string[] scriptsToRemove = new string[]
        {
            "ImpMovementDebug",
            "ImpRuntimeDebug",
            "ImpSpawnFix",
            "ImpHeightFixer",
            "ImpPositionFixer",
            "SimpleImpDiagnostic",
            "DiagnoseImpIssues",
            "VerifyImpSetup"
        };
        
        Component[] allComponents = prefabRoot.GetComponents<Component>();
        foreach (Component comp in allComponents)
        {
            if (comp == null) continue;
            if (comp is Transform) continue;
            
            string typeName = comp.GetType().Name;
            
            foreach (string scriptName in scriptsToRemove)
            {
                if (typeName == scriptName || typeName.Contains(scriptName))
                {
                    Debug.Log($"  ❌ Removing: {typeName}");
                    Object.DestroyImmediate(comp, true);
                    removedCount++;
                    break;
                }
            }
        }
        
        Debug.Log($"\n✅ Removed {removedCount} debug/fix components");
        
        PrefabUtility.SaveAsPrefabAsset(prefabRoot, path);
        PrefabUtility.UnloadPrefabContents(prefabRoot);
        
        Debug.Log("\n╔═══════════════════════════════════════════════════════════╗");
        Debug.Log("║              ✅ CLEANUP COMPLETE!                        ║");
        Debug.Log("╚═══════════════════════════════════════════════════════════╝\n");
    }
}
