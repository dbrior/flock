using UnityEngine;
using UnityEngine.UI;

public class ShopExitButton : MonoBehaviour
{
    private Button button;
    private Shop shop;

    void Awake() {
        button = GetComponent<Button>();
        shop = GetComponentInParent<Shop>();
    }

    void Start() {
        Debug.Log(shop.gameObject.name);
        button.onClick.AddListener(shop.CloseShop);
    }
}
