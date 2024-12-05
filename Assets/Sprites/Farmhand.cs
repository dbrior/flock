using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum FarmhandState {
    Wander,
    Plant,
    Water,
    Harvest,
    Shear
}

public enum FarmhandType {
    Farmer,
    Herder,
    Hunter
}

public class Farmhand : MonoBehaviour
{
    [SerializeField] private FarmhandType farmhandType;
    [SerializeField] public FarmPlot farmPlot;

    private List<Vector2> needsPlant;
    private List<Vector2> needsWater;
    private List<Vector2> needsHarvest;
    private List<Transform> needsShear;
    private ToolBelt toolBelt;
    private Rigidbody2D rb;
    private Vector2 targetPosition;
    private Transform targetTransform;
    [SerializeField] private float moveSpeed;
    [SerializeField] private FarmhandState state;

    void Awake() {
        toolBelt = GetComponent<ToolBelt>();
    }

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        targetPosition = transform.position;
        targetTransform = transform;
        // CheckCrops();
        StartCoroutine(ContinuouslyScan());
    }

    void Update() {
        // if (needsPlant.Count == 0 && needsWater.Count == 0) return;

        if (farmhandType == FarmhandType.Farmer) {
            if (((Vector2) transform.position - targetPosition).magnitude < 0.09) {
                if (state == FarmhandState.Plant) {
                    toolBelt.UseTool(Tool.SeedBag);
                    if (needsPlant.Count > 0) {
                        needsPlant.RemoveAt(0);
                    }
                } else if (state == FarmhandState.Water) {
                    toolBelt.UseTool(Tool.WateringCan);
                    if (needsWater.Count > 0) {
                        needsWater.RemoveAt(0);
                    }
                } else if (state == FarmhandState.Harvest) {
                    toolBelt.UseTool(Tool.Shears);
                    if (needsHarvest.Count > 0) {
                        needsHarvest.RemoveAt(0);
                    }
                }
                // CheckCrops();
            }
        }
    }

    void FixedUpdate() {
        Vector2 targetDirection = Vector2.zero;
        if (farmhandType == FarmhandType.Farmer) {
            targetDirection = (targetPosition - (Vector2) transform.position);
        } else if (farmhandType == FarmhandType.Herder) {
            if (targetTransform == null) {
                CheckSheep();
            }
            targetDirection = (Vector2) (targetTransform.position - transform.position);
        }
        Vector2 desiredVelocity = targetDirection.normalized * moveSpeed;
        Vector2 deltaVelocity = desiredVelocity - rb.velocity;
        Vector2 force = rb.mass * deltaVelocity / Time.fixedDeltaTime;
        rb.AddForce(force);
    }

    public void SetFarmPlot(FarmPlot plot) {
        farmPlot = plot;
        CheckCrops();
    }

    public void CheckCrops() {
        farmPlot.ScanCrops();
        needsPlant = farmPlot.needsPlant;
        needsWater = farmPlot.needsWater;
        needsHarvest = farmPlot.needsHarvest;

        if (needsPlant.Count > 0) {
            state = FarmhandState.Plant;
            targetPosition = needsPlant[Random.Range(0, needsPlant.Count)];
        } else if (needsWater.Count > 0) {
            state = FarmhandState.Water;
            targetPosition = needsWater[Random.Range(0, needsWater.Count)];
        } else if (needsHarvest.Count > 0) {
            state = FarmhandState.Harvest;
            targetPosition = needsHarvest[Random.Range(0, needsHarvest.Count)];
        } else {
            state = FarmhandState.Wander;
            targetPosition = transform.position;
        }

        Debug.Log(state);
    }

    public void CheckSheep() {
        needsShear = SheepManager.Instance.GetTameSheep().Where(sheep => !sheep.IsSheared()).Select(sheep => sheep.transform).ToList();

        if (needsShear.Count > 0) {
            state = FarmhandState.Shear;
            if (!needsShear.Contains(targetTransform)) {
                targetTransform = needsShear[Random.Range(0, needsShear.Count)];
            }
        } else {
            targetTransform = transform;
            state = FarmhandState.Wander;
        }
    }

    private void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.TryGetComponent<Sheep>(out Sheep sheep)) {
            if (!sheep.IsSheared()) {
                toolBelt.UseTool(Tool.Shears);
            }
            if (farmhandType == FarmhandType.Herder) {
                CheckSheep();
            }
        }
    }

    private IEnumerator ContinuouslyScan() {
        while (true) {
            if (farmhandType == FarmhandType.Farmer) {
                CheckCrops();
            } else if (farmhandType == FarmhandType.Herder) {
                CheckSheep();
            }
            yield return new WaitForSeconds(3f);
        }
    }
}
