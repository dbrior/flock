using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask openers;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;
    private int numObjectsInRange;

    void OnTriggerEnter2D(Collider2D col) {
        if (((1 << col.gameObject.layer) & openers) == 0) return;
        if (col.isTrigger) return;

        numObjectsInRange += 1;
        if (numObjectsInRange == 1) animator.SetTrigger("Open");

        if (audioSource != null && openSound != null) {
            audioSource.PlayOneShot(openSound);
        }
    }

    void OnTriggerExit2D(Collider2D col) {
        if (((1 << col.gameObject.layer) & openers) == 0) return;
        if (col.isTrigger) return;

        numObjectsInRange -= 1;
        if (numObjectsInRange == 0) animator.SetTrigger("Close");

        if (audioSource != null && closeSound != null) {
            audioSource.PlayOneShot(closeSound);
        }
    }
}
