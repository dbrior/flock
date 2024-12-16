using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    private bool suckedIn;
    private float suckSpeed;
    private GameObject target;
    private Collider2D collider;
    public int amount {get; private set;}
    private Vector3 originalScale;
    [SerializeField] public Item item;
    private float depsawnSec = 30f;
    private float aliveSec = 0f;

    void Awake() {
        collider = GetComponent<Collider2D>();
        originalScale = transform.localScale;
        suckedIn = false;
        amount = 1;
    }

    void Start() {
        StartCoroutine("ScanForItemDrops");
    }

    void Update() {
        aliveSec += Time.deltaTime;
        if (aliveSec > depsawnSec) {
            Destroy(gameObject);
        }

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

    public void StopAggregation() {
        StopCoroutine("ScanForItemDrops");
    }

    public void SuckedInBy(ItemDropMagnet targetMagnet, float speed) {
        if (suckedIn) return;
        targetMagnet.CollectItem(this);
        
        // target = targetObj;
        // suckSpeed = speed;
        suckedIn = true;
    }

    IEnumerator ScanForItemDrops() {
        while (true) {
            yield return new WaitForSeconds(2f);
            Collider2D[] itemDropsInRange = Physics2D.OverlapCircleAll(transform.position, 0.6f, LayerMask.GetMask("ItemDrop"));
            int mergedCount = 0;
            Vector3 sumWeightedPositions = transform.position * amount;
            int sumWeights = amount;
            for (int i=0; i<itemDropsInRange.Length; i++) {
                if (itemDropsInRange[i] == collider) continue;

                ItemDrop itemDrop = itemDropsInRange[i].GetComponent<ItemDrop>();
                if (itemDrop != null && itemDrop.item == item) {
                    amount += itemDrop.amount;
                    itemDrop.StopAggregation();

                    transform.localScale = (1f + (0.05f * amount)) * originalScale;
                    // transform.localScale = Mathf.Min(1f + (0.1f * amount), 1.3f) * originalScale;
                    aliveSec = 0;

                    sumWeightedPositions += itemDrop.transform.position * itemDrop.amount;
                    sumWeights += itemDrop.amount;

                    mergedCount += 1;

                    Destroy(itemDrop.gameObject);
                }
            }

            if (mergedCount > 0) {
                transform.position = sumWeightedPositions / sumWeights;
            }
        }
    }
}
