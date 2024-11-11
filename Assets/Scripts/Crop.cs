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

    void Awake() {
        totalStates = System.Enum.GetValues(typeof(CropState)).Length;
        TryGetComponent<Animator>(out Animator animatorComp);
        animator = animatorComp;

        SetState(CropState.Dry);
    }

    public void SetState(CropState newState) {
        state = newState;
        if (animator != null) {
            animator.SetInteger("State", (int) state);
        }
    }

    public void Water() {
        if (state == CropState.Dry) {
            SetState(CropState.Watered);
        }
    }

    public void NextState() {
        if (state == CropState.Dry || state == CropState.Ready) {
            CropManager.Instance.RemoveCrop(this);
        } else if (state == CropState.Watered) {
            SetState(CropState.Growing);
        } else if (state == CropState.Growing) {
            SetState(CropState.Ready);
        }
    }
}
