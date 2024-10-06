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

    private bool _canMove = false;


    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        animator = GetComponentInChildren<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();

        movementController = GetComponent<MovementController>();
        movementController.OnDirectionChanged += MovementController_OnDirectionChanged;
        
    }

    public void StartGame()
    {
        animator.SetInteger("direction", 1);
        _canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (_canMove)
        {
            animator.SetBool("moving", true);
            Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

            //Use the Max Value if two pressed.
            float X = Mathf.Abs(inputVector.x);
            float Y = Mathf.Abs(inputVector.y);
            if (X > Y)
            {
                if (inputVector.x > 0)
                {
                    movementController.SetDirection("right");
                }
                if (inputVector.x < 0)
                {
                    movementController.SetDirection("left");
                }
            }
            else if (Y > X)
            {
                if (inputVector.y > 0)
                {
                    movementController.SetDirection("up");
                }
                if (inputVector.y < 0)
                {
                    movementController.SetDirection("down");
                }
            }
        }


    }
    private void MovementController_OnDirectionChanged(object sender, EventArgs e)
    {
        //Debug.Log($"Direction Changed: {movementController.Direction}");
        //bool flipX = false;
        //bool flipY = false;
        if (movementController.Direction == "left")
        {
            animator.SetInteger("direction", 1);
        }
        else if (movementController.Direction == "right")
        {
            animator.SetInteger("direction", 2);
            //flipX = true;
        }
        else if (movementController.Direction == "up")
        {
            animator.SetInteger("direction", 0);
        }
        else if (movementController.Direction == "down")
        {
            animator.SetInteger("direction", 3);
            //flipY = true;
        }

        //sprite.flipX = flipX;
        //sprite.flipY = flipY;
    }


    private void LateUpdate()
    {
        //bool flipX = false;
        //bool flipY = false;
        //if (movementController.LastMovingDirection == "left")
        //{
        //    animator.SetInteger("direction", 0);
        //}
        //else if (movementController.LastMovingDirection == "right")
        //{
        //    animator.SetInteger("direction", 0);
        //    flipX = true;
        //}
        //else if (movementController.LastMovingDirection == "up")
        //{
        //    animator.SetInteger("direction", 1);
        //}
        //else if (movementController.LastMovingDirection == "down")
        //{
        //    animator.SetInteger("direction", 1);
        //    flipY = true;
        //}

        //sprite.flipX = flipX;
        //sprite.flipY = flipY;
    }
}
