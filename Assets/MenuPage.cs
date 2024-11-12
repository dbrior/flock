using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine;

public class MenuPage : MonoBehaviour
{
    [SerializeField] private UnityEvent onBack;
    void OnEnable() {
        EventSystem.current.SetSelectedGameObject(transform.GetChild(0).gameObject);
    }

    public void OnBack() {
        onBack?.Invoke();
    }
}
