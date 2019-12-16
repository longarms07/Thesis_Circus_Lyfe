using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TrickGUI : MonoBehaviour
{

    public GameObject textObject;
    public GameObject scoreObject;

    private TextMeshProUGUI textTMP;
    private TextMeshProUGUI scoreTMP;
    private int score = 0;
    private static TrickGUI instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this.gameObject);
    }


    // Start is called before the first frame update
    void Start()
    {
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
    

    public static TrickGUI GetInstance() { return instance; }
}
