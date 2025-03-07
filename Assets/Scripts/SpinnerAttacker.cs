using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinnerAttacker : MonoBehaviour
{
    [SerializeField] private List<RadialProjectileSpawner> spinners;
    [SerializeField] private float damage;

    void Start() {
        SetDamage(damage);
    }

    public float GetDamage() {
        return damage;
    }

    public void SetDamage(float newDamage) {
        damage = newDamage;
        foreach (RadialProjectileSpawner spinner in spinners) {
            spinner.SetDamage(damage);
        }
    }

}
