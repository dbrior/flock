using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CreatureSpawn {
    public GameObject creaturePrefab;
    public IntRange spawnAmount;
    public FloatRange spawnInterval;
    public List<FloatRange> spawnTimeRanges;
    public int startSpawnDay;
}

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    [SerializeField] private PolygonCollider2D spawnZone;
    [SerializeField] private Transform backupSpawnPoint;
    [SerializeField] private int maxAttempts;
    [SerializeField] private List<CreatureSpawn> creatureSpawns;

    void Awake() {
        if (Instance == null) {Instance = this;}
        else {Destroy(gameObject);}
    }

    void Start() {
        foreach (CreatureSpawn creatureSpawn in creatureSpawns) {
            StartCoroutine(CreatureSpawner(creatureSpawn));
        }
    }

    public GameObject SpawnObject(GameObject obj) {
        Vector2 spawnPoint = GetSpawnPoint();
        if (spawnPoint != Vector2.zero) {
            return Instantiate(obj, spawnPoint, Quaternion.identity);
        } else {
            return Instantiate(obj, backupSpawnPoint.position, Quaternion.identity);
        }
    }

    Vector2 GetSpawnPoint()
    {
        Bounds bounds = spawnZone.bounds;
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector2 randomPoint = new Vector2(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y)
            );

            if (spawnZone.OverlapPoint(randomPoint))
            {
                return randomPoint;
            }
        }

        return Vector2.zero;
    }

    IEnumerator CreatureSpawner(CreatureSpawn creatureSpawn) {
        while (true) {
            // Only spawn if >= startSpawnDay
            if (WaveManager.Instance.GetCurrentDay() < creatureSpawn.startSpawnDay) {
                yield return new WaitForSeconds(30f);
                continue;
            }


            // Check if in valid spawning time
            float currentTime = WaveManager.Instance.getCurrentTime();
            bool shouldSpawn = false;
            foreach (FloatRange range in creatureSpawn.spawnTimeRanges) {
                if (currentTime >= range.min && currentTime <= range.max) {
                    shouldSpawn = true;
                    break;
                }
            }

            // Spawn creatures
            if (shouldSpawn) {
                int spawnAmount = Random.Range(creatureSpawn.spawnAmount.min, creatureSpawn.spawnAmount.max);
                for (int i=0; i<spawnAmount; i++) {
                    SpawnObject(creatureSpawn.creaturePrefab);
                }
            }

            yield return new WaitForSeconds(Random.Range(creatureSpawn.spawnInterval.min, creatureSpawn.spawnInterval.max));
        }
    }

}
