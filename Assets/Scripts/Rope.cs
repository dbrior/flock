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

    private DistanceJoint2D player1Segment;
    private DistanceJoint2D player2Segment;

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
            // Replace distance joint with hinge joint
            player1Segment.gameObject.AddComponent<HingeJoint2D>();
            AttatchToSegment(segment, player1Segment.gameObject);
            Destroy(player1Segment);
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
        Destroy(segment.GetComponent<HingeJoint2D>());
        DistanceJoint2D joint = segment.AddComponent<DistanceJoint2D>();
        joint.autoConfigureConnectedAnchor = false;
        joint.anchor = new Vector2(0, 0);
        joint.connectedBody = player1AnchorPoint;
        joint.connectedAnchor = new Vector2(0, 0);
        joint.autoConfigureDistance = false;
        joint.distance = 0;
        player1Segment = joint;
    }

    void AttachToPlayer2(GameObject segment) {
        DistanceJoint2D lastJoint = segment.AddComponent<DistanceJoint2D>();
        lastJoint.connectedBody = player2AnchorPoint;
        lastJoint.autoConfigureConnectedAnchor = false;
        lastJoint.anchor = new Vector2(segmentLength, 0);
        lastJoint.connectedAnchor = new Vector2(0, 0);
        lastJoint.autoConfigureDistance = false;
        lastJoint.distance = 0;
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

    // void FixedUpdate() {
    //     // Debug.Log(ropeSegments[2].GetComponent<HingeJoint2D>().reactionForce);
    // }
}