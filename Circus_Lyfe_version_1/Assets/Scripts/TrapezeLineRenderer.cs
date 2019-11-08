using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapezeLineRenderer : MonoBehaviour
{
    LineRenderer ln;
    public GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        ln = gameObject.AddComponent<LineRenderer>();
        ln.sortingLayerName = "Default";
        ln.SetWidth(0.1f, 0.1f);
        ln.SetVertexCount(2);
        ln.SetPosition(0, gameObject.transform.position);
        ln.SetPosition(1, target.transform.position);
        ln.useWorldSpace = true;
    }

    // Update is called once per frame
    void Update()
    {
        ln.SetPosition(1, target.transform.position);
    }
}
