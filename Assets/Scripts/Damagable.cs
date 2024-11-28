using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Damagable : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    private float currHealth;
    [SerializeField] private Image healthUI;
    private Rigidbody2D rb;
    [SerializeField] private UnityEvent onDeath;


    [SerializeField] private AudioClip hitSound;
    private AudioSource audioSource;
    [SerializeField] private Animator hitAnimator;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        currHealth = maxHealth;   
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

    public void SetMaxHealth(float newMax) {
        maxHealth = newMax;
    }

    public void Hit(Vector2 damagePos, float damage, float knockback) {
        ChangeHealth(-damage);
        if(currHealth <= 0) {
            onDeath?.Invoke();
            Destroy(gameObject);
        }
        Vector3 damageVector = ((Vector2) transform.position - damagePos).normalized * knockback;
        rb.AddForce(damageVector);

        if (hitSound != null) {
            audioSource.PlayOneShot(hitSound);
        }

        if (hitAnimator != null) {
            hitAnimator.SetTrigger("Hit");
        }
    }
}
