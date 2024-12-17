using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Inventory : MonoBehaviour
{
    private Dictionary<Item, int> inventory = new Dictionary<Item, int>();

    [SerializeField] private List<Item> items = new List<Item>();
    [SerializeField] private List<TextMeshProUGUI> uiElements = new List<TextMeshProUGUI>();

    private Dictionary<Item, TextMeshProUGUI> uiMappings = new Dictionary<Item, TextMeshProUGUI>();

    private Worker worker;

    protected virtual void Awake() {
        worker = GetComponent<Worker>();
        for (int i = 0; i < items.Count && i < uiElements.Count; i++) {
            uiMappings[items[i]] = uiElements[i];
        }
    }

    private void UpdateItemUI(Item item, int count)
    {
        if (uiMappings.TryGetValue(item, out TextMeshProUGUI uiElement)) {
            uiElement.text = count.ToString();
        }
    }

    public virtual void AddItem(Item item, int count)
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