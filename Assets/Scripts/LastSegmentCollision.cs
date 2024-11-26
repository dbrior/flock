// using UnityEngine;

// public class LastSegmentCollision : MonoBehaviour
// {
//     private bool hingeCreated = false;
//     public Rope rope; // Reference to the Rope script

//     void OnCollisionEnter2D(Collision2D collision)
//     {
//         if (!hingeCreated && collision.gameObject.CompareTag("RopeSegment"))
//         {
//             // Get the collision point
//             ContactPoint2D contactPoint = collision.GetContact(0);
//             Vector2 collisionPoint = contactPoint.point;

//             // Create a hinge joint at the collision point
//             HingeJoint2D hinge = gameObject.AddComponent<HingeJoint2D>();
//             hinge.connectedBody = collision.rigidbody;
//             hinge.autoConfigureConnectedAnchor = false;
//             hinge.anchor = transform.InverseTransformPoint(collisionPoint);
//             hinge.connectedAnchor = collision.transform.InverseTransformPoint(collisionPoint);

//             hingeCreated = true; // Prevent multiple hinge joints from being created

//             // Notify the Rope script that attachment is complete
//             if (rope != null)
//             {
//                 rope.OnRopeAttached(hinge);
//             }
//         }
//     }
// }