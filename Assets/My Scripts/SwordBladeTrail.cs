using UnityEngine;
using System.Collections.Generic;

public class SwordBladeTrail : MonoBehaviour
{
    [Header("Tip/Base Points")]
    public Transform tipPoint;
    public Transform basePoint;

    [Header("Trail Settings")]
    public Material trailMaterial; // optional
    public float trailDuration = 0.3f;
    public float width = 0.5f; // thick for visibility
    public Color trailColor = Color.cyan;

    private GameObject trailObject;
    private Mesh trailMesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private List<Vector3> tipPositions = new List<Vector3>();
    private List<Vector3> basePositions = new List<Vector3>();
    private List<float> timeStamps = new List<float>();

    private bool isEmitting = false;
    private float currentTime = 0f;

    void Awake()
    {
        SetupTrailObject();
    }

    void SetupTrailObject()
    {
        trailObject = new GameObject("BladeTrail");
        trailObject.transform.SetParent(transform);
        trailObject.transform.localPosition = Vector3.zero;
        trailObject.transform.localRotation = Quaternion.identity;

        meshFilter = trailObject.AddComponent<MeshFilter>();
        meshRenderer = trailObject.AddComponent<MeshRenderer>();

        if (trailMaterial != null)
            meshRenderer.material = trailMaterial;
        else
        {
            meshRenderer.material = new Material(Shader.Find("Unlit/Color"));
            meshRenderer.material.color = trailColor;
        }

        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        meshRenderer.receiveShadows = false;

        trailMesh = new Mesh();
        trailMesh.name = "SwordTrailMesh";
        meshFilter.mesh = trailMesh;
    }

    void Update()
    {
        if (!isEmitting) return;

        currentTime += Time.deltaTime;

        // Always add point every frame, no minDistance check
        tipPositions.Add(tipPoint.position);
        basePositions.Add(basePoint.position);
        timeStamps.Add(currentTime);

        // Remove old points beyond trailDuration
        while (timeStamps.Count > 0 && currentTime - timeStamps[0] > trailDuration)
        {
            tipPositions.RemoveAt(0);
            basePositions.RemoveAt(0);
            timeStamps.RemoveAt(0);
        }

        UpdateMesh();
    }

    void UpdateMesh()
    {
        if (tipPositions.Count < 2)
        {
            trailMesh.Clear();
            return;
        }

        int vertexCount = tipPositions.Count * 2;
        int triangleCount = (tipPositions.Count - 1) * 6;

        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[triangleCount];
        Vector2[] uvs = new Vector2[vertexCount];
        Color[] colors = new Color[vertexCount];

        for (int i = 0; i < tipPositions.Count; i++)
        {
            float age = (currentTime - timeStamps[i]) / trailDuration;
            float alpha = Mathf.Clamp01(1f - age);

            vertices[i * 2] = basePositions[i];
            vertices[i * 2 + 1] = tipPositions[i];

            colors[i * 2] = new Color(trailColor.r, trailColor.g, trailColor.b, alpha);
            colors[i * 2 + 1] = new Color(trailColor.r, trailColor.g, trailColor.b, alpha);

            float uvX = (float)i / (tipPositions.Count - 1);
            uvs[i * 2] = new Vector2(uvX, 0);
            uvs[i * 2 + 1] = new Vector2(uvX, 1);
        }

        int triIndex = 0;
        for (int i = 0; i < tipPositions.Count - 1; i++)
        {
            int baseIndex = i * 2;

            triangles[triIndex++] = baseIndex;
            triangles[triIndex++] = baseIndex + 1;
            triangles[triIndex++] = baseIndex + 2;

            triangles[triIndex++] = baseIndex + 1;
            triangles[triIndex++] = baseIndex + 3;
            triangles[triIndex++] = baseIndex + 2;
        }

        trailMesh.Clear();
        trailMesh.vertices = vertices;
        trailMesh.triangles = triangles;
        trailMesh.uv = uvs;
        trailMesh.colors = colors;
        trailMesh.RecalculateBounds();
        trailMesh.RecalculateNormals();
    }

    public void EnableTrail()
    {
        isEmitting = true;
        currentTime = 0f;
        tipPositions.Clear();
        basePositions.Clear();
        timeStamps.Clear();
    }

    public void DisableTrail()
    {
        isEmitting = false;
    }

    void OnDestroy()
    {
        if (trailObject != null)
            Destroy(trailObject);
    }
}
