using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceProcessingBuilding : MonoBehaviour
{
    private Inventory inventory;
    private WorkerBuilding workerBuilding;
    [SerializeField] private Item inputItem;
    [SerializeField] private Item outputItem;
    [SerializeField] private int targetAmount;
    [SerializeField] private float processingTimeSec;
    private ResourceNode node;
    
    void Awake() {
        inventory = GetComponent<Inventory>();
        workerBuilding = GetComponent<WorkerBuilding>();
    }

    void Start() {
        node = TaskManager.Instance.GetNodeForItem(inputItem);
        StartCoroutine("CheckResources");
    }

    IEnumerator CheckResources() {
        while (true) {
            int missingInputCount = targetAmount - PlayerInventory.Instance.GetItemCount(outputItem);
            if (node != null && missingInputCount > 0) {
                Debug.Log("Task needed");
                Task newTask = new Task(node.transform, TaskType.CollectItem, inputItem, 0);
                int alreadyRequested = workerBuilding.GetItemCollectTaskCount(newTask);
                newTask.amount = missingInputCount - alreadyRequested;

                if (newTask.amount > 0) {
                    Debug.Log("Dispatching task");
                    workerBuilding.AddTask(newTask);
                }
            }

            yield return new WaitForSeconds(3f);
        }
    }
}
