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
            GameObject sheep = SpawnManager.Instance.SpawnObject(sheepPrefab);
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
