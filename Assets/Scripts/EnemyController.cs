using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
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
    public GhostNodesStatesEnum StartGhostNodesState;
    private GhostNodesStatesEnum _respawnState;
    [SerializeField] private GhostType _ghostType;


    [SerializeField] private GameObject _ghostNodeStart; //Blinky
    [SerializeField] private GameObject _ghostNodeCenter; //Pinky
    [SerializeField] private GameObject _ghostNodeLeft; //Inky
    [SerializeField] private GameObject _ghostNodeRight; //Clyde
    [SerializeField] private MovementController _movementController;
    [SerializeField] private GameObject _startingNode;

    [SerializeField] private GameManager _gameManager;
    public bool ReadyToLeaveHome = false;
    [SerializeField] private bool _testRespawn = false;
    public bool IsFrightened = false;
    private bool _allowReverseDirection = false;

    public GameObject[] scatterNodes;
    public int ScatterNodeIndex;
    private const float DISTANCE_BETWEEN_NODES = 0.35f;
    public bool LeftHomeBefore = false;
    public bool isVisible = true;

    public SpriteRenderer ghostSprite;
    public SpriteRenderer eyesSprite;
    public Animator animator;
    public Color color;

    private void Awake()
    {
        ghostSprite = GetComponent<SpriteRenderer>();
        //eyesSprite = GetComponentInChildren<SpriteRenderer>();  
        animator = GetComponentInChildren<Animator>();
        ScatterNodeIndex = 0;
        _gameManager.OnGameStart += GameManager_OnGameStart;
        switch (_ghostType)
        {
            case GhostType.Blinky:
                StartGhostNodesState = GhostNodesStatesEnum.StartNode;
                _respawnState = GhostNodesStatesEnum.CenterNode;
                _startingNode = _ghostNodeStart;
                ReadyToLeaveHome = true;
                LeftHomeBefore = true;
                //Blinky Starts in the Center when Respawning, but in _startNode at the beginning of the game so this seems wrong
                break;
            case GhostType.Pinky:
                StartGhostNodesState = GhostNodesStatesEnum.CenterNode;
                _respawnState = GhostNodesStatesEnum.CenterNode;
                _startingNode = _ghostNodeCenter;
                break;
            case GhostType.Inky:
                StartGhostNodesState = GhostNodesStatesEnum.LeftNode;
                _respawnState = GhostNodesStatesEnum.LeftNode;
                _startingNode = _ghostNodeLeft;
                break;
            case GhostType.Clyde:
                StartGhostNodesState = GhostNodesStatesEnum.RightNode;
                _respawnState = GhostNodesStatesEnum.RightNode;
                _startingNode = _ghostNodeRight;
                break;
        }
        ghostSprite.color = color;

    }
    public void StopGame()
    {
        _movementController.StopGame();
    }

    public void Setup()
    {
        animator.SetBool("moving", false);


        //Reset the ghosts back to their home position
        GhostNodesState = StartGhostNodesState;
        _movementController.CurrentNode = _startingNode;
        _movementController.LastMovingDirection = NodeController.Directions.None;
        _movementController.Direction = NodeController.Directions.None;
        transform.position = _startingNode.transform.position;
        isVisible = true;
        //set their scatter node index to 0
        ScatterNodeIndex = 0;
        //set isFrightened to false
        IsFrightened = false;
        //set ready to leave home to false if Inky or Clyde
        if (_ghostType == GhostType.Inky || _ghostType == GhostType.Clyde)
        {
            ReadyToLeaveHome = false;
            LeftHomeBefore = false;
        }
        else if (_ghostType == GhostType.Blinky)
        {
            ReadyToLeaveHome = true;
            LeftHomeBefore = true;
        }
        else if (_ghostType == GhostType.Pinky)
        {
            ReadyToLeaveHome = true;
            LeftHomeBefore = false;
        }
        //_movementController.StartGame();


    }

    private void GameManager_OnGameStart(object sender, EventArgs e)
    {
        //tell the Enemy Controller that movement can start
        _movementController.StartGame();

    }
    // Update is called once per frame
    private void Update()
    {
        if (!_gameManager.IsPowerPelletRunning)
        { 
            IsFrightened = false;
        }
        //Show Ghost Sprite
        if (isVisible)
        {
            ghostSprite.enabled = true;
            eyesSprite.enabled = true;
        }
        //Hide Ghost Sprite
        else
        {
            ghostSprite.enabled = false;
            eyesSprite.enabled = false;
        }


        if (IsFrightened)
        {
            animator.SetBool("frightened", true);
            eyesSprite.enabled = false;
            ghostSprite.color = new Color(255, 255, 255, 255);
        }
        else
        {
            animator.SetBool("frightened", false);
            eyesSprite.enabled = true;
            ghostSprite.color = color;
        }

        if (!_gameManager.gameIsRunning)
        {
            return;
        }


        animator.SetBool("moving", true);


        if (_testRespawn)
        {
            ReadyToLeaveHome = false;
            GhostNodesState = GhostNodesStatesEnum.Respawning;
            _testRespawn = false;
        }

        if (_movementController.CurrentNode.GetComponent<NodeController>().IsSideNode)
        {
            _movementController.SetSpeed(1);
        }
        else
        {
            _movementController.SetSpeed(2);
        }
    }

    public void SetFrightened(bool frightened)
    {
        IsFrightened = frightened;
    }

    public void ReachedCenterOfNode(NodeController nodeController)
    {
        NodeController.Directions direction = NodeController.Directions.None;
        //Debug.Log("Reached Center of Node");
        switch (GhostNodesState)
        {
            case GhostNodesStatesEnum.StartNode:
                if (ReadyToLeaveHome)
                {
                    GhostNodesState = GhostNodesStatesEnum.MovingInNodes;
                    _movementController.SetDirection(NodeController.Directions.Left);
                }
                break;
            case GhostNodesStatesEnum.LeftNode:
                if (ReadyToLeaveHome)
                {
                    GhostNodesState = GhostNodesStatesEnum.CenterNode;
                    _movementController.SetDirection(NodeController.Directions.Right);
                }
                break;
            case GhostNodesStatesEnum.RightNode:
                if (ReadyToLeaveHome)
                {
                    GhostNodesState = GhostNodesStatesEnum.CenterNode;
                    _movementController.SetDirection(NodeController.Directions.Left);
                }
                break;
            case GhostNodesStatesEnum.CenterNode:
                if (ReadyToLeaveHome)
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
                LeftHomeBefore = true;
                //Scatter Mode
                if (_gameManager.CurrentGhostMode == GameManager.GhostMode.Scatter)
                {
                    determineGhostScatterModeDirection();

                }
                //Frightened Mode
                else if (IsFrightened)
                {
                    // _allowReverseDirection
                    _movementController.SetDirection(getRandomDirection());
                }
                //Chase Mode
                else
                {
                    if (_ghostType == GhostType.Blinky)
                    {
                        determineBlinkyDirection();
                    }
                    else if (_ghostType == GhostType.Pinky)
                    {
                        determinePinkyDirection();
                    }
                    else if (_ghostType == GhostType.Inky)
                    {
                        determinInkyDirection();
                    }
                    else if (_ghostType == GhostType.Clyde)
                    {
                        determineClydeDirection();
                    }
                }

                break;
        }
    }
    private NodeController.Directions getRandomDirection()
    {
        List<NodeController.Directions> possibleDirections = new List<NodeController.Directions>();
        NodeController nodeController = _movementController.CurrentNode.GetComponent<NodeController>();

        if (nodeController.CanMoveUp && _movementController.Direction != NodeController.Directions.Down)
        {
            possibleDirections.Add(NodeController.Directions.Up);
        }
        if (nodeController.CanMoveDown && _movementController.Direction != NodeController.Directions.Up)
        {
            possibleDirections.Add(NodeController.Directions.Down);
        }
        if (nodeController.CanMoveLeft && _movementController.Direction != NodeController.Directions.Right)
        {
            possibleDirections.Add(NodeController.Directions.Left);
        }
        if (nodeController.CanMoveRight && _movementController.Direction != NodeController.Directions.Left)
        {
            possibleDirections.Add(NodeController.Directions.Right);
        }
        if (possibleDirections.Count > 0)
        {
            return possibleDirections[UnityEngine.Random.Range(0, possibleDirections.Count)];
        }
        else
        {
            return _movementController.Direction;
        }


    }
    private void determineGhostScatterModeDirection()
    {
        //if we reached the scatter node, move to the next scatter node
        if (transform.position.x == scatterNodes[ScatterNodeIndex].transform.position.x && transform.position.y == scatterNodes[ScatterNodeIndex].transform.position.y)
        {
            ScatterNodeIndex++;
            if (ScatterNodeIndex == scatterNodes.Length - 1)
            {
                ScatterNodeIndex = 0;
            }
            //Debug.Log($"ScatterNodeIndex: {ScatterNodeIndex}");
        }

        NodeController.Directions direction = GetClosestDirection(scatterNodes[ScatterNodeIndex].transform.position);
        _movementController.SetDirection(direction);

    }
    private void determineBlinkyDirection()
    {
        NodeController.Directions direction = GetClosestDirection(_gameManager.Pacman.transform.position);
        _movementController.SetDirection(direction);
    }
    private void determinePinkyDirection()
    {
        NodeController.Directions pacmansDirection = _gameManager.Pacman.GetComponent<MovementController>().LastMovingDirection;

        Vector2 target = _gameManager.Pacman.transform.position;

        if (pacmansDirection == NodeController.Directions.Left)
        {
            target.x -= DISTANCE_BETWEEN_NODES * 2;
        }
        else if (pacmansDirection == NodeController.Directions.Right)
        {
            target.x += DISTANCE_BETWEEN_NODES * 2;
        }
        else if (pacmansDirection == NodeController.Directions.Up)
        {
            target.y += DISTANCE_BETWEEN_NODES * 2;
        }
        else if (pacmansDirection == NodeController.Directions.Down)
        {
            target.y -= DISTANCE_BETWEEN_NODES * 2;
        }
        NodeController.Directions direction = GetClosestDirection(target);
        _movementController.SetDirection(direction);
    }
    private void determinInkyDirection()
    {
        NodeController.Directions pacmansDirection = _gameManager.Pacman.GetComponent<MovementController>().LastMovingDirection;

        Vector2 target = _gameManager.Pacman.transform.position;

        if (pacmansDirection == NodeController.Directions.Left)
        {
            target.x -= DISTANCE_BETWEEN_NODES * 2;
        }
        else if (pacmansDirection == NodeController.Directions.Right)
        {
            target.x += DISTANCE_BETWEEN_NODES * 2;
        }
        else if (pacmansDirection == NodeController.Directions.Up)
        {
            target.y += DISTANCE_BETWEEN_NODES * 2;
        }
        else if (pacmansDirection == NodeController.Directions.Down)
        {
            target.y -= DISTANCE_BETWEEN_NODES * 2;
        }

        float xDistance = target.x - _gameManager.Blinky.transform.position.x;
        float yDistance = target.y - _gameManager.Blinky.transform.position.y;
        Vector2 inkyTarget = new Vector2(target.x + xDistance, target.y + yDistance);

        NodeController.Directions direction = GetClosestDirection(inkyTarget);
        _movementController.SetDirection(direction);
    }
    private void determineClydeDirection()
    {
        float distance = Math.Abs(Vector2.Distance(_gameManager.Pacman.transform.position, transform.position));
        //if Clyde is within 8 nodes of pacman, chase him using Blinky's Logic
        if (distance <= DISTANCE_BETWEEN_NODES * 8)
        {
            determineBlinkyDirection();
        }
        else
        {
            //Scatter mode.
            determineGhostScatterModeDirection();
        }
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
            if (distance < shortestDistance || shortestDistance == 0)
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
    public void SetVisible(bool newIsVisible)
    { 
        isVisible = newIsVisible;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //If player
        if (collision.gameObject.layer == 7 )
        {
            //Get Eaten
            if (IsFrightened)
            {

            }
            //Eat Player
            else
            { 
                StartCoroutine(_gameManager.PlayerEaten());
            }

        }
    }
}
