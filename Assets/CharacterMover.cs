using System.Collections.Generic;
using UnityEngine;
using System;

public class CharacterMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private bool flippingSprite = false;
    [SerializeField] private float distanceTolerance;
    public Action onReachDestination;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool hasDestination;
    private Vector3[] points;
    private int pointIdx;
    private Vector2 heading;
    private Vector2 animationDirection;

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
        hasDestination = false;

        rb.drag = 10f;
    }

    void Update() {
        // Handle movement animations
        if (flippingSprite) {
            spriteRenderer.flipX = heading.x < 0 ? true : false;
        } else {
            // Determine required animation direction
            Vector2 newAnimationDirection;
            if (heading == Vector2.zero) {
                newAnimationDirection = Vector2.zero;
            } else if (Mathf.Abs(heading.x) > Mathf.Abs(heading.y)) {
                newAnimationDirection = heading.x > 0 ? Vector2.right : Vector2.left;
            } else {
                newAnimationDirection = heading.y > 0 ? Vector2.up : Vector2.down;
            }

            // Update animation direction if needed
            if (newAnimationDirection != animationDirection) {
                animationDirection = newAnimationDirection;
                animator.SetInteger("Direction", cardinalIntMappings[animationDirection]);
            }
        }
    }

    void FixedUpdate() {
        if (!hasDestination || points == null || points.Length == 0) return;

        // Move to next waypoint if reached current
        Vector2 distance = (Vector2) points[pointIdx] - (Vector2) transform.position;
        if (distance.magnitude <= distanceTolerance) {
            NextWaypoint();
        }

        // If we still have a destination, move in direction of next waypoin
        if (hasDestination) {
            distance = (Vector2) points[pointIdx] - (Vector2) transform.position;
            heading = distance.normalized;
            Vector2 targetVel = heading * moveSpeed;
            Vector2 velDelta = targetVel - rb.velocity;
            Vector2 requiredAccel = velDelta / Time.fixedDeltaTime;

            rb.AddForce(requiredAccel * rb.mass);
        }
    }

    public void SetPoints(Vector3[] newPoints) {
        points = newPoints;
        pointIdx = 0;
        hasDestination = true;
    }

    private void NextWaypoint() {
        pointIdx += 1;
        if (pointIdx >= points.Length) {
            ReachedDestination();
        }
    }

    private void ReachedDestination() {
        hasDestination = false;
        heading = Vector2.zero;
        rb.velocity = Vector2.zero;
        onReachDestination?.Invoke();
    }
}
