using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TouchInputManager : MonoBehaviour
{
    [Tooltip("The prefab to use for creating touch cursors. Must have script 'TouchCursor'")]
    public GameObject touchCursorPrefab;

    [Tooltip("The number of fingers/touches the manager will track.")]
    public int fingersSupported;

    [Tooltip("The maximum length of a tap")]
    public float maxTapTime;

    [Tooltip("Whether or not Debug mode is active. Off by default.")]
    public bool debugMode;
    [Tooltip ("Display GUI to be used for devugging")]
    public GameObject displayText;


    private TouchCursor[] touchCursors;
    private List<ITapListener>[] tapListeners;
    private List<ISwipeListener>[] swipeListeners;
    private List<Vector2>[] touchDeltaPoisitons;
    private Vector3[][] touchStartEndPoints;
    private Vector2[] touchTimes;
    private bool[] swipes;

    private TextMeshProUGUI display;
    static TouchInputManager instance;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null) { instance = this; }
        else { Destroy(this); }
        display = displayText.GetComponent<TextMeshProUGUI>();
        touchCursors = new TouchCursor[fingersSupported];
        tapListeners = new List<ITapListener>[fingersSupported];
        swipeListeners = new List<ISwipeListener>[fingersSupported];
        touchDeltaPoisitons = new List<Vector2>[fingersSupported];
        touchStartEndPoints = new Vector3[fingersSupported][];
        touchTimes = new Vector2[fingersSupported];
        swipes = new bool[fingersSupported];
        for(int i = 0; i < fingersSupported; i++)
        {
            touchStartEndPoints[i] = new Vector3[2];
            tapListeners[i] = new List<ITapListener>();
            swipeListeners[i] = new List<ISwipeListener>();
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
            Vector3 worldPosition = ScreenPointToWorldPoint(touch.position);
            if (touch.phase == TouchPhase.Began)
            {
                touchCursors[i] = Instantiate(touchCursorPrefab).GetComponent<TouchCursor>();
                touchCursors[i].changePosition(worldPosition);
                touchTimes[i] = new Vector2(Time.time, Time.time);
                swipes[i] = false;
                touchStartEndPoints[i][0] = worldPosition;
                if(debugMode) { Debug.Log("Touch Event " + i + " Began"); }
            }
            if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
            {
                
                if (touch.phase == TouchPhase.Moved)
                {
                    touchCursors[i].changePosition(worldPosition);
                    touchDeltaPoisitons[i].Add(touch.deltaPosition);
                    if (!swipes[i]) { swipes[i] = true; }
                }
                touchStartEndPoints[i][1] = worldPosition;
                touchTimes[i].y = Time.time;
                if (!swipes[i]) { swipes[i] = Overtime(touchTimes[i]); }
                
                if (debugMode)
                {
                    text += "\n Touch " + i + "ended at Position " + touch.position + "at deltaPosition " + touch.deltaPosition;
                    if (swipes[i]) { text += " , Type = Swipe"; }
                    else { text += " , Type = Tap"; }
                }
            }
            else
            {
                //Notify listeners
                if (swipes[i])
                {
                    foreach (ISwipeListener listener in swipeListeners[i])
                    {
                        listener.SwipeDetected(touchStartEndPoints[i], touchDeltaPoisitons[i]);
                    }
                }
                else
                {
                    foreach (ITapListener listener in tapListeners[i])
                    {
                        listener.TapDetected(touchStartEndPoints[i][0]);
                    }
                }

                if (touchCursors[i] != null) { 
                    touchCursors[i].endTouch();
                    touchCursors[i] = null;
                    if (debugMode) { Debug.Log("Touch Event " + i + " Ended"); }
                }
                if (debugMode)
                { text += "\n Touch " + i + " Ended."; }
            }
        }
        if (display != null && debugMode) { display.text = text; }
   
    }

    public void FixedUpdate()
    {
        //Iterate thorugh and shift everything
        for (int i = 0; i < fingersSupported; i++)
        {
            if (touchCursors[i] == null)
            {
                int j = i + 1;
                bool stop = false;
                while (j < fingersSupported && !stop)
                {
                    if (touchCursors[j] != null)
                    {
                        touchCursors[i] = touchCursors[j];
                        touchCursors[j] = null;
                    }
                    j++;
                }
                i = j;
            }
        }


        //Garbage collect touch cursors.
        if (Input.touchCount == 0)
        {
            foreach (TouchCursor cursor in touchCursors)
            {
                if (cursor != null)
                {
                    cursor.endTouch();
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

    private bool ListenerIndexValid(int touchToMonitor)
    {
        if (!ListenerIndexValid(touchToMonitor))
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

    private bool Overtime(Vector2 times)
    {
        if ((times.y - times.x) > maxTapTime)
        {
            return true;
        }
        return false;
    }


}
