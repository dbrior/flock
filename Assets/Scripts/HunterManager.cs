using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterManager : MonoBehaviour
{
    public static HunterManager Instance { get; private set; }

    [SerializeField] private List<RangedAttacker> hunters;
    [SerializeField] private GameObject hunterPrefab;
    [SerializeField] private float fireHz;
    [SerializeField] private float damage;
    [SerializeField] private Transform spawnLocation;
    [SerializeField] private Transform watchpoint;

    void Awake() {
        if (Instance == null) {Instance = this;} 
        else {Destroy(gameObject);}
    }

    // public void IncreaseFireRate(float hz) {
    //     fireHz += hz;
    //     DeployFireRate();
    // }

    // public void IncreaseDamage(float pct) {
    //     damage = damage * (1f + pct);
    //     DeployDamage();
    // }

    // private void DeployDamage() {
    //     foreach (RangedAttacker hunter in hunters) {
    //         hunter.SetDamage(damage);
    //     }
    // }

    // private void DeployFireRate() {
    //     foreach (RangedAttacker hunter in hunters) {
    //         hunter.SetCooldownSec(1f/fireHz);
    //     }
    // }

    // public void SendToWatchpoint() {
    //     {}
    //     // foreach (Hunter hunter in hunters) {
    //     //     hunter.goToWatchpoint = true;
    //     // }
    // }

    // public void SendToPlayer() {
    //     {}
    //     // foreach (Hunter hunter in hunters) {
    //     //     hunter.goToWatchpoint = false;
    //     // }
    // }

    // public void SpawnHunter(Player player) {
    //     GameObject hunterObj = Instantiate(hunterPrefab, spawnLocation.position, spawnLocation.rotation);
    //     hunterObj.GetComponent<CharacterMover>().SetWanderAnchor(player.transform);

    //     RangedAttacker rangedAttacker = hunterObj.GetComponent<RangedAttacker>();
    //     rangedAttacker.SetCooldownSec(1f/fireHz);
    //     rangedAttacker.SetDamage(damage);

    //     hunters.Add(rangedAttacker);
    // }
}
