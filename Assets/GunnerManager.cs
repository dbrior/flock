using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterManager : MonoBehaviour
{
    public static HunterManager Instance { get; private set; }

    [SerializeField] private List<Hunter> hunters;

    void Awake() {
        if (Instance == null) {Instance = this;} 
        else {Destroy(gameObject);}
    }

    public void IncreaseFireRate(float hz) {
        foreach (Hunter hunter in hunters) {
            hunter.fireHz += hz;
        }
    }
}
