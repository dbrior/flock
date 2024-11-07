using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolSlot : MonoBehaviour
{
    [SerializeField] private GameObject activeIndicator;

    public void Activate() {
        activeIndicator.SetActive(true);
    }

    public void Deactivate() {
        activeIndicator.SetActive(false);
    }
}
