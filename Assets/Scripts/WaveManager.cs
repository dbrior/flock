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

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [SerializeField] private IntRange wolfSpawnRange;
    [SerializeField] private IntRange sheepSpawnRange;
    [SerializeField] private float dayLengthSeconds;
    [SerializeField] private List<Color> lightingColors;
    [SerializeField] private Light2D sceneLight;

    private float currentTimeSeconds;
    private Coroutine dayTimer;

    void Awake() {
        if (Instance == null) { Instance = this; } 
        else { Destroy(gameObject); }
    }

    void Start() {
        StartWave();
        currentTimeSeconds = (dayLengthSeconds / 24f) * 7;
    }

    public void Sleep() {
        float dayPct = 0.66f;
        dayPct = 0.01f;
        if (currentTimeSeconds / dayLengthSeconds > dayPct) {
            EndWave();
        }
    }

    public void StartWave() {
        SheepManager.Instance.SetSpawnCount(Random.Range(sheepSpawnRange.min, sheepSpawnRange.max));
        SheepManager.Instance.SpawnSheep();

        WolfManager.Instance.SetSpawnCount(Random.Range(wolfSpawnRange.min, wolfSpawnRange.max));
        WolfManager.Instance.SpawnWolves();

        CropManager.Instance.SpawnRandomCrops();
        currentTimeSeconds = 0;
        dayTimer = StartCoroutine(DayCycle());
    }

    public void EndWave() {
        if (dayTimer != null) {
            StopCoroutine(dayTimer);
            dayTimer = null;
        }
        SheepManager.Instance.Reset();
        WolfManager.Instance.Reset();
        CropManager.Instance.AdvanceCrops();
        StartWave();
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
