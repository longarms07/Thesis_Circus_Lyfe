using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapezeForce : MonoBehaviour
{
    [Tooltip("The amount of updates to take before adding force in other direction")]
    public float rotateInterval;
    [Tooltip("The amount of force to add per direction")]
    public float forceToAdd;
    [Tooltip("True if going right, false if going left")]
    public bool inversed;

    private Rigidbody2D rb;
    private float timer;
    

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        timer++;
        if (timer >= rotateInterval)
        {
            inversed = !inversed;
            if (inversed)
                rb.AddForce(forceToAdd * Vector3.right);
            else
                rb.AddForce(forceToAdd * Vector3.left);
        }
    }
}
