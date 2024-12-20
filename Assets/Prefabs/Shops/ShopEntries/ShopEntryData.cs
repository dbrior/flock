using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public struct ShopEntryData {
    public string entryName;
    public List<Cost> costs;
    public float costScaling;
    public UnityEvent onPurchase;
    public UnityEvent<Player> onPurchasePlayer;
    public int maxPurchaseCount;
    public GameObject nextShopEntry;
}
