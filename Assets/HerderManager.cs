using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HerderManager : MonoBehaviour
{
    public static HerderManager Instance  { get; private set; }

    [SerializeField] private GameObject herderPrefab;
    [SerializeField] private Transform spawnLocation;

    void Awake() {
        if (Instance == null) {Instance = this;} 
        else {Destroy(gameObject);}
    }

    public void SpawnHerder() {
        GameObject hunterObj = Instantiate(herderPrefab, spawnLocation.position, spawnLocation.rotation);
    }
}
