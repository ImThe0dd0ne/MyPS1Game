using UnityEngine;

public class NumberGeneratorInteract : MonoBehaviour, IInteractable { 
   public void Interact()
    {
        Debug.Log(Random.Range(0, 100));
    }
}
