using System.Collections;
using System.Linq;
using UnityEngine;

public class Wolf : MonoBehaviour
{
    [SerializeField]
    private LayerMask sheepLayer;
    private GameObject targetSheep;
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float detectionRadius;
    private bool hasTarget;
    private Rigidbody2D rb;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        hasTarget = false;
    }

    void Start()
    {
        StartCoroutine(ScanForSheep());
    }

    void FixedUpdate() {
        if (hasTarget) {
            Vector2 moveDirection = (targetSheep.transform.position - transform.position).normalized;
            rb.MovePosition((Vector2)transform.position + (moveDirection * moveSpeed) * Time.fixedDeltaTime);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("WildSheep"))
        {
            Destroy(col.gameObject);
            hasTarget = false;
        }
    }

    IEnumerator ScanForSheep() {
        while (true)
        {
            Collider2D[] sheepInRange = Physics2D.OverlapCircleAll(transform.position, detectionRadius, sheepLayer);
            if (sheepInRange.Length > 0)
            {
                Collider2D closestSheep = sheepInRange.OrderBy(sheep => Vector2.Distance(transform.position, sheep.transform.position)).First();
                targetSheep = closestSheep.gameObject;
                hasTarget = true;
            } else {
                hasTarget = false;
            }
            yield return new WaitForSeconds(1f); // Wait 
        }
    }
}
