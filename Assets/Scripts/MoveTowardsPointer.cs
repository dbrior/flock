using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowardsPointer : MonoBehaviour
{
    [SerializeField] private float moveForce;
    private Rigidbody2D rb;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
        Vector2 targetDirection = (Vector2) (Pointer.Instance.transform.position - transform.position).normalized;
        rb.AddForce(targetDirection * moveForce);
    }
}
