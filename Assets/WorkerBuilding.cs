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

    [SerializeField] private List<Task> openTasks = new List<Task>();
    [SerializeField] private List<Task> claimedTasks = new List<Task>();

    [SerializeField] private float workerMaxHealth;
    [SerializeField] private float workerDamage;
    [SerializeField] private float fireRate;

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
        Worker newWorker = Instantiate(workerPrefab, spawnTransform.position, spawnTransform.rotation).GetComponent<Worker>();
        newWorker.SetWorkerBuilding(this);
        workers.Add(newWorker);

        if (wanderAnchor != null) {
            newWorker.SetWanderAnchor(wanderAnchor);
        }

        // Set worker values
        // workerDamage.GetComponent<Damagable>().SetMax
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
        if (!openTasks.Contains(task)) openTasks.Add(task);
    }

    public void AddTask(Task task) {
        if (!openTasks.Contains(task) && !claimedTasks.Contains(task)) {
            openTasks.Add(task);
        }
    }

    public void AbandonTask(Task task) {
        UnclaimTask(task);
    }

    public void RemoveTask(Task task) {
        if (openTasks.Contains(task)) openTasks.Remove(task);
        if (claimedTasks.Contains(task)) claimedTasks.Remove(task);
    }

    public int GetItemCollectItemCount(Item item) {
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

    public int GetItemCollectTaskCount(Item item) {
        int count = 0;
        foreach (Task task in openTasks) {
            if (task.type == TaskType.CollectItem && task.item == item) {
                count += 1;
            }
        }
        foreach (Task task in claimedTasks) {
            if (task.type == TaskType.CollectItem && task.item == item) {
                count += 1;
            }
        }
        return count;
    }

    public Task RequestTask() {
        if (openTasks.Count == 0) return null;

        Task task = openTasks[Random.Range(0, openTasks.Count)];
        if (task.transform == null) {
            RemoveTask(task);
            return RequestTask();
        }
        ClaimTask(task);

        return task;
    }

    public void CompleteTask(Task task) {
        Debug.Log("Task complete " + task.type.ToString());
        RemoveTask(task);
    }

    public bool IsTaskValid(Task task) {
        if (task == null) return false;

        return openTasks.Contains(task) || claimedTasks.Contains(task);
    }
}
