using UnityEngine;
using UnityEngine.Events;

public class PlayerTrigger : MonoBehaviour
{
    [SerializeField] private UnityEvent onEnter;
    [SerializeField] private UnityEvent onExit;

    void OnTriggerEnter2D(Collider2D col) {
        onEnter?.Invoke();
    }

    void OnTriggerExit2D(Collider2D col) {
        onExit?.Invoke();
    }
}
