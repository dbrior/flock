using UnityEngine;

public class SheepWander : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float minWaitTime = 1f;
    public float maxWaitTime = 3f;
    public float moveRadius = 5f;

    private Vector2 targetPosition;
    private bool isWaiting = false;
    private Rigidbody2D rb;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        SetNewTargetPosition();
    }

    void Update()
    {
        if (!isWaiting)
        {
            MoveToTarget();
        }
    }

    private void MoveToTarget()
    {
        rb.AddForce(targetPosition);
        StartCoroutine(WaitBeforeMoving());
    }

    private void SetNewTargetPosition()
    {
        float randomX = Random.Range(-moveRadius, moveRadius);
        float randomY = Random.Range(-moveRadius, moveRadius);
        targetPosition = new Vector2(randomX, randomY);
    }

    private System.Collections.IEnumerator WaitBeforeMoving()
    {
        isWaiting = true;
        float waitTime = Random.Range(minWaitTime, maxWaitTime);
        yield return new WaitForSeconds(waitTime);
        SetNewTargetPosition();
        isWaiting = false;
    }
}
