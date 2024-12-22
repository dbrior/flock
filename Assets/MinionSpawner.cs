using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionSpawner : MonoBehaviour
{
    [SerializeField] private GameObject minionPrefab;
    
    public void SpawnMinions(int count) {
        for (int i=0; i<count; i++) {
            Vector3 offset = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 0);
            GameObject newMinion = Instantiate(minionPrefab, transform.position + offset, transform.rotation);
            Destroy(newMinion, 60f);
        }
    }
}
