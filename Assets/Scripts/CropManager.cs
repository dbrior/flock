using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum CropType : int {
    None,
    Wheat,
    Healing
}

[System.Serializable]
public class CropTypePrefab {
    public CropType type;
    public GameObject prefab;
}

public class CropManager : MonoBehaviour
{
    public static CropManager Instance { get; private set;}
    [SerializeField] private GameObject cropPrefab;
    [SerializeField] private List<CropTypePrefab> cropPrefabList;
    [SerializeField] private IntRange dailySpawnAmount;
    [SerializeField] private Collider2D spawnZone;

    private Dictionary<CropType,GameObject> cropPrefabs = new Dictionary<CropType,GameObject>();
    private List<Crop> crops;
    private List<Crop> cropsToRemove;

    void Awake() {
        if (Instance == null) {Instance = this;} 
        else {Destroy(gameObject);}

        foreach (CropTypePrefab cropTypePrefab in cropPrefabList) {
            cropPrefabs[cropTypePrefab.type] = cropTypePrefab.prefab;
        }

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

    public Crop PlantCrop(Vector2 position, CropType cropType) {
        Crop crop = Instantiate(cropPrefabs[cropType], position, Quaternion.identity).GetComponent<Crop>();
        crops.Add(crop);
        return crop;
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
            Crop crop = SpawnManager.Instance.SpawnObject(cropPrefab, newSpawnZone: spawnZone).GetComponent<Crop>();
            crop.isWildCrop = true;
            crop.SetState(CropState.Ready);
            crops.Add(crop);
        }
    }
}
