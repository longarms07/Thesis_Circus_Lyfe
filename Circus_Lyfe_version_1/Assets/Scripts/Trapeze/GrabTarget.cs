using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabTarget : MonoBehaviour, IInteractable
{
    public Sprite activeSprite;
    private Sprite inactiveSprite;
    public Vector3 activeSize;
    private Vector3 inactiveSize;

    private bool inRange;
    private DistanceJoint2D joint;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        inRange = false;
        joint = this.gameObject.GetComponent<DistanceJoint2D>();
        GameManager_Trapeze.GetInstance().RegisterInteractable(gameObject.transform, this);
        inactiveSize = gameObject.transform.localScale;
        spriteRenderer = GetComponent<SpriteRenderer>();
        inactiveSprite = spriteRenderer.sprite;
        //InRange(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnInteraction()
    {
        if (inRange)
        {
            GameManager_Trapeze.GetInstance().GetPlayerManager().AttachTo(joint);
        }
    }

    public void InRange(bool inRange)
    {
        this.inRange = inRange;
        if (inRange)
        {
            spriteRenderer.sprite = activeSprite;
            gameObject.transform.localScale = activeSize;

        }
        else
        {
            spriteRenderer.sprite = inactiveSprite;
            gameObject.transform.localScale = inactiveSize;
        }
    }


}
