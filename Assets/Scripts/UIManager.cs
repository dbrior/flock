using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    // UI Elements
    [SerializeField] private TextMeshProUGUI tameSheepCounter;
    [SerializeField] private TextMeshProUGUI woolCounter;

    void Awake() {
        if (Instance == null) {Instance = this;} 
        else {Destroy(gameObject);}
    }

    public void UpdateSheepCountsUI(int wildCount, int tameCount, int deadCount) {
        tameSheepCounter.text = tameCount.ToString();
        // wildSheepCount.text = wildCount.ToString();
        // deadSheepCount.text = deadCount.ToString();
    }

    public void UpdateWoolCount(int woolCount) {
        woolCounter.text = woolCount.ToString();
    }
}
