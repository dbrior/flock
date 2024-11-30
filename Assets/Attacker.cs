using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacker : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayer;

    [SerializeField] private Animator attackAnimator;
    [SerializeField] private float damage;
    [SerializeField] private float knockbackForce;
    [SerializeField] private float hitCooldownSec;
    [SerializeField] private Collider2D attackZone;
    private bool readyToAttack = true;
    private Damagable currentTarget;

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
        if (currentTarget.GetComponent<Collider2D>().IsTouching(attackZone)) {
            currentTarget.Hit(transform.position, damage, knockbackForce);
        }
        StartCoroutine(HitTimer(hitCooldownSec));
    }

    IEnumerator HitTimer(float cooldown) {
        yield return new WaitForSeconds(cooldown);
        readyToAttack = true;
    }
}
