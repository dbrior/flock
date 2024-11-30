using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterMover : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 heading;
    private Vector2 animationDirection;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private float moveSpeed;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float detectionRadius;
    [SerializeField] private float waitMin;
    [SerializeField] private float waitMax;
    [SerializeField] private bool flippingSprite = false;

    private Dictionary<Vector2, int> cardinalIntMappings = new Dictionary<Vector2, int>{
        { Vector2.zero, 0 },
        { Vector2.up, 1 },
        { Vector2.right, 2 },
        { Vector2.down, 3 },
        { Vector2.left, 4 }
    };

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        StartCoroutine(ScanForTarget());
    }

    void Update() {
        if (flippingSprite) {
            spriteRenderer.flipX = heading.x < 0 ? true : false;
        } else {
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
    }

    void FixedUpdate() {
        Vector2 targetVel = heading * moveSpeed;
        Vector2 velDelta = targetVel - rb.velocity;
        Vector2 requiredAccel = velDelta / Time.fixedDeltaTime;

        float maxForce = rb.mass * 9.81f;
        rb.AddForce(Vector2.ClampMagnitude(requiredAccel * rb.mass, maxForce));
    }

    IEnumerator ScanForTarget() {
        while (true)
        {
            if (targetLayer.value == 0) {
                heading =  Random.insideUnitCircle.normalized;
            } else {
                Collider2D[] targetsInRange = Physics2D.OverlapCircleAll(transform.position, detectionRadius, targetLayer);
                if (targetsInRange.Length > 0)
                {
                    Collider2D closestTarget = targetsInRange.OrderBy(target => Vector2.Distance(transform.position, target.transform.position)).First();
                    heading = (closestTarget.transform.position - transform.position).normalized;
                } else {
                    heading =  Random.insideUnitCircle.normalized;
                }
            }
            // if (hasTarget) {
                // Collider2D[] targetsInRange = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
                // if (targetsInRange.Length > 0)
                // {
                //     Collider2D closestSheep = sheepInRange.OrderBy(sheep => Vector2.Distance(transform.position, sheep.transform.position)).First();
                //     heading = (closestSheep.transform.position - transform.position).normalized;
                // } else {
                //     heading =  Random.insideUnitCircle.normalized;
                // }
            // }
            // heading =  Random.insideUnitCircle.normalized;
            yield return new WaitForSeconds(Random.Range(waitMin, waitMax));
        }
    }
}
