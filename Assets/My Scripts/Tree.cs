using UnityEngine;

public class TreeRotationFixer : MonoBehaviour
{
    [Header("Tree Settings")]
    public string treeTag = "Tree"; // Set this tag on your trees
    public Vector3 correctRotation = Vector3.zero; // The rotation you want

    [Header("Debug")]
    public bool showDebugInfo = true;

    void Start()
    {
        FixAllTreeRotations();
    }

    [ContextMenu("Fix Tree Rotations")]
    public void FixAllTreeRotations()
    {
        // Find all objects with the tree tag
        GameObject[] trees = GameObject.FindGameObjectsWithTag(treeTag);

        if (trees.Length == 0)
        {
            Debug.LogWarning("No trees found with tag: " + treeTag);
            return;
        }

        int fixedCount = 0;

        foreach (GameObject tree in trees)
        {
            // Store original rotation for debug
            Vector3 originalRotation = tree.transform.eulerAngles;

            // Apply correct rotation
            tree.transform.eulerAngles = correctRotation;

            fixedCount++;

            if (showDebugInfo)
            {
                Debug.Log($"Fixed tree: {tree.name} | Was: {originalRotation} | Now: {correctRotation}");
            }
        }

        Debug.Log($"Fixed rotation for {fixedCount} trees");
    }

    // Alternative method: Fix trees by name pattern
    [ContextMenu("Fix Trees By Name")]
    public void FixTreesByName()
    {
        string treeNamePattern = "Tree"; // Change this to match your tree names

        GameObject[] allObjects = FindObjectsOfType<GameObject>();
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