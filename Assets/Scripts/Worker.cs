using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : MonoBehaviour
{
    [SerializeField] private bool shouldScanForTasks;
    [SerializeField] private bool shouldContinuouslyNavigate;
    [SerializeField] private float scanIntervalSec;

    private WorkerBuilding building;
    private CharacterMover characterMover;
    [SerializeField] private Task currentTask;
    private Inventory inventory;
    private Item targetItem;
    private int targetAmount;

    void Awake() {
        characterMover = GetComponent<CharacterMover>();
        inventory = GetComponent<Inventory>();
    }

    void Start() {
        if (building != null && TryGetComponent<Damagable>(out Damagable damagable)) {
            damagable.onDeath.AddListener(() => building.RemoveWorker(this));
            damagable.onDeath.AddListener(AbandonTask);
        }
        if (building != null && shouldScanForTasks) StartCoroutine("ScanForTask");

        if (!shouldScanForTasks) characterMover.StartWandering();
    }

    public void SetWorkerBuilding(WorkerBuilding newBuilding)  {
        building = newBuilding;
    }

    public void SetWanderAnchor(Transform location) {
        characterMover.SetWanderAnchor(location);
    }

    private void RequestTask() {
        Task newTask = building.RequestTask();
        // Start wandering if no task
        if (newTask == null) {
            characterMover.StartWandering();
            return;
        }

        characterMover.StopWandering();

        currentTask = newTask;

        // Allow for either transform-based or position-based task navigation
        if (currentTask.transform != null) {
            characterMover.NavigateTo(currentTask.transform);
            // Debug.Log("Navigate to " + currentTask.transform.gameObject.name);
        } else {
            characterMover.NavigateTo(currentTask.position);
            // Debug.Log("Navigate to " + currentTask.position);
        }

        if (newTask.type == TaskType.CollectItem) {
            targetItem = currentTask.item;
            targetAmount = currentTask.amount;
        } else if (newTask.type == TaskType.Heal || newTask.type == TaskType.Shear) {
            // Do nothing since the target is the one that clears the task once complete
            {}
        } else {
            characterMover.onReachDestination = () => CompleteTask(currentTask);
            characterMover.onAbandonDestination = () => CompleteTask(currentTask);
            // TODO make separate invalid destination
        }
    }

    public void ReceivedItem(Item item) {
        if (currentTask == null) return;
        
        if (currentTask.type == TaskType.CollectItem && item == targetItem) {
            if (inventory.GetItemCount(targetItem) >= targetAmount) {
                characterMover.NavigateTo(building.transform);
                characterMover.onReachDestination = () => building.GetComponent<ResourceProcessingBuilding>().DepositItem(inventory, targetItem, inventory.GetItemCount(targetItem));
                characterMover.onReachDestination += () => CompleteTask(currentTask);
            }
        }
    }

    private void CompleteTask(Task task) {
        building.CompleteTask(task);
        if (currentTask == task) {
            currentTask = null;
        }
    }

    public void AbandonTask() {
        if (currentTask == null) return;

        // Debug.Log("Abandoning task: " + currentTask.type.ToString());
        building.AbandonTask(currentTask);
    }

    IEnumerator ScanForTask() {
        while (shouldScanForTasks) {
            if (!building.IsTaskValid(currentTask)) {
                RequestTask();
            } else if (shouldContinuouslyNavigate) {
                characterMover.NavigateTo(currentTask.transform);
            }
            yield return new WaitForSeconds(scanIntervalSec);
        }
    }
}
