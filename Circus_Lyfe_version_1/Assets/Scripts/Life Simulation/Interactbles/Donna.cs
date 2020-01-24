using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class Donna : NPCInteractable, IButtonListener
{

    private bool followingPlayer = false;
    public string followingPlayerNode;
    public string startDialogNode;
    private bool attemptTalking = false;

    void Update()
    {
        if (attemptTalking)
        {
            if (!dialogueRunner.isDialogueRunning)
            {
                attemptTalking = false;
                TalkToDonna();
            }
        }
    }

   protected override void AddToStart()
    {
        savefile = "DonnaStats.save";
    }

    public override float GetTrustLevel()
    {
        return gm.GetPlayerManager().getTrustDonna();
    }

    override
    public void OnInteraction()
    {
        if (!followingPlayer)
        {
            //TextboxManager.GetInstance().NotifyMe(this);
            dialogueRunner.StartDialogue(startDialogNode);
        }
        else TalkToDonna();
        /*
        TextboxManager.GetInstance().SetText("Hi, my name is Donna!<page>Welcome to Circus Lyfe Version 0.3.", this);
        TextboxManager.GetInstance().TextBoxActive(true);*/
    }
    
    public void TalkToDonna()
    {
        dialogueRunner.StartDialogue(ChooseDialogNode());
    }

    private string ChooseDialogNode()
    {
        if (followingPlayer) return followingPlayerNode;
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

    public override void AddToDayTimeChange(DayEnums newDay, TimeEnums newTime)
    {
        yarnVars.SetValue("$DonnaInvitable", schedule[newDay][newTime].invitable);
    }

    [YarnCommand("DFollowPlayer")]
    public void FollowPlayer(string s)
    {
        if (s.Equals("true") && !followingPlayer)
        {
            followingPlayer = true;
            gm.GetPlayerMovable().NewFollower(rb);
        }
        else if(s.Equals("false") && followingPlayer)
        {
            followingPlayer = false;
            gm.GetPlayerMovable().StopFollower();
        }
    }
    
    override
    public void OnTextEnded()
    { }

    [YarnCommand("DAttemptTalk")]
    public void AttemptTalking()
    {
        if(!followingPlayer)
        {
            //TalkToDonna();
            attemptTalking = true;
        }
    }



}
