using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerBuilding : MonoBehaviour
{
    [SerializeField] private List<TaskType> taskTypes;
    [SerializeField] private GameObject workerPrefab;
    [SerializeField] int workerSlots;
    [SerializeField] float respawnCooldownSec;
    [SerializeField] Transform spawnTransform;
    [SerializeField] Transform wanderAnchor;

    private List<Task> openTasks = new List<Task>();
    private List<Task> claimedTasks = new List<Task>();

    private List<Worker> workers = new List<Worker>();

    void Start() {
        foreach (TaskType type in taskTypes) {
            TaskManager.Instance.AddBuilding(type, this);
        }

        SpawnAllMissingWorkers();
    }

    // -------- Worker Spawning --------

    private void SpawnAllMissingWorkers() {
        for (int i=0; i<workerSlots-workers.Count; i++) {
            SpawnWorker();
        }
    }

    private void SpawnWorker() {
        Debug.Log("Spawning worker");
        Worker newWorker = Instantiate(workerPrefab, spawnTransform.position, spawnTransform.rotation).GetComponent<Worker>();
        newWorker.SetWorkerBuilding(this);
        workers.Add(newWorker);

        if (wanderAnchor != null) {
            newWorker.SetWanderAnchor(wanderAnchor);
        }
    }

    public void RemoveWorker(Worker worker) {
        workers.Remove(worker);
        if (workers.Count < workerSlots) StartCoroutine("SpawnTimer");
    }

    public void IncreaseWorkerCount(int amount) {
        workerSlots += 1;
        SpawnWorker();
    }

    IEnumerator SpawnTimer() {
        Debug.Log("Worker spawn timer started");
        yield return new WaitForSeconds(respawnCooldownSec);
        if (workers.Count < workerSlots) SpawnWorker();
    }

    // -------- Tasks --------
    private void ClaimTask(Task task) {
        if (openTasks.Contains(task)) openTasks.Remove(task);
        if (!claimedTasks.Contains(task)) claimedTasks.Add(task);
    }

    private void UnclaimTask(Task task) {
        if (claimedTasks.Contains(task)) claimedTasks.Remove(task);
    }

    public void AddTask(Task task) {
        if (!openTasks.Contains(task) && !claimedTasks.Contains(task)) {
            openTasks.Add(task);
        }
    }

    public int GetItemCollectTaskCount(Item item) {
        int count = 0;
        foreach (Task task in openTasks) {
            if (task.type == TaskType.CollectItem && task.item == item) {
                count += task.amount;
            }
        }
        foreach (Task task in claimedTasks) {
            if (task.type == TaskType.CollectItem && task.item == item) {
                count += task.amount;
            }
        }
        return count;
    }

    public Task RequestTask() {
        if (openTasks.Count == 0) return null;

        Task task = openTasks[Random.Range(0, openTasks.Count)];
        ClaimTask(task);
        Debug.Log("Task calimed");

        return task;
    }

    public void CompleteTask(Task task) {
        UnclaimTask(task);
    }

    public bool IsTaskValid(Task task) {
        if (task == null) return false;

        return openTasks.Contains(task) || claimedTasks.Contains(task);
    }
}
