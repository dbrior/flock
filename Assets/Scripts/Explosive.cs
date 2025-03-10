using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : MonoBehaviour
{
    [SerializeField] private float radius;
    [SerializeField] private float detonationTimeSec;
    [SerializeField] private float damage;
    [SerializeField] private float knockback;
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private AudioClip fuseSound;
    [SerializeField] private AudioClip explosionSound;

    void Start() {
        AudioSource.PlayClipAtPoint(fuseSound, transform.position);
    }

    private void Explode() {
        AudioSource.PlayClipAtPoint(explosionSound, transform.position);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, targetLayers);
        for (int i=0; i<colliders.Length; i++) {
            Collider2D col = colliders[i];
            if (col.gameObject.TryGetComponent<Damagable>(out Damagable damagable)) {
                damagable.Hit(transform.position, damage, knockback);
            }
        }
        Destroy(gameObject);
    }

    IEnumerator DetonationTimer() {
        yield return new WaitForSeconds(detonationTimeSec);
        Explode();
    }
}
