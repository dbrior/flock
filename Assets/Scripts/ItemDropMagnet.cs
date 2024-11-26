using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropMagnet : MonoBehaviour
{
    [SerializeField] private float pullSpeed;
    public void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.TryGetComponent<ItemDrop>(out ItemDrop item)) {
            item.SuckedInBy(gameObject, pullSpeed);
        }
    }
}
