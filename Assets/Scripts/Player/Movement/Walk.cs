using UnityEngine;

using Gameplay;

namespace Player.Movement
{
    public class Walk : MonoBehaviour
    {
        [SerializeField] private UniversalGroundChecker leftWallChecker;
        [SerializeField] private UniversalGroundChecker rightWallChecker;
        [SerializeField] private UniversalGroundChecker leftGroundChecker;
        [SerializeField] private UniversalGroundChecker rightGroundChecker;
        [SerializeField] private Rigidbody2D rigidbody2d;
        [SerializeField] private float speed = 3;
        [SerializeField] private float delecerationForce = 2;

        private bool _wasMoving;
        private bool _isMovingHorizotal = true;
        private Vector2 _input;

        private void Update()
        {
            if (_input == Vector2.zero)
            {
                if (_isMovingHorizotal)
                    rigidbody2d.linearVelocityX = 0;
                else
                    rigidbody2d.linearVelocityY = 0;
                
                return;
            }

            if (_isMovingHorizotal)
                rigidbody2d.linearVelocityX = _input.x * speed;
            else
                rigidbody2d.linearVelocityY = _input.y * speed;

            _wasMoving = true;
        }

        public void SetInput(Vector2 input)
        {
            _input = input;
        }

        public void SwitchWall()
        {
            if (!_isMovingHorizotal)
            {
                ApplyWallTransition(
                    0f,
                    Vector3.down,
                    new Vector3(-0.5f, 0),
                    new Vector3(0.5f, 0)
                );
                
                _isMovingHorizotal = true;
            }
            else if (leftWallChecker.IsGrounded)
            {
                ApplyWallTransition(
                    -90f,
                    Vector3.left,
                    new Vector3(0, 0.5f),
                    new Vector3(0, -0.5f)
                );
            }
            else if (rightWallChecker.IsGrounded)
            {
                ApplyWallTransition(
                    90f,
                    Vector3.right,
                    new Vector3(0, -0.5f),
                    new Vector3(0, 0.5f)
                );
            }

            rigidbody2d.gravityScale = _isMovingHorizotal ? 1 : 0;
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
            _isMovingHorizotal = !_isMovingHorizotal;
        }
    }
}
