using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public struct Cost {
    public Item currency;
    public int amount;
    public TextMeshProUGUI costText;

    public Cost(Item currency, int amount, TextMeshProUGUI costText) {
        this.currency = currency;
        this.amount = amount;
        this.costText = costText;

        this.costText.text = amount.ToString();
    }

    public void SetPrice(int newPrice) {
        this.amount = newPrice;
        this.costText.text = amount.ToString();
    }
}

public class ShopEntry : MonoBehaviour
{
    [SerializeField] private List<Cost> costs;
    [SerializeField] private float costScaling;
    [SerializeField] private UnityEvent onPurchase;
    [SerializeField] private UnityEvent<Player> onPurchasePlayer;

    private Shop shop;
    private Button button;
    private int purchaseCount = 0;

    void Awake() {
        button = GetComponent<Button>();
    }

    void Start() {
        shop = GetComponentInParent<Shop>();
        button.onClick.AddListener(AttemptPurchase);
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

    private void PurchaseComplete() {
        purchaseCount += 1;

        onPurchase?.Invoke();
        onPurchasePlayer?.Invoke(shop.lastActivePlayer);

        foreach (Cost cost in costs) {
            int newPrice = Mathf.RoundToInt((1f + costScaling) * cost.amount);
            cost.SetPrice(newPrice);
        }
    }

    public void AttemptPurchase() {
        // Check if we can afford
        bool canAfford = true;
        foreach (Cost cost in costs) {
            Item currency = cost.currency;
            int amount = cost.amount;

            if (!CanAfford(currency, amount)) {
                canAfford = false;
                break;
            }
        }
        if (!canAfford) return;

        // Take payment
        foreach (Cost cost in costs) {
            Item currency = cost.currency;
            int amount = cost.amount;

            RemoveCurrency(currency, amount);
        }

        PurchaseComplete();
    }
}
