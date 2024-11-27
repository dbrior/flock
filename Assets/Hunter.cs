using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hunter : MonoBehaviour
{
    [SerializeField] private LayerMask wolfLayer;
    [SerializeField] private float detectionRadius;
    [SerializeField] private float moveSpeed;
    private RotateToFaceTarget tracker;
    public float fireHz;

    private ToolBelt toolBelt;
    private Transform targetTransform;
    private bool hasTarget;
    private Rigidbody2D rb;
    public Transform player;
    private Vector2 positionOffset;
    void Start()
    {
        hasTarget = false;
        toolBelt = GetComponent<ToolBelt>();
        tracker = GetComponent<RotateToFaceTarget>();
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(ScanForEnemies());
        StartCoroutine(Attack());
        StartCoroutine(Bob());
        // player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private Vector2 currentVelocity;

    void FixedUpdate()
    {
        // Smoothly damp the hunter's position towards the player's position
        Vector2 targetPosition = (Vector2) player.position + positionOffset;
        Vector2 smoothedPosition = Vector2.SmoothDamp(transform.position, targetPosition, ref currentVelocity, 0.75f, moveSpeed, Time.fixedDeltaTime);

        rb.MovePosition(smoothedPosition);

        Debug.Log(player.position);
        Debug.Log(rb.velocity);
    }


    IEnumerator ScanForEnemies() {
        while (true)
        {
            Collider2D[] wolvesInRange = Physics2D.OverlapCircleAll(transform.position, detectionRadius, wolfLayer);
            if (wolvesInRange.Length > 0)
            {
                Collider2D closestWolf = wolvesInRange.OrderBy(wolf => Vector2.Distance(transform.position, wolf.transform.position)).First();
                hasTarget = true;
                tracker.target = closestWolf.transform;
                targetTransform = closestWolf.transform;
            } else {
                hasTarget = false;
            }
            yield return new WaitForSeconds(2f);
        }
    }

    IEnumerator Attack() {
        while (true)
        {
            if (hasTarget) {
                if (targetTransform != null) {
                    toolBelt.UseSlingshotAtTarget((Vector2) targetTransform.position + targetTransform.gameObject.GetComponent<Rigidbody2D>().velocity/2f);
                } else {
                    hasTarget = false;
                }
            }
            yield return new WaitForSeconds(1f/fireHz);
        }
    }

    IEnumerator Bob() {
        while (true)
        {
            positionOffset = UnityEngine.Random.insideUnitCircle.normalized * Random.Range(0, 1.75f);
            yield return new WaitForSeconds(Random.Range(1.5f, 3f));
        }
    }
}
