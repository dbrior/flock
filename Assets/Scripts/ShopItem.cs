using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Item/ShopItem")]
public class ShopItem : ScriptableObject
{
    public string itemName;
    public int price;
    public Sprite sprite;
    public UpgradeType upgradeType;
}