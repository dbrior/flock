using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float moveSpeed;
    private Vector2 moveVec;
    private Rigidbody2D rb;
    private Animator animator;
    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
    }

    void FixedUpdate() {
        rb.MovePosition(rb.position + moveVec * moveSpeed * Time.fixedDeltaTime);
    }

    public void OnMove(InputValue inputValue) {
        moveVec = inputValue.Get<Vector2>();

        if (moveVec.y == 0 && moveVec.x == 0) {
            animator.SetTrigger("Idle");
        } else {
            if (moveVec.y < 0) {
                animator.SetTrigger("MoveDown");
            } else if (moveVec.y > 0) {
                animator.SetTrigger("MoveUp");
            }
            if (moveVec.x < 0) {
                animator.SetTrigger("MoveLeft");
            } else if (moveVec.x > 0) {
                animator.SetTrigger("MoveRight");
            }
        }
    }
}
