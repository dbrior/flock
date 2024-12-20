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

        if (this.costText != null) {
            this.costText.text = amount.ToString();
        }
    }

    public void SetPrice(int newPrice) {
        this.amount = newPrice;
        if (this.costText != null) {
            this.costText.text = UIManager.FormatNumber(amount);
        }
    }
}

[System.Serializable]
public class ShopEntry : MonoBehaviour
{
    [SerializeField] private GameObject currencyPrefab;
    public GameObject currencyContainer;
    public TextMeshProUGUI entryText;
    [SerializeField] private List<Cost> costs;
    [SerializeField] private float costScaling;
    [SerializeField] private UnityEvent onPurchase;
    [SerializeField] private UnityEvent<Player> onPurchasePlayer;
    [SerializeField] private int maxPurchaseCount;
    [SerializeField] private GameObject nextShopEntry;

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

    public void SetShopEntryData(ShopEntryData newData) {
        entryText.text = newData.entryName;
        costs = newData.costs;
        costScaling = newData.costScaling;
        onPurchase = newData.onPurchase;
        onPurchasePlayer = newData.onPurchasePlayer;
        maxPurchaseCount = newData.maxPurchaseCount;
        nextShopEntry = newData.nextShopEntry;

        for  (int i=0; i<costs.Count; i++) {
            Cost cost = costs[i];
            CurrencyUI currencyUI = Instantiate(currencyPrefab, currencyContainer.transform).GetComponent<CurrencyUI>();
            currencyUI.SetCostData(cost);
            cost.costText = currencyUI.costText;
            costs[i] = cost;
        }
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
        
        // Update each cost with scaled price
        for (int i = 0; i < costs.Count; i++) {
            Cost currentCost = costs[i];
            int newPrice = Mathf.RoundToInt(currentCost.amount * (1f + costScaling));
            currentCost.SetPrice(newPrice);
            
            // Important: Update the list with the modified cost
            costs[i] = currentCost;
        }

        // Destroy if at max purchase count
        if (maxPurchaseCount != 0 && purchaseCount >= maxPurchaseCount) {
            if (nextShopEntry != null) nextShopEntry.SetActive(true);
            Destroy(gameObject);
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
