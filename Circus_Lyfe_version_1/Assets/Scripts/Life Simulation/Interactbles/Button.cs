using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public GameObject colliderObj;

    private Collider2D btnCollider;
    public IButtonListener listener;
    public int buttonCode = -1;
    private GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        initVariables();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTap()
    {
        if (listener != null)
        {
            listener.OnButtonPressed(buttonCode);
            Debug.Log(name + "I've been tapped!");
        }
    }

    public void SetButtonCode(int code) { buttonCode = code; }
    public void SetListener(IButtonListener l) { listener = l; }

    private void OnDestroy()
    {
        if(gm!=null) gm.DeregisterButton(btnCollider.transform);
    }

    protected void initVariables()
    {
        btnCollider = colliderObj.GetComponent<Collider2D>();
        gm = GameManager.getInstance();
        if (gm == null) gm = GameManager_Trapeze.GetInstance();
        gm.RegisterButton(btnCollider.transform, this);
    }
}
