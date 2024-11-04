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
        if (Instance == null) {Instance = this;} 
        else {Destroy(gameObject);}
    }

    public void UpdateSheepCountsUI(int wildCount, int tameCount, int deadCount) {
        wildSheepCount.text = wildCount.ToString() + "    WILD";
        tameSheepCount.text = tameCount.ToString() + "    TAME";
        deadSheepCount.text = deadCount.ToString() + "    DEAD";
    }
}
