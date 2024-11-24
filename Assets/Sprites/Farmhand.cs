using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farmhand : MonoBehaviour
{
    [SerializeField] private FarmPlot farmPlot;

    private List<Vector2> needsPlant;
    private List<Vector2> needsWater;
    private ToolBelt toolBelt;

    void Awake() {
        toolBelt = GetComponent<ToolBelt>();
    }

    void Start() {
        CheckCrops();
        StartCoroutine(ContinuouslyScan());
    }

    void Update() {
        if (needsPlant.Count == 0 && needsWater.Count == 0) return;

        if (needsPlant.Count > 0) {
            if ((Vector2) transform.position == needsPlant[0]) {
                toolBelt.UseTool(Tool.SeedBag);
                needsPlant.RemoveAt(0);
            } else {
                transform.position = Vector2.Lerp((Vector2) transform.position, needsPlant[0], 0.01f);
            }
        } else if (needsWater.Count > 0) {
            if ((Vector2) transform.position == needsWater[0]) {
                toolBelt.UseTool(Tool.WateringCan);
                needsWater.RemoveAt(0);
            } else {
                transform.position = Vector2.Lerp((Vector2) transform.position, needsWater[0], 0.01f);
            }
        }
    }

    public void CheckCrops() {
        needsPlant = farmPlot.needsPlant;
        needsWater = farmPlot.needsWater;
    }

    private IEnumerator ContinuouslyScan() {
        while (true) {
            CheckCrops();
            yield return new WaitForSeconds(10f);
        }
    }
}
