using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class Damagable : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    private float currHealth;
    [SerializeField] private Image healthUI;
    [SerializeField] private Transform hitNumberLocation;
    private Rigidbody2D rb;
    private float blockChance;
    [SerializeField] private UnityEvent onDeath;
    private float regenPerSecond;


    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip critSound;
    [SerializeField] private AudioClip deathSound;
    private AudioSource audioSource;
    [SerializeField] private Animator hitAnimator;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        currHealth = maxHealth;
        blockChance = 0f;
        regenPerSecond = 1f;
        StartCoroutine(Regen());
    }

    public void ChangeBlockChance(float delta) {
        blockChance += delta;
    }

    public void ChangeHealthRegen(float delta) {
        regenPerSecond += delta;
    }

    public void HealPct(float pct) {
        ChangeHealth(pct * maxHealth);
    }

    public void ChangeHealth(float delta) {
        currHealth += delta;
        currHealth = Mathf.Min(currHealth, maxHealth);
        healthUI.fillAmount = currHealth / maxHealth;
    }

    public void RestoreHealth() {
        currHealth = maxHealth;
        healthUI.fillAmount = currHealth / maxHealth;
    }

    public void ChangeMaxHealth(float delta) {
        maxHealth += delta;
        healthUI.fillAmount = currHealth / maxHealth;
    }

    public void ChangeMaxHealthPct(float pct) {
        maxHealth += 1f + pct;
        healthUI.fillAmount = currHealth / maxHealth;
    }

    public void SetMaxHealth(float newMax) {
        maxHealth = newMax;
    }

    public void Hit(Vector2 damagePos, float damage, float knockback, bool isCrit = false) {
        float hitRoll = Random.Range(0, 1f);
        if (hitRoll <= blockChance) return;

        ChangeHealth(-damage);
        if(currHealth <= 0) {
            if (deathSound != null) {
                AudioSource.PlayClipAtPoint(deathSound, transform.position, 1f);
            }
            onDeath?.Invoke();
            Destroy(gameObject);
        }
        Vector3 damageVector = ((Vector2) transform.position - damagePos).normalized * knockback;
        rb.AddForce(damageVector);

        if (isCrit && critSound != null) {
            audioSource.PlayOneShot(critSound);
        } else if (hitSound != null) {
            audioSource.PlayOneShot(hitSound);
        }

        if (hitAnimator != null) {
            hitAnimator.SetTrigger("Hit");
        }

        if (hitNumberLocation != null) {
            DamageNumberSpawner.Instance.SpawnDamageNumber(hitNumberLocation.position, Mathf.Round(damage).ToString(), isCrit);
        }
    }

    IEnumerator Regen() {
        while (true) {
            ChangeHealth(regenPerSecond);
            yield return new WaitForSeconds(1f);
        }
    }
}
