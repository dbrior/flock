using UnityEngine;
using UnityEngine.Rendering.Universal; // Required for Light2D

public class RainbowCycleUnified : MonoBehaviour
{
    [SerializeField] private float colorCycleSpeed = 1.0f; // Speed of the rainbow cycle
    [SerializeField, Range(0.1f, 1.0f)] private float pastelSaturation = 0.5f; // Lower saturation for pastel colors
    [SerializeField, Range(0.5f, 1.0f)] private float pastelBrightness = 0.9f; // Higher brightness for pastel colors

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Light2D spriteLight;

    private float hue; // Tracks the current hue value (0-1)

    void Update()
    {
        // Increment the hue over time
        hue += colorCycleSpeed * Time.deltaTime;
        if (hue > 1.0f) hue -= 1.0f; // Keep hue in the range [0, 1]

        // Convert hue to pastel RGB
        Color pastelColor = Color.HSVToRGB(hue, pastelSaturation, pastelBrightness);

        // Apply the same pastel color to both the sprite and light
        if (spriteRenderer != null) spriteRenderer.color = pastelColor;

        if (spriteLight != null) spriteLight.color = pastelColor;
    }
}
