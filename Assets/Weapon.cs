using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float damage;
    public float knockbackForce;
    public void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.TryGetComponent<Damagable>(out Damagable damagable)) {
            damagable.Hit(transform.position, damage, knockbackForce);
        }
    }
}
