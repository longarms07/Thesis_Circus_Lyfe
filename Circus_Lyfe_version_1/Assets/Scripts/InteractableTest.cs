using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableTest : MonoBehaviour, IInteractable
{
    // Start is called before the first frame update
    void Start()
    {
        GameManager.getInstance().RegisterInteractable(this.gameObject.transform, this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnInteraction()
    {
        Debug.Log(this.gameObject.name + " has been interacted with");
    }
    public void InRange(bool inRange)
    {
        if (inRange)
            Debug.Log(this.gameObject.name + "Is in Range of Player");
        else Debug.Log(this.gameObject.name + "Is not in Range of Player");
    }

}
