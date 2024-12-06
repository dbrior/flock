using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Wolf : MonoBehaviour
{
    [SerializeField]
    private LayerMask sheepLayer;
    [SerializeField]
    private float moveSpeed;
    [SerializeField] private float maxForce;
    private Vector2 heading;
    [SerializeField]
    private float detectionRadius;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 animationDirection;
    [SerializeField] private float waitMin;
    [SerializeField] private float waitMax;
    [SerializeField] private float attackDamage;
    [SerializeField] private float attackCooldownSec;

    private float lastHitTime;

    private Damagable damagable;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        damagable = GetComponent<Damagable>();
    }

    void Start()
    {
        lastHitTime = 0;
        StartCoroutine(ScanForSheep());
        animator.SetLayerWeight(0, 0);
        animator.SetLayerWeight(1, 0);
        animator.SetLayerWeight(2, 1f);
    }
    private Dictionary<Vector2, int> cardinalIntMappings = new Dictionary<Vector2, int>{
        { Vector2.zero, 0 },
        { Vector2.up, 1 },
        { Vector2.right, 2 },
        { Vector2.down, 3 },
        { Vector2.left, 4 }
    };
    void Update() {
        Vector2 newAnimationDirection;
        if (heading == Vector2.zero) {
            newAnimationDirection = Vector2.zero;
        } else if (Mathf.Abs(heading.x) > Mathf.Abs(heading.y)) {
            newAnimationDirection = heading.x > 0 ? Vector2.right : Vector2.left;
        } else {
            newAnimationDirection = heading.y > 0 ? Vector2.up : Vector2.down;
        }

        if (newAnimationDirection != animationDirection) {
            animationDirection = newAnimationDirection;
            animator.SetInteger("Direction", cardinalIntMappings[animationDirection]);
        }
    }

    void FixedUpdate() {
        Vector2 targetVel = heading * moveSpeed;
        Vector2 velDelta = targetVel - rb.velocity;
        Vector2 requiredAccel = velDelta / Time.fixedDeltaTime;

        float maxForce = rb.mass * 9.81f;
        rb.AddForce(Vector2.ClampMagnitude(requiredAccel * rb.mass, maxForce));
    }

    public void SetAttackDamage(float damage) {
        attackDamage = damage;
    }

    public void SetMaxHealth(float health) {
        damagable.SetMaxHealth(health);
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.TryGetComponent<Damagable>(out Damagable targetDamagable) && col.gameObject.GetComponent<Wolf>() == null && Time.time >= lastHitTime + attackCooldownSec) {
            targetDamagable.Hit(transform.position, attackDamage, 100f);
            lastHitTime = Time.time;
        }
        // if (col.gameObject.layer == LayerMask.NameToLayer("WildSheep"))
        // {
        //     SheepManager.Instance.KillSheep(col.gameObject);
        // }
    }

    IEnumerator ScanForSheep() {
        while (true)
        {
            Collider2D[] sheepInRange = Physics2D.OverlapCircleAll(transform.position, detectionRadius, sheepLayer);
            if (sheepInRange.Length > 0)
            {
                Collider2D closestSheep = sheepInRange.OrderBy(sheep => Vector2.Distance(transform.position, sheep.transform.position)).First();
                heading = (closestSheep.transform.position - transform.position).normalized;
            } else {
                heading =  Random.insideUnitCircle.normalized;
            }
            yield return new WaitForSeconds(Random.Range(waitMin, waitMax));
        }
    }
}
