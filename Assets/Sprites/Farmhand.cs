using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farmhand : MonoBehaviour
{
    [SerializeField] private FarmPlot farmPlot;

    private List<Vector2> targetPositions;

    void Start() {
        CheckCrops();
    }

    void Update() {
        if (targetPositions.Count == 0) return;

        if ((Vector2) transform.position == targetPositions[0]) {
            // Debug.Log("Reached point");
            targetPositions.RemoveAt(0);
        } else {
            transform.position = Vector2.Lerp((Vector2) transform.position, targetPositions[0], 0.01f);
        }
    }

    public void CheckCrops() {
        targetPositions = farmPlot.plotPoints;
    }
}
