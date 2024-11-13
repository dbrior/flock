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

    public void PurchaseHeal(int cost) {
        lastActivePlayer.PurchaseHeal(cost);
    }

    public void PurchaseDamage(int cost) {
        lastActivePlayer.PurchaseDamage(5f, cost);
    }
}