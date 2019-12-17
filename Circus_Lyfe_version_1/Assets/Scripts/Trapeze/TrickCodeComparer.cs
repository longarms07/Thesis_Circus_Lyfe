using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrickCodeComparer : IEqualityComparer<List<SwipeDirection>>
{
    public bool Equals(List<SwipeDirection> x, List<SwipeDirection> y)
    {
        //Debug.Log(x.Count + "," + y.Count);
        if (x.Count != y.Count)
        {
            return false;
        }
        for (int i = 0; i < x.Count; i++)
        {
            //Debug.Log("Xlist: " + x[i] + " , yList: "+y[i]);
            if (x[i] != y[i])
            {
                return false;
            }
        }
        return true;
    }

    public int GetHashCode(List<SwipeDirection> swipes)
    {
        int code = 0;
        foreach(SwipeDirection s in swipes)
        {
            code = code + s.GetHashCode();
        }
        return code;
    }
}
