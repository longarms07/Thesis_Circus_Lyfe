using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonnaManager_Trapeze : PlayerManager_Trapeze
{
    public float shortDegreesForwardMin;
    public float shortDegreesForwardMax;
    public float shortDegreesBackwardMin;
    public float shortDegreesBackwardMax;
    public float waitTime;
    public float longDegrees;
    private bool shortDone;
    private bool longDone;
    private bool runAI = false;
    private float waitTimer = 0;

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
        }
        if (runAI)
        {
            if (state == EnumPTrapezeState.InAir)
            {
                DoInAir();
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
                        if (!shortDone && grabTarget.angleDegrees < 0
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
                        if (!shortDone && grabTarget.angleDegrees > 0
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

    public override bool AttachTo(GrabTarget gt)
    {
        runAI = false;
        waitTimer = 0;
        return base.AttachTo(gt);
    }
}
