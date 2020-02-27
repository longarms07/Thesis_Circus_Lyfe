using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class TutorialManager : MonoBehaviour
{

    public PlayerManager_Trapeze player;
    public DonnaManager_Trapeze donna;
    public DialogueRunner dialogueRunner;
    public YarnProgram scriptToLoad;
    public List<string> tutorialNodes;
    public int nextTutorial = 0;
    public GrabTarget rightTrapezeTarget;
    public GrabTarget playerLegTarget;
    public GrabTarget donnaLegTarget;

    private int numDetected = 0;
    private int targetNum = 0;
    private PlayerManager_Trapeze targetManager = null;
    private GrabTarget targetGrabTarget = null;
    private string successNode = "";
    private string failNode = "";
    private bool detecting = false;
    private string targetTrick;

    // Start is called before the first frame update
    void Start()
    {
        if (scriptToLoad != null)
        {
            dialogueRunner.Add(scriptToLoad);
        }
        dialogueRunner.StartDialogue(tutorialNodes[nextTutorial]);
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    [YarnCommand("DetectShort")]
    public void DetectShort(string number, string target, string nextNode)
    {
        if (detecting) return;
        detecting = true;
        SetTarget(target);
        if (targetManager == null) return;
        //Debug.Log("Made it past the checks");
        numDetected = 0;
        targetNum = int.Parse(number);
        successNode = nextNode;
        targetManager.OnShort += this.InvokeShortDetected;
    }

    private void InvokeShortDetected()
    {
        Invoke("ShortDetected", 1);
    }

    private void ShortDetected()
    {
        //Debug.Log("Short Detected run");
        if (!detecting) return;
        numDetected++;
        if (numDetected >= targetNum)
        {
            targetManager.OnShort -= this.InvokeShortDetected;
            dialogueRunner.StartDialogue(successNode);
            //Debug.Log("Started runnning "+ successNode);
            ResetVars();
        }
    }

    [YarnCommand("DetectLong")]
    public void DetectLong(string number, string target, string nextNode)
    {
        if (detecting) return;
        detecting = true;
        SetTarget(target);
        if (targetManager == null) return;
        numDetected = 0;
        targetNum = int.Parse(number);
        successNode = nextNode;
        targetManager.OnLong += this.InvokeLongDetected;
    }

    private void InvokeLongDetected()
    {
        Invoke("LongDetected", 1);
    }

    private void LongDetected()
    {
        if (!detecting) return;
        numDetected++;
        if (numDetected >= targetNum)
        {
            targetManager.OnLong -= this.InvokeLongDetected;
            dialogueRunner.StartDialogue(successNode);
            ResetVars();
        }
    }


    [YarnCommand("DetectJump")]
    public void DetectJump(string number, string target, string nextNode)
    {
        if (detecting) return;
        detecting = true;
        SetTarget(target);
        if (targetManager == null) return;
        numDetected = 0;
        targetNum = int.Parse(number);
        successNode = nextNode;
        targetManager.OnJump += this.InvokeJumpDetected;

    }

    private void InvokeJumpDetected()
    {
        Invoke("JumpDetected", 0.5f);
    }

    private void JumpDetected()
    {
        if (!detecting) return;
        numDetected++;
        if (numDetected >= targetNum)
        {
            targetManager.OnJump -= this.InvokeJumpDetected;
            dialogueRunner.StartDialogue(successNode);
            ResetVars();
        }
    }

    [YarnCommand("DetectGrab")]
    public void DetectAttachTo(string target, string grabTarget, string nextNode)
    {
        if (detecting) return;
        detecting = true;
        SetTarget(target);
        if (grabTarget.Equals("right")) targetGrabTarget = rightTrapezeTarget;
        else if (grabTarget.Equals("donna")) targetGrabTarget = donnaLegTarget;
        else if (grabTarget.Equals("player")) targetGrabTarget = playerLegTarget;
        else return;
        if (targetManager == null) return;
        successNode = nextNode;
        targetManager.OnAttachTo += this.InvokeAttachToDetected;

    }

    private void InvokeAttachToDetected()
    {
        Invoke("AttachToDetected", 2);
    }

    private void AttachToDetected()
    {
        if (!detecting) return;
        if(player.GetGrabTarget() == targetGrabTarget)
        {
            targetManager.OnAttachTo -= this.InvokeAttachToDetected;
            dialogueRunner.StartDialogue(successNode);
            ResetVars();
        }
    }


    [YarnCommand("DetectTrick")]
    public void DetectTrick(string number, string target, string trickName, string nextNode, string wrongNode)
    {
        if (detecting) return;
        detecting = true;
        SetTarget(target);
        if (targetManager == null) return;
        numDetected = 0;
        targetNum = int.Parse(number);
        successNode = nextNode;
        failNode = wrongNode;
        targetTrick = trickName;
        targetManager.OnTrick += this.TrickDetected;

    }

    private void TrickDetected()
    {
        if (!detecting) return;
        if (player.GetLastTrickPerformed().Equals(targetTrick))
        {
            targetManager.OnTrick -= this.TrickDetected;
            dialogueRunner.StartDialogue(successNode);
            ResetVars();
        }
        else
        {
            dialogueRunner.StartDialogue(failNode);
        }
    }

    private void ResetVars()
    {
        targetNum = 0;
        numDetected = 0;
        targetManager = null;
        successNode = "";
        failNode = "";
        targetGrabTarget = null;
        targetTrick = "";
        detecting = false;
    }

    private void SetTarget(string target)
    {
        if (target.Equals("player"))
            targetManager = player;
        else if (target.Equals("donna"))
            targetManager = donna;
        else return;
    }



}
