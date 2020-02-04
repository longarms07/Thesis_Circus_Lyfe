using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabTarget : MonoBehaviour, IInteractable
{
    public Sprite activeSprite;
    public GameObject inactiveObject;
    public float angleDegrees;
    //private Sprite inactiveSprite;
    //public Vector3 activeSize;
    //private Vector3 inactiveSize;
    private SpriteRenderer inactiveSpriteRenderer;
    protected PlayerManager_Trapeze pm;

    protected bool inRange;
    public Joint2D joint;
    private SpriteRenderer spriteRenderer;
    private CircleCollider2D activeCollider;

    // Start is called before the first frame update
    void Start()
    {
        inRange = false;
        //joint = this.gameObject.GetComponent<DistanceJoint2D>();
        //joint = inactiveObject.GetComponent<Joint2D>();
        GameManager_Trapeze.GetInstance().RegisterInteractable(gameObject.transform, this);
        //inactiveSize = gameObject.transform.localScale;
        spriteRenderer = GetComponent<SpriteRenderer>();
        //inactiveSprite = spriteRenderer.sprite;
        inactiveSpriteRenderer = inactiveObject.GetComponent<SpriteRenderer>();
        pm = GameManager_Trapeze.GetInstance().playerAvatar.GetComponent<PlayerManager_Trapeze>();
        activeCollider = GetComponent<CircleCollider2D>();
        InRange(false);
    }

    // Update is called once per frame
    void Update()
    {
        CalculateAngleDegrees();
        if (pm.state == EnumPTrapezeState.InAir && joint.connectedBody == null)
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

    public virtual void OnInteraction()
    {
        if (inRange)
        {
            GameManager_Trapeze.GetInstance().GetPlayerManager().Target(this);
        }
    }

    public virtual void InRange(bool inRange)
    {
        this.inRange = inRange;
        if (inRange)
        {
            //spriteRenderer.sprite = activeSprite;
            //gameObject.transform.localScale = activeSize;
            spriteRenderer.enabled = true;
            inactiveSpriteRenderer.enabled = false;
            activeCollider.enabled = true;

        }
        else
        {
            //spriteRenderer.sprite = inactiveSprite;
            //gameObject.transform.localScale = inactiveSize;
            spriteRenderer.enabled = false;
            inactiveSpriteRenderer.enabled = true;
            activeCollider.enabled = false;
        }
    }

    protected virtual void CalculateAngleDegrees()
    {
        angleDegrees = Mathf.Atan2(this.transform.localPosition.x, this.transform.localPosition.y) * Mathf.Rad2Deg;
    }

}
