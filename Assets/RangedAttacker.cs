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
    private bool onCooldown = false;

    public void Attack(Vector2 position) {
        if (!onCooldown) {
            Vector2 heading = (position - (Vector2) transform.position).normalized;

            Projectile projectile = Instantiate(projectilePrefab, (Vector2) transform.position + (heading*0.1f), transform.rotation).GetComponent<Projectile>();
            projectile.heading = heading;
            projectile.damage = damage;
            projectile.knockbackForce = knockbackForce;
            projectile.moveSpeed = projectileSpeed;
            projectile.source = gameObject;

            onCooldown = true;
            StartCoroutine(CooldownTimer(cooldownSec));
        }
    }
    
    IEnumerator CooldownTimer(float cooldown) {
        yield return new WaitForSeconds(cooldown);
        onCooldown = false;
    }
}
