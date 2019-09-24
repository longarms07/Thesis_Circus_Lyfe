using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ISwipeListener : MonoBehaviour
{

    public float xThreshold;
    public float yThreshold;

    public abstract void SwipeDetected(Vector3[] swipePositions, List<Vector2> deltaPositions);

    public SwipeDirection FindDirection(Vector3[] swipePositions)
    {
        if (swipePositions.Length != 2 || swipePositions[0] == null || swipePositions[1] == null) { return SwipeDirection.NONE; }
        SwipeDirection swipeDirection = SwipeDirection.NONE;
        float changeInX = swipePositions[1].x - swipePositions[0].x;
        float changeInY = swipePositions[1].y - swipePositions[0].y;
        if(changeInX > xThreshold)
        {
            //North Family
            if(changeInY > yThreshold)
            {
                swipeDirection = SwipeDirection.NorthEast;
            }
            if (changeInY < -yThreshold)
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
            if (changeInY < -yThreshold)
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
            if (changeInY > yThreshold)
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
