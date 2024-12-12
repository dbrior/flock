using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrestigeShop : MonoBehaviour
{
    [SerializeField] private Item prestigePointItem;

    private List<PrestigeShopEntry> entries = new List<PrestigeShopEntry>();
    
    private int currPrestigePoints;
    private Inventory inventory;
    private string currMenu;

    void Awake() {
        PlayerPrefs.SetInt("PrestigePoints", 0);
        inventory = GetComponent<Inventory>();
        currPrestigePoints = PlayerPrefs.GetInt("PrestigePoints", 0);
    }

    void Start() {
        inventory.AddItem(prestigePointItem, currPrestigePoints);
    }

    public void AddEntry(PrestigeShopEntry newEntry) {
        entries.Add(newEntry);
    }
    
    public void SetMenu(string newMenu) {
        currMenu = newMenu;
        foreach (PrestigeShopEntry entry in entries) {
            entry.PullData(currMenu);
        }
    }

    public string GetMenu() {
        return currMenu;
    }

    public void AttemptPurchase(PrestigeShopEntry entry) {
        if (inventory.GetItemCount(prestigePointItem) >= entry.price) {
            inventory.RemoveItem(prestigePointItem, entry.price);
            PlayerPrefs.SetInt("PrestigePoints", inventory.GetItemCount(prestigePointItem));
            entry.CompletePurchase();
        }
    }
}
