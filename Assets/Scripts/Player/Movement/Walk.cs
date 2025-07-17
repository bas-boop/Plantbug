using UnityEngine;

using Gameplay;

namespace Player.Movement
{
    public class Walk : MonoBehaviour
    {
        [SerializeField] private UniversalGroundChecker leftChecker;
        [SerializeField] private UniversalGroundChecker rightChecker;
        [SerializeField] private float speed = 3f;
        [SerializeField] private float maxSpeed = 5f;
        [SerializeField] private float acceleration = 50f;
        [SerializeField] private float gravityStrength = 30f;
        [SerializeField] private float friction = 20f;

        private Vector2 _input;
        private Vector2 _velocity;
        private Vector2 _gravity = Vector2.down;
        private bool _isMovingHorizontal = true;

        private void Update()
        {
            ApplyGravity();
            ApplyMovement();
            ApplyFriction();
            Move();
        }

        public void SetInput(Vector2 input)
        {
            _input = input;
        }

        public void SwitchWall()
        {
            if (leftChecker.IsGrounded)
            {
                transform.rotation = Quaternion.Euler(0, 0, -90);
                _isMovingHorizontal = !_isMovingHorizontal;
                _gravity = Vector2.left;
                _velocity = Vector2.zero; // Optional: reset velocity
            }

            if (rightChecker.IsGrounded)
            {
                transform.rotation = Quaternion.Euler(0, 0, 90);
                _isMovingHorizontal = !_isMovingHorizontal;
                _gravity = Vector2.right;
                _velocity = Vector2.zero; // Optional: reset velocity
            }
        }

        private void ApplyGravity()
        {
            _velocity += _gravity * (gravityStrength * Time.deltaTime);
        }

        private void ApplyMovement()
        {
            Vector2 moveDir = _isMovingHorizontal ? Vector2.right : Vector2.up;
            float inputAxis = _isMovingHorizontal ? _input.x : _input.y;

            if (Mathf.Abs(inputAxis) > 0.01f)
            {
                _velocity += moveDir * (inputAxis * acceleration * Time.deltaTime);

                // Clamp speed in move direction
                float dot = Vector2.Dot(_velocity, moveDir);
                if (Mathf.Abs(dot) > maxSpeed)
                {
                    float overflow = dot - Mathf.Sign(dot) * maxSpeed;
                    _velocity -= moveDir * overflow;
                }
            }
        }

        private void ApplyFriction()
        {
            if (_input == Vector2.zero)
            {
                Vector2 moveDir = _isMovingHorizontal ? Vector2.right : Vector2.up;
                Vector2 frictionForce = moveDir * (Vector2.Dot(_velocity, moveDir) * friction * Time.deltaTime);
                _velocity -= frictionForce;
            }
        }

        private void Move()
        {
            transform.position += (Vector3)(_velocity * Time.deltaTime);
        }
    }
}
