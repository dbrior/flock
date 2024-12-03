using UnityEngine;
using TMPro;

public class CostCurrency : MonoBehaviour
{
    public TextMeshProUGUI costText;

    void Start() {
        GetComponentInParent<ShopEntry>().costCurrencies.Add(this);
    }
}
