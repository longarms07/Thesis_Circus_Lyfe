using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableTest : MonoBehaviour, IInteractable
{
    private SpriteRenderer spriteRenderer;
    Color Red = new Color(255, 0, 0);
    Color Pink = new Color(255, 0, 255);
    Color Blue = new Color(0, 0, 255);
    bool red = true;


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
        Debug.Log(this.gameObject.name + " has been interacted with");
        if (!red)
        {
            spriteRenderer.color = Red;
            red = true;
        }
        else
        {
            spriteRenderer.color = Blue;
            red = false;
        }
    }
    public void InRange(bool inRange)
    {
        if (inRange)
        {
            Debug.Log(this.gameObject.name + "Is in Range of Player");
            spriteRenderer.color = Pink;
        }
        else { Debug.Log(this.gameObject.name + "Is not in Range of Player");
            if (red) spriteRenderer.color = Red;
            else spriteRenderer.color = Blue;
        }
    }

}
