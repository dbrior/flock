using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class CharacterMover : MonoBehaviour
{
    // [SerializeField] private float moveSpeed;
    [SerializeField] private float pathfindingIntervalSec = 3f;

    // Wandering
    [SerializeField] private bool shouldWander;
    [SerializeField] private bool anchoredWandering;
    [SerializeField] private Transform wanderingAnchor;
    [SerializeField] private float wanderRadius;
    [SerializeField] private FloatRange wanderingWaitTimeSec;
    private bool wandering;

    public Action onReachDestination;
    public Action onAbandonDestination;
    
    Coroutine navCoroutine;
    private NavMeshAgent agent;
    private bool shouldNavigate;

    void Awake() {
        agent = GetComponent<NavMeshAgent>();
        shouldNavigate = false;
    }

    void Start() {
        if (shouldWander) StartWandering();
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

        if (shouldWander) StartWandering();
    }

    public void CancelNavigation() {
        Debug.Log("Navigation cancelled");
        StopCoroutine("Navigate");
        shouldNavigate = false;

        onAbandonDestination?.Invoke();

        onReachDestination = null;
        onAbandonDestination = null;

        if (shouldWander) StartWandering();
    }

    public bool TryNavigateTo(Vector3 worldPosition) {
        StopWandering();
        StopCoroutine("Navigate");
        shouldNavigate = true;
        return agent.SetDestination(worldPosition);
    }
    public void NavigateTo(Vector3 worldPosition) {
        StopWandering();
        StopCoroutine("Navigate");
        shouldNavigate = true;
        agent.SetDestination(worldPosition);
    }
    public void NavigateTo(Transform targetTransform) {
        StopWandering();
        StopCoroutine("Navigate");
        shouldNavigate = true;
        StartCoroutine(Navigate(targetTransform));
    }

    IEnumerator Navigate(Transform target) {
        while (shouldNavigate) {
            if (target == null) {
                CancelNavigation();
                break;
            };

            agent.SetDestination(target.position);
            
            yield return new WaitForSeconds(pathfindingIntervalSec);
        }
    }

    // -------- WANDERING -------- //

    public void SetWanderAnchor(Transform anchor) {
        wanderingAnchor = anchor;
        anchoredWandering = true;
    }

    public bool ShouldWander() {
        return shouldWander;
    }

    public void EnableWandering() {
        shouldWander = true;
    }

    public void DisableWandering() {
        shouldWander = false;
        StopCoroutine("Wander");
    }

    public void StartWandering() {
        wandering = true;
        StartCoroutine("Wander");
    }

    public void StopWandering() {
        wandering = false;
        StopCoroutine("Wander");
    }

    private void AttemptWander() {
        Vector3 wanderPoint = (UnityEngine.Random.insideUnitSphere * wanderRadius) + (anchoredWandering ? wanderingAnchor.position : transform.position);
        agent.SetDestination(wanderPoint);
    }

    IEnumerator Wander() {
        while (true) {
            if (wandering) AttemptWander();
            yield return new WaitForSeconds(UnityEngine.Random.Range(wanderingWaitTimeSec.min, wanderingWaitTimeSec.max));
        }
    }
}
