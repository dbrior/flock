using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    private bool suckedIn;
    private float suckSpeed;
    private GameObject target;
    [SerializeField] public Item item;

    void Awake() {
        suckedIn = false;
    }

    void Update() {
        if (suckedIn) {
            if (target == null) {
                suckedIn = false;
                return;
            }

            Vector2 distance = (Vector2) (target.transform.position - transform.position);
            if (distance.magnitude <= 0.05f) {
                target.GetComponent<ItemDropMagnet>().CollectItem(this);
            }
            Vector2 moveDistance = distance.normalized * Time.deltaTime * suckSpeed;
            transform.position += (Vector3) moveDistance;
        }
    }

    public void SuckedInBy(GameObject targetObj, float speed) {
        if (suckedIn) return;
        
        target = targetObj;
        suckSpeed = speed;
        suckedIn = true;
    }
}
