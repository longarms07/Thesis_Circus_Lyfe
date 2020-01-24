using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class CutsceneController : MonoBehaviour
{
    [Tooltip("The position in which this object spawns in the trapeze tent")]
    public Vector3 TrapezeTentPos;
    [Tooltip("The position in which this object spawns in front of the player house")]
    public Vector3 PlayerHousePos;
    public float maxMove;
    private Rigidbody2D rb;
    private Vector3 targetPos;
    bool moveActive = false;



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (moveActive && transform.position != targetPos)
        {
            rb.MovePosition(Vector3.MoveTowards(transform.position, targetPos, maxMove * Time.deltaTime));
            if (rb.transform.position == targetPos) moveActive = false;
        }   
    }

    [YarnCommand("TeleportTrapezeTent")]
    public void TeleportTrapezeTent()
    {
        transform.position = TrapezeTentPos;
        moveActive = false;
    }

    [YarnCommand("TeleportPlayerHouse")]
    public void TeleportPlayerHouse()
    {
        transform.position = PlayerHousePos;
        moveActive = false;
    }

    [YarnCommand("Move")]
    public void MoveTo(string dir, string number)
    {
        float num = float.Parse(number);
        Vector3 direction;
        dir = dir.ToLower();
        switch (dir)
        {
            case "left":
                direction = Vector3.left;
                break;
            case "right":
                direction = Vector3.right;
                break;
            case "up":
                direction = Vector3.up;
                break;
            case "down":
                direction = Vector3.down;
                break;
            default:
                direction = Vector3.zero;
                break;
        }
        direction = direction * num;
        targetPos = transform.position + direction;
        moveActive = true;
    }


}
