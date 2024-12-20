using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public enum UnitType {
    None = 0,
    Farmhand = 1,
    Knight = 2,
    Hunter = 3,
    Healer = 4,
    Miner = 5,
    Shepard = 6
}


public class WorkerBuilding : MonoBehaviour
{
    [Header("Building Settings")]
    [SerializeField] private UnitType unitType;
    [SerializeField] private List<TaskType> taskTypes;
    [SerializeField] private GameObject workerPrefab;
    [SerializeField] private int workerSlots;
    [SerializeField] private Transform spawnTransform;
    [SerializeField] private Transform wanderAnchor;

    [Header("Worker Stats")]
    [SerializeField] private bool isStatlessWorker;
    [SerializeField] private float workerMaxHealth;
    [SerializeField] private TextMeshProUGUI maxHealthUI;
    [SerializeField] private float workerBlockChance;
    [SerializeField] private TextMeshProUGUI blockChanceUI;
    [SerializeField] private float workerDamage;
    [SerializeField] private TextMeshProUGUI damageUI;
    [SerializeField] private float workerAttackCooldownSec;
    [SerializeField] private TextMeshProUGUI attackCooldownUI;
    [SerializeField] private float respawnCooldownSec;
    [SerializeField] private TextMeshProUGUI respawnCooldownUI;

    [Header("Tasks")]
    [SerializeField] private List<Task> openTasks = new List<Task>();
    [SerializeField] private List<Task> claimedTasks = new List<Task>();

    private List<Worker> workers = new List<Worker>();

    private float CompoundedRate(float rate, int count) {
        return Mathf.Pow(rate, count);
    }

    void Awake() {
        // Pull prestige stats
        workerSlots = workerSlots + PlayerPrefs.GetInt(unitType.ToString() + "-UnitCount-PurchaseCount", 0);
        workerDamage = workerDamage * CompoundedRate(1.2f, PlayerPrefs.GetInt(unitType.ToString() + "-Damage-PurchaseCount", 0));
        workerAttackCooldownSec = workerAttackCooldownSec * CompoundedRate(0.8f, PlayerPrefs.GetInt(unitType.ToString() + "-AttackCooldown-PurchaseCount", 0));
        workerMaxHealth = workerMaxHealth * CompoundedRate(1.2f, PlayerPrefs.GetInt(unitType.ToString() + "-MaxHealth-PurchaseCount", 0));
        workerBlockChance = workerBlockChance * CompoundedRate(1.2f, PlayerPrefs.GetInt(unitType.ToString() + "-BlockChance-PurchaseCount", 0));
        respawnCooldownSec = respawnCooldownSec * CompoundedRate(0.8f, PlayerPrefs.GetInt(unitType.ToString() + "-RespawnCooldown-PurchaseCount", 0));

        // Debug.Log(unitType + " UnitCount " + workerSlots + " | " + PlayerPrefs.GetInt(unitType.ToString() + "-UnitCount-PurchaseCount", 0) + " purchased");
        // Debug.Log(unitType + " Damage " + workerDamage + " | " + PlayerPrefs.GetInt(unitType.ToString() + "-Damage-PurchaseCount", 0) + " purchased");
    }

    void Start() {
        foreach (TaskType type in taskTypes) {
            TaskManager.Instance.AddBuilding(type, this);
        }

        SpawnAllMissingWorkers();
        if (!isStatlessWorker) UpdateStatUI();
    }

    public int GetWorkerCount() {
        return workerSlots;
    }

    // -------- Worker Spawning --------

    private void SpawnAllMissingWorkers() {
        int missingWorkers = workerSlots-workers.Count;
        for (int i=0; i<missingWorkers; i++) {
            SpawnWorker();
        }
    }

    private void SetWorkerStats(GameObject worker) {
        // Set worker stats
        Damagable workerHealth = worker.GetComponent<Damagable>();
        workerHealth.SetMaxHealth(workerMaxHealth);
        workerHealth.SetBlockChance(workerBlockChance);

        if (worker.TryGetComponent<Attacker>(out Attacker attacker)) {
            attacker.SetAttackCooldownSec(workerAttackCooldownSec);
            attacker.SetDamage(workerDamage);
        } else if (worker.TryGetComponent<RangedAttacker>(out RangedAttacker rangedAttacker)) {
            rangedAttacker.SetAttackCooldownSec(workerAttackCooldownSec);
            rangedAttacker.SetDamage(workerDamage);
        }
    }

    private void UpdateAllWorkerStats() {
        foreach (Worker worker in workers) {
            SetWorkerStats(worker.gameObject);
        }
    }

