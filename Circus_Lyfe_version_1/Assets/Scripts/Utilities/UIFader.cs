using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIFader : MonoBehaviour
{

    CanvasGroup CG;
    public bool fadeInAtStart = true;
    public float fadeTime = 1f;
    public float waitBeforeFade = 0f;
    public bool loopFades = false;
    public float loopPause = 0f;
    bool fadingIn = false;
    bool fadingOut = false;
    float startTime = -1f;

    // Start is called before the first frame update
    void Start()
    {
        CG = GetComponent<CanvasGroup>();
        if (fadeInAtStart)
        {
            CG.alpha = 0;
            Invoke("FadeIn", waitBeforeFade);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (fadingIn)
        {
            if (!HasTimeElapsed())
                CG.alpha += ((1 - CG.alpha) * (Time.deltaTime));
            else
            {
                CG.alpha = 1f;
                fadingIn = false;
                startTime = -1f;
                if (loopFades) Invoke("FadeOut", loopPause);
            }
        }
        else if (fadingOut)
        {
            if (!HasTimeElapsed())
                CG.alpha -= (CG.alpha * Time.deltaTime);
            else
            {
                CG.alpha = 0f;
                fadingOut = false;
                startTime = -1f;
                if (loopFades) Invoke("FadeIn", loopPause);
            }
        }
    }

    public bool FadeIn()
    {
        if (CG.alpha != 0) return false;
        if (fadingOut || fadingIn) return false;
        fadingIn = true;
        startTime = Time.time;
        return true;
    }

    public bool FadeOut()
    {
        if (CG.alpha != 1) return false;
        if (fadingOut || fadingIn) return false;
        fadingOut = true;
        startTime = Time.time;
        return true;
    }

    private bool HasTimeElapsed()
    {
        float timeElapsed = Time.time - startTime;
        return (timeElapsed >= fadeTime);
    }
}
