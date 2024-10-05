using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public GameObject CurrentNode;
    public float Speed = 1.0f;
    public string direction = "";
    public string lastMovingDirection = "";

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

    public void SetDirection(string newDirection)
    {
        direction = newDirection;
    }
}
