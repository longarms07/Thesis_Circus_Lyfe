using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TrickGUI : MonoBehaviour
{
    
    public GameObject textObject;
    public GameObject scoreObject;
    public GameObject quitButton;
    public GameObject canvasObject;
    public Vector2 screenScale;

    private TextMeshProUGUI textTMP;
    private RectTransform textRect;
    private TextMeshProUGUI scoreTMP;
    private RectTransform scoreRect;
    private int score = 0;
    private static TrickGUI instance;
    private Canvas c;
    private RectTransform cRect;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this.gameObject);
    }


    // Start is called before the first frame update
    void Start()
    {
        c = canvasObject.GetComponent<Canvas>();
        cRect = canvasObject.GetComponent<RectTransform>();
        textRect = textObject.GetComponent<RectTransform>();
        scoreRect = scoreObject.GetComponent<RectTransform>();
        /*Vector2 screenScale2 = new Vector2(cRect.sizeDelta.x / screenScale.x, cRect.sizeDelta.y / screenScale.y);
        textRect.sizeDelta = textRect.sizeDelta * screenScale2;
        scoreRect.sizeDelta = scoreRect.sizeDelta * screenScale2;
        RectTransform quitRect = quitButton.GetComponent<RectTransform>();
        quitRect.sizeDelta = quitRect.sizeDelta * screenScale2;
        scoreRect.localPosition = new Vector2(Mathf.Abs((cRect.sizeDelta.x / 2) - (textRect.sizeDelta.x/2)), Mathf.Abs((cRect.sizeDelta.y / 2)-scoreRect.sizeDelta.y));
        //Debug.Log(scoreRect.localPosition + " should be " + new Vector2(Mathf.Abs((cRect.sizeDelta.x / 2) - (textRect.sizeDelta.x)), Mathf.Abs(cRect.sizeDelta.y / 2)));
        textRect.localPosition = new Vector2(scoreRect.localPosition.x, scoreRect.localPosition.y - scoreRect.sizeDelta.y);
        quitRect.localPosition = new Vector3(-scoreRect.localPosition.x, scoreRect.localPosition.y, quitButton.transform.position.z);
        */
        textTMP = textObject.GetComponent<TextMeshProUGUI>();
        scoreTMP = scoreObject.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DidTrick(string trick, int addThis)
    {
        textTMP.text = trick;
        score += addThis;
        scoreTMP.text = "Score: " + score;
    }

    public void DecreaseScore(int decrease)
    {
        score -= Mathf.Abs(decrease);
        if (score < 0) score = 0;
        scoreTMP.text = "Score: " + score;
    }
    
    public void DisplayTrickCode(List<SwipeDirection> trickCode)
    {
        string text = "Current Swipes: \n";
        foreach(SwipeDirection swipe in trickCode)
        {
            switch (swipe)
            {
                case SwipeDirection.North:
                    text += "\u2191 ";
                    break;
                case SwipeDirection.NorthWest:
                    text += "\u2196 ";
                    break;
                case SwipeDirection.NorthEast:
                    text += "\u2197 ";
                    break;
                case SwipeDirection.South:
                    text += "\u2193 ";
                    break;
                case SwipeDirection.SouthWest:
                    text += "\u2199 ";
                    break;
                case SwipeDirection.SouthEast:
                    text += "\u2198 ";
                    break;
                case SwipeDirection.East:
                    text += "\u2192 ";
                    break;
                case SwipeDirection.West:
                    text += "\u2190 ";
                    break;
            }
        }
        textTMP.text = text;
    }

    public static TrickGUI GetInstance() { return instance; }

    public int GetScore() { return score; }
}
