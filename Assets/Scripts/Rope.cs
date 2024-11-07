using UnityEngine;
using System.Collections.Generic;

public class Rope : MonoBehaviour
{
    public static Rope Instance { get; private set; }
    [SerializeField] private Transform player1;
    [SerializeField] private Transform player2;
    [SerializeField] private GameObject ropeSegmentPrefab;
    [SerializeField] private int maxSegmentCount;
    [SerializeField] private float segmentLength;

    private float currentSegmentCount;
    private List<GameObject> ropeSegments = new List<GameObject>();

    void Awake() {
        if (Instance == null){Instance = this;} 
        else {Destroy(gameObject);}
    }

    void Start()
    {
        currentSegmentCount = maxSegmentCount;
        GenerateRope();
    }

    public void AddSegment() {
        if (currentSegmentCount < maxSegmentCount) {
            currentSegmentCount += 1;
            RedrawRope();
        }
    }

    public void RemoveSegment() {
        if (currentSegmentCount >= 0) {
            currentSegmentCount -= 1;
            RedrawRope();
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

    void GenerateRope()
    {
        Vector3 ropeStartPoint = player1.position;
        Vector3 ropeEndPoint = player2.position;
        Vector3 direction = (ropeEndPoint - ropeStartPoint).normalized;

        // Calculate total rope length
        float ropeLength = Vector3.Distance(ropeStartPoint, ropeEndPoint);

        // Adjust segment count based on rope length and segment length
        int calculatedSegmentCount = Mathf.RoundToInt(ropeLength / segmentLength);

        Vector3 segmentPosition = ropeStartPoint;

        GameObject previousSegment = null;

        for (int i = 0; i < currentSegmentCount; i++)
        {
            GameObject segment = Instantiate(ropeSegmentPrefab, segmentPosition, Quaternion.identity, transform);
            ropeSegments.Add(segment);

            HingeJoint2D joint = segment.GetComponent<HingeJoint2D>();
            Rigidbody2D rb = segment.GetComponent<Rigidbody2D>();
            joint.autoConfigureConnectedAnchor = false;
            joint.anchor = new Vector2(0, 0);
            // joint.breakForce = 100f;

            if (previousSegment == null)
            {
                joint.connectedBody = player1.GetComponent<Rigidbody2D>();
                joint.connectedAnchor = new Vector2(0, 0);
            }
            else
            {
                joint.connectedBody = previousSegment.GetComponent<Rigidbody2D>();
                joint.connectedAnchor = new Vector2(segmentLength, 0);
            }

            previousSegment = segment;
            segmentPosition += direction * segmentLength;

            // if (i == Mathf.Ceil(segmentCount/2)) {
            //     rb.mass = 50f;
            //     rb.drag = 10f;
            //     rb.angularDrag = 10f;
            // }
        }

        HingeJoint2D lastHingeJoint = previousSegment.AddComponent<HingeJoint2D>();
        lastHingeJoint.connectedBody = player2.GetComponent<Rigidbody2D>();
        lastHingeJoint.autoConfigureConnectedAnchor = false;
        lastHingeJoint.anchor = new Vector2(segmentLength, 0);
        lastHingeJoint.connectedAnchor = new Vector2(0, 0);
    }

    // void FixedUpdate() {
    //     // Debug.Log(ropeSegments[2].GetComponent<HingeJoint2D>().reactionForce);
    // }
}