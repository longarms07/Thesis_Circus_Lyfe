using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Yarn.Unity;

public class TutorialManager : MonoBehaviour
{

    public PlayerManager_Trapeze player;
    public DonnaManager_Trapeze donna;
    public DialogueRunner dialogueRunner;
    public YarnProgram scriptToLoad;
    public List<tutorialNode> tutorialNodes;
    public int nextTutorial = 0;
    public string duoTutorialNode;
    public GrabTarget rightTrapezeTarget;
    public GrabTarget leftTrapezeTarget;
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
    public bool duoTutorialDone;
    private static string savefile = "tutorial.save";

    [System.Serializable]
    public struct tutorialNode
    {
        public string name;
        public int minPracticeSessions;
    }

    private void Awake()
    {
        //DeleteSaveData();
        LoadSaveData();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (scriptToLoad != null)
        {
            dialogueRunner.Add(scriptToLoad);
        }

        //Debug.Log((nextTutorial < tutorialNodes.Count) + " , " + (tutorialNodes[nextTutorial].minPracticeSessions <= GameManager_Trapeze.GetInstance().GetTimesPlayed()));
        if (GameManager_Trapeze.GetInstance().GetDuoTutorial() && donna.isActiveAndEnabled && !duoTutorialDone)
            dialogueRunner.StartDialogue(duoTutorialNode);
        else if (nextTutorial < tutorialNodes.Count && tutorialNodes[nextTutorial].minPracticeSessions <= GameManager_Trapeze.GetInstance().GetTimesPlayed() && !donna.isActiveAndEnabled)
        {
            dialogueRunner.StartDialogue(tutorialNodes[nextTutorial].name);
        }
        else this.gameObject.SetActive(false);
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
            string s = successNode;
            ResetVars();
            dialogueRunner.StartDialogue(s);
            
            //Debug.Log("Started runnning "+ successNode);
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
            string s = successNode;
            ResetVars();
            dialogueRunner.StartDialogue(s);
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
        Debug.Log("Jump detected in tutorial manager!");
        numDetected++;
        if (numDetected >= targetNum)
        {
            targetManager.OnJump -= this.InvokeJumpDetected;
            string s = successNode;
            ResetVars();
            dialogueRunner.StartDialogue(s);
        }
    }

    [YarnCommand("DetectGrab")]
    public void DetectAttachTo(string target, string grabTarget, string nextNode)
    {
        Debug.Log("Detect grab");
        if (detecting) return;
        detecting = true;
        //Debug.Log("Detect grab made it to past detecting check");
        SetTarget(target);
        //Debug.Log("grabTarget == " + grabTarget + ", is right? " + (grabTarget.Equals("right")));
        if (grabTarget.Equals("right")) targetGrabTarget = rightTrapezeTarget;
        else if (grabTarget.Equals("left")) targetGrabTarget = leftTrapezeTarget;
        else if (grabTarget.Equals("donna")) targetGrabTarget = donnaLegTarget;
        else if (grabTarget.Equals("player")) targetGrabTarget = playerLegTarget;
        else return;
        //Debug.Log("Detect grab made it to past grab target check");
        if (targetManager == null) return;
        //Debug.Log("Detect grab made it to past target check");
        successNode = nextNode;
        targetManager.OnAttachTo += this.InvokeAttachToDetected;

        //Debug.Log("Detect grab made it to end");

    }

    private void InvokeAttachToDetected()
    {
        //Debug.Log("here");
        //Debug.Log("detecting = " + detecting);
        Invoke("AttachToDetected", 2);
    }

    private void AttachToDetected()
    {
        Debug.Log((targetManager.GetGrabTarget() == targetGrabTarget)+" , "+targetGrabTarget.name+", "+ targetManager.GetGrabTarget().name);
        if (!detecting) return;
        if(targetManager.GetGrabTarget() == targetGrabTarget)
        {
            targetManager.OnAttachTo -= this.InvokeAttachToDetected;
            string s = successNode;
            ResetVars();
            dialogueRunner.StartDialogue(s);
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
        if (targetManager.GetLastTrickPerformed().Equals(targetTrick))
        {
            numDetected++;
            if (numDetected >= targetNum)
            {
                targetManager.OnTrick -= this.TrickDetected;
                string s = successNode;
                ResetVars();
                dialogueRunner.StartDialogue(s);
            }
        }
        else
        {
            dialogueRunner.StartDialogue(failNode);
        }
    }

    [YarnCommand("DetectDuoTrick")]
    public void DetectDuoTrick(string number, string trickName, string nextNode, string wrongNode)
    {
        if (detecting) return;
        detecting = true;
        SetTarget("player");
        if (targetManager == null) return;
        numDetected = 0;
        targetNum = int.Parse(number);
        successNode = nextNode;
        failNode = wrongNode;
        targetTrick = trickName;
        TrickManager.GetInstance().OnDuoTrick += this.InvokeDuoTrickDetected;

    }

    private void InvokeDuoTrickDetected()
    {
        Invoke("DuoTrickDetected", 1.5f);
    }

    private void DuoTrickDetected()
    {
        if (!detecting) return;
        if (targetManager.GetLastTrickPerformed().Equals(targetTrick))
        {
            numDetected++;
            if (numDetected >= targetNum)
            {
                TrickManager.GetInstance().OnDuoTrick -= this.InvokeDuoTrickDetected;
                string s = successNode;
                ResetVars();
                dialogueRunner.StartDialogue(s);
            }
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

    [YarnCommand("TutorialDone")]
    public void TutorialDone()
    {
        if (GameManager_Trapeze.GetInstance().GetDuoTutorial()) duoTutorialDone = true;
        else nextTutorial++;
        SaveData();
        this.gameObject.SetActive(false);
    }

    [System.Serializable]
    class TutorialSave
    {
        public int nextTutorial;
        public bool duoTutorialDone;
    }

    public virtual void SaveData()
    {
        TutorialSave save = new TutorialSave();
        save.nextTutorial = nextTutorial;
        save.duoTutorialDone = duoTutorialDone;
        BinaryFormatter format = new BinaryFormatter();
        FileStream fs = File.Create(Application.persistentDataPath + savefile);
        //Debug.Log(Application.persistentDataPath + savefile);
        format.Serialize(fs, save);
        fs.Close();
        Debug.Log("Game Saved");
    }

    public virtual bool LoadSaveData()
    {
        if (File.Exists(Application.persistentDataPath + savefile))
        {
            BinaryFormatter format = new BinaryFormatter();
            FileStream fs = File.Open(Application.persistentDataPath + savefile, FileMode.Open);
            TutorialSave save = (TutorialSave)format.Deserialize(fs);
            fs.Close();
            nextTutorial = save.nextTutorial;
            duoTutorialDone = save.duoTutorialDone;
            return true;
        }
        return false;
    }


    public void DeleteSaveData()
    {
        if (File.Exists(Application.persistentDataPath + savefile))
        {
            File.Delete(Application.persistentDataPath + savefile);
        }
    }


    private void OnApplicationPause(bool pause)
    {
        SaveData();
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }

    private void OnDestroy()
    {
        SaveData();
    }


}
