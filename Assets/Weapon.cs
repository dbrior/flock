using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float damage;
    public float knockbackForce;
    private bool damageEnabled;

    public void EnableDamage() {
        damageEnabled = true;
    }

    public void DisableDamage() {
        damageEnabled = false;
    }

    public void OnTriggerEnter2D(Collider2D col) {
        if (damageEnabled && col.gameObject.TryGetComponent<Damagable>(out Damagable damagable)) {
            damagable.Hit(transform.position, damage, knockbackForce);
        }
    }
}
