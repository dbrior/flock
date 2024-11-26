using UnityEngine;
using System.Collections.Generic;

public class Rope : MonoBehaviour
{
    public static Rope Instance { get; private set; }
    [SerializeField] private Rigidbody2D player1AnchorPoint;
    [SerializeField] private Rigidbody2D player2AnchorPoint;
    [SerializeField] private GameObject ropeSegmentPrefab;
    [SerializeField] private int maxSegmentCount;
    [SerializeField] private float segmentLength;

    private HingeJoint2D player1Segment;
    private HingeJoint2D player2Segment;

    private float currentSegmentCount;
    private List<GameObject> ropeSegments = new List<GameObject>();

    [SerializeField] private float ropeSlideSpeed;
    private bool shrinking;
    private bool expanding;

    void Awake() {
        if (Instance == null){Instance = this;} 
        else {Destroy(gameObject);}
    }

    void Start()
    {
        currentSegmentCount = maxSegmentCount;
        GenerateRope();
    }

    void FixedUpdate() {
        if (shrinking) {
            float newAnchorDistance = player1Segment.anchor.x + (ropeSlideSpeed * Time.fixedDeltaTime);
            if (newAnchorDistance >= segmentLength) {
                if (ropeSegments.Count > 1) {
                    AttachToPlayer1(ropeSegments[1]);
                }
                Destroy(ropeSegments[0]);
                ropeSegments.RemoveAt(0);
                shrinking = false;
                currentSegmentCount -= 1;
            } else {
                player1Segment.anchor = new Vector2(newAnchorDistance, 0);
            }
        } else if (expanding) {
            if (currentSegmentCount == 0) {
                SpawnSegment();
                expanding = false;
            } else if (player1Segment.anchor.x == 0) {
                SpawnSegment();
            } else {
                float newAnchorDistance = player1Segment.anchor.x - (ropeSlideSpeed * Time.fixedDeltaTime);
                if (newAnchorDistance <= 0) {
                    player1Segment.anchor = new Vector2(0, 0);
                    expanding = false;
                } else {
                    player1Segment.anchor = new Vector2(newAnchorDistance, 0);
                }
            }
        }
    }

    private void SpawnSegment() {
        Vector2 spawnPosition = new Vector2(player1AnchorPoint.transform.position.x - segmentLength, player1AnchorPoint.transform.position.y);

        if (currentSegmentCount > 0) {
            GameObject segment = Instantiate(ropeSegmentPrefab, spawnPosition, player1Segment.transform.rotation, transform);
            // player1Segment.gameObject.AddComponent<HingeJoint2D>();
            AttatchToSegment(segment, player1Segment.gameObject);
            ropeSegments.Insert(0, segment);
            AttachToPlayer1(segment);
            player1Segment.anchor = new Vector2(segmentLength, 0);
        } else {
            GameObject segment = Instantiate(ropeSegmentPrefab, spawnPosition, Quaternion.identity, transform);
            ropeSegments.Insert(0, segment);
            AttachToPlayer1(segment);
            AttachToPlayer2(segment);
        }
        currentSegmentCount += 1;
    }

    public void RemoveSegment() {
        shrinking = true;
    }

    public void AddSegment() {
        if (currentSegmentCount < maxSegmentCount) {
            expanding = true;
        }
    }

    public void AdjustMaxSegments(int delta) {
        maxSegmentCount += delta;
    }

    public void RedrawRope() {
        DestroyRope();
        if (currentSegmentCount > 0) {
            GenerateRope();
        }
    }

    void DestroyRope() {
        for (int i=0; i<ropeSegments.Count; i++) {
            Destroy(ropeSegments[i]);
        }
        ropeSegments.Clear();
    }

    void AttachToPlayer1(GameObject segment) {
        HingeJoint2D joint = segment.GetComponent<HingeJoint2D>();
        joint.autoConfigureConnectedAnchor = false;
        joint.anchor = new Vector2(0, 0);
        joint.connectedBody = player1AnchorPoint;
        joint.connectedAnchor = new Vector2(0, 0);
        player1Segment = joint;
    }

