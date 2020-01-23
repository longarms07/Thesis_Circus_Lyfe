using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokerTableInteractable : YesNoInteractable
{

    public string pokerNodeFollower;
    public Donna follower;
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
                gm.MajorActionCompleted(true);
                follower.FollowPlayer("false");
            }
        }
    }

    override
    public void OnButtonPressed(string answer)
    {
        if (answer == "yes" && gm.GetPlayerMovable().GetFollower().gameObject == follower.gameObject)
        {
            attemptTalking = true;
        }
    }

}
