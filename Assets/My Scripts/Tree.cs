using UnityEngine;

public class TreeRotationFixer : MonoBehaviour
{
    [Header("Tree Settings")]
    public string treeTag = "Tree";
    public Vector3 correctRotation = Vector3.zero;

    [Header("Debug")]
    public bool showDebugInfo = true;

    void Start()
    {
        FixAllTreeRotations();
    }

    [ContextMenu("Fix Tree Rotations")]
    public void FixAllTreeRotations()
    {
        GameObject[] trees = GameObject.FindGameObjectsWithTag(treeTag);

        if (trees.Length == 0)
        {
            Debug.LogWarning("No trees found with tag: " + treeTag);
            return;
        }

        int fixedCount = 0;

        foreach (GameObject tree in trees)
        {
            Vector3 originalRotation = tree.transform.eulerAngles;
            tree.transform.eulerAngles = correctRotation;
            fixedCount++;

            if (showDebugInfo)
            {
                Debug.Log($"Fixed tree: {tree.name} | Was: {originalRotation} | Now: {correctRotation}");
            }
        }

        Debug.Log($"Fixed rotation for {fixedCount} trees");
    }

    [ContextMenu("Fix Trees By Name")]
    public void FixTreesByName()
    {
        string treeNamePattern = "Tree";
        GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        int fixedCount = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains(treeNamePattern))
            {
                obj.transform.eulerAngles = correctRotation;
                fixedCount++;

                if (showDebugInfo)
                {
                    Debug.Log($"Fixed tree by name: {obj.name}");
                }
            }
        }

        Debug.Log($"Fixed rotation for {fixedCount} trees by name pattern");
    }
}
