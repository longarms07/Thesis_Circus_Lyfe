using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ISwipeListener : MonoBehaviour
{

    [Tooltip("The amount of distance between cardinal directions for swipe detection on the x axis")]
    public float xThreshold;
    [Tooltip("The amount of distance between cardinal directions for swipe detection on the y axis")]
    public float yThreshold;

    public abstract void SwipeDetected(Vector3[] swipePositions);

    public SwipeDirection FindDirection(Vector3[] swipePositions)
    {
        if (swipePositions.Length != 2 || swipePositions[0] == null || swipePositions[1] == null) { return SwipeDirection.NONE; }
        SwipeDirection swipeDirection = SwipeDirection.NONE;
        float changeInY = swipePositions[1].x - swipePositions[0].x;
        float changeInX = swipePositions[1].y - swipePositions[0].y;
        //Debug.Log("Change in x = " + changeInX + " , Change in y = " + changeInY);
        if(changeInX > xThreshold)
        {
            //North Family
            if(changeInY > yThreshold)
            {
                swipeDirection = SwipeDirection.NorthEast;
            }
            else if (changeInY < -yThreshold)
            {
                swipeDirection = SwipeDirection.NorthWest;
            }
            else
            {
                swipeDirection = SwipeDirection.North;
            }
        }else if(changeInX < -xThreshold)
        {
            //South Family
            if (changeInY > yThreshold)
            {
                swipeDirection = SwipeDirection.SouthEast;
            }
            else if (changeInY < -yThreshold)
            {
                swipeDirection = SwipeDirection.SouthWest;
            }
            else
            {
                swipeDirection = SwipeDirection.South;
            }
        }
        else
        {
            //East threshold
            if (changeInY >= yThreshold)
            {
                swipeDirection = SwipeDirection.East;
            }
            //West threshold
            if (changeInY < -yThreshold)
            {
                swipeDirection = SwipeDirection.West;
            }
        }

        return swipeDirection;
    }
}
