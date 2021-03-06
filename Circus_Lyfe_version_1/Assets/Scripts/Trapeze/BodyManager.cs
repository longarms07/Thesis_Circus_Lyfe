﻿using System.Collections;
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

    protected SpriteRenderer headSprite;
    protected SpriteRenderer torsoSprite;
    protected SpriteRenderer armsSprite;
    protected SpriteRenderer upperArmsSprite;
    protected SpriteRenderer upperLegsSprite;
    protected SpriteRenderer lowerLegsSprite;

    public HingeJoint2D torso2upperLegs;
    public HingeJoint2D arms2torso;
    public HingeJoint2D upperArms2arms;
    public HingeJoint2D upperLegs2lowerLegs;
    public bool facingRight = true;

    private Quaternion headRot;
    private Quaternion torsoRot;
    private Quaternion armsRot;
    private Quaternion upperArmsRot;
    private Quaternion lowerLegsRot;
    private Quaternion upperLegsRot;

    private Quaternion headRot2;
    private Quaternion torsoRot2;
    private Quaternion armsRot2;
    private Quaternion upperArmsRot2;
    private Quaternion lowerLegsRot2;
    private Quaternion upperLegsRot2;
    
    private GameObject lowerLegs2;
    private GameObject upperLegs2;
    private GameObject arms2;
    private GameObject upperArms2;



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
        headSprite = head.GetComponent<SpriteRenderer>(); ;
        torsoSprite = torso.GetComponent<SpriteRenderer>();
        armsSprite = arms.GetComponent<SpriteRenderer>();
        upperLegsSprite = upperLegs.GetComponent<SpriteRenderer>();
        lowerLegsSprite = lowerLegs.GetComponent<SpriteRenderer>();
        upperArmsSprite = upperArms.GetComponent<SpriteRenderer>();
        headRot = head.transform.rotation;
        torsoRot = torso.transform.rotation;
        armsRot =  arms.transform.localRotation;
        upperLegsRot = upperLegs.transform.rotation;
        lowerLegsRot = lowerLegs.transform.rotation;
        upperArmsRot = upperArms.transform.rotation;
        HingeJoint2D[] torsoJoints = torso.GetComponents<HingeJoint2D>();
        foreach(HingeJoint2D tj in torsoJoints)
        {
            if (tj.connectedBody == armsRB) arms2torso = tj;
            else torso2upperLegs = tj;
        }
       /* torso2upperLegs = torso.GetComponent<HingeJoint2D>();
        arms2torso = arms.GetComponent<HingeJoint2D>();
        upperArms2arms = upperArms.GetComponent<HingeJoint2D>();
        upperLegs2lowerLegs = upperLegs.GetComponent<HingeJoint2D>();*/
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
        SetKinematic(true);
        ResetRotation();
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
        //Debug.Log("Anchor x before: "+ arms2torso.connectedAnchor.x);
        arms2torso.anchor = new Vector2(arms2torso.anchor.x * -1, arms2torso.anchor.y);
        //Debug.Log("Anchor x after: " + arms2torso.connectedAnchor.x);
        JointAngleLimits2D lim = new JointAngleLimits2D();
        lim.min = torso2upperLegs.limits.min + -f*300;
        lim.max = torso2upperLegs.limits.max + -f*300;
        torso2upperLegs.limits = lim;
        lim.min = arms2torso.limits.min + f * 500;
        lim.max = arms2torso.limits.max + f * 500;
        arms2torso.limits = lim;
        lim.min = upperArms2arms.limits.min + f * 150;
        lim.max = upperArms2arms.limits.max + f * 150;
        upperArms2arms.limits = lim;
        lim.min = upperLegs2lowerLegs.limits.min + f * 270;
        lim.max = upperLegs2lowerLegs.limits.max + f * 270;
        upperLegs2lowerLegs.limits = lim;

        SetKinematic(false);


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

    protected void ResetRotation()
    {
        headRot2 = head.transform.localRotation;
        torsoRot2 = torso.transform.localRotation;
        armsRot2 = arms.transform.localRotation;
        upperLegsRot2 = upperLegs.transform.localRotation;
        lowerLegsRot2 = lowerLegs.transform.localRotation;
        upperArmsRot2 = upperArms.transform.localRotation;
        head.transform.rotation = headRot;
        torso.transform.rotation = torsoRot;
        arms.transform.rotation = armsRot;
        upperLegs.transform.rotation = upperLegsRot;
        lowerLegs.transform.rotation = lowerLegsRot;
        upperArms.transform.rotation = upperArmsRot;
    }

    protected void ResumeRotation()
    {
        head.transform.localRotation = headRot2;
        torso.transform.localRotation = torsoRot2;
        arms.transform.localRotation = armsRot2;
        upperLegs.transform.localRotation = upperLegsRot2;
        lowerLegs.transform.localRotation = lowerLegsRot2;
        upperArms.transform.localRotation = upperArmsRot2;

    }

    public void EnableSprites(bool enable)
    {
        headSprite.enabled = enable;
        torsoSprite.enabled = enable;
        armsSprite.enabled = enable;
        upperLegsSprite.enabled = enable;
        lowerLegsSprite.enabled = enable;
        upperArmsSprite.enabled = enable;
        lowerLegs.SetActive(enable);
    }

}
