using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask openers;
    private int numObjectsInRange;

    void OnTriggerEnter2D(Collider2D col) {
        if (((1 << col.gameObject.layer) & openers) == 0) return;
        if (col.isTrigger) return;

        numObjectsInRange += 1;
        if (numObjectsInRange == 1) animator.SetTrigger("Open");
    }

    void OnTriggerExit2D(Collider2D col) {
        if (((1 << col.gameObject.layer) & openers) == 0) return;
        if (col.isTrigger) return;

        numObjectsInRange -= 1;
        if (numObjectsInRange == 0) animator.SetTrigger("Close");
    }
}
