using System.Collections;
using System.Linq;
using UnityEngine;

public class Wolf : MonoBehaviour
{
    [SerializeField]
    private LayerMask sheepLayer;
    [SerializeField]
    private float moveSpeed;
    private Vector2 heading;
    [SerializeField]
    private float detectionRadius;
    private Rigidbody2D rb;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        StartCoroutine(ScanForSheep());
    }

    void FixedUpdate() {
        Vector2 targetVel = heading * moveSpeed;
        Vector2 velDelta = targetVel - rb.velocity;
        Vector2 requiredAccel = velDelta / Time.fixedDeltaTime;
        rb.AddForce(requiredAccel * rb.mass);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("WildSheep"))
        {
            SheepManager.Instance.KillSheep(col.gameObject);
        }
    }

    IEnumerator ScanForSheep() {
        while (true)
        {
            Collider2D[] sheepInRange = Physics2D.OverlapCircleAll(transform.position, detectionRadius, sheepLayer);
            if (sheepInRange.Length > 0)
            {
                Collider2D closestSheep = sheepInRange.OrderBy(sheep => Vector2.Distance(transform.position, sheep.transform.position)).First();
                heading = (closestSheep.transform.position - transform.position).normalized;
            } else {
                heading =  Random.insideUnitCircle.normalized;
            }
            yield return new WaitForSeconds(1f);
        }
    }
}
