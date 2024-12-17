using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private Transform taskContainer;
    [SerializeField] private GameObject questTaskUIPrefab;

    private Quest quest;
    private List<QuestTaskUI> taskUIList = new List<QuestTaskUI>();

    public void SetQuest(Quest newQuest) {
        quest = newQuest;

        foreach (QuestTask task in quest.tasks) {
            task.taskUI = AddTask(task);
        }
    }

    public QuestTaskUI AddTask(QuestTask task) {
        QuestTaskUI newTaskUI = Instantiate(questTaskUIPrefab, taskContainer).GetComponent<QuestTaskUI>();
        newTaskUI.SetTask(task);
        newTaskUI.SetText();
        taskUIList.Add(newTaskUI);
        return newTaskUI;
    }
}
