using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddForceTest : MonoBehaviour
{

    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

        rb.AddForce(100 * Vector3.left);
    }
}
