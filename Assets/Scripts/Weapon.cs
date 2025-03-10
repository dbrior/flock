using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float damage;
    public float knockbackForce;
    private bool damageEnabled;
    [SerializeField] private float critChance;
    [SerializeField] private float critMutliplier;
    [SerializeField] private LayerMask targetLayer;


    public void EnableDamage() {
        damageEnabled = true;
    }

    public void DisableDamage() {
        damageEnabled = false;
    }

    public void ChangeCritChance(float delta) {
        critChance += delta;
    }

    public void ChangeCritMultiplier(float delta) {
        critMutliplier += delta;
    }

    public void OnTriggerEnter2D(Collider2D col) {
        if (damageEnabled && ((targetLayer.value & (1 << col.gameObject.layer)) != 0) && col.gameObject.TryGetComponent<Damagable>(out Damagable damagable)) {
            if (Random.Range(0, 1f) <= critChance) {
                damagable.Hit(transform.position, damage*critMutliplier, knockbackForce*2f, true);
            } else {
                damagable.Hit(transform.position, damage, knockbackForce);
            }
            
        }
    }
}
