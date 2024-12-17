using UnityEngine;
using TMPro;

public class QuestTaskUI : MonoBehaviour
{
    private TextMeshProUGUI textUI;
    private QuestTask task;

    void Awake() {
        textUI = GetComponent<TextMeshProUGUI>();
    }

    public void SetTask(QuestTask newTask) {
        task = newTask;
    }

    public void SetText() {
        textUI.text = "[    ]    " + task.type.ToString().ToUpper() + "    " + Mathf.Max(task.amount - task.currAmount, 0) + "    " + task.GetTargetName().ToUpper();
    }

    public void SetColor(Color newColor) {
        textUI.color = newColor;
    }
}
