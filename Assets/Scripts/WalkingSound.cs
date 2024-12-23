using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingSound : MonoBehaviour
{
    [SerializeField] private AudioClip walkingSound;
    private AudioSource audioSource;

    void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound() {
        audioSource.PlayOneShot(walkingSound);
    }
}
