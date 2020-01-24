using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.UI;

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
    public int maxNumButtons;
    public float buttonOffset;
    public float buttonHeightPercent;
    public GameObject timeText;
    public SpriteRenderer imageView;
    public List<ImageStruct> imageStructs;
    

    private static TextboxManager instance;
    private TextMeshProUGUI textBox;
    private RectTransform canvasRect;
    private RectTransform textRect;
    private BoxCollider2D textCollider;
    private List<Button> textButtons;
    private Vector2 buttonSizeDelta;
    private Vector2 buttonBgrdSizeDelta;
    private ITextboxListener notifyTarget;
    private TextMeshProUGUI timeTextMesh;
    private RectTransform timeTextRect;
    private GameManager gm;
    private DialogueUI dUI;
    private bool lineComplete;
    private bool checkBtnSize;
    private Dictionary<string, ImageStruct> imageDict;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
        textButtons = new List<Button>();
        for (int i = 0; i < maxNumButtons; i++)
        {
            GameObject btn = Instantiate(textButtonPrefab);
            textButtons.Add(btn.GetComponent<Button>());
        }
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
        dUI = GetComponent<DialogueUI>();
        lineComplete = false;
        imageView.enabled = false;
        imageDict = new Dictionary<string, ImageStruct>();
        foreach (ImageStruct i in imageStructs)
        {
            imageDict.Add(i.name, i);
        }


        float height = canvasRect.sizeDelta.y / heightPercent;
        textRect.sizeDelta = new Vector2(canvasRect.sizeDelta.x-(2*textOffset), height-(2*textOffset));
        textRect.localPosition = new Vector3(textRect.transform.localPosition.x,
                                             -(canvasRect.sizeDelta.y/2)+height/2,
                                             textRect.localPosition.z);
        textBackground.transform.localScale = new Vector3(canvasRect.sizeDelta.x, height, 1);
        textBackground.transform.localPosition = new Vector3(textBackground.transform.localPosition.x,
                                                             -(canvasRect.sizeDelta.y / 2) + height/2,
                                                                textBackground.transform.localPosition.z);
        timeTextRect.sizeDelta = new Vector2(canvasRect.sizeDelta.x/5, canvasRect.sizeDelta.y/5);
        timeTextRect.transform.localPosition = new Vector3((canvasRect.sizeDelta.x/2),
                                                            canvasRect.sizeDelta.y / 2, timeTextRect.localPosition.z);
        textButtonPositions = new Vector3[maxNumButtons];
        buttonHeightPercent = canvasRect.sizeDelta.y / buttonHeightPercent;
        buttonSizeDelta = new Vector2(textRect.sizeDelta.x - (2*textOffset) ,buttonHeightPercent / maxNumButtons);
        buttonBgrdSizeDelta = new Vector2(textRect.sizeDelta.x, buttonSizeDelta.y);
        Debug.Log("Button size delta: " + buttonSizeDelta + ", Bgd size delta: " + buttonBgrdSizeDelta);
        textButtonPositions[maxNumButtons - 1] = new Vector3(0, textRect.localPosition.y + buttonSizeDelta.y+buttonOffset,
            textRect.localPosition.z);
        for(int i = maxNumButtons-2; i >=0; i--)
        {
            textButtonPositions[i] = new Vector3(0, textButtonPositions[i+1].y + buttonSizeDelta.y+buttonOffset,
                textRect.localPosition.z);
        }
        //textButtons = new List<Button>();
        for (int i = 0; i < maxNumButtons; i++)
        {
            Button btn = textButtons[i];
            btn.gameObject.name = "TextButton#" + i;
            /*TextButton txt = btn.GetComponent<TextButton>();
            textButtons[i] = txt;
            txt.SetSizeDelta(buttonSizeDelta, buttonBgrdSizeDelta);*/
            btn.transform.SetParent(textCanvas.transform);
            btn.transform.localPosition = textButtonPositions[i];
            RectTransform[] rects = GetComponentsInChildren<RectTransform>();
            btn.transform.localScale = Vector3.one;
            btn.GetComponent<BoxCollider2D>().size = buttonBgrdSizeDelta;
            btn.gameObject.SetActive(false);
        }
        checkBtnSize = true;
        dUI.optionButtons = textButtons;

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
        textBox.text = "";
        textBox.pageToDisplay = 1;
        textMeshPro.SetActive(activate);
        textBackground.SetActive(activate);
        HideButtons();
        if (checkBtnSize && textButtons[0].GetComponent<RectTransform>().sizeDelta != buttonSizeDelta)
        {
            foreach (Button btn in textButtons)
            {
                RectTransform rect = btn.GetComponent<RectTransform>();
                rect.sizeDelta = buttonBgrdSizeDelta;
            }
            checkBtnSize = false;
        }
        if (!activate) HideImage();
        GameManager.getInstance().TogglePlayerMovement(!activate);
    }

    public void HideTextBox(bool hide)
    {
        if (!textBox.text.Equals(""))
        {
            textMeshPro.SetActive(!hide);
            textBackground.SetActive(!hide);
            HideImage();
        }
    }

    public void HideButtons()
    {
        foreach (Button btn in textButtons)
        {
            btn.gameObject.SetActive(false);
        }
    }

    public void OnTap()
    {
        Debug.Log("On tap called");
        if (textBox.pageToDisplay < textBox.textInfo.pageCount)
        {
            textBox.pageToDisplay = textBox.pageToDisplay + 1;
            Debug.Log("Page to display is now " + textBox.pageToDisplay);
        }
        else if (lineComplete)
        {
            lineComplete = false;
            dUI.MarkLineComplete();
        }
    }

    public void NotifyMe(ITextboxListener me)
    {
        notifyTarget = me;
    }

    public void OnTextComplete()
    {
        textBox.pageToDisplay = 1;
        TextBoxActive(false);
        if(notifyTarget!=null) notifyTarget.OnTextEnded();
        
        notifyTarget = null;
    }

    public void LineComplete(bool b)
    {
        lineComplete = b;
    }

    public void SetText(string text, ITextboxListener target)
    {
        textBox.text = text;
        textBox.pageToDisplay = 1;
        notifyTarget = target;
    }

    public void SetText(string text)
    {
        Debug.Log("Here" + text);
        textBox.text = text;
        textBox.pageToDisplay = 1;
    }

    public void GenerateTextButtons(IButtonListener target, string[] buttonText, int[] buttonCodes)
    {

        GameManager.getInstance().TogglePlayerMovement(false);
        if (buttonText.Length == buttonCodes.Length)
        {
            textButtons = new List<Button>();
            for (int i = 0; i < Mathf.Min(buttonText.Length, textButtonPositions.Length); i++)
            {
                /*IButton btn = textButtons[i];
                btn.gameObject.SetActive(true);
                TextButton txt = btn.gameObject.GetComponent<TextButton>();
                txt.SetListener(target);
                txt.SetText(buttonText[i]);
                txt.SetButtonCode(buttonCodes[i]);*/
                Button btn = textButtons[i];
                btn.gameObject.SetActive(true);
            }
        }
    }

    public void DespawnTextButtons()
    {
        /*foreach(TextButton btn in textButtons)
        {
            btn.gameObject.SetActive(false);
        }*/
        GameManager.getInstance().TogglePlayerMovement(true);
    }

    [YarnCommand("ShowImage")]
    public void ShowImage(string imagename)
    {
        Debug.Log(imageDict.ContainsKey(imagename) + ", " + imageDict.Keys);
        if (imageDict.ContainsKey(imagename))
        {
            ImageStruct imageStruct = imageDict[imagename];
            imageView.sprite = imageStruct.image;
            imageView.transform.localScale = imageStruct.scale;
            imageView.gameObject.SetActive(true);
            imageView.enabled = true;
        }
    }

    [YarnCommand("HideImage")]
    public void HideImage()
    {
        imageView.gameObject.SetActive(false);
    }

    


}
