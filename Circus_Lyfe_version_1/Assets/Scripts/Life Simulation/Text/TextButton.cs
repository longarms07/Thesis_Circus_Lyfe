using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextButton : IButton
{

    private TextMeshProUGUI textMesh;
    private RectTransform rect;
    public Transform bgdRect;

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

    public void SetSizeDelta(Vector2 newSizeDelta, Vector2 bgdSizeDelta)
    {
        if(rect == null)
            rect = GetComponent<RectTransform>();
        Debug.Log(bgdRect.gameObject);
        rect.sizeDelta = newSizeDelta;
        Vector3 scale = bgdRect.localScale;
        scale.Set(bgdSizeDelta.x, bgdSizeDelta.y, bgdRect.localScale.z);
        Debug.Log(scale);
        bgdRect.localScale = scale;
        Debug.Log(bgdRect.localScale);
    }

    public float GetHeight() { return rect.sizeDelta.y; }
}
