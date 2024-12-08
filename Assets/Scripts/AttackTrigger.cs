using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTrigger : MonoBehaviour
{
    private Attacker attacker;
    private LayerMask targetLayer;
    private LayerMask ignoreLayer;

    void Start() {
        attacker = GetComponentInParent<Attacker>();
        targetLayer = attacker.GetTargetLayer();
        ignoreLayer = attacker.GetIgnoreLayer();
    }

    private void OnTriggerStay2D(Collider2D col) {
        if (((1 << col.gameObject.layer) & ignoreLayer) != 0) return;

        if (((1 << col.gameObject.layer) & targetLayer) != 0) {
            if (col.gameObject.TryGetComponent<Damagable>(out Damagable damagable)) {
                attacker.AttackStart(damagable);
            }
        }
    }
}
