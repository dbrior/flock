using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepTaskGenerator : MonoBehaviour
{
    [SerializeField] private float taskGenerationIntervalSec;

    private WorkerBuilding building;
    private List<Transform> unshearedSheepTransforms;

    void Awake() {
        building = GetComponent<WorkerBuilding>();
    }

    void Start() {
        StartCoroutine("GenerateTasks");
    }

    IEnumerator GenerateTasks() {
        while (true) {
            unshearedSheepTransforms = SheepManager.Instance.GetTameSheep().Where(sheep => sheep != null && !sheep.IsSheared()).Select(sheep => sheep.transform).ToList();
            foreach (Transform sheepTransform in unshearedSheepTransforms) {
                Task shearTask = new Task(sheepTransform, TaskType.Shear);
                building.AddTask(shearTask);
            }
            yield return new WaitForSeconds(taskGenerationIntervalSec);
        }
    }
}
