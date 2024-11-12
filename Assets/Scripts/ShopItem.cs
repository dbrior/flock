using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "NewItem", menuName = "Item/ShopItem")]
public class ShopItem : ScriptableObject
{
    public string itemName;
    public int price;
    public Sprite sprite;
    public UpgradeType upgradeType;

    private void OnValidate()
    {
        if (sprite != null)
        {
            EditorGUIUtility.SetIconForObject(this, sprite.texture);
        }
    }
}
