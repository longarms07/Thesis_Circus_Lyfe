using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextButton : Button
{

    private TextMeshProUGUI textMesh;
    private RectTransform rect;

    // Start is called before the first frame update
    void Start()
    {
        initVariables();
        textMesh = GetComponent<TextMeshProUGUI>();
        rect = GetComponent<RectTransform>();
        rect.localScale = new Vector3(1, 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetText(string newText) {
        if(textMesh == null) textMesh = GetComponent<TextMeshProUGUI>();
        textMesh.text = newText;
    }

    public float GetHeight() { return rect.sizeDelta.y; }
}
