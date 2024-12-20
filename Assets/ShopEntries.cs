using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopEntries : MonoBehaviour
{
    [SerializeField] private GameObject entryPrefab;
    [SerializeField] private List<ShopEntryData> entries;

    void Start() {
        foreach (ShopEntryData shopEntryData in entries) {
            ShopEntry shopEntry = Instantiate(entryPrefab, transform).GetComponent<ShopEntry>();
            shopEntry.SetShopEntryData(shopEntryData);
        }
    }
}
