using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager_Trapeze : ISwipeListener, ITapListener
{

    [Tooltip("The player object. Needs script 'PlayerManager_Trapeze'")]
    public GameObject playerAvatar;
    [Tooltip("Whether or not the game is paused")]
    public bool paused;

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
    }

    void Start()
    {
        TouchInputManager t = TouchInputManager.getInstance();
        if (t == null) Destroy(this);
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
    }

    public void TapDetected(Vector3 position)
    {
        //Stubbed
    }

    public static GameManager_Trapeze GetInstance()
    {
        return instance;
    }
}
