using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFinder : MonoBehaviour
{
    [SerializeField] private float scanIntervalSec;
    [SerializeField] private float detectionRadius;
    [SerializeField] private bool findEnemyUnits;
    // [SerializeField] private Transform relativePoint;

    private CharacterMover characterMover;
    private Attacker attacker;
    private Transform sourceTransform;

    void Start() {
        characterMover = GetComponent<CharacterMover>();
        attacker = GetComponent<Attacker>();

        // Transform sourceTransform = relativePoint != null ? relativePoint : transform;
        sourceTransform = transform;

        StartCoroutine("ScanForTarget");
    }

    IEnumerator ScanForTarget() {
        while (true)
        {
            Transform closestTarget = findEnemyUnits ? UnitManager.Instance.GetClosestEnemyUnit(sourceTransform) : UnitManager.Instance.GetClosestFriendlyUnit(sourceTransform);

            if (closestTarget != null) {
                if (detectionRadius > 0) {
                    if (Vector2.Distance(closestTarget.position, transform.position) <= detectionRadius) {
                        characterMover.NavigateTo(closestTarget);
                        if (attacker != null) attacker.SetTarget(closestTarget);
                    } else {
                        characterMover.StartWandering();
                    }
                } else {
                    characterMover.NavigateTo(closestTarget);
                    if (attacker != null) attacker.SetTarget(closestTarget);
                } 
            }

            yield return new WaitForSeconds(scanIntervalSec);
        }
    }
}
