using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowardsPointer : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float minDistance;
    [SerializeField] private float damping;

    public bool active = true;

    private Rigidbody2D rb;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
        if (active) {
            float maxDistance = moveSpeed * Time.fixedDeltaTime;
            Vector3 newPosition = Vector3.MoveTowards(transform.position, Pointer.Instance.transform.position, maxDistance);
            rb.MovePosition(newPosition);
        }
    }

    // void FixedUpdate() {
    //     if (active) {
    //         Vector2 targetDirection = (Vector2)(Pointer.Instance.transform.position - transform.position);
    //         float distance = targetDirection.magnitude;

    //         if (distance > minDistance) {
    //             Vector2 desiredVelocity = targetDirection.normalized * moveSpeed;
    //             Vector2 smoothVelocity = Vector2.Lerp(rb.velocity, desiredVelocity, Time.fixedDeltaTime * damping);
    //             rb.velocity = smoothVelocity;
    //         } else {
    //             rb.velocity = Vector2.zero;
    //         }
    //     } else {
    //         rb.velocity = Vector2.zero;
    //     }
    // }

}
