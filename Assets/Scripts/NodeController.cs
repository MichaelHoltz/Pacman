using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NodeController : MonoBehaviour
{
    public bool CanMoveLeft = false;
    public bool CanMoveRight = false;
    public bool CanMoveUp = false;
    public bool CanMoveDown = false;
    public GameObject NodeLeft;
    public GameObject NodeRight;
    public GameObject NodeUp;
    public GameObject NodeDown;

    public bool IsWarpRightNode = false;
    public bool IsWarpLeftNode = false;

    //If the node contains a pellet when the game starts
    public bool IsPelletNode = false;
    //If the node sill has a pellet;
    public bool HasPellet = false;

    public SpriteRenderer pelletSprite;

    private void Awake()
    {
        if(transform.childCount > 0)
        {
            IsPelletNode = true;
            HasPellet = true;
            pelletSprite = transform.GetComponentInChildren<SpriteRenderer>();
        }

        RaycastHit2D[] hitsDown;
        //Shoot a raycast line going down
        hitsDown = Physics2D.RaycastAll(transform.position, Vector2.down);

        //loop through all the hits
        for (int i = 0; i < hitsDown.Length; i++)
        {
            float distance = Mathf.Abs(hitsDown[i].point.y - transform.position.y);
            if (distance < 0.4f)
            {
                CanMoveDown = true;
                NodeDown = hitsDown[i].collider.gameObject;
            }
        }

        RaycastHit2D[] hitsUp;
        //Shoot a raycast line going up
        hitsUp = Physics2D.RaycastAll(transform.position, Vector2.up);

        //loop through all the hits
        for (int i = 0; i < hitsUp.Length; i++)
        {
            {
                float distance = Mathf.Abs(hitsUp[i].point.y - transform.position.y);
                if (distance < 0.4f)
                {
                    CanMoveUp = true;
                    NodeUp = hitsUp[i].collider.gameObject;
                }
            }
        }
        RaycastHit2D[] hitsRight;
        //Shoot a raycast line going Right
        hitsRight = Physics2D.RaycastAll(transform.position, Vector2.right);

        //loop through all the hits
        for (int i = 0; i < hitsRight.Length; i++)
        {
            {
                float distance = Mathf.Abs(hitsRight[i].point.x - transform.position.x);
                if (distance < 0.4f)
                {
                    CanMoveRight = true;
                    NodeRight = hitsRight[i].collider.gameObject;
                }
            }
        }

        RaycastHit2D[] hitsLeft;
        //Shoot a raycast line going Left
        hitsLeft = Physics2D.RaycastAll(transform.position, Vector2.left);

        //loop through all the hits
        for (int i = 0; i < hitsLeft.Length; i++)
        {
            {
                float distance = Mathf.Abs(hitsLeft[i].point.x - transform.position.x);
                if (distance < 0.4f)
                {
                    CanMoveLeft = true;
                    NodeLeft = hitsLeft[i].collider.gameObject;
                }
            }
        }

    }



    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject GetNodeFromDirection(string direction)
    { 
        if(direction == "left" && CanMoveLeft)
        {
            return NodeLeft;
        }
        else if (direction == "right" && CanMoveRight)
        {
            return NodeRight;
        }
        else if (direction == "up" && CanMoveUp)
        {
            return NodeUp;
        }
        else if (direction == "down" && CanMoveDown)
        {
            return NodeDown;
        }
        else
        {
            return null;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 7 && HasPellet)
        {
            HasPellet = false;
            pelletSprite.enabled = false;
        }
    }

}
