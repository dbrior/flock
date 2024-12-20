using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CurrencyUI : MonoBehaviour
{
    public Image currencyIcon;
    public TextMeshProUGUI costText;

    public void SetCostData(Cost newCost) {
        currencyIcon.sprite = newCost.currency.sprite;
        costText.text = UIManager.FormatNumber(newCost.amount);
    }
}
