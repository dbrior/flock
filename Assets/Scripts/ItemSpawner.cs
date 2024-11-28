using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemSpawn {
    public GameObject itemPrefab;
    public float probability;
    public int maxAmount;

    void Init(GameObject itemPrefab, float probability, int maxAmount) {
        this.itemPrefab = itemPrefab;
        this.probability = probability;
        this.maxAmount = maxAmount;
    }
}

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private List<ItemSpawn> itemSpawns;
    public void SpawnItems() {
        foreach (ItemSpawn itemSpawn in itemSpawns) {
            for (int i=0; i<itemSpawn.maxAmount; i++) {
                float spawnRoll = UnityEngine.Random.Range(0, 1f);
                if (spawnRoll > itemSpawn.probability) continue;
                // Vector2 spawnLocation = (Vector2) transform.position + (UnityEngine.Random.insideUnitCircle.normalized * 0.1f);
                GameObject spawnedObj = Instantiate(itemSpawn.itemPrefab, transform.position, Quaternion.identity);
                Rigidbody2D rb = spawnedObj.GetComponent<Rigidbody2D>();
                Vector2 randomForce = (UnityEngine.Random.insideUnitCircle.normalized * 50f);
                rb.AddForce(randomForce);
                Destroy(spawnedObj, 30f);
            }
        }
    }
}
