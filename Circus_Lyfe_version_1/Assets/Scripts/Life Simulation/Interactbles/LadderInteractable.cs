using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderInteractable : YesNoInteractable
{
    public int trustPoints;

    private void Awake()
    {
        isMajorAction = true;
    }

    override
    public void OnButtonPressed(string answer)
    {
        //TextboxManager.GetInstance().DespawnTextButtons();
        if (answer == "yes")
        {
            gm.MajorActionCompleted(false);
            gm.GetPlayerManager().increaseTrustDonna(trustPoints);
            gm.ChangeSceneTrapeze();
        }
    }
}
