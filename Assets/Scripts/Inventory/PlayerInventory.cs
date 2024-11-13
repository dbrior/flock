using UnityEngine;

public class PlayerInventory : Inventory
{
    public static PlayerInventory inventory { get; private set;}

    void Awake() {
        if (inventory == null) {inventory = this;}
        else {Destroy(gameObject);}
    }
}
