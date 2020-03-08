using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using System;

public class PlayerManager_Trapeze : BodyManager
{

    [Tooltip("The distance the player can fall before respawning at the initial trapeze")]
    public int fallDis;
    public float moveTowardsDist;

    public Vector3 jumpForce;

    public GrabTarget attachedTo;

    public float grabTargetXRange;

    public bool goingRight = false;

    public float shortArmForce;
    public float shortLegForce;
    public float shortHeadForce;
    public float longLegForce;
    public float grabRange;
    public float targetMoveSpeed;
    public float cooldownTimeSL;
    public bool canSloMo = true;
    public float grabRangeTargetingOffest;
    public FixedJoint2D playerJoint;
    public GrabTarget legGrabTarget;

    protected GameManager_Trapeze gm;

    private bool initFacingRight;
    protected float timerSL;
    protected GrabTarget grabTarget;
    protected GrabTarget targetGrabTarget;
    protected GameObject target;
    public EnumPTrapezeState state = EnumPTrapezeState.NONE;
    protected List<IPTrapezeStateListener> listeners;
    protected Rigidbody2D rb;
    protected GrabTarget initGrabTarget;
    protected GrabTarget lastGrabTarget;
    protected Quaternion initRot;

    protected Vector3 offset;
    protected TrickManager tm;
    protected Vector3 position;
    protected Quaternion rotation;
    protected float jumpX = 0;
    protected string lastTrick;
    protected bool punishFall = true;
    


    void Awake()
    {
        initFacingRight = facingRight;
        listeners = new List<IPTrapezeStateListener>();
        rb = GetComponent<Rigidbody2D>();
        if (jumpForce == null) jumpForce = new Vector3(0, 0, 0);
        initRot = transform.rotation;
        offset = transform.position - head.transform.position;
        position = transform.position;
        rotation = transform.rotation;
        timerSL = cooldownTimeSL;

    }

    // Start is called before the first frame update
    void Start()
    {
        DoAtStart(); 
    }


    protected void DoAtStart() {
        gm = GameManager_Trapeze.GetInstance();
        InitRBs();
        if (!facingRight)
        {
            facingRight = true;
            TurnAround();
        }
        AttachTo(attachedTo);
        lowerLegsRB.AddForce(10 * Vector2.right, ForceMode2D.Impulse);
        tm = TrickManager.GetInstance();
        animator.enabled = false;
        //TurnAround();
    }

