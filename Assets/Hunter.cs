using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hunter : MonoBehaviour
{
    [SerializeField] private LayerMask wolfLayer;
    [SerializeField] private float detectionRadius;

    private ToolBelt toolBelt;
    private Vector2 targetPosition;
    private bool hasTarget;
    void Start()
    {
        hasTarget = false;
        toolBelt = GetComponent<ToolBelt>();
        StartCoroutine(ScanForEnemies());
        StartCoroutine(Attack());
    }

    IEnumerator ScanForEnemies() {
        while (true)
        {
            Collider2D[] wolvesInRange = Physics2D.OverlapCircleAll(transform.position, detectionRadius, wolfLayer);
            if (wolvesInRange.Length > 0)
            {
                Collider2D closestWolf = wolvesInRange.OrderBy(wolf => Vector2.Distance(transform.position, wolf.transform.position)).First();
                hasTarget = true;
                targetPosition = closestWolf.gameObject.transform.position;
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
                toolBelt.UseSlingshotAtTarget(targetPosition);
            }
            yield return new WaitForSeconds(1f);
        }
    }
}
