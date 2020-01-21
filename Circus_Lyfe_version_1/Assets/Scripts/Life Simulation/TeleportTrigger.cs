using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportTrigger : MonoBehaviour
{

    public TeleportTrigger target;
    public bool active;


    // Start is called before the first frame update
    void Start()
    {
        active = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.name + " Entered " + this.gameObject.name);
        if (collision.gameObject == GameManager.getInstance().playerAvatar && active)
        {
            target.Deactivate();
            collision.gameObject.transform.position = new Vector3(target.transform.position.x,
                                                                  target.transform.position.y,
                                                                  collision.gameObject.transform.position.z);
            if (GameManager.getInstance().GetPlayerMovable().GetIsFollowed())
            {
                TouchMovable t =GameManager.getInstance().GetPlayerMovable();
                t.GetFollower().transform.localPosition = new Vector3(target.transform.position.x + t.followDist,
                                                            target.transform.position.y,
                                                            collision.gameObject.transform.position.z);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == GameManager.getInstance().playerAvatar)
        {
            active = true;
        }
    }

    public void Deactivate()
    {
        active = false;
    }
}
