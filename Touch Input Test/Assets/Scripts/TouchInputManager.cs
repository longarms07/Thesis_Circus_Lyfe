using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TouchInputManager : MonoBehaviour
{

    public GameObject displayText;

    private TextMeshProUGUI display;
    static TouchInputManager instance;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null) { instance = this; }
        else { Destroy(this); }
        display = displayText.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        string text = "Size of Input.touches " + Input.touches.Length;
        if (display != null) { display.text = text; }

    }

    public static TouchInputManager getInstance()
    {
        return instance;
    }
}
