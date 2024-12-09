using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropMagnet : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private float pullSpeed;
    [SerializeField] private AudioClip collectSound;
    [SerializeField] private List<Item> filter = new List<Item>();
    private AudioSource audioSource;

    void Start() {
        audioSource = GetComponent<AudioSource>();
    }
    public void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.TryGetComponent<ItemDrop>(out ItemDrop item)) {
            if (filter.Count > 0 && !filter.Contains(item.item)) return;

            item.SuckedInBy(gameObject, pullSpeed);
        }
    }

    public void CollectItem(ItemDrop itemDrop) {
        if (filter.Count > 0 && !filter.Contains(itemDrop.item)) return;

        audioSource.PlayOneShot(collectSound);

        if (inventory == null) {
            PlayerInventory.Instance.AddItem(itemDrop.item, 1);
        } else {
            inventory.AddItem(itemDrop.item, 1);
        }

        Destroy(itemDrop.gameObject);
    }
}
