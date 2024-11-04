using System;
using System.Collections;
using UnityEngine;

public class Sheep : MonoBehaviour
{
    [SerializeField] private AudioClip captureSound;
    [SerializeField] private AudioClip deathSound;
    private AudioSource audioSource;
    private Rigidbody2D rb;
    private Animator animator;

    // Wander
    private bool moving;
    private Vector2 heading;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float minMoveTime;
    [SerializeField] private float maxMoveTime;
    [SerializeField] private float minWaitTime;
    [SerializeField] private float maxWaitTime;


    void Awake() {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        moving = false;
        heading = Vector2.zero;
    }

    void Start() {
        StartCoroutine(Wander());
    }

    void FixedUpdate() {
        if (moving) {
            rb.MovePosition((Vector2) transform.position + heading * moveSpeed * Time.fixedDeltaTime);
        }
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

    private IEnumerator Wander() {
        while (true) {
            float waitTime = UnityEngine.Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(waitTime);
            float moveTime = UnityEngine.Random.Range(minMoveTime, maxMoveTime);
            moving = true;
            heading = UnityEngine.Random.insideUnitCircle.normalized;
            if (heading.y >= heading.x) {
                if (heading.y < 0) {
                    animator.SetTrigger("MoveDown");
                } else if (heading.y > 0) {
                    animator.SetTrigger("MoveUp");
                }
            } else {
                if (heading.x < 0) {
                animator.SetTrigger("MoveLeft");
                } else if (heading.x > 0) {
                    animator.SetTrigger("MoveRight");
                }
            }
            yield return new WaitForSeconds(moveTime);
            moving = false;
            animator.SetTrigger("Idle");
        }
    }
}
