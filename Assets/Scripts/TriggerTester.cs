using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTester : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private LayerMask ignoreLayer;

    private void OnTriggerStay2D(Collider2D col) {
        if (((1 << col.gameObject.layer) & ignoreLayer) != 0) return;

        if (((1 << col.gameObject.layer) & targetLayer) != 0) {
            Debug.Log("Tester firing");
        }
    }
}
