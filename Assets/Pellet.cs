using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pellet : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.TryGetComponent<Damagable>(out Damagable damagable)) {
            damagable.Hit(transform.position, 20f, 100f);
        }
        Destroy(gameObject);
    }
}
