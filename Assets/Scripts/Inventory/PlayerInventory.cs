using UnityEngine;

public class PlayerInventory : Inventory
{
    public static PlayerInventory Instance { get; private set;}

    void Awake() {
        if (Instance == null) {Instance = this;}
        else {Destroy(gameObject);}
        
        base.Awake();
    }

    public override void AddItem(Item item, int count) {
        base.AddItem(item, count);
        QuestManager.Instance.CollectItem(item, count);
    }
}
