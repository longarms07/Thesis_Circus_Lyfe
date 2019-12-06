using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Donna : NPCInteractable, IButtonListener
{
    
    override
    public void OnInteraction()
    {
        TextboxManager.GetInstance().SetText("Hi, my name is Donna!<page>Welcome to Circus Lyfe Version 1.<page>Tap on the squares to change their color.<page>Would you like to practice Trapeze?", this);
        TextboxManager.GetInstance().TextBoxActive(true);
    }

    public void OnButtonPressed(int buttonCode)
    {
        TextboxManager.GetInstance().DespawnTextButtons();
        if(buttonCode == 0)
        {
            GameManager.getInstance().ChangeSceneTrapeze();
        }
    }
    
    override
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
