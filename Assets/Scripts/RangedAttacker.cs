using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttacker : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private float knockbackForce;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float cooldownSec;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private AudioClip shootSound;

    private AudioSource audioSource;
    private bool onCooldown = false;
    private Collider2D collider;
    private Animator animator;

    void Awake() {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        collider = GetComponent<Collider2D>();
    }

    public void SetDamage(float newDamage) {
        damage = damage;
    }

    public void SetAttackCooldownSec(float newAttackCooldownSec) {
        cooldownSec = newAttackCooldownSec;
    }

    public void Attack(Vector2 position) {
        if (!onCooldown) {
            Vector2 heading = (position - (Vector2) transform.position).normalized;

            Projectile projectile = Instantiate(projectilePrefab, (Vector2) transform.position + (heading*0.1f), transform.rotation).GetComponent<Projectile>();
            projectile.heading = heading;
            projectile.damage = damage;
            projectile.knockbackForce = knockbackForce;
            projectile.moveSpeed = projectileSpeed;
            
            projectile.SetOwner(collider);

            onCooldown = true;
            StartCoroutine(CooldownTimer(cooldownSec));

            if (audioSource != null) {
                audioSource.PlayOneShot(shootSound);
            }

            if (animator != null) {
                animator.SetTrigger("Attack");
            }
        }
    }
    
    IEnumerator CooldownTimer(float cooldown) {
        yield return new WaitForSeconds(cooldown);
        onCooldown = false;
    }
}
