using UnityEngine;
using TMPro;

public class QuestTaskUI : MonoBehaviour
{
    private TextMeshProUGUI textUI;
    private QuestTask task;
    [SerializeField] private Color completeColor;

    void Awake() {
        textUI = GetComponent<TextMeshProUGUI>();
    }

    public void SetTask(QuestTask newTask) {
        task = newTask;
    }

    public void SetText() {
        if (task.textOverride != null && task.textOverride != "") {
            textUI.text = (task.isComplete ? "[X]" : "[    ]") + "    " + task.textOverride;
        } else {
            textUI.text = (task.isComplete ? "[X]" : "[    ]") + "    " + task.type.ToString().ToUpper() + "    " + Mathf.Max(task.amount - task.currAmount, 0) + "    " + task.GetTargetName().ToUpper();
        }

        if (task.isComplete) {
            textUI.fontStyle |= FontStyles.Strikethrough;
        }
    }

    public void CompleteTask() {
        task.isComplete = true;
        textUI.color = completeColor;
        SetText();
    }
}
