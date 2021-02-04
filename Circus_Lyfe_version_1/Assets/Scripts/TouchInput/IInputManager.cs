using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IInputManager : MonoBehaviour
{

    [Tooltip("The prefab to use for creating touch cursors. Must have script 'TouchCursor'")]
    public GameObject touchCursorPrefab;

    [Tooltip("The maximum length of a tap")]
    public float maxTapTime;
    [Tooltip("The maximum length of a swipe")]
    public float maxSwipeTime;

    [Tooltip("Whether or not Debug mode is active. Off by default.")]
    public bool debugMode;
    [Tooltip("Display GUI to be used for devugging")]
    public GameObject displayText;

    protected static IInputManager instance;


    public static IInputManager getInstance()
    {
        return instance;
    }


    public abstract bool SubscribeTapListener(ITapListener tap, int touchToMonitor = 0);

    public abstract bool SubscribeSwipeListener(ISwipeListener swipe, int touchToMonitor = 0);

    public abstract bool SubscribeDragListener(IDragListener drag, int touchToMonitor = 0);

    public abstract bool UnsubscribeTapListener(ITapListener tap, int touchToMonitor = 0);

    public abstract bool UnsubscribeSwipeListener(ISwipeListener swipe, int touchToMonitor = 0);

    public abstract bool UnsubscribeDragListener(IDragListener drag, int touchToMonitor = 0);

    protected Vector3 ScreenPointToWorldPoint(Vector3 touch)
    {
        Vector3 screenPoint = Camera.main.ScreenToWorldPoint(touch);
        return new Vector3(screenPoint.x, screenPoint.y, 0);
    }

    protected TouchInputType Overtime(Vector2 times, TouchInputType type)
    {
        if (type == TouchInputType.TAP && (times.y - times.x) > maxTapTime)
        {
            return TouchInputType.DRAG;
        }
        else if (type == TouchInputType.SWIPE && (times.y - times.x) > maxSwipeTime)
        {
            return TouchInputType.DRAG;
        }
        return type;
    }

    public enum TouchInputType
    {
        TAP,
        SWIPE,
        DRAG,
        NONE
    }
}
