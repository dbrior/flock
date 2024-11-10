using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoCropSpawn : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.TryGetComponent<Player>(out Player player)) {
            player.allowedPlanting = false;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.TryGetComponent<Player>(out Player player)) {
            player.allowedPlanting = true;
        }
    }
}
