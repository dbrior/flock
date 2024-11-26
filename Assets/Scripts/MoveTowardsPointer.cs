using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowardsPointer : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    // [SerializeField] private float minDistance;
    private Rigidbody2D rb;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
        Vector2 targetDirection = (Vector2) (Pointer.Instance.transform.position - transform.position);
        Vector2 desiredVelocity = targetDirection.normalized * moveSpeed;
        Vector2 currentVelocity = rb.velocity;
        Vector2 deltaVelocity = desiredVelocity - currentVelocity;
        Vector2 force = rb.mass * deltaVelocity / Time.fixedDeltaTime;
        rb.AddForce(force);

        // Vector2 distanceDelta = (Vector2) (Pointer.Instance.transform.position - transform.position);
        // Vector2 targetVel = Mathf.Min()
        // Vector2 targetDirection = distanceDelta.normalized;
        // rb.AddForce(targetDirection * moveForce);
        // if (distanceDelta.magnitude >= minDistance) {
        //     Vector2 targetDirection = distanceDelta.normalized;
        //     rb.AddForce(targetDirection * moveForce);
        // }
    }
}
