using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    private bool suckedIn;
    private float suckSpeed;
    private GameObject target;
    private Rigidbody2D rb;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        suckedIn = false;
    }
    void FixedUpdate() {
        if (suckedIn) {
            Vector2 targetVel = (Vector2) (target.transform.position - transform.position).normalized * suckSpeed;
            Vector2 velDelta = targetVel - rb.velocity;
            Vector2 requiredAccel = velDelta / Time.fixedDeltaTime;
            rb.AddForce(requiredAccel * rb.mass);
        }
    }
    public void SuckedInBy(GameObject targetObj, float speed) {
        target = targetObj;
        suckSpeed = speed;
        suckedIn = true;
    }

    public void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.TryGetComponent<Player>(out Player player)) {
            player.CollectItem(this);
        }
    }
}
