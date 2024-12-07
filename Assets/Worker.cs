using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : MonoBehaviour
{
    [SerializeField] private WorkerBuilding building;
    private CharacterMover characterMover;

    void Awake() {
        characterMover = GetComponent<CharacterMover>();
        if (TryGetComponent<Damagable>(out Damagable damagable)) {
            damagable.onDeath.AddListener(() => building.RemoveWorker(this));
        }
    }

    public void SetWorkerBuilding(WorkerBuilding newBuilding)  {
        building = newBuilding;
    }

    public void SetWanderAnchor(Transform location) {
        characterMover.SetWanderAnchor(location);
    }
}
