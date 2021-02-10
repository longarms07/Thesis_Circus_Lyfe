using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleOverTime : MonoBehaviour
{

    public Vector3 targetScale;
    public float scaleTime;
    float startTime;
    bool scale = false;

    // Start is called before the first frame update
    void Start()
    {
        StartScale();
    }

    // Update is called once per frame
    void Update()
    {
        if (scale)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, targetScale, Time.deltaTime);
            if (HasTimeElapsed())
            {
                transform.localScale = targetScale;
                scale = false;
                startTime = -1f;
            }
        }
    }

    public bool StartScale()
    {
        if (scale = true || transform.localScale == targetScale) return false;
        startTime = Time.time;
        scale = true;
        return true;
    }

    private bool HasTimeElapsed()
    {
        float timeElapsed = Time.time - startTime;
        return (timeElapsed >= scaleTime);
    }


}
