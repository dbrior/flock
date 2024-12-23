using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollower : MonoBehaviour
{
    private CharacterMover characterMover;

    void Awake() {
        characterMover = GetComponent<CharacterMover>();
        characterMover.SetWanderAnchor(PlayerManager.Instance.currentPlayer.transform);
    }
}
