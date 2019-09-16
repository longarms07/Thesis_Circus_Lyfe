using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchCursor : MonoBehaviour
{


    public GameObject touchCursorPrefab;
    public int fadeSpeed;

    private bool touchEnded;
    private GameObject currentCursor;
    private List<GameObject> pastCursors;
    private List<SpriteRenderer> pastCursorSprites;

    // Start is called before the first frame update
    void Start()
    {
        pastCursors = new List<GameObject>();
        pastCursorSprites = new List<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //


}
