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

    public Vector2 boxcastSize;
    public float boxcastDistance;
    public float boxcastAngle;
    public Vector2 boxcastDir;
    public Vector2 boxcastOffset;
    


    private DistanceJoint2D joint;
    public EnumPTrapezeState state = EnumPTrapezeState.NONE;
    private List<IPTrapezeStateListener> listeners;
    private Rigidbody2D rb;
    private DistanceJoint2D initJoint;
    private Quaternion initRot;
    private bool doLong;
    private bool facingRight = true;
    private Collider2D[] nearbyInteractables;

    void Awake()
    {
        listeners = new List<IPTrapezeStateListener>();
        rb = GetComponent<Rigidbody2D>();
        if (jumpForce == null) jumpForce = new Vector3(0, 0, 0);
        initRot = transform.rotation;
        nearbyInteractables = new Collider2D[0];

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

        NotfiyInteractablesMovedAway(CheckNearbyInteractables());
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
        if(hasFallen>=fallDis) MassTeleport(joint2D.gameObject.transform.position);
        hasFallen = 0;
        ClearForce();
        joint = joint2D;
        joint.enabled = true;
        joint.connectedBody = upperArmsRB;
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
        joint.enabled = false;
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

    public Collider2D[] CheckNearbyInteractables()
    {
        Vector2 dir = Vector2.down;
        if (facingRight) dir = Vector2.down + Vector2.right;
        else dir = Vector2.down + Vector2.left;
        RaycastHit2D[] nearbyHits = Physics2D.BoxCastAll(headRB.position+boxcastOffset, boxcastSize, boxcastAngle,
            boxcastDir, boxcastDistance, LayerMask.GetMask("Interactable"));
        Collider2D[] nh = new Collider2D[nearbyHits.Length];
        int i = 0;
        foreach (RaycastHit2D hit in nearbyHits)
        {
            IInteractable tempInteractable = GameManager_Trapeze.GetInstance().GetInteractable(hit.transform);
            nh[i] = hit.collider;
            if (nh[i]!=null && tempInteractable != null && !nearbyInteractables.Contains(hit.collider))
            {
                tempInteractable.InRange(true);
            }
            i++;
        }
        return nh;
    }

    public void NotfiyInteractablesMovedAway(Collider2D[] tempInteractables)
    {
        if (nearbyInteractables != null)
        {
            foreach (Collider2D collider in nearbyInteractables)
            {
                if (collider!=null && collider.transform!=null && !tempInteractables.Contains(collider))
                {
                    IInteractable movedFrom = GameManager_Trapeze.GetInstance().GetInteractable(collider.transform);
                    if (movedFrom != null) movedFrom.InRange(false);
                }
            }
        }
        nearbyInteractables = tempInteractables;
    }

}
