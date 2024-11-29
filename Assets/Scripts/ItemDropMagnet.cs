using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropMagnet : MonoBehaviour
{
    [SerializeField] private float pullSpeed;
    [SerializeField] private AudioClip collectSound;
    private AudioSource audioSource;

    void Start() {
        audioSource = GetComponent<AudioSource>();
    }
    public void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.TryGetComponent<ItemDrop>(out ItemDrop item)) {
            item.SuckedInBy(gameObject, pullSpeed);
        }
    }

    public void CollectItem(ItemDrop itemDrop) {
        audioSource.PlayOneShot(collectSound);
        PlayerInventory.Instance.AddItem(itemDrop.item, 1);
        Destroy(itemDrop.gameObject);
    }
}
