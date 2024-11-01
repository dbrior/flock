using UnityEngine;
using System.Collections.Generic;

public class Rope : MonoBehaviour
{
    public Transform player1;
    public Transform player2;
    public GameObject ropeSegmentPrefab;
    public int segmentCount = 35;
    public float segmentLength = 0.15f;

    private List<GameObject> ropeSegments = new List<GameObject>();

    void Start()
    {
        GenerateRope();
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

        for (int i = 0; i < segmentCount; i++)
        {
            GameObject segment = Instantiate(ropeSegmentPrefab, segmentPosition, Quaternion.identity, transform);
            ropeSegments.Add(segment);

            HingeJoint2D joint = segment.GetComponent<HingeJoint2D>();
            Rigidbody2D rb = segment.GetComponent<Rigidbody2D>();
            joint.autoConfigureConnectedAnchor = false;
            joint.anchor = new Vector2(-segmentLength/2, 0);

            if (previousSegment == null)
            {
                joint.connectedBody = player1.GetComponent<Rigidbody2D>();
                joint.connectedAnchor = new Vector2(0, 0);
            }
            else
            {
                joint.connectedBody = previousSegment.GetComponent<Rigidbody2D>();
                joint.connectedAnchor = new Vector2(segmentLength/2, 0);
            }

            previousSegment = segment;
            segmentPosition += direction * segmentLength;
        }

        DistanceJoint2D distanceJoint = previousSegment.AddComponent<DistanceJoint2D>();
        distanceJoint.autoConfigureConnectedAnchor = false;
        distanceJoint.autoConfigureDistance = false;
        distanceJoint.anchor = new Vector2(segmentLength/2, 0);
        distanceJoint.connectedAnchor = new Vector2(0, 0);
        distanceJoint.distance = 0f;
        distanceJoint.maxDistanceOnly = true;
        distanceJoint.connectedBody = player2.GetComponent<Rigidbody2D>();


        // GameObject lastSegmentChild = new GameObject("LastSegmentJoint");
        // lastSegmentChild.transform.parent = previousSegment.transform;
        // lastSegmentChild.transform.localPosition = Vector3.zero;

        // HingeJoint2D childJoint = lastSegmentChild.AddComponent<HingeJoint2D>();
        // childJoint.connectedBody = player2.GetComponent<Rigidbody2D>();
        // childJoint.autoConfigureConnectedAnchor = false;
        // childJoint.anchor = Vector2.zero;
        // childJoint.connectedAnchor = Vector2.zero;

    }
}