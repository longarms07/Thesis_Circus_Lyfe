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
    [Tooltip("The gameobject with the collision box to use for detecting valid tap locations.")]
    public GameObject tapCollider;



    private bool canMove;
    private TouchInputManager touchInputManager;
    private Rigidbody2D da_Rigidbody;
    private bool drag = false;
    private bool tap = true;
    private Vector3 targetPosition;
    private TouchCursor touchCursor;
    private BoxCollider2D tapColliderRB;
    
    
    
    void Start()
    {
        touchInputManager = TouchInputManager.getInstance();
        if (touchInputManager == null)
        {
            Debug.LogError("Touch Movable Error: There is no TouchInputManager!");
            Destroy(this.gameObject);
        }
        else if(tapCollider == null || (tapColliderRB = tapCollider.GetComponent<BoxCollider2D>()) == null){
            Debug.LogError("Touch Movable Error: There is no Tap Box Collider!");
            Destroy(this.gameObject);
        }
        else
        {
            da_Rigidbody = this.gameObject.GetComponent<Rigidbody2D>();
            ToggleMovement(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            if (drag) MoveTowards(targetPosition);
            if (tap)
            {
                MoveTowards(targetPosition);
                if (this.gameObject.transform.localPosition == targetPosition) TapEnded();
            }
        }
    }


    public void DragStarted(Vector3[] dragPositions)
    {
        if (canMove)
        {
            if (tap) TapEnded();
            drag = true;
            targetPosition = dragPositions[1];
            MoveTowards(targetPosition);
        }
    }

    public void DragPoisitonChanged(Vector3[] dragPositions)
    {
        if (canMove)
        {
            targetPosition = dragPositions[1];
            MoveTowards(targetPosition);
        }
    }

    public void DragEnded(Vector3[] dragPositions, List<Vector2> deltaPositions)
    {
        drag = false;
        if(canMove) TapDetected(dragPositions[1]);
    }

    public void TouchStarted(Vector3 startPosition)
    {
        if (tap) TapEnded();
    }

    public void TapDetected(Vector3 position)
    {
        if (canMove)
        {
            if (tap) TapEnded();
            if (CheckValid(position))
            {
                targetPosition = position;
                tap = true;
                MoveTowards(targetPosition);
                touchCursor = Instantiate(touchInputManager.touchCursorPrefab).GetComponent<TouchCursor>();
                touchCursor.changePosition(targetPosition);
                //Needs pathfinding
            }
        }
    }

    public void TapEnded()
    {
        tap = false;
        if(touchCursor!=null) touchCursor.endTouch();

    }

    public void MoveTowards(Vector3 moveTo)
    {
        if (canMove)
        {
            Vector3 moveTowards;
            //Check if we should walk (in loop) or run (out of loop)
            /*Debug.Log("X difference = " + (moveTo.x - this.gameObject.transform.localPosition.x)
                + " , Y difference - " + (moveTo.y - this.gameObject.transform.localPosition.y));*/
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

    private Boolean CheckValid(Vector3 position)
    {
        if (canMove) { 
            RaycastHit2D hit = Physics2D.Raycast(position, Vector2.down);
            if (hit.collider != null && hit.collider == tapColliderRB)
            {
                return true;
            }
        }
        return false;
    
    }

    public void ToggleMovement(bool toggle)
    {
        if (toggle)
        {
            canMove = true;
            touchInputManager.SubscribeTapListener(this, touchToFollow);
            touchInputManager.SubscribeDragListener(this, touchToFollow);
        }
        else
        {
            canMove = false;
            touchInputManager.UnsubscribeTapListener(this, touchToFollow);
            touchInputManager.UnsubscribeDragListener(this, touchToFollow);
            if (tap) TapEnded();
            if (drag) DragEnded(null, null);
        }

    }

}
