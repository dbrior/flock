using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenExit : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("TameSheep"))
        {
            SheepManager.Instance.ReleaseSheep(col.gameObject);
        }
    }
}
