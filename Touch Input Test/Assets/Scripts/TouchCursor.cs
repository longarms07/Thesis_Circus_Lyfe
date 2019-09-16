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
        touchEnded = false;
        Debug.Log("Created touch cursor object");
    }

    // Update is called once per frame
    void Update()
    {
        fadeCursor();
    }

    // Creates new cursor, adding the past one to the list if it exisits
    public void changePosition(Vector3 touchPosition)
    {
        if (currentCursor != null) { addPast(); }
        currentCursor = Instantiate(touchCursorPrefab);
        currentCursor.transform.position = touchPosition;
        Debug.Log("Created a new current cursor");
    }

    //Adds the previous cursor to the list, decreases its scale
    void addPast()
    {
        if (currentCursor != null)
        {
            pastCursors.Add(currentCursor);
            currentCursor.transform.localScale = new Vector3(currentCursor.transform.localScale.x / 2,
                                                             currentCursor.transform.localScale.y / 2,
                                                             currentCursor.transform.localScale.z / 2);
            pastCursorSprites.Add(currentCursor.GetComponent<SpriteRenderer>());
            Debug.Log("Past cursor is now in the list");
        }
    }

    public void endTouch()
    {
        Debug.Log("Touch has ended");
        touchEnded = true;
        addPast();
        currentCursor = null;
    }

    //Go through the list, and fade the previous cursors at the given rate. When it's invisible remove them from the list.
    void fadeCursor()
    {
        List<int> toRemove = new List<int>();
        for(int i = 0; i < pastCursors.Count; i++)
        {
            SpriteRenderer sprite = pastCursorSprites[i];
            float newAlpha = sprite.color.a - fadeSpeed;
            if (newAlpha > 0)
            {
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.g, sprite.color.a - fadeSpeed);
            }
            else
            {
                toRemove.Add(i);
            }
        }
        foreach(int i in toRemove)
        {
            GameObject pastCursor = pastCursors[i];
            pastCursors.Remove(pastCursor);
            SpriteRenderer sprite = pastCursorSprites[i];
            pastCursorSprites.Remove(sprite);
            Destroy(pastCursor);
        }
        if(pastCursors.Count == 0 && touchEnded)
        {
            destroyThis();
        }
    }

    private void destroyThis()
    {
        if(touchEnded && currentCursor == null && pastCursors.Count == 0)
        {
            Debug.Log("Destroying this...");
            Destroy(this.gameObject);
        }
    }

}
