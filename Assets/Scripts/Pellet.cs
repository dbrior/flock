using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pellet : MonoBehaviour
{
    public float damage;

    private void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.tag == "Player") return;

        if (col.gameObject.TryGetComponent<Damagable>(out Damagable damagable)) {
            damagable.Hit(transform.position, damage, 100f);
        }
        Destroy(gameObject);
    }
}
