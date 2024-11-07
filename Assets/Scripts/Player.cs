using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

[System.Serializable]
public enum Tool : int {
    Shears = 0,
    Slingshot = 1,
    SeedBag = 2,
    WateringCan = 3
}

[System.Serializable]
public enum UpgradeType : int {
    ShearRadius = 0,
    RopeLength = 1,
    Strength = 2
}

public class Player : MonoBehaviour
{
    [SerializeField] private int playerId;
    private Rigidbody2D rb;
    private Animator animator;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    private Vector2 moveVec;
    private Vector2 moveCardinal;
    private Vector2 heading;

    [Header("Interaction")]
    [SerializeField] private float interactRange;
    [SerializeField] private LayerMask interactionLayer;
    private InteractionHints interactionHints;

    [Header("Audio")]
    [SerializeField] private AudioClip collectSound;
    private AudioSource audioSource;
    
    [Header("Misc")]
    // Anything here should probably not be here
    private Tool currentTool;
    private int totalToolCount;
    [SerializeField] private ToolUI toolUI;
    [SerializeField] private float shearRadius;
    [SerializeField] private float wateringRadius;
    [SerializeField] private GameObject pelletPrefab;
    [SerializeField] private float pelletSpeed;
    private int woolCount;
    
    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        interactionHints = GetComponent<InteractionHints>();
        woolCount = 0;
        heading = Vector2.down;
        currentTool = Tool.Shears;
        totalToolCount = System.Enum.GetNames(typeof(Tool)).Length;
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
        Vector2 newMoveCardinal;
        if (moveVec == Vector2.zero) {
            newMoveCardinal = Vector2.zero;
        } else if (Mathf.Abs(moveVec.x) > Mathf.Abs(moveVec.y)) {
            newMoveCardinal = moveVec.x > 0 ? Vector2.right : Vector2.left;
        } else {
            newMoveCardinal = moveVec.y > 0 ? Vector2.up : Vector2.down;
        }

        if (newMoveCardinal != moveCardinal) {
            moveCardinal = newMoveCardinal;
            animator.SetInteger("Direction", cardinalIntMappings[moveCardinal]);
        }
        if (moveCardinal != Vector2.zero) {
            heading = moveCardinal;
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

        RaycastHit2D hit = Physics2D.Raycast(transform.position, heading, interactRange, interactionLayer);
        if (hit && hit.transform.gameObject.TryGetComponent<Interactable>(out Interactable interactable)) {
            interactionHints.ShowHint(interactable.interactionText);
        } else {
            interactionHints.HideHint();
        }
    }

    // void OnDrawGizmos()
    // {
    //     Gizmos.DrawLine((Vector2) transform.position + (heading), (Vector2) transform.position + (heading * interactRange));
    // }

    public void OnInteract() {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, heading, interactRange, interactionLayer);
        if (hit) {
            Debug.Log("Player " + playerId + ": Interact with " + hit.transform.gameObject.name);
        }
        if (hit && hit.transform.gameObject.TryGetComponent<Interactable>(out Interactable interactable)) {
            interactable.Interact(this);
        }
    }

    public void OnUseItem() {
        Debug.Log("Tool used " + currentTool);
        if (currentTool == Tool.Slingshot) {
            GameObject pellet = Instantiate(pelletPrefab, (Vector2) transform.position + (heading * 0.1f), Quaternion.identity);
            Rigidbody2D pelletRb = pellet.GetComponent<Rigidbody2D>();
            pelletRb.velocity = heading * pelletSpeed;
            Destroy(pellet, 2f);
        } else if (currentTool == Tool.Shears) {
            Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(transform.position, shearRadius, interactionLayer);
            if (objectsInRange.Length > 0) {
                foreach (Collider2D obj in objectsInRange) {
                    if (obj.TryGetComponent<ToolInteraction>(out ToolInteraction toolInteraction)) {
                        toolInteraction.UseTool(currentTool);
                    }
                }
            }
        } else if (currentTool == Tool.SeedBag) {
            CropManager.Instance.PlantCrop(transform.position);
        } else if (currentTool == Tool.WateringCan) {
            Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(transform.position, wateringRadius);
            if (objectsInRange.Length > 0) {
                foreach (Collider2D obj in objectsInRange) {
                    if (obj.TryGetComponent<ToolInteraction>(out ToolInteraction toolInteraction)) {
                        toolInteraction.UseTool(currentTool);
                    }
                }
            }
        }
    }

    public void OnChangeTool() {
        int toolIdx = ((int) currentTool + 1) % totalToolCount;
        currentTool = (Tool) toolIdx;
        toolUI.SetActiveTool(toolIdx);
    }

    public void CollectItem(ItemDrop item) {
        audioSource.PlayOneShot(collectSound);
        AdjustWoolCount(1);
        Destroy(item.gameObject);
    }

    public void AdjustWoolCount(int delta) {
        woolCount += delta;
        UIManager.Instance.UpdateWoolCount(woolCount);
    }

    public int GetWoolCount() {
        return woolCount;
    }

    public void AddUpgrade(UpgradeType upgradeType) {
        if (upgradeType == UpgradeType.ShearRadius) {
            shearRadius += 0.1f;
        } else if (upgradeType == UpgradeType.Strength) {
            rb.mass += 1;
        } else if (upgradeType == UpgradeType.RopeLength) {
            Rope.Instance.AdjustMaxSegments(1);
        }
    }

    public void OnIncreaseRope() {
        Rope.Instance.AddSegment();
    }

    public void OnDecreaseRope() {
        Rope.Instance.RemoveSegment();
    }

    // void OnCollisionEnter2D(Collision2D col) {
    //     if (col.gameObject.TryGetComponent<Interactable>(out Interactable interactable)) {
    //         interactionHints.ShowHint(interactable.interactionText);
    //     }
    // }

    // void OnCollisionExit2D(Collision2D col) {
    //     if (col.gameObject.TryGetComponent<Interactable>(out Interactable interactable)) {
    //         interactionHints.HideHint();
    //     }
    // }
}