    private void SpawnWorker() {
        Worker newWorker = Instantiate(workerPrefab, spawnTransform.position, spawnTransform.rotation).GetComponent<Worker>();
        newWorker.SetWorkerBuilding(this);
        workers.Add(newWorker);

        if (wanderAnchor != null) {
            newWorker.SetWanderAnchor(wanderAnchor);
        }

        if (newWorker.TryGetComponent<Damagable>(out Damagable damagable)) {
            SetWorkerStats(newWorker.gameObject);
            damagable.RestoreHealth();
        }
    }   

    private void UpdateStatUI() {
        maxHealthUI.text = UIManager.FormatNumber(workerMaxHealth);
        blockChanceUI.text = UIManager.FormatNumber(workerBlockChance);
        damageUI.text = UIManager.FormatNumber(workerDamage);
        attackCooldownUI.text = UIManager.FormatNumber(workerAttackCooldownSec);
        respawnCooldownUI.text = UIManager.FormatNumber(respawnCooldownSec);
    }

    public void SetWanderAnchor(Transform newWanderAnchor) {
        wanderAnchor = newWanderAnchor;
        for (int i=0; i<workers.Count; i++) {
            workers[i].SetWanderAnchor(wanderAnchor);
        }
    }

    public void ChangeWorkerMaxHealth(float pct) {
        workerMaxHealth *= pct;
        UpdateStatUI();
        UpdateAllWorkerStats();
    }

    public void ChangeWorkerBlockChance(float delta) {
        workerBlockChance += delta;
        UpdateStatUI();
        UpdateAllWorkerStats();
    }

    public void ChangeWorkerDamage(float pct) {
        workerDamage *= pct;
        UpdateStatUI();
        UpdateAllWorkerStats();
    }

    public void ChangeWorkerAttackCooldownSec(float pct) {
        workerAttackCooldownSec *= pct;
        UpdateStatUI();
        UpdateAllWorkerStats();
    }

    public void ChangeWorkerRespawnSec(float pct) {
        respawnCooldownSec *= pct;
        UpdateStatUI();
        UpdateAllWorkerStats();
    }

    public void RemoveWorker(Worker worker) {
        workers.Remove(worker);
        if (workers.Count < workerSlots) StartCoroutine("SpawnTimer");
    }

    public void IncreaseWorkerCount(int amount) {
        workerSlots += 1;
        QuestManager.Instance.PurchaseUnit(unitType);
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
        if (!ContainsTask(task)) openTasks.Add(task);
    }

    public void AbandonTask(Task task) {
        UnclaimTask(task);
    }

    public bool ContainsTask(Task task) {
        for (int i=openTasks.Count-1; i>=0; i--) {
            bool samePosition = !openTasks[i].isPositionTask ? openTasks[i].transform == task.transform : openTasks[i].position == task.position;
            bool sameType = openTasks[i].type == task.type;

            if (samePosition && sameType) return true;
        }
        for (int i=claimedTasks.Count-1; i>=0; i--) {
            bool samePosition = !claimedTasks[i].isPositionTask ? claimedTasks[i].transform == task.transform : claimedTasks[i].position == task.position;
            bool sameType = claimedTasks[i].type == task.type;

            if (samePosition && sameType) return true;
        }
        return false;
    }

    public void RemoveTask(Task task) {
        for (int i=openTasks.Count-1; i>=0; i--) {
            bool samePosition = !openTasks[i].isPositionTask ? openTasks[i].transform == task.transform : openTasks[i].position == task.position;
            bool sameType = openTasks[i].type == task.type;

            if (samePosition && sameType) openTasks.RemoveAt(i);
        }
        for (int i=claimedTasks.Count-1; i>=0; i--) {
            bool samePosition = !claimedTasks[i].isPositionTask ? claimedTasks[i].transform == task.transform : claimedTasks[i].position == task.position;
            bool sameType = claimedTasks[i].type == task.type;

            if (samePosition && sameType) claimedTasks.RemoveAt(i);
        }
        // if (openTasks.Contains(task)) openTasks.Remove(task);
        // if (claimedTasks.Contains(task)) claimedTasks.Remove(task);
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
        if (task.transform == null && !task.isPositionTask) {
            RemoveTask(task);
            return RequestTask();
        }
        ClaimTask(task);

        return task;
    }

    public List<Task> GetClaimedTasks() {
        return claimedTasks;
    }

    public List<Task> GetOpenTasks() {
        return openTasks;
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
