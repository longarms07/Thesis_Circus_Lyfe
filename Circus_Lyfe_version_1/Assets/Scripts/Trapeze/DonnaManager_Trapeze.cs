using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonnaManager_Trapeze : PlayerManager_Trapeze, IPTrapezeStateListener
{
    public float shortDegreesForwardMin;
    public float shortDegreesForwardMax;
    public float shortDegreesBackwardMin;
    public float shortDegreesBackwardMax;
    public float longDegrees;
    public Vector2 jumpDegrees;
    [Tooltip("The odds that Donna will jump. In the format of x:y, for example 1:5")]
    public Vector2 jumpOdds;
    public int maxNumTricks;
    public float waitTime;
    public GrabTarget playerGrabTarget;
    public GrabTarget trapezeLeftGrabTarget;
    public GrabTarget trapezeRightGrabTarget;
    public GameObject jumpWarning;
    private bool shortDone;
    private bool longDone;
    private bool doJump;
    private bool runAI;
    private float waitTimer = 0;
    private int tricksPerformed;
    private int numTricksToDo;
    private bool targetChosen;
    
    void Start()
    {
        runAI = false;
        DoAtStart();
        gm.GetPlayerManager().SubscribeStateListener(this);
        punishFall = false;
    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {

        timerSL += Time.deltaTime;
        bool gr = GoingRight();
        if (goingRight != gr)
        {
            goingRight = gr;
            shortDone = false;
            longDone = false;
            if (runAI 
                && state == EnumPTrapezeState.OnTrapeze
                &&(facingRight && grabTarget.angleDegrees > -jumpDegrees.x && grabTarget.angleDegrees < 0)
                || (!facingRight && grabTarget.angleDegrees < jumpDegrees.x && grabTarget.angleDegrees > 0))
                CalculateJump();
        }
        if (runAI)
        {
            if (state == EnumPTrapezeState.InAir)
            {
                if(torsoRB.velocity.y > 0 && torsoRB.velocity.y<2 && tricksPerformed < numTricksToDo)
                {
                    List<Trick> trickCodes = tm.GetAvaliableTricks();
                    List<SwipeDirection> trickToDo = trickCodes[Random.Range(0, trickCodes.Count - 1)].code;
                    tricksPerformed++;
                    tm.ExecuteTrickDonna(trickToDo);
                }
                else if (torsoRB.velocity.y<0)
                {
                    DoInAir();
                }
            }
            else if (state == EnumPTrapezeState.OnTrapeze)
            {
                if (!facingRight)
                {
                    if (goingRight)
                    {
                        if (!longDone && grabTarget.angleDegrees <= -longDegrees)
                        {
                            Long();
                            longDone = true;
                        }
                        else if (!shortDone && grabTarget.angleDegrees > 0
                            && grabTarget.angleDegrees <= Mathf.Abs(shortDegreesBackwardMin)
                            && grabTarget.angleDegrees > Mathf.Abs(shortDegreesBackwardMax))
                        {
                            Short();
                            shortDone = true;
                        }
                    }
                    else
                    {
                        if (doJump && grabTarget.angleDegrees < 0
                            && grabTarget.angleDegrees >= -jumpDegrees.x
                            && grabTarget.angleDegrees < -jumpDegrees.y)
                            Jump();
                        else if (!shortDone && grabTarget.angleDegrees < 0
                            && grabTarget.angleDegrees >= -shortDegreesForwardMin
                            && grabTarget.angleDegrees < -shortDegreesForwardMax)
                        {
                            Short();
                            shortDone = true;
                        }
                    }
                }
                else
                {
                    if (goingRight)
                    {

                        if (doJump && grabTarget.angleDegrees > 0
                           && grabTarget.angleDegrees <= jumpDegrees.x
                           && grabTarget.angleDegrees > jumpDegrees.y)
                            Jump();
                        else if (!shortDone && grabTarget.angleDegrees > 0
                            && grabTarget.angleDegrees <= shortDegreesForwardMin
                            && grabTarget.angleDegrees > shortDegreesForwardMax)
                        {
                            Short();
                            shortDone = true;
                        }
                    }
                    else
                    {

                        if (!longDone && grabTarget.angleDegrees >= longDegrees)
                        {
                            Long();
                            longDone = true;
                        }
                        else if (!shortDone && grabTarget.angleDegrees < 0
                            && grabTarget.angleDegrees >= shortDegreesBackwardMin
                            && grabTarget.angleDegrees < shortDegreesBackwardMax)
                        {
                            Short();
                            shortDone = true;
                        }
                    }
                }

            }
        }
        else
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTime) runAI = true;
        }
    }

    public override bool AttachTo(GrabTarget gt, bool turnAround=true)
    {
        legGrabTarget.gameObject.SetActive(true);
        runAI = false;
        waitTimer = 0;
        return base.AttachTo(gt, turnAround);
    }

    public void CalculateJump()
    {
        jumpWarning.SetActive(false);
        int jumpNum = Mathf.CeilToInt(Random.Range(0, jumpOdds.y));
        Debug.Log("jumpNum is " + jumpNum + " , num to beat is "+ jumpOdds.x);
        if (jumpNum <= jumpOdds.x && playerJoint.connectedBody == null && state == EnumPTrapezeState.OnTrapeze) {
            doJump = true;
            tricksPerformed = 0;
            targetChosen = false;
            numTricksToDo = Random.Range(1, maxNumTricks);
            WarnJump();
            ChooseTarget();
        }
        else doJump = false;
            
    }

    public void WarnJump()
    {
        
            jumpWarning.SetActive(true);
            Debug.Log("Donna is about to Jump!!!");
    }

    public override bool Jump()
    {
        doJump = false;
        jumpWarning.SetActive(false);
        legGrabTarget.gameObject.SetActive(false) ;
        return base.Jump();
    }

    private void ChooseTarget()
    {
        EnumPTrapezeState pState = gm.GetPlayerManager().state;
        
        if (pState == EnumPTrapezeState.OnTrapeze && gm.GetPlayerManager().facingRight != facingRight)
        {
            Target(playerGrabTarget);
            Debug.Log("Donna is targeting the player");
        }
        else
        {
            if (facingRight) Target(trapezeRightGrabTarget);
            else Target(trapezeLeftGrabTarget);
            Debug.Log("Donna is targeting the trapeze");

        }
    }

    public override string GetAnimName(string animName)
    {
        return "d_" + animName;
    }

    public void OnPlayerStateChange(EnumPTrapezeState pState)
    {
        if(pState == EnumPTrapezeState.InAir)
            numTricksToDo = 0;
        if (state == EnumPTrapezeState.InAir)
        {
            ChooseTarget();
        }
    }
}
