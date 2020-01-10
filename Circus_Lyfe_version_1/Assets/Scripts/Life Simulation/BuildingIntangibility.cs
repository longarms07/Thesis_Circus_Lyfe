using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingIntangibility : MonoBehaviour
{

    public GameObject spriteTarget;
    public float zAmount;
    public float opacity;
    private SpriteRenderer spriteR;
    private Vector3 setTo;
    private Vector3 original;
    private Color oColor;
    private Color transparent;

    // Start is called before the first frame update
    void Start()
    {
        spriteR = spriteTarget.GetComponent<SpriteRenderer>();
        original = spriteTarget.transform.localPosition;
        setTo = new Vector3(original.x, original.y, zAmount);
        oColor = spriteR.color;
        transparent = new Color(oColor.r, oColor.g, oColor.b, opacity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject == GameManager.getInstance().playerAvatar)
        {
            spriteTarget.transform.localPosition = setTo;
            spriteR.color = transparent;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == GameManager.getInstance().playerAvatar)
        {
            spriteTarget.transform.localPosition = original;
            spriteR.color = oColor;
        }
    }



}
