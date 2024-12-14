using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ChromaticAberrationRandomizer : MonoBehaviour
{
    [Header("Randomization Settings")]
    [Tooltip("The Volume component to modify")]
    public Volume globalVolume;

    [Tooltip("Minimum intensity value")]
    [Range(0f, 1f)]
    public float minIntensity = 0f;

    [Tooltip("Maximum intensity value")]
    [Range(0f, 1f)]
    public float maxIntensity = 0.5f;

    [Tooltip("Duration of intensity transition")]
    public float lerpDuration = 2f;

    [Tooltip("Time between intensity changes")]
    public float changeInterval = 3f;

    [Tooltip("Smoothness of the lerp")]
    [Range(0.1f, 10f)]
    public float lerpSmoothing = 1f;

    [Tooltip("Start with randomization enabled")]
    public bool startEnabled = true;

    // Reference to the Chromatic Aberration component
    private ChromaticAberration chromaticAberration;

    // Lerp-related variables
    private float currentIntensity;
    private float targetIntensity;
    private float lerpTime;
    private float changeTimer;

    // Randomization state
    private bool isRandomizing;

    private void Start()
    {
        // Validate the global volume reference
        if (globalVolume == null)
        {
            Debug.LogError("Global Volume is not assigned! Drag and drop a Volume from the scene.");
            enabled = false;
            return;
        }

        // Try to get the Chromatic Aberration component
        if (!globalVolume.profile.TryGet(out chromaticAberration))
        {
            Debug.LogError("No Chromatic Aberration component found in the Volume Profile!");
            enabled = false;
            return;
        }

        // Initial setup
        currentIntensity = chromaticAberration.intensity.value;
        
        // Set initial randomization state
        if (startEnabled)
        {
            EnableRandomization();
        }
        else
        {
            DisableRandomization();
        }
    }

    private void Update()
    {
        if (!isRandomizing) return;

        // Update timers
        changeTimer += Time.deltaTime;
        lerpTime += Time.deltaTime;

        // Check if it's time to set a new target
        if (changeTimer >= changeInterval)
        {
            SetNewTarget();
            changeTimer = 0f;
            lerpTime = 0f;
        }

        // Lerp the intensity
        float t = Mathf.Clamp01(lerpTime / lerpDuration);
        t = Mathf.Pow(t, lerpSmoothing); // Apply smoothing curve

        currentIntensity = Mathf.Lerp(currentIntensity, targetIntensity, t);
        chromaticAberration.intensity.value = currentIntensity;
    }

    /// <summary>
    /// Sets a new target intensity for lerping
    /// </summary>
    private void SetNewTarget()
    {
        // Generate a new target intensity
        targetIntensity = Random.Range(minIntensity, maxIntensity);
    }

    /// <summary>
    /// Enables randomization of Chromatic Aberration
    /// </summary>
    public void EnableRandomization()
    {
        if (chromaticAberration == null) return;

        isRandomizing = true;
        SetNewTarget();
        lerpTime = 0f;
        changeTimer = 0f;
    }

    /// <summary>
    /// Disables randomization and sets Chromatic Aberration to zero
    /// </summary>
    public void DisableRandomization()
    {
        if (chromaticAberration == null) return;

        isRandomizing = false;
        currentIntensity = 0f;
        targetIntensity = 0f;
        lerpTime = 0f;
        changeTimer = 0f;
        chromaticAberration.intensity.value = 0f;
    }

    /// <summary>
    /// Manually set a specific target intensity
    /// </summary>
    public void SetTargetIntensity(float intensity)
    {
        if (!isRandomizing) return;

        targetIntensity = Mathf.Clamp(intensity, minIntensity, maxIntensity);
        lerpTime = 0f;
    }

    /// <summary>
    /// Toggle the randomization state
    /// </summary>
    public void ToggleRandomization()
    {
        if (isRandomizing)
        {
            DisableRandomization();
        }
        else
        {
            EnableRandomization();
        }
    }
}