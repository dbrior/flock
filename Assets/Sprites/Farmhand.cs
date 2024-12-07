using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Linq;
using UnityEngine;

public enum FarmhandState {
    Wander,
    Plant,
    Water,
    Harvest,
    Shear
}

public enum FarmhandType {
    Farmer,
    Herder,
    Hunter
}

public class Farmhand : MonoBehaviour
{
    [SerializeField] private FarmhandType farmhandType;
    [SerializeField] public FarmPlot farmPlot;

    private CharacterMover characterMover;
    private Task currTask;
    private NavMeshPath path;

    private List<Transform> needsShear;
    private ToolBelt toolBelt;
    private Rigidbody2D rb;
    [SerializeField] private FarmhandState state;

    void Awake() {
        toolBelt = GetComponent<ToolBelt>();
        rb = GetComponent<Rigidbody2D>();
        characterMover = GetComponent<CharacterMover>();
        path = new NavMeshPath();
    }

    void Start() {
        StartCoroutine(ContinuouslyScan());
    }

    private Vector3[] GeneratePointsToTarget(Vector3 targetPosition) {
        if(NavMesh.CalculatePath(transform.position, targetPosition, NavMesh.AllAreas, path)) {
            return path.corners;
        } else {
            return null;
        }
    }

    public void SetFarmPlot(FarmPlot plot) {
        farmPlot = plot;
        CheckCrops();
    }

    public void CheckCrops() {
        // If current task still needs to be done, exit
        if (farmPlot.allTasks.Contains(currTask) || farmPlot.openTasks.Count == 0) return;
        farmPlot.UnclaimTask(currTask);

        Task newTask = farmPlot.openTasks[Random.Range(0, farmPlot.openTasks.Count)];
        farmPlot.ClaimTask(newTask);
        currTask = newTask;

        // Set destination
        Vector3[] waypoints = GeneratePointsToTarget(currTask.position);
        if (waypoints == null) return;
        characterMover.SetPoints(waypoints);

        // Set action at destination
        if (newTask.type == TaskType.Water) {
            characterMover.onReachDestination = () => toolBelt.UseTool(Tool.WateringCan);
        } else if (newTask.type == TaskType.Harvest) {
            characterMover.onReachDestination = () => toolBelt.UseTool(Tool.Shears);
        }

        // Remove claimed task once complete
        characterMover.onReachDestination += () => farmPlot.UnclaimTask(currTask);
    }

    public void CheckSheep() {
        needsShear = SheepManager.Instance.GetTameSheep().Where(sheep => !sheep.IsSheared()).Select(sheep => sheep.transform).ToList();

        if (needsShear.Count > 0) {
            Transform targetSheep = needsShear[Random.Range(0, needsShear.Count)];
            
            Vector3[] waypoints = GeneratePointsToTarget(targetSheep.position);
            if (waypoints != null) {
                characterMover.SetPoints(waypoints);
                characterMover.onReachDestination = () => toolBelt.UseTool(Tool.Shears);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.TryGetComponent<Sheep>(out Sheep sheep)) {
            if (!sheep.IsSheared()) {
                toolBelt.UseTool(Tool.Shears);
            }
            if (farmhandType == FarmhandType.Herder) {
                CheckSheep();
            }
        }
    }

    private IEnumerator ContinuouslyScan() {
        while (true) {
            if (farmhandType == FarmhandType.Farmer) {
                CheckCrops();
            } else if (farmhandType == FarmhandType.Herder) {
                CheckSheep();
            }
            yield return new WaitForSeconds(3f);
        }
    }
}
