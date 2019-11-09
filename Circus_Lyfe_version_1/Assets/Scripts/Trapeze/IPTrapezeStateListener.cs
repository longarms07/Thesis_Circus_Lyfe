using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPTrapezeStateListener
{
    void OnPlayerStateChange(EnumPTrapezeState state);
}
