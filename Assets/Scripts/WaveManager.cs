using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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
public struct FloatRange {
    public float min;
    public float max;

    public FloatRange(float min, float max) {
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
    [SerializeField] private UnityEvent onBossSpawn;
    [SerializeField] private UnityEvent onBossDeath;

    private bool lightsOn;
    private List<Light2D> toggleableLights;

    private float currentTimeSeconds;
    [SerializeField] private float wakeTime;
    private Coroutine dayTimer;

    private int currDay;

    [SerializeField] private int bossSpawnDayInterval;
    [SerializeField] private List<GameObject> bossList;
    [SerializeField] private Transform bossSpawnLocation;
    private bool shouldSpawnBoss = false;

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

    private void SpawnBoss() {
        float multiplier = Mathf.Pow(2, (currDay/bossSpawnDayInterval)-1);

        GameObject prefab = bossList[Random.Range(0, bossList.Count)];
        GameObject bossObj =Instantiate(prefab, bossSpawnLocation.position, bossSpawnLocation.rotation);

        // Increase boss health
        Damagable bossHealth = bossObj.GetComponent<Damagable>();
        bossHealth.SetMaxHealth(bossHealth.GetMaxHealth() * multiplier);
        bossHealth.RestoreHealth();

        // Set onDeath
        bossHealth.onDeath = onBossDeath;

        // Increase boss damage
        // TODO: this is specific to occultist boss
        SpinnerAttacker bossSpinners = bossObj.GetComponent<SpinnerAttacker>();
        bossSpinners.SetDamage(bossSpinners.GetDamage() * multiplier);

        // Set boss title w/ level
        TextSetter bossHealthText = bossObj.GetComponent<TextSetter>();
        bossHealthText.SetText("ARCANE    OCCULTIST    -    LVL     " + (currDay/bossSpawnDayInterval));

        shouldSpawnBoss = false;
        onBossSpawn?.Invoke();
    }

    public int GetCurrentDay() {
        return currDay;
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
        // SheepManager.Instance.SetSpawnCount(Random.Range(sheepSpawnRange.min, sheepSpawnRange.max));
        SheepManager.Instance.SetSpawnCount(sheepSpawnRange);
        SheepManager.Instance.SetSpawnInterval(sheepSpawnFrequency);
        // SheepManager.Instance.SpawnSheep();

        WolfManager.Instance.SetSpawnCount(wolfSpawnRange);
        WolfManager.Instance.SetSpawnInterval(wolfSpawnFrequency);
        WolfManager.Instance.IncreaseDamageByPct(0.1f);
        WolfManager.Instance.IncreaseHealthByPct(0.05f);
        // WolfManager.Instance.SpawnWolves();

        CropManager.Instance.SpawnRandomCrops();
        currentTimeSeconds = 0;
        dayTimer = StartCoroutine(DayCycle());

        UIManager.Instance.UpdateDay(currDay);

        // Check if boss should spawn tonight
        if (currDay % bossSpawnDayInterval == 0) {
            shouldSpawnBoss = true;
        }
    }

    public void EndWave() {
        if (dayTimer != null) {
            StopCoroutine(dayTimer);
            dayTimer = null;
        }
        currDay += 1;
        wolfSpawnFrequency.min *= 0.80f;
        wolfSpawnFrequency.max *= 0.80f;
        // SheepManager.Instance.Reset();
        // WolfManager.Instance.Reset();
        // CropManager.Instance.AdvanceCrops();
        SheepManager.Instance.CleanReferences();
        // SheepManager.Instance.AdvanceSheep();
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

            // Set toggleable lights
            float currentTime = getCurrentTime();
            if (!lightsOn && (currentTime < lightsTurnOffTime || currentTime >= lightsTurnOnTime)) {
                TurnOnLights();
                MusicManager.Instance.FadeToNightMusic();
            } else if (lightsOn && (currentTime >= lightsTurnOffTime && currentTime < lightsTurnOnTime)) {
                TurnOffLights();
                MusicManager.Instance.FadeToDayMusic();
            }

            // Spawn boss
            if (shouldSpawnBoss && currentTime >= 21) {
                SpawnBoss();
            }
        }
        EndWave();
    }

    public void DecideMusic() {
        float currentTime = getCurrentTime();
        if (currentTime < lightsTurnOffTime || currentTime >= lightsTurnOnTime) {
            MusicManager.Instance.FadeToNightMusic();
        } else if (currentTime >= lightsTurnOffTime && currentTime < lightsTurnOnTime) {
            MusicManager.Instance.FadeToDayMusic();
        }
    }

    private Color GetInterpolatedColor(float normalizedTime) {
        if (lightingColors.Count < 2) return Color.white;

        int colorIndex = Mathf.FloorToInt(normalizedTime * (lightingColors.Count - 1));
        float lerpFactor = (normalizedTime * (lightingColors.Count - 1)) - colorIndex;

        if (colorIndex >= lightingColors.Count - 1) return lightingColors[lightingColors.Count - 1];

        return Color.Lerp(lightingColors[colorIndex], lightingColors[colorIndex + 1], lerpFactor);
    }
}
