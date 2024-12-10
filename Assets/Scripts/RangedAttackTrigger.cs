using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackTrigger : MonoBehaviour
{
    private RangedAttacker attacker;
    [SerializeField] private LayerMask targetLayer;

    void Start() {
        attacker = GetComponentInParent<RangedAttacker>();
    }

    private void OnTriggerStay2D(Collider2D col) {
        if (((1 << col.gameObject.layer) & targetLayer) != 0) {
            if (col.gameObject.TryGetComponent<Damagable>(out Damagable damagable)) {
                attacker.Attack(col.gameObject.transform.position);
            }
        }
    }
}
