using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderInteractable : YesNoInteractable
{
    public int trustPoints;

    private void Awake()
    {
        message = "Would you like to practice Trapeze?";
        isMajorAction = true;
    }

    override
    public void OnButtonPressed(int buttonCode)
    {
        TextboxManager.GetInstance().DespawnTextButtons();
        if (buttonCode == 0)
        {
            gm.MajorActionCompleted(false);
            gm.GetPlayerManager().increaseTrustDonna(trustPoints);
            gm.ChangeSceneTrapeze();
        }
    }
}
