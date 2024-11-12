using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine;

public class MenuItem : MonoBehaviour
{
    [SerializeField] private UnityEvent onConfirm;
    [SerializeField] private UnityEvent onBack;
    [SerializeField] private bool isDefaultSelected;

    void Start() {
        if (isDefaultSelected) {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }

    public void OnInteract() {
        if(EventSystem.current.currentSelectedGameObject == gameObject) {
            onConfirm?.Invoke();
        }
    }

    public void OnBack() {
        onBack?.Invoke();
    }
}
