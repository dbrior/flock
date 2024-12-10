using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum TaskType : int {
    Plant = 0,
    Water = 1,
    Harvest = 2,
    Shear = 3,
    Heal = 4,
    CollectItem = 5
}

[System.Serializable]
public class Task {
    public Transform transform;
    public TaskType type;
    public Item item;
    public int amount;

    public Task(Transform transform, TaskType type) {
        this.transform = transform;
        this.type = type;
    }
    public Task(Transform transform, TaskType type, Item item, int amount) {
        this.transform = transform;
        this.type = type;
        this.item = item;
        this.amount = amount;
    }
}

[System.Serializable]
public class ResourceNode {
    public Transform transform;
    public Item item;
}

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance { get; private set; }

    [SerializeField] private List<ResourceNode> resourceNodes = new List<ResourceNode>();
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

    public void CompleteTask(Task task) {
        if (taskBuildings.ContainsKey(task.type)) {
            List<WorkerBuilding> buildings = taskBuildings[task.type];
            foreach (WorkerBuilding building in buildings) {
                building.CompleteTask(task);
            }
        }
    }

    public void RemoveTask(Task task) {
        if (taskBuildings.ContainsKey(task.type)) {
            List<WorkerBuilding> buildings = taskBuildings[task.type];
            foreach (WorkerBuilding building in buildings) {
                building.RemoveTask(task);
            }
        }
    }

    public ResourceNode GetNodeForItem(Item item) {
        foreach (ResourceNode node in resourceNodes) {
            if (node.item == item) {
                return node;
            }
        }
        return null;
    }
}
