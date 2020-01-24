using System.Collections;
using System.Collections.Generic;
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
    private int majorConvoIndex;


    protected Rigidbody2D rb;
    protected GameManager gm;
    public List<DayTimeScheduleConverter> schedule2convert; 
    protected Dictionary<DayEnums, Dictionary<TimeEnums, NPCLocation>> schedule;
    protected InMemoryVariableStorage yarnVars;
    protected DialogueRunner dialogueRunner;
    protected string filename;

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

        majorConvoIndex = 0;

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
        if(newTime==TimeEnums.Morning) Invoke("CheckConversation", 0.5f);
    }

    public abstract void AddToDayTimeChange(DayEnums newDay, TimeEnums newTime);

    public abstract void OnTextEnded();

    void OnDestroy()
    {
        if (gm != null) gm.UnsubscribeDayTimeChangeListener(this);
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
            this.transform.position = gm.GetPlayerManager().transform.position + (Vector3.down*gm.GetPlayerMovable().followDist);
            dialogueRunner.StartDialogue(majorConvoNodes[majorConvoIndex]);
            majorConvoIndex++; 
        }
    }
}
