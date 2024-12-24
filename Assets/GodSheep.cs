using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodSheep : MonoBehaviour
{
    [SerializeField] private GameObject firstDialogueBox;
    [SerializeField] private GameObject secondDialogueBox;

    public void ActivateFirstDialogue() {
        firstDialogueBox.SetActive(true);
    }

    public void ActivateSecondDialogue() {
        secondDialogueBox.SetActive(true);
    }
    
}
