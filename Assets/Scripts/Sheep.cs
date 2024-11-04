using System;
using System.Collections;
using UnityEngine;

public class Sheep : MonoBehaviour
{
    [SerializeField] private AudioClip captureSound;
    [SerializeField] private AudioClip deathSound;
    private AudioSource audioSource;
    void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    public void Kill() {
        audioSource.PlayOneShot(deathSound);
        StartCoroutine(WaitThenExecute(deathSound.length, Death));
    }

    public void Capture() {
        audioSource.PlayOneShot(captureSound);
    }

    private void Death() {
        Destroy(gameObject);
    }

    private IEnumerator WaitThenExecute(float duration, Action action)
    {
        yield return new WaitForSeconds(duration);
        action?.Invoke();
    }
}
