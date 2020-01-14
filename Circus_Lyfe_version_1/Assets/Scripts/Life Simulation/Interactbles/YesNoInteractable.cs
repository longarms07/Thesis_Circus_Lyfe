using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class YesNoInteractable : MonoBehaviour, IInteractable, IButtonListener, ITextboxListener
{

     protected string message;
     protected SpriteRenderer spriteRenderer;
     public Sprite defaultSprite;
     public Sprite inRangeSprite;
    protected bool isMajorAction;
    protected bool buttonsEnabled;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.getInstance().RegisterInteractable(this.gameObject.transform, this);
        spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        buttonsEnabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnInteraction()
    {
        if (!isMajorAction || !GameManager.getInstance().MajorActionDone())
        {
            TextboxManager.GetInstance().SetText(message, this);
        }
        else
        {
            TextboxManager.GetInstance().SetText("You're too tired to do this right now.<page> Go home and sleep.", this);
            buttonsEnabled = false;
        }

        TextboxManager.GetInstance().TextBoxActive(true);
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

    public abstract void OnButtonPressed(int buttonCode);

    public void OnTextEnded()
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
    
    }
}
