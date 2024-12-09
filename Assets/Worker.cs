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
    private Inventory inventory;
    private Item targetItem;
    private int targetAmount;

    void Awake() {
        characterMover = GetComponent<CharacterMover>();
        inventory = GetComponent<Inventory>();
        if (building != null && TryGetComponent<Damagable>(out Damagable damagable)) {
            damagable.onDeath.AddListener(() => building.RemoveWorker(this));
        }
    }

    void Start() {
        if (building != null && shouldScanForTasks) StartCoroutine("ScanForTask");
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

        Debug.Log("Navigate to " + currentTask.transform.gameObject.name);

        if (newTask.type != TaskType.CollectItem) {
            characterMover.onReachDestination = () => CompleteTask(currentTask);
            characterMover.onAbandonDestination = () => CompleteTask(currentTask);
        } else {
            targetItem = currentTask.item;
            targetAmount = currentTask.amount;
        }
    }

    public void ReceivedItem(Item item) {
        if (currentTask.type == TaskType.CollectItem && item == targetItem) {
            Debug.Log("worker received itrem");
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

    IEnumerator ScanForTask() {
        while (shouldScanForTasks) {
            if (!building.IsTaskValid(currentTask)) {
                RequestTask();
            }
            yield return new WaitForSeconds(scanIntervalSec);
        }
    }
}
