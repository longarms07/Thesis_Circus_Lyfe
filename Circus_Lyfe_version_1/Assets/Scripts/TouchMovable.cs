using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchMovable : MonoBehaviour, IDragListener, ITapListener
{
    [Tooltip("The touch ID this object should follow")]
    public int touchToFollow;
    [Tooltip("The speed at which this object moves while walking.")]
    public float walkSpeed;
    [Tooltip("The speed at which this object moves while running.")]
    public float runSpeed;
    [Tooltip("The threshold to determine whether this object is walking (<) or running (>=).")]
    public float runThreshold;

    private TouchInputManager touchInputManager;
    private Rigidbody2D da_Rigidbody;
    private bool drag = false;
    private Vector3 targetPosition;
    
    void Start()
    {
        touchInputManager = TouchInputManager.getInstance();
        if (touchInputManager == null)
        {
            Debug.LogError("Touch Movable Error: There is no TouchInputManager!");
            Destroy(this.gameObject);
        }
        else
        {
            da_Rigidbody = this.gameObject.GetComponent<Rigidbody2D>();
            touchInputManager.SubscribeTapListener(this, touchToFollow);
            touchInputManager.SubscribeDragListener(this, touchToFollow);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (drag) MoveTowards(targetPosition);
    }


    public void DragStarted(Vector3[] dragPositions)
    {
        drag = true;
        targetPosition = dragPositions[1];
        MoveTowards(targetPosition);
    }

    public void DragPoisitonChanged(Vector3[] dragPositions)
    {
        targetPosition = dragPositions[1];
        MoveTowards(targetPosition);
    }

    public void DragEnded(Vector3[] dragPositions, List<Vector2> deltaPositions)
    {
        drag = false;
        TapDetected(dragPositions[1]);
    }

    public void TapDetected(Vector3 position)
    {
        //Stubbed
    }

    public void MoveTowards(Vector3 moveTo)
    {
        Vector3 moveTowards;
        //Check if we should walk (in loop) or run (out of loop)
        Debug.Log("X difference = " + (moveTo.x - this.gameObject.transform.localPosition.x)
            + " , Y difference - " + (moveTo.y - this.gameObject.transform.localPosition.y));
        if (Math.Abs(moveTo.x - this.gameObject.transform.localPosition.x) < runThreshold
            && Math.Abs(moveTo.y - this.gameObject.transform.localPosition.y) < runThreshold)
        {
            moveTowards = Vector3.MoveTowards(this.gameObject.transform.localPosition, moveTo, Time.fixedDeltaTime * walkSpeed);
        }
        else
        {
            moveTowards = Vector3.MoveTowards(this.gameObject.transform.localPosition, moveTo, Time.fixedDeltaTime * runSpeed);
        }

        da_Rigidbody.MovePosition(moveTowards);
    }

}
