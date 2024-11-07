using UnityEngine;
using TMPro;

public class InteractionHints : MonoBehaviour
{
    [SerializeField] private GameObject hintContainer;
    [SerializeField] private TextMeshProUGUI hintText;

    public void ShowHint(string text) {
        hintText.text = text;
        hintContainer.SetActive(true);
    }

    public void HideHint() {
        hintContainer.SetActive(false);
    }
}
