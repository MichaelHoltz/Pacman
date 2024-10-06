using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    public enum GhostNodesStatesEnum
    { 
        Respawning,
        LeftNode,
        RightNode,
        CenterNode,
        StartNode,
        MovingInNodes

    }
    public enum GhostType
    { 
        Blinky,
        Pinky,
        Inky,
        Clyde
    }

    public GhostNodesStatesEnum GhostNodesState;
    private GhostNodesStatesEnum _respawnState;
    [SerializeField] private GhostType _ghostType;

    [SerializeField] private GameObject _ghostNodeStart;
    [SerializeField] private GameObject _ghostNodeCenter;
    [SerializeField] private GameObject _ghostNodeLeft;
    [SerializeField] private GameObject _ghostNodeRight;
    [SerializeField] private MovementController _movementController;
    [SerializeField] private GameObject _startingNode;

    [SerializeField] private GameManager _gameManager;
    [SerializeField] private bool _readyToLeaveHome = false;
    [SerializeField] private bool _testRespawn = false;
    public bool IsFrightened = false;

    public GameObject[] scatterNodes;
    public int ScatterNodeIndex;

    private void Awake()
    {
        ScatterNodeIndex = 0;
        _gameManager.OnGameStart += GameManager_OnGameStart;
        switch (_ghostType)
        {
            case GhostType.Blinky:
                GhostNodesState = GhostNodesStatesEnum.StartNode;
                _respawnState = GhostNodesStatesEnum.CenterNode;
                _startingNode = _ghostNodeStart;
                _readyToLeaveHome = true;
                //Blinky Starts in the Center when Respawning, but in _startNode at the beginning of the game so this seems wrong
                break;
            case GhostType.Pinky:
                GhostNodesState = GhostNodesStatesEnum.CenterNode;
                _respawnState = GhostNodesStatesEnum.CenterNode;
                _startingNode = _ghostNodeCenter;
                break;
            case GhostType.Inky:
                GhostNodesState = GhostNodesStatesEnum.LeftNode;
                _respawnState = GhostNodesStatesEnum.LeftNode;
                _startingNode = _ghostNodeLeft;
                break;
            case GhostType.Clyde:
                GhostNodesState = GhostNodesStatesEnum.RightNode;
                _respawnState = GhostNodesStatesEnum.RightNode;
                _startingNode = _ghostNodeRight;
                break;
        }
        _movementController.CurrentNode = _startingNode;
        transform.position = _startingNode.transform.position;
    }



    // Start is called before the first frame update
    void Start()
    {


    }
    private void GameManager_OnGameStart(object sender, EventArgs e)
    {
        //tell the Enemy Controller that movement can start
        _movementController.StartGame();

    }
    // Update is called once per frame
    private void Update()
    {
        if(_testRespawn)
        {
            _readyToLeaveHome = false;
            GhostNodesState = GhostNodesStatesEnum.Respawning;
            _testRespawn = false;
        }

        if(_movementController.CurrentNode.GetComponent<NodeController>().IsSideNode)
        {
            _movementController.SetSpeed(1);
        }
        else
        {
            _movementController.SetSpeed(2);
        }
    }

    public void ReachedCenterOfNode(NodeController nodeController)
    {
        NodeController.Directions direction = NodeController.Directions.None;
        //Debug.Log("Reached Center of Node");
        switch (GhostNodesState)
        {

            case GhostNodesStatesEnum.StartNode:
                if (_readyToLeaveHome)
                {
                    GhostNodesState = GhostNodesStatesEnum.MovingInNodes;
                    _movementController.SetDirection(NodeController.Directions.Left);
                }
                break;
            case GhostNodesStatesEnum.LeftNode:
                if (_readyToLeaveHome)
                {
                    GhostNodesState = GhostNodesStatesEnum.CenterNode;
                    _movementController.SetDirection(NodeController.Directions.Right);
                }
                break;
            case GhostNodesStatesEnum.RightNode:
                if (_readyToLeaveHome)
                {
                    GhostNodesState = GhostNodesStatesEnum.CenterNode;
                    _movementController.SetDirection(NodeController.Directions.Left);
                }
                break;
            case GhostNodesStatesEnum.CenterNode:
                if (_readyToLeaveHome)
                {
                    GhostNodesState = GhostNodesStatesEnum.StartNode;
                    _movementController.SetDirection(NodeController.Directions.Up);
                }

                break;
            case GhostNodesStatesEnum.Respawning:
                
                //we have reached our start node, move to the center node
                if (transform.position.x == _ghostNodeStart.transform.position.x && transform.position.y == _ghostNodeStart.transform.position.y)
                {
                    direction = NodeController.Directions.Down;
                }
                //we have reached the center node, either finish respawn, or move to the left/right node
                else if (transform.position.x == _ghostNodeCenter.transform.position.x && transform.position.y == _ghostNodeCenter.transform.position.y)
                {
                    if (_respawnState == GhostNodesStatesEnum.CenterNode)
                    {
                        GhostNodesState = _respawnState;
                    }
                    else if (_respawnState == GhostNodesStatesEnum.LeftNode)
                    {
                        direction = NodeController.Directions.Left;
                    }
                    else if (_respawnState == GhostNodesStatesEnum.RightNode)
                    {
                        direction = NodeController.Directions.Right;
                    }
                }
                else if (
                    (transform.position.x == _ghostNodeLeft.transform.position.x && transform.position.y == _ghostNodeLeft.transform.position.y)
                    || (transform.position.x == _ghostNodeRight.transform.position.x && transform.position.y == _ghostNodeRight.transform.position.y)
                    )
                {
                    GhostNodesState = _respawnState;

                }
                else 
                {
                    //Determine quickest direction to home
                    direction = GetClosestDirection(_ghostNodeStart.transform.position);
                }

                _movementController.SetDirection(direction);
                break;
            case GhostNodesStatesEnum.MovingInNodes:

                //Scatter Mode
                if (_gameManager.CurrentGhostMode == GameManager.GhostMode.Scatter)
                {
                    //if we reached the scatter node, move to the next scatter node
                    if (transform.position.x == scatterNodes[ScatterNodeIndex].transform.position.x && transform.position.y == scatterNodes[ScatterNodeIndex].transform.position.y)
                    {
                        ScatterNodeIndex++;
                        if (ScatterNodeIndex == scatterNodes.Length - 1)
                        {
                            ScatterNodeIndex = 0;
                        }
                        
                    }
                    direction = GetClosestDirection(scatterNodes[ScatterNodeIndex].transform.position);
                    _movementController.SetDirection(direction);
                }
                //Frightened Mode
                else if (IsFrightened)
                { 
                
                }
                //Chase Mode
                else
                {
                    if (_ghostType == GhostType.Blinky)
                    {
                        DetermineBlinkyDirection();
                    }
                    else if (_ghostType == GhostType.Pinky)
                    {
                        DeterminePinkyDirection();
                    }
                    else if (_ghostType == GhostType.Inky)
                    {
                        DeterminInkyDirection();
                    }
                    else if (_ghostType == GhostType.Clyde)
                    {
                        DetermineClydeDirection();
                    }
                }

                break;
        }
    }

    private void DetermineBlinkyDirection()
    {
        NodeController.Directions direction = GetClosestDirection(_gameManager.Pacman.transform.position);
        _movementController.SetDirection(direction);
    }
    private void DeterminePinkyDirection()
    { 
    }
    private void DeterminInkyDirection()
    { 
    }
    private void DetermineClydeDirection()
    {
    }
    private NodeController.Directions GetClosestDirection(Vector2 target)
    {
        float shortestDistance = 0;
        NodeController.Directions lastMovingDirection = _movementController.LastMovingDirection;
        NodeController nodeController = _movementController.CurrentNode.GetComponent<NodeController>();
        NodeController.Directions newDirection = NodeController.Directions.None;
        //if we can move up and not reversing
        if (nodeController.CanMoveUp && lastMovingDirection != NodeController.Directions.Down)
        { 
            //Get the node above us
            GameObject node = nodeController.NodeUp;

            //Get the distance between our top node and pacman
            float distance = Vector2.Distance(node.transform.position, target);

            //if this is the shortest distance so far, set our direction
            if(distance < shortestDistance || shortestDistance == 0)
            {
                shortestDistance = distance;
                newDirection = NodeController.Directions.Up;
            }
        }
        //if we can move down and not reversing
        if (nodeController.CanMoveDown && lastMovingDirection != NodeController.Directions.Up)
        {
            //Get the node  below us
            GameObject node = nodeController.NodeDown;

            //Get the distance between our top node and pacman
            float distance = Vector2.Distance(node.transform.position, target);

            //if this is the shortest distance so far, set our direction
            if (distance < shortestDistance || shortestDistance == 0)
            {
                shortestDistance = distance;
                newDirection = NodeController.Directions.Down;
            }
        }
        //if we can move Left and not reversing
        if (nodeController.CanMoveLeft && lastMovingDirection != NodeController.Directions.Right)
        {
            //Get the node to left
            GameObject node = nodeController.NodeLeft;

            //Get the distance between our top node and pacman
            float distance = Vector2.Distance(node.transform.position, target);

            //if this is the shortest distance so far, set our direction
            if (distance < shortestDistance || shortestDistance == 0)
            {
                shortestDistance = distance;
                newDirection = NodeController.Directions.Left;
            }
        }
        //if we can move right and not reversing
        if (nodeController.CanMoveRight && lastMovingDirection != NodeController.Directions.Left)
        {
            //Get the node to Right
            GameObject node = nodeController.NodeRight;

            //Get the distance between our top node and pacman
            float distance = Vector2.Distance(node.transform.position, target);

            //if this is the shortest distance so far, set our direction
            if (distance < shortestDistance || shortestDistance == 0)
            {
                shortestDistance = distance;
                newDirection = NodeController.Directions.Right;
            }
        }

        return newDirection;
    }
}
