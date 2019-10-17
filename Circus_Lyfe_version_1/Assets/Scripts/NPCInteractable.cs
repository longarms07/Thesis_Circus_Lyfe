using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteractable : MonoBehaviour, IInteractable
{
    private SpriteRenderer spriteRenderer;
    public Sprite defaultSprite;
    public Sprite inRangeSprite;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.getInstance().RegisterInteractable(this.gameObject.transform, this);
        spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnInteraction()
    {
        TextboxManager.GetInstance().SetText("Hi, my name is Donna!<page>Welcome to Circus Lyfe Version 1.<page>Tap on the squares to change their color.");
        TextboxManager.GetInstance().TextBoxActive(true);
    }
    public void InRange(bool inRange)
    {
        if (inRange)
        {
            spriteRenderer.sprite = inRangeSprite;
        }
        else
        {
            spriteRenderer.sprite = defaultSprite;
        }
    }
}
