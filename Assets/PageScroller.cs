using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;

public class PageScroller : MonoBehaviour
{
    [SerializeField] private List<GameObject> pages;
    [SerializeField] private TextMeshProUGUI pageCounter;
    private int currentPage = 0;

    private void SetActivePage(int pageNum) {
        for (int i=0; i<pages.Count; i++) {
            pages[i].SetActive(i == pageNum);
        }
        currentPage = pageNum;
        EventSystem.current.SetSelectedGameObject(pages[currentPage].transform.GetChild(0).gameObject);
        pageCounter.text = (currentPage+1).ToString() + "    /    " + pages.Count.ToString();
    }

    public void NextPage() {
        int newPage = Mathf.Clamp(currentPage+1, 0, pages.Count-1);
        if (newPage != currentPage) {
            SetActivePage(newPage);
        }
    }

    public void PreviousPage() {
        int newPage = Mathf.Clamp(currentPage-1, 0, pages.Count-1);
        if (newPage != currentPage) {
            SetActivePage(newPage);
        }
    }

    public void OnNavigate(InputValue inputValue) {
        Vector2 nav = inputValue.Get<Vector2>();
        if (nav.x > 0) {
            NextPage();
        } else if (nav.x < 0) {
            PreviousPage();
        }
    }
}
