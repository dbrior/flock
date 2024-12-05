using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector2 heading;
    public float moveSpeed;
    public float damage;
    public float knockbackForce;
    public GameObject owner;
    public bool isKinematic = true;
    public bool isDestructible = true;

    void Update() {
        if (isKinematic) {
            transform.Translate(heading * moveSpeed * Time.deltaTime);
        }
    }

    public void SetOwner(GameObject newOwner) {
        owner = newOwner;
        if (newOwner.TryGetComponent<Collider2D>(out Collider2D ownerCollider)) {
            Physics2D.IgnoreCollision(ownerCollider, GetComponent<Collider2D>());
        }
    }

    private void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.TryGetComponent<Damagable>(out Damagable damagable)) {
            damagable.Hit(transform.position, damage, knockbackForce);
        }

        if (isDestructible) {
            Destroy(gameObject);
        }
    }
}
