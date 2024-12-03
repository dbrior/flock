using UnityEngine;
using TMPro;

public class CostCurrency : MonoBehaviour
{
    public TextMeshProUGUI costText;
    public Item currency;
    public int cost;

    void Start() {
        GetComponentInParent<ShopEntry>().costCurrencies.Add(this);
    }
}
