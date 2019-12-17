using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Trick 
{
    
    public string name;
    public float duration;
    public int score;
    public List<SwipeDirection> code;
    

    public Trick(string n, float d, int s, List<SwipeDirection> c)
    {
        name = n;
        duration = d;
        score = s;
        code = c;
    }





}
