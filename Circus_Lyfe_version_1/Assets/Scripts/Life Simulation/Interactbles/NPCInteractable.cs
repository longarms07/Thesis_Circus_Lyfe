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
    protected Rigidbody2D rb;
    protected GameManager gm;
    public List<DayTimeScheduleConverter> schedule2convert; 
    protected Dictionary<DayEnums, Dictionary<TimeEnums, NPCLocation>> schedule;
    protected InMemoryVariableStorage yarnVars;
    protected DialogueRunner dialogueRunner;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.getInstance();
        ConvertSchedule();
        gm.RegisterInteractable(this.gameObject.transform, this);
        spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        gm.SubscribeDayTimeChangeListener(this);
        DayTimeChange(gm.currentDay, gm.currentTime);
        rb = GetComponent<Rigidbody2D>();
        yarnVars = FindObjectOfType<InMemoryVariableStorage>();
        if (scriptToLoad != null)
        {
            dialogueRunner = FindObjectOfType<Yarn.Unity.DialogueRunner>();
            dialogueRunner.Add(scriptToLoad);
        }
        AddToStart();
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
    }

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


}
