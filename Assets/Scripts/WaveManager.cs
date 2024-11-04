using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    private List<(int, int)> waves = new List<(int, int)>{
        (3, 0),
        (5, 1),
        (10, 3),
        (10, 5),
        (20, 8)
    };
    [SerializeField] private int currentWave;
    void Awake() {
        if (Instance == null) {Instance = this;} 
        else {Destroy(gameObject);}
    }

    void Start() {
        StartWave();
    }

    public void StartWave() {
        (int sheepCount, int wolfCount) = waves[currentWave];

        SheepManager.Instance.SetSpawnCount(sheepCount);
        SheepManager.Instance.SpawnSheep();

        WolfManager.Instance.SetSpawnCount(wolfCount);
        WolfManager.Instance.SpawnWolves();
    }

    public void EndWave() {
        SheepManager.Instance.Reset();
        WolfManager.Instance.Reset();
        currentWave += 1;
        StartWave();
    }
}
