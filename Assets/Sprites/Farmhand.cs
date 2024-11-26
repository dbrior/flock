using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FarmhandState {
    Wander,
    Plant,
    Water
}

public class Farmhand : MonoBehaviour
{
    [SerializeField] private FarmPlot farmPlot;

    private List<Vector2> needsPlant;
    private List<Vector2> needsWater;
    private ToolBelt toolBelt;
    private Rigidbody2D rb;
    private Vector2 targetPosition;
    [SerializeField] private float moveSpeed;
    private FarmhandState state;

    void Awake() {
        toolBelt = GetComponent<ToolBelt>();
    }

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        CheckCrops();
        // StartCoroutine(ContinuouslyScan());
    }

    void Update() {
        // if (needsPlant.Count == 0 && needsWater.Count == 0) return;

        if (((Vector2) transform.position - targetPosition).magnitude < 0.05) {
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
            }

            CheckCrops();
        }
    }

    void FixedUpdate() {
        Vector2 targetDirection = (targetPosition - (Vector2) transform.position);
        Vector2 desiredVelocity = targetDirection.normalized * moveSpeed;
        Vector2 deltaVelocity = desiredVelocity - rb.velocity;
        Vector2 force = rb.mass * deltaVelocity / Time.fixedDeltaTime;
        rb.AddForce(force);
    }

    public void CheckCrops() {
        needsPlant = farmPlot.needsPlant;
        needsWater = farmPlot.needsWater;

        if (needsPlant.Count > 0) {
            state = FarmhandState.Plant;
            targetPosition = needsPlant[0];
        } else if (needsWater.Count > 0) {
            state = FarmhandState.Water;
            targetPosition = needsWater[0];
        } else {
            state = FarmhandState.Wander;
            targetPosition = transform.position;
        }
    }

    private IEnumerator ContinuouslyScan() {
        while (true) {
            CheckCrops();
            yield return new WaitForSeconds(10f);
        }
    }
}
