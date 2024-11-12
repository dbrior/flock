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
}
