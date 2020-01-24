using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Yarn.Unity;

public abstract class NPCInteractable : MonoBehaviour, IInteractable, ITextboxListener, IDayTimeChangeListener
{

    
    public YarnProgram scriptToLoad;

    private SpriteRenderer spriteRenderer;
    public Sprite defaultSprite;
    public Sprite inRangeSprite;

    [Tooltip("The Yarn Nodes for each major conversation")]
    public string[] majorConvoNodes;
    [Tooltip("The Trust Levels required to unlock each conversation")]
    public float[] majorConvoTrust;
    public bool doMajorConvos;
    private int majorConvoIndex;


    protected Rigidbody2D rb;
    protected GameManager gm;
    public List<DayTimeScheduleConverter> schedule2convert; 
    protected Dictionary<DayEnums, Dictionary<TimeEnums, NPCLocation>> schedule;
    protected InMemoryVariableStorage yarnVars;
    protected DialogueRunner dialogueRunner;
    protected string savefile;

    // Start is called before the first frame update
    void Start()
    {
        if (majorConvoNodes.Length != majorConvoTrust.Length)
        {
            Debug.LogError("Error! There are not the same number of MajorConvo nodes and required trust levels!");
            Destroy(this.gameObject);
        }
        gm = GameManager.getInstance();
        ConvertSchedule();
        gm.RegisterInteractable(this.gameObject.transform, this);
        spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        gm.SubscribeDayTimeChangeListener(this);
        rb = GetComponent<Rigidbody2D>();
        yarnVars = FindObjectOfType<InMemoryVariableStorage>();
        if (scriptToLoad != null)
        {
            dialogueRunner = FindObjectOfType<Yarn.Unity.DialogueRunner>();
            dialogueRunner.Add(scriptToLoad);
        }

        if(!LoadStats()) majorConvoIndex = 0;
        

        AddToStart();
        DayTimeChange(gm.currentDay, gm.currentTime);
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected abstract void AddToStart();

    public abstract void OnInteraction();
    public void InRange(bool inRange)
    {
        if (inRange)
        {
            spriteRenderer.sprite = inRangeSprite;
        }
        else
        {
            spriteRenderer.sprite = defaultSprite;
        }
    }

    public void DayTimeChange(DayEnums newDay, TimeEnums newTime)
    {
        this.transform.position = schedule[newDay][newTime].pos.position;
        AddToDayTimeChange(newDay, newTime);
        if (newTime == TimeEnums.Morning && doMajorConvos) Invoke("CheckConversation", 0.5f);
    }

    public abstract void AddToDayTimeChange(DayEnums newDay, TimeEnums newTime);

    public abstract void OnTextEnded();

    void OnDestroy()
    {
        if (gm != null) gm.UnsubscribeDayTimeChangeListener(this);
        SaveStats();
    }

    public void ConvertSchedule()
    {
        schedule = new Dictionary<DayEnums, Dictionary<TimeEnums, NPCLocation>>();
        foreach(DayTimeScheduleConverter c in schedule2convert)
        {
            if (schedule.ContainsKey(c.day))
            {
                if (!schedule[c.day].ContainsKey(c.time)) schedule[c.day].Add(c.time, c.position);
            }
            else {
                schedule.Add(c.day, new Dictionary<TimeEnums, NPCLocation>());
                schedule[c.day].Add(c.time, c.position);
            }
        }
    }

    public abstract float GetTrustLevel();

    public void CheckConversation()
    {
        if(majorConvoIndex < majorConvoNodes.Length && GetTrustLevel() >= majorConvoTrust[majorConvoIndex])
        {
            Debug.Log("Init major conversation: " + majorConvoNodes[majorConvoIndex]);
            //this.transform.position = gm.GetPlayerManager().transform.position + (Vector3.down*gm.GetPlayerMovable().followDist);
            dialogueRunner.StartDialogue(majorConvoNodes[majorConvoIndex]);
            majorConvoIndex++; 
        }
    }

    public void ResetSaveStats()
    {
        DeleteSaveData();
        majorConvoIndex = 0;
    }

    public void SaveStats()
    {
        NPCStats save = new NPCStats();
        save.convoIndex = majorConvoIndex;
        BinaryFormatter format = new BinaryFormatter();
        FileStream fs = File.Create(Application.persistentDataPath + savefile);
        format.Serialize(fs, save);
        fs.Close();
        Debug.Log(gameObject.name +" Stats Saved");
    }

    public bool LoadStats()
    {
        if (File.Exists(Application.persistentDataPath + savefile))
        {
            BinaryFormatter format = new BinaryFormatter();
            FileStream fs = File.Open(Application.persistentDataPath + savefile, FileMode.Open);
            NPCStats save = (NPCStats)format.Deserialize(fs);
            fs.Close();
            majorConvoIndex = save.convoIndex;
            Debug.Log(gameObject.name+" stats loaded");
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
    }

    private void OnApplicationPause(bool pause)
    {
        SaveStats();
    }

    private void OnApplicationQuit()
    {
        SaveStats();
    }

    
}
