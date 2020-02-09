using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Trick 
{
    
    public string name;
    public bool unlocked;
    public int score;
    public List<SwipeDirection> code;
    public string playerAnimFile;
    

    public Trick(string n, float d, int s, List<SwipeDirection> c)
    {
        name = n;
        unlocked = false;
        score = s;
        code = c;
    }





}
