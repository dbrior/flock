using System;
using System.Linq;
using System.Collections;
using UnityEngine;

[System.Serializable]
public enum SheepState : int {
    Healthy = 0,
    Hungry1 = 1,
    Hungry2 = 2,
    Dead = 4
}

public class Sheep : MonoBehaviour
{
    [SerializeField] private float maxForce;
    [SerializeField] private AudioClip captureSound;
    [SerializeField] private AudioClip deathSound;
    private AudioSource audioSource;
    private Rigidbody2D rb;
    private Animator animator;
    private ItemSpawner itemSpawner;
    public bool isDying;
    private SheepState state;
    private Damagable damagable;

    // Wool
    private bool isSheared;
    private bool isCaptured;
    [SerializeField] private GameObject woolPrefab;

    // Wander
    private Vector2 heading;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float detectionRadius;
    [SerializeField] private float maxAcceleration;
    [SerializeField] private float minMoveTime;
    [SerializeField] private float maxMoveTime;
    [SerializeField] private float minWaitTime;
    [SerializeField] private float maxWaitTime;


    void Awake() {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        itemSpawner = GetComponent<ItemSpawner>();
        damagable = GetComponent<Damagable>();
        heading = Vector2.zero;
        isDying = false;
        isSheared = false;
    }

    void Start() {
        StartCoroutine(Wander());
        SetState(SheepState.Healthy);
    }

    private void SetState(SheepState newState) {
        state = newState;
        if (state == SheepState.Dead) {
            SheepManager.Instance.KillSheep(gameObject);
        } else if ((int) state >= 1) {
            float hungerDamage = 20f;
            damagable.Hit(Vector2.zero, hungerDamage, 0f);
        }
    }

    public void AdvanceState() {
        SetState((SheepState) ((int) state + 1));
    }

    void FixedUpdate() {
        Vector2 targetVel = heading * moveSpeed;
        Vector2 velDelta = targetVel - rb.velocity;
        Vector2 requiredAccel = velDelta / Time.fixedDeltaTime;
        rb.AddForce(Vector2.ClampMagnitude(requiredAccel * rb.mass, maxForce));
        // rb.MovePosition((Vector2) transform.position + heading * moveSpeed * Time.fixedDeltaTime);
    }

    public void Kill() {
        isDying = true;
        audioSource.PlayOneShot(deathSound);
        StartCoroutine(WaitThenExecute(deathSound.length, Death));
    }

    public void Capture() {
        audioSource.PlayOneShot(captureSound);
        isCaptured = true;
    }

    public void Release() {
        isCaptured = false;
    }

    private void Death() {
        Destroy(gameObject);
    }

    public void Shear() {
        if (!isSheared) {
            SpawnWool();
            animator.SetLayerWeight(0, 0);
            animator.SetLayerWeight(1, 1.0f);
            isSheared = true;
        }
    }

    public void Regrow() {
        if (isSheared) {
            animator.SetLayerWeight(0, 1.0f);
            animator.SetLayerWeight(1, 0);
            isSheared = false;
        }
    }

    private void SpawnWool() {
        itemSpawner.SpawnItems();
    }

    public void OnTriggerEnter2D(Collider2D col) {
        // Sheared sheep can eat crops
        if (isSheared && col.gameObject.TryGetComponent<Crop>(out Crop crop)) {
            if (crop.state == CropState.Ready) {
                Regrow();
                CropManager.Instance.RemoveCropImmediately(crop);
            }
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

            // Search for crops
            if (isSheared && !isCaptured) {
                Collider2D[] collidersInRange = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
                Crop[] cropsInRange = collidersInRange.Select(
                                        collider => collider.GetComponent<Crop>()
                                    ).Where(
                                        crop => crop != null && crop.state == CropState.Ready
                                    ).ToArray();
                if (cropsInRange.Length > 0) {
                    Crop targetCrop = cropsInRange[UnityEngine.Random.Range(0, cropsInRange.Length)];
                    heading = (targetCrop.transform.position - transform.position).normalized;
                } else {
                    heading = UnityEngine.Random.insideUnitCircle.normalized;
                }
            } else {
                heading = UnityEngine.Random.insideUnitCircle.normalized;
            }

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
