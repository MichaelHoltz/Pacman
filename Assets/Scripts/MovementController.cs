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
    public NodeController.Directions Direction = NodeController.Directions.None;
    public NodeController.Directions LastMovingDirection = NodeController.Directions.None;
    private bool _canWarp = true;
    
    private bool _canMove = false;
    [SerializeField] private bool _isGhost = false;

    private void Awake()
    {

    }

    public void StartGame()
    {
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
                (Direction == NodeController.Directions.Left && LastMovingDirection == NodeController.Directions.Right) ||
                (Direction == NodeController.Directions.Right && LastMovingDirection == NodeController.Directions.Left) ||
                (Direction == NodeController.Directions.Up && LastMovingDirection == NodeController.Directions.Down) ||
                (Direction == NodeController.Directions.Down && LastMovingDirection == NodeController.Directions.Up)
                )
            {
                reverseDirection = true;
            }

            //Figure out if we're at the center of our current node
            if ((transform.position.x == CurrentNode.transform.position.x && transform.position.y == CurrentNode.transform.position.y) || reverseDirection)
            {
                if (_isGhost)
                {
                    GetComponent<EnemyController>().ReachedCenterOfNode(currentNodeController);
                }

                //If we reached the center of the left warp, warp to the right warp
                if (currentNodeController.IsWarpLeftNode && _canWarp)
                {
                    CurrentNode = gameManager.RightWarpNode;
                    Direction = NodeController.Directions.Left;
                    LastMovingDirection = NodeController.Directions.Left;
                    transform.position = CurrentNode.transform.position;
                    _canWarp = false;
                }
                //if we reached the center of the right warp, warp to the left warp
                else if (currentNodeController.IsWarpRightNode && _canWarp)
                {
                    CurrentNode = gameManager.LeftWarpNode;
                    Direction = NodeController.Directions.Right;
                    LastMovingDirection = NodeController.Directions.Right;
                    transform.position = CurrentNode.transform.position;
                    _canWarp = false;
                }
                else
                {
                    //if we are not a ghost that is respawning, and we are on the start node, and we are trying to move down, stop
                    if(currentNodeController.IsGhostStartingNode && Direction == NodeController.Directions.Down && 
                        (!_isGhost || GetComponent<EnemyController>().GhostNodesState != EnemyController.GhostNodesStatesEnum.Respawning))
                    {
                        Direction = LastMovingDirection;
                    }

                    //Get the next node from our node controller using our current direction
                    GameObject newNode = currentNodeController.GetNodeFromDirection(Direction);

                    //If we can move in the direction we're trying to move
                    if (newNode != null)
                    {
                        //Set our current node to the next node
                        CurrentNode = newNode;
                        if (LastMovingDirection != Direction)
                        {
                            OnDirectionChanged?.Invoke(this, EventArgs.Empty);
                        }
                        LastMovingDirection = Direction;

                    }
                    //we can't move in the desired direction, try to keep moving in the last direction
                    else
                    {
                        Direction = LastMovingDirection;
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

    public void SetSpeed(float speed)
    {
        Speed = speed;
    }

    public void SetDirection(NodeController.Directions newDirection)
    {
        Direction = newDirection;
    }
}
