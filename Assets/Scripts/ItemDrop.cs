using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    private bool suckedIn;
    private float suckSpeed;
    private GameObject target;
    private Rigidbody2D rb;

    [SerializeField] public Item item;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        suckedIn = false;
    }
    void FixedUpdate() {
        if (suckedIn) {
            if (target == null) {
                suckedIn = false;
                return;
            }

            Vector2 distance = (Vector2) (target.transform.position - transform.position);
            if (distance.magnitude <= 0.05f) {
                target.GetComponent<ItemDropMagnet>().CollectItem(this);
            }

            Vector2 targetVel = distance.normalized * suckSpeed;
            Vector2 velDelta = targetVel - rb.velocity;
            Vector2 requiredAccel = velDelta / Time.fixedDeltaTime;
            rb.AddForce(requiredAccel * rb.mass);
        }
    }
    public void SuckedInBy(GameObject targetObj, float speed) {
        if (suckedIn) return;
        
        target = targetObj;
        suckSpeed = speed;
        suckedIn = true;
    }
    // public void OnTriggerStay2D(Collider2D col) {
    //     float objDistance = (col.gameObject.transform.position - transform.position).magnitude;
    //     if (objDistance <= 0.01 && col.gameObject.TryGetComponent<Player>(out Player player)) {
    //         player.CollectItem(this);
    //     }
    // }
}
