using System;
using UnityEngine;

using Gameplay;

namespace Player.Movement
{
    public class Walk : MonoBehaviour
    {
        private enum MoveState
        {
            HORIZONTAL,
            VERTICAL,
            HORIZONTAL_JUMP,
            VERTICAL_JUMP
        }
        
        private enum Ledge
        {
            NONE,
            LEFT,
            RIGHT
        }
        
        [SerializeField] private UniversalGroundChecker groundChecker;
        [SerializeField] private UniversalGroundChecker leftWallChecker;
        [SerializeField] private UniversalGroundChecker rightWallChecker;
        [SerializeField] private UniversalGroundChecker leftGroundChecker;
        [SerializeField] private UniversalGroundChecker rightGroundChecker;
        [SerializeField] private Rigidbody2D rigidbody2d;
        [SerializeField] private float speed = 3;
        [SerializeField] private float delecerationForce = 2;
        [SerializeField] private float jumpForce = 5;

        [SerializeField] private bool _isMovingHorizotal = true;
        [SerializeField] private MoveState moveState;
        [SerializeField] private Ledge ledgeState;
        
        private Vector2 _input;
        private Vector2 _currentDirection = Vector2.down;

        private void Update()
        {
            UpdateMoveState();
            CheckLedge();
        }

        public void SetInput(Vector2 input)
        {
            _input = input;
        }

        public void Jump()
        {
            Debug.Log("jump");
            
            if (moveState == MoveState.HORIZONTAL && groundChecker.IsGrounded)
            {
                rigidbody2d.linearVelocityY = jumpForce;
                moveState = MoveState.HORIZONTAL_JUMP;
                groundChecker.enabled = false;
                Invoke(nameof(TurnOnGroundChecker), 0.2f);
                return;
            }

            Debug.Log(-_currentDirection.x * jumpForce);
            moveState = MoveState.VERTICAL_JUMP;
            rigidbody2d.gravityScale = _isMovingHorizotal ? 1 : 0;
            rigidbody2d.linearVelocityX = -_currentDirection.x * jumpForce;
            
            ApplyWallTransition(
                0f,
                Vector3.down,
                new Vector3(-0.5f, 0),
                new Vector3(0.5f, 0)
            );
            
            leftWallChecker.enabled = false;
            rightWallChecker.enabled = false;
            Invoke(nameof(TurnOnGroundChecker), 0.2f);
        }

        public void WallAction()
        {
            if (ledgeState == Ledge.NONE)
                SwitchWall();
            else
                PlaceOverLedge();
        }
        
        private void SwitchWall(bool skipHorizontal = false)
        {
            if (moveState == MoveState.VERTICAL_JUMP)
            {
                if (leftWallChecker.IsGrounded)
                {
                    ApplyWallTransition(
                        -90f,
                        Vector3.left,
                        new Vector3(0, 0.5f),
                        new Vector3(0, -0.5f)
                    );

                    moveState = MoveState.VERTICAL;
                }
                else if (rightWallChecker.IsGrounded)
                {
                    ApplyWallTransition(
                        90f,
                        Vector3.right,
                        new Vector3(0, -0.5f),
                        new Vector3(0, 0.5f)
                    );
                
                    moveState = MoveState.VERTICAL;
                }
            }
            else if (!skipHorizontal &&
                moveState != MoveState.HORIZONTAL)
            {
                ApplyWallTransition(
                    0f,
                    Vector3.down,
                    new Vector3(-0.5f, 0),
                    new Vector3(0.5f, 0)
                );

                moveState = MoveState.HORIZONTAL;
            }
            else if (leftWallChecker.IsGrounded)
            {
                ApplyWallTransition(
                    -90f,
                    Vector3.left,
                    new Vector3(0, 0.5f),
                    new Vector3(0, -0.5f)
                );

                moveState = MoveState.VERTICAL;
            }
            else if (rightWallChecker.IsGrounded)
            {
                ApplyWallTransition(
                    90f,
                    Vector3.right,
                    new Vector3(0, -0.5f),
                    new Vector3(0, 0.5f)
                );
                
                moveState = MoveState.VERTICAL;
            }

            rigidbody2d.gravityScale = moveState is MoveState.HORIZONTAL or MoveState.HORIZONTAL_JUMP ? 1 : 0;
        }
        
        private void ApplyWallTransition(
            float rotationZ, 
            Vector3 direction, 
            Vector3 leftOffset, 
            Vector3 rightOffset)
        {
            transform.rotation = Quaternion.Euler(0, 0, rotationZ);
            
            leftGroundChecker.SetDirection(direction);
            leftGroundChecker.SetOffset(leftOffset);
            rightGroundChecker.SetDirection(direction);
            rightGroundChecker.SetOffset(rightOffset);
            
            _currentDirection = direction;
        }

        private void UpdateMoveState()
        {
            switch (moveState)
            {
                case MoveState.HORIZONTAL:
                    rigidbody2d.linearVelocityX = _input.x * speed;
                    ledgeState = Ledge.NONE;
                    break;
                
                case MoveState.VERTICAL:
                    rigidbody2d.linearVelocityY = _input.y * speed;
                    break;
                
                case MoveState.HORIZONTAL_JUMP:
                    if (groundChecker.IsGrounded)
                        moveState = MoveState.HORIZONTAL;
                
                    ledgeState = Ledge.NONE;
                    break;
                
                case MoveState.VERTICAL_JUMP:
                    if (leftWallChecker.IsGrounded
                        && _currentDirection.x == 1)
                    {
                        SwitchWall(true);
                        _currentDirection.x = -_currentDirection.x;
                    }
                    
                    if (rightWallChecker.IsGrounded
                        && _currentDirection.x == -1)
                    {
                        SwitchWall(true);
                        _currentDirection.x = -_currentDirection.x;
                    }
                    
                    if (groundChecker.IsGrounded)
                        moveState = MoveState.HORIZONTAL;
                    break;
            }
        }

        private void CheckLedge()
        {
            if (moveState != MoveState.VERTICAL)
                return;
            
            if (!leftGroundChecker.IsGrounded)
                ledgeState = Ledge.LEFT;
            else if (!rightGroundChecker.IsGrounded)
                ledgeState = Ledge.RIGHT;
            else
                ledgeState = Ledge.NONE;
        }

        private void PlaceOverLedge()
        {
            if (moveState != MoveState.VERTICAL)
                return;
            
            switch (ledgeState)
            {
                case Ledge.LEFT:
                    transform.position += new Vector3(-1, 1, 0);
                    break;
                
                case Ledge.RIGHT:
                    transform.position += new Vector3(1, 1, 0);
                    break;
            }
            
            ApplyWallTransition(
                0f,
                Vector3.down,
                new Vector3(-0.5f, 0),
                new Vector3(0.5f, 0)
            );

            moveState = MoveState.HORIZONTAL;
            rigidbody2d.gravityScale = 1;
        }
        
        private void TurnOnGroundChecker()
        {
            groundChecker.enabled = true;
            leftWallChecker.enabled = true;
            rightWallChecker.enabled = true;
        }
    }
}
