using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IntRange {
    public int min;
    public int max;

    void Init(int min, int max) {
        this.min = min;
        this.max = max;
    }
}

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [SerializeField] private IntRange wolfSpawnRange;
    [SerializeField] private IntRange sheepSpawnRange;
    [SerializeField] private int dayLengthSeconds;
    private int currentTimeSeconds;
    void Awake() {
        if (Instance == null) {Instance = this;} 
        else {Destroy(gameObject);}
    }

    void Start() {
        StartWave();
    }

    public void StartWave() {
        SheepManager.Instance.SetSpawnCount(Random.Range(sheepSpawnRange.min, sheepSpawnRange.max));
        SheepManager.Instance.SpawnSheep();

        WolfManager.Instance.SetSpawnCount(Random.Range(wolfSpawnRange.min, wolfSpawnRange.max));
        WolfManager.Instance.SpawnWolves();

        CropManager.Instance.SpawnRandomCrops();
        currentTimeSeconds = 0;
        StartCoroutine(DayCycle());
    }

    public void EndWave() {
        SheepManager.Instance.Reset();
        WolfManager.Instance.Reset();
        CropManager.Instance.AdvanceCrops();
        StartWave();
    }

    private IEnumerator DayCycle() {
        while (currentTimeSeconds < dayLengthSeconds) {
            yield return new WaitForSeconds(1f);
            currentTimeSeconds += 1;
            UIManager.Instance.UpdateTime(currentTimeSeconds, dayLengthSeconds);
        }
        EndWave();
    }
}
