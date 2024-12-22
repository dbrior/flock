using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

[System.Serializable]
public struct ItemAction {
    public Item item;
    public UnityEvent onGet;
    public UnityEvent onEmpty;
}

public class Inventory : MonoBehaviour
{
    private Dictionary<Item, int> inventory = new Dictionary<Item, int>();


    // TODO: move to custom struct
    [SerializeField] private List<Item> items = new List<Item>();
    [SerializeField] private List<TextMeshProUGUI> uiElements = new List<TextMeshProUGUI>();
    [SerializeField] private List<ItemAction> itemActionsList;

    private Dictionary<Item, TextMeshProUGUI> uiMappings = new Dictionary<Item, TextMeshProUGUI>();
    private Dictionary<Item, ItemAction> itemActions = new Dictionary<Item, ItemAction>();

    private Worker worker;

    protected virtual void Awake() {
        worker = GetComponent<Worker>();
        for (int i = 0; i < items.Count && i < uiElements.Count; i++) {
            uiMappings[items[i]] = uiElements[i];
        }
        for (int i=0; i<itemActionsList.Count; i++) {
            ItemAction itemAction = itemActionsList[i];
            itemActions[itemAction.item] = itemAction;
        }
    }

    private void UpdateItemUI(Item item, int count)
    {
        if (uiMappings.TryGetValue(item, out TextMeshProUGUI uiElement)) {
            uiElement.text = UIManager.FormatNumber(count);
        }
    }

    public virtual void AddItem(Item item, int count, bool ignoreQuest = false)
    {
        if (item.itemName == "Chest") {
            CardManager.Instance.ShowCards();
            return;
        }
        if (item.itemName == "XP") {
            XPManager.Instance.AddXp(1f);
            return;
        }

        int newCount = count;
        if (inventory.TryGetValue(item, out int currCount)) {
            newCount += inventory[item];
        } else {
            if (itemActions.TryGetValue(item, out ItemAction itemAction)) {
                itemAction.onGet?.Invoke();
            }
        }
        inventory[item] = newCount;
        UpdateItemUI(item, newCount);

        if (worker != null) {
            worker.ReceivedItem(item);
        }
    }

    public void RemoveItem(Item item, int count)
    {
        if (inventory.TryGetValue(item, out int currCount))
        {
            int newCount = Mathf.Max(currCount - count, 0);

            if (newCount == 0) {
                inventory.Remove(item);
                if (itemActions.TryGetValue(item, out ItemAction itemAction)) {
                    itemAction.onEmpty?.Invoke();
                }
            } else {
                inventory[item] = newCount;
            }
            UpdateItemUI(item, newCount);
        }
    }

    public int GetItemCount(Item item)
    {
        return inventory.TryGetValue(item, out int currCount) ? currCount : 0;
    }
}