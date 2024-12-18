using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewQuest", menuName = "Quest/New Quest", order = 1)]
public class Quest : ScriptableObject {
    public int questId;
    public string title;
    [SerializeField] public List<QuestTask> tasks;
    [SerializeField] public Item reward;
    public int amount;
    [SerializeField] public Quest nextQuest;

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
