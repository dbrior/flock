using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class MapNode : MonoBehaviour
{
    [SerializeField] private int requiredSheepCount;
    [SerializeField] private float scaleFactor;
    [SerializeField] private UnityEvent onCapture;
    [SerializeField] private GameObject sheepUI;
    [SerializeField] private TextMeshProUGUI sheepCountUI;
    private int currSheepCount;
    private List<GameObject> sheepList = new List<GameObject>();

    void Start() {
        // ClearPips();
        SetSheepCountUI();
    }

    private void SetSheepCountUI() {
        sheepCountUI.text = currSheepCount + "    /    " + requiredSheepCount;
    }

    public void DepositSheep(GameObject sourceObj) {
        Shepard shepard = sourceObj.GetComponent<Shepard>();
        GameObject newSheep = shepard.DepositSheep();

        if (newSheep == null) return;

        Destroy(newSheep);

        currSheepCount += 1;

        // FillPip();

        if (currSheepCount >= requiredSheepCount) {
            Filled();
        }

        SetSheepCountUI();
    }

    private void Filled() {
        onCapture?.Invoke();
        currSheepCount = 0;
        requiredSheepCount = Mathf.RoundToInt(requiredSheepCount * scaleFactor);
        // ClearPips();
    }

    // private void FillPip() {
    //     for (int i=0; i<sheepUI.transform.childCount; i++) {
    //         Image image = sheepUI.transform.GetChild(i).gameObject.GetComponent<Image>();
    //         if (image.color != fillColor) {
    //             image.color = fillColor;
    //             return;
    //         }
    //     }
    // }

    // private void ClearPips() {
    //     for (int i=0; i<sheepUI.transform.childCount; i++) {
    //         Image image = sheepUI.transform.GetChild(i).gameObject.GetComponent<Image>();
    //         image.color = emptyColor;
    //     }
    // }
}
