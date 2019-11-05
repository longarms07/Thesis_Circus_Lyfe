using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{

    public float rotationDegrees;
    public float rotationSpeed;
    public bool inversed;
    private float rotatedDegrees;


    // Start is called before the first frame update
    void Start()
    {
        rotatedDegrees = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!inversed)
        {
            this.gameObject.transform.Rotate(0, 0, rotationSpeed);
            rotatedDegrees+=rotationSpeed;
            if(rotatedDegrees >= rotationDegrees)
            {
                inversed = true;
            }
        }
        else
        {
            this.gameObject.transform.Rotate(0, 0, -rotationSpeed);
            rotatedDegrees-=rotationSpeed;
            if(rotatedDegrees <= -rotationDegrees)
            {
                inversed = false;
            }
        }
        
    }
}
