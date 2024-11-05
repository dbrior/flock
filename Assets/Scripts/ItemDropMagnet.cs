using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropMagnet : MonoBehaviour
{
    [SerializeField] private float pullSpeed;
    public void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.TryGetComponent<ItemDrop>(out ItemDrop item)) {
            // Debug.Log("item in range");
            item.SuckedInBy(gameObject, pullSpeed);
        }
    }
}
