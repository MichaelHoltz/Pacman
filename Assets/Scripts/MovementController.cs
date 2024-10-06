using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public event EventHandler OnDirectionChanged;

    public GameManager gameManager;
    public GameObject CurrentNode;
    public float Speed = 1.0f;
    public string Direction = "";
    private string _lastMovingDirection = "";
    private bool _canWarp = true;
    
    private bool _canMove = false;

    private void Awake()
    {

    }

    public void StartGame()
    {
        Direction = "left";
        _lastMovingDirection = "left";
        _canMove = true;
    }

    // Update is called once per frame
    private void Update()
    {
        if (_canMove)
        {


            NodeController currentNodeController = CurrentNode.GetComponent<NodeController>();
            transform.position = Vector2.MoveTowards(transform.position, CurrentNode.transform.position, Speed * Time.deltaTime);

            bool reverseDirection = false;

            if (
                (Direction == "left" && _lastMovingDirection == "right") ||
                (Direction == "right" && _lastMovingDirection == "left") ||
                (Direction == "up" && _lastMovingDirection == "down") ||
                (Direction == "down" && _lastMovingDirection == "up")
                )
            {
                reverseDirection = true;
            }

            //Figure out if we're at the center of our current node
            if ((transform.position.x == CurrentNode.transform.position.x && transform.position.y == CurrentNode.transform.position.y) || reverseDirection)
            {
                //If we reached the center of the left warp, warp to the right warp
                if (currentNodeController.IsWarpLeftNode && _canWarp)
                {
                    CurrentNode = gameManager.RightWarpNode;
                    Direction = "left;";
                    _lastMovingDirection = "left";
                    transform.position = CurrentNode.transform.position;
                    _canWarp = false;
                }
                //if we reached the center of the right warp, warp to the left warp
                else if (currentNodeController.IsWarpRightNode && _canWarp)
                {
                    CurrentNode = gameManager.LeftWarpNode;
                    Direction = "right";
                    _lastMovingDirection = "right";
                    transform.position = CurrentNode.transform.position;
                    _canWarp = false;
                }
                else
                {
                    //Get the next node from our node controller using our current direction
                    GameObject newNode = currentNodeController.GetNodeFromDirection(Direction);

                    //If we can move in the direction we're trying to move
                    if (newNode != null)
                    {
                        //Set our current node to the next node
                        CurrentNode = newNode;
                        if (_lastMovingDirection != Direction)
                        {
                            OnDirectionChanged?.Invoke(this, EventArgs.Empty);
                        }
                        _lastMovingDirection = Direction;

                    }
                    //we can't move in the desired direction, try to keep moving in the last direction
                    else
                    {
                        Direction = _lastMovingDirection;
                        newNode = currentNodeController.GetNodeFromDirection(Direction);
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
                _canWarp = true;

            }
        }
    }

    public void SetDirection(string newDirection)
    {
        Direction = newDirection;
    }
}
