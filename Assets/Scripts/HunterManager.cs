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

    public void SpawnHunter(Player player) {
        GameObject hunterObj = Instantiate(hunterPrefab, player.transform.position, player.transform.rotation);
        Hunter hunter = hunterObj.GetComponent<Hunter>();
        hunter.fireHz = fireHz;
        hunter.toolBelt.pelletDamage = damage;

        Transform hunterSlot = player.hunterSlots[Random.Range(0, player.hunterSlots.Count)];
        hunter.player = hunterSlot;

        hunters.Add(hunter);
    }
}
