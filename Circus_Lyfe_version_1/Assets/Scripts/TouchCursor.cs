using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchCursor : MonoBehaviour
{
    

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    // Changes the cursor position, and creates a new one if needed
    public void changePosition(Vector3 touchPosition)
    {
        this.gameObject.transform.position = touchPosition;
    }

    //Adds the previous cursor to the list, decreases its scale
    

    public void endTouch()
    {
        Debug.Log("Touch has ended");
        destroyThis();
    }
    
    private void destroyThis()
    {
      
            Debug.Log("Destroying this...");
            Destroy(this.gameObject);
    }

}
