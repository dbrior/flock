using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AutoScroll : MonoBehaviour
{
    public ScrollRect scrollRect;

    private void Update()
    {
        // Get the current selected GameObject
        GameObject selected = EventSystem.current.currentSelectedGameObject;

        // Check if selected GameObject is part of the Scroll Rect's content
        if (selected != null && selected.transform.IsChildOf(scrollRect.content))
        {
            RectTransform selectedRect = selected.GetComponent<RectTransform>();
            RectTransform contentRect = scrollRect.content;

            // Calculate the position of the selected item relative to the content
            float viewportHeight = scrollRect.viewport.rect.height;
            float contentHeight = contentRect.rect.height;
            float itemPos = contentRect.rect.height - selectedRect.anchoredPosition.y;

            // Set new scroll position so that the item is fully visible
            float newNormalizedPosition = Mathf.Clamp01((itemPos - viewportHeight / 2) / (contentHeight - viewportHeight));
            scrollRect.verticalNormalizedPosition = newNormalizedPosition;
        }
    }
}