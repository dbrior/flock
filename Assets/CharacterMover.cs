using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class CharacterMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private bool flippingSprite = false;
    [SerializeField] private float distanceTolerance;
    [SerializeField] private float pathfindingIntervalSec = 3f;

    // Wandering
    [SerializeField] private bool shouldWander;
    [SerializeField] private bool anchoredWandering;
    [SerializeField] private Transform wanderingAnchor;
    [SerializeField] private float wanderRadius;
    [SerializeField] private FloatRange wanderingWaitTimeSec;
    private bool wandering;

    public Action onReachDestination;
    private Transform navTransform;
    private NavMeshPath path;
    Coroutine navCoroutine;

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
        path = new NavMeshPath();
        rb.drag = 10f;
    }

    void Start() {
        if (shouldWander) {
            wandering = true;
            StartCoroutine("Wander");
        }
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
        if (!hasDestination || points == null || points.Length == 0) {
            heading = Vector2.zero;
            return;
        }

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

        if (shouldWander) {
            EnableWander();
        }
    }

    private Vector3[] GeneratePointsToTarget(Vector3 targetPosition) {
        if(NavMesh.CalculatePath(transform.position, targetPosition, NavMesh.AllAreas, path)) {
            return path.corners;
        } else {
            return null;
        }
    }

    public void StopNavigation() {
        if (navCoroutine != null) {
            StopCoroutine("Navigate");
        }
        points = null;
        hasDestination = false;

        if (shouldWander) {
            EnableWander();
        }
    }

    // One time attempt at navigating
    public bool TryNavigateTo(Vector3 worldPosition) {
        Vector3[] waypoints = GeneratePointsToTarget(worldPosition);
        if (waypoints == null) return false;

        StopNavigation();
        DisableWander();
        hasDestination = true;
        navTransform = null;
        SetPoints(waypoints);
        return true;
    }
    public bool TryNavigateTo(Transform targetTransform) {
        Vector3[] waypoints = GeneratePointsToTarget(targetTransform.position);
        if (waypoints == null) return false;

        StopNavigation();
        DisableWander();
        hasDestination = true;
        navTransform = targetTransform;
        SetPoints(waypoints);
        return true;
    }

    // Lock and retry destination
    public void NavigateTo(Vector3 worldPosition) {
        StopNavigation();
        DisableWander();
        hasDestination = true;
        navTransform = null;
        navCoroutine = StartCoroutine(Navigate(worldPosition));
    }
    public void NavigateTo(Transform targetTransform) {
        StopNavigation();
        DisableWander();
        hasDestination = true;
        navTransform = targetTransform;
        navCoroutine = StartCoroutine(Navigate(targetTransform));
    }

    private void OnCollisionEnter2D(Collision2D col) {
        if (hasDestination && col.transform == navTransform) {
            ReachedDestination();
        }
    }

    IEnumerator Navigate(Vector3 position) {
        while (hasDestination) {
            if (Vector3.Distance(position, transform.position) <= distanceTolerance) {
                break;
            }

            Vector3[] waypoints = GeneratePointsToTarget(position);
            if (waypoints != null) {
                SetPoints(waypoints);
            }

            yield return new WaitForSeconds(pathfindingIntervalSec);
        }
    }

    IEnumerator Navigate(Transform target) {
        while (hasDestination) {
            if (Vector3.Distance(target.position, transform.position) <= distanceTolerance) {
                break;
            }

            Vector3[] waypoints = GeneratePointsToTarget(target.position);
            if (waypoints != null) {
                SetPoints(waypoints);
            }
            
            yield return new WaitForSeconds(pathfindingIntervalSec);
        }
    }

    public void DisableWander() {
        wandering = false;
    }

    public void EnableWander() {
        wandering = true;
    }

    private void AttemptWander() {
        // Make 3 attempts to wander
        int WANDER_ATTEMPTS = 3;

        for (int i=0; i<WANDER_ATTEMPTS; i++) {
            NavMeshHit hit;
            Vector2 wanderPoint = (UnityEngine.Random.insideUnitSphere * wanderRadius) + (anchoredWandering ? wanderingAnchor.position : transform.position);

            if (NavMesh.SamplePosition(wanderPoint, out hit, 0.08f, NavMesh.AllAreas)) {
                bool success = TryNavigateTo(hit.position);
                if (success) break;
            }
        }
    }

    IEnumerator Wander() {
        while (true) {
            if (wandering) AttemptWander();
            yield return new WaitForSeconds(UnityEngine.Random.Range(wanderingWaitTimeSec.min, wanderingWaitTimeSec.max));
        }
    }
}
