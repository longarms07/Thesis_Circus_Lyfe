using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public GameObject toFollow;
    public Vector2 constraints;
    public float maxMove;
    private float z;


    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        z = transform.position.z;
        offset = transform.position - toFollow.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float xDis = Mathf.Abs(transform.position.x - toFollow.transform.position.x);
        float yDis = Mathf.Abs(transform.position.y - toFollow.transform.position.y);
        if (xDis >= constraints.x || yDis >= constraints.y) { 
            Vector3 newPosition = Vector3.MoveTowards(transform.position, toFollow.transform.position+offset, maxMove*Time.deltaTime);
            newPosition.z = z;
            transform.position = newPosition;
         }
        //transform.position = toFollow.transform.position + offset;
    }
}