        private void FixedUpdate()
    {
        timerSL += Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {

        goingRight = GoingRight();
        if (centerOfGravity != null)
        {
            //Debug.Log(GetCenterOfMass());
            centerOfGravity.transform.position = GetCenterOfMass();
        }
        if (state == EnumPTrapezeState.InAir)
        {
            DoInAir();
        }

    }

    protected void DoInAir()
    {
        if (target != null)
        {
            if (target.transform.position.y > headRB.position.y || 
                (goingRight && target.transform.position.x+ grabRangeTargetingOffest < torsoRB.position.x)
                || (!goingRight && target.transform.position.x- grabRangeTargetingOffest > torsoRB.position.x))
            {
                Debug.Log(gameObject.name + " gave up on targeting " + targetGrabTarget.gameObject.name);
                if (target.transform.position.y > headRB.position.y)
                    Debug.Log("Because target.transform.position.y "+ target.transform.position.y+" > headRB.position.y "+ headRB.position.y);
                else if (goingRight && target.transform.position.x + grabRangeTargetingOffest < torsoRB.position.x)
                    Debug.Log("Because goingRight && target.transform.position.x + grabRangeTargetingOffest < torsoRB.position.x");
                else
                    Debug.Log("Because !goingRight && target.transform.position.x-grabRangeTargetingOffest > torsoRB.position.x");
                target = null;
                targetGrabTarget = null;
            }
            else
            {
                torsoRB.MovePosition(Vector2.MoveTowards(torsoRB.position, target.transform.position, targetMoveSpeed * Time.deltaTime));
                upperArmsRB.MovePosition(Vector2.MoveTowards(upperArmsRB.position, target.transform.position, targetMoveSpeed * Time.deltaTime));
                if (Mathf.Abs(upperArmsRB.position.x - target.transform.position.x) <= grabRange
                    && Mathf.Abs(upperArmsRB.position.y - target.transform.position.y) <= grabRange)
                {
                    Debug.Log(gameObject.name + " is attempting to attach to " + targetGrabTarget.gameObject.name );//+ " returned "+ AttachTo(targetGrabTarget));
                    AttachTo(targetGrabTarget);
                }
            }
        }
        if (jumpX - headRB.position.y >= fallDis)
        {
            if(punishFall) TrickGUI.GetInstance().DecreaseScore(10);
            AttachToInitial();
        }
        //Debug.Log(jumpX - headRB.position.x);
    }

    public bool HasTarget()
    {
        if (target == null) return false;
        return true;
    }


    public void SetState(EnumPTrapezeState s)
    {
        state = s;
        foreach (IPTrapezeStateListener listener in listeners)
            listener.OnPlayerStateChange(state);
    }

    public EnumPTrapezeState GetState()
    {
        return state;
    }

    public bool SubscribeStateListener(IPTrapezeStateListener listener)
    {
        if (listeners.Contains(listener)) return false;
        listeners.Add(listener);
        return true;
    }

    public bool UnsubscribeStateListener(IPTrapezeStateListener listener)
    {
        if (listeners.Contains(listener))
        {
            listeners.Remove(listener);
            return true;
        }
        return false;
    }

    public void Target(GrabTarget grabTarget)
    {
        if (grabTarget != null)
        {
            targetGrabTarget = grabTarget;
            target = grabTarget.gameObject;
            //Debug.Log("Targeting " + target.name);
        }
    }


    public Action OnAttachTo = () => { };
    public virtual bool AttachTo(GrabTarget gt, bool turnAround=true)
    {
        Joint2D joint2D = gt.joint;
        /*Debug.Log("Joint2d is null?" + (joint2D == null));
        Debug.Log("Connected body is not null?" + (joint2D.connectedBody != null));*/
        if (joint2D == null || joint2D.connectedBody != null) return false;
        if (canSloMo && gm.IsSloMoAllowed() && gm.InSloMo())
        {
            gm.ToggleSloMo();
        }
        if (jumpX - headRB.position.y >= fallDis)
        {
            MassTeleport(joint2D.gameObject.transform.position);
        }
        ClearForce();
        target = null;
        targetGrabTarget = null;
        jumpX = 0;
        if (initGrabTarget == null)
        {
            initGrabTarget = gt;
            lastGrabTarget = gt;
        }
        grabTarget = gt;
        joint2D.enabled = true;
        joint2D.connectedBody = upperArmsRB;
        SetState(EnumPTrapezeState.OnTrapeze);
        if (grabTarget != lastGrabTarget) OnAttachTo();
        if (turnAround && grabTarget != lastGrabTarget) TurnAround();
        return true;
    }

    public bool AttachToInitial()
    {
        if (initGrabTarget.joint.connectedBody != null)
        {
            PlayerManager_Trapeze pm = initGrabTarget.joint.connectedBody.gameObject.GetComponentInParent<PlayerManager_Trapeze>();
            if(pm.facingRight != facingRight) return AttachTo(pm.legGrabTarget);
            return AttachTo(pm.legGrabTarget, false);
        }
        bool turn = true;
        if (lastGrabTarget != initGrabTarget && facingRight == initFacingRight) turn = false;
        return AttachTo(initGrabTarget, turn);
    }

    public bool Deattach()
    {
        if (grabTarget == null) return false;
        grabTarget.joint.connectedBody = null;
        grabTarget.joint.enabled = false;
        lastGrabTarget = grabTarget;
        grabTarget = null;
        SetState(EnumPTrapezeState.InAir);
        return true;
    }

    public Action OnJump = () => { };
    public virtual bool Jump()
    {
        if (playerJoint.connectedBody != null) return false; 
        Debug.Log(gameObject.name + "Jumped at " + grabTarget.angleDegrees);
        if (!Deattach()) return false;
        if (canSloMo && GameManager_Trapeze.GetInstance().IsSloMoAllowed()) GameManager_Trapeze.GetInstance().ToggleSloMo();
        Vector2 dir = new Vector2(1,1);
        jumpX = headRB.position.y;
        if (!facingRight) dir.x = -1;
        torsoRB.AddForce(dir*jumpForce, ForceMode2D.Impulse);
        OnJump();
        return true;
    }

    public Action OnShort = ()=>{};
    public void Short()
    {
        if (timerSL >= cooldownTimeSL)
        {
            //Debug.Log(gameObject.name+" Short Degrees: " +grabTarget.angleDegrees + " Going Right? "+goingRight);
            Vector2 hor;
            if (facingRight) hor = Vector2.right;
            else hor = Vector2.left;

            if ((GoingRight() && facingRight) || (!GoingRight() && !facingRight))
            {
                torsoRB.MovePosition(Vector2.MoveTowards(torsoRB.position, shortArmForce * (Vector2.up + hor), moveTowardsDist * Time.deltaTime));
                Vector2 armsMoveTo = Vector2.MoveTowards(armsRB.position, shortArmForce * (-hor + Vector2.down), moveTowardsDist * Time.deltaTime);
                armsRB.MovePosition(armsMoveTo);
                Vector2 legsMoveTo = Vector2.MoveTowards(lowerLegsRB.position, shortLegForce * (Vector2.up + hor), moveTowardsDist * Time.deltaTime);
                lowerLegsRB.MovePosition(legsMoveTo);
            }
            else
            {
                Vector2 armsMoveTo = Vector2.MoveTowards(armsRB.position, shortArmForce * (-hor + Vector2.down), moveTowardsDist * Time.deltaTime);
                armsRB.MovePosition(armsMoveTo);
                Vector2 legsMoveTo = Vector2.MoveTowards(lowerLegsRB.position, shortLegForce * (Vector2.up + hor), moveTowardsDist * Time.deltaTime);
                lowerLegsRB.MovePosition(legsMoveTo);
                Vector2 headMoveTo = Vector2.MoveTowards(armsRB.position, -hor * shortHeadForce, moveTowardsDist * Time.deltaTime);
                torsoRB.MovePosition(headMoveTo);
            }
            OnShort();
            timerSL = 0;
        }
            
    }


    public Action OnLong = () => { };
    public void Long()
    {
        if (timerSL >= cooldownTimeSL)
        {
           //Debug.Log(gameObject.name + " Long Degrees: " + grabTarget.angleDegrees);
            Vector2 hor;
            if (!facingRight) hor = Vector2.right;
            else hor = Vector2.left;

            Vector2 legsMoveTo = Vector2.MoveTowards(lowerLegsRB.position, longLegForce * (Vector2.down + hor), moveTowardsDist * Time.deltaTime);
            lowerLegsRB.MovePosition(legsMoveTo);
            OnLong();
            timerSL = 0;
        }
    }

    public bool FacingRight()
    {
        return facingRight;
    }

    public bool GoingRight()
    {
        if (state != EnumPTrapezeState.OnTrapeze) return goingRight;
        float totalVelocity = headRB.velocity.x + torsoRB.velocity.x + upperArmsRB.velocity.x
            + armsRB.velocity.x + lowerLegsRB.velocity.x + upperLegsRB.velocity.x;
        if (totalVelocity > 0) return true;
        return false;
    }

    public void ClearForce()
    {
        ClearForce(armsRB);
        ClearForce(upperArmsRB);
        ClearForce(headRB);
        ClearForce(torsoRB);
        ClearForce(lowerLegsRB);
        ClearForce(upperLegsRB);

    }

    public void ClearForce(Rigidbody2D rb)
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
        rb.rotation = 0;
    }


    public Action OnTrick = () => { };
    public void DoAnimation(string animName)
    {
       if (gm.IsSloMoAllowed() && gm.InSloMo() && animator.speed == 1)
        {
            //gm.ToggleSloMo();
            animator.speed = 2;
        }
        transform.position = head.transform.position + offset;
        state = EnumPTrapezeState.Trick;
        //ResetRotation();
        SetKinematic(true);
        animator.enabled = true;
        animator.SetTrigger(GetAnimName(animName));
        lastTrick = animName;

    }

    public void AnimationEnded()
    {
        if (animator.speed != 1)
        {
            //gm.ToggleSloMo();
            animator.speed = 1;
        }
        animator.enabled = false;
        //ResumeRotation();
        //KillClones();
        SetKinematic(false);
        this.gameObject.transform.position = position;
        this.gameObject.transform.localPosition = position;
        state = EnumPTrapezeState.InAir;
        OnTrick();
    }

    public virtual string GetAnimName(string animName)
    {
        return "p_" + animName;
    }
    
    public GrabTarget GetGrabTarget()
    {
        return grabTarget;
    }

    public string GetLastTrickPerformed()
    {
        return lastTrick;
    }
}
