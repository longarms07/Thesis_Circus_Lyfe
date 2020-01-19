using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class Donna : NPCInteractable, IButtonListener
{
    
    override
    public void OnInteraction()
    {

        FindObjectOfType<DialogueRunner>().StartDialogue(ChooseDialogNode());
        /*
        TextboxManager.GetInstance().SetText("Hi, my name is Donna!<page>Welcome to Circus Lyfe Version 0.3.", this);
        TextboxManager.GetInstance().TextBoxActive(true);*/
    }

    private string ChooseDialogNode()
    {
        List<string> toChose = schedule[gm.currentDay][gm.currentTime].dialogNodes;
        string chosen = toChose[Random.Range(0, toChose.Count)];
        return chosen;
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
        /*string[] text = new string[2];
        text[0] = "Yes";
        text[1] = "No";
        int[] codes = new int[2];
        codes[0] = 0;
        codes[1] = 1;
        TextboxManager.GetInstance().GenerateTextButtons(this, text, codes);*/
        //OnButtonPressed(0);
    }
}
