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
    }

    public Vector2 GetCenterOfMass()
    {
        Vector2 centerOfMass = Vector3.zero;
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





}
