using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float moveSpeed;
    private Vector2 moveVec;
    private Rigidbody2D rb;
    private Animator animator;
    private AudioSource audioSource;
    private int woolCount;
    [SerializeField] private AudioClip collectSound;
    [SerializeField] private GameObject pelletPrefab;
    [SerializeField] private float pelletSpeed;
    [SerializeField] private LayerMask interactionLayer;
    [SerializeField] private float interactionRadius;
    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        woolCount = 0;
    }

    void FixedUpdate() {
        Vector2 targetVel = moveVec * moveSpeed;
        Vector2 velDelta = targetVel - rb.velocity;
        Vector2 requiredAccel = velDelta / Time.fixedDeltaTime;
        rb.AddForce(requiredAccel * rb.mass);
        // rb.MovePosition(rb.position + moveVec * moveSpeed * Time.fixedDeltaTime);
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

    public void OnInteract() {
        Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(transform.position, interactionRadius, interactionLayer);
        if (objectsInRange.Length > 0) {
            foreach (Collider2D obj in objectsInRange) {
                Interactable interactable = obj.GetComponent<Interactable>();
                if (interactable != null) {
                    interactable.Interact();
                }
            }
        }
    }

    public void OnUseItem() {
        GameObject pellet = Instantiate(pelletPrefab, transform.position, Quaternion.identity);
        Rigidbody2D pelletRb = pellet.GetComponent<Rigidbody2D>();
        pelletRb.velocity = moveVec * pelletSpeed;
        Destroy(pellet, 2f);
    }

    public void CollectItem(ItemDrop item) {
        audioSource.PlayOneShot(collectSound);
        woolCount += 1;
        UIManager.Instance.UpdateWoolCount(woolCount);
        Destroy(item.gameObject);
    }
}
