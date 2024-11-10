using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSelectorArrow : MonoBehaviour
{
    public RectTransform arrowImage; // Ensure this is the RectTransform of the arrow image

    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            // Check if the current selected GameObject is a selectable UI element
            Selectable selectedUIElement = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();

            if (selectedUIElement != null)
            {
                // Get the RectTransform of the selected UI element
                RectTransform selectedTransform = selectedUIElement.GetComponent<RectTransform>();

                if (selectedTransform != null)
                {
                    // Set the arrow's parent to the selected element and reset its anchored position
                    arrowImage.SetParent(selectedTransform, false);

                    // Anchor the arrow to the left center
                    arrowImage.anchorMin = new Vector2(0, 0.5f);
                    arrowImage.anchorMax = new Vector2(0, 0.5f);
                    arrowImage.pivot = new Vector2(0, 0.5f);

                    // Set posX to -arrowSelector.width - 10 and posY to 0
                    float arrowOffsetX = -arrowImage.rect.width;
                    arrowImage.anchoredPosition = new Vector2(arrowOffsetX, 0);

                    // Make sure the arrow is visible
                    arrowImage.gameObject.SetActive(true);
                }
            }
            else
            {
                // Hide the arrow if the selected object is not a selectable UI element
                arrowImage.gameObject.SetActive(false);
            }
        }
        else
        {
            // Hide the arrow if nothing is selected
            arrowImage.gameObject.SetActive(false);
        }
    }
}
