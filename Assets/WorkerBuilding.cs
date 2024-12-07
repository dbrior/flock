using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerBuilding : MonoBehaviour
{
    [SerializeField] private GameObject workerPrefab;
    [SerializeField] int workerSlots;
    [SerializeField] float respawnCooldownSec;
    [SerializeField] Transform spawnTransform;
    [SerializeField] Transform wanderAnchor;

    private List<Worker> workers = new List<Worker>();

    void Start() {
        SpawnAllMissingWorkers();
    }

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
    }

    public void RemoveWorker(Worker worker) {
        workers.Remove(worker);
        if (workers.Count < workerSlots) StartCoroutine("SpawnTimer");
    }

    IEnumerator SpawnTimer() {
        yield return new WaitForSeconds(respawnCooldownSec);
        if (workers.Count < workerSlots) SpawnWorker();
    }
}
