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
    private Animator animator;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        StartCoroutine(ScanForSheep());
        animator.SetLayerWeight(0, 0);
        animator.SetLayerWeight(1, 0);
        animator.SetLayerWeight(2, 1f);
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

            if (Mathf.Abs(heading.y) >= Mathf.Abs(heading.x)) {
                if (heading.y < 0) {
                    animator.SetTrigger("MoveDown");
                } else if (heading.y > 0) {
                    animator.SetTrigger("MoveUp");
                }
            } else {
                if (heading.x < 0) {
                animator.SetTrigger("MoveLeft");
                } else if (heading.x > 0) {
                    animator.SetTrigger("MoveRight");
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }
}
