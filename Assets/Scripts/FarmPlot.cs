using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum TaskType : int {
    Plant = 0,
    Water = 1,
    Harvest = 2,
    Shear = 3
}

[System.Serializable]
public struct Task {
    public Transform transform;
    public TaskType type;

    public Task(Transform transform, TaskType type) {
        this.transform = transform;
        this.type = type;
    }
}

public class FarmPlot : MonoBehaviour
{
    [SerializeField] private GameObject farmHandPrefab;
    [SerializeField] private int radius;
    [SerializeField] private float gridStep;
    [SerializeField] private CropType cropType;
    public List<Task> openTasks;
    public List<Task> claimedTasks;
    public List<Task> allTasks;
    public List<Vector2> plotPoints;


    public List<Vector2> needsPlant;
    public List<Vector2> needsWater;
    public List<Vector2> needsHarvest;

    void Awake() {
        openTasks = new List<Task>();
        claimedTasks = new List<Task>();
        allTasks = new List<Task>();
    }

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

    public void ClaimTask(Task task) {
        if (openTasks.Contains(task)) openTasks.Remove(task);
        if (!claimedTasks.Contains(task)) claimedTasks.Add(task);
    }

    public void UnclaimTask(Task task) {
        if (claimedTasks.Contains(task)) claimedTasks.Remove(task);
    }

    public void ScanCrops() {
        // All plot points that should have a crop
        List<Vector2> checklist = new List<Vector2>(plotPoints).Select(pos => NormalizeVector2(pos)).ToList();
        List<Vector2> seenLocations = new List<Vector2>();

        openTasks.Clear();
        allTasks.Clear();

        // Check already planted crops in range
        Collider2D[] cols = Physics2D.OverlapBoxAll((Vector2) transform.position, new Vector2(radius,radius), 0);
        foreach (Collider2D col in cols) {
            if (col.gameObject.TryGetComponent<Crop>(out Crop crop)) {
                seenLocations.Add((Vector2) crop.transform.position);

                if (crop.state == CropState.Dry) {
                    allTasks.Add(new Task(crop.transform, TaskType.Water));
                } else if (crop.state == CropState.Ready) {
                    allTasks.Add(new Task(crop.transform, TaskType.Harvest));
                }
            }            
        }
        seenLocations = seenLocations.Select(pos => NormalizeVector2(pos)).ToList();

        // Plant any missing crops
        foreach (Vector2 position in checklist.Except(seenLocations).ToList()) {
            Crop newCrop = CropManager.Instance.PlantCrop(position, cropType);
            allTasks.Add(new Task(newCrop.transform, TaskType.Water));
        }

        openTasks = allTasks.Except(claimedTasks).ToList();
    }

    private IEnumerator ContinouslyScan() {
        while (true) {
            ScanCrops();
            yield return new WaitForSeconds(3f);
        }
    }
}