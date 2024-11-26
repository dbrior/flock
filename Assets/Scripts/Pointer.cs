using UnityEngine;
using UnityEngine.InputSystem;

public class Pointer : MonoBehaviour
{
    public static Pointer Instance { get; private set;}

    [SerializeField] private InputActionReference pointerPositionInput;
    private Vector2 pointerPosition;

    void Awake() {
        if (Instance == null) {Instance = this;} 
        else {Destroy(gameObject);}
    }

    private void OnEnable() {
        pointerPositionInput.action.Enable();
    }

    private void OnDisable() {
        pointerPositionInput.action.Disable();
    }

    private void Update() {
        pointerPosition = pointerPositionInput.action.ReadValue<Vector2>();

        if (Mouse.current != null && Mouse.current.wasUpdatedThisFrame) {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector2(pointerPosition.x, pointerPosition.y));
            transform.position = new Vector2(worldPosition.x, worldPosition.y);
        } else {
            transform.position += new Vector3(pointerPosition.x, pointerPosition.y, 0) * Time.deltaTime;
        }
    }
   
}
