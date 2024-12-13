using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class SelectionContainer : MonoBehaviour
{
    [SerializeField] private string selectedValue;

    [SerializeField] private Color selectedColor;
    [SerializeField] private Color deselectedColor;
    [SerializeField] private UnityEvent<string> onSelection;

    private SelectionItem[] children;
    private int selectedIdx;

    void Awake() {
        int childCount = transform.childCount;
        children = new SelectionItem[childCount];

        for (int i=0; i<childCount; i++) {
            SelectionItem child = transform.GetChild(i).GetComponent<SelectionItem>();
            children[i] = child;
        }
    }

    void Start() {
        TrySelectItem(children[0]);
    }

    public void TrySelectItem(SelectionItem targetItem) {
        for (int i=0; i<children.Length; i++) {
            SelectionItem child = children[i];

            if (child == targetItem) {
                child.Select(selectedColor);
                selectedValue = child.gameObject.name;
                onSelection?.Invoke(selectedValue);
            } else {
                child.Deselect(deselectedColor);
            }
        }
    }
}
