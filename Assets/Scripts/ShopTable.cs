using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopTable : MonoBehaviour
{
    [SerializeField] private ShopItem item;
    [SerializeField] private SpriteRenderer spriteRenderer;

    void OnValidate() {
        if (spriteRenderer != null && item != null) {
            spriteRenderer.sprite = item.sprite;
        }
    }

    public void Purchase(GameObject playerGameObject)
    {
        Player player = playerGameObject.GetComponent<Player>();
        if (player != null && player.GetWoolCount() >= item.price)
        {
            player.AdjustWoolCount(-item.price);
            player.AddUpgrade(item.upgradeType);
        }
    }
}
