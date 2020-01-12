using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderClimb : MonoBehaviour
{

    public GameObject invis;
    private BoxCollider2D[] trigger;
    private EdgeCollider2D walls;
    private LadderInteractable inter;
    private bool active;


    // Start is called before the first frame update
    void Start()
    {
        trigger = GetComponents<BoxCollider2D>();
        walls = GetComponent<EdgeCollider2D>();
        inter = GetComponentInParent<LadderInteractable>();
        walls.enabled = false;
        inter.enabled = false;
        Debug.Log(invis.gameObject);
        active = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Player is in range, should now be enabling everything.");
        if (collision.gameObject == GameManager.getInstance().playerAvatar)
        {
            if (active)
            {
                invis.SetActive(true);
                walls.enabled = false;
                inter.enabled = false;
                trigger[1].enabled = true;
                active = false;
            }
            else
            {
                invis.SetActive(false);
                inter.enabled = true;
                walls.enabled = true;
                trigger[1].enabled = false;
                active = true;

            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == GameManager.getInstance().playerAvatar)
        {
            if (active)
            {
                collision.gameObject.transform.position = new Vector3(collision.gameObject.transform.position.x,
                                                                        collision.gameObject.transform.position.y,
                                                                        collision.gameObject.transform.position.z - 3);
            }
            else
            {

                collision.gameObject.transform.position = new Vector3(collision.gameObject.transform.position.x,
                                                                        collision.gameObject.transform.position.y,
                                                                        collision.gameObject.transform.position.z + 3);
            }
        }

    }
}
