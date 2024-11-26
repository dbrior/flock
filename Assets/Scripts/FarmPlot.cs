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

    private void ScanCrops() {
        List<Vector2> checklist = new List<Vector2>(plotPoints);
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

        needsPlant = checklist.Except(cropLocations).ToList();
    }

    private IEnumerator ContinouslyScan() {
        while (true) {
            ScanCrops();
            yield return new WaitForSeconds(30f);
        }
    }
}