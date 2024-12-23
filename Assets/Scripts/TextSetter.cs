using UnityEngine;
using TMPro;

public class TextSetter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textObj;

    public void SetText(string newText) {
        textObj.text = newText;
    }
}
