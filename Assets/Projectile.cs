using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector2 heading;
    public float moveSpeed;
    public float damage;
    public float knockbackForce;
    public GameObject source;

    void Update() {
        transform.Translate(heading * moveSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject == source) return;

        if (col.gameObject.TryGetComponent<Damagable>(out Damagable damagable)) {
            damagable.Hit(transform.position, damage, knockbackForce);
        }
        Destroy(gameObject);
    }
}
