using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{

    private Collider2D btnCollider;
    private IButtonListener listener;
    private int buttonCode;


    // Start is called before the first frame update
    void Start()
    {
        buttonCode = -1;
        btnCollider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTap()
    {
        if(listener!=null) listener.OnButtonPressed(buttonCode);
    }

    public void SetButtonCode(int code) { buttonCode = code; }
    public void SetListener(IButtonListener l) { listener = l; }
}
