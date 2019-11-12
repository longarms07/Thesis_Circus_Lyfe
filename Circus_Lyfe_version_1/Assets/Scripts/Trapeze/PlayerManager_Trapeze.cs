using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager_Trapeze : MonoBehaviour
{

    [Tooltip("The distance the player can fall before respawning at the initial trapeze")]
    public int fallDis;
    private int hasFallen = 0;

    public Vector3 jumpForce;

    public GameObject attachedTo;


    private HingeJoint2D joint;
    public EnumPTrapezeState state = EnumPTrapezeState.NONE;
    private List<IPTrapezeStateListener> listeners;
    private Rigidbody2D rb;
    private HingeJoint2D initJoint;
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
        AttachTo(attachedTo.GetComponent<HingeJoint2D>());
    }

    // Update is called once per frame
    void Update()
    {
        if(state == EnumPTrapezeState.InAir)
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

    public bool AttachTo(HingeJoint2D joint2D)
    {
        if (joint2D == null || joint2D.connectedBody != null) return false;
        ClearForce();
        hasFallen = 0;
        joint = joint2D;
        joint.connectedBody = rb;
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
        rb.AddForce(jumpForce, ForceMode2D.Impulse);
        return true;
    }

    public void ClearForce()
    {
        rb.velocity = new Vector2(0,0);
        rb.angularVelocity = 0;
        transform.rotation = initRot;
    }

}
