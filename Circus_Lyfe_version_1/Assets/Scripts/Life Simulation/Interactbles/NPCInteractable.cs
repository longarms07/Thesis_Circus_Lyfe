using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NPCInteractable : MonoBehaviour, IInteractable, ITextboxListener, IDayTimeChangeListener
{
    private SpriteRenderer spriteRenderer;
    public Sprite defaultSprite;
    public Sprite inRangeSprite;
    protected GameManager gm;
    public List<DayTimeScheduleConverter> schedule2convert; 
    protected Dictionary<DayEnums, Dictionary<TimeEnums, Vector3>> schedule;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.getInstance();
        ConvertSchedule();
        gm.RegisterInteractable(this.gameObject.transform, this);
        spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        gm.SubscribeDayTimeChangeListener(this);
        DayTimeChange(gm.currentDay, gm.currentTime);
    }

    // Update is called once per frame
    void Update()
    {

    }

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
        this.transform.position = schedule[newDay][newTime];
    }

    public abstract void OnTextEnded();

    void OnDestroy()
    {
        if (gm != null) gm.UnsubscribeDayTimeChangeListener(this);
    }

    public void ConvertSchedule()
    {
        schedule = new Dictionary<DayEnums, Dictionary<TimeEnums, Vector3>>();
        foreach(DayTimeScheduleConverter c in schedule2convert)
        {
            if (schedule.ContainsKey(c.day))
            {
                if (!schedule[c.day].ContainsKey(c.time)) schedule[c.day].Add(c.time, c.position.transform.position);
            }
            else {
                schedule.Add(c.day, new Dictionary<TimeEnums, Vector3>());
                Debug.Log(schedule.ContainsKey(c.day));
                Debug.Log(c.day + ", " + c.time);
                Debug.Log(c.position.name);
                Debug.Log(c.position.transform.position);
                Debug.Log(c.time);
                schedule[c.day].Add(c.time, c.position.transform.position);
            }
        }
    }


}
