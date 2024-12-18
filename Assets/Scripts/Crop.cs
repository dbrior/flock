using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum CropState {
    Dry = 0,
    Watered = 1,
    Growing = 2,
    Ready = 3,
    Dead = 4
}
public class Crop : MonoBehaviour
{
    public CropType type;

    public CropState state;
    private int totalStates;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private ItemSpawner itemSpawner;
    public bool isWildCrop = false;
    private float growthTimeSec = 30f;

    private WorkerBuilding building;

    void Awake() {
        totalStates = System.Enum.GetValues(typeof(CropState)).Length;
        TryGetComponent<Animator>(out Animator animatorComp);
        animator = animatorComp;
        itemSpawner = GetComponent<ItemSpawner>();
    }

    void Start() {
        if (!isWildCrop) SetState(CropState.Dry);
        StartCoroutine("GrowthTimer");
    }

    public void SetBuilding(WorkerBuilding newBuilding) {
        building = newBuilding;
    }

    public void SetState(CropState newState) {
        state = newState;
        if (animator != null) {
            animator.SetInteger("State", (int) state);
        }
        StopCoroutine("GrowthTimer");
        StartCoroutine("GrowthTimer");

        if (building != null) {
            if (state == CropState.Dry) building.AddTask(new Task(transform, TaskType.Water));
            else if (state == CropState.Ready) building.AddTask(new Task(transform, TaskType.Harvest));
        }
        
    }

    public void Water() {
        if (state == CropState.Dry) {
            SetState(CropState.Watered);
            if (building != null) building.RemoveTask(new Task(transform, TaskType.Water));
            QuestManager.Instance.PlantCrop(type);
        }
    }

    public void Harvest() {
        if (state == CropState.Ready) {
            itemSpawner.SpawnItems();
            if (isWildCrop) {
                CropManager.Instance.RemoveCropImmediately(this);
            } else {
                if (building != null) building.RemoveTask(new Task(transform, TaskType.Harvest));
                SetState(CropState.Dry);
            }
        }
    }

    public void WorkCrop() {
        if (state == CropState.Dry) {
            Water();
        } else if (state == CropState.Ready) {
            Harvest();
        }
    }

    public void NextState() {
        // if (state == CropState.Dry || state == CropState.Ready) {
            // CropManager.Instance.RemoveCrop(this);
        // } else 
        if (isWildCrop && state == CropState.Dry) {
            CropManager.Instance.RemoveCrop(this);
        }

        if (state == CropState.Watered) {
            SetState(CropState.Growing);
        } else if (state == CropState.Growing) {
            SetState(CropState.Ready);
        }
    }

    public float GetGrowthTimeSec() {
        return growthTimeSec;
    }

    public void SetGrowthTimeSec(float newGrowthTimeSec) {
        growthTimeSec = newGrowthTimeSec;
    }

    private void OnTriggerStay2D(Collider2D col) {
        if (col.gameObject.TryGetComponent<CropTools>(out CropTools cropTools)) {
            WorkCrop();
        }
    }

    IEnumerator GrowthTimer() {
        while (true) {
            yield return new WaitForSeconds(growthTimeSec);
            NextState();
        }
    }
}
