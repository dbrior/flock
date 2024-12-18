using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatchPointTrigger : MonoBehaviour
{
    [SerializeField] private WorkerBuilding building;
    [SerializeField] private Transform triggeredAnchor;
    [SerializeField] private Transform untriggeredAnchor;

    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.TryGetComponent<Player>(out Player player)) {
            building.SetWanderAnchor(triggeredAnchor);
        }
    }

    void OnTriggerExit2D(Collider2D col) {
        if (col.gameObject.TryGetComponent<Player>(out Player player)) {
            building.SetWanderAnchor(untriggeredAnchor);
        }
    }
}
