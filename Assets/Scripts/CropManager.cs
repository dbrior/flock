using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropManager : MonoBehaviour
{
    public static CropManager Instance { get; private set;}
    [SerializeField] private GameObject cropPrefab;
    [SerializeField] private IntRange dailySpawnAmount;
    private List<Crop> crops;
    private List<Crop> cropsToRemove;

    void Awake() {
        if (Instance == null) {Instance = this;} 
        else {Destroy(gameObject);}

        crops = new List<Crop>();
        cropsToRemove = new List<Crop>();
    }

    public void AdvanceCrops() {
        foreach (Crop crop in crops) {
            crop.NextState();
        }
        foreach (Crop crop in cropsToRemove) {
            crops.Remove(crop);
            Destroy(crop.gameObject);
        }
        cropsToRemove.Clear();
    }

    public void PlantCrop(Vector2 position) {
        Crop crop = Instantiate(cropPrefab, position, Quaternion.identity).GetComponent<Crop>();
        crops.Add(crop);
    }

    public void RemoveCrop(Crop crop) {
        cropsToRemove.Add(crop);
    }

    public void RemoveCropImmediately(Crop crop) {
        crops.Remove(crop);
        Destroy(crop.gameObject);
    }

    public void SpawnRandomCrops() {
        int spawnAmount = Random.Range(dailySpawnAmount.min, dailySpawnAmount.max);
        for (int i=0; i<spawnAmount; i++) {
            Crop crop = SpawnManager.Instance.SpawnObject(cropPrefab).GetComponent<Crop>();
            crop.SetState(CropState.Ready);
            crops.Add(crop);
        }
    }
}
