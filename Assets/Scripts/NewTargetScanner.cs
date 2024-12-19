
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class NewTargetScanner : MonoBehaviour
{
    // private CharacterMover characterMover;
    // private Attacker attacker;
    // private LayerMask targetLayer;
    // private LayerMask ignoreLayer;
    // [SerializeField] private float scanIntervalSec;

    // private List<Transform> targetsInRange = new List<Transform>();

    // void Start() {
    //     attacker = GetComponentInParent<Attacker>();
    //     targetLayer = attacker.GetTargetLayer();
    //     ignoreLayer = attacker.GetIgnoreLayer();
    //     characterMover = GetComponentInParent<CharacterMover>();

    //     ProcessInitialColliders();
    // }

    // private void ProcessInitialColliders() {
    //     Collider2D[] initialColliders = Physics2D.OverlapCircleAll(transform.position, GetComponent<CircleCollider2D>().radius);
    //     for (int i=0; i<initialColliders.Length; i++) {
    //         Collider2D col = initialColliders[i];
    //         if (col.gameObject == gameObject) continue;

    //         ProcessIncomingTarget(col);
    //     }
    // }

    // private void ProcessIncomingTarget(Collider2D col) {
    //     if (((targetLayer.value & (1 << col.gameObject.layer)) == 0) || targetsInRange.Contains(col.transform)) return;

    //     targetsInRange.Add(col.transform);
    //     if (targetsInRange.Count == 1) {
    //         StartCoroutine("ScanForTarget");
    //         characterMover.StopWandering();
    //     }
    // }

    // private void ProcessingOutgoingTarget(Collider2D col) {
    //     if (!targetsInRange.Contains(col.transform)) return;

    //     targetsInRange.Remove(col.transform);
    //     if (targetsInRange.Count == 0) {
    //         StopCoroutine("ScanForTarget");
    //         characterMover.StartWandering();
    //     }
    // }

    // void OnTriggerEnter2D(Collider2D col) {
    //     ProcessIncomingTarget(col);
    // }

    // void OnTriggerExit2D(Collider2D col) {
    //     ProcessingOutgoingTarget(col);
    // }

    // IEnumerator ScanForTarget() {
    //     while (true)
    //     {
    //         if (targetsInRange.Count > 0) {
    //             float minDistance = float.MaxValue;
    //             Transform closestTarget = null;

    //             for (int i=0; i<targetsInRange.Count; i++) {
    //                 Transform target = targetsInRange[i];
    //                 float distance = (target.position - transform.position).sqrMagnitude;
    //                 if (distance >= minDistance) continue;

    //                 minDistance = distance;
    //                 closestTarget = target;
    //             }

    //             characterMover.NavigateTo(closestTarget);
    //         }
    //         yield return new WaitForSeconds(scanIntervalSec);
    //     }
    // }
}
