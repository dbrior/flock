using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    [SerializeField] private int playerId;
    public float moveSpeed;
    private Vector2 moveVec;
    private Vector2 heading;
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
        heading = Vector2.down;
    }

    void Start() {
        // Set Player Skin
        for (int i=0; i<animator.layerCount; i++) {
            animator.SetLayerWeight(i, i == playerId ? 1f : 0f);
        }
    }

    private Dictionary<Vector2, int> cardinalIntMappings = new Dictionary<Vector2, int>{
        { Vector2.zero, 0 },
        { Vector2.up, 1 },
        { Vector2.right, 2 },
        { Vector2.down, 3 },
        { Vector2.left, 4 }
    };
    void Update() {
        Vector2 newHeading;
        if (moveVec == Vector2.zero) {
            newHeading = Vector2.zero;
        } else if (Mathf.Abs(moveVec.x) > Mathf.Abs(moveVec.y)) {
            newHeading = moveVec.x > 0 ? Vector2.right : Vector2.left;
        } else {
            newHeading = moveVec.y > 0 ? Vector2.up : Vector2.down;
        }

        if (newHeading != heading) {
            heading = newHeading;
            animator.SetInteger("Direction", cardinalIntMappings[heading]);
        }
    }

    void FixedUpdate() {
        Vector2 targetVel = moveVec * moveSpeed;
        Vector2 velDelta = targetVel - rb.velocity;
        Vector2 requiredAccel = velDelta / Time.fixedDeltaTime;
        rb.AddForce(requiredAccel * rb.mass);
    }

    public void OnMove(InputValue inputValue) {
        moveVec = inputValue.Get<Vector2>();
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
        GameObject pellet = Instantiate(pelletPrefab, (Vector2) transform.position + (heading * 0.1f), Quaternion.identity);
        Rigidbody2D pelletRb = pellet.GetComponent<Rigidbody2D>();
        pelletRb.velocity = heading * pelletSpeed;
        Destroy(pellet, 2f);
    }

    public void CollectItem(ItemDrop item) {
        audioSource.PlayOneShot(collectSound);
        woolCount += 1;
        UIManager.Instance.UpdateWoolCount(woolCount);
        Destroy(item.gameObject);
    }
}
