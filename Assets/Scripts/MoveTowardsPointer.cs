using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowardsPointer : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float minDistance;
    [SerializeField] private float damping;

    private Rigidbody2D rb;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
        Vector2 targetDirection = (Vector2)(Pointer.Instance.transform.position - transform.position);
        float distance = targetDirection.magnitude;

        if (distance > minDistance) {
            Vector2 desiredVelocity = targetDirection.normalized * moveSpeed;
            Vector2 smoothVelocity = Vector2.Lerp(rb.velocity, desiredVelocity, Time.fixedDeltaTime * damping);
            rb.velocity = smoothVelocity;
        } else {
            rb.velocity = Vector2.zero;
        }
    }

}
