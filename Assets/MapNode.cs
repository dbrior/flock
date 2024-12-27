using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MapNode : MonoBehaviour
{
    [SerializeField] private int requiredSheepCount;
    [SerializeField] private UnityEvent onCapture;
    private int currSheepCount;
    private List<GameObject> sheepList = new List<GameObject>();

    public void DepositSheep(GameObject sourceObj) {
        Shepard shepard = sourceObj.GetComponent<Shepard>();
        GameObject newSheep = shepard.DepositSheep();

        if (newSheep == null) return;

        Destroy(newSheep);

        currSheepCount += 1;

        if (currSheepCount >= requiredSheepCount) {
            onCapture?.Invoke();
            currSheepCount = 0;
        }
    }
}
