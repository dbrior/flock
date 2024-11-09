using UnityEngine;
using Unity.Netcode;

public class ManagerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject ropePrefab;

    void Awake() {
        if (IsServer) {
            GameObject rope = Instantiate(ropePrefab);
            rope.GetComponent<NetworkObject>().Spawn();
        }
    }
}
