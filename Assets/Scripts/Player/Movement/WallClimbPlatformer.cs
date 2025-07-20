using System;
using Framework.Animation;
using UnityEngine;

using Framework.Extensions;
using Gameplay;

namespace Player.Movement
{
    public class WallClimbPlatformer : MonoBehaviour
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
        
        private const float RANDOM_ANGLE = 0.7071068f;
        
        [SerializeField] private UniversalGroundChecker groundChecker;
        [SerializeField] private UniversalGroundChecker leftWallChecker;
        [SerializeField] private UniversalGroundChecker rightWallChecker;
        [SerializeField] private UniversalGroundChecker leftGroundChecker;
        [SerializeField] private UniversalGroundChecker rightGroundChecker;
        [SerializeField] private SpriteChanger headSpriteChanger;
        [SerializeField] private AnimationController legsAnimationController;
        
        [SerializeField] private Rigidbody2D rigidbody2d;
        [SerializeField] private float speed = 3;
        [SerializeField] private float jumpForce = 5;
        [SerializeField] private float gravityScale = 1;
        [SerializeField] private Vector2 ledgePlacement = Vector2.one;

        [SerializeField] private MoveState moveState;
        [SerializeField] private Ledge ledgeState;
        
        private Vector2 _input;
        private Vector2 _currentDirection = Vector2.down;

        private void Awake() => rigidbody2d.gravityScale = gravityScale;

        private void Update()
        {
            UpdateMoveState();
            CheckLedge();
            UpdateAnimations();
        }

        public void SetInput(Vector2 input)
        {
            _input = input;
        }

        public void Jump()
        {
            if (moveState == MoveState.HORIZONTAL && groundChecker.IsCoyoteGrounded)
            {
                rigidbody2d.linearVelocityY = jumpForce;
                moveState = MoveState.HORIZONTAL_JUMP;
                groundChecker.enabled = false;
                Invoke(nameof(TurnOnGroundChecker), 0.2f);
                legsAnimationController.PlayAnimation("Jump");
                return;
            }

            if (moveState == MoveState.VERTICAL)
            {
                moveState = MoveState.VERTICAL_JUMP;
                rigidbody2d.gravityScale = gravityScale;
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
                legsAnimationController.PlayAnimation("Jump");
            }
        }

        public void WallAction()
        {
            if (ledgeState != Ledge.NONE
                && moveState == MoveState.VERTICAL)
                PlaceOverLedge();
            else
                SwitchWall();
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

            rigidbody2d.gravityScale = moveState is MoveState.HORIZONTAL or MoveState.HORIZONTAL_JUMP ? gravityScale : 0;
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
            //groundChecker.SetDirection(direction);
            
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
                    if (!leftWallChecker.IsGrounded
                        && !rightWallChecker.IsGrounded)
                    {
                        ApplyWallTransition(
                            0f,
                            Vector3.down,
                            new Vector3(-0.5f, 0),
                            new Vector3(0.5f, 0)
                        );

                        moveState = MoveState.HORIZONTAL;
                        rigidbody2d.gravityScale = gravityScale;
                    }

                    rigidbody2d.linearVelocityY = _input.y * speed;
                    break;
                
                case MoveState.HORIZONTAL_JUMP:
                    if (groundChecker.IsGrounded)
                        moveState = MoveState.HORIZONTAL;
                
                    ledgeState = Ledge.NONE;
                    break;
                
                case MoveState.VERTICAL_JUMP:
                    Debug.Log("wall jump");
                    
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
            
            if (!leftGroundChecker.IsGrounded
                && Mathf.Approximately(transform.rotation.z, -RANDOM_ANGLE))
                ledgeState = Ledge.LEFT;
            else if (!rightGroundChecker.IsGrounded
                     && Mathf.Approximately(transform.rotation.z, RANDOM_ANGLE))
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
                    transform.position += (Vector3) ledgePlacement.InvertX();
                    break;
                
                case Ledge.RIGHT:
                    transform.position += (Vector3) ledgePlacement;
                    break;
            }
            
            ApplyWallTransition(
                0f,
                Vector3.down,
                new Vector3(-0.5f, 0),
                new Vector3(0.5f, 0)
            );

            moveState = MoveState.HORIZONTAL;
            rigidbody2d.gravityScale = gravityScale;
        }
        
        private void TurnOnGroundChecker()
        {
            groundChecker.enabled = true;
            leftWallChecker.enabled = true;
            rightWallChecker.enabled = true;
        }

        private void UpdateAnimations()
        {
            if (moveState is MoveState.HORIZONTAL_JUMP or MoveState.VERTICAL_JUMP)
                legsAnimationController.PlayAnimation("Jump");
            else if (_input != Vector2.zero)
                legsAnimationController.PlayAnimation("Moving");
            
            if (moveState == MoveState.HORIZONTAL)
            {
                if (_input.x == 0)
                    headSpriteChanger.ChangeSprite(0); // idle
                else if (_input.x > 0)
                    headSpriteChanger.ChangeSprite(2); // right
                else if (_input.x < 0)
                    headSpriteChanger.ChangeSprite(1); // left
            }
            else if (moveState == MoveState.VERTICAL)
            {
                if (_currentDirection.x == 1)
                {
                    if (_input.y == 0)
                        headSpriteChanger.ChangeSprite(0); // idle
                    else if (_input.y > 0)
                        headSpriteChanger.ChangeSprite(2); // right
                    else if (_input.y < 0)
                        headSpriteChanger.ChangeSprite(1); // left
                }
                else if (_currentDirection.x == -1)
                {
                    if (_input.y == 0)
                        headSpriteChanger.ChangeSprite(0); // idle
                    else if (_input.y > 0)
                        headSpriteChanger.ChangeSprite(1); // right
                    else if (_input.y < 0)
                        headSpriteChanger.ChangeSprite(2); // left
                }
            }
        }
    }
}
