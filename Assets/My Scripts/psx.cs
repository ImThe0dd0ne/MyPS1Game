using UnityEngine;

[ExecuteInEditMode]
public class PSXPostProcess : MonoBehaviour
{
    public Material psxMaterial;

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (psxMaterial != null)
            Graphics.Blit(src, dest, psxMaterial);
        else
            Graphics.Blit(src, dest);
    }
}

