using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Range
{
    public float min;
    public float max;

    public Range(float min, float max)
    {
        this.min = min;
        this.max = max;
    }
}

public class SheepManager : MonoBehaviour
{
    // Serializable Fields
    [SerializeField]
    private Range spawnRangeX;
    [SerializeField]
    private Range spawnRangeY;
    [SerializeField]
    private int spawnCount;
    [SerializeField]
    private GameObject sheepPrefab;
    // Private Fields
    private List<GameObject> sheepList;

    void Awake() {
        sheepList= new List<GameObject>();
    }

    void Start()
    {
        for (int i=0; i<spawnCount; i++) {
            Vector2 spawnPosition = new Vector2(
                Random.Range(spawnRangeX.min, spawnRangeX.max),
                Random.Range(spawnRangeY.min, spawnRangeY.max)
            );
            GameObject sheep = Instantiate(sheepPrefab, spawnPosition, Quaternion.identity);
            sheepList.Add(sheep);
        }
    }

    void DestroySheep() {
        for (int i=0; i<sheepList.Count; i++) {
            Destroy(sheepList[i]);
        }
        sheepList.Clear();
    }
}
