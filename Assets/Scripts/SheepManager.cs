using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SheepManager : MonoBehaviour
{
    public static SheepManager Instance { get; private set; }

    [SerializeField] private int spawnCount;
    [SerializeField] private Transform releasePoint;
    [SerializeField] private float exitSpeed;
    private bool isReleasing;
    private int deadSheepCount;
    private int wildSheepCount;
    private int tameSheepCount;
    [SerializeField]
    private GameObject sheepPrefab;
    private List<Sheep> tameSheepList;
    private List<Sheep> wildSheepList;
    private List<Sheep> releaseSheepList;

    void Awake() {
        // Singleton management
        if (Instance == null){Instance = this;} 
        else {Destroy(gameObject);}

        wildSheepList = new List<Sheep>();
        tameSheepList = new List<Sheep>();
        releaseSheepList = new List<Sheep>();
        wildSheepCount = 0;
        tameSheepCount = 0;
        deadSheepCount = 0;
    }

    void FixedUpdate() {
        foreach (Sheep sheep in releaseSheepList) {
            Rigidbody2D rb = sheep.GetComponent<Rigidbody2D>();
            Vector2 heading = (Vector2) (releasePoint.position - sheep.transform.position).normalized;
            Vector2 targetVel = heading * exitSpeed;
            Vector2 velDelta = targetVel - rb.velocity;
            Vector2 requiredAccel = velDelta / Time.fixedDeltaTime;
            rb.AddForce(requiredAccel * rb.mass);
        }
    }

    public void Reset() {
        DestroyWildSheep();
        wildSheepList = new List<Sheep>();
        wildSheepCount = 0;
        deadSheepCount = 0;

        // // Refresh sheared sheep
        // foreach (Sheep sheep in tameSheepList) {
        //     sheep.Regrow();
        // }

        UpdateUI();
    }

    public void SetSpawnCount(int count) {
        spawnCount = count;
    }

    public void SpawnSheep() {
        for (int i=0; i<spawnCount; i++) {
            Sheep sheep = SpawnManager.Instance.SpawnObject(sheepPrefab).GetComponent<Sheep>();
            wildSheepList.Add(sheep);
            wildSheepCount += 1;
        }
        OnSheepCountChange();
    }

    public void TameSheep(GameObject sheepObj) {
        sheepObj.layer = LayerMask.NameToLayer("TameSheep");

        Sheep sheep = sheepObj.GetComponent<Sheep>();
        wildSheepCount -= 1;
        tameSheepCount += 1;
        sheep.Capture();
        tameSheepList.Add(sheep);
        wildSheepList.Remove(sheep);
        OnSheepCountChange();
    }

    public void ReleaseAllSheep() {
        releaseSheepList = new List<Sheep>(tameSheepList);
        foreach (Sheep sheep in tameSheepList) {
            BoxCollider2D collider = sheep.GetComponent<BoxCollider2D>();
            collider.isTrigger = true;
        }
        tameSheepList.Clear();
    }

    public void ReleaseSheep(GameObject sheepObj) {
        sheepObj.layer = LayerMask.NameToLayer("WildSheep");

        Sheep sheep = sheepObj.GetComponent<Sheep>();
        sheep.Release();
        BoxCollider2D collider = sheepObj.GetComponent<BoxCollider2D>();
        collider.isTrigger = false;
        wildSheepCount += 1;
        tameSheepCount -= 1;
        releaseSheepList.Remove(sheep);
        wildSheepList.Add(sheep);
        OnSheepCountChange();
    }

    public void KillSheep(GameObject sheepObj) {
        Sheep sheep = sheepObj.GetComponent<Sheep>();
        if (sheep.isDying) {
            return;
        }
        wildSheepCount -= 1;
        deadSheepCount += 1;
        sheep.Kill();
        OnSheepCountChange();
    }

    public void DestroyWildSheep() {
        for (int i=0; i<wildSheepList.Count; i++) {
            Sheep sheep = wildSheepList[i];
            if (sheep != null) {
                Destroy(sheep.gameObject);
            }
        }
        wildSheepList.Clear();
    }

    private void UpdateUI() {
        UIManager.Instance.UpdateSheepCountsUI(wildSheepCount, tameSheepCount, deadSheepCount);
    }

    private void OnSheepCountChange() {
        UpdateUI();
        // if (wildSheepCount == 0) {
        //     WaveManager.Instance.EndWave();
        // }
    }
}
