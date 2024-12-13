using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector2 heading;
    public float moveSpeed;
    public float damage;
    public float knockbackForce;
    public Collider2D ownerCollider;
    public bool isKinematic = true;
    public bool isDestructible = true;

    void Update() {
        if (isKinematic) {
            transform.Translate(heading * moveSpeed * Time.deltaTime);
        }
    }

    public void SetOwner(Collider2D newOwnerCollider) {
        ownerCollider = newOwnerCollider;
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (col == ownerCollider || col.isTrigger) return;

        if (col.gameObject.TryGetComponent<Damagable>(out Damagable damagable)) {
            damagable.Hit(transform.position, damage, knockbackForce);
        }

        if (isDestructible) {
            Destroy(gameObject);
        }
    }
}
