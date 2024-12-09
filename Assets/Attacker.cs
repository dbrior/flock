using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
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
    [SerializeField] private UnityEvent onAttackLand;
    [SerializeField] private bool isImmediateAttack;
    private bool readyToAttack = true;
    private Damagable currentTarget;

    private AudioSource audioSource;

    void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    public LayerMask GetTargetLayer() {
        return targetLayer;
    }

    public LayerMask GetIgnoreLayer() {
        return ignoreLayer;
    }

    public void AttackStart(Damagable damagable) {
        if (readyToAttack) {
            currentTarget = damagable;
            readyToAttack = false;
            if (attackAnimator != null) {
                attackAnimator.SetTrigger("Attack");
            } else {
                AttackLand();
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
                if (((1 << collider.gameObject.layer) & ignoreLayer) != 0) continue;

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
        onAttackLand?.Invoke();
    }

    IEnumerator HitTimer(float cooldown) {
        yield return new WaitForSeconds(cooldown);
        readyToAttack = true;
    }
}
