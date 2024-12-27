using System;
using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations;

[System.Serializable]
public enum SheepState : int {
    Healthy = 0,
    Hungry1 = 1,
    Hungry2 = 2,
    Dead = 4
}

public class Sheep : MonoBehaviour
{
    // [SerializeField] private float maxForce;
    [SerializeField] private RuntimeAnimatorController regularController;
    [SerializeField] private RuntimeAnimatorController shearedController;
    [SerializeField] private Item sheepFood;
    [SerializeField] private AudioClip captureSound;
    [SerializeField] private GameObject missingFoodIcon;
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


    private float normalVolume;

    private CharacterMover characterMover;



    void Awake() {
        characterMover = GetComponent<CharacterMover>();
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        itemSpawner = GetComponent<ItemSpawner>();
        damagable = GetComponent<Damagable>();
        isDying = false;
        isSheared = false;
        isCaptured = false;
    }

    void Start() {
        characterMover.StartWandering();
        SetState(SheepState.Healthy);
    }

    private void SetState(SheepState newState) {
        state = newState;
    }

    public void AdvanceState() {
        float hungerDamage = 20f;
        damagable.Hit(Vector2.zero, hungerDamage, 0f);
    }

    public void Hit(float damage) {
        damagable.Hit(Vector2.zero, damage, 0f);
    }

    public void Kill() {
        isDying = true;
        // audioSource.PlayOneShot(deathSound);
        // StartCoroutine(WaitThenExecute(deathSound.length, Death));
    }

    public void Capture() {
        audioSource.PlayOneShot(captureSound);
        isCaptured = true;
        QuestManager.Instance.CaptureCreature(CreatureType.Sheep);
        StartCoroutine("FeedTimer");
        // Destroy(gameObject, 30f);
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
            animator.runtimeAnimatorController = shearedController;
            isSheared = true;
            TaskManager.Instance.RemoveTask(new Task(transform, TaskType.Shear));
        }
    }

    public void Regrow() {
        if (isSheared) {
            animator.runtimeAnimatorController = regularController;
            isSheared = false;
        }
    }

    private void SpawnWool() {
        itemSpawner.SpawnItems();
    }

    private void Eat() {
        Regrow();
        damagable.RestoreHealth();
    }

    public void Feed() {
        Regrow();
        damagable.RestoreHealth();
    }

    private void Hunger() {
        if (PlayerInventory.Instance.GetItemCount(sheepFood) > 0) {
            if (isCaptured) missingFoodIcon.SetActive(false);
            PlayerInventory.Instance.RemoveItem(sheepFood, 1);
            Feed();
        } else {
            if (isSheared && isCaptured) missingFoodIcon.SetActive(true);
            Hit(20f);
        }
    }

    public bool IsSheared() {
        return isSheared;
    }

    public void OnTriggerEnter2D(Collider2D col) {
        // Any character with shears can shear sheep on contact
        if (!isCaptured && col.gameObject.TryGetComponent<Shepard>(out Shepard shepard)) {
            characterMover.SetWanderAnchor(shepard.transform);
        }
        if (isCaptured && !isSheared && col.gameObject.TryGetComponent<Shears>(out Shears shears)) {
            Shear();
        }
        // Sheared sheep can eat crops
        if (isSheared && col.gameObject.TryGetComponent<Crop>(out Crop crop)) {
            if (crop.state == CropState.Ready) {
                CropManager.Instance.RemoveCropImmediately(crop);
                Eat();
            }
        }
    }

    private IEnumerator WaitThenExecute(float duration, Action action)
    {
        yield return new WaitForSeconds(duration);
        action?.Invoke();
    }

    IEnumerator FeedTimer() {
        while (true) {
            yield return new WaitForSeconds(30f);
            Hunger();
        }
    }
}
