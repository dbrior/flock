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
    public static SheepManager Instance { get; private set; }

    // Serializable Fields
    [SerializeField]
    private Range spawnRangeX;
    [SerializeField]
    private Range spawnRangeY;
    [SerializeField]
    private int spawnCount;
    private int deadSheepCount;
    private int wildSheepCount;
    private int tameSheepCount;
    [SerializeField]
    private GameObject sheepPrefab;

    // Hidden Fields
    private List<GameObject> sheepList;

    void Awake() {
        // Singleton management
        if (Instance == null){Instance = this;} 
        else {Destroy(gameObject);}

        sheepList = new List<GameObject>();
    }

    void Start()
    {
        SpawnSheep();
    }

    public void SpawnSheep() {
        for (int i=0; i<spawnCount; i++) {
            Vector2 spawnPosition = new Vector2(
                Random.Range(spawnRangeX.min, spawnRangeX.max),
                Random.Range(spawnRangeY.min, spawnRangeY.max)
            );
            GameObject sheep = Instantiate(sheepPrefab, spawnPosition, Quaternion.identity);
            sheepList.Add(sheep);
            wildSheepCount += 1;
        }
        UpdateUI();
    }

    public void TameSheep(GameObject sheep) {
        sheep.layer = LayerMask.NameToLayer("TameSheep");
        wildSheepCount -= 1;
        tameSheepCount += 1;
        UpdateUI();
    }

    public void KillSheep(GameObject sheep) {
        wildSheepCount -= 1;
        deadSheepCount += 1;
        DestroySheep(sheep);
        UpdateUI();
    }

    private void DestroySheep(GameObject sheep) {
        sheepList.Remove(sheep);
        Destroy(sheep);
    }

    public void DestroyAllSheep() {
        for (int i=0; i<sheepList.Count; i++) {
            Destroy(sheepList[i]);
        }
        sheepList.Clear();
    }

    private void UpdateUI() {
        UIManager.Instance.UpdateSheepCountsUI(wildSheepCount, tameSheepCount, deadSheepCount);
    }
}
