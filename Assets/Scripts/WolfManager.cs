using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfManager : MonoBehaviour
{
    public static WolfManager Instance { get; private set; }

    [SerializeField] private GameObject wolfPrefab;
    private IntRange spawnCount;
    private FloatRange spawnInterval;
    [SerializeField] private float attackDamage;
    [SerializeField] private float maxHealth;
    private int wolfCount;

    void Awake() {
        if (Instance == null) {Instance = this;}
        else {Destroy(gameObject);}
    }

    void Start() {
        StartCoroutine(WolfSpawner());
    }

    public void IncreaseDamageByPct(float pct) {
        attackDamage = attackDamage * (1f+pct);
    }

    public void IncreaseHealthByPct(float pct) {
        maxHealth = maxHealth * (1f+pct);
    }

    public void SetSpawnCount(IntRange newRange) {
        spawnCount = newRange;
    }

    public void SetSpawnInterval(FloatRange newInterval) {
        spawnInterval = newInterval;
    }

    public void IncreaseWolfCount() {
        wolfCount += 1;
    }

    public void DecreaseWolfCount() {
        wolfCount -= 1;
    }

    public void SpawnWolves() {
        for (int i = 0; i < Random.Range(spawnCount.min, spawnCount.max); i++) {
            GameObject wolfObj = SpawnManager.Instance.SpawnObject(wolfPrefab);
            IncreaseWolfCount();

            Wolf wolf = wolfObj.GetComponent<Wolf>();
            wolf.SetMaxHealth(maxHealth);
            wolf.SetAttackDamage(attackDamage);
        }
    }

    private IEnumerator WolfSpawner() {
        while (true) {
            if (wolfCount < 200) SpawnWolves();

            float waitTime = 0f;
            if (WaveManager.Instance.getCurrentTime() < 5 || WaveManager.Instance.getCurrentTime() >= 21) {
                waitTime = Random.Range(spawnInterval.min/2f, spawnInterval.max/2f);
            } else {
                waitTime = Random.Range(spawnInterval.min, spawnInterval.max);
            }
            yield return new WaitForSeconds(waitTime);
        }
    }
}
