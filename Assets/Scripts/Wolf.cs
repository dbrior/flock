using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Wolf : MonoBehaviour
{
    private Damagable damagable;
    private Animator animator;
    private Attacker attacker;

    void Awake() {
        animator = GetComponent<Animator>();
        damagable = GetComponent<Damagable>();
        attacker = GetComponent<Attacker>();
    }

    void Start()
    {
        animator.SetLayerWeight(0, 0);
        animator.SetLayerWeight(1, 0);
        animator.SetLayerWeight(2, 1f);
        damagable.onDeath.AddListener(() => WolfManager.Instance.DecreaseWolfCount());
    }

    public void SetMaxHealth(float health) {
        damagable.SetMaxHealth(health);
    }

    public void SetAttackDamage(float newDamage) {
        attacker.SetDamage(newDamage);
    }
}
