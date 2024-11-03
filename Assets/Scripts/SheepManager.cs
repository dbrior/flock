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
        wildSheepCount = 0;
        tameSheepCount = 0;
        deadSheepCount = 0;
    }

    public void Reset() {
        DestroyAllSheep();
        sheepList = new List<GameObject>();
        wildSheepCount = 0;
        tameSheepCount = 0;
        deadSheepCount = 0;
        UpdateUI();
    }

    public void SetSpawnCount(int count) {
        spawnCount = count;
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
        OnSheepCountChange();
    }

    public void TameSheep(GameObject sheep) {
        sheep.layer = LayerMask.NameToLayer("TameSheep");
        wildSheepCount -= 1;
        tameSheepCount += 1;
        OnSheepCountChange();
    }

    public void KillSheep(GameObject sheep) {
        wildSheepCount -= 1;
        deadSheepCount += 1;
        DestroySheep(sheep);
        OnSheepCountChange();
    }

    private void DestroySheep(GameObject sheep) {
        sheepList.Remove(sheep);
        Destroy(sheep);
    }

    public void DestroyAllSheep() {
        for (int i=0; i<sheepList.Count; i++) {
            GameObject sheep = sheepList[i];
            sheepList.RemoveAt(i);
            Destroy(sheep);
        }
        sheepList.Clear();
    }

    private void UpdateUI() {
        UIManager.Instance.UpdateSheepCountsUI(wildSheepCount, tameSheepCount, deadSheepCount);
    }

    private void OnSheepCountChange() {
        UpdateUI();
        if (wildSheepCount == 0) {
            WaveManager.Instance.EndWave();
        }
    }
}
