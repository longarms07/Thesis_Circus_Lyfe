using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    [Tooltip("Game object for the camera to follow")]
    public GameObject toFollow;
    [Tooltip("Threshold of movement from camera center the game object is allowed before camera movement")]
    public float moveThreshold;
    [Tooltip("The speed of the camera's movement")]
    public float moveSpeed;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float xDistance = this.gameObject.transform.localPosition.x - toFollow.transform.localPosition.x;
        float yDistance = this.gameObject.transform.localPosition.y - toFollow.transform.localPosition.y;

        float newX;
        float newY;

        if (Math.Abs(xDistance) >= moveThreshold)
        {
            if (xDistance < 0)
                newX = this.gameObject.transform.localPosition.x + Math.Max(xDistance, -moveSpeed);
            else
                newX = this.gameObject.transform.localPosition.x + Math.Min(xDistance, moveSpeed);
        }
        else
            newX = this.gameObject.transform.localPosition.x;
        
        if (Math.Abs(yDistance) >= moveThreshold)
        {
            if (yDistance < 0)
                newY = this.gameObject.transform.localPosition.y + Math.Max(yDistance, -moveSpeed);
            else
                newY = this.gameObject.transform.localPosition.y + Math.Min(yDistance, moveSpeed);
        }
        else
            newY = this.gameObject.transform.localPosition.y;
        this.gameObject.transform.localPosition = new Vector3(newX, newY, this.gameObject.transform.localPosition.z);

    }
    
}
