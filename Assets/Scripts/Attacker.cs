using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class Attacker : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private float attackTriggerDist;
    [SerializeField] private float attackLandDist;
    [SerializeField] private bool isImmediateAttack;
    [SerializeField] private bool isUnblockable;
    [SerializeField] private bool canHitSelf;
    [SerializeField] private bool isIndiscriminantDamage;
    [SerializeField] private LayerMask indiscriminantTargetLayers;

    [Header("Attack Stats")]
    [SerializeField] private float damage;
    [SerializeField] private float hitCooldownSec;
    [SerializeField] private float knockbackForce;

    [Header("Post-Attack")]
    [SerializeField] private bool doesNotHaveAttackAnimation;
    [SerializeField] private AudioClip attackLandSound;
    [SerializeField] private UnityEvent onAttackLand;
    

    [SerializeField] private Transform currentTarget;
    private Damagable currentTargetDamagable;
    [SerializeField] private float currentTargetDist;
    private bool readyToAttack = true;
    private AudioSource audioSource;
    private Animator animator;

    void Start() {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    void Update() {
        if (currentTarget == null) return;

        currentTargetDist = Vector2.Distance(currentTarget.position, transform.position);
        if (currentTargetDist <= attackTriggerDist) {
            StartAttack();
        }
    }

    public void SetTarget(Transform newTarget) {
        if (newTarget == currentTarget) return;

        currentTarget = newTarget;
        if (currentTarget.TryGetComponent<Damagable>(out Damagable damagable)) {
            currentTargetDamagable = damagable;
        } else {
            currentTargetDamagable = null;
        }
    }

    public void StartAttack() {
        if (readyToAttack) {
            readyToAttack = false;
            StartCoroutine(HitTimer(hitCooldownSec));
            if (isImmediateAttack) AttackLand();
            if (animator != null && !doesNotHaveAttackAnimation) animator.SetTrigger("Attack");
        }
    }

    public void AttackLand() {
        if (isIndiscriminantDamage) {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackLandDist, indiscriminantTargetLayers);
            for (int i=0; i<colliders.Length; i++) {
                Collider2D col = colliders[i];
                if (!canHitSelf && col.gameObject == gameObject) continue;

                if (col.gameObject.TryGetComponent<Damagable>(out Damagable damagable)) {
                    damagable.Hit(transform.position, damage, knockbackForce, isUnblockable: isUnblockable);
                }
            }
        } else if (currentTargetDist <= attackLandDist) {
            currentTargetDamagable.Hit(transform.position, damage, knockbackForce, isUnblockable: isUnblockable);
        }
        if (audioSource != null && attackLandSound != null) {
            audioSource.PlayOneShot(attackLandSound);
        }
        onAttackLand?.Invoke();
    }

    public void SetAttackCooldownSec(float newAttackCooldownSec) {
        hitCooldownSec = newAttackCooldownSec;
    }

    public void SetDamage(float newDamage) {
        damage = newDamage;
    }

    IEnumerator HitTimer(float cooldown) {
        yield return new WaitForSeconds(cooldown);
        readyToAttack = true;
    }
}
