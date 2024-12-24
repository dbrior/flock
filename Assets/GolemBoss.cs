using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemBoss : MonoBehaviour
{
    [SerializeField] private GameObject crystalPrefab;
    [SerializeField] private float attackCooldownSec;
    [SerializeField] private float smashChargeSec;
    [SerializeField] private AudioClip chargeSound;
    [SerializeField] private AudioClip slamSound;
    [SerializeField] private AudioClip crystalSound;
    [SerializeField] private AudioClip crystalCompleteSound;
    [SerializeField] private GameObject healthBar;
    [SerializeField] private Transform telegraph;
    [SerializeField] private GameObject shadow;
    private bool isReadyToAttack;
    private Animator animator;
    private AudioSource audioSource;
    private Damagable damagable;
    private CharacterMover characterMover;
    private Collider2D physicalCollider;

    void Awake() {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        damagable = GetComponent<Damagable>();
        characterMover = GetComponent<CharacterMover>();
        physicalCollider = GetComponent<Collider2D>();
        isReadyToAttack = true;

        damagable.onDeath.AddListener(() => StartDeath());
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "Player" && isReadyToAttack) {
            animator.SetTrigger("Attack");
            isReadyToAttack = false;
        }
    }

    private void StartDeath() {
        characterMover.NavigateTo(transform.position);
        animator.SetTrigger("Death");
        physicalCollider.enabled = false;
        healthBar.SetActive(false);
        shadow.SetActive(false);
        characterMover.enabled = false;
        damagable.enabled = false;
    }

    public void EndDeath() {
        Destroy(gameObject);
    }

    public void SpawnCrystals() {
        StartCoroutine("CrystalSpawning");
        // float radius = 1.25f;
        // SpawnPrefabsRadially(crystalPrefab, transform.position, 40, radius);
        // for (int i=0; i<Random.Range(2, 5); i++) {
        //     Vector3 offset = new Vector3(Random.Range(-radius*0.75f, radius*0.75f), Random.Range(-radius*0.75f, radius*0.75f));
        //     Instantiate(crystalPrefab, transform.position + offset, Quaternion.identity);
        // }
        // StartCoroutine("AttackCooldown");
        // StartCoroutine("ChargeSmash");
    }

    public void SmashAttack() {
        float radius = 1.25f;
        float damage = 500f;
        float knockbackForce = 200f;
        audioSource.PlayOneShot(slamSound);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);
        for (int i=0; i<colliders.Length; i++) {
            Collider2D col = colliders[i];
            if (col.gameObject == gameObject) continue;

            if (col.gameObject.TryGetComponent<Damagable>(out Damagable damagable)) {
                damagable.Hit(transform.position, damage, knockbackForce);
            }
        }
    }

    IEnumerator ScaleTelegraph() {
        telegraph.gameObject.SetActive(true);

        Vector3 startScale = telegraph.transform.localScale;
        Vector3 targetScale = new Vector3(2.5f, startScale.y * (2.5f / startScale.x), startScale.z);

        float elapsedTime = 0f;
        while (elapsedTime < smashChargeSec)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / smashChargeSec;
            
            telegraph.localScale = Vector3.Lerp(startScale, targetScale, progress);
            
            yield return null;
        }

        // Ensure we end up exactly at the target scale
        telegraph.localScale = startScale;
        telegraph.gameObject.SetActive(false);
    }

    IEnumerator CrystalSpawning() {
        float radius = 1.25f;
        yield return StartCoroutine(SpawnPrefabsRadially(crystalPrefab, transform.position, 40, radius));
        for (int i=0; i<Random.Range(2, 5); i++) {
            Vector3 offset = new Vector3(Random.Range(-radius*0.75f, radius*0.75f), Random.Range(-radius*0.75f, radius*0.75f));
            Instantiate(crystalPrefab, transform.position + offset, Quaternion.identity);
        }
        yield return new WaitForSeconds(0.1f);
        AudioSource.PlayClipAtPoint(crystalCompleteSound, transform.position);
        StartCoroutine("AttackCooldown");
        StartCoroutine("ChargeSmash");
    }

    IEnumerator SpawnPrefabsRadially(GameObject prefab, Vector2 centerPosition, int numberOfPrefabs, float radius, float startAngle = 0f)
    {
        float angleStep = 360f / numberOfPrefabs;
        
        for (int i = 0; i < numberOfPrefabs; i++)
        {
            float angle = startAngle + (i * angleStep);
            // Convert angle to radians for cos/sin calculation
            float angleInRadians = angle * Mathf.Deg2Rad;
            
            // Calculate position using trigonometry
            float xPos = centerPosition.x + (radius * Mathf.Cos(angleInRadians));
            float yPos = centerPosition.y + (radius * Mathf.Sin(angleInRadians));
            
            Vector2 spawnPosition = new Vector2(xPos, yPos);
            
            // Instantiate the prefab at calculated position
            // Instantiate(prefab, spawnPosition, Quaternion.Euler(0, 0, angle));
            Instantiate(prefab, spawnPosition, Quaternion.identity);

            AudioSource.PlayClipAtPoint(crystalSound, spawnPosition);
            yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator AttackCooldown() {
        yield return new WaitForSeconds(attackCooldownSec);
        isReadyToAttack = true;
    }

    IEnumerator ChargeSound() {
        yield return new WaitForSeconds(smashChargeSec-chargeSound.length);
        audioSource.PlayOneShot(chargeSound);
    }

    IEnumerator ChargeSmash() {
        StartCoroutine("ChargeSound");
        StartCoroutine("ScaleTelegraph");
        yield return new WaitForSeconds(smashChargeSec);
        animator.SetTrigger("Attack2");
    }
}
