using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PrestigeShopEntry : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI purchaseCountUI;
    [SerializeField] private TextMeshProUGUI priceUI;

    public string prefEntry {get; private set;}
    public string stat {get; private set;}
    public int purchaseCount {get; private set;}
    public int price {get; private set;}

    private int baseCost = 100;
    private float scaleFactor = 1.75f;

    private PrestigeShop shop;
    private Button button;


    void Awake() {
        button = GetComponent<Button>();
        stat = gameObject.name;
        shop = GetComponentInParent<PrestigeShop>();

        shop.AddEntry(this);
        button.onClick.AddListener(() => shop.AttemptPurchase(this));
    }

    private void SetUI() {
        purchaseCountUI.text = "LVL    " + purchaseCount.ToString();
        priceUI.text = price.ToString();
    }

    private void SetFields() {
        purchaseCount = PlayerPrefs.GetInt(prefEntry + "-PurchaseCount", 0);
        price = Mathf.RoundToInt(baseCost * Mathf.Pow(scaleFactor, purchaseCount));

        SetUI();
    }

    public void PullData(string targetEntity) {
        prefEntry = targetEntity + "-" + stat;
        SetFields();
    }

    public void CompletePurchase() {
        purchaseCount += 1;
        string prefIndex = prefEntry + "-PurchaseCount";
        Debug.Log("Storing " + purchaseCount + " at " + prefIndex);
        PlayerPrefs.SetInt(prefIndex, purchaseCount);
        SetFields();
    }
}
