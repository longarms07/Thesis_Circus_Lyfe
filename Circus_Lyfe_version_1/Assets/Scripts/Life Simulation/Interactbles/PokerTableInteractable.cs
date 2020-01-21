using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokerTableInteractable : YesNoInteractable
{

    public string pokerNodeFollower;
    public GameObject follower;
    private bool attemptTalking = false;

    private void Awake()
    {
        reqFollower = true;
        isMajorAction = true;
    }

    private void Update()
    {
        if (attemptTalking)
        {
            if (!dialogueRunner.isDialogueRunning)
            {
                attemptTalking = false;
                dialogueRunner.StartDialogue(pokerNodeFollower);
                gm.MajorActionDone();
                gm.GetPlayerMovable().StopFollower();
            }
        }
    }

    override
    public void OnButtonPressed(string answer)
    {
        if (answer == "yes" && gm.GetPlayerMovable().GetFollower().gameObject == follower)
        {
            attemptTalking = true;
        }
    }

}
