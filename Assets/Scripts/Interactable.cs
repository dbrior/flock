using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [SerializeField] public UnityEvent onInteract;
    [SerializeField] public UnityEvent<GameObject> onPlayerInteract;
    [SerializeField] public string interactionText;

    public void Interact(Player player)
    {
        onPlayerInteract?.Invoke(player.gameObject);
        onInteract?.Invoke();
    }
}