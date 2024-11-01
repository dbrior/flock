using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float moveSpeed;
    private Vector2 moveVec;
    private Rigidbody2D rb;
    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();   
    }

    void FixedUpdate() {
        rb.MovePosition(rb.position + moveVec * moveSpeed * Time.fixedDeltaTime);
    }

    public void OnMove(InputValue inputValue) {
        moveVec = inputValue.Get<Vector2>();
    }
}