    void AttachToPlayer2(GameObject segment) {
        HingeJoint2D lastJoint = segment.AddComponent<HingeJoint2D>();
        lastJoint.connectedBody = player2AnchorPoint;
        lastJoint.autoConfigureConnectedAnchor = false;
        lastJoint.anchor = new Vector2(segmentLength, 0);
        lastJoint.connectedAnchor = new Vector2(0, 0);
        player2Segment = lastJoint;
    }

    void AttatchToSegment(GameObject segment1, GameObject segment2) {
        HingeJoint2D joint = segment2.GetComponent<HingeJoint2D>();
        joint.autoConfigureConnectedAnchor = false;
        joint.anchor = new Vector2(0, 0);
        joint.connectedBody = segment1.GetComponent<Rigidbody2D>();
        joint.connectedAnchor = new Vector2(segmentLength, 0);
    }

    void GenerateRope()
    {
        Vector3 ropeStartPoint = player1AnchorPoint.transform.position;
        Vector3 ropeEndPoint = player2AnchorPoint.transform.position;
        Vector3 direction = (ropeEndPoint - ropeStartPoint).normalized;

        Vector3 segmentPosition = ropeStartPoint;

        GameObject previousSegment = null;

        for (int i = 0; i < currentSegmentCount; i++)
        {
            GameObject segment = Instantiate(ropeSegmentPrefab, segmentPosition, Quaternion.identity, transform);
            ropeSegments.Add(segment);

            if (previousSegment == null)
            {
                AttachToPlayer1(segment);
            }
            else
            {
                AttatchToSegment(previousSegment, segment);
            }

            previousSegment = segment;
            segmentPosition += direction * segmentLength;
        }

        AttachToPlayer2(previousSegment);
    }

    private bool isAttached = false;
    private HingeJoint2D attachmentHingeJoint;
    public void OnAttachRope() {
        // Get the last segment
        GameObject lastSegment = ropeSegments[ropeSegments.Count - 1];

        if (!isAttached)
        {
            Collider2D lastSegmentCollider = lastSegment.GetComponent<Collider2D>();

            // Create a ContactFilter2D to find overlaps with other rope segments
            ContactFilter2D contactFilter = new ContactFilter2D();
            contactFilter.SetLayerMask(LayerMask.GetMask("Rope")); // Ensure "Rope" layer exists and is assigned to rope segments
            contactFilter.useTriggers = false;

            // Create an array to store the results
            Collider2D[] results = new Collider2D[10];
            int overlapCount = lastSegmentCollider.OverlapCollider(contactFilter, results);

            bool attached = false;

            for (int i = 0; i < overlapCount; i++)
            {
                Collider2D otherCollider = results[i];
                if (otherCollider.gameObject != lastSegment && otherCollider.gameObject != ropeSegments[ropeSegments.Count - 2])
                {
                    // Found an overlapping rope segment
                    // Get the approximate overlap point
                    Vector2 overlapPoint = (lastSegment.transform.position + otherCollider.transform.position) / 2;

                    // Create a hinge joint at the overlap point
                    HingeJoint2D hinge = lastSegment.AddComponent<HingeJoint2D>();
                    hinge.connectedBody = otherCollider.attachedRigidbody;
                    hinge.autoConfigureConnectedAnchor = false;
                    // hinge.anchor = lastSegment.transform.InverseTransformPoint(overlapPoint);
                    hinge.anchor = new Vector2(segmentLength, 0);
                    hinge.connectedAnchor = otherCollider.transform.InverseTransformPoint(overlapPoint);

                    isAttached = true;
                    attachmentHingeJoint = hinge;
                    attached = true;

                    Debug.Log("Rope attached at overlap point.");

                    break; // Exit the loop after attaching
                }
            }

            if (!attached)
            {
                Debug.Log("No overlapping rope segments found to attach.");
            }
        }
        else
        {
            // Remove the hinge joint from the last segment
            if (attachmentHingeJoint != null)
            {
                Destroy(attachmentHingeJoint);
                attachmentHingeJoint = null;
                isAttached = false;
                Debug.Log("Rope detached.");
            }
        }
    }
}