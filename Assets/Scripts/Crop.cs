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
    public CropState state;
    private int totalStates;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private ItemSpawner itemSpawner;
    public bool isWildCrop = false;

    void Awake() {
        totalStates = System.Enum.GetValues(typeof(CropState)).Length;
        TryGetComponent<Animator>(out Animator animatorComp);
        animator = animatorComp;
        itemSpawner = GetComponent<ItemSpawner>();

        SetState(CropState.Dry);
    }

    void Start() {
        StartCoroutine("GrowthTimer");
    }

    public void SetState(CropState newState) {
        state = newState;
        if (animator != null) {
            animator.SetInteger("State", (int) state);
        }
        StopCoroutine("GrowthTimer");
        StartCoroutine("GrowthTimer");
    }

    public void Water() {
        if (state == CropState.Dry) {
            SetState(CropState.Watered);
        }
    }

    public void Harvest() {
        if (state == CropState.Ready) {
            itemSpawner.SpawnItems();
            if (isWildCrop) {
                CropManager.Instance.RemoveCropImmediately(this);
            } else {
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

    private void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.TryGetComponent<Player>(out Player player)) {
            WorkCrop();
        }
    }

    IEnumerator GrowthTimer() {
        while (true) {
            yield return new WaitForSeconds(30f);
            NextState();
        }
    }
}
