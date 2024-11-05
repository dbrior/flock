using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropper : MonoBehaviour
{
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private IntRange spawnAmounts;
    public void SpawnItems() {
        int itemAmount = UnityEngine.Random.Range(spawnAmounts.min, spawnAmounts.max);
        for (int i=0; i<itemAmount; i++) {
            Vector2 spawnLocation = (Vector2) transform.position + (UnityEngine.Random.insideUnitCircle.normalized * 0.1f);
            Instantiate(itemPrefab, spawnLocation, Quaternion.identity);
        }
    }
}
