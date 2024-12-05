using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingStation : MonoBehaviour
{
    [SerializeField] private float healAmount;
    [SerializeField] private float frequencySec;
    [SerializeField] private AudioClip healSound;
    [SerializeField] private Animator healAnimator;

    private List<Damagable> targets = new List<Damagable>();
    private AudioSource audioSource;

    void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    private void AddTarget(Damagable target) {
        targets.Add(target);

        if (targets.Count == 1) {
            StartCoroutine("Heal");
        }
    }

    private void RemoveTarget(Damagable target) {
        if (targets.Contains(target)) {
            targets.Remove(target);
        }

        if (targets.Count == 0) {
            StopCoroutine("Heal");
        }
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.TryGetComponent<Damagable>(out Damagable damagable)) {
            if (damagable.GetHealthPct() >= 1f) return;

            AddTarget(damagable);   
        }
    }

    private void OnTriggerExit2D(Collider2D col) {
        if (col.gameObject.TryGetComponent<Damagable>(out Damagable damagable)) {
            RemoveTarget(damagable);
        }
    }

    private IEnumerator Heal() {
        while (true) {
            foreach (Damagable target in targets) {
                if (target.GetHealthPct() >= 1f) {
                    RemoveTarget(target);
                } else {
                    target.ChangeHealth(healAmount);

                    if (healSound != null) {
                        audioSource.PlayOneShot(healSound);
                    }
                }
            }

            if (healAnimator != null) {
                Debug.Log("Settings animator");
                healAnimator.SetTrigger("Heal");
            }

            yield return new WaitForSeconds(frequencySec);
        }
    }
}
