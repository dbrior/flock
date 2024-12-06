using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacker : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private LayerMask ignoreLayer;
    [SerializeField] private Animator attackAnimator;
    [SerializeField] private float damage;
    [SerializeField] private float knockbackForce;
    [SerializeField] private float hitCooldownSec;
    [SerializeField] private Collider2D attackZone;
    [SerializeField] private bool indiscriminantDamage;
    [SerializeField] private AudioClip attackLandSound;
    private bool readyToAttack = true;
    private Damagable currentTarget;

    private AudioSource audioSource;

    void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    // private void OnTriggerStay2D(Collider2D col) {
    //     if (readyToAttack && ((1 << col.gameObject.layer) & targetLayer) != 0) {
    //         if (col.gameObject.TryGetComponent<Damagable>(out Damagable damagable)) {
    //             damagable.Hit(transform.position, damage, knockbackForce);
    //             readyToAttack = false;
    //             if (attackAnimator != null) {
    //                 attackAnimator.SetTrigger("Attack");
    //             }
    //             StartCoroutine(HitTimer(hitCooldownSec));
    //         }
    //     }
    // }

    public void AttackStart(Damagable damagable) {
        if (readyToAttack) {
            currentTarget = damagable;
            readyToAttack = false;
            if (attackAnimator != null) {
                attackAnimator.SetTrigger("Attack");
            }
        }
    }

    public void AttackLand() {
        if (indiscriminantDamage) {
            List<Collider2D> results = new List<Collider2D>();
            ContactFilter2D filter = new ContactFilter2D().NoFilter();

            int overlapCount = attackZone.OverlapCollider(filter, results);

            foreach (Collider2D collider in results)
            {
                if (collider.gameObject != gameObject && collider.gameObject.TryGetComponent<Damagable>(out Damagable damagable)) {
                    damagable.Hit(transform.position, damage, knockbackForce);
                }
            }
        } else if (currentTarget.GetComponent<Collider2D>().IsTouching(attackZone)) {
            currentTarget.Hit(transform.position, damage, knockbackForce);
        }
        if (audioSource != null) {
            audioSource.PlayOneShot(attackLandSound);
        }
        StartCoroutine(HitTimer(hitCooldownSec));
    }

    IEnumerator HitTimer(float cooldown) {
        yield return new WaitForSeconds(cooldown);
        readyToAttack = true;
    }
}
