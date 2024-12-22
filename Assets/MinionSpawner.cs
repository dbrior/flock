using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionSpawner : MonoBehaviour
{
    [SerializeField] private GameObject minionPrefab;
    [SerializeField] private int count;
    
    public void SpawnMinions(Transform spawnTransform, bool hasSpawnOffset = true) {
        for (int i=0; i<count; i++) {
            Vector3 offset = hasSpawnOffset ? new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 0) : Vector3.zero;
            GameObject newMinion = Instantiate(minionPrefab, spawnTransform.position + offset, transform.rotation);
            Destroy(newMinion, 60f);
        }
    }
}
