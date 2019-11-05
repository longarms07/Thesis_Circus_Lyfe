using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockRotation : MonoBehaviour
{

    Quaternion rotation;

    // Start is called before the first frame update
    void Start()
    {
        rotation = gameObject.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.rotation = rotation;
    }
}
