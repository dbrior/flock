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
    [SerializeField] private int radius;
    [SerializeField] private float gridStep;
    public List<Vector2> plotPoints;
    public List<Vector2> needsPlant;
    public List<Vector2> needsWater;

    void Start() {
        plotPoints = GetPoints();
        StartCoroutine(ContinouslyScan());
    }
    
    private List<Vector2> GetPoints() {
        List<Vector2> points = new List<Vector2>();

        for (float x=transform.position.x-(radius*gridStep); x<=transform.position.x+(radius*gridStep); x+=gridStep) {
            for (float y=transform.position.y-(radius*gridStep); y<=transform.position.y+(radius*gridStep); y+=gridStep) {
                if (x == transform.position.x && y == transform.position.y) continue;
                points.Add(new Vector2(x, y));
            }
        }

        return points;
    }

    public void IncreaseRadius(int delta) {
        radius += delta;
        plotPoints = GetPoints();
    }

    private Vector2 NormalizeVector2(Vector2 vector, int decimals = 2)
    {
        float factor = Mathf.Pow(10, decimals);
        return new Vector2(
            Mathf.Round(vector.x * factor) / factor,
            Mathf.Round(vector.y * factor) / factor
        );
    }

    private void ScanCrops() {
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
                }
            }            
        }
        cropLocations = cropLocations.Select(pos => NormalizeVector2(pos)).ToList();

        needsPlant = checklist.Except(cropLocations).ToList();
        // Debug.Log("Checklist");
        // foreach (var item in cropLocations) {
        //     Debug.Log(item);
        // }
        // Debug.Log("plant");
        // foreach (var item in needsPlant) {
        //     Debug.Log(item);
        // }
    }

    private IEnumerator ContinouslyScan() {
        while (true) {
            ScanCrops();
            yield return new WaitForSeconds(30f);
        }
    }
}