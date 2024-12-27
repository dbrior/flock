using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shepard : MonoBehaviour
{
    private int capturedSheepCount;
    private List<GameObject> sheepList = new List<GameObject>();

    public void AddSheep(GameObject newSheep) {
        capturedSheepCount += 1;
        sheepList.Add(newSheep);
    }

    public GameObject DepositSheep() {
        for (int i=capturedSheepCount-1; i>=0; i--) {
            GameObject sheep = sheepList[i];
            sheepList.RemoveAt(i);
            capturedSheepCount -= 1;
            if (sheep != null) return sheep;
        }
        return null;
    }
}
