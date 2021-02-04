using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ListenerTest :  ISwipeListener, IDragListener, ITapListener
{
    
    private TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        text = this.gameObject.GetComponent<TextMeshProUGUI>();
        IInputManager touchInputManager = IInputManager.getInstance();
        touchInputManager.SubscribeSwipeListener(this, 0);
        touchInputManager.SubscribeTapListener(this, 0);
        touchInputManager.SubscribeDragListener(this, 0);
    }

    override
    public void SwipeDetected(Vector3[] swipePositions)
    {
        text.text = "Swipe from " + swipePositions[0] + " to " + swipePositions[1] + " in direction " + FindDirection(swipePositions);
    }

    public void TapDetected(Vector3 position)
    {
        text.text = "Tap detected at " + position;
    }

    public void DragStarted(Vector3[] dragPositions)
    {
        text.text = "Drag started from " + dragPositions[0] + " to " + dragPositions[1];
    }

    public void DragPoisitonChanged(Vector3[] dragPositions)
    {
        text.text = "Drag position changed to "+dragPositions[1];
    }

    public void DragEnded(Vector3[] dragPositions, List<Vector2> deltaPositions)
    {
        text.text = "Drag ended from " + dragPositions[0] + " to " + dragPositions[1];
    }


}
