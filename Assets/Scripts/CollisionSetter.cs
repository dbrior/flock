using UnityEngine;
using System.Collections.Generic;

public class CollisionSetter : MonoBehaviour
{
    [SerializeField] private List<string> layersA;
    [SerializeField] private List<string> layersB;

    void Awake() {
        IgnoreCollisions();
    }

    private void IgnoreCollisions() {
        foreach (string layerA in layersA) {
            foreach (string layerB in layersB) {
                int indexA = LayerMask.NameToLayer(layerA);
                int indexB = LayerMask.NameToLayer(layerB);

                if (indexA != -1 && indexB != -1) { // Ensure layers are valid
                    Physics2D.IgnoreLayerCollision(indexA, indexB, true);
                } else {
                    Debug.LogWarning($"Invalid layer name: {layerA} or {layerB}");
                }
            }
        }
    }
}