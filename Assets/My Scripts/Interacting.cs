using UnityEngine;
using UnityEngine.UI;

interface IInteractable
{
    public void Interact();
}

public class Interacting : MonoBehaviour
{
    public Transform InteractorSource;
    public float InteractorRange; 
    public RawImage renderTextureDisplay;
    public Camera renderCamera; 

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Vector3 screenCenter = new Vector3(renderCamera.pixelWidth / 2, renderCamera.pixelHeight / 2, 0);
            Ray r = renderCamera.ScreenPointToRay(screenCenter);

            if (Physics.Raycast(r, out RaycastHit hitInfo, InteractorRange, ~LayerMask.GetMask("Player")))
            {
                if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
                {
                    interactObj.Interact();
                }
            }
        }
    }
}