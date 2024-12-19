using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

[System.Serializable]
public struct PathfindingInterval {
    public float dist;
    public float intervalSec;

    public PathfindingInterval(float dist, float intervalSec) {
        this.dist = dist;
        this.intervalSec = intervalSec;
    }
}

public class CharacterMover : MonoBehaviour
{
    // [SerializeField] private float moveSpeed;
    [SerializeField] private float pathfindingIntervalSec = 3f;
    [SerializeField] private PathfindingInterval[] pathfindingIntervals; // Assume sorted descending

    // Wandering
    [SerializeField] private bool anchoredWandering;
    [SerializeField] private Transform wanderingAnchor;
    [SerializeField] private float wanderRadius;
    [SerializeField] private FloatRange wanderingWaitTimeSec;
    private Coroutine wanderingCoroutine;

    public Action onReachDestination;
    public Action onAbandonDestination;
    
    private NavMeshAgent agent;
    private bool shouldNavigate;
    [SerializeField] private Transform currentTarget;

    void Awake() {
        agent = GetComponent<NavMeshAgent>();
        shouldNavigate = false;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update() {
        if(shouldNavigate && IsAtDestination()) {
            ReachedDestination();
        }
    }

    private bool IsAtDestination() {
        if (agent.pathPending) {
            return Vector2.Distance(transform.position, agent.pathEndPosition) <= agent.stoppingDistance;
        } else {
            return (agent.remainingDistance <= agent.stoppingDistance);
        }
    }

    private void ReachedDestination() {
        shouldNavigate = false;

        onReachDestination?.Invoke();

        onReachDestination = null;
        onAbandonDestination = null;
        currentTarget = null;
    }

    public void SetPathfindingInterval(float newIntervalSec) {
        pathfindingIntervalSec = newIntervalSec;
    }

    public Transform GetCurrentTarget() {
        return currentTarget;
    }

    public void CancelNavigation() {
        StopCoroutine("Navigate");
        shouldNavigate = false;

        onAbandonDestination?.Invoke();

        onReachDestination = null;
        onAbandonDestination = null;
        currentTarget = null;
    }

    public bool TryNavigateTo(Vector3 worldPosition) {
        StopWandering();
        StopCoroutine("Navigate");
        // shouldNavigate = true;
        return agent.SetDestination(worldPosition);
    }
    public void NavigateTo(Vector3 worldPosition) {
        StopWandering();
        StopCoroutine("Navigate");
        // shouldNavigate = true;
        agent.SetDestination(worldPosition);
    }
    public void NavigateTo(Transform targetTransform) {
        if (targetTransform == currentTarget) return;
        StopWandering();
        currentTarget = targetTransform;
        if (!shouldNavigate) {
            shouldNavigate = true;
            StartCoroutine("Navigate");
        }
    }

    IEnumerator Navigate() {
        while (shouldNavigate) {
            if (currentTarget == null) {
                CancelNavigation();
                break;
            }

            agent.SetDestination(currentTarget.position);
    
            float elapsedTime = 0;
            while (elapsedTime < pathfindingIntervalSec) {
                if (currentTarget == null) {
                    CancelNavigation();
                    break;
                }

                float distance = Vector2.Distance(transform.position, currentTarget.position);
                for (int i=0; i<pathfindingIntervals.Length; i++) {
                    if (distance >= pathfindingIntervals[i].dist) {
                        pathfindingIntervalSec = pathfindingIntervals[i].intervalSec;
                        break;
                    }
                }

                yield return new WaitForSeconds(0.1f);
                elapsedTime += 0.1f;
            }
        }
    }

    // -------- WANDERING -------- //

    public void SetWanderAnchor(Transform anchor) {
        wanderingAnchor = anchor;
        anchoredWandering = true;
    }

    public void StartWandering() {
        if (wanderingCoroutine != null) return;

        wanderingCoroutine = StartCoroutine("Wander");
    }

    public void StopWandering() {
        StopCoroutine("Wander");
        wanderingCoroutine = null;
    }

    private void AttemptWander() {
        Vector3 wanderPoint = (UnityEngine.Random.insideUnitSphere * wanderRadius) + (anchoredWandering ? wanderingAnchor.position : transform.position);
        agent.SetDestination(wanderPoint);
    }

    IEnumerator Wander() {
        while (true) {
            AttemptWander();
            yield return new WaitForSeconds(UnityEngine.Random.Range(wanderingWaitTimeSec.min, wanderingWaitTimeSec.max));
        }
    }
}
