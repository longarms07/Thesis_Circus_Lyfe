using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour, ITapListener
{
    [Tooltip("The avatar for the player character. Requires TouchMovable.")]
    public GameObject playerAvatar;
    [Tooltip("The name for the layer interactables are found on.")]
    public string interactableLayerName;
    [Tooltip("The name for the layer the floor is found on.")]
    public string floorLayerName;


    private TouchMovable playerTouchMovable;

    private RaycastHit2D lastTap;
    private int floorLayer;
    private int interactableLayer;

    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            floorLayer = LayerMask.NameToLayer(floorLayerName);
            Debug.Log("floor layer = " + floorLayer);
            interactableLayer = LayerMask.NameToLayer(interactableLayerName);
            Debug.Log("interactable layer = " + interactableLayer);
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
            if(lastTap.transform.gameObject.layer == floorLayer) {
                playerTouchMovable.OnTap(position);
            }
        }
    

    }

    private RaycastHit2D CheckTappedPosition(Vector3 position)
    {
            RaycastHit2D hit = Physics2D.Raycast(position, Vector2.down);
            if (hit.collider != null)
            {
            lastTap = hit;
            }
        return hit;

    }


}
