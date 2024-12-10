using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTrigger : MonoBehaviour
{
    private Attacker attacker;
    private LayerMask targetLayer;
    private LayerMask ignoreLayer;

    private List<Damagable> targets = new List<Damagable>();
    private List<Transform> targetsTransforms = new List<Transform>();

    void Start() {
        attacker = GetComponentInParent<Attacker>();
        targetLayer = attacker.GetTargetLayer();
        ignoreLayer = attacker.GetIgnoreLayer();
    }

    void Update() {
        for (int i=0; i<targets.Count; i++) {
            attacker.AttackStart(targets[i]);
        }
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (((targetLayer.value & (1 << col.gameObject.layer)) == 0) || targetsTransforms.Contains(col.transform)) return;
        
        if (col.gameObject.TryGetComponent<Damagable>(out Damagable damagable)) {
            targets.Add(damagable);
            targetsTransforms.Add(damagable.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D col) {
        if(!targetsTransforms.Contains(col.transform)) return;

        targets.Remove(col.gameObject.GetComponent<Damagable>());
        targetsTransforms.Remove(col.transform);
    }
}
