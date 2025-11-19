using UnityEngine;
using TMPro;

public class DamageNumberSpawner : MonoBehaviour
{
    public static DamageNumberSpawner Instance { get; private set; }

    [Header("Damage Number Settings")]
    public float lifetime = 1.5f;
    public float floatSpeed = 2f;
    public float fadeSpeed = 1.5f;
    public Vector3 randomOffset = new Vector3(0.5f, 0.5f, 0.5f);

    [Header("Colors")]
    public Color normalDamageColor = Color.white;
    public Color comboDamageColor = new Color(1f, 0.8f, 0f);
    public Color criticalDamageColor = new Color(1f, 0.3f, 0f);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SpawnDamageNumber(Vector3 position, int damage, int comboCount = 0)
    {
        GameObject textObj = new GameObject("DamageNumber");
        
        Vector3 offset = new Vector3(
            Random.Range(-randomOffset.x, randomOffset.x),
            Random.Range(0, randomOffset.y),
            Random.Range(-randomOffset.z, randomOffset.z)
        );
        textObj.transform.position = position + offset;

        TextMeshPro tmp = textObj.AddComponent<TextMeshPro>();
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
        
        if (comboCount > 1)
        {
            tmp.text = $"{damage}\n<size=60%>x{comboCount}</size>";
            tmp.fontSize = 5;
            tmp.color = comboDamageColor;
        }
        else
        {
            tmp.text = damage.ToString();
            tmp.fontSize = 4;
            tmp.color = normalDamageColor;
        }

        DamageNumber damageNum = textObj.AddComponent<DamageNumber>();
        damageNum.floatSpeed = floatSpeed;
        damageNum.fadeSpeed = fadeSpeed;

        Destroy(textObj, lifetime);
    }
}

public class DamageNumber : MonoBehaviour
{
    public float floatSpeed = 2f;
    public float fadeSpeed = 1.5f;

    private TextMeshPro textMesh;
    private Vector3 moveDirection;

    private void Start()
    {
        textMesh = GetComponent<TextMeshPro>();
        moveDirection = Vector3.up + new Vector3(Random.Range(-0.2f, 0.2f), 0, Random.Range(-0.2f, 0.2f));
        
        transform.localScale = Vector3.one * 0.5f;

        if (Camera.main != null)
        {
            transform.LookAt(Camera.main.transform);
            transform.Rotate(0, 180, 0);
        }
    }

    private void Update()
    {
        transform.position += moveDirection * floatSpeed * Time.deltaTime;
        
        if (textMesh != null)
        {
            Color col = textMesh.color;
            col.a -= fadeSpeed * Time.deltaTime;
            textMesh.color = col;
        }

        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, 3f * Time.deltaTime);

        if (Camera.main != null)
        {
            transform.LookAt(Camera.main.transform);
            transform.Rotate(0, 180, 0);
        }
    }
}
