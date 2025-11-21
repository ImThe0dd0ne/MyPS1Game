using UnityEditor;
using UnityEngine;
using System.IO;

public class DELETE_CORRUPTED_FILES
{
    [MenuItem("Tools/Delete Corrupted ImpAI in Scripts folder")]
    public static void DeleteCorruptedImpAI()
    {
        string corruptedFile = "Assets/Scripts/ImpAI.cs";
        
        if (File.Exists(corruptedFile))
        {
            AssetDatabase.DeleteAsset(corruptedFile);
            AssetDatabase.Refresh();
            Debug.Log("✅ Deleted corrupted ImpAI.cs from Scripts folder");
            Debug.Log("The good ImpAI.cs is in /Assets/My Scripts/ folder");
        }
        else
        {
            Debug.Log("File doesn't exist or already deleted");
        }
    }
}
