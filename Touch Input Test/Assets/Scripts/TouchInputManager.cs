using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TouchInputManager : MonoBehaviour
{

    public GameObject displayText;
    public GameObject touchCursorPrefab;
    public int fingersSupported;

    private TouchCursor[] touchCursors;
    private List<TouchMovable>[] touchMovables;
    private TextMeshProUGUI display;
    static TouchInputManager instance;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null) { instance = this; }
        else { Destroy(this); }
        display = displayText.GetComponent<TextMeshProUGUI>();
        touchCursors = new TouchCursor[fingersSupported];
        touchMovables = new List<TouchMovable>[fingersSupported];
        for(int i = 0; i < touchMovables.Length; i++)
        {
            touchMovables[i] = new List<TouchMovable>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        string text = "Size of Input.touches " + Input.touchCount;
        for (int i=0; i< Math.Min(Input.touchCount, touchCursors.Length); i++)
        {
            Touch touch = Input.touches[i];
            if(touch.phase == TouchPhase.Began)
            {
                touchCursors[i] = Instantiate(touchCursorPrefab).GetComponent<TouchCursor>();
                touchCursors[i].changePosition(Camera.main.ScreenToWorldPoint(touch.position));
                Debug.Log("Touch Event " + i + " Began");
            }
            if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
            {
                
                if (touchCursors[i] != null && touch.phase == TouchPhase.Moved)
                {
                    touchCursors[i].changePosition(Camera.main.ScreenToWorldPoint(touch.position));
                }
                foreach(TouchMovable touchMovable in touchMovables[i])
                {
                    touchMovable.move(touch.deltaPosition, touch.deltaTime);
                }
                text += "\n Touch " + i + " at Position " + touch.position+"at deltaPosition "+touch.deltaPosition;
            }
            else
            {
                if (touchCursors[i] != null) { 
                    touchCursors[i].endTouch();
                    touchCursors[i] = null;
                    int j = 0;
                    while (touchCursors[i] == null && i + j < touchCursors.Length)
                    {
                        touchCursors[i] = touchCursors[i + j];
                        j++;
                    }
                    Debug.Log("Touch Event "+ i+" Ended");
                }
            text += "\n Touch " + i + " Ended.";
            }
        }
        if (display != null) { display.text = text; }
        if(Input.touchCount == 0)
        {
            foreach(TouchCursor cursor in touchCursors)
            {
                if (cursor != null) {
                    cursor.endTouch();
                }
            }
        }
    }
    
    

    public static TouchInputManager getInstance()
    {
        return instance;
    }

    //<summary> Allows a TouchMovable to add itself as the observer for a certain touch number </summary>
    //<param name = "touchMovable" > The TouchMovable that wishes to be an observer </ param >
    //<param name = "touchToMonitor" > What index touch to monitor (the first, second, etc.) </ param >
    public bool subscribeTouchMovement(TouchMovable touchMovable, int touchToMonitor)
    {
        if (touchToMonitor < 0 || touchToMonitor > fingersSupported - 1)
        {
            return false;
        }
        touchMovables[touchToMonitor].Add(touchMovable);
        return true;
    }

    //<summary> Allows a TouchMovable to remove itself as the observer for a certain touch number </summary>
    //<param name = "touchMovable" > The TouchMovable that wishes to stop being an observer </ param >
    //<param name = "touchToMonitor" > What index touch to unsubscribe from (the first, second, etc.) </ param >
    public bool unsubscribeTouchMovement(TouchMovable touchMovable, int touchToMonitor)
    {
        if (touchToMonitor < 0 || touchToMonitor > fingersSupported - 1)
        {
            return false;
        }
        return touchMovables[touchToMonitor].Remove(touchMovable);
    }

}
