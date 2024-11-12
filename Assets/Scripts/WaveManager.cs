using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class IntRange {
    public int min;
    public int max;

    void Init(int min, int max) {
        this.min = min;
        this.max = max;
    }
}

[System.Serializable]
public class FloatRange {
    public float min;
    public float max;

    void Init(float min, float max) {
        this.min = min;
        this.max = max;
    }
}

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [SerializeField] private IntRange wolfSpawnRange;
    [SerializeField] private FloatRange wolfSpawnFrequency;
    [SerializeField] private IntRange sheepSpawnRange;
    [SerializeField] private FloatRange sheepSpawnFrequency;
    [SerializeField] private float dayLengthSeconds;
    [SerializeField] private List<Color> lightingColors;
    [SerializeField] private Light2D sceneLight;
    [SerializeField] private float lightsTurnOffTime;
    [SerializeField] private float lightsTurnOnTime;

    private bool lightsOn;
    private List<Light2D> toggleableLights;

    private float currentTimeSeconds;
    [SerializeField] private float wakeTime;
    private Coroutine dayTimer;

    private int currDay;

    void Awake() {
        if (Instance == null) { Instance = this; } 
        else { Destroy(gameObject); }

        lightsOn = false;
        toggleableLights = new List<Light2D>();
    }

    void Start() {
        currDay = 1;
        StartWave();

        currentTimeSeconds = wakeTime * (dayLengthSeconds / 24f);
    }

    public void AddToggleableLight(Light2D light) {
        toggleableLights.Add(light);
    }

    private void TurnOffLights() {
        foreach (Light2D light in toggleableLights) {
            light.enabled = false;
        }
        lightsOn = false;
    }

    private void TurnOnLights() {
        foreach (Light2D light in toggleableLights) {
            light.enabled = true;
        }
        lightsOn = true;
    }

    public float getCurrentTime() {
        return (currentTimeSeconds / dayLengthSeconds) * 24f; // 17.5 is 5:30PM
    }

    public void Sleep() {
        float dayPct = 0.66f;
        dayPct = 0.01f;
        if (currentTimeSeconds / dayLengthSeconds > dayPct) {
            EndWave();
        }

        currentTimeSeconds = wakeTime;
    }

    public void StartWave() {
        SheepManager.Instance.SetSpawnCount(Random.Range(sheepSpawnRange.min, sheepSpawnRange.max));
        SheepManager.Instance.SpawnSheep();

        WolfManager.Instance.SetSpawnCount(wolfSpawnRange);
        WolfManager.Instance.SetSpawnInterval(wolfSpawnFrequency);
        // WolfManager.Instance.SpawnWolves();

        // CropManager.Instance.SpawnRandomCrops();
        currentTimeSeconds = 0;
        dayTimer = StartCoroutine(DayCycle());
    }

    public void EndWave() {
        if (dayTimer != null) {
            StopCoroutine(dayTimer);
            dayTimer = null;
        }
        currDay += 1;
        // SheepManager.Instance.Reset();
        // WolfManager.Instance.Reset();
        CropManager.Instance.AdvanceCrops();
        // StartWave();
    }

    private IEnumerator DayCycle() {
        while (currentTimeSeconds < dayLengthSeconds) {
            yield return new WaitForSeconds(0.1f);
            currentTimeSeconds += 0.1f;

            // Interpolate color based on time
            float timeNormalized = currentTimeSeconds / dayLengthSeconds;
            Color currentColor = GetInterpolatedColor(timeNormalized);
            sceneLight.color = currentColor;

            UIManager.Instance.UpdateTime(currentTimeSeconds, dayLengthSeconds);

            // Set toggleable lights
            float currentTime = getCurrentTime();
            if (!lightsOn && currentTime < lightsTurnOffTime || currentTime >= lightsTurnOnTime) {
                TurnOnLights();
            } else if (lightsOn && currentTime >= lightsTurnOffTime && currentTime < lightsTurnOnTime) {
                TurnOffLights();
            }
        }
        EndWave();
    }

    private Color GetInterpolatedColor(float normalizedTime) {
        if (lightingColors.Count < 2) return Color.white;

        int colorIndex = Mathf.FloorToInt(normalizedTime * (lightingColors.Count - 1));
        float lerpFactor = (normalizedTime * (lightingColors.Count - 1)) - colorIndex;

        if (colorIndex >= lightingColors.Count - 1) return lightingColors[lightingColors.Count - 1];

        return Color.Lerp(lightingColors[colorIndex], lightingColors[colorIndex + 1], lerpFactor);
    }
}
