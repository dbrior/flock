using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopTable : MonoBehaviour
{
    [SerializeField] private Item item;
    [SerializeField] private Item currency;
    [SerializeField] private int cost;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private Interactable interactable;

    void Start() {
        interactable = GetComponent<Interactable>();
        interactable.interactionText = item.itemName.ToUpper() + "\n" + cost.ToString() + "    " + currency.itemName.ToUpper();
    }

    public void Purchase(GameObject playerGameObject)
    {
        Player player = playerGameObject.GetComponent<Player>();
        int currCurrency = PlayerInventory.Instance.GetItemCount(currency);

        if (currCurrency >= cost) {
            PlayerInventory.Instance.RemoveItem(currency, cost);
            // player.AddUpgrade(item.upgradeType);
        }
    }
}
