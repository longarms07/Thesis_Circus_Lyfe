using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class YesNoInteractable : MonoBehaviour, IInteractable, IButtonListener, ITextboxListener
{

     protected string message;
     protected SpriteRenderer spriteRenderer;
     public Sprite defaultSprite;
     public Sprite inRangeSprite;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.getInstance().RegisterInteractable(this.gameObject.transform, this);
        spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnInteraction()
    {
        TextboxManager.GetInstance().SetText(message, this);
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
        string[] text = new string[2];
        text[0] = "Yes";
        text[1] = "No";
        int[] codes = new int[2];
        codes[0] = 0;
        codes[1] = 1;
        TextboxManager.GetInstance().GenerateTextButtons(this, text, codes);
        //OnButtonPressed(0);
    }
}
