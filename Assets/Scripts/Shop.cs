using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private GameObject shopWindow;

    private Player lastActivePlayer;
    public void OpenShop(GameObject playerObj) {
        Player player = playerObj.GetComponent<Player>();
        player.OpenMenu();
        lastActivePlayer = player;

        shopWindow.SetActive(true);
    }

    public void CloseShop() {
        lastActivePlayer.CloseMenu();
        shopWindow.SetActive(false);
    }

    private bool CanAfford(Item currency, int cost) {
        return PlayerInventory.Instance.GetItemCount(currency) >= cost;
    }

    private void RemoveCurrency(Item currency, int cost) {
        PlayerInventory.Instance.RemoveItem(currency, cost);
    }

    private void GiveItem(Item item, int amount) {
        PlayerInventory.Instance.AddItem(item, amount);
    }

    public void AttemptPurchase(ShopEntry shopEntry) {
        Item item = shopEntry.item;
        Item currency = shopEntry.currency;
        int cost = shopEntry.cost;

        Debug.Log("buyss");

        if (CanAfford(currency, cost)) {
            RemoveCurrency(currency, cost);

            // Special handling of healing & damage increase
            if (item.itemName == "Damage") {
                Debug.Log("Damage Upgrade");
                lastActivePlayer.IncreaseDamage(5f);
            } else if (item.itemName == "Heal") {
                Debug.Log("Heal");
                lastActivePlayer.Heal();
            } else if (item.itemName == "FarmRadius") {
                Debug.Log("Farm increase");
                GetComponent<FarmPlot>().IncreaseRadius(1);
            } else if (item.itemName == "HunterFireRate") {
                HunterManager.Instance.IncreaseFireRate(1f);
            } else {
                GiveItem(item, 1);
            }
        }
    }

    // public void PurchaseHeal(Item currency, int cost) {
    //     if (CanAfford(currency, cost)) {
    //         RemoveCurrency(currency, cost);
    //         lastActivePlayer.Heal();
    //     }
    // }

    // public void PurchaseDamage(Item currency, int cost) {
    //     if (CanAfford(currency, cost)) {
    //         RemoveCurrency(currency, cost);
    //         lastActivePlayer.IncreaseDamage(5f);
    //     }
    // }
}