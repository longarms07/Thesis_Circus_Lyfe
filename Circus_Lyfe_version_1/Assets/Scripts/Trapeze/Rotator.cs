using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{

    public float rotationDegrees;
    public float rotationSpeed;
    public bool inversed;
    private float rotatedDegrees;
    private GameManager_Trapeze gm;


    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager_Trapeze.GetInstance();
        rotatedDegrees = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gm.paused)
        {
            if (!inversed)
            {
                this.gameObject.transform.Rotate(0, 0, rotationSpeed);
                rotatedDegrees += rotationSpeed;
                if (rotatedDegrees >= rotationDegrees)
                {
                    inversed = true;
                }
            }
            else
            {
                this.gameObject.transform.Rotate(0, 0, -rotationSpeed);
                rotatedDegrees -= rotationSpeed;
                if (rotatedDegrees <= -rotationDegrees)
                {
                    inversed = false;
                }
            }
        }
        
    }

    public Vector3 GetMomentum(Rigidbody2D momentumRB)
    {
        float xMomentum = 0;
        float yMomentum = 0;


        return new Vector3(xMomentum, yMomentum, 0);
    }


}
