using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatchPointTrigger : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.TryGetComponent<Player>(out Player player)) {
            HunterManager.Instance.SendToWatchpoint();
        }
    }

    void OnTriggerExit2D(Collider2D col) {
        if (col.gameObject.TryGetComponent<Player>(out Player player)) {
            HunterManager.Instance.SendToPlayer();
        }
    }
}
