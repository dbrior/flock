using UnityEngine;

public class RotateToFaceTarget : MonoBehaviour
{
    public Transform target; // Assign the target in the Inspector
    public float rotationSpeed = 5f; // Adjust for smooth rotation
    public bool useRigidbody = true; // Toggle for Rigidbody2D or Transform rotation

    private Rigidbody2D rb;

    void Start()
    {
        if (useRigidbody)
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                Debug.LogError("No Rigidbody2D found on the object. Please add one or disable 'useRigidbody'.");
            }
        }
    }

    void Update()
    {
        if (target != null)
        {
            // Calculate direction to the target
            Vector3 direction = target.position - transform.position;

            // Get the angle in radians and convert to degrees
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Adjust angle so the bottom of the sprite faces the target
            angle += 90f;

            if (useRigidbody && rb != null)
            {
                // Rotate the Rigidbody2D smoothly
                float smoothedAngle = Mathf.LerpAngle(rb.rotation, angle, Time.deltaTime * rotationSpeed);
                rb.MoveRotation(smoothedAngle);
            }
            else
            {
                // Fallback to Transform rotation
                Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }
    }
}
