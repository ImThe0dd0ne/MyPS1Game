using UnityEngine;

public class HitSpark : MonoBehaviour
{
    [Header("Spark Settings")]
    public float lifetime = 0.5f;
    public float startSize = 0.5f;
    public float endSize = 0.1f;
    public Color startColor = new Color(1f, 0.8f, 0.3f, 1f);
    public Color endColor = new Color(1f, 0.3f, 0f, 0f);

    private float age = 0f;
    private MeshRenderer meshRenderer;

    private void Start()
    {
        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.transform.SetParent(transform);
        quad.transform.localPosition = Vector3.zero;
        quad.transform.localRotation = Quaternion.identity;
        
        Collider col = quad.GetComponent<Collider>();
        if (col != null) Destroy(col);
        
        meshRenderer = quad.GetComponent<MeshRenderer>();
        
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        mat.color = startColor;
        meshRenderer.material = mat;
        
        transform.localScale = Vector3.one * startSize;
        
        if (Camera.main != null)
        {
            transform.LookAt(Camera.main.transform);
        }

        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        age += Time.deltaTime;
        float t = age / lifetime;

        if (meshRenderer != null && meshRenderer.material != null)
        {
            meshRenderer.material.color = Color.Lerp(startColor, endColor, t);
        }

        transform.localScale = Vector3.one * Mathf.Lerp(startSize, endSize, t);

        if (Camera.main != null)
        {
            transform.LookAt(Camera.main.transform);
        }
    }

    public static void Create(Vector3 position, Vector3 normal)
    {
        GameObject spark = new GameObject("HitSpark");
        spark.transform.position = position;
        spark.transform.rotation = Quaternion.LookRotation(normal);
        
        HitSpark hitSpark = spark.AddComponent<HitSpark>();
    }
}
