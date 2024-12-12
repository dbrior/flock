using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceProcessingBuilding : MonoBehaviour
{
    private Inventory inventory;
    private WorkerBuilding workerBuilding;
    private Animator animator;
    [SerializeField] private Item inputItem;
    [SerializeField] private Item outputItem;
    [SerializeField] private int targetAmount;
    [SerializeField] private int maxPerWorker;
    [SerializeField] private float processingTimeSec;
    private ResourceNode node;
    private bool isProcessing;
    
    void Awake() {
        inventory = GetComponent<Inventory>();
        workerBuilding = GetComponent<WorkerBuilding>();
        animator = GetComponent<Animator>();
        isProcessing = false;
    }

    void Start() {
        node = TaskManager.Instance.GetNodeForItem(inputItem);
        StartCoroutine("CheckResources");
    }

    public void DepositItem(Inventory sourceInv, Item item, int amount) {
        if (item != inputItem || sourceInv.GetItemCount(item) < amount) return;
        sourceInv.RemoveItem(item, amount);
        // inventory.AddItem(item, amount);
        PlayerInventory.Instance.AddItem(item, amount);
        // if (!isProcessing) StartCoroutine("Process");
    }

    public void SubmitProcessingJob() {
        if (PlayerInventory.Instance.GetItemCount(inputItem) == 0) return;

        PlayerInventory.Instance.RemoveItem(inputItem, 1);
        inventory.AddItem(inputItem, 1);
        if (!isProcessing) StartCoroutine("Process");
    }

    IEnumerator CheckResources() {
        while (true) {
            int taskCount = workerBuilding.GetItemCollectTaskCount(inputItem);
            if (taskCount < workerBuilding.GetWorkerCount()) {
                Task newTask = new Task(node.transform, TaskType.CollectItem, inputItem, 3);
                workerBuilding.AddTask(newTask);
            }

            // ------ Request Needed Materials ------ //
            // Assumes 1:1 input to output
            // int materialsRequested = workerBuilding.GetItemCollectTaskCount(inputItem);
            // int missingInputCount = targetAmount - (PlayerInventory.Instance.GetItemCount(outputItem) + inventory.GetItemCount(inputItem) + materialsRequested);
            // Debug.Log("Missing " + missingInputCount + " " + inputItem.name + " (" + materialsRequested + " requested)");
            // if (node != null && missingInputCount > 0) {
            //     int tasksToCreate = missingInputCount / maxPerWorker;
            //     if (missingInputCount % maxPerWorker > 0) {
            //         tasksToCreate++;
            //     }
            //     Debug.Log(tasksToCreate + " tasks needed");

            //     for (int i=0; i<tasksToCreate; i++) {
            //         int taskAmount = Mathf.Min(maxPerWorker, missingInputCount - (i * maxPerWorker));
            //         Debug.Log("Dispatching task " + taskAmount + " " + inputItem.name);
                    // Task newTask = new Task(node.transform, TaskType.CollectItem, inputItem, taskAmount);
                    // workerBuilding.AddTask(newTask);
            //     }
            // }
            yield return new WaitForSeconds(3f);
        }
    }

    IEnumerator Process() {
        isProcessing = true;
        if (animator != null) {
            animator.SetTrigger("Process");
        }
        while (inventory.GetItemCount(inputItem) > 0) {
            yield return new WaitForSeconds(processingTimeSec);
            inventory.RemoveItem(inputItem, 1);
            PlayerInventory.Instance.AddItem(outputItem, 1);
        }
        isProcessing = false;
        if (animator != null) {
            animator.SetTrigger("StopProcess");
        }
    }
}
