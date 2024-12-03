using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

public class ShopEntry : MonoBehaviour
{
    private Shop shop;
    Button button;

    [SerializeField] public Item currency;
    [SerializeField] public int cost;
    [SerializeField] public Item item;
    public float costScaling;
    public int purchaseCount = 0;
    public List<CostCurrency> costCurrencies;

    void Start() {
        shop = GetComponentInParent<Shop>();
        button = GetComponent<Button>();

        button.onClick.AddListener(() => shop.AttemptPurchase(this));
    }

    public void PurchaseComplete() {
        cost = Mathf.RoundToInt((1f + costScaling) * cost);
        foreach (CostCurrency costCurrency in costCurrencies) {
            costCurrency.costText.text = cost.ToString();
        }
    }

}
