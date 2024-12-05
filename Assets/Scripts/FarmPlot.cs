using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum FarmTodo : int {
    Plant = 0,
    Water = 1
}

public class FarmPlot : MonoBehaviour
{
    [SerializeField] private GameObject farmHandPrefab;
    [SerializeField] private int radius;
    [SerializeField] private float gridStep;
    [SerializeField] private CropType cropType;
    public List<Vector2> plotPoints;
    public List<Vector2> needsPlant;
    public List<Vector2> needsWater;
    public List<Vector2> needsHarvest;

    void Start() {
        plotPoints = GetPoints();
        StartCoroutine(ContinouslyScan());
    }
    
    private List<Vector2> GetPoints() {
        List<Vector2> points = new List<Vector2>();

        float minX = transform.position.x-(radius*gridStep);
        minX = Mathf.Round(minX*100f)/100f;
        float maxX = transform.position.x+(radius*gridStep);
        maxX = Mathf.Round(maxX*100f)/100f;
        maxX += 0.01f;

        float minY = transform.position.y-(radius*gridStep);
        minY = Mathf.Round(minY*100f)/100f;
        float maxY = transform.position.y+(radius*gridStep);
        maxY = Mathf.Round(maxY*100f)/100f;
        maxY += 0.01f;

        for (float x=minX; x<=maxX; x+=gridStep) {
            for (float y=minY; y<=maxY; y+=gridStep) {
                if (x == transform.position.x && y == transform.position.y) continue;
                points.Add(new Vector2(Mathf.Round(x*100f)/100f, Mathf.Round(y*100f)/100f));
            }
        }

        return points;
    }

    public void SpawnFarmHand() {
        Farmhand newFarmHand = Instantiate(farmHandPrefab, transform.position, transform.rotation).GetComponent<Farmhand>();
        newFarmHand.SetFarmPlot(this);
    }

    public void IncreaseRadius(int delta) {
        radius += delta;
        plotPoints = GetPoints();
        ScanCrops();
    }

    private Vector2 NormalizeVector2(Vector2 vector, int decimals = 2)
    {
        float factor = Mathf.Pow(10, decimals);
        return new Vector2(
            Mathf.Round(vector.x * factor) / factor,
            Mathf.Round(vector.y * factor) / factor
        );
    }

    public void ScanCrops() {
        List<Vector2> checklist = new List<Vector2>(plotPoints).Select(pos => NormalizeVector2(pos)).ToList();
        // needsPlant.Clear();
        needsWater.Clear();

        List<Vector2> cropLocations = new List<Vector2>();
        Collider2D[] cols = Physics2D.OverlapBoxAll((Vector2) transform.position, new Vector2(radius,radius), 0);
        foreach (Collider2D col in cols) {
            if (col.gameObject.TryGetComponent<Crop>(out Crop crop)) {
                cropLocations.Add((Vector2) crop.transform.position);
                if (crop.state == CropState.Dry) {
                    needsWater.Add(crop.transform.position);
                } else if (crop.state == CropState.Ready) {
                    needsHarvest.Add(crop.transform.position);
                }
            }            
        }
        cropLocations = cropLocations.Select(pos => NormalizeVector2(pos)).ToList();

        needsPlant = checklist.Except(cropLocations).ToList();
        foreach (Vector2 position in needsPlant) {
            CropManager.Instance.PlantCrop(position, cropType);
            needsWater.Add(position);
        }
        needsPlant.Clear();
    }

    private IEnumerator ContinouslyScan() {
        while (true) {
            ScanCrops();
            yield return new WaitForSeconds(30f);
        }
    }
}