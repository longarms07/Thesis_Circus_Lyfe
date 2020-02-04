using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrabTarget : GrabTarget
{
    protected override void CalculateAngleDegrees()
    {

        if (pm.GetGrabTarget() != null) angleDegrees = pm.GetGrabTarget().angleDegrees;
        else base.CalculateAngleDegrees();
    }

    public override void InRange(bool inRange)
    {
        this.inRange = false;
    }
}
