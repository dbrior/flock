
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class NewTargetScanner : MonoBehaviour
{
    private CharacterMover characterMover;
    private Attacker attacker;
    private LayerMask targetLayer;
    private LayerMask ignoreLayer;
    [SerializeField] private float scanIntervalSec;

    private List<Transform> targetsInRange = new List<Transform>();

    void Start() {
        attacker = GetComponentInParent<Attacker>();
        targetLayer = attacker.GetTargetLayer();
        ignoreLayer = attacker.GetIgnoreLayer();
        characterMover = GetComponentInParent<CharacterMover>();
    }

    void OnTriggerEnter2D(Collider2D col) {
        // If not target or already tracking, exit
        if (((targetLayer.value & (1 << col.gameObject.layer)) == 0) || targetsInRange.Contains(col.transform.root)) return;

        targetsInRange.Add(col.transform.root);
        if (targetsInRange.Count == 1) {
            StartCoroutine("ScanForTarget");
            characterMover.StopWandering();
        }
    }

    void OnTriggerExit2D(Collider2D col) {
        if (!targetsInRange.Contains(col.transform.root)) return;

        targetsInRange.Remove(col.transform.root);
        if (targetsInRange.Count == 0) {
            StopCoroutine("ScanForTarget");
            characterMover.StartWandering();
        }
    }

    IEnumerator ScanForTarget() {
        while (true)
        {
            if (targetsInRange.Count > 0) {
                float minDistance = float.MaxValue;
                Transform closestTarget = null;

                foreach (Transform target in targetsInRange) {
                    float distance = Vector2.Distance(target.position, transform.position);
                    if (distance >= minDistance) continue;

                    minDistance = distance;
                    closestTarget = target;

                    characterMover.NavigateTo(closestTarget.position);
                }
            }
            yield return new WaitForSeconds(scanIntervalSec);
        }
    }
}
