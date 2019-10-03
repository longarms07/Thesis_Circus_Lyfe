using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Tooltip("The avatar for the player character. Requires TouchMovable.")]
    public GameObject playerAvatar;


    private TouchMovable playerTouchMovable;

    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
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


}
