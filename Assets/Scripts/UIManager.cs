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

    public static string FormatNumber(float number) {
        if (number < 0.01f)
            return "0";
            
        if (number < 1f)
            return number.ToString("0.00").TrimEnd('0').TrimEnd('.');
            
        if (number < 100f)
            return number.ToString("0.0").TrimEnd('0').TrimEnd('.');
            
        if (number < 1000f)
            return number.ToString("0");
            
        if (number < 1000000f)
        {
            float thousands = number / 1000f;
            return thousands.ToString("0.00").TrimEnd('0').TrimEnd('.') + "K";
        }
        
        if (number < 1000000000f)
        {
            float millions = number / 1000000f;
            return millions.ToString("0.00").TrimEnd('0').TrimEnd('.') + "M";
        }
        
        float billions = number / 1000000000f;
        return billions.ToString("0.00").TrimEnd('0').TrimEnd('.') + "B";
    }

}
