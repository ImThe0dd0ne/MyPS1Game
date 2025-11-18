using UnityEngine;
using System.Collections;

public class HitEffect : MonoBehaviour
{
    private Renderer[] renderers;
    private MaterialPropertyBlock propertyBlock;
    private bool isFlashing = false;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        propertyBlock = new MaterialPropertyBlock();
    }

    public void FlashDamage(Color flashColor, float duration = 0.1f)
    {
        if (!isFlashing)
            StartCoroutine(FlashRoutine(flashColor, duration));
    }

    private IEnumerator FlashRoutine(Color flashColor, float duration)
    {
        isFlashing = true;

        foreach (Renderer r in renderers)
        {
            r.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor("_EmissionColor", flashColor * 2f);
            r.SetPropertyBlock(propertyBlock);
        }

        yield return new WaitForSeconds(duration);

        foreach (Renderer r in renderers)
        {
            r.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor("_EmissionColor", Color.black);
            r.SetPropertyBlock(propertyBlock);
        }

        isFlashing = false;
    }

    public void DissolveEffect(float duration = 2f)
    {
        StartCoroutine(DissolveRoutine(duration));
    }

    private IEnumerator DissolveRoutine(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float dissolve = elapsed / duration;

            foreach (Renderer r in renderers)
            {
                r.GetPropertyBlock(propertyBlock);
                propertyBlock.SetFloat("_Dissolve", dissolve);
                r.SetPropertyBlock(propertyBlock);
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}
