using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyManager : MonoBehaviour
{

    public GameObject head;
    public GameObject torso;
    public GameObject arms;
    public GameObject upperArms;
    public GameObject upperLegs;
    public GameObject lowerLegs;
    public GameObject centerOfGravity;

    protected Rigidbody2D headRB;
    protected Rigidbody2D torsoRB;
    protected Rigidbody2D armsRB;
    protected Rigidbody2D upperArmsRB;
    protected Rigidbody2D upperLegsRB;
    protected Rigidbody2D lowerLegsRB;
    

    protected HingeJoint2D torso2upperLegs;
    protected HingeJoint2D arms2torso;
    protected HingeJoint2D upperArms2arms;
    protected HingeJoint2D upperLegs2lowerLegs;
    protected bool facingRight = true;


    protected Animator animator;


    // Start is called before the first frame update
    protected void Start()
    {
        InitRBs();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void InitRBs()
    {
        headRB = head.GetComponent<Rigidbody2D>();
        torsoRB = torso.GetComponent<Rigidbody2D>();
        armsRB = arms.GetComponent<Rigidbody2D>();
        upperLegsRB = upperLegs.GetComponent<Rigidbody2D>();
        lowerLegsRB = lowerLegs.GetComponent<Rigidbody2D>();
        upperArmsRB = upperArms.GetComponent<Rigidbody2D>();
        torso2upperLegs = torso.GetComponent<HingeJoint2D>();
        arms2torso = arms.GetComponent<HingeJoint2D>();
        upperArms2arms = upperArms.GetComponent<HingeJoint2D>();
        upperLegs2lowerLegs = upperLegs.GetComponent<HingeJoint2D>();
        animator = GetComponent<Animator>();

    }

    public Vector2 GetCenterOfMass()
    {
        Vector2 centerOfMass = new Vector3(0,0,-1);
        float mass = 0;
        centerOfMass += (armsRB.mass * armsRB.worldCenterOfMass);
        mass += armsRB.mass;
        centerOfMass += (upperArmsRB.mass * upperArmsRB.worldCenterOfMass);
        mass += upperArmsRB.mass;
        centerOfMass += (headRB.mass * headRB.worldCenterOfMass);
        mass += headRB.mass;
        centerOfMass += (torsoRB.mass * torsoRB.worldCenterOfMass);
        mass += torsoRB.mass;
        centerOfMass += (lowerLegsRB.mass * lowerLegsRB.worldCenterOfMass);
        mass += lowerLegsRB.mass;
        centerOfMass += (upperLegsRB.mass * upperLegsRB.worldCenterOfMass);
        mass += upperLegsRB.mass;

        return centerOfMass/mass;
    }

    public void SetGravityScale(float scale)
    {
        armsRB.gravityScale = scale;
        upperArmsRB.gravityScale = scale;
        lowerLegsRB.gravityScale = scale;
        upperLegsRB.gravityScale = scale;
        headRB.gravityScale = scale;
        torsoRB.gravityScale = scale;
    }

    public void MassTeleport(Vector3 position)
    {
        armsRB.position = position;
        upperArmsRB.position = position;
        lowerLegsRB.position = position;
        upperLegsRB.position = position;
        headRB.position = position;
        torsoRB.position = position;
    }

    public void TurnAround()
    {
        int f = 1;
        if (facingRight) f = -1;
        facingRight = !facingRight;
        animator.SetBool("facingRight", facingRight);
        //arms.transform.Rotate(0, 180, 0); 
        //upperArms.transform.Rotate(0, 180, 0);
        //lowerLegs.transform.Rotate(0, 180, 0);
        head.transform.Rotate(0, 180, 0);
        torso.transform.Rotate(0, 180, 0);
        //upperLegs.transform.Rotate(0, 180, 0);
        JointAngleLimits2D lim = new JointAngleLimits2D();
        lim.min = torso2upperLegs.limits.min + f*60;
        lim.max = torso2upperLegs.limits.max + f*60;
        torso2upperLegs.limits = lim;
        lim.min = arms2torso.limits.min + f * 180;
        lim.max = arms2torso.limits.max + f * 180;
        arms2torso.limits = lim;
        //Debug.Log("Anchor x before: "+ arms2torso.connectedAnchor.x);
        arms2torso.connectedAnchor = new Vector2(arms2torso.connectedAnchor.x*-1, arms2torso.connectedAnchor.y);
        //Debug.Log("Anchor x after: " + arms2torso.connectedAnchor.x);
        lim.min = upperArms2arms.limits.min + f * 180;
        lim.max = upperArms2arms.limits.max + f * 180;
        upperArms2arms.limits = lim;
        lim.min = upperLegs2lowerLegs.limits.min + -f * 90;
        lim.max = upperLegs2lowerLegs.limits.max + -f * 90;
        upperLegs2lowerLegs.limits = lim;



    }


    public void SetKinematic(bool t)
    {
        headRB.isKinematic = t;
        torsoRB.isKinematic = t;
        armsRB.isKinematic = t;
        upperArmsRB.isKinematic = t;
        lowerLegsRB.isKinematic = t;
        upperLegsRB.isKinematic = t;
    }



}
