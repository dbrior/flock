using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterManager : MonoBehaviour
{
    public static HunterManager Instance { get; private set; }

    [SerializeField] private List<Hunter> hunters;
    [SerializeField] private GameObject hunterPrefab;
    [SerializeField] private float fireHz;
    [SerializeField] private float damage;
    [SerializeField] private Transform spawnLocation;
    [SerializeField] private Transform watchpoint;

    void Awake() {
        if (Instance == null) {Instance = this;} 
        else {Destroy(gameObject);}
    }

    public void IncreaseFireRate(float hz) {
        fireHz += hz;
        DeployFireRate();
    }

    public void IncreaseDamage(float pct) {
        damage = damage * (1f + pct);
        DeployDamage();
    }

    private void DeployDamage() {
        foreach (Hunter hunter in hunters) {
            hunter.toolBelt.pelletDamage = damage;
        }
    }

    private void DeployFireRate() {
        foreach (Hunter hunter in hunters) {
            hunter.fireHz = fireHz;
        }
    }

    public void SendToWatchpoint() {
        foreach (Hunter hunter in hunters) {
            hunter.goToWatchpoint = true;
        }
    }

    public void SendToPlayer() {
        foreach (Hunter hunter in hunters) {
            hunter.goToWatchpoint = false;
        }
    }

    public void SpawnHunter(Player player) {
        GameObject hunterObj = Instantiate(hunterPrefab, spawnLocation.position, spawnLocation.rotation);
        Hunter hunter = hunterObj.GetComponent<Hunter>();
        hunter.fireHz = fireHz;
        hunter.toolBelt.pelletDamage = damage;

        Transform hunterSlot = player.hunterSlots[Random.Range(0, player.hunterSlots.Count)];
        hunter.player = hunterSlot;
        hunter.watchpoint = watchpoint;

        hunters.Add(hunter);
    }
}
