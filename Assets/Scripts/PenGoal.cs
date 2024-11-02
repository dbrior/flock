using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenGoal : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("WildSheep"))
        {
            col.gameObject.layer = LayerMask.NameToLayer("TameSheep");
        }
    }

}
