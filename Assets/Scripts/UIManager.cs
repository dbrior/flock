using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    // UI Elements
    [SerializeField] private TextMeshProUGUI wildSheepCount;
    [SerializeField] private TextMeshProUGUI tameSheepCount;
    [SerializeField] private TextMeshProUGUI deadSheepCount;

    void Awake() {
        if (Instance == null){Instance = this;} 
        else {Destroy(gameObject);}
    }

    public void UpdateSheepCountsUI(int wildCount, int tameCount, int deadCount) {
        wildSheepCount.text = wildCount.ToString() + " wild";
        tameSheepCount.text = tameCount.ToString() + " tame";
        deadSheepCount.text = deadCount.ToString() + " dead";

    }
}
