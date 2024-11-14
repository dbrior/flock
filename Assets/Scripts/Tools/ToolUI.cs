using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolUI : MonoBehaviour
{
    private ToolSlot[] toolSlots;
    private int currToolIdx;

    void Start() {
        toolSlots = gameObject.GetComponentsInChildren<ToolSlot>();
        SetActiveTool(0);
    }

    public void SetActiveTool(int toolIdx) {
        for (int i=0; i<toolSlots.Length; i++) {
            if (i == toolIdx) {
                toolSlots[i].Activate();
            } else {
                toolSlots[i].Deactivate();
            }
        }
    }
}
