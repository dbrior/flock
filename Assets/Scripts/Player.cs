using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool isPlayer2 = false;
    public float moveSpeed;
    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();   
    }

    // Update is called once per frame
    void Update()
    {
        if(!isPlayer2) {
            if (Input.GetKey(KeyCode.D))
            {
                rb.AddForce(Vector3.right * moveSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.A))
            {
                rb.AddForce(Vector3.right * -moveSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.W))
            {
                rb.AddForce(Vector3.up * moveSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.S))
            {
                rb.AddForce(Vector3.up * -moveSpeed * Time.deltaTime);
            }
        } else {
            if (Input.GetKey(KeyCode.L))
            {
                rb.AddForce(Vector3.right * moveSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.J))
            {
                rb.AddForce(Vector3.right * -moveSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.I))
            {
                rb.AddForce(Vector3.up * moveSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.K))
            {
                rb.AddForce(Vector3.up * -moveSpeed * Time.deltaTime);
            }
        }
    }
}
