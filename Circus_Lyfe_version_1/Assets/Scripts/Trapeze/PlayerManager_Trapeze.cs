using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    protected GameManager_Trapeze gm;

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


    void Awake()
    {
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
            if (target.transform.position.y > headRB.position.y)
            {
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
                    AttachTo(targetGrabTarget);
                }
            }
        }
        if (jumpX - headRB.position.y >= fallDis) AttachToInitial();
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
        }
    }

    public bool AttachTo(GrabTarget gt)
    {
        DistanceJoint2D joint2D = gt.joint;
        if (joint2D == null || joint2D.connectedBody != null) return false;
        if (gm.IsSloMoAllowed() && gm.InSloMo())
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
        if (grabTarget != lastGrabTarget) TurnAround();
        return true;
    }

    public bool AttachToInitial()
    {
        return AttachTo(initGrabTarget);
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

    public bool Jump()
    {
        if (!Deattach()) return false;
        if (GameManager_Trapeze.GetInstance().IsSloMoAllowed()) GameManager_Trapeze.GetInstance().ToggleSloMo();
        Vector2 dir = new Vector2(1,1);
        jumpX = headRB.position.y;
        if (!facingRight) dir.x = -1;
        torsoRB.AddForce(dir*jumpForce, ForceMode2D.Impulse);
        return true;
    }

    public void Short()
    {
        if (timerSL >= cooldownTimeSL)
        {
            Debug.Log(gameObject.name+" Short Degrees: " +grabTarget.angleDegrees + " Going Right? "+goingRight);
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
            timerSL = 0;
        }
            
    }


    public void Long()
    {
        if (timerSL >= cooldownTimeSL)
        {
            Debug.Log(gameObject.name + " Long Degrees: " + grabTarget.angleDegrees);
            Vector2 hor;
            if (!facingRight) hor = Vector2.right;
            else hor = Vector2.left;

            Vector2 legsMoveTo = Vector2.MoveTowards(lowerLegsRB.position, longLegForce * (Vector2.down + hor), moveTowardsDist * Time.deltaTime);
            lowerLegsRB.MovePosition(legsMoveTo);
            timerSL = 0;
        }
    }

    public bool FacingRight()
    {
        return facingRight;
    }

    public bool GoingRight()
    {
        float totalVelocity = headRB.velocity.x + torsoRB.velocity.x + upperArmsRB.velocity.x
            + armsRB.velocity.x + lowerLegsRB.velocity.x + upperLegsRB.velocity.x;
        if (headRB.velocity.x > 0) return true;
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

    public void DoAnimation(string animName)
    {
        if (gm.IsSloMoAllowed() && gm.InSloMo())
        {
            //gm.ToggleSloMo();
            animator.speed = animator.speed * 2;
        }
        transform.position = head.transform.position + offset;
        state = EnumPTrapezeState.Trick;
        //ResetRotation();
        SetKinematic(true);
        animator.enabled = true;
        animator.SetTrigger(animName);

    }

    public void AnimationEnded()
    {
        if (gm.IsSloMoAllowed() && gm.InSloMo())
        {
            //gm.ToggleSloMo();
            animator.speed = animator.speed / 2;
        }
        animator.enabled = false;
        //ResumeRotation();
        //KillClones();
        SetKinematic(false);
        this.gameObject.transform.position = position;
        this.gameObject.transform.localPosition = position;
        state = EnumPTrapezeState.InAir;
    }
    



}
