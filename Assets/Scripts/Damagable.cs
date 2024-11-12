using UnityEngine;
using UnityEngine.UI;

public class Damagable : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    private float currHealth;
    [SerializeField] private Image healthUI;
    private Rigidbody2D rb;


    [SerializeField] private AudioClip hitSound;
    private AudioSource audioSource;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        currHealth = maxHealth;   
    }

    private void ChangeHealth(float delta) {
        currHealth += delta;
        healthUI.fillAmount = currHealth / maxHealth;
    }

    public void RestoreHealth() {
        currHealth = maxHealth;
        healthUI.fillAmount = currHealth / maxHealth;
    }

    public void Hit(Vector2 damagePos, float damage, float knockback) {
        ChangeHealth(-damage);
        if(currHealth <= 0) {
            Destroy(gameObject);
        }
        Vector3 damageVector = ((Vector2) transform.position - damagePos).normalized * knockback;
        rb.AddForce(damageVector);

        if (hitSound != null) {
            audioSource.PlayOneShot(hitSound);
        }
    }
}
