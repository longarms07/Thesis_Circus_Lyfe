using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameManager_Trapeze : GameManager, ITapListener
{
    
    [Tooltip("How long a short swipe is, distance wise")]
    public float swipeShortDis;

    public TrickGUI trickGUI;
    public TrickManager trickManager;

    private static GameManager_Trapeze instance;


    public DonnaManager_Trapeze donnaManager;
    private PlayerManager_Trapeze pmt;


    private bool sloMo;
    private bool slowMoAllowed;
    private float timePrePause;
    private int timesPlayed = 0;


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
        buttonDict = new Dictionary<Transform, IButton>();
        sloMo = false;
        slowMoAllowed = true;
        savefile = "gamemanager_trapeze.save";
        //DeleteSaveData();
        LoadSaveData();
        timesPlayed++;
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
        pmt = playerAvatar.GetComponent<PlayerManager_Trapeze>();
        if(pmt == null)
        {
            Debug.Log("Player Avatar is missing PlayerManager_Trapeze script");
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Pause()
    {
        paused = !paused;
        if (paused)
        {
            timePrePause = Time.timeScale;
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = timePrePause;
            ignoreTap = true;
        }
    }

    override
    public void SwipeDetected(Vector3[] swipePositions)
    {
        if (paused) return;
        //Stubbed
        SwipeDirection dir = FindDirection(swipePositions);
        if(pmt.state == EnumPTrapezeState.OnTrapeze)
        {
            /*if ((dir != SwipeDirection.North && dir!=SwipeDirection.South) &&
                (Math.Abs(swipePositions[1].x - swipePositions[0].x) <= swipeShortDis))
                pmt.Short();
            else
                pmt.Long();*/
            if (dir == SwipeDirection.East)
            {
                if (pmt.FacingRight()) DoShort();
                else DoLong();
            }
            else if (dir == SwipeDirection.West)
            {
                if (pmt.FacingRight()) DoLong();
                else DoShort();
            }
        }
        else if(pmt.state == EnumPTrapezeState.InAir)
        {
            trickManager.AddSwipe(dir);
        }
    }

    new public void TapDetected(Vector3 position)
    {
        if (paused || ignoreTap)
        {
            ignoreTap = false;
            return;
        }
        //Stubbed
        CheckTappedPosition(position);
        if (pmt.state == EnumPTrapezeState.OnTrapeze)
        {
            if (lastTap.transform != null && newLastTap)
            {
                Debug.Log(lastTap.transform.gameObject);
                if (lastTap.transform.gameObject.layer == 5)
                {
                    IButton btn = GetButton(lastTap.transform);
                    if (btn != null) btn.OnTap();
                }
            }
            else
                pmt.Jump();
        }
        else if (pmt.state == EnumPTrapezeState.InAir)
        {
            if (lastTap.transform != null)
            {
                Debug.Log(lastTap.transform.name);
                if (lastTap.transform.gameObject.layer == interactableLayer || lastTap.transform.gameObject.layer == interactableLayer2)
                {
                    GetInteractable(lastTap.transform).OnInteraction();
                }
            }
        }
    }

    private void DoShort()
    {
        pmt.Short();
        trickGUI.DidTrick("Short", 0);
    }

    private void DoLong()
    {
        trickGUI.DidTrick("Long", 0);
        pmt.Long();
    }

    public static GameManager_Trapeze GetInstance()
    {
        return instance;
    }

    public PlayerManager_Trapeze GetPlayerManager()
    {
        return pmt;
    }

    public void ToggleSloMo()
    {
        if (sloMo)
        {
            Time.timeScale = Time.timeScale * 2;
        }
        else
        {
            Time.timeScale = Time.timeScale / 2;
        }
        Time.fixedDeltaTime = Time.timeScale * .02f;
        Time.maximumDeltaTime = Time.timeScale * .15f;
        sloMo = !sloMo;
    }

    public void SloMoAllowed(bool allow)
    {
        if (!allow && sloMo)
        {
            ToggleSloMo();
        }
        slowMoAllowed = allow;
    }

    public void SloMoAllowed()
    {
        SloMoAllowed(!slowMoAllowed);
    }

    public bool IsSloMoAllowed()
    {
        return slowMoAllowed;
    }

    public bool InSloMo()
    {
        return sloMo;
    }

    void OnDestroy()
    {
        SaveData();
        if (InSloMo()) ToggleSloMo();
    }

    public int GetTimesPlayed()
    {
        return timesPlayed;
    }

    [System.Serializable]
    class TrapezeSaveData
    {
        public int timesPlayed;
    }

    override public void SaveData()
    {
        TrapezeSaveData save = new TrapezeSaveData();
        save.timesPlayed = timesPlayed;
        BinaryFormatter format = new BinaryFormatter();
        FileStream fs = File.Create(Application.persistentDataPath + savefile);
        //Debug.Log(Application.persistentDataPath + savefile);
        format.Serialize(fs, save);
        fs.Close();
        Debug.Log("Game Saved");
    }

    public override bool LoadSaveData()
    {
        if (File.Exists(Application.persistentDataPath + savefile))
        {
            BinaryFormatter format = new BinaryFormatter();
            FileStream fs = File.Open(Application.persistentDataPath + savefile, FileMode.Open);
            TrapezeSaveData save = (TrapezeSaveData) format.Deserialize(fs);
            fs.Close();
            timesPlayed = save.timesPlayed;
            Debug.Log("Game loaded");
            return true;
        }
        return false;
    }

}
