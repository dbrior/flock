using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthListener : MonoBehaviour
{
    [SerializeField] private float firePct;
    private Damagable damagable;
    private bool fired = false;
    private Task healingTask;

    void Awake() {
        damagable = GetComponent<Damagable>();
    }

    void Start() {
        damagable.onDeath.AddListener(() => TaskManager.Instance.RemoveTask(GetHealingTask()));
    }

    private Task GetHealingTask() {
        return healingTask;
    }

    void Update() {
        if (!fired && damagable.GetHealthPct() <= firePct) {
            Debug.Log("Health listener fired: " + gameObject.name);
            fired = true;
            healingTask = new Task(transform, TaskType.Heal);
            TaskManager.Instance.SubmitTask(healingTask);
        } else if (fired && damagable.GetHealthPct() > firePct) {
            Debug.Log("Health listener resolved: " + gameObject.name);
            fired = false;
            TaskManager.Instance.CompleteTask(healingTask);
            healingTask = null;
        }
    }
}
