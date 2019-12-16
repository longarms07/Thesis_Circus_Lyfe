using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager :  ISwipeListener, ITapListener
{
    [Tooltip("The avatar for the player character. Requires TouchMovable.")]
    public GameObject playerAvatar;
    [Tooltip("The layer interactables are found on.")]
    public int interactableLayer;
    [Tooltip("The layer intangible interactables are found on.")]
    public int interactableLayer2;
    [Tooltip("The layer the floor is found on.")]
    public int floorLayer;
    [Tooltip("The layer NPCS are found on")]
    public int npcLayer;

    

    public Dictionary<Transform, IInteractable> interactableDict;
    protected Dictionary<Transform, Button> buttonDict;
    private TouchMovable playerTouchMovable;
    protected RaycastHit2D lastTap;
    
  

    private static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            interactableDict = new Dictionary<Transform, IInteractable>();
            buttonDict = new Dictionary<Transform, Button>();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        if(playerAvatar == null)
        {
            Debug.LogError("Player Avatar is null!");
            Destroy(this.gameObject);
        }
        else
        {
            playerTouchMovable = playerAvatar.GetComponent<TouchMovable>();
            if(playerTouchMovable == null)
            {
                Debug.LogError("Player Avatar has no touch movable!");
                Destroy(this.gameObject);
            }
            TouchInputManager.getInstance().SubscribeTapListener(this, 0);
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void TogglePlayerMovement(bool active)
    {
        playerTouchMovable.ToggleMovement(active);
    }

    public void TapDetected(Vector3 position)
    {
        CheckTappedPosition(position);
        if (lastTap.transform != null) {
            Debug.Log("Hit layer = " + lastTap.transform.gameObject.layer);
            if (lastTap.transform.gameObject.layer == floorLayer)
            {
                playerTouchMovable.OnTap(position);
            }
            else if (lastTap.transform.gameObject.layer == interactableLayer || lastTap.transform.gameObject.layer == npcLayer)
            {
                playerTouchMovable.TargetInteractable(lastTap.collider, GetInteractable(lastTap.transform));
            }
            else if (lastTap.transform.gameObject.layer == 5)
            {
                if (lastTap.transform.gameObject == TextboxManager.GetInstance().textBackground)
                    TextboxManager.GetInstance().OnTap();
                else 
                {
                    Button btn = GetButton(lastTap.transform);
                    if (btn != null) btn.OnTap();
                }
            }
        }
    

    }

    protected RaycastHit2D CheckTappedPosition(Vector3 position)
    {
            RaycastHit2D hit = Physics2D.Raycast(position, Vector2.down);
            if (hit.collider != null)
            {
            lastTap = hit;
            }
        return hit;

    }

    public static GameManager getInstance()
    {
        return instance;
    }

    public void RegisterInteractable(Transform transform, IInteractable interactable)
    {
        interactableDict.Add(transform, interactable);
    }

    public IInteractable GetInteractable(Transform transform)
    {
        //Debug.Log("Key = " + transform);
        if(interactableDict.ContainsKey(transform))
            return interactableDict[transform];
        return null;
    }

    public void RegisterButton(Transform transform, Button button)
    {
            buttonDict.Add(transform, button);
            //Debug.Log("Registered button");
    }

    public Button GetButton(Transform transform)
    {
       // Debug.Log("Key = " + transform);
        if (transform!=null && buttonDict.ContainsKey(transform))
            return buttonDict[transform];
        return null;
    }

    public void DeregisterButton(Transform transform)
    {
        if (buttonDict.ContainsKey(transform))
            buttonDict.Remove(transform);
    }

    override
    public void SwipeDetected(Vector3[] swipePositions)
    { }

    public void ChangeSceneTrapeze()
    {
        if (SceneManager.GetActiveScene().name == "MovementDemoScene") 
        {
            SceneManager.LoadScene("TrapezeDemoScene");
        }
    }

    public void ChangeSceneLifeSim()
    {
        if(SceneManager.GetActiveScene().name == "TrapezeDemoScene")
        {
            SceneManager.LoadScene("MovementDemoScene");
        }
    }

    }
