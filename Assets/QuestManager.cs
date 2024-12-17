using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public enum QuestType {
    Capture,
    Collect,
    Plant,
    Kill,
    Purchase
}

[System.Serializable]
public enum CreatureType {
    None,

    // Tamable
    Sheep,

    // Enemies
    Wolf,
    Eyeball,
    Golem,
    Boss
}

[System.Serializable]
public class QuestTask {
    public QuestType type;
    public int amount;
    public Item item;
    public CreatureType creatureType;
    public UnitType unitType;
    public CropType cropType;

    public int currAmount = 0;
    public bool isComplete = false;
    public QuestTaskUI taskUI;

    public string GetTargetName() {
        if (type == QuestType.Capture) {
            return creatureType.ToString();
        } else if (type == QuestType.Collect) {
            return item.name.ToString();
        } else if (type == QuestType.Plant) {
            return cropType.ToString();
        } else if (type == QuestType.Kill) {
            return creatureType.ToString();
        } else if (type == QuestType.Purchase) {
            return unitType.ToString();
        } else {
            return "None";
        }
    }
}

[CreateAssetMenu()]
public class Quest : ScriptableObject {
    public string title;
    public List<QuestTask> tasks;
    public Item reward;
    public int amount;
}

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance {get; private set;}

    [SerializeField] private Transform questContainer;
    [SerializeField] private List<Quest> activeQuests = new List<Quest>();
    [SerializeField] private Quest startingQuest;
    [SerializeField] private Color completeColor;
    
    [SerializeField] private GameObject questUIPrefab;

    void Awake() {
        if (Instance == null) {Instance = this;}
        else {Destroy(gameObject);}

        AddQuest(startingQuest);
    }

    public void AddQuest(Quest newQuest) {
        activeQuests.Add(newQuest);
        QuestUI newQuestUI = Instantiate(questUIPrefab, questContainer).GetComponent<QuestUI>();
        newQuestUI.SetQuest(newQuest);
    }

    // Quest progress functions
    public void CreatureKill(CreatureType creatureType) {
        // TODO: switch to dictionary lookup
        foreach (Quest quest in activeQuests) {
            foreach (QuestTask task in quest.tasks) {
                if ((task.type != QuestType.Kill) || (task.creatureType != creatureType)) continue;
                task.currAmount += 1;
                task.taskUI.SetText();

                // TODO: set on complete function
                if (task.currAmount >= task.amount) {
                    task.isComplete = true;
                    task.taskUI.SetColor(completeColor);
                }
            }
        }
    }

    public void CollectItem(Item item, int amount) {
        foreach (Quest quest in activeQuests) {
            foreach (QuestTask task in quest.tasks) {
                if (task.isComplete) continue;
                if ((task.type != QuestType.Collect) || (task.item != item)) continue;

                task.currAmount += amount;
                task.taskUI.SetText();

                // TODO: set on complete function
                if (task.currAmount >= task.amount) {
                    task.isComplete = true;
                    task.taskUI.SetColor(completeColor);
                }
            }
        }
    }

    public void PlantCrop(CropType crop) {

    }

    public void CaptureCreature(CreatureType creature) {
        
    }
}
