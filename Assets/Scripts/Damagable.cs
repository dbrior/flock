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
    public float blockChance = 0.01f;
    public UnityEvent onDeath;
    [SerializeField] private bool regenEnabled;
    [SerializeField] private float regenPerSecond;
    [SerializeField] private float regenDelay;
    private float lastHitTime;


    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip blockSound;
    [SerializeField] private AudioClip critSound;
    [SerializeField] private AudioClip deathSound;
    private AudioSource audioSource;
    [SerializeField] private Animator hitAnimator;
    [SerializeField] private bool isBoss;
    [SerializeField] private bool randomizePitch;
    [SerializeField] private bool isIndestructible;

    private float originalPitch;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        if (audioSource != null) {
            originalPitch = audioSource.pitch;
        }
        
        currHealth = maxHealth;
        if (regenEnabled) {
            StartCoroutine(Regen());
        }
        if (!isBoss) {
            HideHealthBar();
        }

        if (isBoss) {
            onDeath.AddListener(GameManager.Instance.BossKill);
        } else if (gameObject.layer == LayerMask.NameToLayer("Wolf")) {
            onDeath.AddListener(GameManager.Instance.EnemyKill);
        }
    }

    public void HideHealthBar() {
        if (healthUI == null) return;

        healthUI.transform.parent.gameObject.SetActive(false);
    }

    public void ShowHealthBar() {
        if (healthUI == null) return;

        healthUI.transform.parent.gameObject.SetActive(true);
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

    public float GetHealthPct() {
        return currHealth / maxHealth;
    }

    public void ChangeHealth(float delta) {
        if (delta > 0 && currHealth >= maxHealth) return;
        if (!isBoss && currHealth == maxHealth && delta < 0) {
            ShowHealthBar();
        }

        currHealth += delta;
        currHealth = Mathf.Min(currHealth, maxHealth);

        if (healthUI != null) {
            healthUI.fillAmount = currHealth / maxHealth;
        }

        // Spawn healing numbers
        if (hitNumberLocation != null && delta > 0) {
            DamageNumberSpawner.Instance.SpawnDamageNumber(hitNumberLocation.position, Mathf.Round(delta).ToString(), DamageNumberType.Heal);
        }

        if (!isBoss && currHealth == maxHealth) {
            HideHealthBar();
        }
    }

    public void RestoreHealth() {
        ChangeHealth(maxHealth - currHealth);
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

    public float GetMaxHealth() {
        return maxHealth;
    }

    public void SetBlockChance(float newBlockChance) {
        blockChance = newBlockChance;
    }

    public void Hit(Vector2 damagePos, float damage, float knockback, bool isCrit = false, bool isUnblockable = false) {
        if (randomizePitch) {
            audioSource.pitch = originalPitch;
        }

        float hitRoll = Random.Range(0, 1f);
        if (!isUnblockable && hitRoll <= blockChance) {
            DamageNumberSpawner.Instance.SpawnStatusIcon(hitNumberLocation.position, StatusIconType.Block);
            audioSource.PlayOneShot(blockSound);
            return;
        }

        lastHitTime = Time.time;

        ChangeHealth(-damage);
        Vector3 damageVector = ((Vector2) transform.position - damagePos).normalized * knockback;
        if (rb != null) rb.AddForce(damageVector);

        if (isCrit && critSound != null) {
            audioSource.PlayOneShot(critSound);
        } else if (hitSound != null) {
            if (randomizePitch) audioSource.pitch = Random.Range(0.8f, 1.2f);
            audioSource.PlayOneShot(hitSound);
        }

        if (hitAnimator != null) {
            hitAnimator.SetTrigger("Hit");
        }

        if (damage > 0 && hitNumberLocation != null) {
            DamageNumberType numberType = isCrit ? DamageNumberType.Crit : DamageNumberType.Normal;
            DamageNumberSpawner.Instance.SpawnDamageNumber(hitNumberLocation.position, Mathf.Round(damage).ToString(), numberType);
        }

        // Death
        if(currHealth <= 0) {
            if (hitNumberLocation != null) DamageNumberSpawner.Instance.SpawnStatusIcon(hitNumberLocation.position, StatusIconType.Death);
            if (deathSound != null) {
                PlayerManager.Instance.currentPlayer.audioSource.PlayOneShot(deathSound);
                // audioSource.PlayOneShot(deathSound);
                // AudioSource.PlayClipAtPoint(deathSound, transform.position, 1f);
            }
            onDeath?.Invoke();
            if (isIndestructible) {
                RestoreHealth();
            } else {
                Destroy(gameObject);
            }
        }
    }

    IEnumerator Regen() {
        while (true) {
            if (currHealth < maxHealth && lastHitTime < Time.time - regenDelay) {
                ChangeHealth(regenPerSecond);
            }
            yield return new WaitForSeconds(1f);
        }
    }
}
