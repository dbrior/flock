using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMover : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 heading;
    private Vector2 animationDirection;

    [SerializeField] private float moveSpeed;
    [SerializeField] private bool hasTarget;
    [SerializeField] private float waitMin;
    [SerializeField] private float waitMax;

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
    }

    void Start()
    {
        StartCoroutine(ScanForTarget());
    }

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

    IEnumerator ScanForTarget() {
        while (true)
        {
            // if (hasTarget) {
            //     Collider2D[] targetsInRange = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
            //     if (targetsInRange.Length > 0)
            //     {
            //         Collider2D closestSheep = sheepInRange.OrderBy(sheep => Vector2.Distance(transform.position, sheep.transform.position)).First();
            //         heading = (closestSheep.transform.position - transform.position).normalized;
            //     } else {
            //         heading =  Random.insideUnitCircle.normalized;
            //     }
            // }
            heading =  Random.insideUnitCircle.normalized;
            yield return new WaitForSeconds(Random.Range(waitMin, waitMax));
        }
    }
}
