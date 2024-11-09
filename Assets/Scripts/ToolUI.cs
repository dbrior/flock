using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolUI : MonoBehaviour
{
    public static ToolUI Instance { get; private set;}
    private ToolSlot[] toolSlots;
    private int currToolIdx;

    void Awake() {
        if (Instance == null){Instance = this;} 
        else {Destroy(gameObject);}
    }

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
