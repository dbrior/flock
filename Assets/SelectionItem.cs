using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectionItem : MonoBehaviour
{
    private Button button;
    private SelectionContainer container;
    private TextMeshProUGUI textUI;

    void Awake() {
        button = GetComponent<Button>();
        textUI = GetComponent<TextMeshProUGUI>();
        container = GetComponentInParent<SelectionContainer>();
    }

    void Start() {
        button.onClick.AddListener(() => container.TrySelectItem(this));
    }

    public void Select(Color color) {
        textUI.color = color;
    }

    public void Deselect(Color color) {
        textUI.color = color;
    }
}
