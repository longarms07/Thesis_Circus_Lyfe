using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabTarget : MonoBehaviour, IInteractable
{
    public Sprite activeSprite;
    public GameObject inactiveObject;
    //private Sprite inactiveSprite;
    //public Vector3 activeSize;
    //private Vector3 inactiveSize;
    private SpriteRenderer inactiveSpriteRenderer;
    private PlayerManager_Trapeze pm;

    private bool inRange;
    private DistanceJoint2D joint;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        inRange = false;
        //joint = this.gameObject.GetComponent<DistanceJoint2D>();
        joint = inactiveObject.GetComponent<DistanceJoint2D>();
        GameManager_Trapeze.GetInstance().RegisterInteractable(gameObject.transform, this);
        //inactiveSize = gameObject.transform.localScale;
        spriteRenderer = GetComponent<SpriteRenderer>();
        //inactiveSprite = spriteRenderer.sprite;
        inactiveSpriteRenderer = inactiveObject.GetComponent<SpriteRenderer>();
        pm = GameManager_Trapeze.GetInstance().playerAvatar.GetComponent<PlayerManager_Trapeze>();
        InRange(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(pm.state == EnumPTrapezeState.InAir)
        {

            if (pm.head.transform.position.y > this.transform.position.y &&
                    (pm.FacingRight() && pm.head.gameObject.transform.position.x < this.transform.position.x 
                    && this.transform.position.x < pm.head.gameObject.transform.position.x+pm.grabTargetXRange)
                    || (!pm.FacingRight() && pm.head.gameObject.transform.position.x > this.transform.position.x
                    && this.transform.position.x > pm.head.gameObject.transform.position.x - pm.grabTargetXRange))
            {
               InRange(true);
            }
            else
            {
               InRange(false);
            }
            //InRange(true);

        }
        else
        {
            InRange(false);
        }
    }

    public void OnInteraction()
    {
        if (inRange)
        {
            GameManager_Trapeze.GetInstance().GetPlayerManager().Target(joint);
        }
    }

    public void InRange(bool inRange)
    {
        this.inRange = inRange;
        if (inRange)
        {
            //spriteRenderer.sprite = activeSprite;
            //gameObject.transform.localScale = activeSize;
            spriteRenderer.enabled = true;
            inactiveSpriteRenderer.enabled = false;

        }
        else
        {
            //spriteRenderer.sprite = inactiveSprite;
            //gameObject.transform.localScale = inactiveSize;
            spriteRenderer.enabled = false;
            inactiveSpriteRenderer.enabled = true;
        }
    }


}
