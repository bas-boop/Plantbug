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
        private Vector2 _input;
        private Vector2 _currentDirection = Vector2.down;

        private void Update()
        {
            switch (moveState)
            {
                case MoveState.HORIZONTAL:
                    rigidbody2d.linearVelocityX = _input.x * speed;
                    break;
                case MoveState.VERTICAL:
                    rigidbody2d.linearVelocityY = _input.y * speed;
                    break;
                case MoveState.HORIZONTAL_JUMP:
                    if (groundChecker.IsGrounded)
                        moveState = MoveState.HORIZONTAL;
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

        public void SwitchWall(bool skipHorizontal = false)
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

        private void TurnOnGroundChecker()
        {
            groundChecker.enabled = true;
            leftWallChecker.enabled = true;
            rightWallChecker.enabled = true;
        }
    }
}
