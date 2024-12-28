using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefensePoint : MonoBehaviour
{
    [SerializeField] private GameObject guardPrefab;

    public void SpawnGuard() {
        GameObject guardObj = Instantiate(guardPrefab, transform.position, transform.rotation);
        guardObj.GetComponent<CharacterMover>().SetWanderAnchor(transform);
    }
}
