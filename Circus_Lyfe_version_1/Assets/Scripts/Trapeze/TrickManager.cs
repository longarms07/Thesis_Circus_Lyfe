using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrickManager : MonoBehaviour
{
    [Tooltip("Add tricks to this list and they will be added to the Tricktionary.")]
    public List<Trick> tricks;
    public float execTime;

    private Dictionary<List<SwipeDirection>, Trick> tricktionary = new Dictionary<List<SwipeDirection>, Trick>(new TrickCodeComparer());

    private List<SwipeDirection> currentSwipe;
    private float timer;
    public PlayerManager_Trapeze playerManager;
    public DonnaManager_Trapeze donnaManager;
    private GameManager_Trapeze gm;
    private static TrickManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
        foreach( Trick t in tricks){
            tricktionary.Add(t.code, t);
        }
        currentSwipe = new List<SwipeDirection>();
    }

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager_Trapeze.GetInstance();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (playerManager == null) playerManager = gm.GetPlayerManager();
        if (playerManager.state == EnumPTrapezeState.InAir && currentSwipe.Count != 0 && !playerManager.HasTarget())
        {
            timer+=Time.deltaTime;
            if (timer >= execTime) ExecuteTrick();
        }
        else
        {
            if (timer != 0) timer = 0;
            if (currentSwipe.Count != 0) currentSwipe.Clear();
        }
    }

    public void AddSwipe(SwipeDirection s)
    {
        currentSwipe.Add(s);
        timer = 0;
        Debug.Log("Recieved swipe going " + s);
    }

    public void ExecuteTrick()
    {
        Debug.Log("Executing trick with currentSwipe: " + PrintList(currentSwipe));
        if (tricktionary.ContainsKey(currentSwipe))
        {
            Trick t = tricktionary[currentSwipe];
            if (donnaManager.state == EnumPTrapezeState.InAir)
            {
                ExecuteTrickDonna(currentSwipe);
                gm.trickGUI.DidTrick("Duo "+t.name, t.score*4);
            }
            else
            {
                gm.trickGUI.DidTrick(t.name, t.score);
            }
            playerManager.DoAnimation(t.playerAnimFile);

        }
        currentSwipe.Clear();
        timer = 0;

    }

    public void ExecuteTrickDonna(List<SwipeDirection> trickCode)
    {
        if (tricktionary.ContainsKey(trickCode))
        {
            Trick t = tricktionary[trickCode];
            //gm.trickGUI.DidTrick(t.name, t.score);
            donnaManager.DoAnimation(t.playerAnimFile);

        }
    }

    public string PrintList(List<SwipeDirection> l)
    {
        string s = " ";
        for (int i = 0; i < l.Count; i++)
        {
            s += l[i];
            if (i + 1 < l.Count) s += ", ";
        }
        return s;
    }

    public Dictionary<List<SwipeDirection>, Trick> GetTrickionary()
    {
        return tricktionary;
    }

    public static TrickManager GetInstance()
    {
        return instance;
    }

    public List<Trick> GetAvaliableTricks()
    {
        return tricks;
    }

}
