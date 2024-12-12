using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TimeLight : MonoBehaviour
{
    private Light2D light2D;

    void Awake() {
        light2D = GetComponent<Light2D>();
    }

    void Start()
    {
        light2D.enabled = false;
        WaveManager.Instance.AddToggleableLight(light2D);    
    }
}
