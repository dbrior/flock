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
    [SerializeField] private TextMeshProUGUI toothCounter;
    [SerializeField] private TextMeshProUGUI worldTime;
    [SerializeField] private TextMeshProUGUI day;

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

    public void UpdateToothCount(int toothCount) {
        toothCounter.text = toothCount.ToString();
    }

    public void UpdateTime(float currentTimeSeconds, float dayLengthSeconds) {
        float hourValue = dayLengthSeconds / 24f;
        float minuteValue = hourValue / 60f;

        int currentHour = (int) Mathf.Floor(currentTimeSeconds / hourValue);
        int currentMinute = (int) Mathf.Floor((currentTimeSeconds - (currentHour * hourValue)) / minuteValue);

        worldTime.text = currentHour.ToString("00") + ":" + currentMinute.ToString("00");
    }

    public void UpdateDay(int dayValue) {
        day.text = "DAY    " + dayValue.ToString("");
    }
}
