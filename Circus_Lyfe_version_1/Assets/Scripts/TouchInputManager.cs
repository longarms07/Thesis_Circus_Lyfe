using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TouchInputManager : MonoBehaviour
{
    [Tooltip("The prefab to use for creating touch cursors. Must have script 'TouchCursor'")]
    public GameObject touchCursorPrefab;

    [Tooltip("The number of fingers/touches the manager will track.")]
    public int fingersSupported;

    [Tooltip("The maximum length of a tap")]
    public float maxTapTime;
    [Tooltip("The maximum length of a swipe")]
    public float maxSwipeTime;

    [Tooltip("Whether or not Debug mode is active. Off by default.")]
    public bool debugMode;


    public TouchCursor[] touchCursors;
    private List<ITapListener>[] tapListeners;
    private List<ISwipeListener>[] swipeListeners;
    private List<IDragListener>[] dragListeners;
    private List<Vector2>[] touchDeltaPoisitons;
    private Vector3[][] touchStartEndPoints;
    private Vector2[] touchTimes;
    private TouchInputType[] inputTypes;
    
    static TouchInputManager instance;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null) { instance = this; }
        else Destroy(this);
        touchCursors = new TouchCursor[fingersSupported];
        tapListeners = new List<ITapListener>[fingersSupported];
        swipeListeners = new List<ISwipeListener>[fingersSupported];
        dragListeners = new List<IDragListener>[fingersSupported];
        touchDeltaPoisitons = new List<Vector2>[fingersSupported];
        touchStartEndPoints = new Vector3[fingersSupported][];
        touchTimes = new Vector2[fingersSupported];
        inputTypes = new TouchInputType[fingersSupported];
        for(int i = 0; i < fingersSupported; i++)
        {
            touchDeltaPoisitons[i] = new List<Vector2>();
            touchStartEndPoints[i] = new Vector3[2];
            tapListeners[i] = new List<ITapListener>();
            swipeListeners[i] = new List<ISwipeListener>();
            dragListeners[i] = new List<IDragListener>();
            inputTypes[i] = TouchInputType.TAP;
        }
    }

    // Update is called once per frame
    void Update()
    {
        string text = "";
        if (debugMode) { text = "Size of Input.touches " + Input.touchCount; }
        for (int i=0; i< Math.Min(Input.touchCount, touchCursors.Length); i++)
        {
            Touch touch = Input.touches[i];
            int touchIndex = touch.fingerId;
            if (touchIndex < fingersSupported)
            {
                if (debugMode) { Debug.Log("i = " + i + " , touchID = " + touch.fingerId); }
                Vector3 worldPosition = ScreenPointToWorldPoint(touch.position);
                if (touch.phase == TouchPhase.Began)
                {
                    if (touchCursors[touchIndex] == null)
                    {
                        touchCursors[touchIndex] = Instantiate(touchCursorPrefab).GetComponent<TouchCursor>();
                        touchCursors[touchIndex].changePosition(worldPosition);
                        touchDeltaPoisitons[touchIndex] = new List<Vector2>();
                        touchTimes[touchIndex] = new Vector2(Time.time, Time.time);
                        inputTypes[touchIndex] = TouchInputType.TAP;
                        touchStartEndPoints[touchIndex][0] = worldPosition;
                    }
                    else if (debugMode) { Debug.Log("For some reason touchCursor " + touchIndex + " was unequal to null."); }
                    foreach (IDragListener drag in dragListeners[touchIndex])
                    {
                        drag.TouchStarted(worldPosition);
                    }
                    if (debugMode) { Debug.Log("Touch Event " + i + " Began"); }
                }
                else if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
                {
                    if (touchCursors[touchIndex] != null)
                    {
                        touchCursors[touchIndex].changePosition(worldPosition);
                    }
                    if (touch.phase == TouchPhase.Moved)
                    {
                        if (touchCursors[touchIndex] != null)
                        {
                            touchDeltaPoisitons[touchIndex].Add(touch.deltaPosition);
                            if (inputTypes[touchIndex] == TouchInputType.TAP) { inputTypes[touchIndex] = TouchInputType.SWIPE; }
                            if (inputTypes[touchIndex] == TouchInputType.DRAG)
                            {
                                foreach (IDragListener listener in dragListeners[touchIndex])
                                {
                                    listener.DragPoisitonChanged(touchStartEndPoints[touchIndex]);
                                }
                            }
                        }
                        else if (debugMode) { Debug.Log("Touch is null @ " + i); }
                    }
                    touchStartEndPoints[touchIndex][1] = worldPosition;
                    touchTimes[touchIndex].y = Time.time;
                    if (inputTypes[touchIndex]!=TouchInputType.DRAG)
                    {
                        inputTypes[touchIndex] = Overtime(touchTimes[touchIndex], inputTypes[touchIndex]);
                        if (inputTypes[touchIndex] == TouchInputType.DRAG)
                        {
                            foreach (IDragListener listener in dragListeners[touchIndex])
                            {
                                listener.DragStarted(touchStartEndPoints[touchIndex]);
                            }
                        }
                    }
                    
                        if (debugMode)
                    {
                        text += "\n Touch " + touchIndex + "ended at Position " + touch.position + "at deltaPosition " + touch.deltaPosition;
                        if (inputTypes[touchIndex] == TouchInputType.SWIPE) { text += " , Type = Swipe"; }
                        else if (inputTypes[touchIndex] == TouchInputType.TAP) { text += " , Type = Tap"; }
                        else { text += " , Type = Drag"; }
                    }
                }
                else
                {
                    //Notify listeners
                    if (inputTypes[touchIndex] == TouchInputType.SWIPE)
                    {
                        foreach (ISwipeListener listener in swipeListeners[touchIndex])
                        {
                            listener.SwipeDetected(touchStartEndPoints[touchIndex]);
                        }
                    }
                    else if(inputTypes[touchIndex] == TouchInputType.TAP)
                    {
                        foreach (ITapListener listener in tapListeners[touchIndex])
                        {
                            listener.TapDetected(touchStartEndPoints[touchIndex][0]);
                        }
                    }
                    else
                    {
                        foreach (IDragListener listener in dragListeners[touchIndex])
                        {
                            listener.DragEnded(touchStartEndPoints[touchIndex], touchDeltaPoisitons[touchIndex]);
                        }
                    }

                    if (touchCursors[touchIndex] != null)
                    {
                        touchCursors[touchIndex].endTouch();
                        touchCursors[touchIndex] = null;
                        if (debugMode) { Debug.Log("Touch Event " + touchIndex + " Ended"); }
                    }
                    if (debugMode)
                    { text += "\n Touch " + touchIndex + " Ended."; }
                }
            }
        }
       
    }
    



    public static TouchInputManager getInstance()
    {
        return instance;
    }

    
    public bool SubscribeTapListener(ITapListener tap, int touchToMonitor)
    {
        if (!ListenerIndexValid(touchToMonitor))
        {
            return false;
        }
        tapListeners[touchToMonitor].Add(tap);
        return true;
    }

    public bool SubscribeSwipeListener(ISwipeListener swipe, int touchToMonitor)
    {
        if (!ListenerIndexValid(touchToMonitor))
        {
            return false;
        }
        swipeListeners[touchToMonitor].Add(swipe);
        return true;
    }

    public bool SubscribeDragListener(IDragListener drag, int touchToMonitor)
    {
        if (!ListenerIndexValid(touchToMonitor))
        {
            return false;
        }
        dragListeners[touchToMonitor].Add(drag);
        return true;
    }



    public bool UnsubscribeTapListener(ITapListener tap, int touchToMonitor)
    {
        if (!ListenerIndexValid(touchToMonitor))
        {
            return false;
        }
        return tapListeners[touchToMonitor].Remove(tap);
    }

    public bool UnsubscribeSwipeListener(ISwipeListener swipe, int touchToMonitor)
    {
        if (!ListenerIndexValid(touchToMonitor))
        {
            return false;
        }
        return swipeListeners[touchToMonitor].Remove(swipe);
    }

    public bool UnsubscribeDragListener(IDragListener drag, int touchToMonitor)
    {
        if (!ListenerIndexValid(touchToMonitor))
        {
            return false;
        }
        return dragListeners[touchToMonitor].Remove(drag);
    }

    private bool ListenerIndexValid(int touchToMonitor)
    {
        if (touchToMonitor<0 || touchToMonitor>=fingersSupported)
        {
            return false;
        }
        return true;
    }

    private Vector3 ScreenPointToWorldPoint(Vector3 touch)
    {
        Vector3 screenPoint = Camera.main.ScreenToWorldPoint(touch);
        return new Vector3(screenPoint.x, screenPoint.y, 0);
    }

    private TouchInputType Overtime(Vector2 times, TouchInputType type)
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


}

public enum TouchInputType
{
    TAP,
    SWIPE,
    DRAG
}
