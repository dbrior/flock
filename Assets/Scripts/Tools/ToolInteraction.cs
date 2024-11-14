using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Interaction {
    public Tool tool;
    public UnityEvent action;

    void Init(Tool tool, UnityEvent action) {
        this.tool = tool;
        this.action = action;
    }
}

public class ToolInteraction : MonoBehaviour
{
    [SerializeField] private List<Interaction> toolInteractions;

    public void UseTool(Tool tool) {
        foreach (Interaction interaction in toolInteractions) {
            if (interaction.tool == tool) {
                interaction.action.Invoke();
            }
        }
    }
}