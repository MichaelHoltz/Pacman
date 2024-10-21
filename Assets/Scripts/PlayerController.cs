using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private MovementController movementController;
    private PlayerInputActions playerInputActions;

    public SpriteRenderer sprite;
    public Animator animator;

    
    [SerializeField] private GameObject _startNode;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        

        animator = GetComponentInChildren<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();

        movementController = GetComponent<MovementController>();
        movementController.OnDirectionChanged += MovementController_OnDirectionChanged;
        
    }
    //Setup needs to be called early in the game
    public void Setup()
    {
        movementController.CurrentNode = _startNode;
        transform.position = _startNode.transform.position;
        movementController.Direction = NodeController.Directions.None;
        movementController.LastMovingDirection = NodeController.Directions.None; 

        animator.SetInteger("direction", 0);
        animator.SetBool("moving", false);
        animator.SetBool("dead", false);
        animator.speed = 0f;
        //playerInputActions.Player.Disable();


    }

    public void StartGame()
    {
        //Direction Left
        movementController.Direction = NodeController.Directions.Left;
        movementController.LastMovingDirection = NodeController.Directions.Left;
        animator.SetInteger("direction", 1);
        animator.SetBool("moving", true);
        animator.speed = 1f;
        playerInputActions.Player.Enable();
    }
    public void StopGame()
    { 
        movementController.StopGame();
        //movementController.Direction = NodeController.Directions.None;
        //movementController.LastMovingDirection = NodeController.Directions.None;
        //animator.SetInteger("direction", 0);
        //animator.SetBool("moving", false);
        animator.speed = 0f;

    }
    // Update is called once per frame
    void Update()
    {
            
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        //Use the Max Value if two pressed.
        float X = Mathf.Abs(inputVector.x);
        float Y = Mathf.Abs(inputVector.y);
        if (X > Y)
        {
            if (inputVector.x > 0)
            {
                movementController.SetDirection(NodeController.Directions.Right);
            }
            if (inputVector.x < 0)
            {
                movementController.SetDirection(NodeController.Directions.Left);
            }
        }
        else if (Y > X)
        {
            if (inputVector.y > 0)
            {
                movementController.SetDirection(NodeController.Directions.Up);
            }
            if (inputVector.y < 0)
            {
                movementController.SetDirection(NodeController.Directions.Down);
            }
        }

    }
    private void MovementController_OnDirectionChanged(object sender, EventArgs e)
    {
        if (movementController.Direction == NodeController.Directions.Left)
        {
            animator.SetInteger("direction", 1);
        }
        else if (movementController.Direction == NodeController.Directions.Right)
        {
            animator.SetInteger("direction", 2);
        }
        else if (movementController.Direction == NodeController.Directions.Up)
        {
            animator.SetInteger("direction", 0);
        }
        else if (movementController.Direction == NodeController.Directions.Down)
        {
            animator.SetInteger("direction", 3);
        }

    }

    public void Death()
    { 
        animator.SetBool("moving", false);
        animator.SetBool("dead", true);
        animator.speed = 1f;
    }
}
