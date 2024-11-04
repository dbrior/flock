using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepManager : MonoBehaviour
{
    public static SheepManager Instance { get; private set; }

    [SerializeField]
    private int spawnCount;
    private int deadSheepCount;
    private int wildSheepCount;
    private int tameSheepCount;
    [SerializeField]
    private GameObject sheepPrefab;
    private List<Sheep> sheepList;

    void Awake() {
        // Singleton management
        if (Instance == null){Instance = this;} 
        else {Destroy(gameObject);}

        sheepList = new List<Sheep>();
        wildSheepCount = 0;
        tameSheepCount = 0;
        deadSheepCount = 0;
    }

    public void Reset() {
        DestroyAllSheep();
        sheepList = new List<Sheep>();
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
            Sheep sheep = SpawnManager.Instance.SpawnObject(sheepPrefab).GetComponent<Sheep>();
            sheepList.Add(sheep);
            wildSheepCount += 1;
        }
        OnSheepCountChange();
    }

    public void TameSheep(GameObject sheep) {
        sheep.layer = LayerMask.NameToLayer("TameSheep");
        wildSheepCount -= 1;
        tameSheepCount += 1;
        sheep.GetComponent<Sheep>().Capture();
        OnSheepCountChange();
    }

    public void KillSheep(GameObject sheep) {
        wildSheepCount -= 1;
        deadSheepCount += 1;
        sheep.GetComponent<Sheep>().Kill();
        OnSheepCountChange();
    }

    public void DestroyAllSheep() {
        for (int i=0; i<sheepList.Count; i++) {
            Sheep sheep = sheepList[i];
            if (sheep != null) {
                Destroy(sheep.gameObject);
            }
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
