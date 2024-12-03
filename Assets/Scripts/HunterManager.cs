using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterManager : MonoBehaviour
{
    public static HunterManager Instance { get; private set; }

    [SerializeField] private List<Hunter> hunters;
    [SerializeField] private GameObject hunterPrefab;
    [SerializeField] private float fireHz;

    void Awake() {
        if (Instance == null) {Instance = this;} 
        else {Destroy(gameObject);}
    }

    public void IncreaseFireRate(float hz) {
        fireHz += hz;
        DeployFireRate();
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

        Transform hunterSlot = player.hunterSlots[Random.Range(0, player.hunterSlots.Count)];
        hunter.player = hunterSlot;

        hunters.Add(hunter);
    }
}
