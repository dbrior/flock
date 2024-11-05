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
    public bool isDying;

    // Wool
    private bool isSheared;
    [SerializeField] private GameObject woolPrefab;

    // Wander
    private Vector2 heading;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxAcceleration;
    [SerializeField] private float minMoveTime;
    [SerializeField] private float maxMoveTime;
    [SerializeField] private float minWaitTime;
    [SerializeField] private float maxWaitTime;


    void Awake() {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        heading = Vector2.zero;
        isDying = false;
        isSheared = false;
    }

    void Start() {
        StartCoroutine(Wander());
        GetComponent<Interactable>().onInteract.AddListener(Shear);
    }

    void FixedUpdate() {
        Vector2 targetVel = heading * moveSpeed;
        Vector2 velDelta = targetVel - rb.velocity;
        Vector2 requiredAccel = velDelta / Time.fixedDeltaTime;
        rb.AddForce(requiredAccel * rb.mass);
        // rb.MovePosition((Vector2) transform.position + heading * moveSpeed * Time.fixedDeltaTime);
    }

    public void Kill() {
        isDying = true;
        audioSource.PlayOneShot(deathSound);
        StartCoroutine(WaitThenExecute(deathSound.length, Death));
    }

    public void Capture() {
        audioSource.PlayOneShot(captureSound);
    }

    private void Death() {
        Destroy(gameObject);
    }

    public void Shear() {
        SpawnWool();
        animator.SetLayerWeight(1, 1.0f);
    }

    private void SpawnWool() {
        int woolAmount = UnityEngine.Random.Range(1, 3);
        Debug.Log("Spawn " + woolAmount.ToString() + " wool");
        for (int i=0; i<woolAmount; i++) {
            Vector2 spawnLocation = (Vector2) transform.position + (UnityEngine.Random.insideUnitCircle.normalized * 0.1f);
            Instantiate(woolPrefab, spawnLocation, Quaternion.identity);
        }
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
            heading = UnityEngine.Random.insideUnitCircle.normalized;
            if (Mathf.Abs(heading.y) >= Mathf.Abs(heading.x)) {
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
            heading = Vector2.zero;
            animator.SetTrigger("Idle");
        }
    }
}
