using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager_Trapeze : GameManager, ITapListener
{
    
    [Tooltip("Whether or not the game is paused")]
    public bool paused;
    [Tooltip("How long a short swipe is, distance wise")]
    public float swipeShortDis;

    public TrickGUI trickGUI;

    private static GameManager_Trapeze instance;



    private PlayerManager_Trapeze pm;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        interactableDict = new Dictionary<Transform, IInteractable>();
    }

    void Start()
    {
        TouchInputManager t = TouchInputManager.getInstance();
        if (t == null) Destroy(this);
        trickGUI = TrickGUI.GetInstance();
        t.SubscribeTapListener(this, 0);
        t.SubscribeSwipeListener(this, 0);
        if(playerAvatar == null)
        {
            Debug.Log("Player Avatar is null");
            Destroy(this);
        }
        pm = playerAvatar.GetComponent<PlayerManager_Trapeze>();
        if(pm == null)
        {
            Debug.Log("Player Avatar is missing PlayerManager_Trapeze script");
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    override
    public void SwipeDetected(Vector3[] swipePositions)
    {
        //Stubbed
        SwipeDirection dir = FindDirection(swipePositions);
        if(pm.state == EnumPTrapezeState.OnTrapeze)
        {
            /*if ((dir != SwipeDirection.North && dir!=SwipeDirection.South) &&
                (Math.Abs(swipePositions[1].x - swipePositions[0].x) <= swipeShortDis))
                pm.Short();
            else
                pm.Long();*/
            if (dir == SwipeDirection.East)
            {
                pm.Short();
                trickGUI.DidTrick("Short", 0);
            }
            else if (dir == SwipeDirection.West)
            {

                trickGUI.DidTrick("Long", 0);
                pm.Long();
            }
        }
    }

    new public void TapDetected(Vector3 position)
    {
        //Stubbed
        CheckTappedPosition(position);
        if (pm.state == EnumPTrapezeState.OnTrapeze) pm.Jump();
        else if (pm.state == EnumPTrapezeState.InAir)
        {
            if (lastTap.transform != null)
            {
                if (lastTap.transform.gameObject.layer == interactableLayer || lastTap.transform.gameObject.layer == interactableLayer2)
                {
                    GetInteractable(lastTap.transform).OnInteraction();
                }
            }
        }
    }

    public static GameManager_Trapeze GetInstance()
    {
        return instance;
    }

    public PlayerManager_Trapeze GetPlayerManager()
    {
        return pm;
    }

}
