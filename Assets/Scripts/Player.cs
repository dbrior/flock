using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    [SerializeField] private int playerId;
    private Rigidbody2D rb;
    private Animator animator;
    private ToolBelt toolBelt;

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
    private AudioSource audioSource;
    
    [Header("Misc")]
    // Anything here should probably not be here
    [SerializeField] private float attackRadius;
    [SerializeField] private GameObject flashlight;
    private bool flashlightEnabled;
    private bool inMenu;
    private bool isAttacking;
    private Damagable damagable;
    [SerializeField] private float attackDamange;
    [SerializeField] private float knockbackForce = 200f;
    [SerializeField] private Rigidbody2D ropeRb;
    [SerializeField] private Weapon ropeWeapon;
    public List<Transform> hunterSlots;
    [SerializeField] private AudioClip stepSound;
    public Spinner spinner;
    
    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        interactionHints = GetComponent<InteractionHints>();
        damagable = GetComponent<Damagable>();
        toolBelt = GetComponent<ToolBelt>();
        heading = Vector2.down;
    }

    void Start() {
        // Set Player Skin
        for (int i=0; i<2; i++) {
            animator.SetLayerWeight(i, i == playerId ? 1f : 0f);
        }

        ropeWeapon.damage = attackDamange;
        ropeWeapon.knockbackForce = knockbackForce;
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

            if (moveCardinal != Vector2.zero) {
                SetHeading(moveCardinal);
            }
        }

        // Rotate flashlight
        float zRotation = 0;
        if (heading == Vector2.up) {
            zRotation = 0;
        } else if (heading == Vector2.right) {
            zRotation = -90f;
        } else if (heading == Vector2.down) {
            zRotation = 180f;
        } else if (heading == Vector2.left) {
            zRotation = 90f;
        }

        float rotateSpeed = 0.05f;
        flashlight.transform.rotation = Quaternion.Lerp(flashlight.transform.rotation, Quaternion.Euler(0, 0, zRotation), rotateSpeed);
    }

    private void SetHeading(Vector2 newHeading) {
        heading = newHeading;
        toolBelt.heading = newHeading;
    }

    void FixedUpdate() {
        Vector2 targetVel = moveVec * moveSpeed;
        Vector2 velDelta = targetVel - rb.velocity;
        Vector2 requiredAccel = velDelta / Time.fixedDeltaTime;
        rb.AddForce(requiredAccel * rb.mass);

        if (secondaryActive) {
            ropeRb.angularVelocity = 1250f;
        }
    }

    public void OpenMenu() {
        inMenu = true;
        moveVec = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.velocity = Vector2.zero;
    }

    public void CloseMenu() {
        inMenu = false;
        moveVec = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    private bool stepping = false;
    public void OnMove(InputValue inputValue) {
        if (inMenu) return;
        Vector2 newMoveVec = inputValue.Get<Vector2>();

        // Move sound
        if (!stepping && (moveVec == Vector2.zero && newMoveVec != Vector2.zero)) {
            audioSource.clip = stepSound;
            audioSource.loop = true;
            audioSource.Play();
            audioSource.loop = true;
            stepping = true;
        } else if (stepping && (newMoveVec == Vector2.zero && moveVec != Vector2.zero)) {
            audioSource.loop = false;
            stepping = false;
        }

        moveVec = newMoveVec;

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
        if (inMenu) return;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, heading, interactRange, interactionLayer);
        if (hit) {
            Debug.Log("Player " + playerId + ": Interact with " + hit.transform.gameObject.name);
        }
        if (hit && hit.transform.gameObject.TryGetComponent<Interactable>(out Interactable interactable)) {
            interactable.Interact(this);
        }
    }

    private Vector2 GetGridLocation(Vector2 position)
    {
        float x = Mathf.Round((position.x - 0.08f) / 0.16f) * 0.16f + 0.08f;
        float y = Mathf.Round((position.y - 0.08f) / 0.16f) * 0.16f + 0.08f;

        return new Vector2(x, y);
    }

    public void FinishAttack() {
        isAttacking = false;
    }

    public void StartAttack() {
        isAttacking = true;
    }

    public void OnAttack() {
        if (isAttacking || inMenu) return;
        Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(transform.position, attackRadius);
        if (objectsInRange.Length > 0) {
            foreach (Collider2D obj in objectsInRange) {
                if (obj.gameObject != gameObject && obj.TryGetComponent<Damagable>(out Damagable damagable)) {
                    damagable.Hit(transform.position, attackDamange, knockbackForce);
                }
            }
        }
        animator.SetTrigger("Attack");
        isAttacking = true;
    }

    private bool secondaryActive = false;
    public void OnSecondaryAttack(InputValue value) {
        // toolBelt.UseSlingshot();
        if (value.isPressed) {
            ropeWeapon.EnableDamage();
            secondaryActive = true;
        } else {
            ropeWeapon.DisableDamage();
            secondaryActive = false;
        }
        // ropeTool.angularVelocity = 10f;
    }
    // public void OnSecondaryAttackCanceled() {
    //     // toolBelt.UseSlingshot();
    //     Debug.Log("inactive");
    //     secondaryActive = false;
    //     // ropeTool.angularVelocity = 10f;
    // }

    // WateringRadius,
    // Knockback,
    // MoveSpeed,
    // PenCapacity
    public void AddUpgrade(UpgradeType upgradeType, float value) {
        Debug.Log(upgradeType);
        if (upgradeType == UpgradeType.ShearRadius) {
            toolBelt.shearRadius += value;
        } else if (upgradeType == UpgradeType.Strength) {
            rb.mass += value;
        } else if (upgradeType == UpgradeType.RopeLength) {
            Rope.Instance.AdjustMaxSegments((int) value);
        } else if (upgradeType == UpgradeType.MoveSpeed) {
            moveSpeed += value/10f;
        } else if (upgradeType == UpgradeType.Damage) {
            attackDamange *= 1f + (value/100f);
        } else if (upgradeType == UpgradeType.Knockback) {
            knockbackForce *= 1f + (value/100f);
        } else if (upgradeType == UpgradeType.MaxHealth) {
            damagable.ChangeMaxHealthPct(value/100f);
        } else if (upgradeType == UpgradeType.Heal) {
            damagable.HealPct(value/100f);
        } else if (upgradeType == UpgradeType.BlockChance) {
            damagable.ChangeBlockChance(value/100f);
        } else if (upgradeType == UpgradeType.HealthRegen) {
            damagable.ChangeHealthRegen(value);
        } else if (upgradeType == UpgradeType.CritChance) {
            ropeWeapon.ChangeCritChance(value/100f);
        } else if (upgradeType == UpgradeType.CritMultiplier) {
            ropeWeapon.ChangeCritMultiplier(value/100f);
        }

        ropeWeapon.damage = attackDamange;
        ropeWeapon.knockbackForce = knockbackForce;
    }

    public void OnIncreaseRope() {
        Rope.Instance.AddSegment();
    }

    public void OnDecreaseRope() {
        Rope.Instance.RemoveSegment();
    }

    public void Heal() {
        damagable.RestoreHealth();
    }

    public void IncreaseDamage(float addedDamage) {
        attackDamange += addedDamage;
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
