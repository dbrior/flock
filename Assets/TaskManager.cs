using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum TaskType : int {
    Plant = 0,
    Water = 1,
    Harvest = 2,
    Shear = 3,
    Heal = 4
}

[System.Serializable]
public class Task {
    public Transform transform;
    public TaskType type;

    public Task(Transform transform, TaskType type) {
        this.transform = transform;
        this.type = type;
    }
}

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance { get; private set; }

    private Dictionary<TaskType,List<WorkerBuilding>> taskBuildings = new Dictionary<TaskType, List<WorkerBuilding>>();

    void Awake() {
        if (Instance == null) {Instance = this;} 
        else {Destroy(gameObject);}
    }

    public void AddBuilding(TaskType type, WorkerBuilding building) {
        if (!taskBuildings.ContainsKey(type)) {
            taskBuildings[type] = new List<WorkerBuilding>();
        }
        taskBuildings[type].Add(building);
    }

    public void SubmitTask(Task task) {
        if (taskBuildings.ContainsKey(task.type)) {
            List<WorkerBuilding> buildings = taskBuildings[task.type];
            buildings[Random.Range(0, buildings.Count)].AddTask(task);
        }
    }
}
