using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetScanner : MonoBehaviour
{
    private CharacterMover characterMover;

    // Target Aquisition
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float detectionRadius;
    [SerializeField] private float scanIntervalSec;

    void Awake() {
        characterMover = GetComponent<CharacterMover>();
    }

    void Start() {
        if (targetLayer != 0) {
            StartCoroutine("ScanForTarget");
        }
    }

    IEnumerator ScanForTarget() {
        while (true)
        {
            Collider2D[] targetsInRange = Physics2D.OverlapCircleAll(transform.position, detectionRadius, targetLayer);
            if (targetsInRange.Length > 0)
            {
                foreach(Collider2D target in targetsInRange.OrderBy(target => Vector2.Distance(transform.position, target.transform.position)).ToList()) {
                    bool success = characterMover.TryNavigateTo(target.transform.position);
                    if (success) break;
                }
            } else {
                characterMover.StartWandering();
            }
            yield return new WaitForSeconds(scanIntervalSec);
        }
    }
}
