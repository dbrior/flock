using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class QuestUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private Transform taskContainer;
    [SerializeField] private GameObject questTaskUIPrefab;
    [SerializeField] private Button rewardButton;
    [SerializeField] private Image rewardIcon;
    [SerializeField] private TextMeshProUGUI rewardCount;
    [SerializeField] private HoverEffect buttonScaling;
    [SerializeField] private Color completeColor;

    private Quest quest;
    private List<QuestTaskUI> taskUIList = new List<QuestTaskUI>();

    public void SetQuest(Quest newQuest) {
        quest = newQuest;

        foreach (QuestTask task in quest.tasks) {
            task.taskUI = AddTask(task);
        }

        title.text = quest.title;
        rewardCount.text = rewardCount.text.Replace("1", quest.amount.ToString());

        rewardButton.onClick.AddListener(() => PlayerInventory.Instance.AddItem(quest.reward, quest.amount));
        rewardButton.onClick.AddListener(() => QuestManager.Instance.RemoveQuest(quest));

        rewardIcon.sprite = quest.reward.sprite;
    }

    public void CompleteQuest() {
        rewardButton.enabled = true;
        buttonScaling.enabled = true;

        TextMeshProUGUI buttonText = buttonScaling.GetComponent<TextMeshProUGUI>();
        buttonText.color = completeColor;
        buttonText.text = buttonText.text.Replace("REWARD:", "CLAIM: ");
    }

    public QuestTaskUI AddTask(QuestTask task) {
        QuestTaskUI newTaskUI = Instantiate(questTaskUIPrefab, taskContainer).GetComponent<QuestTaskUI>();
        newTaskUI.SetTask(task);
        newTaskUI.SetText();
        taskUIList.Add(newTaskUI);
        return newTaskUI;
    }
}
