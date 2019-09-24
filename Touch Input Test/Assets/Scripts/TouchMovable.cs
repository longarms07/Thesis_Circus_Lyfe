using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchMovable : MonoBehaviour
{

    public int touchToFollow;
    public float moveSpeed;

    TouchInputManager touchInputManager;
    private Rigidbody2D rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        touchInputManager = TouchInputManager.getInstance();
        if(touchInputManager == null)
        {
            Debug.LogError("Touch Movable Error: There is no TouchInputManager!");
            Destroy(this.gameObject);
        }
        else
        {
            rigidBody = this.gameObject.GetComponent<Rigidbody2D>();
            //touchInputManager.subscribeTouchMovement(this, touchToFollow);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Update movement of the TouchMoveable
    public void move(Vector2 deltaPosition, float deltaTime)
    {
        //Vector3 transformVector = new Vector3(deltaPosition.x * deltaTime, deltaPosition.y * deltaTime, 0);
        //this.gameObject.transform.Translate(transformVector, Space.World);
        Vector3 currentPosition = this.gameObject.transform.localPosition;
        Vector3 transformVector = new Vector3(currentPosition.x+deltaPosition.x, currentPosition.y+deltaPosition.y, 0);
        Vector3 moveTowards = Vector3.MoveTowards(currentPosition, transformVector, deltaTime*moveSpeed);
        rigidBody.MovePosition(moveTowards);
        //Debug.Log("TouchMovable: Moving " + transformVector.x + " x, " + transformVector.y + " y.");
    }
}
