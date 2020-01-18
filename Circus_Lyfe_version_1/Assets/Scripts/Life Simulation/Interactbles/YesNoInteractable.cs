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
    public string yarnNode;
    public string tiredNode;
    public YarnProgram scriptToLoad;
    protected bool isMajorAction;
    //protected bool buttonsEnabled;
    protected GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        DoAtStart();
    }

    protected void DoAtStart()
    {
        gm = GameManager.getInstance();
        gm.RegisterInteractable(this.gameObject.transform, this);
        spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        //buttonsEnabled = true;
        if (scriptToLoad != null)
        {
            DialogueRunner dialogueRunner = FindObjectOfType<Yarn.Unity.DialogueRunner>();
            dialogueRunner.Add(scriptToLoad);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnInteraction()
    {
        if (!isMajorAction || !GameManager.getInstance().MajorActionDone())
        {
            FindObjectOfType<DialogueRunner>().StartDialogue(yarnNode);
        }
        else
        {
            FindObjectOfType<DialogueRunner>().StartDialogue(tiredNode);
            //buttonsEnabled = false;
        }

        //TextboxManager.GetInstance().TextBoxActive(true);
    }

    public void InRange(bool inRange)
    {
        Debug.Log("Player house in range called");
        if (inRange)
        {
            spriteRenderer.sprite = inRangeSprite;
        }
        else
        {
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
