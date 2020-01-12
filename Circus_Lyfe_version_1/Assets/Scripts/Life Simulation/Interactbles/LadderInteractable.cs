﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderInteractable : YesNoInteractable
{
    private void Awake()
    {
            message = "Would you like to practice Trapeze?";
    }

    override
    public void OnButtonPressed(int buttonCode)
    {
        TextboxManager.GetInstance().DespawnTextButtons();
        if (buttonCode == 0)
        {
            GameManager.getInstance().ChangeSceneTrapeze();
        }
    }
}
