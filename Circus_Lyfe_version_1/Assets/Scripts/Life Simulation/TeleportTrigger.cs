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
            collision.gameObject.transform.position = target.transform.position;
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
