using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHouseInteractable : YesNoInteractable
{
    
    void Start()
    {
        DoAtStart();
        //yarnNode = "YesNoInteractables.PlayerHouse";
        spriteRenderer = this.gameObject.GetComponentInParent<SpriteRenderer>();
    }

    override
    public void OnButtonPressed(string answer)
    {
        //TextboxManager.GetInstance().DespawnTextButtons();
        if (answer == "yes")
        {
            GameManager.getInstance().IncrementDay();
        }
    }
}
