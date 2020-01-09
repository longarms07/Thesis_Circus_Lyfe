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
    [Tooltip("Prefab to use for text button generation")]
    public GameObject textButtonPrefab;
    [Tooltip("The legal positions textbuttons can be in.")]
    public Vector3[] textButtonPositions;
    public GameObject timeText;
    

    private static TextboxManager instance;
    private TextMeshProUGUI textBox;
    private RectTransform canvasRect;
    private RectTransform textRect;
    private BoxCollider2D textCollider;
    private Button[] textButtons;
    private NPCInteractable notifyNPC;
    private TextMeshProUGUI timeTextMesh;
    private RectTransform timeTextRect;
    private GameManager gm;
    

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
        timeTextMesh = timeText.GetComponent<TextMeshProUGUI>();
        timeTextRect = timeText.GetComponent<RectTransform>();
        gm = GameManager.getInstance();



        float height = canvasRect.sizeDelta.y / heightPercent;
        textRect.sizeDelta = new Vector2(canvasRect.sizeDelta.x-textOffset, height-(2*textOffset));
        textRect.localPosition = new Vector3(textRect.localPosition.x+textOffset,
                                             -(canvasRect.sizeDelta.y/2)+height/2,
                                             textRect.localPosition.z);
        textBackground.transform.localScale = new Vector3(canvasRect.sizeDelta.x, height, 1);
        textBackground.transform.localPosition = new Vector3(textBackground.transform.localPosition.x,
                                                             -(canvasRect.sizeDelta.y / 2) + height/2,
                                                                textBackground.transform.localPosition.z);
        timeTextRect.sizeDelta = new Vector2(canvasRect.sizeDelta.x/5, canvasRect.sizeDelta.y/5);
        timeTextRect.transform.localPosition = new Vector3((canvasRect.sizeDelta.x/2),
                                                            canvasRect.sizeDelta.y / 2, timeTextRect.localPosition.z);

        UpdateDateTime();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void UpdateDateTime()
    {
        timeTextMesh.text = gm.GetCurrentDay() + "\n" + gm.GetCurrentTime();
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
            textBox.pageToDisplay = textBox.pageToDisplay + 1;
            Debug.Log("Page to display is now " + textBox.pageToDisplay);
        }
        else
        {
            TextBoxActive(false);
            notifyNPC.OnTextEnded();
            notifyNPC = null;
        }
    }

    public void SetText(string text, NPCInteractable target)
    {
        textBox.text = text;
        textBox.pageToDisplay = 1;
        notifyNPC = target;
    }

    public void GenerateTextButtons(IButtonListener target, string[] buttonText, int[] buttonCodes)
    {

        GameManager.getInstance().TogglePlayerMovement(false);
        if (buttonText.Length == buttonCodes.Length)
        {
            textButtons = new TextButton[buttonText.Length];
            for (int i = 0; i < Mathf.Min(buttonText.Length, textButtonPositions.Length); i++)
            {
                GameObject btn = Instantiate(textButtonPrefab);
                btn.name = "TextButton#" + buttonCodes[i];
                TextButton txt = btn.GetComponent<TextButton>();
                btn.transform.SetParent(textCanvas.transform);
                btn.transform.localPosition = textButtonPositions[i];
                txt.SetListener(target);
                txt.SetText(buttonText[i]);
                txt.SetButtonCode(buttonCodes[i]);
                textButtons[i] = txt;
            }
        }
    }

    public void DespawnTextButtons()
    {
        foreach(TextButton btn in textButtons)
        {
            Destroy(btn.gameObject);
        }
        GameManager.getInstance().TogglePlayerMovement(true);
    }



}
