using UnityEngine;
using UnityEngine.Events;

public class ShopEntry : MonoBehaviour
{
    [SerializeField] private Shop shop;
    [SerializeField] public Item currency;
    [SerializeField] public int cost;
    [SerializeField] public Item item;
}
