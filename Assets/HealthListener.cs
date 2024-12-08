using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthListener : MonoBehaviour
{
    [SerializeField] private float firePct;
    private Damagable damagable;
    private bool fired = false;

    void Awake() {
        damagable = GetComponent<Damagable>();
    }

    void Update() {
        if (!fired && damagable.GetHealthPct() <= firePct) {
            fired = true;
            TaskManager.Instance.SubmitTask(new Task(transform, TaskType.Heal));
        } else if (fired && damagable.GetHealthPct() >= firePct) {
            fired = false;
        }
    }
}
