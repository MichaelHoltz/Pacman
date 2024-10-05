using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject CurrentNode;
    public float Speed = 1.0f;
    public string direction = "";
    public string lastMovingDirection = "";
    public bool canWarp = true;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        NodeController currentNodeController = CurrentNode.GetComponent<NodeController>();
        transform.position = Vector2.MoveTowards(transform.position, CurrentNode.transform.position, Speed * Time.deltaTime);

        bool reverseDirection = false;

        if (
            (direction == "left" && lastMovingDirection == "right") ||
            (direction == "right" && lastMovingDirection == "left") ||
            (direction == "up" && lastMovingDirection == "down") ||
            (direction == "down" && lastMovingDirection == "up")
            )
        {
            reverseDirection = true;
        }

        //Figure out if we're at the center of our current node
        if ((transform.position.x == CurrentNode.transform.position.x && transform.position.y == CurrentNode.transform.position.y) || reverseDirection)
        {
            //If we reached the center of the left warp, warp to the right warp
            if (currentNodeController.IsWarpLeftNode && canWarp)
            {
                CurrentNode = gameManager.RightWarpNode;
                direction = "left;";
                lastMovingDirection = "left";
                transform.position = CurrentNode.transform.position;
                canWarp = false;
            }
            //if we reached the center of the right warp, warp to the left warp
            else if (currentNodeController.IsWarpRightNode && canWarp)
            {
                CurrentNode = gameManager.LeftWarpNode;
                direction = "right";
                lastMovingDirection = "right";
                transform.position = CurrentNode.transform.position;
                canWarp = false;
            }
            else
            {
                //Get the next node from our node controller using our current direction
                GameObject newNode = currentNodeController.GetNodeFromDirection(direction);

                //If we can move in the direction we're trying to move
                if (newNode != null)
                {
                    //Set our current node to the next node
                    CurrentNode = newNode;
                    lastMovingDirection = direction;
                }
                //we can't move in the desired direction, try to keep moving in the last direction
                else
                {
                    direction = lastMovingDirection;
                    newNode = currentNodeController.GetNodeFromDirection(direction);
                    if (newNode != null)
                    {
                        CurrentNode = newNode;
                    }
                }
            }

        }
        //not in the center of the current node
        else
        { 
            canWarp = true;

        }
    }

    public void SetDirection(string newDirection)
    {
        direction = newDirection;
    }
}
