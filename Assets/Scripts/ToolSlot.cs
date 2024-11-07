using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolSlot : MonoBehaviour
{
    [SerializeField] private GameObject activeIndicator;

    public void Activate() {
        // activeIndicator.SetActive(true);
        activeIndicator.SetActive(false);
        transform.localScale = Vector3.one;
    }

    public void Deactivate() {
        activeIndicator.SetActive(false);
        transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
    }
}
