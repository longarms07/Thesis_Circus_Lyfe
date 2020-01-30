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

    private bool hasCloned = false;
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
        headRot = head.transform.localRotation;
        torsoRot = torso.transform.localRotation;
        armsRot =  arms.transform.localRotation;
        upperLegsRot = upperLegs.transform.localRotation;
        lowerLegsRot = lowerLegs.transform.localRotation;
        upperArmsRot = upperArms.transform.localRotation;
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
        //Debug.Log("Anchor x before: "+ arms2torso.connectedAnchor.x);
        arms2torso.connectedAnchor = new Vector2(arms2torso.connectedAnchor.x * -1, arms2torso.connectedAnchor.y);
        //Debug.Log("Anchor x after: " + arms2torso.connectedAnchor.x);
        JointAngleLimits2D lim = new JointAngleLimits2D();
        lim.min = torso2upperLegs.limits.min + f*60;
        lim.max = torso2upperLegs.limits.max + f*60;
        torso2upperLegs.limits = lim;
        lim.min = arms2torso.limits.min + f * 180;
        lim.max = arms2torso.limits.max + f * 180;
        arms2torso.limits = lim;
        lim.min = upperArms2arms.limits.min + f * 139;
        lim.max = upperArms2arms.limits.max + f * 152;
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

    protected void ResetRotation()
    {
        headRot2 = head.transform.localRotation;
        torsoRot2 = torso.transform.localRotation;
        armsRot2 = arms.transform.localRotation;
        upperLegsRot2 = upperLegs.transform.localRotation;
        lowerLegsRot2 = lowerLegs.transform.localRotation;
        upperArmsRot2 = upperArms.transform.localRotation;
        head.transform.localRotation = headRot;
        torso.transform.localRotation = torsoRot;
        arms.transform.localRotation = armsRot;
        upperLegs.transform.localRotation = upperLegsRot;
        lowerLegs.transform.localRotation = lowerLegsRot;
        upperArms.transform.localRotation = upperArmsRot;
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

   /* protected void CloneLimbs()
    {
        lowerLegs2 = Object.Instantiate(lowerLegs.gameObject, this.gameObject.transform);
        lowerLegs2.name = "Lower legs(1)";
        upperLegs2 = Object.Instantiate(upperLegs.gameObject, this.gameObject.transform);
        upperLegs2.name = "Upper Legs (1)";
        arms2 = Object.Instantiate(arms.gameObject, this.gameObject.transform);
        arms2.name = "Arms (1)";
        upperArms2 = Object.Instantiate(upperArms.gameObject, this.gameObject.transform);
        upperArms2.name = "Upper Arms (1)";

        hasCloned = true;
    }

    protected void KillClones()
    {
        if (hasCloned)
        {
            Destroy(lowerLegs2);
            Destroy(arms2);
            Destroy(upperArms2);
            Destroy(upperLegs2);

        }
    }*/


}
