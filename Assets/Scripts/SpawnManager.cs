using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    [SerializeField] private PolygonCollider2D spawnZone;
    [SerializeField] private Transform backupSpawnPoint;
    [SerializeField] private int maxAttempts;

    void Awake() {
        if (Instance == null) {Instance = this;}
        else {Destroy(gameObject);}
    }

    public GameObject SpawnObject(GameObject obj) {
        Vector2 spawnPoint = GetSpawnPoint();
        if (spawnPoint != Vector2.zero) {
            GameObject spawnedObject = Instantiate(obj, spawnPoint, Quaternion.identity);
            spawnedObject.GetComponent<NetworkObject>().Spawn();
            return spawnedObject;
        } else {
            GameObject spawnedObject = Instantiate(obj, backupSpawnPoint.position, Quaternion.identity);
            spawnedObject.GetComponent<NetworkObject>().Spawn();
            return spawnedObject;
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

}
