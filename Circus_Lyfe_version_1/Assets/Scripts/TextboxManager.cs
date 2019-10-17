using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextboxManager : MonoBehaviour
{

    [Tooltip("The TextMeshPro for the textbox")]
    public GameObject textMeshPro;
    [Tooltip("The background for the textbox")]
    public GameObject textBackground;
    [Tooltip("The canvase the textbox is attacthed to")]
    public GameObject textCanvas;
    [Tooltip("Percentage of the screen height to cover with textbox")]
    public float heightPercent;
    [Tooltip("Offset for text from the sides of the textbox")]
    public float textOffset;


    private static TextboxManager instance;
    private TextMeshProUGUI textBox;
    private RectTransform canvasRect;
    private RectTransform textRect;
    private BoxCollider2D textCollider;
    

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        textBox = textMeshPro.GetComponent<TextMeshProUGUI>();
        if (textBox == null) Destroy(this.gameObject);
        canvasRect = textCanvas.GetComponent<RectTransform>();
        if (canvasRect == null) Destroy(this.gameObject);
        textRect = textMeshPro.GetComponent<RectTransform>();
        if (textRect == null) Destroy(this.gameObject);
        textCollider = textBackground.AddComponent<BoxCollider2D>();



        float height = canvasRect.sizeDelta.y / heightPercent;
        textRect.sizeDelta = new Vector2(canvasRect.sizeDelta.x-textOffset, height-(2*textOffset));
        textRect.localPosition = new Vector3(textRect.localPosition.x+textOffset,
                                             -(canvasRect.sizeDelta.y / heightPercent),
                                             textRect.localPosition.z);
        textBackground.transform.localScale = new Vector3(canvasRect.sizeDelta.x, height, 1);
        textBackground.transform.localPosition = new Vector3(textBackground.transform.localPosition.x, 
                                                                -(canvasRect.sizeDelta.y/heightPercent), 
                                                                textBackground.transform.localPosition.z);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public static TextboxManager GetInstance() { return instance; }

    public void TextBoxActive(bool activate)
    {
        textBox.pageToDisplay = 1;
        textMeshPro.SetActive(activate);
        textBackground.SetActive(activate);
        GameManager.getInstance().TogglePlayerMovement(!activate);
    }

    public void OnTap()
    {
        Debug.Log("On tap called");
        if (textBox.pageToDisplay < textBox.textInfo.pageCount)
        {
            textBox.pageToDisplay = textBox.pageToDisplay+1;
            Debug.Log("Page to display is now " + textBox.pageToDisplay);
        }
        else TextBoxActive(false);
    }

    public void SetText(string text)
    {
        textBox.text = text;
        textBox.pageToDisplay = 1;
    }

    


}
