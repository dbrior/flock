using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : MonoBehaviour
{
    [SerializeField] private bool shouldScanForTasks;
    [SerializeField] private float scanIntervalSec;

    private WorkerBuilding building;
    private CharacterMover characterMover;
    private Task currentTask;

    void Awake() {
        characterMover = GetComponent<CharacterMover>();
        if (TryGetComponent<Damagable>(out Damagable damagable)) {
            damagable.onDeath.AddListener(() => building.RemoveWorker(this));
        }
    }

    void Start() {
        if (shouldScanForTasks) StartCoroutine("ScanForTask");
    }

    public void SetWorkerBuilding(WorkerBuilding newBuilding)  {
        building = newBuilding;
    }

    public void SetWanderAnchor(Transform location) {
        characterMover.SetWanderAnchor(location);
    }

    private void RequestTask() {
        Task newTask = building.RequestTask();
        if (newTask == null) return;

        currentTask = newTask;
        characterMover.NavigateTo(currentTask.transform);
        characterMover.onReachDestination = () => CompleteTask(currentTask);
        characterMover.onAbandonDestination = () => CompleteTask(currentTask);
    }

    private void CompleteTask(Task task) {
        building.CompleteTask(task);
        if (currentTask == task) {
            currentTask = null;
        }
    }

    IEnumerator ScanForTask() {
        while (shouldScanForTasks) {
            if (!building.IsTaskValid(currentTask)) {
                RequestTask();
            }
            yield return new WaitForSeconds(scanIntervalSec);
        }
    }
}
