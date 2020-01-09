using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NPCInteractable : MonoBehaviour, IInteractable
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

    public abstract void OnInteraction();
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


    public abstract void OnTextEnded();
}
