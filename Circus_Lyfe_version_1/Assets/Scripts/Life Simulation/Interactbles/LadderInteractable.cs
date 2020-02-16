using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderInteractable : YesNoInteractable
{
    public int trustPoints;
    public string inviteDonnaNode;
    public int trustLevelReq;
    public Donna donna;
    protected bool ask2;

    private void Awake()
    {
        isMajorAction = true;
        ask2 = false;
    }

    private void Update()
    {
        if (ask2 && !dialogueRunner.isDialogueRunning)
        {
            dialogueRunner.StartDialogue(inviteDonnaNode);
        }
    }

    override
    public void OnButtonPressed(string answer)
    {
        if (ask2 == false)
        {
            //TextboxManager.GetInstance().DespawnTextButtons();
            if (answer == "yes")
            {
                if (trustLevelReq > gm.GetPlayerManager().trustDonna || donna.transform.position != donnaSpawnPosition.transform.position)
                {
                    //Debug.Log("Starting trapeze: "+(trustLevelReq < gm.GetPlayerManager().trustDonna) + " , " + (donna.transform.position != donnaSpawnPosition.transform.position));
                    StartTrapeze();
                }
                else
                {
                    Debug.Log("here");
                    ask2 = true;
                }
            }
        }
        else
        {
            ask2 = false;
            if (answer == "yes") gm.AllowDonnaOnTrapeze(true);
            StartTrapeze();
        }
    }

    private void StartTrapeze()
    {
        gm.MajorActionCompleted(false);
        gm.GetPlayerManager().increaseTrustDonna(trustPoints);
        gm.ChangeSceneTrapeze();
    }
}
