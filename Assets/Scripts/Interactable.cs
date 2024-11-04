using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [SerializeField] public UnityEvent onInteract;

    public void Interact() {
        if (onInteract != null) {
            onInteract.Invoke();
        }
    }
}