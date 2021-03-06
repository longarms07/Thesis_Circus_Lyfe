﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    [Tooltip("The float used to determine the radius by which this object will search for nearby objects")]
    public float overlapRadius;
    [Tooltip("The layers of interactables to look for")]
    public String[] layers;
    public float followDist;

    private bool canMove;
    private TouchInputManager touchInputManager;
    private Rigidbody2D da_Rigidbody;
    private bool drag = false;
    private bool tap = false;
    private bool targetingInteractable = false;
    private IInteractable targetInteractable;
    private Collider2D targetCollider;
    public Vector3 targetPosition;
    private TouchCursor touchCursor;
    private BoxCollider2D tapColliderRB;
    private Collider2D[] nearbyInteractables;
    private bool isFollowed;
    private Rigidbody2D followedBy;
    
    
    
    void Start()
    {
        followedBy = null;
        isFollowed = false;
        nearbyInteractables = new Collider2D[0];
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
            Vector3 currentPosition = this.gameObject.transform.position;
            if (drag) MoveTowards(targetPosition);
            if (tap)
            {
                MoveTowards(targetPosition);
                if (this.gameObject.transform.position == targetPosition) TapEnded();
            }
            CheckTarget();
            if (isFollowed)
            {
                if (Mathf.Abs(transform.position.x - followedBy.transform.position.x) > followDist
                    || Mathf.Abs(transform.position.y - followedBy.transform.position.y) > followDist)
                {
                    float speed = walkSpeed;
                    if (Math.Abs(this.transform.position.x - followedBy.gameObject.transform.position.x) < runThreshold
                    && Math.Abs(this.transform.position.y - followedBy.gameObject.transform.position.y) < runThreshold)
                    {
                        speed = runSpeed;
                    }
                    followedBy.MovePosition(Vector3.MoveTowards(followedBy.gameObject.transform.position, currentPosition, Time.fixedDeltaTime * speed));
                }
                followedBy.velocity = Vector2.zero;
                followedBy.angularDrag = 0;
                followedBy.angularVelocity = 0;

            }
            da_Rigidbody.velocity = Vector2.zero;
            da_Rigidbody.angularDrag = 0;
            da_Rigidbody.angularVelocity = 0;
        }
        NotfiyInteractablesMovedAway(CheckNearbyInteractables());
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
                OnTap(position);
            }
        }
    }

    //Tap detected without validity checking, added interactable targeting
    public void OnTap(Vector3 position)
    {
        if (canMove)
        {
            if (tap) TapEnded();
            targetPosition = position;
            tap = true;
            MoveTowards(targetPosition);
            touchCursor = Instantiate(touchInputManager.touchCursorPrefab).GetComponent<TouchCursor>();
            touchCursor.changePosition(targetPosition);
            
            //Needs pathfinding
        }
    }

    public void TapEnded()
    {
        tap = false;
        targetInteractable = null;
        targetingInteractable = false;
        targetCollider = null;
        if (touchCursor != null)
        {
            touchCursor.endTouch();
            touchCursor = null;
        }
        da_Rigidbody.velocity = Vector2.zero;

    }

    public void MoveTowards(Vector3 moveTo)
    {
        if (canMove)
        {

            Vector3 moveTowards;
            //Check if we should walk (in loop) or run (out of loop)
            /*Debug.Log("X difference = " + (moveTo.x - this.gameObject.transform.position.x)
                + " , Y difference - " + (moveTo.y - this.gameObject.transform.position.y));*/
            if (Math.Abs(moveTo.x - this.gameObject.transform.position.x) < runThreshold
                && Math.Abs(moveTo.y - this.gameObject.transform.position.y) < runThreshold)
            {
                moveTowards = Vector3.MoveTowards(this.gameObject.transform.position, moveTo, Time.fixedDeltaTime * walkSpeed);
            }
            else
            {
                moveTowards = Vector3.MoveTowards(this.gameObject.transform.position, moveTo, Time.fixedDeltaTime * runSpeed);
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

    public Collider2D[] CheckNearbyInteractables()
    {

        Collider2D[] nearbyHits = Physics2D.OverlapCircleAll(this.gameObject.transform.position, overlapRadius,
                                                             LayerMask.GetMask(layers));
        foreach (Collider2D hit in nearbyHits)
        {
            IInteractable tempInteractable = GameManager.getInstance().GetInteractable(hit.transform);
            if(tempInteractable!=null && !nearbyInteractables.Contains(hit))
                tempInteractable.InRange(true);
        }
        return nearbyHits;
    }

    public void NotfiyInteractablesMovedAway(Collider2D[] tempInteractables)
    {
        if (nearbyInteractables != null)
        {
            foreach (Collider2D collider in nearbyInteractables)
            {
                if (!tempInteractables.Contains(collider))
                {
                    IInteractable movedFrom = GameManager.getInstance().GetInteractable(collider.transform);
                    if (movedFrom != null) movedFrom.InRange(false);
                }
            }
        }
        nearbyInteractables = tempInteractables;
    }


    public void ToggleMovement(bool toggle)
    {
        if (toggle)
        {
            canMove = true;
            //touchInputManager.SubscribeTapListener(this, touchToFollow);
            touchInputManager.SubscribeDragListener(this, touchToFollow);
        }
        else
        {
            canMove = false;
            //touchInputManager.UnsubscribeTapListener(this, touchToFollow);
            touchInputManager.UnsubscribeDragListener(this, touchToFollow);
            if (tap) TapEnded();
            if (drag) DragEnded(null, null);
        }

    }

    public void ToggleMovement()
    {
        ToggleMovement(!canMove);

    }

    public Collider2D[] GetNearbyInteractables()
    {
        return nearbyInteractables;
    }

    public void TargetInteractable(Collider2D collider, IInteractable interactable)
    {
        if (canMove)
        {
            if (nearbyInteractables.Contains(collider)) interactable.OnInteraction();
            else
            {
                targetInteractable = interactable;
                targetingInteractable = true;
                targetCollider = collider;
                OnTap(targetCollider.transform.position);
            }
        }
    }

    private void CheckTarget()
    {
        if (targetingInteractable && nearbyInteractables.Contains(targetCollider))
        {
            targetInteractable.OnInteraction();
            TapEnded();
        }
    }

    public void NewFollower(Rigidbody2D follower)
    {
        followedBy = follower;
        followedBy.isKinematic = false;
        isFollowed = true;
    }

    public void StopFollower()
    {
        followedBy.isKinematic = true;
        followedBy = null;
        isFollowed = false;
    }

    public Rigidbody2D GetFollower()
    {
        return followedBy;
    }

    public bool GetIsFollowed()
    {
        return isFollowed;
    }
}
