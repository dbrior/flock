using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfManager : MonoBehaviour
{
    public static WolfManager Instance { get; private set; }

    [SerializeField] private GameObject wolfPrefab;
    [SerializeField] private int spawnCount;
    private List<GameObject> wolves;

    void Awake() {
        if (Instance == null) {Instance = this;}
        else {Destroy(gameObject);}

        wolves = new List<GameObject>();
    }

    public void Reset() {
        DestroyAllWovles();
    }

    public void SetSpawnCount(int count) {
        spawnCount = count;
    }

    public void SpawnWolves() {
        for (int i = 0; i < spawnCount; i++) {
            GameObject wolf = SpawnManager.Instance.SpawnObject(wolfPrefab);
            wolves.Add(wolf);
        }
    }

    public void DestroyAllWovles() {
        for (int i = 0; i<wolves.Count; i++) {
            Destroy(wolves[i]);
        }
        wolves.Clear();
    }
}
