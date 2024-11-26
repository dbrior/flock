using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoCropSpawn : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.TryGetComponent<ToolBelt>(out ToolBelt toolBelt)) {
            toolBelt.allowedPlanting = false;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.TryGetComponent<ToolBelt>(out ToolBelt toolBelt)) {
            toolBelt.allowedPlanting = true;
        }
    }
}
