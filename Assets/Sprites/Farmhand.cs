using System.Collections;
using System.Collections.Generic;
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

    private List<Transform> needsShear;
    private ToolBelt toolBelt;
    private Rigidbody2D rb;
    [SerializeField] private FarmhandState state;

    void Awake() {
        toolBelt = GetComponent<ToolBelt>();
        rb = GetComponent<Rigidbody2D>();
        characterMover = GetComponent<CharacterMover>();
    }

    void Start() {
        StartCoroutine(ContinuouslyScan());
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
        characterMover.NavigateTo(currTask.transform);

        // Set action at destination
        if (newTask.type == TaskType.Water) {
            characterMover.onReachDestination = () => toolBelt.UseTool(Tool.WateringCan);
        } else if (newTask.type == TaskType.Harvest) {
            characterMover.onReachDestination = () => toolBelt.UseTool(Tool.Shears);
        }

        // Remove claimed task once complete
        characterMover.onReachDestination += () => farmPlot.UnclaimTask(currTask);
    }


    // TODO remove
    private Transform sheepTask;
    public void ClearSheepTask() {
        sheepTask = null;
        // characterMover.StopNavigation();
    }
    public void CheckSheep() {
        needsShear = SheepManager.Instance.GetTameSheep().Where(sheep => sheep != null && !sheep.IsSheared()).Select(sheep => sheep.transform).ToList();
        if (needsShear.Contains(sheepTask)) {
            characterMover.NavigateTo(sheepTask.position);
            return;
        }

        if (needsShear.Count > 0) {
            Transform targetSheep = needsShear[Random.Range(0, needsShear.Count)];
            sheepTask = targetSheep;
            
            characterMover.NavigateTo(targetSheep.position);
            characterMover.onReachDestination = () => toolBelt.UseTool(Tool.Shears);
            // characterMover.onReachDestination += () => ClearSheepTask();
        }
    }

    private void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.TryGetComponent<Sheep>(out Sheep sheep)) {
            if (!sheep.IsSheared()) {
                toolBelt.UseTool(Tool.Shears);
            }

            if (sheep.transform == sheepTask) {
                ClearSheepTask();
            }

            // if (farmhandType == FarmhandType.Herder) {
            //     CheckSheep();
            // }
        }
    }

    private IEnumerator ContinuouslyScan() {
        while (true) {
            if (farmhandType == FarmhandType.Farmer) {
                CheckCrops();
            } else if (farmhandType == FarmhandType.Herder) {
                CheckSheep();
            }
            yield return new WaitForSeconds(1f);
        }
    }
}
