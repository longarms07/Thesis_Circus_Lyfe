using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MouseInputManager : IInputManager
{
    private List<ITapListener> tapListeners = new List<ITapListener>();
    private List<ISwipeListener> swipeListeners = new List<ISwipeListener>();
    private List<IDragListener> dragListeners = new List<IDragListener>();
    private List<Vector2> touchDeltaPoisitons;
    private Vector3[] touchStartEndPoints;
    private Vector2 touchTimes;
    private TouchInputType inputType;
    TouchCursor touchCursor;

    private TextMeshProUGUI display;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null && 
            (Application.platform == RuntimePlatform.WebGLPlayer || Application.platform == RuntimePlatform.WindowsEditor)) 
            { instance = this; }
        else { Destroy(this); }
        touchDeltaPoisitons = new List<Vector2>();
        touchStartEndPoints = new Vector3[2];
        touchTimes = new Vector2();
        inputType = TouchInputType.NONE;
        Debug.Log("MouseInputManager Awake Called");
    }

    private void Start()
    {
        if (display != null) display = displayText.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        // When mouse button is first clicked
        if (Input.GetMouseButtonDown(0))
        {
            touchTimes = new Vector2(Time.time, Time.time);
            Vector3 worldPosition = ScreenPointToWorldPoint(mousePosition);
            inputType = TouchInputType.TAP;
            touchStartEndPoints[0] = worldPosition;
            touchDeltaPoisitons.Add(mousePosition);
            touchCursor = Instantiate(touchCursorPrefab).GetComponent<TouchCursor>();
            touchCursor.changePosition(worldPosition);
        }
        // When mouse button is held down
        else if (Input.GetMouseButton(0))
        {
            // If Mouse has moved
            if (mousePosition.x != touchDeltaPoisitons[touchDeltaPoisitons.Count - 1].x
                && mousePosition.y != touchDeltaPoisitons[touchDeltaPoisitons.Count - 1].y)
            {
                touchDeltaPoisitons.Add(mousePosition);
                Vector3 worldPosition = ScreenPointToWorldPoint(mousePosition);
                touchStartEndPoints[1] = worldPosition;
                touchCursor.changePosition(worldPosition);
                if (inputType == TouchInputType.TAP) { inputType = TouchInputType.SWIPE; }
                if (inputType == TouchInputType.DRAG)
                {
                    foreach (IDragListener listener in dragListeners)
                    {
                        listener.DragPoisitonChanged(touchStartEndPoints);
                    }
                }
            }
            touchTimes.y = Time.time;
            // Check if enough time has passed to be considered a drag
            if (inputType != TouchInputType.DRAG)
            {
                inputType = Overtime(touchTimes, inputType);
                if (inputType == TouchInputType.DRAG)
                {
                    foreach (IDragListener listener in dragListeners)
                    {
                        listener.DragStarted(touchStartEndPoints);
                    }
                }
            }
        }
        // Handle the end of input event
        else if (inputType != TouchInputType.NONE)
        {
            //Notify listeners
            if (inputType == TouchInputType.SWIPE)
            {
                foreach (ISwipeListener listener in swipeListeners)
                {
                    listener.SwipeDetected(touchStartEndPoints);
                }
            }
            else if (inputType == TouchInputType.TAP)
            {
                foreach (ITapListener listener in tapListeners)
                {
                    listener.TapDetected(touchStartEndPoints[0]);
                }
            }
            else
            {
                foreach (IDragListener listener in dragListeners)
                {
                    listener.DragEnded(touchStartEndPoints, touchDeltaPoisitons);
                }
            }
            touchCursor.endTouch();
            inputType = TouchInputType.NONE;
        }
    }

    public override bool SubscribeTapListener(ITapListener tap, int touchToMonitor)
    {
        if (!ListenerIndexValid(touchToMonitor) || tap == null || tapListeners.Contains(tap))
        {
            return false;
        }
        tapListeners.Add(tap);
        return true;
    }

    public override bool SubscribeSwipeListener(ISwipeListener swipe, int touchToMonitor)
    {
        if (!ListenerIndexValid(touchToMonitor) || swipe == null || swipeListeners.Contains(swipe))
        {
            return false;
        }
        swipeListeners.Add(swipe);
        return true;
    }

    public override bool SubscribeDragListener(IDragListener drag, int touchToMonitor)
    {
        if (!ListenerIndexValid(touchToMonitor) || drag == null || dragListeners.Contains(drag))
        {
            return false;
        }
        dragListeners.Add(drag);
        return true;
    }



    public override bool UnsubscribeTapListener(ITapListener tap, int touchToMonitor)
    {
        if (!ListenerIndexValid(touchToMonitor))
        {
            return false;
        }
        return tapListeners.Remove(tap);
    }

    public override bool UnsubscribeSwipeListener(ISwipeListener swipe, int touchToMonitor)
    {
        if (!ListenerIndexValid(touchToMonitor))
        {
            return false;
        }
        return swipeListeners.Remove(swipe);
    }

    public override bool UnsubscribeDragListener(IDragListener drag, int touchToMonitor)
    {
        if (!ListenerIndexValid(touchToMonitor))
        {
            return false;
        }
        return dragListeners.Remove(drag);
    }

    public bool ListenerIndexValid(int index) { if (index == 0) return true; else return false; }
}
