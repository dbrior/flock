using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepManager : MonoBehaviour
{
    public static SheepManager Instance { get; private set; }

    private IntRange spawnCount;
    private FloatRange spawnInterval;
    [SerializeField] private Transform releasePoint;
    [SerializeField] private float exitSpeed;
    [SerializeField] private Item sheepFood;
    [SerializeField] private Transform tameWanderPoint;
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

    void Start() {
        StartCoroutine(SheepSpawner());
    }

    void FixedUpdate() {
        foreach (Sheep sheep in releaseSheepList) {
            if (sheep == null) continue;
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
        UpdateUI();
    }

    public void CleanReferences() {
        for (int i=wildSheepList.Count-1; i>=0; --i) {
            if (wildSheepList[i] == null) wildSheepList.RemoveAt(i);
        }

        for (int i=tameSheepList.Count-1; i>=0; --i) {
            if (tameSheepList[i] == null) tameSheepList.RemoveAt(i);
        }

        for (int i=releaseSheepList.Count-1; i>=0; --i) {
            if (releaseSheepList[i] == null) releaseSheepList.RemoveAt(i);
        }
    }

    public void AdvanceSheep() {
        foreach(Sheep sheep in tameSheepList) {
            if (sheep == null) continue;
            if (PlayerInventory.Instance.GetItemCount(sheepFood) > 0) {
                PlayerInventory.Instance.RemoveItem(sheepFood, 1);
                sheep.Feed();
            } else {
                sheep.Hit(20f);
            }
            // sheep.AdvanceState();
        }
    }

    // public void FeedSheep() {
    //     foreach(Sheep sheep in tameSheepList) {
    //         if (sheep == null) continue;
    //         sheep.AdvanceState();
    //     }
    // }

    public void SetSpawnCount(IntRange newRange) {
        spawnCount = newRange;
    }

    public void SetSpawnInterval(FloatRange newInterval) {
        spawnInterval = newInterval;
    }
    public void SpawnSheep() {
        for (int i=0; i<Random.Range(spawnCount.min, spawnCount.max); i++) {
            Sheep sheep = SpawnManager.Instance.SpawnObject(sheepPrefab).GetComponent<Sheep>();
            wildSheepList.Add(sheep);
            wildSheepCount += 1;
        }
        OnSheepCountChange();
    }

    public void TameSheep(GameObject sheepObj) {
        sheepObj.layer = LayerMask.NameToLayer("TameSheep");

        sheepObj.GetComponent<CharacterMover>().SetWanderAnchor(tameWanderPoint);
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
            if (sheep == null) continue;
            BoxCollider2D collider = sheep.GetComponent<BoxCollider2D>();
            collider.isTrigger = true;
        }
        tameSheepList.Clear();
    }

    public List<Sheep> GetTameSheep() {
        return tameSheepList;
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

    private IEnumerator SheepSpawner() {
        while (true) {
            SpawnSheep();
            float waitTime = 0f;
            if (!(WaveManager.Instance.getCurrentTime() < 5 || WaveManager.Instance.getCurrentTime() >= 21)) {
                waitTime = Random.Range(spawnInterval.min, spawnInterval.max);
            } else {
                waitTime = Random.Range(spawnInterval.min/2f, spawnInterval.max/2f);
            }
            yield return new WaitForSeconds(waitTime);
        }
    }
}
