using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager_Trapeze : BodyManager
{

    [Tooltip("The distance the player can fall before respawning at the initial trapeze")]
    public int fallDis;
    public float moveTowardsDist;
    private int hasFallen = 0;

    public Vector3 jumpForce;

    public GameObject attachedTo;


    private DistanceJoint2D joint;
    public EnumPTrapezeState state = EnumPTrapezeState.NONE;
    private List<IPTrapezeStateListener> listeners;
    private Rigidbody2D rb;
    private DistanceJoint2D initJoint;
    private Quaternion initRot;
    private bool doLong;
    private bool facingRight = true;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (centerOfGravity != null)
        {
            //Debug.Log(GetCenterOfMass());
            centerOfGravity.transform.position = GetCenterOfMass();
        }
        if (state == EnumPTrapezeState.InAir)
        {
            hasFallen++;
            if (hasFallen >= fallDis) AttachToInitial();
        }
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

    public bool AttachTo(DistanceJoint2D joint2D)
    {
        if (joint2D == null || joint2D.connectedBody != null) return false;
        //rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        hasFallen = 0;
        joint = joint2D;
        joint.connectedBody = upperArmsRB;
        //joint.connectedBody = rb;
        //ClearForce();
        //rb.constraints = RigidbodyConstraints2D.None;
        SetState(EnumPTrapezeState.OnTrapeze);
        if (initJoint == null) initJoint = joint;
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
        joint = null;
        SetState(EnumPTrapezeState.InAir);
        return true;
    }

    public bool Jump()
    {
        if (!Deattach()) return false;
        transform.rotation = initRot;
        //rb.AddForce(jumpForce, ForceMode2D.Impulse);
        return true;
    }

    public void Short()
    {
        Debug.Log("Short");
        Vector2 hor;
        if (facingRight) hor = Vector2.right;
        else hor = Vector2.left;

        Vector2 armsMoveTo = Vector2.MoveTowards(armsRB.position, 100*(hor+Vector2.down), moveTowardsDist * Time.deltaTime);
        armsRB.MovePosition(armsMoveTo);
        Vector2 legsMoveTo = Vector2.MoveTowards(lowerLegsRB.position, 100 * (Vector2.up+hor), moveTowardsDist * Time.deltaTime);
        lowerLegsRB.MovePosition(legsMoveTo);
            
    }


    public void Long()
    {
        Debug.Log("Long");
        Vector2 hor;
        if (!facingRight) hor = Vector2.right;
        else hor = Vector2.left;
    
        Vector2 legsMoveTo = Vector2.MoveTowards(lowerLegsRB.position, 100 * (Vector2.down + hor), moveTowardsDist * Time.deltaTime);
        lowerLegsRB.MovePosition(legsMoveTo);
    }


    public void ClearForce()
    {
        /*rb.velocity = Vector3.zero;
        rb.angularVelocity = 0;
        transform.rotation = initRot;
        rb.rotation = 0;*/
    }

}
