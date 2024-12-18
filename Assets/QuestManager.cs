using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public enum QuestType : int {
    PrimaryWeapon = 0,
    SecondaryWeapon = 1,
    Capture = 2,
    Collect = 3,
    Plant = 4,
    Kill = 5,
    Purchase = 6
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
    public string textOverride;

    public int currAmount = 0;
    public bool isComplete = false;
    public QuestTaskUI taskUI;

    public string GetTargetName() {
        if (type == QuestType.Capture) {
            return creatureType.ToString();
        } else if (type == QuestType.Collect) {
            return item.itemName.ToString();
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
    public int questId;
    public string title;
    public List<QuestTask> tasks;
    public Item reward;
    public int amount;
    public Quest nextQuest;

    public bool isComplete;
    public QuestUI questUI;

    public void CheckComplete() {
        foreach (QuestTask task in tasks) {
            if (!task.isComplete) return;
        }

        if (!isComplete) CompleteQuest();
    }

    private void CompleteQuest() {
        isComplete = true;
        string prefPath = "Quest-" + questId.ToString() + "-IsComplete";
        PlayerPrefs.SetInt(prefPath, 1);
        questUI.CompleteQuest();
        if (nextQuest != null) {
            QuestManager.Instance.AddQuest(nextQuest);
        }
    }

    public Quest Clone() {
        Quest clone = Instantiate(this);
        clone.tasks = new List<QuestTask>();
        foreach (var task in tasks) {
            QuestTask taskCopy = new QuestTask {
                type = task.type,
                amount = task.amount,
                item = task.item,
                creatureType = task.creatureType,
                unitType = task.unitType,
                cropType = task.cropType,
                currAmount = 0, // Reset progress
                isComplete = false,
                textOverride = task.textOverride,
                taskUI = null // UI not cloned
            };
            clone.tasks.Add(taskCopy);
        }
        return clone;
    }
}

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance {get; private set;}

    [SerializeField] private Transform questContainer;
    [SerializeField] private List<Quest> activeQuests = new List<Quest>();
    [SerializeField] private Quest startingQuest;
    
    [SerializeField] private GameObject questUIPrefab;

    [SerializeField] private List<Quest> allQuests;

    void Awake() {
        if (Instance == null) {Instance = this;}
        else {Destroy(gameObject);}

        for (int i=0; i<allQuests.Count; i++) {
            string prefPath = "Quest-" + i.ToString() + "-IsComplete";
            if (PlayerPrefs.GetInt(prefPath, 0) == 0) {
                AddQuest(allQuests[i]);
                break;
            }
        }
    }

    public void AddQuest(Quest newQuest) {
        Quest questClone = newQuest.Clone();
        activeQuests.Add(questClone);
        QuestUI newQuestUI = Instantiate(questUIPrefab, questContainer).GetComponent<QuestUI>();
        newQuestUI.SetQuest(questClone);
        questClone.questUI = newQuestUI;
    }

    public void RemoveQuest(Quest quest) {
        activeQuests.Remove(quest);
        Destroy(quest.questUI.gameObject);
    }

    // Quest progress functions
    public void CreatureKill(CreatureType creatureType) {
        // TODO: switch to dictionary lookup
        foreach (Quest quest in activeQuests) {
            foreach (QuestTask task in quest.tasks) {
                if ((task.type != QuestType.Kill) || (task.creatureType != creatureType)) continue;
                task.currAmount += 1;

                // TODO: set on complete function
                if (task.currAmount >= task.amount) {
                    task.isComplete = true;
                    task.taskUI.CompleteTask();

                    quest.CheckComplete();
                }

                task.taskUI.SetText();
            }
        }
    }

    public void CollectItem(Item item, int amount) {
        foreach (Quest quest in activeQuests) {
            foreach (QuestTask task in quest.tasks) {
                if (task.isComplete) continue;
                if ((task.type != QuestType.Collect) || (task.item != item)) continue;

                task.currAmount += amount;

                // TODO: set on complete function
                if (task.currAmount >= task.amount) {
                    task.isComplete = true;
                    task.taskUI.CompleteTask();

                    quest.CheckComplete();
                }

                task.taskUI.SetText();
            }
        }
    }

    public void PlantCrop(CropType cropType) {
        foreach (Quest quest in activeQuests) {
            foreach (QuestTask task in quest.tasks) {
                if ((task.type != QuestType.Plant) || (task.cropType != cropType)) continue;
                task.currAmount += 1;

                // TODO: set on complete function
                if (task.currAmount >= task.amount) {
                    task.isComplete = true;
                    task.taskUI.CompleteTask();

                    quest.CheckComplete();
                }

                task.taskUI.SetText();
            }
        }
    }

    public void CaptureCreature(CreatureType creatureType) {
        foreach (Quest quest in activeQuests) {
            foreach (QuestTask task in quest.tasks) {
                if ((task.type != QuestType.Capture) || (task.creatureType != creatureType)) continue;
                task.currAmount += 1;

                // TODO: set on complete function
                if (task.currAmount >= task.amount) {
                    task.isComplete = true;
                    task.taskUI.CompleteTask();

                    quest.CheckComplete();
                }

                task.taskUI.SetText();
            }
        }
    }

    public void PurchaseUnit(UnitType unitType) {
        foreach (Quest quest in activeQuests) {
            foreach (QuestTask task in quest.tasks) {
                if ((task.type != QuestType.Purchase) || (task.unitType != unitType)) continue;
                task.currAmount += 1;

                // TODO: set on complete function
                if (task.currAmount >= task.amount) {
                    task.isComplete = true;
                    task.taskUI.CompleteTask();

                    quest.CheckComplete();
                }

                task.taskUI.SetText();
            }
        }
    }

    public void PrimaryWeapon() {
        foreach (Quest quest in activeQuests) {
            foreach (QuestTask task in quest.tasks) {
                if (task.type != QuestType.PrimaryWeapon) continue;
                task.currAmount += 1;

                // TODO: set on complete function
                if (task.currAmount >= task.amount) {
                    task.isComplete = true;
                    task.taskUI.CompleteTask();

                    quest.CheckComplete();
                }

                task.taskUI.SetText();
            }
        }
    }

    public void SecondaryWeapon() {
        foreach (Quest quest in activeQuests) {
            foreach (QuestTask task in quest.tasks) {
                if (task.type != QuestType.SecondaryWeapon) continue;
                task.currAmount += 1;

                // TODO: set on complete function
                if (task.currAmount >= task.amount) {
                    task.isComplete = true;
                    task.taskUI.CompleteTask();

                    quest.CheckComplete();
                }

                task.taskUI.SetText();
            }
        }
    }
}
