using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TargetScanner : MonoBehaviour
{
    private CharacterMover characterMover;

    // Target Aquisition
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float detectionRadius;
    [SerializeField] private float scanIntervalSec;

    // Wandering
    [SerializeField] private bool shouldWander;
    [SerializeField] private bool anchoredWandering;
    [SerializeField] private Transform wanderingAnchor;
    [SerializeField] private float wanderRadius;
    [SerializeField] private FloatRange wanderingWaitTimeSec;
    private bool wandering;

    // Nav Mesh
    private NavMeshPath path;
    private NavMeshObstacle obstacle;

    void Awake() {
        path = new NavMeshPath();
        characterMover = GetComponent<CharacterMover>();
        obstacle = GetComponent<NavMeshObstacle>();
    }

    void Start() {
        if (targetLayer != 0) {
            StartCoroutine("ScanForTarget");
        } else if (shouldWander) {
            wandering = true;
            StartCoroutine("Wander");
        }
    }

    private Vector3[] GeneratePointsToTarget(Vector3 targetPosition) {
        // if (obstacle != null) obstacle.enabled = false;
            
        if(NavMesh.CalculatePath(transform.position, targetPosition, NavMesh.AllAreas, path)) {
            // if (obstacle != null) obstacle.enabled = true;
            return path.corners;
        } else {
            return null;
        }
    }

    private void AttemptWander() {
        // Make 3 attempts to wander
        int WANDER_ATTEMPTS = 3;

        for (int i=0; i<WANDER_ATTEMPTS; i++) {
            NavMeshHit hit;
            Vector2 wanderPoint = (Random.insideUnitSphere * wanderRadius) + (anchoredWandering ? wanderingAnchor.position : transform.position);

            if (NavMesh.SamplePosition(wanderPoint, out hit, 0.08f, NavMesh.AllAreas)) {
                Vector3[] points = GeneratePointsToTarget(hit.position);
                if (points == null) continue;

                if (characterMover != null) {
                    characterMover.SetPoints(points);
                }
                break;
            }
        }
    }


    IEnumerator ScanForTarget() {
        while (true)
        {
            Collider2D[] targetsInRange = Physics2D.OverlapCircleAll(transform.position, detectionRadius, targetLayer);
            if (targetsInRange.Length > 0)
            {
                foreach(Collider2D target in targetsInRange.OrderBy(target => Vector2.Distance(transform.position, target.transform.position)).ToList()) {
                    Vector3[] points = GeneratePointsToTarget(target.transform.position);
                    if (points == null) continue;

                    // Target found
                    StopCoroutine("Wander");
                    wandering = false;
                    characterMover.SetPoints(points);
                    break;
                }
            } else {
                if (shouldWander && !wandering) StartCoroutine("Wander");
                wandering = true;
            }
            yield return new WaitForSeconds(scanIntervalSec);
        }
    }

    IEnumerator Wander() {
        while (true) {
            if (wandering) AttemptWander();
            yield return new WaitForSeconds(Random.Range(wanderingWaitTimeSec.min, wanderingWaitTimeSec.max));
        }
    }
}
