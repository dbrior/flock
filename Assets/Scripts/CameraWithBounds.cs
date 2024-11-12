using UnityEngine;

public class CameraWithBounds : MonoBehaviour
{
    public Transform player1; // First Player Transform to follow
    public Transform player2; // Second Player Transform to follow
    public Vector2 toleranceBounds = new Vector2(2f, 2f); // The tolerance bounds around the camera's center
    public Vector2 maxDistanceFromOrigin = new Vector2(10f, 10f); // Maximum allowed distance from origin (0,0) on each axis

    private Vector3 offset;

    void Start()
    {
        // Calculate the initial offset between the camera and the midpoint of the two players
        offset = transform.position - GetMidpoint(player1.position, player2.position);
    }

    void LateUpdate()
    {
        // Get the midpoint between the two players
        Vector3 midpoint = GetMidpoint(player1.position, player2.position);
        Vector3 targetPos = transform.position;

        // Calculate the difference between each player position and the camera position
        Vector3 player1Diff = player1.position - transform.position;
        Vector3 player2Diff = player2.position - transform.position;

        // Check if player1 is outside of tolerance bounds and adjust the camera accordingly
        if (Mathf.Abs(player1Diff.x) > toleranceBounds.x)
        {
            targetPos.x = player1.position.x - Mathf.Sign(player1Diff.x) * toleranceBounds.x;
        }

        if (Mathf.Abs(player1Diff.y) > toleranceBounds.y)
        {
            targetPos.y = player1.position.y - Mathf.Sign(player1Diff.y) * toleranceBounds.y;
        }

        // Check if player2 is outside of tolerance bounds and adjust the camera accordingly
        if (Mathf.Abs(player2Diff.x) > toleranceBounds.x)
        {
            targetPos.x = player2.position.x - Mathf.Sign(player2Diff.x) * toleranceBounds.x;
        }

        if (Mathf.Abs(player2Diff.y) > toleranceBounds.y)
        {
            targetPos.y = player2.position.y - Mathf.Sign(player2Diff.y) * toleranceBounds.y;
        }

        // Constrain the target position to the maximum distance from the origin (0,0) on each axis
        targetPos.x = Mathf.Clamp(targetPos.x, -maxDistanceFromOrigin.x, maxDistanceFromOrigin.x);
        targetPos.y = Mathf.Clamp(targetPos.y, -maxDistanceFromOrigin.y, maxDistanceFromOrigin.y);

        // Update the camera position smoothly to follow the target
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 3f);
    }

    // Helper method to get the midpoint between two positions
    private Vector3 GetMidpoint(Vector3 pos1, Vector3 pos2)
    {
        return (pos1 + pos2) / 2f;
    }
}