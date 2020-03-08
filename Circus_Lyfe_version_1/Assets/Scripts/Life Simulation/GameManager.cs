using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class GameManager :  ISwipeListener, ITapListener
{
    [Tooltip("The avatar for the player character. Requires TouchMovable.")]
    public GameObject playerAvatar;
    [Tooltip("The layer interactables are found on.")]
    public int interactableLayer;
    [Tooltip("The layer intangible interactables are found on.")]
    public int interactableLayer2;
    [Tooltip("The layer the floor is found on.")]
    public int floorLayer;
    [Tooltip("The layer NPCS are found on")]
    public int npcLayer;
    [Tooltip("Use for Fade to Black")]
    public GameObject fadeToBlack;

    public DayEnums currentDay;
    public TimeEnums currentTime;
    public bool majorActionDone;
    public int daysTillPerformance;
    public TMPro.TextMeshProUGUI dayCountdown;
    
 
    public bool paused = false;
    public Dictionary<Transform, IInteractable> interactableDict;
    protected Dictionary<Transform, IButton> buttonDict;
    private TouchMovable playerTouchMovable;
    protected RaycastHit2D lastTap;
    protected bool newLastTap;
    protected string savefile = "gamemanager.save";
    private List<IDayTimeChangeListener> dayTimeChangeListeners;
    private PlayerManager pm;
    private DialogueRunner dr;
    protected bool ignoreTap = false;
    protected static bool duoTrapeze = false;
    protected static bool canTutorial = false;
    protected static bool duoTutorial = false;
    protected static bool gradedPerformance = false;
    private int daysSoFar;


    private static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            interactableDict = new Dictionary<Transform, IInteractable>();
            buttonDict = new Dictionary<Transform, IButton>();
        }
        else
        {
            Destroy(this.gameObject);
        }
        if (!LoadSaveData())
        {
            currentDay = DayEnums.Monday;
            currentTime = TimeEnums.Morning;
            majorActionDone = false;
            daysSoFar = 1;
        }
        dayTimeChangeListeners = new List<IDayTimeChangeListener>();

    }

    private void Start()
    {
        if(playerAvatar == null)
        {
            Debug.LogError("Player Avatar is null!");
            Destroy(this.gameObject);
        }
        else
        {
            playerTouchMovable = playerAvatar.GetComponent<TouchMovable>();
            if(playerTouchMovable == null)
            {
                Debug.LogError("Player Avatar has no touch movable!");
                Destroy(this.gameObject);
            }
            TouchInputManager.getInstance().SubscribeTapListener(this, 0);
            dayCountdown.text = "Days until performance: " + (daysTillPerformance - (daysSoFar % daysTillPerformance));
            pm = playerAvatar.GetComponent<PlayerManager>();
            dr = FindObjectOfType<DialogueRunner>();
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void TogglePlayerMovement(bool active)
    {
        playerTouchMovable.ToggleMovement(active);
    }

    public void TogglePlayerMovement()
    {
        playerTouchMovable.ToggleMovement();
    }

    public virtual void Pause()
    {
        paused = !paused;
        ignoreTap = true;
        if(!dr.isDialogueRunning)
            TogglePlayerMovement(!paused);
    }

    public void TapDetected(Vector3 position)
    {
        if (paused || ignoreTap)
        {
            ignoreTap = false;
            return;
        }
        CheckTappedPosition(position);
        if (lastTap.transform != null && newLastTap) {
            Debug.Log("Hit layer = " + lastTap.transform.gameObject.layer + "name = " + lastTap.transform.gameObject.name);
            if (lastTap.transform.gameObject.layer == floorLayer)
            {
                playerTouchMovable.OnTap(position);
            }
            else if (lastTap.transform.gameObject.layer == interactableLayer || lastTap.transform.gameObject.layer == npcLayer 
                || lastTap.transform.gameObject.layer == interactableLayer2)
            {
                playerTouchMovable.TargetInteractable(lastTap.collider, GetInteractable(lastTap.transform));
            }
            else if (lastTap.transform.gameObject.layer == 5)
            {
                if (lastTap.transform.gameObject == TextboxManager.GetInstance().textBackground)
                {
                    TextboxManager.GetInstance().OnTap();
                }
                else
                {
                    /*IButton btn = GetButton(lastTap.transform);
                    if (btn != null) btn.OnTap();*/
                }
            }
        }
    

    }

    protected RaycastHit2D CheckTappedPosition(Vector3 position)
    {
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.down, 0.1f);
        if (hit.collider != null)
        {
            lastTap = hit;
            newLastTap = true;
        }
        else
            newLastTap = false;
        return hit;

    }

    public static GameManager getInstance()
    {
        return instance;
    }

    public void RegisterInteractable(Transform transform, IInteractable interactable)
    {
        interactableDict.Add(transform, interactable);
    }

    public IInteractable GetInteractable(Transform transform)
    {
        //Debug.Log("Key = " + transform);
        if(interactableDict.ContainsKey(transform))
            return interactableDict[transform];
        return null;
    }

    public void RegisterButton(Transform transform, IButton button)
    {
            buttonDict.Add(transform, button);
            //Debug.Log("Registered button");
    }

    public IButton GetButton(Transform transform)
    {
       // Debug.Log("Key = " + transform);
        if (transform!=null && buttonDict.ContainsKey(transform))
            return buttonDict[transform];
        return null;
    }

    public void DeregisterButton(Transform transform)
    {
        if (buttonDict.ContainsKey(transform))
            buttonDict.Remove(transform);
    }

    override
    public void SwipeDetected(Vector3[] swipePositions)
    { }


    [YarnCommand("ChangeSceneTrapeze")]
    public void ChangeSceneTrapeze()
    {
        if (SceneManager.GetActiveScene().name == "MovementDemoScene") 
        {
            SceneManager.LoadScene("TrapezeDemoScene");
        }
    }


    [YarnCommand("ChangeSceneLifeSim")]
    public void ChangeSceneLifeSim()
    {
        if(SceneManager.GetActiveScene().name == "TrapezeDemoScene")
        {
            SceneManager.LoadScene("MovementDemoScene");
        }
    }

    public DayEnums GetCurrentDay()
    {
        return currentDay;
    }

    public TimeEnums GetCurrentTime()
    {
        return currentTime;
    }

    public bool MajorActionDone()
    {
        return majorActionDone;
    }

    public void MajorActionCompleted(bool reload)
    {
        if(currentTime == TimeEnums.Evening)
        {
            //Disable major interactables. Probably should do this in their script.
            majorActionDone = true;
        }
        else
        {
            if (reload) FadeToBlack();
            currentTime = TimeEnums.Evening;
            TextboxManager.GetInstance().UpdateDateTime();
                foreach (IDayTimeChangeListener listener in dayTimeChangeListeners)
                {
                    listener.DayTimeChange(currentDay, currentTime);
                }
            if (reload) Invoke("FadeOutBlack", 0.5f);
        }
    }

    public void IncrementDay()
    {
        FadeToBlack();
        switch (currentDay)
        {
            case DayEnums.Monday:
                currentDay = DayEnums.Tuesday;
                break;
            case DayEnums.Tuesday:
                currentDay = DayEnums.Wednesday;
                break;
            case DayEnums.Wednesday:
                currentDay = DayEnums.Thursday;
                break;
            case DayEnums.Thursday:
                currentDay = DayEnums.Friday;
                break;
            case DayEnums.Friday:
                currentDay = DayEnums.Saturday;
                break;
            case DayEnums.Saturday:
                currentDay = DayEnums.Sunday;
                break;
            case DayEnums.Sunday:
                currentDay = DayEnums.Monday;
                break;
        }
        currentTime = TimeEnums.Morning;
        majorActionDone = false;
        daysSoFar++;
        dayCountdown.text = "Days until performance: " + (daysTillPerformance - (daysSoFar % daysTillPerformance));
        if (daysSoFar % daysTillPerformance == 0)
        {
            gradedPerformance = true;
            foreach (IDayTimeChangeListener listener in dayTimeChangeListeners)
            {
                listener.PerformanceDay();
            }
        }
        else
        {
            foreach (IDayTimeChangeListener listener in dayTimeChangeListeners)
            {
                listener.DayTimeChange(currentDay, currentTime);
            }
        }
        TextboxManager.GetInstance().UpdateDateTime();
        
        Invoke("FadeOutBlack", 0.5f);
    }

    public void FadeToBlack()
    {
        fadeToBlack.SetActive(true);
    }

    public void FadeOutBlack()
    {
        fadeToBlack.SetActive(false);

    }

    public void ResetSavedVars()
    {
        FadeToBlack();
        DeleteSaveData();
        currentTime = TimeEnums.Morning;
        currentDay = DayEnums.Monday;
        majorActionDone = false;
        Invoke("FadeOutBlack", 1f);
    }

    public virtual void SaveData()
    {
        LifeSimSaveData save = new LifeSimSaveData();
        save.day = currentDay;
        save.time = currentTime;
        save.majorActionDone = majorActionDone;
        save.duoTutorial = duoTutorial;
        save.days = daysSoFar;
        BinaryFormatter format = new BinaryFormatter();
        FileStream fs = File.Create(Application.persistentDataPath + savefile);
        //Debug.Log(Application.persistentDataPath + savefile);
        format.Serialize(fs, save);
        fs.Close();
        Debug.Log("Game Saved");    
    }

    public virtual bool LoadSaveData()
    {
        if (File.Exists(Application.persistentDataPath + savefile))
        {
            BinaryFormatter format = new BinaryFormatter();
            FileStream fs = File.Open(Application.persistentDataPath + savefile, FileMode.Open);
            LifeSimSaveData save = (LifeSimSaveData)format.Deserialize(fs);
            fs.Close();
            currentDay = save.day;
            currentTime = save.time;
            majorActionDone = save.majorActionDone;
            daysSoFar = save.days;
            duoTutorial = save.duoTutorial;
            Debug.Log("Game loaded");
            return true;
        }
        return false;
    }


    public void DeleteSaveData()
    {
        if (File.Exists(Application.persistentDataPath + savefile))
        {
           File.Delete(Application.persistentDataPath + savefile);
        }
        if (File.Exists(Application.persistentDataPath + "gamemanager_trapeze.save"))
        {
            File.Delete(Application.persistentDataPath + "gamemanager_trapeze.save");
        }
        TutorialManager.DeleteSaveData();
        TrickManager.DeleteSaveData();
        
    }
    

    private void OnApplicationPause(bool pause)
    {
        SaveData(); 
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }

    private void OnDestroy()
    {
        SaveData();
    }

    public bool SubscribeDayTimeChangeListener(IDayTimeChangeListener listener)
    {
        if (!dayTimeChangeListeners.Contains(listener))
        {
            dayTimeChangeListeners.Add(listener);
            return true;
        }
        return false;
    }

    public bool UnsubscribeDayTimeChangeListener(IDayTimeChangeListener listener)
    {
        if (dayTimeChangeListeners.Contains(listener))
        {
            dayTimeChangeListeners.Remove(listener);
            return true;
        }
        return false;

    }

    public void AllowTrapezeTutorial(bool yesno)
    {
        canTutorial = yesno;
    }

    [YarnCommand("ActivateDuoTutorial")]
    public void ActivateDuoTutorial()
    {
        duoTutorial = true;
    }

    public bool GetDuoTutorial() { return duoTutorial; }

    public void AllowDonnaOnTrapeze(bool yesno)
    {
        duoTrapeze = yesno;
    }

    public PlayerManager GetPlayerManager()
    {
        return pm;
    }

    public TouchMovable GetPlayerMovable()
    {
        return playerTouchMovable;
    }

    public void ReloadScene()
    {
        Invoke("ReloadScene4Real", 1);
    }

    private void ReloadScene4Real()
    {
        SceneManager.LoadScene("MovementDemoScene");
    } 

}
