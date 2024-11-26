using UnityEngine;

public class PlayerInventory : Inventory
{
    public static PlayerInventory Instance { get; private set;}

    void Awake() {
        if (Instance == null) {Instance = this;}
        else {Destroy(gameObject);}
    }
}
