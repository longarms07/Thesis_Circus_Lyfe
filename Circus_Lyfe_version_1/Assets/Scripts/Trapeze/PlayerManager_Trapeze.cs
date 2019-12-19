using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerManager_Trapeze : BodyManager
{

    [Tooltip("The distance the player can fall before respawning at the initial trapeze")]
    public int fallDis;
    public float moveTowardsDist;
    private int hasFallen = 0;

    public Vector3 jumpForce;

    public GameObject attachedTo;

    public float grabTargetXRange;

    public bool goingRight = false;

    public float shortArmForce;
    public float shortLegForce;
    public float shortHeadForce;
    public float longLegForce;
    public float grabRange;
    public float targetMoveSpeed;


    private DistanceJoint2D joint;
    private DistanceJoint2D targetJoint;
    private GameObject target;
    public EnumPTrapezeState state = EnumPTrapezeState.NONE;
    private List<IPTrapezeStateListener> listeners;
    private Rigidbody2D rb;
    private DistanceJoint2D initJoint;
    private DistanceJoint2D lastJoint;
    private Quaternion initRot;

    void Awake()
    {
        listeners = new List<IPTrapezeStateListener>();
        rb = GetComponent<Rigidbody2D>();
        if (jumpForce == null) jumpForce = new Vector3(0, 0, 0);
        initRot = transform.rotation;

    }

    // Start is called before the first frame update
    void Start()
    {
        InitRBs();
        AttachTo(attachedTo.GetComponent<DistanceJoint2D>());
        lowerLegsRB.AddForce(10 * Vector2.right, ForceMode2D.Impulse);
        //TurnAround();
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
            if (target != null)
            {
                if (target.transform.position.y > headRB.position.y)
                {
                    target = null;
                    targetJoint = null;
                }
                else
                {
                    torsoRB.MovePosition(Vector2.MoveTowards(torsoRB.position, target.transform.position, targetMoveSpeed * Time.deltaTime));
                    upperArmsRB.MovePosition(Vector2.MoveTowards(upperArmsRB.position, target.transform.position, targetMoveSpeed * Time.deltaTime));
                    if (Mathf.Abs(upperArmsRB.position.x - target.transform.position.x) <= grabRange
                        && Mathf.Abs(upperArmsRB.position.y - target.transform.position.y) <= grabRange)
                    {
                        AttachTo(targetJoint);
                    }
                }
            }
            hasFallen++;
            if (hasFallen >= fallDis) AttachToInitial();  
        }
        if (state == EnumPTrapezeState.OnTrapeze)
        {
        }
        
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

    public void Target(DistanceJoint2D joint)
    {
        if (joint != null)
        {
            targetJoint = joint;
            target = joint.gameObject;
        }
    }

    public bool AttachTo(DistanceJoint2D joint2D)
    {
        if (joint2D == null || joint2D.connectedBody != null) return false;
        if(hasFallen>=fallDis) MassTeleport(joint2D.gameObject.transform.position);
        target = null;
        targetJoint = null;
        hasFallen = 0;
        ClearForce();
        if (initJoint == null)
        {
            initJoint = joint2D;
            lastJoint = joint2D;
        }
        joint = joint2D;
        joint.enabled = true;
        joint.connectedBody = upperArmsRB;
        SetState(EnumPTrapezeState.OnTrapeze);
        if (joint != lastJoint) TurnAround();
        ClearForce();
        return true;
    }

    public bool AttachToInitial()
    {
        return AttachTo(initJoint);
    }

    public bool Deattach()
    {
        if (joint == null) return false;
        joint.connectedBody = null;
        joint.enabled = false;
        lastJoint = joint;
        joint = null;
        SetState(EnumPTrapezeState.InAir);
        return true;
    }

    public bool Jump()
    {
        if (!Deattach()) return false;
        torsoRB.AddForce(jumpForce, ForceMode2D.Impulse);
        return true;
    }

    public void Short()
    {

        Debug.Log("Short");
        Vector2 hor;
        if (facingRight) hor = Vector2.right;
        else hor = Vector2.left;

        if (GoingRight()) {
            torsoRB.MovePosition(Vector2.MoveTowards(torsoRB.position, shortArmForce * (Vector2.up + Vector2.right), moveTowardsDist * Time.deltaTime));
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
            Vector2 headMoveTo = Vector2.MoveTowards(armsRB.position, Vector2.left*shortHeadForce, moveTowardsDist * Time.deltaTime);
            torsoRB.MovePosition(headMoveTo);
        }
            
    }


    public void Long()
    {
        Debug.Log("Long");
        Vector2 hor;
        if (!facingRight) hor = Vector2.right;
        else hor = Vector2.left;
    
        Vector2 legsMoveTo = Vector2.MoveTowards(lowerLegsRB.position, longLegForce * (Vector2.down + hor), moveTowardsDist * Time.deltaTime);
        lowerLegsRB.MovePosition(legsMoveTo);
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

    

    
}
