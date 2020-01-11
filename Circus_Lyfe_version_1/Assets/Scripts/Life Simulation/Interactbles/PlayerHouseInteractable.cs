using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHouseInteractable : YesNoInteractable
{

    void Start()
    {
        message = "Would you like to go home and sleep?";
        GameManager.getInstance().RegisterInteractable(this.gameObject.transform, this);
        spriteRenderer = this.gameObject.GetComponentInParent<SpriteRenderer>();
    }

    override
    public void OnButtonPressed(int buttonCode)
    {
        TextboxManager.GetInstance().DespawnTextButtons();
        if (buttonCode == 0)
        {
            GameManager.getInstance().IncrementDay();
        }
    }
}
