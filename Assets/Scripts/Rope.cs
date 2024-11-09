using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;

public class Rope : NetworkBehaviour
{
    public static Rope Instance { get; private set; }
    private NetworkVariable<int> maxSegmentCount = new NetworkVariable<int>(
        5,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );
    private NetworkVariable<int> currentSegmentCount = new NetworkVariable<int>(
        5,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    [SerializeField] private Rigidbody2D player1AnchorPoint;
    [SerializeField] private Rigidbody2D player2AnchorPoint;
    [SerializeField] private GameObject ropeSegmentPrefab;
    [SerializeField] private float segmentLength;

    private HingeJoint2D player1Segment;
    private HingeJoint2D player2Segment;

    
    private List<GameObject> ropeSegments = new List<GameObject>();

    [SerializeField] private float ropeSlideSpeed;
    private bool shrinking;
    private bool expanding;

    // void Awake() {
    //     if (Instance == null){Instance = this;} 
    //     else {Destroy(gameObject);}
    // }

    // void Start()
    // {
    //     currentSegmentCount.Value = maxSegmentCount.Value;
    //     GenerateRope();
    // }

    public override void OnNetworkSpawn() {
        if (Instance == null){Instance = this;} 
        else {Destroy(gameObject);}

        maxSegmentCount.Value = 5;
        currentSegmentCount.Value = maxSegmentCount.Value;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnClientConnected(ulong clientId) {
        Debug.Log("Client " + clientId.ToString() + " connected");
        int clientCount = NetworkManager.Singleton.ConnectedClients.Count;

        if (clientCount == 2)
        {
            Debug.Log("Second player has joined!");
            // InitRopeServerRpc();
            InitRope();
        }
    }

    public void SetPlayer1(Rigidbody2D input) {
        player1AnchorPoint = input;
    }

    public void SetPlayer2(Rigidbody2D input) {
        player2AnchorPoint = input;
    }

    [ServerRpc]
    private void InitRopeServerRpc() {
        foreach (var client in NetworkManager.Singleton.ConnectedClients) {
            ulong clientId = client.Key;
            Rigidbody2D rb = client.Value.PlayerObject.gameObject.GetComponent<Rigidbody2D>();

            if (clientId == NetworkManager.ServerClientId) {
                SetPlayer1(rb);
            } else {
                SetPlayer2(rb);
            }
        }
        Debug.Log("Set Rbs");
        GenerateRope();
        Debug.Log("Generated Rope");
    }
    public void InitRope() {
        foreach (var client in NetworkManager.Singleton.ConnectedClients) {
            ulong clientId = client.Key;
            Rigidbody2D rb = client.Value.PlayerObject.gameObject.GetComponent<Rigidbody2D>();

            if (clientId == NetworkManager.ServerClientId) {
                SetPlayer2(rb);
            } else {
                SetPlayer1(rb);
            }
        }
        Debug.Log("Set Rbs");
        GenerateRope();
        Debug.Log("Generated Rope");
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
                currentSegmentCount.Value -= 1;
            } else {
                player1Segment.anchor = new Vector2(newAnchorDistance, 0);
            }
        } else if (expanding) {
            if (currentSegmentCount.Value == 0) {
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

        if (currentSegmentCount.Value > 0) {
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
        currentSegmentCount.Value += 1;
    }

    public void RemoveSegment() {
        shrinking = true;
    }

    public void AddSegment() {
        if (currentSegmentCount.Value < maxSegmentCount.Value) {
            expanding = true;
        }
    }

    public void AdjustMaxSegments(int delta) {
        maxSegmentCount.Value += delta;
    }

    public void RedrawRope() {
        DestroyRope();
        if (currentSegmentCount.Value > 0) {
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

        for (int i = 0; i < currentSegmentCount.Value; i++)
        {
            // GameObject segment = Instantiate(ropeSegmentPrefab, segmentPosition, Quaternion.identity, transform);
            GameObject segment = Instantiate(ropeSegmentPrefab, segmentPosition, Quaternion.identity);
            segment.GetComponent<NetworkObject>().Spawn();
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
}