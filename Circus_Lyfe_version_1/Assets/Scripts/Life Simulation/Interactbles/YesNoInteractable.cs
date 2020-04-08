using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public abstract class YesNoInteractable : MonoBehaviour, IInteractable
{

    protected string message;
    protected SpriteRenderer spriteRenderer;
    public Sprite defaultSprite;
    public Sprite inRangeSprite;
    public GameObject donnaSpawnPosition;
    public string yarnNode;
    public string tiredNode;
    public YarnProgram scriptToLoad;
    protected bool isMajorAction;
    //protected bool buttonsEnabled;
    protected GameManager gm;
    protected bool reqFollower = false;
    protected bool isInRange;
    protected DialogueRunner dialogueRunner;

    // Start is called before the first frame update
    void Start()
    {
        DoAtStart();
    }

    protected void DoAtStart()
    {
        isInRange = false;
        gm = GameManager.getInstance();
        gm.RegisterInteractable(this.gameObject.transform, this);
        spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        //buttonsEnabled = true;
        dialogueRunner = FindObjectOfType<Yarn.Unity.DialogueRunner>();
        if (scriptToLoad != null)
        {
            dialogueRunner.Add(scriptToLoad);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnInteraction()
    {
        if (isInRange)
        {
            if (!isMajorAction || !GameManager.getInstance().MajorActionDone())
            {
                dialogueRunner.StartDialogue(yarnNode);
            }
            else
            {
                dialogueRunner.StartDialogue(tiredNode);
                //buttonsEnabled = false;
            }
        }
        //TextboxManager.GetInstance().TextBoxActive(true);
    }

    public void InRange(bool inRange)
    {

        if (inRange && 
            ((reqFollower && gm.GetPlayerMovable().GetIsFollowed())
            || (!reqFollower && !gm.GetPlayerMovable().GetIsFollowed())))
        {
            isInRange = true;
            spriteRenderer.sprite = inRangeSprite;
        }
        else
        {
            isInRange = false;
            spriteRenderer.sprite = defaultSprite;
        }
    }

    [YarnCommand("YesNoAnswer")]
    public abstract void OnButtonPressed(string answer);

    /*public void OnTextEnded()
    {
        if (buttonsEnabled)
        {
            string[] text = new string[2];
            text[0] = "Yes";
            text[1] = "No";
            int[] codes = new int[2];
            codes[0] = 0;
            codes[1] = 1;
            TextboxManager.GetInstance().GenerateTextButtons(this, text, codes);
        }
        else
        {
            buttonsEnabled = true;
        }
    }*/



}
