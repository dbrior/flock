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
    private CropState state;
    private int totalStates;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private Sprite drySprite;
    [SerializeField] private Sprite wateredSprite;
    [SerializeField] private Sprite growingSprite;
    [SerializeField] private Sprite readySprite;
    [SerializeField] private Sprite deadSprite;
    private Dictionary<CropState, Sprite> sprites;

    void Awake() {
        state = CropState.Dry;
        totalStates = System.Enum.GetValues(typeof(CropState)).Length;
        spriteRenderer = GetComponent<SpriteRenderer>();

        sprites = new Dictionary<CropState,Sprite>{
            {CropState.Dry, drySprite},
            {CropState.Watered, wateredSprite},
            {CropState.Growing, growingSprite},
            {CropState.Ready, readySprite},
            {CropState.Dead, deadSprite}
        };

        spriteRenderer.sprite = sprites[state];
    }

    public void SetState(CropState newState) {
        state = newState;
        spriteRenderer.sprite = sprites[state];
    }

    public void Water() {
        if (state == CropState.Dry) {
            state = CropState.Watered;
            spriteRenderer.sprite = sprites[state];
        }
    }

    public void NextState() {
        if (state == CropState.Dry || state == CropState.Ready) {
            CropManager.Instance.RemoveCrop(this);
        } else if (state == CropState.Watered) {
            state = CropState.Growing;
            spriteRenderer.sprite = sprites[state];
        } else if (state == CropState.Growing) {
            state = CropState.Ready;
            spriteRenderer.sprite = sprites[state];
        }
    }
}
