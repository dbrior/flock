using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public Player currentPlayer;

    void Awake() {
        if (Instance == null) {Instance = this;}
        else {Destroy(gameObject);}
    }

    public void IncreaseMagicCount(int amount) {
        currentPlayer.spinner.instanceCount += amount;
        currentPlayer.spinner.SpawnRadialInstances();
    }

    public void IncreaseMagicSpeed(float pctIncrease) {
        currentPlayer.spinner.SetSpeed(currentPlayer.spinner.rotateSpeed * (1f + pctIncrease));
    }

    public void IncreaseMagicDamage(float pctIncrease) {
        currentPlayer.spinner.IncreaseDamage(pctIncrease);
    }
}
