using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverseRotation : MonoBehaviour
{
    public bool active;

    Quaternion lastRotation;

    // Start is called before the first frame update
    void Start()
    {
        lastRotation = gameObject.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            Quaternion r = gameObject.transform.rotation;
            Quaternion difference = new Quaternion(r.x - lastRotation.x, r.y - lastRotation.y, r.z - lastRotation.z, r.w - lastRotation.w);
            //difference = Quaternion.Inverse(difference);
            lastRotation = new Quaternion(lastRotation.x, lastRotation.y, lastRotation.z - difference.z, lastRotation.w);

            gameObject.transform.rotation = lastRotation;
        }
    }
}
