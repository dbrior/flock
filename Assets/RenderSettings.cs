using UnityEngine.Rendering;
using UnityEngine;

public class RenderSettings : MonoBehaviour
{
    void Start()
    {
        GraphicsSettings.transparencySortMode = TransparencySortMode.CustomAxis;
        GraphicsSettings.transparencySortAxis = new Vector3(0, 1, 0);
    }
}
